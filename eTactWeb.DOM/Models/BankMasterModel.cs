using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class BankMasterModel : TimeStamp
    {
        public int Account_Code { get; set; }
        public string? Account_Name { get; set; }
        public IList<BankMasterModel>? BankMasterList { get; set; }
        public string? AccountType { get; set; }
        public string? ApprovalDate { get; set; }
        public string? Approved { get; set; }
        public string? Approved_By { get; set; }
        public string? BankAccount_No { get; set; }
        public string? BankAddress { get; set; }
        public string? BankIFSCCode { get; set; }
        public string? BankSwiftCode { get; set; } 
        public string? CC { get; set; }
        public string? City { get; set; }
        public string? ComAddress { get; set; }
        public string? ComAddress1 { get; set; }
        public string? Commodity { get; set; }
        public string? ContactPerson { get; set; }
        public string? Country { get; set; } = "India";
        public int CreditDays { get; set; }
        public int CreditLimit { get; set; }
        public string? DebCredCode { get; set; }
        public string? DisplayName { get; set; }
        public string? Division { get; set; }
        public string? EMail { get; set; }
        public string? Entry_Date { get; set; }
        public string? MainGroup { get; set; }
        public string? MobileNo { get; set; }
        public string? PANNO { get; set; }
        public int ParentAccountCode { get; set; }
        public string? ParentAccountName { get; set; }
        public IList<TextValue>? ParentGroupList { get; set; }
        public string? Party_Code { get; set; }
        public string? PartyType { get; set; }
        public string? PhoneNo { get; set; }
        public string? PinCode { get; set; }
         public int RateOfInt { get; set; }
        public string? ResponsibleEmpContactNo { get; set; }
        public string? ResponsibleEmployee { get; set; }
        public string? salesemailid { get; set; }
        public string? salesmobileno { get; set; }
        public string? salesperson_name { get; set; }
        public string? SalesPersonEmailId { get; set; }
        public string? SalesPersonMobile { get; set; }
        public string? SalesPersonName { get; set; }
         public string? State { get; set; }
        public IList<TextValue>? StateList { get; set; }
        public string? SubGroup { get; set; }
        public string? SubSubGroup { get; set; }
         public string? Uid { get; set; }
        public string? UnderGroup { get; set; }
         public int YearCode { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? DashboardType { get; set; }

         
    }
}
