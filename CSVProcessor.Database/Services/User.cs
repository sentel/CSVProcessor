using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class User
    {
        public User()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_User> GetUsers()
        {
            return Context["backendAdmin"].GetTable<tbl_User>();
        }

        public tbl_User InsertUser()
        {
            NewUser = mapper.MapUser();
            Context["backendAdmin"].GetTable<tbl_User>().InsertOnSubmit(NewUser);
            configuration.SaveAdminChanges();
            return NewUser;
        }

        //

        private tbl_User NewUser { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
