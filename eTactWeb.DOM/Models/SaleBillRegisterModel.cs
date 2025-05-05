using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class SaleBillRegisterModel
    {

        public string? FromDate { get; set; }
        public string? ReportType { get; set; }
        public string? ToDate { get; set;} 
         public string? PartCode { get; set; }
        public string? ItemName { get; set; }
         public string? ReportMode { get; set; }
        public string? SaleBillNo {  get; set; }
        public string? docname { get; set; }
        public string? SONo { get; set; }
        public string? Schno { get; set; }
        public string? GSTNO { get; set; }
        public string? HSNNO { get; set; }

        public string? CustomerName { get; set; }
        public IList<SaleBillRegisterDetail>? SaleBillRegisterDetail { get; set; }
    }
    public class SaleBillRegisterDetail
    {
        public string? Description {  get; set; }
        public decimal? forTheDuration { get; set; }
        public decimal? ForFinYear { get; set; }

        public decimal TotStk { get; set; }
        public int SeqNo { get; set; }
        public string? SaleBillNo { get; set; }
        public string? SaleBillDate { get; set; }
        public string? EntryDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustAddress { get; set; }
        public string? State { get; set; }
        public string? StateCode { get; set; }
        public string? Country { get; set; }
        public string? ConsigneeName { get; set; }
        public string? ConsigneeAddress { get; set; }
        public decimal? SoldQty { get; set; }


        public string? PartCode { get; set; }
        public string? ReportType { get; set; }
        public string? ItemName { get; set; }
        public string? SONo { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? GSTNO { get; set; }
        public string? SchNo { get; set; }
        public string? ItemRemark { get; set; }

        
        public decimal? BillAmt { get; set;}
        public decimal? TaxableAmt { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? TotalBillQty { get; set; }
        public string? Currency { get; set; }
        public decimal? TotalDisAmt { get; set; }
        public decimal? TotalItemAmt { get; set; }
        public decimal? CGSTPer { get; set; }
        public decimal? CGSTAmt { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal? SGSTAmt { get; set; }

        public decimal? IGSTPer { get; set; }
        public decimal? IGSTAmt { get; set; }

        public decimal? ExpenseAmt { get; set; }
        public decimal? InvAmt { get; set; }  
        public string? TypeItemServAssets { get; set; }
        public string? DomesticExportNEPZ { get; set; }
        public string? SupplyType { get; set; }
        public string? LastUpdatedByEmp { get; set; }
        public string? LastUpdationDate { get; set; }
         
        public Int16? EntryId { get; set; }
        public Int16? YearCode { get; set; }
        public string? Unit { get; set; }
        public decimal? Rate { get; set;}
        public decimal? AltQty { get; set; }
        public decimal? PendPOQty { get; set; }
        public decimal? AltPendQty { get; set; }
        public int? SaleBillYearCode { get; set; }
        public decimal? SaleBillQty { get; set; }
 
        public string? ItemSize { get; set; }
        public string? ItemColor { get; set; }
        public string? BatchNo { get; set; }
        public string? SOtype { get; set; }
        public string? Remark { get; set; }
        public string? PreparedByEmp { get; set; }
        //public string? ActualEntryByEmp { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? LastUpdatedby { get; set; }
        public string? Updationdate { get; set; }

        public string? CompanyAddress { get; set; }
        public string? City { get; set; }
        public string? CC { get; set; }
        public int SaleBillEntryId { get; set; }
        public int SOYearCode { get; set; }
        public int SchYearCode { get; set; }

        public decimal Total { get; set; } 
        public int SeqNum { get; set; }
        public string? FromDate { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? ToDate { get; set; }
        public decimal? Amount { get; set; }
        public string? ActualEntryByEMp {  get; set; }
        public string? UpdatedByEMp { get; set; }
        public string? AltUnit { get; set; }
        public string? Ewaybillno {  get; set; }
        public string? EInvNo { get; set; }
        public string? EinvGenerated {  get; set; }
        public string? CancelBill { get; set; }
        public string? Canceldate { get; set; }
        public string? CancelReason { get; set; }
        public string? CostcenterName { get; set; }
        public string? HSNNO { get; set; }
        public decimal? Amout { get; set; }
        public decimal? AprAmt { get; set; }
        public decimal? MayAmt { get; set; }
        public decimal? JunAmt { get; set; }
        public decimal? JulAmt { get; set; }
        public decimal? AugAmt { get; set; }
        public decimal? SepAmt { get; set; }
        public decimal? OctAmt { get; set; }
        public decimal? NovAmt { get; set; }
        public decimal? DecAmt { get; set; }
        public decimal? JanAmt { get; set; }
        public decimal? FebAmt { get; set; }
        public decimal? MarAmt { get; set; }
        public decimal? AprQty { get; set; }
        public decimal? MayQty { get; set; }
        public decimal? JunQty { get; set; }
        public decimal? JulQty { get; set; }
        public decimal? AugQty { get; set; }
        public decimal? SepQty { get; set; }
        public decimal? OctQty { get; set; }
        public decimal? NovQty { get; set; }
        public decimal? DecQty { get; set; }
        public decimal? JanQty { get; set; }
        public decimal? FebQty { get; set; }
        public decimal? MarQty { get; set; }
        public string? MonthName { get; set; }
        public string? SalesPersonName { get; set; }
        public string? Approved { get; set; }
        public string? CountryOfSupply { get; set; }
        public string? DispatchDelayReason { get; set; }
        public string? DispatchThrough { get; set; }
        public decimal? DistanceKM { get; set; }
        public string? BankName { get; set; }
        public string? Commodity { get; set; }
        public string? CustOrderNo { get; set; }
        public string? SODate { get; set; }
        public string? CustomerPartCode { get; set; }
        public string? UniqueBatchno { get; set; }
        public string? FromStore { get; set; }
    }
}
