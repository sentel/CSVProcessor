using System.Collections.Generic;

namespace CSVProcessor.Business.Contracts
{
    public interface ILogger
    {
        string Success { get; set; }

        string Error { get; set; }

        List<string> Logs { get; }

        void AddLog(bool state);
    }
}
