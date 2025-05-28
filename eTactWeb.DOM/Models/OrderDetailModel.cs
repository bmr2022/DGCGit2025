using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    [Serializable()]
    public partial class DeliverySchedule
    {
        public int AltQty { get; set; }
        public string? Date { get; set; }
        public int Days { get; set; }
        public int DPartCode { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public string? Remarks { get; set; }
        public int SRNo { get; set; }
        public int TotalQty { get; set; }
    }
    [Serializable()]
    public partial class SaleOrderBillToShipTo
    {
        public int MainCustomerId { get; set; }
        public string MainCustomer { get; set; }
        public int BillToAccountCode { get; set; }
        public string BillToAccountName { get; set; }
        public string BuyerAddress { get; set; }
        public int ShiptoAccountCode { get; set; }
        public string ShiptoAccountName { get; set; }
        public string ShipToAddress { get; set; }
        public int SeqNo { get; set; }
    }

    [Serializable()]
    public class ItemDetail : TaxModel
    {
        public int SOEntryId { get; set; }
        public int SOYearCode { get; set; }
        public decimal AltQty { get; set; }
        public string? AltUnit { get; set; }
        public string? AmendmentDate { get; set; }
        public string? AmendmentNo { get; set; }
        public string? AmendmentReason { get; set; }
        public decimal Amount { get; set; }
        public string? Color { get; set; }
        public IList<DeliverySchedule>? DeliveryScheduleList { get; set; }
        public string? Description { get; set; }
        public decimal DiscPer { get; set; }
        public decimal DiscRs { get; set; }
        public int Excessper { get; set; }
        public decimal ExRate { get; set; }
        public int HSNNo { get; set; }
        public int ItemCode { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ItemNetAmount { get; set; }

        public string? ItemText { get; set; }
        public decimal OtherRateCurr { get; set; }
        public int PartCode { get; set; }
        public string? PartText { get; set; }
        public decimal ProjQty1 { get; set; }
        public decimal ProjQty2 { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Rejper { get; set; }
        public string? Remark { get; set; }
        public int SeqNo { get; set; }
        public decimal StockQty { get; set; }
        public string? StoreName { get; set; }
        public decimal TolLimit { get; set; }
        public string? Unit { get; set; }
        public string? UnitRate { get; set; }
        public string? CustomerSaleOrder { get; set; }
        public string? DeliveryDate { get; set; }
        public string? CustomerLocation { get; set; }
        public string? ItemModel { get; set; }
        public string? CustItemCategory { get; set; }
    }

    [Serializable()]
    public class SaleOrderDashboard
    {
        public string? AmmEffDate { get; set; }
        public int AmmNo { get; set; }
        public string AmendmentDate { get; set; }
        public string AmendmentReason { get; set; }
        public string Color { get; set; }
        public decimal Rejper { get; set; }
        public decimal ProjQty1 { get; set; }
        public decimal ProjQty2 { get; set; }
        public int Excessper { get; set; }
        public string? Approved { get; set; }
        public string? AmmApproved { get; set; }
        public string? ApprovedDate { get; set; }
        public string? CC { get; set; }
        public int HSNNO { get; set; }
        public float Qty{ get; set; }
        public string Unit { get; set; }
        public float AltQty{ get; set; }
        public string AltUnit{ get; set; }
        public decimal Rate{ get; set; }
        public decimal OtherRateCurr{ get; set; }
        public string UnitRate { get; set; }
        public decimal DiscPer { get; set; }
        public decimal Amount { get; set; }
        public decimal TolLimit { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public string StoreName { get; set; }
        public float StockQty{ get; set; }
        public string? Consignee { get; set; }
        public string? ConsigneeAddress { get; set; }
        public string? CreatedOn { get; set; }
        [Required]
        public string? Currency { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerName { get; set; }
        public string? CustOrderNo { get; set; }
        [Required]
        public string? DeliveryAddress { get; set; }
        public string? EID { get; set; }
        public int EntryID { get; set; }
        public string? FromDate { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public string? Mode { get; set; }
        public string? OrderAmt { get; set; }
        public string? OrderNetAmt { get; set; }
        public string? OrderType { get; set; }
        public string? SOCloseDate { get; set; }
        public string? SOComplete { get; set; }
        public string? SOConfirmDate { get; set; }
        public IList<SaleOrderDashboard>? SODashboard { get; set; }
        [Required]
        public string? SOFor { get; set; }
        public int SONo { get; set; }
        public List<TextValue>? SONoList { get; set; }
        public string? SOType { get; set; }
        public string? ToDate { get; set; }
        public string? UpdatedOn { get; set; }
        public string? WEF { get; set; }
        public int Year { get; set; }
        public  string EntryDate { get; set; }
        public  string EntryTime { get; set; }
        public  string SODate { get; set; }
        public  int QuotNo { get; set; }
        public  string QDate { get; set; }
        [Required]
        public  int QuotYear { get; set; }
        public  string Address { get; set; }
        public  string ConsigneeAccountName { get; set; }
        public  string FreightPaidBy { get; set; }
        public  string InsuApplicable { get; set; }
        public  string ModeTransport { get; set; }
        public  string DeliverySch { get; set; }
        public  string PackingChgApplicable { get; set; }
        public  string DeliveryTerms { get; set; }
        public  int PreparedBy { get; set; }
        public  float TotalDiscount { get; set; }
        public  string SODelivery { get; set; }
        public  float TotalDisPercent { get; set; }
        public  float TotalDiscAmt { get; set; }
        public  string DespatchAdviseComplete { get; set; }
        public  string PortToLoading { get; set; }
        public  string PortOfDischarge { get; set; }
        public  int ResposibleSalesPersonID { get; set; }
        public  string CustContactPerson { get; set; }
        public  int SaleDocType { get; set; }
        public  string OtherDetail { get; set; }
        public  string  OrderDelayReason{ get; set; }
        public  int  UID{ get; set; }
        public  string  RoundOff{ get; set; }
        public  int  UpdatedBy{ get; set; }
        public  string  EntryByMachineName{ get; set; }
        public  string  SummaryDetail{ get; set; }
        public IList<TextValue>? BranchList { get; set; }
    }

    [Serializable()]
    public class SaleOrderModel : ItemDetail
    {
        private IList<SelectListItem> _FreightPaidBy = new List<SelectListItem>()
        {
            new() { Value = "By US", Text = "By US" },
            new() { Value = "By Customer", Text = "By Customer" },
        };

        private IList<SelectListItem> _OrderType = new List<SelectListItem>()
        {
            new() { Value = "Domestic", Text = "Domestic" },
            new() { Value = "Export", Text = "Export" },
        };

        //private IList<SelectListItem> _TaxList = new List<SelectListItem>()
        //{
        //    new() { Value = "TAX", Text = "TAX" },
        //    new() { Value = "EXPENSES", Text = "EXPENSES" },
        //};

        private IList<SelectListItem> _TransportMode = new List<SelectListItem>()
        {
            new() { Value = "By Road", Text = "By Road" },
            new() { Value = "By Air", Text = "By Air" },
            new() { Value = "By Ship", Text = "By Ship" },
            new() { Value = "Others", Text = "Others" },
        };

        private IList<SelectListItem> _UnitRate = new List<SelectListItem>()
        {
            new() { Value = "Unit", Text = "Unit" },
            new() { Value = "AltUnit", Text = "AltUnit" },
        };

        private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };
        //[Required(ErrorMessage = "Party Name is required")]
        public int AccountCode { get; set; }
        public IList<TextValue>? AccountList { get; set; }
        public IList<SaleOrderBillToShipTo>? SaleOrderBillToShipTo { get; set; }
        public string? Address { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //[Required(ErrorMessage = "Please Select Amendment Effective Date.")]
        public string? AmmEffDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //[Required]
        public int AmmNo { get; set; }
        public string? TypeOfSave { get; set; }

        public string? Branch { get; set; }
        public IList<TextValue>? BranchList { get; set; }
        public int ConsigneeAccountCode { get; set; }
        public string? ConsigneeAddress { get; set; }
        public int CurrencyID { get; set; }
        public IList<TextValue>? CurrencyList { get; set; }
        public string? CustContactPerson { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }

        //[Required]
        public string? CustOrderNo { get; set; }

        public int DAltQty { get; set; }
        public string? DDate { get; set; }
        public int DDays { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliverySch { get; set; }
        public string? DeliveryTerms { get; set; }
        public int DPartCode { get; set; }
        public int DQty { get; set; }
        public string? DRemark { get; set; }
        public int DTQty { get; set; }
        public string? EntryDate { get; set; }
        public int EntryID { get; set; }
        public int AmmEntryId { get; set; }
        public int AmmYearCode { get; set; }
        public string? EntryTime { get; set; }

        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public int MMainCustomerId { get; set; }
        public int MBillToAccountCode { get; set; }
        public string MBillToAccountName { get; set; }
        public string MBuyerAddress { get; set; }
        public int MShiptoAccountCode { get; set; }
        public string MShipToAccountName { get; set; }
        public string MShipToAddress { get; set; }
        public bool PN1BillTo { get; set; }
        public bool PN1ShipTo { get; set; }

        public IList<SelectListItem> FreightByList
        {
            get => _FreightPaidBy;
            set => _FreightPaidBy = value;
        }

        public string? FreightPaidBy { get; set; }
        public bool IN1 { get; set; }
        public string? InsuApplicable { get; set; }
        //[Required(ErrorMessage = "Item Detail grid is required")]
        public IList<ItemDetail>? ItemDetailGrid { get; set; }
        public IList<TextValue>? ItemNameList { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal NetTotal { get; set; }
        //[Required]
        public string? OrderType { get; set; }

        public IList<SelectListItem> OrderTypeList
        {
            get => _OrderType;
            set => _OrderType = value;
        }

        public string? OtherDetail { get; set; }
        public string? PackingChgApplicable { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public bool PN1 { get; set; }
        public bool PN2 { get; set; }
        public string? Port2Loading { get; set; }
        public string? PortOfDischarge { get; set; }
        public int PreparedBy { get; set; }
        public string PreparedByName { get; set; }
        public IList<TextValue>? PreparedByList { get; set; }
        public string? QDate { get; set; }
        public IList<TextValue>? QuotList { get; set; }
        public string? QuotNo { get; set; }
        public int QuotYear { get; set; }
        public IList<TextValue>? QuotYearList { get; set; }
        public int ResponsibleSalesPersonID { get; set; }
        public IList<TextValue>? ResponsibleSalesPersonList { get; set; }
        public int SaleDocType { get; set; }
        public IList<TextValue>? SaleDocTypeList { get; set; }
        public bool SH { get; set; }
        public string? SOCloseDate { get; set; }
        public string? SOConfirmDate { get; set; }
        public string? SODate { get; set; }
        public string? SODeliveryDate { get; set; }
        //[Required(ErrorMessage = "SO FOR CAN NOT BE BLANK")]
        public string? SOFor { get; set; }
        public IList<TextValue>? SOForList { get; set; }
        public int SOItemCode { get; set; }
        public int SONo { get; set; }
        public string? SORemark { get; set; }
        public string EntryByMachineName { get; set; }
        //[Required(ErrorMessage = "SO FOR CAN NOT BE BLANK")]

        public string? SOType { get; set; }
        public IList<TextValue>? SOTypeList { get; set; }
        public IList<TextValue>? StoreList { get; set; }
        //public IList<TaxModel>? TaxDetailGrid { get; set; }

        //public IList<SelectListItem> TaxList
        //{
        //    get => _TaxList;
        //    set => _TaxList = value;
        //}

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        public decimal TotalDiscountPercentage { get; set; }
        public string? TotalRoundOff { get; set; }
        public string? TransportMode { get; set; }

        public int UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }

        public IList<SelectListItem> TransportModeList
        {
            get => _TransportMode;
            set => _TransportMode = value;
        }

        public IList<SelectListItem> UnitRateList
        {
            get => _UnitRate;
            set => _UnitRate = value;
        }
        //[Required]
        public string? WEF { get; set; }
        public int YearCode { get; set; }

        //public string? TotalDisPercent { get; set; }
        public IList<SelectListItem> YesNoList
        {
            get => _YesNo;
            set => _YesNo = value;
        }
    }


}