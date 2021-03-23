using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace CSVProcessor.Database.Services
{
    public class SiteUnitType
    {
        public SiteUnitType()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_Site_UnitType> GetSiteUnitTypes()
        {
            return Context["backendAdmin"].GetTable<tbl_Site_UnitType>();
        }

        public tbl_Site_UnitType InsertSiteUnitType(int siteId, int unitTypeId)
        {
            NewSiteUnitType = mapper.MapSiteUnitType(siteId, unitTypeId);
            Context["backendAdmin"].GetTable<tbl_Site_UnitType>().InsertOnSubmit(NewSiteUnitType);
            configuration.SaveAdminChanges();
            return NewSiteUnitType;
        }

        public tbl_Site_UnitType UpdateSiteUnitType(int siteId, int unitTypeId)
        {
            var tables = Context["backendAdmin"].GetTable<tbl_Site_UnitType>().AsEnumerable();
            var siteUnitType = tables.FirstOrDefault(it => it.tbl_SiteId.Equals(siteId));
            if (siteUnitType == null)
                return null;

            siteUnitType.tbl_UnitTypeId = unitTypeId;
            configuration.SaveAdminChanges();

            return siteUnitType;
        }

        //

        private tbl_Site_UnitType NewSiteUnitType { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
