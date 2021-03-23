using CSVProcessor.Business.Contracts;
using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using CSVProcessor.Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSVProcessor.Tests
{
    [TestClass]
    public class FileProcessorTests
    {
        [TestInitialize]
        public void Setup()
        {
            streams = new List<StreamReader>();
            fileProcessor = new FileProcessor(streams);
            yesterdayFolder = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            yesterdayFile = DateTime.Now.AddDays(-1).ToString("yy-MM-dd");
            originalFile = $@"C:\Users\Gabriel\Documents\Top-Level\{yesterdayFolder}\{yesterdayFile}-0.0.0.0..csv";
            details = new CurlDetails();
        }

        [TestMethod]
        public void IsOriginalFileIsReadReturnsTrue()
        {
            var isOriginalFileRead = fileProcessor.IsOriginalFileRead(originalFile);
            Assert.IsTrue(isOriginalFileRead);
        }

        [TestMethod]
        public void RenameFileTargetsReturnsFilesStartingWithSiteNumberAsName()
        {
            details.SiteNumbers = new List<int?> { 10411, 10412, 10413 };
            details.Directory = new DirectoryInfo($"{Constants.ROOT_PATH}\\Top-Level");
            var files = fileProcessor.RenameTargetFiles(details.Directory, details);
            for (var i = 0; i < files.Count; i++)
            {
                Assert.AreEqual($"{details.SiteNumbers[i]}--2021-02-17.csv", files[i]);
            }
        }

        [TestMethod]
        public void MoveAllFilesMovesFilesToNetwork()
        {
            details.SiteNumbers = new List<int?> { 10411, 10412, 10413 };
            details.Directory = new DirectoryInfo($"{Constants.ROOT_PATH}\\Top-Level");
            var result = fileProcessor.MoveAllFiles(details.Directory, details);
            Assert.IsNull(result);
        }


        //

        private CurlDetails details;
        private IFileProcessor fileProcessor;
        private List<StreamReader> streams;
        private string originalFile;
        private string yesterdayFolder;
        private string yesterdayFile;
    }
}
