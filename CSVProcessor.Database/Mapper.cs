using CSVProcessor.Business.Helpers;
using CSVProcessor.Business.Models;
using CSVProcessor.Database.Services;
using CSVProcessor.Database.Tables;
using System;

namespace CSVProcessor.Database
{
    public class Mapper
    {
        public tbl_Company MapAdminCompany(AdministrativeDomain domain)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Company
            {
                Company_Name = domain.DomainName,
                tbl_AddressId = 2,
                Company_Active = true,
                tbl_Collection_ProcessId = 4,
                tbl_Server_FTP_TypeId = 2
            };
        }

        public tbl_UnitType MapUnitType(string companyName)
        {
            return new tbl_UnitType
            {
                UnitType_SerialClass = 'V',
                UnitType_Name = companyName,
                UnitType_Description = $"{companyName} is using Centile type of Master Parser"
            };
        }

        public tbl_Site MapAdminSite(AdministrativeDomain domain, int companyId)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Site
            {
                tbl_companyId = companyId,
                tbl_addressId = 11,
                tbl_timezonesId = 1,
                Site_name = domain.DomainName,
                Site_active = true,
                Site_MainLineNumber = domain.AdmtiveDomainId,
                tbl_Server_DBDetailsId = 17,
                tbl_Server_FTP_AccountId_Remote = 130,
                tbl_Server_ArchiveFilesId = 8,
                tbl_ContactFrequencyID = 2
            };
        }

        public tbl_Site_Enterprise MapSiteEnterprise(AdministrativeDomain domain, int siteId)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Site_Enterprise
            {
                tbl_Site_EnterpriseId = 1,
                tbl_SiteId = siteId,
                tbl_Site_Enterprise_Name = $"{domain.AdmtiveDomainId}-{domain.DomainName}",
                tbl_Site_Enterprise_Active = true
            };
        }

        public tbl_Site_Community MapSiteCommunity(int siteId, string domainId)
        {
            return new tbl_Site_Community
            {
                tbl_SiteId = siteId,
                ComunityId = domainId,
                tbl_Site_Community_active = true
            };
        }

        public tbl_Site_XSI_Detail MapSiteXsiDetails(int siteEnterpriseId, string siteEnterpriseName)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Site_XSI_Detail
            {
                tbl_Company_XSI_MasterDetailsId = 1,
                tbl_SiteId = siteEnterpriseId,
                tbl_Site_XSI_Username = $"{siteEnterpriseName}",
                tbl_Site_XSI_Password = "Bm6090ckgg".HasAndSaltIt()[0],
                tbl_Site_XSI_Active = true,
            };
        }

        public tbl_User MapUser()
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_User
            {
                tbl_User_PermissionsId = 3,
                User_Title = "Mr.",
                User_Forename = "Gabriel",
                User_Surname = "Popescu",
                User_Active = true,
                User_FirstTimeLogin = false,
                User_CreatedDatetime = DateTime.Now
            };
        }

        public tbl_User_Credential MapUserCredential(int userId, string username, string password)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_User_Credential
            {
                tbl_User_CredentialsId = userId,
                User_Credentials_Username = username.GetBetween("-", "."),
                User_Credentials_Password = password,
                User_Credentials_EmailAddress = username.GetAfter("."),
                User_Credentials_SecurityQuestion = "1+1",
                User_Credentials_SecurityAnswer = "two",
                User_Credentials_PasswordMustBeChanged = false,
                User_Credentials_PasswordExpiryDateTime = DateTime.MaxValue,
                User_Credentials_FirstLogin = false,
                User_Credentials_Salt = "GP1gp2gp3".HasAndSaltIt()[1]
            };
        }

        public tbl_company MapCentileCompany(AdministrativeDomain domain)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_company
            {
                company_name = domain.DomainName,
                address_id_PK = 2,
                branding_id = 3,
                tbl_company_level_id_PK = 1,
                tbl_Centile_Community_id_FK = domain.AdmtiveDomainId
            };
        }

        public tbl_site MapCentileSite(AdministrativeDomain domain, int siteNumber)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_site
            {
                site_number = siteNumber,
                site_name = domain.DomainName,
                site_type = domain.DomainType,
                site_userlimit = 15,
                site_lastinfeedmodifieddate = DateTime.Now,
                company_id_PK = 1,
                address_id_PK = 1,
                site_datadropfrequency = 1,
                tbl_Centile_Community_Id = domain.AdmtiveDomainId
            };
        }

        public tbl_Job_PND MapJob(int siteNumber)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Job_PND
            {
                tbl_BatchFileId = 1,
                tbl_SiteId = siteNumber,
                Job_PND_CreatedTime = DateTime.Now
            };
        }

        public tbl_Site_UnitType MapSiteUnitType(int siteId, int unitTypeId)
        {
            //TODO: need to get rid of magic numbers and strings
            return new tbl_Site_UnitType
            {
                tbl_SiteId = siteId,
                tbl_Site_UnitTypeId = unitTypeId,
                tbl_Site_UnitType_ValidFrom = DateTime.Today.AddDays(-1),
                tbl_Site_UnitType_ValidTo = null
            };
        }

        public Directory MapDirectory(Extension extension)
        {
            return new Directory
            {
                SiteId = extension.Site,
                Extension = extension.Number
            };
        }
    }
}
