using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Models;
using EncryptionProcessor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CSVProcessor.Business.Services
{
    public class EmailProcessor : IEmailProcessor
    {
        public EmailProcessor()
        {
            encryptionProcessor = new Processor();
        }

        public bool IsSent { get; private set; }

        public void SendEmail(EmailDetails details, Dictionary<string, string> brandingDictionary)
        {
            var credentials = new NetworkCredential
            {
                UserName = !string.IsNullOrEmpty(brandingDictionary["Username"]) ? ConfigurationManager.AppSettings["smtpEmail"] : brandingDictionary["Username"]
            };
            if (!string.IsNullOrEmpty(brandingDictionary["Password"]))
            {
                var password = ConfigurationManager.AppSettings["smtpPass"];
                var decryptedPassword = encryptionProcessor.Decrypt(password, 1);
                credentials.Password = decryptedPassword;
            }
            else
            {
                var password = brandingDictionary["Password"];
                credentials.Password = password;
            }

            var message = new MailMessage
            {
                From = !string.IsNullOrEmpty(details.FromEmail) ? new MailAddress(ConfigurationManager.AppSettings["smtpEmail"]) : new MailAddress(details.FromEmail)
            };
            message.To.Add(new MailAddress(details.ToEmails.First()));
            message.To.Add(new MailAddress(details.ToEmails.Last()));
            message.Subject = details.Subject;
            message.Body = details.Body;
            if (!string.IsNullOrEmpty(details.Attachment))
            {
                message.Attachments.Add(new Attachment(details.Attachment));
            }

            message.IsBodyHtml = true;

            using (var client = new SmtpClient())
            {
                client.Host = !string.IsNullOrEmpty(brandingDictionary["Host"]) ? ConfigurationManager.AppSettings["smtpHost"] : brandingDictionary["Host"];
                client.Port = !string.IsNullOrEmpty(brandingDictionary["Port"])
                                  ? Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"])
                                  : Convert.ToInt32(brandingDictionary["Port"]);

                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
                client.Credentials = credentials;
                client.Send(message);
            }
            IsSent = true;
        }

        //

        private readonly Processor encryptionProcessor;
    }
}