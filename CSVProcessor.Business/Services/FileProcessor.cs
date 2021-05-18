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

        public bool Clean(string rootPath, string topLevel)
        {
            var fullPath = Path.Combine(rootPath, topLevel);
            var doesExists = Directory.Exists(fullPath);
            if (!doesExists)
            {
                return false;
            }

            var files = Directory.GetFiles(fullPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            var remainingFiles = Directory.GetFiles(fullPath);
            return !remainingFiles.Any();
        }

        public DirectoryInfo CreateDirectory(string topLevel)
        {
            var setDirectory = Constants.SetDirectory(topLevel);
            var directory = Directory.CreateDirectory(setDirectory);
            return directory;
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

        public IEnumerable<string> RenameTargetFiles(DirectoryInfo directory, CurlDetails details)
        {
            var newFiles = new List<string>();
            var files = directory.EnumerateFiles("*.csv", SearchOption.TopDirectoryOnly).ToList();
            for (var index = 0; index < files.Count; index++)
            {
                var currentName = files[index].Name;
                var newName = $"{details.SiteNumbers[index]}--{details.Yesterday:yyyy-MM-dd}.csv";
                var path = Constants.ROOT_PATH + "\\" + directory.Name + "\\";

                if (File.Exists(path + currentName))
                {
                    File.Copy(path + currentName, path + newName, true);
                }

                File.Delete(path + currentName);

                newFiles.Add(newName);
            }

            return newFiles.Any() ? newFiles : new List<string>();
        }

        public IEnumerable<string> MoveAllFiles(DirectoryInfo directory, CurlDetails details)
        {
            var currentFiles = Directory.EnumerateFiles($"{Constants.ROOT_PATH}\\{directory.Name}", "*.csv", SearchOption.TopDirectoryOnly)
                                        .Where(IsNotEmpty)
                                        .ToList();
            if (!currentFiles.Any())
            {
                return currentFiles;
            }

            foreach (var currentFile in currentFiles)
            {
                foreach (var siteNumber in details.SiteNumbers.Where(siteNumber => currentFile.GetAfter("\\").StartsWith(siteNumber.ToString())))
                {
                    File.Copy(currentFile, Constants.UncPaths[0] + "\\" + currentFile.GetAfter("\\"), true);

                    if (!Directory.Exists($"{Constants.UncPaths[1]}\\{siteNumber}"))
                    {
                        Directory.CreateDirectory($"{Constants.UncPaths[1]}\\{siteNumber}");
                    }

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

        private bool IsNotEmpty(string currentFile)
        {
            var lines = File.ReadAllLines(currentFile);
            return lines.Length > 1;
        }
    }
}
