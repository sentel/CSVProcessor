using System.Collections.Generic;

namespace CSVProcessor.Business.Models
{
    public class EmailDetails
    {
        public string FromEmail { get; set; }

        public List<string> ToEmails { get; set; }

        public string Subject { get; set; }

        public string Attachment { get; set; }

        public string Body { get; set; }
    }
}