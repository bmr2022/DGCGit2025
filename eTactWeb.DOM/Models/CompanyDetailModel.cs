using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public  class CompanyDetailModel: TimeStamp
    {
        public string Flag { get; set; } = string.Empty;
        public long EntryID { get; set; } = 0;
        public string DispName { get; set; } = string.Empty;
        public string Com_Name { get; set; } = string.Empty;
        public string WebSite { get; set; } = string.Empty;
        public string OfficeAdd1 { get; set; } = string.Empty;
        public string OfficeAdd2 { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string Commodity { get; set; } = string.Empty;
        public string? Start_Date { get; set; } = null;
        public string? End_Date { get; set; } = null;
        public string PhoneF { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string OrgType { get; set; } = string.Empty;
        public string PANNo { get; set; } = string.Empty;
        public string TDSAccount { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public string TDSRange { get; set; } = string.Empty;
        public string GSTNO { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
        public string DataBase_Name { get; set; } = string.Empty;
        public string PFApplicable { get; set; } = string.Empty;
        public string PFNO { get; set; } = string.Empty;
        public string Registration_No { get; set; } = string.Empty;
        public string VENDOR_CODE { get; set; } = string.Empty;
        public string CC { get; set; } = string.Empty;
        public string? SoftwareStartDate { get; set; } = null;
        public string Prefix { get; set; } = string.Empty;
        public string ContactPersonSales { get; set; } = string.Empty;
        public string ContacPersonPurchase { get; set; } = string.Empty;
        public string ContactPersonQC { get; set; } = string.Empty;
        public string ContactPersonAccounts { get; set; }
        public string Country { get; set; } 
        public string LUTNO { get; set; } 
        public string? LUTDATE { get; set; } 
        public string? ActualEntryBy { get; set; } 
        public string? ActualEntryDate { get; set; } 
        public string UDYMANO { get; set; } 
        public string FromDate { get; set; } 
        public string ToDate { get; set; }
        public string Searchbox { get; set; } 

        public List<CompanyDetailModel> CompanyDetailGrid { get; set; }
    }
}
