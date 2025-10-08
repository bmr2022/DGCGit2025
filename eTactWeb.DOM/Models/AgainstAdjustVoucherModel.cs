using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AgainstAdjustVoucherModel : TimeStamp
    {
        public string? FromDateBack { get; set; }
        public string? ToDateBack { get; set; }
        public string? LedgerNameBack { get; set; }
        public string? VoucherNoBack { get; set; }
        public string? AgainstVoucherRefNoBack { get; set; }
        public string? AgainstVoucherNoBack { get; set; }
        public string? GlobalSearchBack { get; set; }
        public string? DashboardTypeBack { get; set; }
        public int AccEntryId { get; set; }
        public string VoucherType { get; set; }
        public string VoucherNo { get; set; }
        public string SubVoucherName { get; set; }
        public string SubVoucherID { get; set; }
        public string VoucherDate { get; set; }
        public string? DueDate { get; set; }
        public string? BankRECO { get; set; }
        public string Flag { get; set; }
        public int YearCode { get; set; }
        public string SubVoucher { get; set; }
        public int OursalespersonId { get; set; }
        public string Type { get; set; }
        public string ShowAllLedger { get; set; }
        public string LedgerName { get; set; }
        public int SrNO { get; set; }
        public string? ActualDrCr { get; set; }
        //instrument
        public string Ins { get; set; }
        public string? InsNo { get; set; }
        public string InsDate { get; set; }
        public string DRCR { get; set; }
        public decimal Balance { get; set; }
        public string? InVoiceNo { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public string AccountName { get; set; }
        public int AccountCode { get; set; }
        public string? BankType { get; set; }
        public string Intrument { get; set; }
        public string ModeOfAdjustment { get; set; }
        public decimal AdjustmentAmt { get; set; }
        public string? Adjusted { get; set; }
        //other currency
        public decimal AdjustmentAmtOthCur { get; set; }
        public string Mode { get; set; }
        public string Branch { get; set; }
        public int EntryId { get; set; }
        public string EntryDate { get; set; }
        public string Currency { get; set; }
        public string CostCenterName { get; set; }
        public int CostCenterId { get; set; }
        //exchange rate
        public double ExRate { get; set; }
        public string PartyWiseNaration { get; set; }
        public string Naration { get; set; }
        public decimal DrAmt { get; set; }
        public decimal CrAmt { get; set; }
        public double VoucherAmt { get; set; }
        public IList<AgainstAdjustVoucherModel> AgainstAdjustVoucherList { get; set; }
        public List<PopUpDataTableAgainstRef> DataTable { get; set; }
        public int BooktrnsEntryId { get; set; }
        public int DocEntryId { get; set; }
        public string VoucherDocNo { get; set; }
        public string BillVouchNo { get; set; }
        public string VoucherDocDate { get; set; }
        public string BillInvoiceDate { get; set; }
        public int BillYearCode { get; set; }
        public string VoucherRefNo { get; set; }
        public int SeqNo { get; set; }
        public int BankCashAccountCode { get; set; }
        public string AccountGroupType { get; set; }
        public string Description { get; set; }
        public string VoucherRemark { get; set; }
        public string EntryBankCash { get; set; }
        public string? ChequeDate { get; set; }
        public string? ChequeClearDate { get; set; }
        public int UID { get; set; }
        public string CC { get; set; }
        public string TDSNatureOfPayment { get; set; }
        public decimal RoundDr { get; set; }
        public decimal RoundCr { get; set; }
        public string? AgainstVoucherNo { get; set; }
        public string AgainstVoucherDate { get; set; }
        public int AgainstVoucheryearCode { get; set; }
        public string AgainstVoucherType { get; set; }
        public int AgainstVoucherEntryId { get; set; }
        public int AgainstAccOpeningEntryId { get; set; }
        public int AgainstOpeningVoucheryearcode { get; set; }
        public string AgainstVoucherRefNo { get; set; }
        public decimal VoucherBillAmt { get; set; }
        public decimal PendBillAmt { get; set; }
        public string? NewrefNo { get; set; }
        public string AgainstBillno { get; set; }
        public string PONo { get; set; }
        public string PoDate { get; set; }
        public int POYear { get; set; }
        public int SONo { get; set; }
        public string CustOrderNo { get; set; }
        public string SoDate { get; set; }
        public int SOYear { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string Approved { get; set; }
        public string AccountNarration { get; set; }
        public int CurrencyId { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal AdjAmountInOtherCurrency { get; set; }
        public decimal AmountInOtherCurr { get; set; }
        public int ItemCode { get; set; }
        public string ChequePrintAC { get; set; }
        public int EmpCode { get; set; }
        public int DeptCode { get; set; }
        public string MRNO { get; set; }
        public string MRNDate { get; set; }
        public int MRNYearCode { get; set; }
        public string PaymentMode { get; set; }
        public string EntryTypebankcashLedger { get; set; }
        public string TDSApplicable { get; set; }
        public string TDSChallanNo { get; set; }
        public string TDSChallanDate { get; set; }
        public int PreparedByEmpId { get; set; }
        public int CGSTAccountCode { get; set; }
        public decimal CGSTPer { get; set; }
        public decimal CGSTAmt { get; set; }
        public int SGSTAccountCode { get; set; }
        public decimal SGSTPer { get; set; }
        public decimal SGSTAmt { get; set; }
        public int IGSTAccountCode { get; set; }
        public decimal IGSTPer { get; set; }
        public decimal IGSTAmt { get; set; }
        public string NameOnCheque { get; set; }
        public string BalanceSheetClosed { get; set; }
        public string ProjectNo { get; set; }
        public int ProjectYearcode { get; set; }
        public string ProjectDate { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByEmp { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? UpdatedByEmp { get; set; }
        public string EntryByMachine { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Searchbox { get; set; }
        public string? DashboardType { get; set; }
        public decimal PopUpBalance { get; set; }
        public decimal AdjustAmount { get; set; }
        public decimal PopDrAmt { get; set; }
        public decimal PopCrAmt { get; set; }
        public decimal TotalPopDrAmt { get; set; }
        public decimal TotalPopCrAmt { get; set; }
        public decimal PopAdjustmentAmt { get; set; }
        public string? Bank { get; set; }
        public AdjustmentModel? adjustmentModel { get; set; }
        //public List<AdjustmentModel>? AdjAdjustmentDetailGrid { get; set; }
        //public IList<SelectListItem> AdjModeOfAdjustmentList { get; set; } = new List<SelectListItem>()
        //{
        //    new() { Value = "NewRef", Text = "New Ref", Selected = true  },
        //    new() { Value = "AgainstRef", Text = "Against Ref" },
        //    new() { Value = "Advance", Text = "Advance" },
        //};
        //public IList<SelectListItem> AdjDrCrList { get; set; } = new List<SelectListItem>()
        //{
        //    new() { Value = "DR", Text = "DR" },
        //    new() { Value = "CR", Text = "CR", Selected = true },
        //};
        //public IList<SelectListItem> AdjPurchOrderNoList { get; set; } = new List<SelectListItem>()
        //{
        //    new() { Value = "1", Text = "1" },
        //    new() { Value = "2", Text = "2" },
        //};
        //public int? AdjSeqNo { get; set; }
        //public string? AdjModeOfAdjstment { get; set; }
        //public string? AdjModeOfAdjstmentName { get; set; }
        //public string? AdjNewRefNo { get; set; }
        //public string? AdjDescription { get; set; }
        //public string? AdjDrCrName { get; set; }
        //public string? AdjDrCr { get; set; }
        ////[Column(TypeName = "decimal(18, 6)")]
        //public decimal? AdjPendAmt { get; set; }
        //public float? AdjAdjstedAmt { get; set; }
        //public float? AdjTotalAmt { get; set; }
        //public float? AdjRemainingAmt { get; set; }
        //public int? AdjOpenEntryID { get; set; }
        //public int? AdjOpeningYearCode { get; set; }
        //public DateTime? AdjDueDate { get; set; }

        //public string? AdjPurchOrderNo { get; set; }
        //public int? AdjPOYear { get; set; }
        //public DateTime? AdjPODate { get; set; }
        //public string? AdjPageName { get; set; }
        //public int? AdjAgnstAccEntryID { get; set; }
        //public int? AdjAgnstAccYearCode { get; set; }
        //public string? AdjAgnstModeOfAdjstment { get; set; }
        //public string? AdjAgnstModeOfAdjstmentName { get; set; }
        //public string? AdjAgnstNewRefNo { get; set; }
        //public string? AdjAgnstVouchNo { get; set; }
        //public string? AdjAgnstVouchType { get; set; }
        //public string? AdjAgnstDrCrName { get; set; }
        //public string? AdjAgnstDrCr { get; set; }
        //[Column(TypeName = "decimal(18, 6)")]
        //public float? AdjAgnstPendAmt { get; set; }
        //public float? AdjAgnstAdjstedAmt { get; set; }
        //public float? AdjAgnstTotalAmt { get; set; }
        //public float? AdjAgnstRemainingAmt { get; set; }
        //public int? AdjAgnstOpenEntryID { get; set; }
        //public int? AdjAgnstOpeningYearCode { get; set; }
        //public DateTime? AdjAgnstVouchDate { get; set; }
        //public string? AdjAgnstTransType { get; set; }
        //public float TotalBillAmount { get; set; }
        //public float TotalRemainingAmount { get; set; }
        //public float TotalAdjustAmount { get; set; }
        //public float? PendingToAdjustAmount { get; set; }
    }
   
}
