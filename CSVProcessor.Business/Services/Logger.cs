using CSVProcessor.Business.Contracts;
using System.Collections.Generic;

namespace CSVProcessor.Business.Services
{
    public class Logger : ILogger
    {
        public Logger()
        {
            Logs = new List<string>();
        }

        public string Success { get; set; }

        public string Error { get; set; }

        public List<string> Logs { get; }

        public void AddLog(bool state)
        {
            Logs.Add(state ? Success : Error);
        }
    }
}