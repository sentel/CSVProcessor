using CSVProcessor.Business.Models;
using CSVProcessor.Database.Tables;
using System.Collections.Generic;
using System.Data.Linq;

namespace CSVProcessor.Database.Services
{
    public class Company
    {
        public Company()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public Table<tbl_company> GetCentileCompanies()
        {
            return Context["centile"].GetTable<tbl_company>();
        }

        public Table<tbl_Company> GetAdminCompany()
        {
            return Context["backendAdmin"].GetTable<tbl_Company>();
        }

        public tbl_Company InsertAdminCompany(AdministrativeDomain domain)
        {
            var company = mapper.MapAdminCompany(domain);
            Context["backendAdmin"].GetTable<tbl_Company>().InsertOnSubmit(company);
            configuration.SaveAdminChanges();
            return company;
        }

        //

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");

        private readonly Mapper mapper;
    }
}
