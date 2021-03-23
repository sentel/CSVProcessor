using CSVProcessor.Business.Models;
using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace CSVProcessor.Database.Services
{
    public class Site
    {
        public Site()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_Site> GetAdminSites()
        {
            return Context["backendAdmin"].GetTable<tbl_Site>();
        }

        public Table<tbl_site> GetCentileSites()
        {
            return Context["centile"].GetTable<tbl_site>();
        }

        public tbl_Site InsertAdminSite(AdministrativeDomain domain, int companyId)
        {
            var adminSite = mapper.MapAdminSite(domain, companyId);
            Context["backendAdmin"].GetTable<tbl_Site>().InsertOnSubmit(adminSite);
            configuration.SaveAdminChanges();
            return adminSite;
        }


        public tbl_site InsertCentileSite(AdministrativeDomain domain, int siteNumber)
        {
            var centileSite = mapper.MapCentileSite(domain, siteNumber);
            Context["centile"].GetTable<tbl_site>().InsertOnSubmit(centileSite);
            configuration.SaveCentileChanges(centileSite.site_id_PK, siteNumber);
            return centileSite;
        }

        public tbl_site UpdateCentileSite(AdministrativeDomain enterprise, int siteNumber)
        {
            var tables = Context["centile"].GetTable<tbl_site>().AsEnumerable();
            var centileSite = tables.FirstOrDefault(it => it.site_name.Equals(enterprise.DomainName));
            if (centileSite == null)
            {
                return null;
            }

            centileSite.site_number = siteNumber;
            configuration.SaveCentileChanges(centileSite.site_id_PK, siteNumber);
            return centileSite;
        }

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;

    }
}
