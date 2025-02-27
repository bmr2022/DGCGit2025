using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class SaleBillDashboard : SaleBillModel
    {
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? Searchbox { get; set; }
        public string? SummaryDetail { get; set; }
        public string? Currency { get; set; }
        public List<SaleBillDashboard>? SaleBillDataDashboard { get; set; }
    }
    public class SaleBillDetail : TaxModel
    {
        public AdjustmentModel? adjustmentModel { get; set; }
        //public int SaleBillEntryId { get; set; }
        //public int SaleBillYearCode { get; set; }
        public int SeqNo { get; set; }
        public string? SONO { get; set; }
        public string? CustOrderNo { get; set; }
        public int SOYearCode { get; set; }
        public string? SODate { get; set; }
        public string? SchNo { get; set; }
        public string? Schdate { get; set; }
        public int SaleSchYearCode { get; set; }
        public string? SOAmendNo { get; set; }
        public string? SOAmendDate { get; set; }
        public int? SchAmendNo { get; set; }
        public string? SchAmendDate { get; set; }
        public int ItemCode { get; set; }
        public bool ItemSA { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public int HSNNo { get; set; }
        public string? Unit { get; set; }
        public float NoofCase { get; set; }
        public float Qty { get; set; }
        public string? UnitOfRate { get; set; }
        public float RateInOtherCurr { get; set; }
        public float Rate { get; set; }
        public string? AltUnit { get; set; }
        public float AltQty { get; set; }
        public float ItemWeight { get; set; }
        public int NoofPcs { get; set; }
        public string? CustomerPartCode { get; set; }
        public float MRP { get; set; }
        public float OriginalMRP { get; set; }
        public float SOPendQty { get; set; }
        public int AltSOPendQty { get; set; }
        public float DiscountPer { get; set; }
        public decimal DiscountAmt { get; set; }
        public string? ItemSize { get; set; }
        public string? Itemcolor { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        //public float ItemAmount { get; set; }
        public decimal Amount { get; set; }
        public string? AdviceNo { get; set; }
        public int AdviseEntryId { get; set; }
        public int AdviceYearCode { get; set; }
        public string? AdviseDate { get; set; }
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? Batchno { get; set; }
        public string? Uniquebatchno { get; set; }
        public float LotStock { get; set; }
        public float TotalStock { get; set; }
        public string? AgainstProdPlanNo { get; set; }
        public int AgainstProdPlanYearCode { get; set; }
        public string? AgaisntProdPlanDate { get; set; }
        public float? GSTPer { get; set; }
        public string? GSTType { get; set; }
        public string? PacketsDetail { get; set; }
        public string? OtherDetail { get; set; }
        public string? ItemRemark { get; set; }
        public bool SH { get; set; }
        public string? ProdSchno { get; set; }
        public int CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public int ProdSchYearcode { get; set; }
        public int ProdSchEntryId { get; set; }
        public string? ProdSchDate { get; set; }
        public string? SchdeliveryDate { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal ItemNetAmount { get; set; }
    }
    public class SaleBillModel : SaleBillDetail
    {
      

        public int SaleBillEntryId { get; set; }
        public int SaleBillYearCode { get; set; }
        public string? SaleBillEntryDate { get; set; }
        public string? InvPrefix { get; set; }
        public string? SaleBillJobwork { get; set; }
        public string? SaleBillNo { get; set; }
        public string? SaleBillDate { get; set; }
        public string? InvoiceTime { get; set; }
        public string? ExportInvoiceNo { get; set; }
        public string? PerformaInvNo { get; set; }
        public string? PerformaInvDate { get; set; }
        public int PerformaInvYearCode { get; set; }

        public string? AgstInvNo { get; set; }
        public string? AgstInvDate { get; set; }
        public int AgstInvYearCode { get; set; }


        public string? BILLAgainstWarrenty { get; set; }
        public string? RemovalDate { get; set; }
        public string? RemovalTime { get; set; }
        public bool PN1 { get; set; }
        public bool DT { get; set; }
        public int AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? GSTNO { get; set; }
        public string? DomesticExportNEPZ { get; set; }
        public string? SupplyType { get; set; }
        public string? CustAddress { get; set; }
        public string? StateNameofSupply { get; set; }
        public string? StateCode { get; set; }
        public string? CityofSupply { get; set; }
        public string? CountryOfSupply { get; set; }
        public decimal DistanceKM { get; set; }
        public string? vehicleNo { get; set; }
        public string? TransporterName { get; set; }
        public string? TransporterdocNo { get; set; }
        public int TransporterId { get; set; }
        public string? TransportModeBYRoadAIR { get; set; }
        public int ConsigneeAccountcode { get; set; }
        public bool PNConsingee { get; set; }
        public string? ConsigneeAccountName { get; set; }
        public string? ConsigneeAddress { get; set; }
        public string? DispatchTo { get; set; }
        public string? DispatchThrough { get; set; }
        public int DocTypeAccountCode { get; set; }
        public int DocTypeAccountName { get; set; }
        public int PaymentTerm { get; set; }
        public decimal BillAmt { get; set; }
        public string? BillAmtWord { get; set; }
        public decimal TaxableAmt { get; set; }
        public string? TaxbaleAmtInWord { get; set; }
        public decimal GSTAmount { get; set; }
        public string? RoundTypea { get; set; }
        public decimal RoundOffAmt { get; set; }
        public decimal DiscountPercent { get; set; }
        // public decimal DiscountAmt { get; set; }
        public decimal INVNetAmt { get; set; }
        public string? NetAmtInWords { get; set; }
        public string? Remark { get; set; }
        public string? PermitNo { get; set; }
        public decimal CashDisPer { get; set; }
        public decimal CashDisRs { get; set; }
        public string? SoDelTime { get; set; }
        public string? TypeJob { get; set; }
        public string? Approved { get; set; }
        public string? ApprovDate { get; set; }
        public int ApprovedBy { get; set; }
        public int currencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public string? TypeItemServAssets { get; set; }
        public string? Shippingdate { get; set; }
        public string? CancelBill { get; set; }
        public string? Canceldate { get; set; }
        public int CancelBy { get; set; }
        public string? Cancelreason { get; set; }
        public string? BankName { get; set; }
        public string? Ewaybillno { get; set; }
        public string? FreightPaid { get; set; }
        public string? dispatchLocation { get; set; }
        public decimal currExchangeRate { get; set; }
        public string? DispatchDelayReason { get; set; }
        public string? AttachmentFilePath1 { get; set; }
        public IFormFile? AttachmentFile1 { get; set; }
        public string? AttachmentFilePath2 { get; set; }
        public IFormFile? AttachmentFile2 { get; set; }
        public string? AttachmentFilePath3 { get; set; }
        public IFormFile? AttachmentFile3 { get; set; }
        public string? DocketNo { get; set; }
        public string? DispatchDelayreson { get; set; }
        public string? Commodity { get; set; }
        public string? EInvNo { get; set; }
        public string? EinvGenerated { get; set; }
        public string? CC { get; set; }
        public int Uid { get; set; }
        public int EntryByempId { get; set; }
        public string? MachineName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string? ActualEnteredByName { get; set; }
        public int? LastUpdatedBy { get; set; } // Nullable
        public string? LastUpdatedByName { get; set; } // Nullable
        public string? LastUpdationDate { get; set; } // Nullable
        public string? EntryFreezToAccounts { get; set; }
        public int PaymentCreditDay { get; set; }
        public string? ChallanNo { get; set; }
        public string? ChallanDate { get; set; }
        public int ChallanYearCode { get; set; }
        public int ChallanEntryid { get; set; }
        public string? BalanceSheetClosed { get; set; }
        public string? SaleQuotNo { get; set; }
        public int? SaleQuotEntryID { get; set; } // Nullable
        public int? SaleQuotyearCode { get; set; } // Nullable
        public string? SaleQuotDate { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalDiscountPercentage { get; set; }

        public string? TotalRoundOff { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalRoundOffAmt { get; set; }

        public string? DashboardTypeBack { get; set; }
        public string? FromDateBack { get; set; }
        public string? ToDateBack { get; set; }
        public string? PartCodeBack { get; set; }
        public string? ItemNameBack { get; set; }
        public string? SaleBillNoBack { get; set; }
        public string? CustNameBack { get; set; }
        public string? SonoBack { get; set; }
        public string? CustOrderNoBack { get; set; }
        public string? SchNoBack { get; set; }
        public string? PerformaInvNoBack { get; set; }
        public string? SaleQuoteNoBack { get; set; }
        public string? DomesticExportNEPZBack { get; set; }
        public string? SearchBoxBack { get; set; }
        public string? SummaryDetailBack { get; set; }
        public List<SaleBillDetail>? saleBillDetails { get; set; }
        public IList<SaleBillDetail>? ItemDetailGrid { get; set; }
        public IList<DPBItemDetail>? DPBItemDetails { get; set; }

        private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };

        public IList<SelectListItem> YesNoList
        {
            get => _YesNo;
            set => _YesNo = value;
        }
    }

    public class CustomerJobWorkIssueAdjustDetail
    {
        public int ItemCode { get; set; }
        public string Unit { get; set; }
        public int BillQty { get; set; }
        public int JWRate { get; set; }
        public int ProcessId { get; set; }
        public string SONO { get; set; }
        public string CustOrderNo { get; set; }
        public int SOYearCode { get; set; }
        public string SchNo { get; set; }
        public int SchYearcode { get; set; }
        public int BOMIND { get; set; }
        public string BOMNO { get; set; }
        public string BOMEffDate { get; set; }
        public string Produnprod { get; set; }
        public string fromChallanOrSalebill { get; set; }
        public bool ItemAdjustmentRequired { get; set; }
    }
}
