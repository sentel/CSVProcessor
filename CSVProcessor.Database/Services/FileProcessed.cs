using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace CSVProcessor.Database.Services
{
    public class FileProcessed
    {
        public FileProcessed()
        {
            configuration = new DatabaseConfiguration();
            mapper = new Mapper();
        }

        public bool IsUpdated { get; set; }

        public Table<Tables.FileProcessed> GetFilesProcessed()
        {
            return Context["backendAdmin"].GetTable<Tables.FileProcessed>();
        }

        //

        private Tables.FileProcessed NewFileProcessed { get; set; }

        private readonly DatabaseConfiguration configuration;
        private Dictionary<string, DataContext> Context => configuration.SetupConnectionString("Live");
        private readonly Mapper mapper;
    }
}
