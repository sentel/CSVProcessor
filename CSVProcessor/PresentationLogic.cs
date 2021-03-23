using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSVProcessor
{
    public class PresentationLogic
    {
        public void SetWindow(int size)
        {
            var process = Process.GetCurrentProcess();
            ShowWindow(process.MainWindowHandle, size);
        }

        public void DisplayWelcomeMessage()
        {
            var application = GetApplicationName();
            var decorator = new string('=', application.Length);
            Console.WriteLine(decorator);
            Console.WriteLine(application);
            Console.WriteLine(decorator);
        }

        public void ExitApplication()
        {
            Console.Write("Exiting... ");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }

        public void DisplayLog(ILogger logger)
        {
            logger.Logs.ForEach(Console.WriteLine);
            var decorator = new string('-', string.IsNullOrEmpty(logger.Success) ? logger.Error.Length + Constants.TAB_SIZE : logger.Success.Length + Constants.TAB_SIZE);
            Console.WriteLine(decorator);
        }

        public void ClearLogs(ILogger logger)
        {
            logger.Logs.Clear();
            logger.Success = string.Empty;
            logger.Error = string.Empty;
        }

        //

        private string GetApplicationName()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = assembly.FullName;
            var parts = fullName.Split(',');
            return parts[0];
        }


        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    }
}
