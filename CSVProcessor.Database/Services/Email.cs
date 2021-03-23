using System.Collections.Generic;
using System.Data.Linq;
using CSVProcessor.Database.Tables;

namespace CSVProcessor.Database.Services
{
    public class Email
    {
        public Email()
        {
            configuration = new DatabaseConfiguration();
        }

        public Table<tbl_email> GetEmails()
        {
            return Context["centile"].GetTable<tbl_email>();
        }

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");

    }
}