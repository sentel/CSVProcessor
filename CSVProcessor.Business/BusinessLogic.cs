using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CSVProcessor.Business
{
    public class BusinessLogic : IBusinessLogic
    {
        public BusinessLogic(ILogger logger,
                             IScriptProcessor scriptProcessor,
                             IFileProcessor fileProcessor,
                             IEmailProcessor emailProcessor,
                             IDatabaseLogic databaseLogic)
        {
            this.logger = logger;
            this.scriptProcessor = scriptProcessor;
            this.fileProcessor = fileProcessor;
            this.emailProcessor = emailProcessor;
            this.databaseLogic = databaseLogic;
            curlDetails = new CurlDetails();
        }

        public ILogger GetBranding(string clientName)
        {
            Index++;

            BrandingDictionary = databaseLogic.GetBranding(clientName);
            foreach (var kv in BrandingDictionary)
            {
                logger.Success += $"{Index}:\t{kv.Key} - {kv.Value}\n";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger ConnectToFs01()
        {
            Index++;

            var isConnected = scriptProcessor.ConnectToFs01(curlDetails);
            if (isConnected)
            {
                logger.Success = $"{Index}:\tConnection to FS01 created";
            }
            else
            {
                logger.Error = $"{Index}:\tConnection to FS01 no created";
            }
            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger GetSessionId()
        {
            Index++;
            var isLong = false;
            var headers = scriptProcessor.GetSessionId(curlDetails);
            var kv = headers.FirstOrDefault(it => it.Key == "Set-Cookie");
            if (!string.IsNullOrEmpty(kv.Key))
            {
                var value = kv.Value.LastOrDefault();
                var sessionValue = value.GetBetween("SESSIONID=", "; Path");
                isLong = long.TryParse(sessionValue, out var sessionId);
                curlDetails.SessionId = sessionId;
            }

            if (isLong)
            {
                logger.Success = $"{Index}:\tSession id retrieved.";
            }
            else
            {
                logger.Error = $"{Index}:\tSession id not retrieved.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger GetDomains()
        {
            Index++;
            var reader = scriptProcessor.GetDomains(curlDetails);
            var result = string.Empty;
            while (!reader.EndOfStream)
            {
                result = reader.ReadToEnd();
            }
            if (string.IsNullOrEmpty(result) || result.ContainsHtmlTag())
            {
                logger.Error = $"{Index}:\t{Domains.Count} domains found.";
            }
            else
            {
                Domains = result.Deserialize();
                logger.Success = $"{Index}:\t{Domains.Count} domains retrieved.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger CleanUp()
        {
            Index++;
            var topLevel = Domains.Where(IsTopLevel).Select(domain => domain.DomainName).FirstOrDefault();
            var isCleaned = fileProcessor.Clean(Constants.ROOT_PATH, topLevel);
            if (isCleaned)
            {
                logger.Success = $"{Index}:\tDirectory has been cleaned.";
            }
            else
            {
                logger.Error = $"{Index}\t:Directory has not been cleaned.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger CreateDirectoryStructure()
        {
            Index++;
            var topLevel = Domains.Where(IsTopLevel).Select(domain => domain.DomainName).FirstOrDefault();
            DirectoryInfo = fileProcessor.CreateDirectory(topLevel);

            logger.Success = $"{Index}:\tDirectory structure created for {curlDetails.Yesterday:d}";
            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger GetFiles()
        {
            Index++;
            DomainsWithFiles = new List<AdministrativeDomain>();
            foreach (var domain in Domains)
            {
                curlDetails.DomainId = domain.AdmtiveDomainId;
                curlDetails.Directory = DirectoryInfo;
                var result = scriptProcessor.RequestFile(curlDetails).ReadToEnd();
                var content = result.HasNoRecordForCommunity() ? "" : result;
                var file = "";
                if (!string.IsNullOrEmpty(content))
                {
                    file = fileProcessor.SetOriginalFile(curlDetails, content);
                }

                if (string.IsNullOrEmpty(file))
                {
                    logger.Error = $"{Index}:\tRequested file not found for '{domain.DomainName} - {domain.AdmtiveDomainId}' domain.";
                    var state = GetLoggerState();
                    logger.AddLog(state);
                    logger.Error = "";
                }
                else
                {
                    logger.Success = $"{Index}:\tDownload file requested for '{domain.DomainName} - {domain.AdmtiveDomainId}' domain";
                    DomainsWithFiles.Add(domain);
                    var state = GetLoggerState();
                    logger.AddLog(state);
                    logger.Success = "";
                }
            }

            return logger;
        }

        public ILogger GetSiteNumber()
        {
            Index++;
            curlDetails.SiteNumbers = databaseLogic.GetSiteNumbers(DomainsWithFiles).ToList();
            if (curlDetails.SiteNumbers.Any())
            {
                logger.Success = $"{Index}:\tSite numbers {string.Join(", ", curlDetails.SiteNumbers)} is retrieved.";
            }
            else
            {
                logger.Error = $"{Index}:\tSite number {string.Join(",", curlDetails.SiteNumbers)} is not retrieved.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger SetAttemptsFile()
        {
            curlDetails.Directory = DirectoryInfo;
            foreach (var domain in Domains.Except(DomainsWithFiles))
            {
                Index++;
                curlDetails.DomainId = domain.AdmtiveDomainId;
                
                if (domain.IsTopLevel())
                {
                    curlDetails.AttemptsFile = Constants.GetAttemptsFilename(curlDetails);
                    fileProcessor.SetAttemptsFile(curlDetails);
                }
                else
                {
                    File.AppendAllText(curlDetails.AttemptsFile, Constants.SetAttemptText(curlDetails), Encoding.UTF8);
                }

                logger.Success = $"{Index}:\tAttempt for downloading file for {domain.AdmtiveDomainId} domain was done.";
                var state = GetLoggerState();
                logger.AddLog(state);
            }


            return logger;
        }

        public ILogger GetAttemptsNumbers()
        {
            var topLevelDomain = Domains.Where(IsTopLevel).First();
            curlDetails.DomainId = topLevelDomain.AdmtiveDomainId;
            var lines = fileProcessor.ReadAttemptsFile(curlDetails).ToList();
            curlDetails.Attempts = new Dictionary<string, int>();
            foreach (var domain in Domains.Except(DomainsWithFiles))
            {
                Index++;
                var count = lines
                           .Where(line => line.Contains($"'{domain.AdmtiveDomainId}'"))
                           .Select(number => number.GetBefore("x"))
                           .Count();

                curlDetails.Attempts.Add(domain.AdmtiveDomainId, count);

                var message = $"{Index}:\t{count} attempts were done for downloading file for '{domain.AdmtiveDomainId}'.";
                if (Constants.EVEN.Contains(count))
                {
                    logger.Success = message;
                }

                if (Constants.ODD.Contains(count))
                {
                    logger.Error = message;
                }

                var state = GetLoggerState();
                logger.AddLog(state);
            }

            return logger;
        }

        public ILogger SendEmail(string email = null)
        {
            var emailDetails = new EmailDetails
            {
                FromEmail = string.IsNullOrEmpty(BrandingDictionary["Email"]) ? ConfigurationManager.AppSettings["smtpEmail"] : BrandingDictionary["Email"],
                ToEmails = string.IsNullOrEmpty(email)
                               ? new List<string> { ConfigurationManager.AppSettings["toEmail"], BrandingDictionary["Email"] }
                               : new List<string> { email },
                Attachment = curlDetails.AttemptsFile
            };

            if (curlDetails.SessionId == 0)
            {
                Index++;
                emailDetails.Subject = $"Session Id error. for {BrandingDictionary["Company"]}";
                emailDetails.Body = $"Hi {BrandingDictionary["FirstName"]}" +
                                    "Session Id was not retrieved from your servers. Please check your settings." + Environment.NewLine +
                                    $"Login:\t{curlDetails.MainUrl}" + Environment.NewLine +
                                    $"User: \t{curlDetails.Username}" + Environment.NewLine +
                                    "Method:\t POST";

                emailProcessor.SendEmail(emailDetails, BrandingDictionary);
                if (emailProcessor.IsSent)
                {
                    logger.Success = $"{Index}:\tEmails sent to {string.Join(" ", emailDetails.ToEmails)} for '{curlDetails.DomainId}' domain.";
                }
                else
                {
                    logger.Error = $"{Index}:\tEmails were not sent to {string.Join(" ", emailDetails.ToEmails)} for '{curlDetails.DomainId}' domain.";
                }
            }
            else if (!DomainsWithFiles.Any())
            {
                var topLevels = Domains.Where(domain => domain.IsTopLevel()).ToList();
                foreach (var topLevel in topLevels)
                {
                    var attempts = curlDetails.Attempts[topLevel.AdmtiveDomainId];
                    Index++;
                    if (Constants.EVEN.Contains(attempts))
                    {
                        emailDetails.Subject = $"Even attempt was done to download file for {BrandingDictionary["Company"]}";
                        emailDetails.Body = $"Hi {BrandingDictionary["FirstName"]}" +
                                            "Downloading attempts are recorded in attached file.Please check your settings." + Environment.NewLine +
                                            $"Url: \t{curlDetails.DomainUrl}" + Environment.NewLine +
                                            $"User:\t{curlDetails.Username}" + Environment.NewLine;

                        File.AppendAllText(curlDetails.AttemptsFile, Constants.EMAIL_SENT_MESSAGE);
                        emailProcessor.SendEmail(emailDetails, BrandingDictionary);
                        if (emailProcessor.IsSent)
                        {
                            logger.Success = $"{Index}:\tEmails sent to {string.Join(" ", emailDetails.ToEmails)} for '{curlDetails.DomainId}' domain.";
                        }
                        else
                        {
                            logger.Error = $"{Index}:\tEmails were not sent to {string.Join(" ", emailDetails.ToEmails)} for '{curlDetails.DomainId}' domain.";
                        }
                    }
                    if (Constants.ODD.Contains(attempts))
                    {
                        logger.Error = $"{Index}:\tNo emails is about to be send for '{curlDetails.DomainId}' domain.";
                    }
                }
            }
            else
            {
                Index++;
                logger.Error = $"{Index}:\tNo emails needs to be send for '{curlDetails.DomainId}' domain.";
            }


            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger CheckNumberOfEmailsSent()
        {
            curlDetails.Directory = DirectoryInfo;
            var topLevels = Domains.Where(IsTopLevel);

            var count = 0;
            foreach (var topLevel in topLevels)
            {
                curlDetails.DomainId = topLevel.AdmtiveDomainId;
                count = fileProcessor
                           .ReadAttemptsFile(curlDetails)
                           .Count(line => line.Contains("Email sent!"));
            }

            Index++;
            if (Constants.EVEN[0] == count)
            {
                logger.Success = $"{Index}:\t{Constants.EVEN[0]} emails has been sent to the user!";
                logger.AddLog(true);
            }
            else if (Constants.ODD[0] == count)
            {
                logger.Error = $"{Index}:\t{Constants.ODD[0]} email has been sent to the user!";
                logger.AddLog(false);
            }
            else
            {
                logger.Error = $"{Index}:\tNo files has been set to send emails!";
                logger.AddLog(false);
            }

            return logger;
        }

        public ILogger WasAlreadyDownloaded()
        {
            Index++;

            var fileDownloaded = fileProcessor.WereOriginalFilesDownloaded(curlDetails);
            curlDetails.Directory = DirectoryInfo;

            if (fileDownloaded)
            {
                foreach (var domain in DomainsWithFiles)
                {
                    curlDetails.DomainId = domain.AdmtiveDomainId;
                    foreach (var networkFile in curlDetails.NetworkFiles)
                    {
                        logger.Success += $"{Index}:\tOriginal file(s) '{networkFile}' already downloaded for '{curlDetails.DomainId}'.";
                    }
                }
            }
            else
            {
                logger.Error = $"{Index}:\tNo original files found on '{Constants.UncPaths[1]}' for '{curlDetails.DomainId}' domain.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger RenameFiles()
        {
            Index++;

            var files = fileProcessor.RenameTargetFiles(DirectoryInfo, curlDetails)
                       .Select(it => it)
                       .ToList();
            if (files.Any())
            {
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        logger.Success += $"{Index}:\t{file.GetAfter("\\")} file have been renamed.\r\n";
                    }
                    else
                    {
                        logger.Error += $"{Index}:\t{file.GetAfter("\\")} file have not been renamed.";
                    }
                }
            }
            else
            {
                logger.Error = $"{Index}:\tNo files to rename.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger DelayProcess()
        {
            Index++;
            var timer = new Timer(2000);
            timer.Elapsed += (sender, e) => Delay();
            timer.Start();
            logger.Success = $"{Index}:\t{timer.Interval} (in milliseconds) has been set.";
            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        private void Delay()
        {
            logger.Success = "Interval has passed";
        }

        public ILogger MoveFiles()
        {
            Index++;

            var files = fileProcessor.MoveAllFiles(DirectoryInfo, curlDetails)
                       .Select(it => it)
                       .ToList();

            if (files.Any())
            {
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        logger.Success += $"{Index}:\tFile {file.GetAfter("\\")} has been moved in order to be processed.";
                    }
                    else
                    {
                        logger.Error += $"{Index}:\tFile {file.GetAfter("\\")} has not been moved and it won't be processed.";
                    }
                }
            }
            else
            {
                logger.Error = $"{Index}:\tNo file to move.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);

            return logger;
        }

        public ILogger UpdateDatabase()
        {
            //TODO: This portion needs major refactoring and get rid of the LINQ2SQL thing
            Index++;
            var isUpdated = databaseLogic.UpdateBackendAdmin(Domains);
            if (isUpdated)
            {
                logger.Success = $"{Index}:\tBackendAdmin database updated.";
            }
            else
            {
                logger.Error = $"{Index}:\tBackendAdmin database has not being updated.";
            }

            isUpdated = databaseLogic.UpdateCentile(Domains);
            if (isUpdated)
            {
                logger.Success = $"{Index}:\tCentile database updated.";
            }
            else
            {
                logger.Error = $"{Index}:\tCentile database has not being updated.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger AddJob()
        {
            Index++;
            var isAdded = databaseLogic.AddJob(DomainsWithFiles);
            if (isAdded)
            {
                logger.Success = $"{Index}:\tJob added.";
            }
            else
            {
                logger.Error = $"{Index}:\tNo jobs added.";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        public ILogger UpdateExtensionDirectory()
        {
            Index++;
            var isUpdated = databaseLogic.UpdateExtensionDirectory();
            if (isUpdated)
            {
                logger.Success = $"{Index}:\tExtension Directory updated.";
            }
            else
            {
                logger.Error = $"{Index}:\tExtension Directory is not updated";
            }

            var state = GetLoggerState();
            logger.AddLog(state);
            return logger;
        }

        //

        private Dictionary<string, string> BrandingDictionary { get; set; }

        private List<AdministrativeDomain> Domains { get; set; }

        private DirectoryInfo DirectoryInfo { get; set; }

        private List<AdministrativeDomain> DomainsWithFiles { get; set; }

        private int Index { get; set; }

        private readonly ILogger logger;
        private readonly CurlDetails curlDetails;
        private readonly IScriptProcessor scriptProcessor;
        private readonly IFileProcessor fileProcessor;
        private readonly IEmailProcessor emailProcessor;
        private readonly IDatabaseLogic databaseLogic;

        private static bool IsTopLevel(AdministrativeDomain domain)
        {
            return domain.AdmtiveDomainId.IsTopLevel();
        }

        private bool GetLoggerState()
        {
            return !string.IsNullOrEmpty(logger.Success);
        }
    }
}