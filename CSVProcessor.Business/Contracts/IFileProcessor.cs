using CSVProcessor.Business.Models;
using System.Collections.Generic;
using System.IO;

namespace CSVProcessor.Business.Contracts
{
    public interface IFileProcessor
    {
        bool WereOriginalFilesDownloaded(CurlDetails details);

        IEnumerable<string> ReadAttemptsFile(CurlDetails details);

        bool Clean(string rootPath, string topLevel);

        List<DirectoryInfo> CreateDirectory(IEnumerable<string> topLevels);

        bool IsOriginalFileRead(string originalFile);

        List<string> RenameTargetFiles(DirectoryInfo directory, CurlDetails details);

        List<string> MoveAllFiles(DirectoryInfo directory, CurlDetails details);

        void SetAttemptsFile(CurlDetails details);

        string SetOriginalFile(CurlDetails details, string file);
        
    }
}