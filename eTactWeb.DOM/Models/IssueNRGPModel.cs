using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    
    
    [Serializable()]
    public class IssueNRGPDetail : IssueNRGPTaxDetail
    {
        public string PONO { get; set; }
        public int POYearCode { get; set; }
        public string PODate { get; set; }
        public string ItemLocation { get; set; }
        public int POAmendementNo { get; set; }
        public int SEQNo { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int HSNNo { get; set; }
        public bool IC1 { get; set; }
        public string unit { get; set; }
        public float Qty { get; set; }
        public float AltQty { get; set; }
        public string AltUnit { get; set; }
        public float Rate { get; set; }
        public float PurchasePrice { get; set; }
        public string Purpose { get; set; }
        public int Storeid { get; set; }
        public string StoreName { get; set; }
        public float TotalStock { get; set; }
        public float BatchStock { get; set; }
        public string BatchNo { get; set; }
        public string uniquebatchno { get; set; }
        public float discper { get; set; }
        public float disamt { get; set; }
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ItemNetAmount { get; set; }
        public int AgainstChallanEntryId { get; set; }
        public string AgainstChallanNo { get; set; }
        public int AgainstChallanYearCode { get; set; }
        public string AgainstChallanType { get; set; }
        public string RemarkDetail { get; set; }
        public string BatchWise { get; set; }
        public string Stockable { get; set; }
        public string ItemSize { get; set; }
        public string ItemColor { get; set; }
        public string ItemModel { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Closed { get; set; }
        public float PendQty { get; set; }
        public float PendAltQty { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
    }

    public class IssueNRGPTaxDetail : TimeStamp
    {
        public IList<SelectListItem> TaxList { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "TAX", Text = "TAX" },
            new() { Value = "EXPENSES", Text = "EXPENSES" },
        };
        public IList<SelectListItem> YesNo { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };
        public int SeqNo { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalTaxAmt { get; set; }
        public string TxType { get; set; }
        public int TxItemCode { get; set; }
        public string TxPartCode { get; set; }
        public string TxItemName { get; set; }
        public int TxTaxType { get; set; }
        public string? TxTaxTypeName { get; set; }
        public int TxAccountCode { get; set; }
        public string? TxAccountName { get; set; }
        public float TxPercentg { get; set; }
        public string TxRoundOff { get; set; }
        public float TxAmount { get; set; }
        public float TxOnExp { get; set; }
        public string TxRefundable { get; set; }
        public string TxAdInTxable { get; set; }
        public string TxRemark { get; set; }
        public string Active { get; set; }

    }
    [Serializable]
    public class IssueNRGPDashboard
    {
        public string VendorName { get; set; }
        public string ChallanEntryFrom { get; set; }

        
        public string ChallanDate { get; set; }
        public string ChallanNo { get; set; }
        public string RGPNRGP { get; set; }
        public string ChallanType { get; set; }
        public string EntryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string VendorStateCode { get; set; }
        public string Remarks { get; set; }
        public string closed { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string SummaryDetail { get; set; }
        public string partcode { get; set; }
        public string ItemNamePartCode { get; set; }
        public int ItemCode { get; set; }
        public int HSNNO { get; set; }
        public string Store { get; set; }
        public string BatchNo { get; set; }
        public string uniquebatchno { get; set; }
        public float Qty { get; set; }
        public float TotalStock { get; set; }
        public float BatchStock { get; set; }
        public int ProcessId { get; set; }
        public string unit { get; set; }
        public float Rate { get; set; }
        public decimal Amount { get; set; }
        public float PurchasePrice { get; set; }
        public float AltQty { get; set; }
        public string altUnit { get; set; }
        public string StageDescription { get; set; }
        public string ActualEnteredEmp { get; set; }
        public string ActualEntryDate { get; set; }
        public string UpdatedByEmpName { get; set; }
        public string UpdatedDate { get; set; }
        public string MachinName { get; set; }
        public string PONo { get; set; }
        public int PoYear{ get; set; }
        public string PODate { get; set; }
        public int POAmmendNo { get; set; }
        public float discper { get; set; }
        public float discamt { get; set; }
        public string ItemSize { get; set; }
        public string ItemColor { get; set; }
        public string ItemModel { get; set; }
        public int AgainstChallanNoEntryId { get; set; }
        public string AgainstChallanNo { get; set; }
        public int AgainstChallanYearCode { get; set; }
        public string AgainstChallanType { get; set; }
        public float PendQty { get; set; }
        public float PendAltQty { get; set; }
        public int ActualEnteredEMpBy { get; set; }
        public string SalesPersonEmailId { get; set; }
        public string eMailFromCC1 { get; set; }
        public string eMailFromCC2 { get; set; }
        public string eMailFromCC3 { get; set; }
        public List<IssueNRGPDashboard>? INNDashboard { get; set; }
    }

    [Serializable]
    public class INDashboard : IssueNRGPDashboard
    {
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int AccountCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }

    }
    [Serializable]
    public class IssueNRGPModel : IssueNRGPDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string AllowToAddNegativeStockInStore { get; set; }

        public string AllowToChangeStore { get; set; }
        public string EntryTime { get; set; }
        public string IssByEmpCodeName { get; set; }
        public int IssByEmpCode { get; set; }
        public string Prefix { get; set; }
        public string RGPNRGP { get; set; }
        public string ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public string ChallanType { get; set; }
        public int AccountCode { get; set; }
        public bool AC1 { get; set; }
        public string DeliveryAddress { get; set; }
        public string VendorStateCode { get; set; }
        public string GSTType { get; set; }
        public float TotalAmount { get; set; }
        public float NetAmount { get; set; }
        public string Completed { get; set; }
        public string FullyReturned { get; set; }
        public int TotalGSTAmt { get; set; }
        public int FromDepartId { get; set; }
        public string VehicleNo { get; set; }
        public string Remark { get; set; }
        public string TransportMode { get; set; }
        public string RemovalTime { get; set; }
        public int EmpId { get; set; }
        public int Uid { get; set; }
        public string CC { get; set; }
        public string ActualEntryDate { get; set; }
        public int ActualEnteredEMpBy { get; set; }
        public string ActualEnteredEmpByName { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string UpdatedDate { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string UpdatedByEmpName { get; set; }
        public string MachineName { get; set; }
        public bool SH { get; set; }
        public string PurchaseBillNo { get; set; }
        public string PurchaseBillDate { get; set; }
        public int PurchaseBillYearCode { get; set; }
        public string Transporter { get; set; }
        public float Distance { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        public decimal TotalDiscountPercentage { get; set; }
        public string? TotalRoundOff { get; set; }
        public string? FromDateBack { get; set; }
        public string? ToDateBack { get; set; }
        public string? PartCodeBack { get; set; }
        public string? ItemNameBack { get; set; }
        public string? VendorNameBack { get; set; }
        public string? RGPNRGPBack { get; set; }
        public string? ChallanNoBack { get; set; }
        public string? ChallanTypeBack { get; set; }
        public string? DashboardTypeBack { get; set; }
        public IList<IssueNRGPDetail> IssueNRGPDetailGrid { get; set; }
        public IList<IssueNRGPTaxDetail> IssueNRGPTaxGrid { get; set; }
        public IList<TextValue>? ChallanTypeList { get; set; }
        public IList<TextValue>? BranchList { get; set; }
        public IList<TextValue>? VendorList { get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? ProcessList { get; set; }
        public IList<TextValue>? EmployeeList { get; set; }
        public IList<TextValue>? StoreList { get; set; }
        public IList<TextValue>? RecScrapPartCodeList { get; set; }
        public IList<TextValue>? RecScrapItemNameList { get; set; }
    }

}
