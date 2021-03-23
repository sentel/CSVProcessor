using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVProcessor.Business.Services
{
    public class FileProcessor : IFileProcessor
    {
        public FileProcessor(List<StreamReader> streams)
        {
            this.streams = streams;
        }

        public bool WereOriginalFilesDownloaded(CurlDetails details)
        {
            return details.NetworkFiles.Any(File.Exists);
        }

        public IEnumerable<string> ReadAttemptsFile(CurlDetails details)
        {
            details.AttemptsFile = Constants.GetAttemptsFilename(details);
            return File.Exists(details.AttemptsFile)
                       ? File.ReadAllLines(details.AttemptsFile).ToList()
                       : new List<string>();
        }

        public bool Clean(string rootPath,string topLevel)
        {
            var fullPath = Path.Combine(rootPath, topLevel);
            var files = Directory.GetFiles(fullPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            var remainingFiles = Directory.GetFiles(fullPath);
            return !remainingFiles.Any();
        }

        public List<DirectoryInfo> CreateDirectory(IEnumerable<string> topLevels)
        {
            Directories = new List<DirectoryInfo>();
            var directories = topLevels
                             .Select(Constants.SetDirectory)
                             .Select(Directory.CreateDirectory);

            Directories.AddRange(directories);
            return Directories;
        }

        public bool IsOriginalFileRead(string originalFile)
        {
            if (!File.Exists(originalFile))
            {
                return streams.Any();
            }

            var fileStream = new FileStream(originalFile, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream, Encoding.UTF8);
            streams.Add(streamReader);
            return streams.Any();
        }

        public List<string> RenameTargetFiles(DirectoryInfo directory, CurlDetails details)
        {
            var newFiles = new List<string>();
            var files = directory.EnumerateFiles("*.csv", SearchOption.AllDirectories);
            var yesterdayFiles = files.Where(it => it.DirectoryName?.Contains($"{details.Yesterday:yyyy-MM-dd}") == true).ToArray();
            for (var index = 0; index < yesterdayFiles.Length; index++)
            {
                var currentName = yesterdayFiles[index].Name;
                var newName = $"{details.SiteNumbers[index]}--{details.Yesterday:yyyy-MM-dd}.csv";
                var path = Constants.ROOT_PATH + "\\" + directory.Name + "\\" + details.Yesterday.ToString("yyyy-MM-dd") + "\\";

                if (File.Exists(path + currentName))
                {
                    File.Copy(path + currentName, path + newName, true);
                }

                File.Delete(path + currentName);

                newFiles.Add(newName);
            }

            return newFiles.Any() ? newFiles : new List<string>();
        }

        public List<string> MoveAllFiles(DirectoryInfo directory, CurlDetails details)
        {
            var path = $"{Constants.ROOT_PATH}\\{directory.Name}\\{details.Yesterday:yyyy-MM-dd}";
            var currentFiles = Directory.EnumerateFiles(path, "*.csv", SearchOption.TopDirectoryOnly).ToList();
            if (!currentFiles.Any())
            {
                return currentFiles;
            }

            foreach (var currentFile in currentFiles)
            {
                foreach (var siteNumber in details.SiteNumbers
                                                  .Where(siteNumber => currentFile.GetAfter("\\")
                                                                                  .StartsWith(siteNumber.ToString()) &&
                                                                       IsNotEmpty(currentFile)))
                {
                    File.Copy(currentFile, Constants.UncPaths[0] + "\\" + currentFile.GetAfter("\\"), true);
                    File.Copy(currentFile, Constants.UncPaths[1] + $"\\{siteNumber}\\" + currentFile.GetAfter("\\"), true);
                }
            }

            return currentFiles;
        }

        public void SetAttemptsFile(CurlDetails details)
        {
            var text = Constants.SetAttemptText(details);
            File.AppendAllText(details.AttemptsFile, text, Encoding.UTF8);
        }

        public string SetOriginalFile(CurlDetails details, string file)
        {
            var stream = new FileStream($"{details.OriginalFile}", FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);
            writer.Write(file);
            writer.Flush();
            stream.Position = 0;
            stream.Close();
            return stream.ToString();
        }

        //

        private readonly List<StreamReader> streams;

        private List<DirectoryInfo> Directories { get; set; }

        private bool IsNotEmpty(string currentFile)
        {
            var lines = File.ReadAllLines(currentFile);
            return lines.Length > 1;
        }
    }
}
