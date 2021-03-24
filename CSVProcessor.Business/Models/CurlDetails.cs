using CSVProcessor.Business.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSVProcessor.Business.Models
{
    public class CurlDetails
    {
        ////TODO: Move these URL in a separate class with properties and use them
        public string Username => "apalo_sentel";

        public string Password => "y4TWQzEG7ex3fWa5a8cZ";

        public string MainUrl => "http://thirdparty-api.apalo.io/restletrouter/v1/service/Login";

        public string DomainUrl => "https://thirdparty-api.apalo.io/restletrouter/v1/3rdParty/AdmtiveDomain";

        public string FileUrl => $@"https://thirdparty-api.apalo.io/restletrouter/v1/3rdParty/CallRecord/{DomainId}?day={Yesterday:yyyyMMdd}";

        //public string Username => "sentel-cdrs";

        //public string Password => @"nZI23pJ*CSo!$&wawANDN%@TxBD0nu0i&8RJCh";

        //public string MainUrl => "https://thirdparty-api.centrex.centile.net/restletrouter/v1/service/Login";

        //public string DomainUrl => "https://thirdparty-api.centrex.centile.net/restletrouter/v1/3rdParty/AdmtiveDomain";

        //public string FileUrl => $"https://thirdparty-api.centrex.centile.net/restletrouter/v1/3rdParty/CallRecord/{DomainId}?day={Yesterday:yyyyMMdd}";

        public string GetCommand(string specificUrl)
        {
            return $"curl -k -v -H \"Cookie: thirdParty_SESSIONID={SessionId}\" -H \"X-Application: 3rdParty\" {specificUrl}";
        }

        public string Domain => "Sentel";

        public long SessionId { get; set; }

        public string DomainId { get; set; }

        public List<int?> SiteNumbers { get; set; }

        public DateTime Today { get; } = DateTime.Now;

        public DateTime Yesterday => Today.AddDays(-1);

        public string CurlApplication => Constants.GetCurlCommand();

        public string OriginalFile => Constants.GetOriginalFile(Yesterday, Directory.Name,DomainId);

        public List<string> NetworkFiles
        {
            get { return Constants.GetNetworkYesterdayFiles(this); }
        }

        public string AttemptsFile { get; set; }

        public DirectoryInfo Directory { get; set; }

        public Dictionary<string, int> Attempts { get; set; }
    }
}
