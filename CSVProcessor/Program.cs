using CSVProcessor.Business;
using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Enums;
using CSVProcessor.Business.Services;
using CSVProcessor.Database;
using EncryptionProcessor;
using System.Collections.Generic;
using System.IO;

namespace CSVProcessor
{
    public static class Program
    {
        static Program()
        {
            var streams = new List<StreamReader>();
            logger = new Logger();
            presentationLogic = new PresentationLogic();
            var encryptionProcessor = new Processor();
            IScriptProcessor scriptProcessor = new ScriptProcessor(encryptionProcessor);
            IFileProcessor fileProcessor = new FileProcessor(streams);
            IEmailProcessor emailProcessor = new EmailProcessor();
            IDatabaseLogic databaseLogic = new DatabaseLogic();
            businessLogic = new BusinessLogic(logger, scriptProcessor, fileProcessor, emailProcessor, databaseLogic);
        }

        private static void Main()
        {
            presentationLogic.SetWindow((int)WindowType.Maximize);
            presentationLogic.DisplayWelcomeMessage();

            logger = businessLogic.GetBranding("analyticsuccloudio");
            Display();

            logger = businessLogic.ConnectToFs01();
            if (string.IsNullOrEmpty(logger.Success))
            {
                Display();
                SendEmail("gabriel.popescu@sentel.co.uk");
                Exit();
            }
            Display();

            logger = businessLogic.GetSessionId();
            if (!string.IsNullOrEmpty(logger.Error))
            {
                Display();
                SendEmail();
                Exit();
            }
            Display();

            logger = businessLogic.GetDomains();
            if (!string.IsNullOrEmpty(logger.Error))
            {
                Display();
                SendEmail();
                Exit();
            }
            Display();

            logger = businessLogic.CleanUp();
            Display();

            logger = businessLogic.UpdateDatabase();
            Display();

            logger = businessLogic.CreateDirectoryStructure();
            Display();

            logger = businessLogic.CheckNumberOfEmailsSent();
            if (!string.IsNullOrEmpty(logger.Success))
            {
                Display();
                Exit();
            }
            Display();

            logger = businessLogic.GetFiles();
            Display();

            logger = businessLogic.GetSiteNumber();
            Display();

            logger = businessLogic.SetAttemptsFile();
            Display();

            logger = businessLogic.GetAttemptsNumbers();
            Display();
            SendEmail();

            logger = businessLogic.WasAlreadyDownloaded();
            if (!string.IsNullOrEmpty(logger.Success))
            {
                Display();
                Exit();
            }
            Display();

            logger = businessLogic.RenameFiles();
            Display();

            logger = businessLogic.DelayProcess();
            Display();

            logger = businessLogic.MoveFiles();
            Display();

            logger = businessLogic.DelayProcess();
            Display();

            logger = businessLogic.AddJob();
            Display();

            logger = businessLogic.UpdateExtensionDirectory();
            Display();

            logger = businessLogic.CleanUp();
            Display();

            presentationLogic.SetWindow((int)WindowType.Restore);
            Exit();
        }

        //

        private static ILogger logger;
        private static readonly IBusinessLogic businessLogic;
        private static readonly PresentationLogic presentationLogic;

        private static void Display()
        {
            presentationLogic.DisplayLog(logger);
            presentationLogic.ClearLogs(logger);
        }

        private static void SendEmail(string email = null)
        {
            logger = businessLogic.SendEmail(email);
            if (!string.IsNullOrEmpty(logger.Success))
            {
                Display();
                Exit();
            }

            Display();
        }

        private static void Exit() => presentationLogic.ExitApplication();
    }
}
