using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using EncryptionProcessor;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;

namespace CSVProcessor.Business.Services
{
    public class ScriptProcessor : IScriptProcessor
    {
        public ScriptProcessor(Processor encryptionProcessor)
        {
            this.encryptionProcessor = encryptionProcessor;
        }

        public bool ConnectToFs01(CurlDetails details)
        {
            var isUpdated = false;
            var username = ConfigurationManager.AppSettings["FS01UN"];
            var password = ConfigurationManager.AppSettings["FS01PS"];
            var decrypted = encryptionProcessor.Decrypt(password, 1);
            var networkCredentials = new NetworkCredential(username, decrypted, details.Domain);
            var netCache = new CredentialCache { { "\\FS01", 80, "Basic", networkCredentials } };
            var credential = netCache.GetCredential("\\FS01", 80, "Basic");
            if (credential != null)
            {
                var ping = new Ping();
                var reply = ping.Send("192.168.10.150", 100);
                if (reply?.Status == IPStatus.Success)
                {
                    isUpdated = true;
                }
            }

            return isUpdated;
        }

        public HttpResponseHeaders GetSessionId(CurlDetails details)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic
            };

            using (var httpClient = new HttpClient(handler))
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), details.MainUrl))
            {
                request.Headers.TryAddWithoutValidation("X-Application", "3rdParty");
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{details.Username}:{details.Password}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                var response = httpClient.SendAsync(request);
                return response.Result.Headers;
            }
        }

        public StreamReader GetDomains(CurlDetails details)
        {
            return StartProcess(details, details.DomainUrl);
        }

        public StreamReader RequestFile(CurlDetails details)
        {
            return StartProcess(details, details.FileUrl);
        }

        //

        private readonly Processor encryptionProcessor;
        private static StreamReader StartProcess(CurlDetails details, string specificUrl)
        {
            using (var process = new Process())
            {
                process.StartInfo.Domain = details.Domain;
                process.StartInfo.FileName = details.CurlApplication;
                process.StartInfo.Verb = "runas";
                process.StartInfo.Arguments = "/c " + details.GetCommand(specificUrl);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                process.WaitForExit(Constants.TIME_OUT);
                return process.StandardOutput;
            }
        }
    }
}