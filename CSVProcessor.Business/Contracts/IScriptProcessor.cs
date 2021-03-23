using CSVProcessor.Business.Models;
using System.IO;
using System.Net.Http.Headers;

namespace CSVProcessor.Business.Contracts
{
    public interface IScriptProcessor
    {
        bool ConnectToFs01(CurlDetails details);

        HttpResponseHeaders GetSessionId(CurlDetails details);

        StreamReader GetDomains(CurlDetails details);

        StreamReader RequestFile(CurlDetails details);
    }
}