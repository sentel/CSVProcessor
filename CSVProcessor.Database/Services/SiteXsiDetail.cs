using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class SiteXsiDetail
    {
        public SiteXsiDetail()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_Site_XSI_Detail> GetSiteXsiDetails()
        {
            return Context["backendAdmin"].GetTable<tbl_Site_XSI_Detail>();
        }


        public tbl_Site_XSI_Detail InsertSiteXsiDetail(int siteEnterpriseId, string siteEnterpriseName)
        {
            NewSiteXsiDetail = mapper.MapSiteXsiDetails(siteEnterpriseId, siteEnterpriseName);
            Context["backendAdmin"].GetTable<tbl_Site_XSI_Detail>().InsertOnSubmit(NewSiteXsiDetail);
            configuration.SaveAdminChanges();
            return NewSiteXsiDetail;
        }

        //


        private tbl_Site_XSI_Detail NewSiteXsiDetail { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
