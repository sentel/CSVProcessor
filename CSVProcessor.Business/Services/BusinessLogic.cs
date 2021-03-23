using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using System.IO;
using System.Linq;

namespace CSVProcessor.Business.Services
{
    public class BusinessLogic : IBusinessLogic
    {
        public BusinessLogic(ILogger logger, IScriptProcessor scriptProcessor, IFileProcessor fileProcessor, IEmailProcessor emailProcessor, IDatabaseLogic databaseLogic)
        {
            curlDetails = new CurlDetails();
            this.logger = logger;
            this.scriptProcessor = scriptProcessor;
            this.fileProcessor = fileProcessor;
            this.emailProcessor = emailProcessor;
            this.databaseLogic = databaseLogic;
        }

        public ILogger GetSessionId() =>
            scriptProcessor.GetSessionId(curlDetails);

        public ILogger GetDomainId() =>
            scriptProcessor.GetDomainId(curlDetails);

        public ILogger FileDownloaded() =>
            fileProcessor.FileDownloaded(curlDetails);

        public ILogger GetFile() =>
            scriptProcessor.GetFile(curlDetails);

        public ILogger ReadOriginalFile() =>
            fileProcessor.ReadSourceFile(curlDetails.OriginalFile);

        public ILogger SplitSourceFile() =>
            fileProcessor.SplitSourceFile();

        public ILogger RenameTargetFiles() =>
            fileProcessor.RenameTargetFiles(curlDetails.Yesterday);

        public ILogger MoveFiles() =>
            fileProcessor.MoveFiles(curlDetails.OriginalFile);

        public ILogger AddJob()
        {
            var files = Directory.EnumerateFiles(Constants.UncPaths[0], $"{Constants.SITE_NUMBER}*.csv", SearchOption.TopDirectoryOnly);
            if (files.Any())
                databaseLogic.AddJob(Constants.SITE_NUMBER);

            return logger;
        }

        public bool CreateOrAppendAttemptFile()
        {
            AttemptsFile = fileProcessor.CreateOrAppendAttemptsFile(curlDetails);
            var noOfAttempts = fileProcessor.CheckNumberOfAttempts(curlDetails);
            return noOfAttempts == 4;
        }

        public ILogger SendEmail(string message)
        {
            var emailDetails = new EmailDetails("sdemail@sentel.co.uk", new[] { "gabriel.popescu@sentel.co.uk", "jonathan.briggs@sentel.co.uk","ahamdaoui@centile.com" });
            emailProcessor.SendEmail(emailDetails, message, AttemptsFile);
            return logger;
        }

        //

        private readonly ILogger logger;
        private readonly CurlDetails curlDetails;
        private readonly IScriptProcessor scriptProcessor;
        private readonly IFileProcessor fileProcessor;
        private readonly IEmailProcessor emailProcessor;
        private readonly IDatabaseLogic databaseLogic;

        private string AttemptsFile { get; set; }
    }
}