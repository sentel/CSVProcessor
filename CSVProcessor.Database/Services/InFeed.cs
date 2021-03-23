using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class InFeed
    {
        public InFeed()
        {
            configuration = new DatabaseConfiguration();
        }

        public Table<tbl_infeed> GetExtensions() => Context["centile"].GetTable<tbl_infeed>();

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
    }
}
