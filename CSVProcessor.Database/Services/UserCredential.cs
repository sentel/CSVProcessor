using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class UserCredential
    {
        public UserCredential()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_User_Credential> GetUserCredentials()
        {
            return Context["backendAdmin"].GetTable<tbl_User_Credential>();
        }

        public tbl_User_Credential InsertUserCredential(int userId, string username, string password)
        {
            NewUserCredential = mapper.MapUserCredential(userId, username, password);
            Context["backendAdmin"].GetTable<tbl_User_Credential>().InsertOnSubmit(NewUserCredential);
            configuration.SaveAdminChanges();
            return NewUserCredential;
        }

        //

        private tbl_User_Credential NewUserCredential { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
