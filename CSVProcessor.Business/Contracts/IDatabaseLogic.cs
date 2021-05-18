using CSVProcessor.Business.Models;
using System.Collections.Generic;

namespace CSVProcessor.Business.Contracts
{
    public interface IDatabaseLogic
    {
        Dictionary<string, string> GetBranding(string clientName);

        bool UpdateBackendAdmin(List<AdministrativeDomain> domains);

        bool UpdateCentile(IEnumerable<AdministrativeDomain> domains);

        IEnumerable<int?> GetSiteNumbers(List<AdministrativeDomain> domainsWithFiles);

        bool AddJob(List<string> files);

        bool UpdateExtensionDirectory();
    }
}
