using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class RoutingModel : RoutingDetail
    {
        public int EntryId { get; set; }
        public string EntryDate { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        
        public string RouteNo { get; set; }
        public int RevNo { get; set; }
        public string RevDate { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public string ActualEntryDate { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string LastUpdateDate { get; set; }
        public int LastUpdateBy { get; set; }
        public string LastUpdateByEmpName { get; set; }

        public IList<TextValue>? BranchList { get; set; }
        public IList<TextValue>? StageList { get; set; }
        public IList<TextValue>? EmpList { get; set; }
        public IList<TextValue>? WorkCenterList { get; set; }
        public IList<TextValue>? TransfertoWCList { get; set; }
        public IList<TextValue>? MachineList { get; set; }
        public IList<TextValue>? StoreList { get; set; }
        public IList<RoutingDetail>? RoutingDetailGrid { get; set; }

    }
    public class RoutingDetail : TimeStamp
    {
        public int SequenceNo { get; set; }
        public int RevNo { get; set; }
        public int StageID { get; set; }
        public string Stage { get; set; }
        public int MachineGroupID { get; set; }
        public string MachineGroup { get; set; }
        public int WorkCenterID { get; set; }
        public string WorkCenter { get; set; }
        public int TransferToWCID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string TransferToWC { get; set; }
        public float IntialSetupTime { get; set; }
        public float LeadTime { get; set; }
        public string LeadTimeType { get; set; }
        public decimal LeadTimeInMin { get; set; }
        public int SubItemCode { get; set; }
        public string SubPartCode { get; set; }
        public string SubItemName { get; set; }
        public string Remark { get; set; }
        public string MandatoryOptionalProcess { get; set; }
        public int NoOfWorkers { get; set; }
        public int LaboursCost { get; set; }
        public float ProdCost { get; set; }
        public string QCRequired { get; set; }

    }

    public class RoutingGridDashBoard
    {

        public int EntryId { get; set; }
        public string EntryDate { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int StageID { get; set; }
        public string StageDescription { get; set; }
        public int MachineGroupID { get; set; }
        public string MachGroup { get; set; }
        public int WorkCenterID { get; set; }
        public string WorkCenterDescription { get; set; }
        public int TransferToWCID { get; set; }
        public string TransferWCName { get; set; }
        public float InitialSetupTime { get; set; }
        public float LeadTime { get; set; }
        public string LeadTimeType { get; set; }
        public float LeadTimeInMin { get; set; }
        public int Subitemcode { get; set; }
        public string SubitemName { get; set; }
        public string SubPartCode { get; set; }
        public string Remark { get; set; }
        public string MandatoryOptionalProcess { get; set; }
        public int NoOfWorkers { get; set; }
        public int LaboursCost { get; set; }
        public float ProdCost { get; set; }
        public string RouteNo { get; set; }
        public int RevNo { get; set; }
        public string RevDate { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public string ActualEntryDate { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public int ActualEntryBy { get; set; }
        public string LastUpdateDate { get; set; }
        public string LastUpdatedByEmpName { get; set; }
        public int LatUpdatedBy { get; set; }
        public string SummaryDetail { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public IList<RoutingGridDashBoard>? RoutingGrid { get; set; }


    }
    public class RoutingMainDashboard : RoutingGridDashBoard
    {
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string Stage { get; set; }
        public string WorkCenter { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }


    }

}
