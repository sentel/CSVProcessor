using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using CSVProcessor.Database.Services;
using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Linq;

namespace CSVProcessor.Database
{
    public class DatabaseLogic : IDatabaseLogic
    {
        public DatabaseLogic()
        {
            details = new TableDetails();
            IsUpdated = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> IsUpdated { get; }

        public Dictionary<string, string> GetBranding(string clientName)
        {
            var dictionary = new Dictionary<string, string>();

            var companies = details.Company.GetCentileCompanies();
            var dbCompany = companies.FirstOrDefault(it => it.company_name == clientName);
            if (dbCompany == null)
            {
                return dictionary;
            }

            dictionary.Add("Company", dbCompany.company_name);

            var brands = details.Brand.GetBrands();
            var dbBrand = brands.FirstOrDefault(it => it.SiteName.Equals(dbCompany.company_name));
            if (dbBrand == null)
            {
                return dictionary;
            }

            dictionary.Add("FirstName", $"{dbBrand.FirstName}");
            dictionary.Add("Logo", $"{dbBrand.Logo}");
            dictionary.Add("Colors", $"{dbBrand.Colors}");

            var emails = details.Email.GetEmails();
            var dbEmail = emails.FirstOrDefault(it => it.Email.Equals(dbBrand.Email));
            if (dbEmail == null)
            {
                return dictionary;
            }

            dictionary.Add("Email", dbEmail.Email);
            dictionary.Add("Username", dbEmail.Username);
            dictionary.Add("Password", dbEmail.Password);
            dictionary.Add("Host", dbEmail.Host);
            dictionary.Add("Port", dbEmail.Port.ToString());

            return dictionary;
        }

        public bool UpdateBackendAdmin(List<AdministrativeDomain> domains)
        {
            var topLevel = domains.First(IsTopLevel);
            var companies = details.Company.GetAdminCompany();
            var dbCompany = companies.FirstOrDefault(it => it.Company_Name == topLevel.DomainName || it.Company_Name == "Centile");
            if (dbCompany == null)
            {
                dbCompany = details.Company.InsertAdminCompany(topLevel);
            }


            var unitTypes = details.UnitType.GetUnitTypes();
            var dbUnitType = unitTypes.FirstOrDefault(it => it.UnitType_Name == dbCompany.Company_Name || it.UnitType_Name == "Centile");
            if (dbUnitType == null)
            {
                dbUnitType = details.UnitType.InsertUniType(dbCompany.Company_Name);
            }


            var enterprises = domains
                             .Where(it => !IsTopLevel(it))
                             .Where(domain => domain.DomainType.Equals("Enterprise"))
                             .ToList();

            tbl_Site_XSI_Detail dbSiteXsiDetails = null;
            foreach (var enterprise in enterprises)
            {
                var sites = details.Site.GetAdminSites();
                BackendAdminDbSite = sites.FirstOrDefault(it => it.Site_name == enterprise.DomainName);
                if (BackendAdminDbSite == null)
                {
                    BackendAdminDbSite = details.Site.InsertAdminSite(enterprise, dbCompany.tbl_CompanyId);
                }


                var siteEnterprises = details.SiteEnterprise.GetSiteEnterprises();
                var dbSiteEnterprise = siteEnterprises.FirstOrDefault(it => it.tbl_SiteId == BackendAdminDbSite.tbl_SiteId);
                if (dbSiteEnterprise == null)
                {
                    dbSiteEnterprise = details.SiteEnterprise.InsertSiteEnterprise(enterprise, BackendAdminDbSite.tbl_SiteId);
                }


                var siteXsiDetails = details.SiteXsiDetail.GetSiteXsiDetails();
                dbSiteXsiDetails = siteXsiDetails.FirstOrDefault(it => it.tbl_SiteId == dbSiteEnterprise.tbl_SiteId);
                if (dbSiteXsiDetails == null)
                {
                    dbSiteXsiDetails = details.SiteXsiDetail.InsertSiteXsiDetail(dbSiteEnterprise.tbl_SiteId, dbSiteEnterprise.tbl_Site_Enterprise_Name);
                }


                var siteCommunities = details.SiteCommunity.GetSiteCommunities();
                var dbSiteCommunity = siteCommunities.FirstOrDefault(it => it.tbl_SiteId == dbSiteXsiDetails.tbl_SiteId);
                if (dbSiteCommunity == null)
                {
                    dbSiteCommunity = details.SiteCommunity.InsertSiteCommunity(dbSiteXsiDetails.tbl_SiteId, enterprise.AdmtiveDomainId);
                }

                var siteUnitTypes = details.SiteUnitType.GetSiteUnitTypes();
                // ReSharper disable once UnusedVariable
                var dbSiteUnitType = siteUnitTypes.FirstOrDefault(it => it.tbl_SiteId == dbSiteCommunity.tbl_SiteId);
                dbSiteUnitType = dbSiteUnitType == null
                                     ? details.SiteUnitType.InsertSiteUnitType(dbSiteCommunity.tbl_SiteId, dbUnitType.tbl_UnitTypeId)
                                     : details.SiteUnitType.UpdateSiteUnitType(dbSiteCommunity.tbl_SiteId, dbUnitType.tbl_UnitTypeId);

            }

            var users = details.User.GetUsers();
            var dbUser = users.FirstOrDefault(it => it.User_Forename.Equals("Gabriel") &&
                                                    it.User_Surname.Equals("Popescu"));
            if (dbUser == null)
            {
                dbUser = details.User.InsertUser();
            }


            var userCredentials = details.UserCredential.GetUserCredentials();

            var dbUserCredential = userCredentials.FirstOrDefault(it => it.tbl_User_CredentialsId == dbUser.tbl_UserId);
            if (dbUserCredential == null)
            {
                dbUserCredential = details.UserCredential.InsertUserCredential(dbUser.tbl_UserId,
                                                                               dbSiteXsiDetails.tbl_Site_XSI_Username,
                                                                               dbSiteXsiDetails.tbl_Site_XSI_Password);
            }


            return true;
        }

        public bool UpdateCentile(IEnumerable<AdministrativeDomain> domains)
        {
            var enterprises = domains.Where(it => !IsTopLevel(it)).Where(domain => domain.DomainType.Equals("Enterprise")).ToList();

            foreach (var enterprise in enterprises)
            {
                var centileSites = details.Site.GetCentileSites().AsEnumerable();
                foreach (var centileSite in centileSites)
                {
                    if (enterprise.DomainName == centileSite.site_name)
                    {
                        var adminSites = details.Site.GetAdminSites().AsEnumerable();
                        foreach (var adminSite in adminSites)
                        {
                            if (adminSite.Site_name == centileSite.site_name)
                            {
                                details.Site.UpdateCentileSite(enterprise, adminSite.tbl_SiteId);
                            }
                        }
                    }
                }
            }


            return true;
        }

        public IEnumerable<int?> GetSiteNumbers(List<AdministrativeDomain> domainsWithFiles)
        {
            var sites = new List<int?>();
            if (!domainsWithFiles.Any())
            {
                return sites;
            }

            sites.AddRange(domainsWithFiles.Select(domain => details.Site.GetCentileSites().FirstOrDefault(it => it.site_name == domain.DomainName))
                                           .Where(siteWithFile => siteWithFile != null)
                                           .Where(siteWithFile => siteWithFile.site_number != null)
                                           .Select(siteWithFile => siteWithFile.site_number));

            return sites;
        }

        public bool AddJob(List<string> files)
        {
            var jobs = files
               .Select(file => details.Site.GetAdminSites().FirstOrDefault(it => it.tbl_SiteId.Equals(int.Parse(file.GetBetween("\\","--")))))
               .Select(dbSite => new
               {
                   dbSite,
                   dbJob = details.Job.GetJobs().FirstOrDefault(it => it.tbl_SiteId.Equals(dbSite.tbl_SiteId))
               })
               .Where(it => it.dbJob == null)
               .Select(it => details.Job.InsertJob(it.dbSite.tbl_SiteId))
                      .ToList();
            return jobs.Any();
        }

        public bool UpdateExtensionDirectory()
        {
            var extensions = details.Extension.GetExtensions().ToList();
            var incoming = details.InFeed.GetExtensions()
                                  .ToList()
                                  .Where(table => table.infeed_firstextension.All(char.IsDigit))
                                  .Where(table => !extensions.Any(it => it.Extension.Equals(long.Parse(table.infeed_firstextension))))
                                  .Select(table => new Extension
                                  {
                                      Site = table.infeed_site_id_FK,
                                      Number = long.Parse(table.infeed_firstextension)
                                  })
                                  .ToList();

            var outgoing = details.OutFeed.GetExtensions()
                                  .ToList()
                                  .Where(table => table.outfeed_extensionnumber.All(char.IsDigit))
                                  .Where(table => !extensions.Any(it => it.Extension.Equals(long.Parse(table.outfeed_extensionnumber))))
                                  .Select(table => new Extension
                                  {
                                      Site = table.outfeed_site_id_FK,
                                      Number = long.Parse(table.outfeed_extensionnumber)
                                  })
                                  .ToList();


            var newExtensions = new List<Extension>();
            var allExtensions = incoming.Concat(outgoing).ToList();

            foreach (var ext in allExtensions.Where(ext => !newExtensions.Any(it => it.Number.Equals(ext.Number))))
            {
                newExtensions.Add(ext);
            }

            details.Extension.InsertDirectory(newExtensions);

            return details.Extension.IsUpdated;
        }

        //

        private readonly TableDetails details;

        private tbl_Site BackendAdminDbSite { get; set; }

        private static bool IsTopLevel(AdministrativeDomain domain)
        {
            return domain.AdmtiveDomainId.IsTopLevel();
        }
    }
}