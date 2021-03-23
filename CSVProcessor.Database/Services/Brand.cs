using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class Brand
    {
        public Brand()
        {
            configuration = new DatabaseConfiguration();
        }

        public Table<tbl_branding> GetBrands()
        {
            return Context["centile"].GetTable<tbl_branding>();
        }

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
    }
}