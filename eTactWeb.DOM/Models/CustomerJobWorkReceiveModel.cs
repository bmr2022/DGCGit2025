using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CustomerJobWorkReceiveModel : TimeStamp
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? EntryDate { get; set; }
        public string? IPAddress { get; set; }
        public string? ChallanNo { get; set; }
        public string? ChallanDate { get; set; }
        public string? GateNo { get; set; } //ask if its int (declared int in database)
        public string? GateDate { get; set; }
        public string? MRNNo { get; set; } //ask same like gateno
        public int GateYearCode { get; set; }
        public string? AccountCode { get; set; }
        public string? JobWorkType { get; set; }
        public int UID { get; set; }
        public int RecStoreId { get; set; }
        public string? RecStoreName { get; set; }
        public string? CC { get; set; }
        public string? Remark { get; set; }
        public int ReceivedBy { get; set; }
        public string? Complete { get; set; }
        public string? Closed { get; set; }
        public string? QcCheck { get; set; }
        public string? QcCompleted { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int AccountID { get; set; }
        public string? EntryByMachineName { get; set; }
        public string VendorNameBack { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string ChallanNoBack { get; set; }
        public string MRNNoBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public string DashboardTypeBack { get; set; }
        public IList<CustomerJobWorkReceiveDetail>? CustomerJWRGrid { get; set; }
        public IList<TextValue>? EmployeeList { get; set; }
    }
    public class CustomerJobWorkReceiveDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public int RecitemCode { get; set; }//
        public string? RecItemName { get; set; }//
        public string? RecPartCode { get; set; }//
        public float BillQty { get; set; }//
        public float RecQty { get; set; }//
        public string? Unit { get; set; }//
        public float RecAltQty { get; set; }//
        public string? AltUnit { get; set; }//
        public float Rate { get; set; }//
        public float Amount { get; set; }//
        public float ShortExcessQty { get; set; }//
        public string? Remark { get; set; }//
        public string? Purpose { get; set; }//
        public int FinishItemCode { get; set; }//
        public string? FinishItemName { get; set; }//
        public string? FinishPartCode { get; set; }//
        public float FinsihedQty { get; set; }//
        public float PendQty { get; set; }//
        public float RecScrap { get; set; }//
        public float AllowedRejPer { get; set; }//
        public int ProcessId { get; set; }//
        public string? ProcessName { get; set; }//
        public string? Color { get; set; }//
        public string? QcCompleted { get; set; }//
        public string? CustbatchNo { get; set; }
        public string? BatchNo { get; set; }//
        public string? UniqueBatchNo { get; set; }//
        public string? SoNo { get; set; }//
        public int SoYearCode { get; set; }//
        public string? CustOrderno { get; set; }
        public string? SOSchNo { get; set; }//
        public int SOSchYearCode { get; set; }//
        public string? SoDate { get; set; }//
        public string? Schdate { get; set; }//
        public int BomNo { get; set; }//
        public string? BomName { get; set; }//
        public string? BomDate { get; set; }//
        public string? INDBOM { get; set; }//
        public int SeqNo { get; set; }
    }
    public class CustomerJWRQDashboard : CustomerJWRDashboard
    {
        public string? VendorName { get; set; }
        public string? ChallanNo { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? MRNNo { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set;}
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? DashboardType { get; set; }
        public string Searchbox { get; set; }
    }
    public class CustomerJWRDashboard
    {
        public IList<CustomerJWRDashboard>? CustomerJWRQDashboard { get; set; }  
        public string? VendorName { get; set; }
        public string? MrnNo { get; set; }
        public string? MrnDate { get; set; }
        public string? GateNo { get; set; }
        public string? GateDate { get; set; }
        public string? ChallanNo { get; set; }
        public string? ChallanDate { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? EntryBy { get; set; }
        public string? ReceByEmp { get; set; }
        public string? LastUpdatedByEmp { get; set; }
        public string? JobworkType { get; set; }
        public string? CC { get; set; }
        public string? UID { get; set; }
        public int GateYearCode { get; set; }
        public string? Remark { get; set; }
        public string? MRNQCCompleted { get; set; }
        public string? Complete { get; set; }
        public string? Closed { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? UpdatedOn { get; set; }
        public string? EntryByMachineName { get; set; }
        [JsonIgnore]
        public string? RecPartCode { get; set; }                        
        [JsonIgnore]
        public string? RecItemName { get; set; }
        [JsonIgnore]
        public decimal Billqty { get; set; }
        [JsonIgnore]
        public decimal RecQty { get; set; }
        [JsonIgnore]
        public string? Unit { get; set; }
        [JsonIgnore]
        public decimal RecAltQty { get; set; }
        [JsonIgnore]
        public string? AltUnit { get; set; }
        [JsonIgnore]
        public decimal Rate { get; set; }
        [JsonIgnore]
        public decimal Amount { get; set; }
        [JsonIgnore]
        public decimal ShortExcessQty { get; set; }
        [JsonIgnore]
        public string? ItemRemark { get; set; }
        [JsonIgnore]
        public string? Purpose { get; set; }
        [JsonIgnore]
        public int FinishedItemCode { get; set; }
        [JsonIgnore]
        public decimal FinishedQty { get; set; }
        [JsonIgnore]
        public decimal PendQty { get; set; }
        [JsonIgnore]
        public string? RecScrap { get; set; }
        [JsonIgnore]
        public string? AllowedRejPer { get; set; }
        [JsonIgnore]
        public int ProcessId { get; set; }
        [JsonIgnore]
        public string? Color { get; set; }
        [JsonIgnore]
        public string? ItemQcCompleted { get; set; }
        [JsonIgnore]
        public string? CustBatchno { get; set; }
        [JsonIgnore]
        public string? batchno { get; set; }
        [JsonIgnore]
        public string? UniqueBatchNo { get; set; }
        [JsonIgnore]
        public string? SoNo { get; set; }
        [JsonIgnore]
        public int SoYearCode { get; set; }
        [JsonIgnore]
        public string? CustOrderno { get; set; }
        [JsonIgnore]
        public string? SOSchNo { get; set; }
        [JsonIgnore]
        public int SOSchYearCode { get; set; }
        [JsonIgnore]
        public string? SODate { get; set; }
        [JsonIgnore]
        public string? SchDate { get; set; }
        [JsonIgnore]
        public string? bomno { get; set; }
        [JsonIgnore]
        public string? BomName { get; set; }
        [JsonIgnore]
        public string? BomDate { get; set; }
        [JsonIgnore]
        public string? INDBOM { get; set; }
        [JsonIgnore]
        public int FGITEMCODE { get; set; }
        [JsonIgnore]
        public string? FGPartCode { get; set; }
        [JsonIgnore]
        public string? FGItemName { get; set; }

    }
}
