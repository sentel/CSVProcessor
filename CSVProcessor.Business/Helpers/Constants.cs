using CSVProcessor.Business.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace CSVProcessor.Business.Helpers
{
    public static class Constants
    {
        public const int TIME_OUT = 500;
        public const int MAXIMUM_LINES = 9999;
        public const int TAB_SIZE = 5;

        public static readonly int[] EVEN = { 2, 4 };
        public static readonly int[] ODD = { 1, 3 };
        public const string EMAIL_SENT_MESSAGE = "\r\nEmail sent!\r\n===========\r\n\r\n";

        public static readonly string ROOT_PATH = Path.Combine(FixedDrives[0], MyDocumentsPath);

        public static string GetCurlCommand() =>
            Path.Combine(FixedDrives[0], ProgramFilesPath, CurlPath, "bin", "curl.exe");

        public static string GetAttemptsFilename(CurlDetails details) =>
            Path.Combine(ROOT_PATH, details.Directory.Name, $"{details.Yesterday:yy-MM-dd}-{details.DomainId} attempts.txt");

        public static string SetDirectory(string topLevel) =>
            Path.Combine(ROOT_PATH, topLevel);

        public static string GetOriginalFile(CurlDetails details) =>
            Path.Combine(ROOT_PATH, $"{details.Directory.Name}", $"{details.Yesterday:yy-MM-dd}-{details.DomainId}.csv");

        public static List<string> GetNetworkYesterdayFiles(CurlDetails details)
        {
            var files = new List<string>();
            if (!details.SiteNumbers.Any())
                return files;

            files.AddRange(details.SiteNumbers.Select(siteNumber => Path.Combine(UncPaths[1], $"{siteNumber}", $"{siteNumber}--{details.Yesterday:yyyy-MM-dd}" + ".csv")));

            return files;
        }

        public static string[] UncPaths { get; } = { UncPathRawDataPnd, UncPathRawDataArc };

        public static string SetTarget(DirectoryInfo directory, DateTime yesterday, int rows, int index) =>
            Path.Combine(ROOT_PATH, directory.Name, $"{yesterday:yyy-MM-dd}", $"{rows}-{index}.csv");

        public static string SetAttemptText(CurlDetails details) =>
            "1x attempt to download file " +
            $"for '{details.DomainId}' domain " +
            $"for '{details.Yesterday:D}' " +
            $"done on '{details.Today:D}' " +
            $"at '{details.Today:HH:mm}'." +
            $"{Environment.NewLine}";

        //

        private static string UncPathRawDataPnd => ConfigurationManager.AppSettings["UncRawDataPendPath"];

        private static string UncPathRawDataArc => ConfigurationManager.AppSettings["UncRawDataArchPath"];

        private static string[] FixedDrives => DriveInfo.GetDrives()
                                                        .Where(drive => drive.DriveType == DriveType.Fixed)
                                                        .Select(drive => drive.Name)
                                                        .ToArray();

        private static string ProgramFilesPath => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        private static string CurlPath => "cURL";

        private static string MyDocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}
