using CSVProcessor.Business.Models;
using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class SiteCommunity
    {
        public SiteCommunity()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_Site_Community> GetSiteCommunities()
        {
            return Context["backendAdmin"].GetTable<tbl_Site_Community>();
        }

        public tbl_Site_Community InsertSiteCommunity(int siteId, string domainId)
        {
            NewSiteCommunity = mapper.MapSiteCommunity(siteId, domainId);
            Context["backendAdmin"].GetTable<tbl_Site_Community>().InsertOnSubmit(NewSiteCommunity);
            configuration.SaveAdminChanges();
            return NewSiteCommunity;
        }

        //

        private tbl_Site_Community NewSiteCommunity { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}