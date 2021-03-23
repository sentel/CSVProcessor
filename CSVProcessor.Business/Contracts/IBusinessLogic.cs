namespace CSVProcessor.Business.Contracts
{
    public interface IBusinessLogic
    {
        ILogger GetBranding(string clientName);

        ILogger ConnectToFs01();

        ILogger GetSessionId();

        ILogger GetDomains();

        ILogger CleanUp();

        ILogger UpdateDatabase();
        
        ILogger CreateDirectoryStructure();

        ILogger GetFiles();

        ILogger GetSiteNumber();

        ILogger SetAttemptsFile();

        ILogger GetAttemptsNumbers();

        ILogger SendEmail(string email = null);

        ILogger CheckNumberOfEmailsSent();

        ILogger WasAlreadyDownloaded();

        ILogger RenameFiles();

        ILogger DelayProcess();

        ILogger MoveFiles();

        ILogger AddJob();

        ILogger UpdateExtensionDirectory();
    }
}
