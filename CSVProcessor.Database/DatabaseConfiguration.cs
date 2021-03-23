using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;

namespace CSVProcessor.Database
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration()
        {
            adminDbContext = new BackEndAdminDataContext();
            centileDbContext = new CentileDataContext();
        }

        public Dictionary<string, DataContext> SetupConnectionString(string environment)
        {
            adminDbContext = new BackEndAdminDataContext(ConfigurationManager.ConnectionStrings["BackendAdmin" + environment].ConnectionString);
            centileDbContext = new CentileDataContext(ConfigurationManager.ConnectionStrings["Centile" + environment].ConnectionString);

            return new Dictionary<string, DataContext>
            {
                {"backendAdmin", adminDbContext},
                {"centile", centileDbContext}
            };
        }

        public void SaveAdminChanges()
        {
            adminDbContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public void SaveCentileChanges(int siteId, int? siteNumber)
        {
            if (siteNumber != 0)
                centileDbContext.ExecuteCommand($"UPDATE tbl_site SET site_number = {siteNumber} WHERE site_id_PK = {siteId}");
            else
                centileDbContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        //

        private BackEndAdminDataContext adminDbContext;
        private CentileDataContext centileDbContext;
    }
}
