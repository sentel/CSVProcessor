using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class UnitType
    {
        public UnitType()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_UnitType> GetUnitTypes()
        {
            return Context["backendAdmin"].GetTable<tbl_UnitType>();
        }

        public tbl_UnitType InsertUniType(string companyName)
        {
            NewUnitType = mapper.MapUnitType(companyName);
            Context["backendAdmin"].GetTable<tbl_UnitType>().InsertOnSubmit(NewUnitType);
            configuration.SaveAdminChanges();
            return NewUnitType;
        }

        //

        private tbl_UnitType NewUnitType { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
