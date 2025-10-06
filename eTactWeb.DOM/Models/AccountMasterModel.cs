using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccountMasterModel : TimeStamp
    {
        public int Account_Code { get; set; }
        public string? Account_Name { get; set; }
        public IList<AccountMasterModel>? AccountMasterList { get; set; }
        public string? AccountType { get; set; }
        public string? ApprovalDate { get; set; }
        public string? Approved { get; set; }
        public string? Approved_By { get; set; }
        public string? BankAccount_No { get; set; }
        public string? BankAddress { get; set; }
        public string? BankIFSCCode { get; set; }
        public string? BankSwiftCode { get; set; }
        public string? BlackListed { get; set; }
        public string? BlackListed_By { get; set; }
        public string? CC { get; set; }
        public string? City { get; set; }
        public string? ComAddress { get; set; }
        public string? ComAddress1 { get; set; }
        public string? Commodity { get; set; }
        public string? ContactPerson { get; set; }
        public string? Country { get; set; }
        public string? CreditDays { get; set; }
        public string? CreditLimit { get; set; }
        public string? DebCredCode { get; set; }
        public string? DisplayName { get; set; }
        public string? Division { get; set; }
        public string? EMail { get; set; }
        public string? Entry_Date { get; set; }
        public string? GSTNO { get; set; }

        public IList<SelectListItem> GstPartList { get; set; } =
            new List<SelectListItem>()
            {
                new() { Value = "CGST", Text = "Registered" },
                new() { Value = "ICGST", Text = "Composition" },
                new() { Value = "IGST", Text = "Unregistered" }
            };

        public string? GSTPartyTypes { get; set; }
        public string? GSTRegistered { get; set; }
        public string? GSTTAXTYPE { get; set; }

        public IList<SelectListItem> GstTypeList { get; set; } =
            new List<SelectListItem>()
            {
                new() { Value = "CGST", Text = "CGST" },
                new() { Value = "IGST", Text = "IGST" },
            };

        public string? InterbranchSaleBILL { get; set; }
        public string? MainGroup { get; set; }
        public string? MobileNo { get; set; }
        public string? PANNO { get; set; }
        public int ParentAccountCode { get; set; }
        public int SalePersonEmpId { get; set; }
        public string? ParentAccountName { get; set; }
        public IList<TextValue>? ParentGroupList { get; set; }
        public IList<TextValue>? DiscountCategoryList { get; set; }
        public IList<TextValue>? GroupDiscountCategoryList { get; set; }
        public IList<TextValue>? RegionList { get; set; }
        public string? Party_Code { get; set; }
        public string? MSMEType { get; set; }
        public string? MSMENo { get; set; }
        public string? Region { get; set; }
        public string? DiscountCategory { get; set; }
        public int? GroupDiscountCategory { get; set; }
        public string? PartyType { get; set; }

        public IList<SelectListItem> PartyTypeList { get; set; } =
            new List<SelectListItem>()
            {
                new() { Value = "Vendor", Text = "Vendor" },
                new() { Value = "Export", Text = "Export" },
                new() { Value = "Traders", Text = "Traders" },
                new() { Value = "Job Work", Text = "Job Work" },
                new() { Value = "Customer", Text = "Customer" },
                new() { Value = "Others", Text = "Others" },
                new() { Value = "Service Provider", Text = "Service Provider" },
            };

        public string? PhoneNo { get; set; }
        public string? PinCode { get; set; }
        public string? PurchasePersonEmailId { get; set; }
        public string? PurchMobileNo { get; set; }
        public string? PurchPersonName { get; set; }
        public string? QCPersonEmailId { get; set; }
        public string? RANGE { get; set; }
        public string? RateOfInt { get; set; }
        public string? ResponsibleEmpContactNo { get; set; }
        public string? ResponsibleEmployee { get; set; }
        public string? salesemailid { get; set; }
        public string? salesmobileno { get; set; }
        public string? salesperson_name { get; set; }
        public string? SalesPersonEmailId { get; set; }
        public string? SalesPersonMobile { get; set; }
        public string? SalesPersonName { get; set; }
        public string? Segment { get; set; }
        public string? SSL { get; set; }
        public string? SSLNo { get; set; }
        public string? State { get; set; }
        public IList<TextValue>? StateList { get; set; }
        public string? SubGroup { get; set; }
        public string? SubSubGroup { get; set; }
        public string? TDS { get; set; }
        public string? TDSPartyCategery { get; set; }
        public string? TDSRate { get; set; }
        public string? Uid { get; set; }
        public string? UnderGroup { get; set; }
        public string? WebSite_Add { get; set; }
        public string? WorkingAdd1 { get; set; }
        public string? WorkingAdd2 { get; set; }
        public int YearCode { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? DashboardType { get; set; }
        public string? Act { get; set; }
        public string? ImageURL { get; set; }

        public IList<SelectListItem> YesNo { get; set; } =
            new List<SelectListItem>()
            {
                new() { Value = "Y", Text = "Yes" },
                new() { Value = "N", Text = "No" },
            };
    }
}