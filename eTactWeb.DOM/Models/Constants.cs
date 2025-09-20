using System.Globalization;

namespace eTactWeb.DOM.Models
{
    public enum eTactPages
    {
        SaleOrder,
        PurchaseOrder
    }

    public enum StatusCodeList
    {
    }

    public static class Constants
    {
        public const string AccountCode = "Account_Code";
        public const string AccountName = "Account_Name";
        public const string CompanyName = "Com_Name";
        public const string CC = "CC";

        public const string CostCenterName = "CostCenterName";
        public const string Currency = "Currency";
        public const string RecStoreName = "Store_Name";
        public const string RecStoreId = "StoreId";

        public const string DeptName = "DeptName";
        public const string DeptId = "DeptId";
        public const string EmpyID = "Emp_Id";
        public const string EmpName = "EmployeeName";
        public const string EmpyName = "EmpNameCode";
        public const string EntryID = "EntryID";
        public const string ItemCode = "Item_Code";
        public const string Item_Code = "ItemCode";
        public const string BatchNo = "BatchNo";
        public const string UniqueBatchNo = "UniqueBatchNo";
        public const string ItemName = "ItemName";
        public const string MRPNo = "MRPNo";
        public const string PartCode = "PartCode";
        public const string QuotNo = "QuotNo";
        public const string SOFor = "SOFor";
        public const string SOType = "SOType";
        public const string POFOR = "POFOR";
        public const string POType = "POType";
        public const string ServiceName = "servicename";
        public const string StageDesc = "StageDescription";
        public const string StoreName = "Store_Name";
        public const string TaxID = "TaxID";
        public const string TaxName = "Tax_Name";
        public const string TaxType = "TaxType";
        public static readonly int UserID = 77777;
        public static int EmpID = 1;
        public static int FinincialYear { get; set; }=2023;
        public const string PONO = "PONO";
        public const string PODate = "PO_Date";
        public const string YearCode = "YearCode";
        public const string DocName = "DocName";
        public const string ProcessName = "StageDescription";
        public const string ProcessId = "EntryId";
        public const string VendorName = "VendorName";
        public const string DocumentName = "DocumentName";
        public const string desigEntryId = "desigEntryId";
        public const string Designation = "Designation";
        public const string CategoryId = "CategoryId";
        public const string EmpCateg = "EmpCateg";
        public const string ShiftName = "ShiftName";
        public const string ShiftId = "ShiftId";
        //public static string FYEndDate =
        public const string CustomerName = "Account_Name";
        //    new DateTime(DateTime.Today.Year , 3, 31).ToString("dd-MM-yy", new CultureInfo("en-GB"));

        //public static string FYStartDate =
        //    new DateTime(DateTime.Today.Year-1, 4, 1).ToString("dd-MM-yy", new CultureInfo("en-GB"));
        public static DateTime FYEndDate =
           new DateTime(DateTime.Today.Year, 3, 31);

        public static DateTime FYStartDate =
            new DateTime(DateTime.Today.Year - 1, 4, 1);
        public static DateTime ServerDate = DateTime.Now;
    }
}