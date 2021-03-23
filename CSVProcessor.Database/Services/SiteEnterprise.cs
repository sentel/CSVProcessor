using CSVProcessor.Business.Models;
using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class SiteEnterprise
    {
        public SiteEnterprise()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_Site_Enterprise> GetSiteEnterprises()
        {
            return Context["backendAdmin"].GetTable<tbl_Site_Enterprise>();
        }

        public tbl_Site_Enterprise InsertSiteEnterprise(AdministrativeDomain domain, int siteId)
        {
            NewSiteEnterprise = mapper.MapSiteEnterprise(domain, siteId);
            Context["backendAdmin"].GetTable<tbl_Site_Enterprise>().InsertOnSubmit(NewSiteEnterprise);
            configuration.SaveAdminChanges();
            return NewSiteEnterprise;
        }

        //

        private tbl_Site_Enterprise NewSiteEnterprise { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
