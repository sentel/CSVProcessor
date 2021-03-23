using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class OutFeed
    {
        public OutFeed()
        {
            configuration = new DatabaseConfiguration();
        }

        public Table<tbl_outfeed> GetExtensions() => Context["centile"].GetTable<tbl_outfeed>();

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
    }
}
