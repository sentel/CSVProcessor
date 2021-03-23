using CSVProcessor.Business.Models;

namespace CSVProcessor.Business.Helpers
{
    public static class DomainsExtensions
    {
        public static bool IsTopLevel(this AdministrativeDomain domain) =>
            domain.AdmtiveDomainId.Length == 2 &&
            char.IsDigit(domain.AdmtiveDomainId, 0) &&
            char.IsPunctuation(domain.AdmtiveDomainId, 1);
    }
}
