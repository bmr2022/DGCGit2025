using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ReOfferItemModel: ReofferItemDetail
    {
       
            public int? ReofferEntryId { get; set; }

            public int? ReofferYearcode { get; set; }

            
            public string? ReofferNo { get; set; }

            public string? ReofferEntrydate { get; set; }

           
            public string? QcType { get; set; }

            public int? MirEntryid { get; set; }

            public string? MIRNo { get; set; }

            public string? MIRDate { get; set; }

            public int? MIRYearCode { get; set; }

           
            public string? MRNNO { get; set; }

            public string? MRNDate { get; set; }

            public int? MrnYearCode { get; set; }

            
            public string? MRNJOBWORK { get; set; }

            
            public string? BillNo { get; set; }

            public string? BillDate { get; set; }

            
            public string? HoldRejrewStatus { get; set; }

            public int? AccountCode { get; set; }
            public string? AccountName { get; set; }

            public int? QCStore { get; set; }

            
            public string? CC { get; set; }

            public int? UID { get; set; }

            public int? EnteredByEmpId { get; set; }
            public string? EnteredByEmpName { get; set; }

            public string? ActualEntryDate { get; set; }

            public int? ActualEnteredBy { get; set; }

            
            public string? EntryByMachineName { get; set; }

            public int? UpdatedBy { get; set; }
            public string? UpdatedByName { get; set; }

            public string? UpdatedOn { get; set; }

        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string SlipNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string BatchNoback { get; set; }
        public string DashboardTypeBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? ReportType { get; set; }
        public string? Searchbox { get; set; }

        public IList<ReofferItemDetail> ReofferItemDetail { get; set; }
        public IList<ReOfferItemModel> reofferdashboard { get; set; }


    }
    public class ReofferItemDetail: TimeStamp
    {
        public int? ReofferEntryId { get; set; }

        public int? ReofferYearcode { get; set; }

        public int? MIREntryId { get; set; }

        public int? MIRYearCode { get; set; }

        public int? SeqNo { get; set; }

        public string? PONo { get; set; }

        public int? POYearCode { get; set; }

        public string? SchNo { get; set; }

        public int? SchYearCode { get; set; }

        public string? MRNno { get; set; }

        public int? mrnYearCode { get; set; }

        public string? MRNJWCUSTJW { get; set; }

        public int? Itemcode { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }

        public string? Unit { get; set; }

        public string? AltUnit { get; set; }

        public decimal? BillQty { get; set; }

        public decimal? RecQty { get; set; }

        public decimal? AltRecQty { get; set; }

        public decimal? PrevAcceptedQty { get; set; }

        public decimal? AcceptedQty { get; set; }

        public decimal? AltAcceptedQty { get; set; }

        public int? OkRecStore { get; set; }
        public string? OkRecStoreName { get; set; }

        public decimal? DeviationQty { get; set; }

        public int? ResponsibleEmpForDeviation { get; set; }

        public decimal? PreviousRejectedQty { get; set; }

        public decimal? RejectedQty { get; set; }

        public decimal? AltRejectedQty { get; set; }

        public int? RejRecStore { get; set; }
        public string? RejRecStoreName { get; set; }

  
        public string? Remarks { get; set; }


        public string? Defaulttype { get; set; }

        public int? ApprovedByEmp { get; set; }
        public string? ApprovedByEmpName { get; set; }

        public decimal? PreviousHoldQty { get; set; }

        public decimal? HoldQty { get; set; }

        public int? HoldStoreId { get; set; }
        public string? HoldStoreName { get; set; }

        public int? ProcessId { get; set; }

        public decimal? PreviousReworkqty { get; set; }

        public decimal? Reworkqty { get; set; }

        public int? RewokStoreId { get; set; }
        public string? RewokStoreName { get; set; }

       
        public string? Color { get; set; }

      
        public string? ItemSize { get; set; }

        
        public string? ResponsibleFactor { get; set; }

       
        public string? SupplierBatchno { get; set; }

        public decimal? shelfLife { get; set; }

        
        public string? BatchNo { get; set; }

       
        public string? uniqueBatchno { get; set; }

     
        public string? AllowDebitNote { get; set; }

        public decimal? Rate { get; set; }

        public decimal? rateinother { get; set; }

        public string? PODate { get; set; }

        public string? FilePath { get; set; }

        public string? schdate { get; set; }
    }

}
