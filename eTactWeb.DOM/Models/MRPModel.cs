using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class MRPMain : MRPDetail
    {
        public int EntryID { get; set; }
        public int YearCode { get; set; }
        public string Entry_Date { get; set; }
        public string MRPNo { get; set; }
        public string MRPDate { get; set; }
        public int MRPRevNo { get; set; }
        public int CreatedByEmpId { get; set; }
        public string MRPREvDate { get; set; }
        public int FirstMonth { get; set; }
        public string EffectiveFromDate { get; set; }
        public string MrpFirstDate { get; set; }
        public string MrpComplete { get; set; }
        public string CreatedByEmpName { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByEmpName { get; set; }
        public string LastUpdatedDate { get; set; }
        public string UID { get; set; }
        public string CC { get; set; }
        public int ActualEnteredBy { get; set; }
        public string ActualEnteredByName { get; set; }
        public string ActualEnteredDate { get; set; }
        public string MachineNo { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }

        public List<MRPDetail>? MRPGrid { get; set; }
        public List<MRPFDRMDetail>? MRPFGRMGrid { get; set; }
        public List<MRPSaleOrderDetail>? MRPSOGrid { get; set; }
    }
    public class MRPDetail : MRPFDRMDetail
    {
        public int MRPEntryId { get; set; }
        public string MRPNo { get; set; }
        public int Month { get; set; }
        public int ForMonthYear { get; set; }
        public int FGItemCode { get; set; }
        public string FGPartCode { get; set; }
        public string FGItemName { get; set; }
        public int SeqNo { get; set; }
        public int RMItemCode { get; set; }
        public string RMPartCode { get; set; }
        public string RMItemName { get; set; }
        public float OrderQtyInclPrevPOQty { get; set; }
        public float CurrMonthQty { get; set; }
        public float IIndMonthQty { get; set; }
        public float IIrdMonthQty { get; set; }
        public float StoreStock { get; set; }
        public float WIPStock { get; set; }
        public float TotalStock { get; set; }
        public float MinLevel { get; set; }
        public float RecorderLvl { get; set; }
        public float PrevOrderQty { get; set; }
        public float OrderQty { get; set; }
        public float ReqQty { get; set; }
        public float IIndMonthActualReqQty { get; set; }
        public float IIrdMonthActualReqQty { get; set; }
        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public float AllocatedQty { get; set; }
        public float ConsumedQty { get; set; }
        public float PORate { get; set; }
        public int LeadTime { get; set; }
        public string BOMExist { get; set; }
        public string POExist { get; set; }
        public float MaxPORate { get; set; }
        public int AccountCode { get; set; }
        public string AccountName { get; set; }
    }

    public class MRPFDRMDetail : TimeStamp
    {
        public int FGItemCode { get; set; }
        public string FGPartCode { get; set; }
        public string FGItemName { get; set; }
        public int SeqNo { get; set; }
        public int RMItemCode { get; set; }
        public string RMPartCode { get; set; }
        public string RMItemName { get; set; }
        public int BOMNo { get; set; }
        public float CurrMonthOrderQty { get; set; }
        public float OrderQtyInclPrevPOQty { get; set; }
        public string AltUnit { get; set; }
        public float AllocatedQty { get; set; }
        public float FGQty { get; set; }
        public float BOMQty { get; set; }
        public string BOMEffDate { get; set; }
    }

    [Serializable]
    public class MRPSaleOrderDetail
    {
        public int SeqNo { get; set; }
        public string SONo { get; set; }
        public int SOYearCode { get; set; }
        public string SODAte { get; set; }
        public string ScheduleNo { get; set; }
        public int SchYearCode { get; set; }
        public int ProjNo { get; set; }
        public int ProjYearCode { get; set; }
        public int AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Months { get; set; }
        public int MonthYear { get; set; }
        public string BOMExist { get; set; }
    }

    public class MRPDeatilDashborad
    {
        public int EntryID { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string MRPNo { get; set; }
        public string MRPDate { get; set; }
        public int MRPRevNo { get; set; }
        public int CreatedByEmpId { get; set; }
        public string MRPREvDate { get; set; }
        public int ForMonth { get; set; }
        public string EffectiveFromDate { get; set; }
        public string MrpFirstDate { get; set; }
        public string MrpComplete { get; set; }
        public string UID { get; set; }
        public string CC { get; set; }
        public string ActualEntryDate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string ActualEnteredByName { get; set; }
        public string EntryByMachineName { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdatedDate { get; set; }

    }

    public class MRPDashboard : MRPDeatilDashborad
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<MRPDashboard>? MRPDahboardGrid { get; set; }
    }

}

