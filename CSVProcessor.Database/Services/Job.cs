using CSVProcessor.Database.Tables;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;

namespace CSVProcessor.Database.Services
{
    public class Job
    {
        public Job()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public bool IsUpdated { get; private set; }

        public Table<tbl_Job_PND> GetJobs()
        {
            return Context["backendAdmin"].GetTable<tbl_Job_PND>();
        }

        public tbl_Job_PND InsertJob(int siteNumber)
        {
            var newJob = mapper.MapJob(siteNumber);
            var connectionString = Context["backendAdmin"].Connection.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@tbl_BatchFileId", newJob.tbl_BatchFileId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@tbl_SiteId", newJob.tbl_SiteId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Job_CreatedTime", newJob.Job_PND_CreatedTime, DbType.DateTime, ParameterDirection.Input);
                connection.Execute("spInsertCentileJob", parameters, commandType: CommandType.StoredProcedure);
                return newJob;
            }
        }

        //

        private readonly DatabaseConfiguration configuration;

        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");

        private readonly Mapper mapper;
    }
}
