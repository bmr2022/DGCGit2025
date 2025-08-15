using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class AssetsMasterModel
    {
        public string Flag { get; set; }
        public int AssetsEntryId { get; set; }
        public int AccountCode { get; set; }
        public string? EntryDate { get; set; }
        public string AssetsCode { get; set; }
        public string AssetsName { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public int AssetsCateogryId { get; set; }
        public string AssetsCateogryName { get; set; }
        public int ParentAccountCode { get; set; }
        public string ParentAccountName { get; set; }
        public string MainGroup { get; set; }
        public string SubGroup { get; set; }
        public string UnderGroup { get; set; }
        public int SubSubGroup { get; set; }
        public int CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        public int FiscalYear { get; set; }
        public int VendoreAccountCode { get; set; }
        public string PONO { get; set; }
        public string? PODate { get; set; }
        public int POYear { get; set; }
        public string InvoiceNo { get; set; }
        public string? InvoiceDate { get; set; }
        public int InvoiceYearCode { get; set; }
        public decimal NetBookValue { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal ResidualValue { get; set; }
        public string DepreciationMethod { get; set; }
        public string PurchaseNewUsed { get; set; }
        public string CountryOfOrigin { get; set; }
        public string? FirstAqusitionOn { get; set; }
        public decimal OriginalValue { get; set; }
        public string? CapatalizationDate { get; set; }
        public string BarCode { get; set; }
        public string SerialNo { get; set; }
        public string LocationOfInsallation { get; set; }
        public int ForDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Technician { get; set; }
        public string TechnicialcontactNo { get; set; }
        public string TechEmployeeName { get; set; }
        public int CustoidianEmpId { get; set; }
        public string ConsiderInInvetory { get; set; }
        public string InsuranceCompany { get; set; }
        public decimal InsuredAmount { get; set; }
        public string InsuranceDetail { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public int ActualEntryBy { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastupdatedBy { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string EntryByMachine { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public decimal DepreciationRate { get; set; }
        public decimal DepriciationAmt { get; set; }
        public int UseFullLifeInYear { get; set; }
        public int YearCode { get; set; }
        public int ActualEntryByEmpId { get; set; }
        public int ApprovedByEmpId { get; set; }
        public int LastUpdatedbyEmpId { get; set; }
        public string ApprovedByEmpName { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string LastUpdatedbyEmpName { get; set; }
        public string LastupDationDate { get; set; }
        public string Mode { get; set; }
        public string Searchbox { get; set; }
        public IList<AssetsMasterModel> AssetsMasterGrid { get; set; }
    }
}
