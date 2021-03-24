using CSVProcessor.Business.Models;
using System.Collections.Generic;

namespace CSVProcessor.Business.Contracts
{
    public interface IEmailProcessor
    {
        bool IsSent { get; }

        void SendEmail(EmailDetails details, Dictionary<string, string> brandingDictionary);
    }
}