using CSVProcessor.Database.Tables;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace CSVProcessor.Database.Services
{
    public class Extension
    {
        public Extension()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public int Site { get; set; }

        public long Number { get; set; }

        public bool IsUpdated { get; private set; }

        public Table<Directory> GetExtensions() => Context["centile"].GetTable<Directory>();

        public void InsertDirectory(List<Extension> extensions)
        {
            NewDirectories = extensions.Select(mapper.MapDirectory).ToList();

            var connectionString = Context["centile"].Connection.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                foreach (var directory in NewDirectories)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@siteId", directory.SiteId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@extension", directory.Extension, DbType.Int64, ParameterDirection.Input);
                    connection.ExecuteScalar<Directory>("spInsertExtensions", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            IsUpdated = true;
        }

        //

        private List<Directory> NewDirectories { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");

        private readonly Mapper mapper;
    }
}
