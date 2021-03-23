using System.Collections.Generic;
using System.IO;

namespace CSVProcessor.Business.Helpers
{
    public static class FileExtensions
    {
        public static void MoveRenamedFiles(this List<string> files, int uncLocation)
        {
            files.RemoveAt(files.Count - 1);

            foreach (var file in files)
            {
                var item = file.GetAfter("\\");
                File.Copy(file, Constants.UncPaths[uncLocation] + "\\" + item);
            }

            files.ForEach(File.Delete);
        }

        public static void MoveOriginalFile(this string originalFile, int siteNumber)
        {
            var item = originalFile.GetAfter("\\");
            File.Copy(originalFile, Constants.UncPaths[1] + $"\\{siteNumber}\\" + item, true);
        }
    }
}
