using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ProductionScheduleModel : ProductionScheduleDetail
    {
        public int EntryID { get; set; }
        public int YearCode { get; set; }
        public string EntryDate{ get; set; }
        public int WCID{ get; set; }
        public string ProdSchNo{ get; set; }
        public string ProdSchDate{ get; set; }
        public string EffectiveFrom{ get; set; }
        public string EffectiveTill{ get; set; }
        public string EffectiveTo{ get; set; }
        public int PlanForNoOFDays { get; set; }
        public int RevNo{ get; set; }
        public string RevDate{ get; set; }
        public string CC{ get; set; }
        public int UID{ get; set; }
        public string ActualEntryDate{ get; set; }
        public int ActualEntryBy{ get; set; }
        public string ActualEntryByName{ get; set; }
        public string LastUpdatedDate{ get; set; }
        public int LastUpdatedBy{ get; set; }
        public string LastUpdatedByName{ get; set; }
        public string EntryByMachineName{ get; set; }
        public string Closed{ get; set; }
        public string Completed{ get; set; }
        public int ForTheMonth{ get; set; }
        public string Remark{ get; set; }
        public string FinFromDate{ get; set; }
        public string FinToDate{ get; set; }
        public string ShowWOWithOrWOItem{ get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string AccountNameBack { get; set; }
        public string ProdSchNoBack { get; set; }
        public string WONOBack { get; set; }
        public string SummaryDetailBack { get; set; }
        public string SearchBoxBack { get; set; }
        public List<ProductionScheduleDetail> ProductionScheduleDetails { get; set; }
        public List<ProductionScheduleProdPlanDetail> prodPlanDetails{ get; set; }
        public ProductionScheduleBOMData BomDatamodel { get; set; } 
    }


    public class ProductionScheduleDetail : TimeStamp
    {
        public int ItemCode { get; set; }
        public string PartCode { get; set; }    
        public string ItemName { get; set; }
        public int BOMNO { get; set; }
        public string BomEffDate { get; set; }
        public string SchDate { get; set; }
        public int ShiftID { get; set; }
        public int ProdInWC { get; set; }
        public float Qty { get; set; }
        public float ProdPendQty { get; set; }
        public float Originalqty { get; set; }
        public float TotalWOQty { get; set; }
        public int PlannedMachineid1 { get; set; }
        public int PlannedMachineid2 { get; set; }
        public int PlannedMachineid3 { get; set; }
        public string StartFromTime { get; set; }
        public string ToTime { get; set; }
        public string WONo { get; set; }
        public int WOEntryId { get; set; }
        public int WOYearCode { get; set; }
        public string WODate { get; set; }
        public int SOEntryId { get; set; }
        public string SONo { get; set; }
        public string CustOrderNo { get; set; }
        public int SOYearCode { get; set; }
        public string SODate { get; set; }
        public string SubBOM { get; set; }
        public string InhouseJOBProd { get; set; }
        public string DrawingNo { get; set; }
        public string RemarkItem { get; set; }
        public string ProdCompleted { get; set; }
        public string ProdCanceled { get; set; }
        public string QCMandatory { get; set; }
        public int ProdSeq { get; set; }
        public int AccountCode { get; set; }
        public string AccountName { get; set; }
    }

    //public class ProductionScheduleDashboard
    //{
    //    public string 
    //}

    public class ProductionScheduleProdPlanDetail
    {
        public int EntryId { get; set;}
        public int YearCode { get; set;}
        public string ProdSchNo { get; set;}
        public string PlanNo { get; set;}
        public int PlanNoEntryId { get; set;}
        public int PlanNoYearCode { get; set;}
        public string PlanNoDate{ get; set;}
        public string SONO { get; set;}
        public string CustOrderNo{ get; set;}
        public int SOEntryId { get; set;}
        public int SOYearCode { get; set;}
        public int AccountCode { get; set;}
        public string WOEffectiveFrom { get; set;}
        public string WOEndDate{ get; set;}
        public int SeqNo{ get; set;}
        public string SaleSchNo{ get; set;}
        public int SaleSchEntryId{ get; set;}
        public int SaleSchYearCode{ get; set;}
        public string SaleSchDate{ get; set;}
    }

    public class ProductionScheduleDashboard : ProductionScheduleModel
    {
        public string SummaryDetail { get; set; }
        public string SearchBox { get; set; }
        public List<ProductionScheduleDashboard> productionScheduleDashboards { get; set; }
    }
    // FillBOMCHILDPART
    public class ProductionScheduleBomDetail
    {
        public int FGItemCode { get; set; }
        public string FGPartCode { get; set; }
        public string FGItemName { get; set; }
        public float FGQty { get; set; }
        public int RMItemCode { get; set; }
        public string RMItemName { get; set; }
        public string RMPartCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string Unit{ get; set; }
        public float ReqQty{ get; set; }
        public float PendQty{ get; set; }
        public int BomNo{ get; set; }
        public string BomEffDate{ get; set; }
        public float BomQty{ get; set; }
    }
    public class ProductionScheduleBomSummary
    {
        public int RMItemCode { get; set; }
        public string RMItemName { get; set; }
        public string RMPartCode { get; set; }
        public string Unit { get; set; }
        public float TotalReqQty { get; set; }
        public float PendQty { get; set; }
        public float MainStoreStock { get; set; }
        public float QcStoreStock { get; set; }
        public float WIPStock { get; set; }
    }
    public class ProductionScheduleBOMData
    {
        public List<ProductionScheduleBomSummary> BomSummaries { get; set; }
        public List<ProductionScheduleBomDetail> BomDetails { get; set; }
    }

    public class ProductionScheduleInputData
    {
        public int PlanNo { get; set; }
        public int PlanYearCode { get; set; }
        public int PlanEntryId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOrderNo { get; set; }
        public float FGStock { get; set; }
        public string SchDate { get; set; }
        public string SODate { get; set; }
        public string SONO { get; set; }
        public string SOYearCode { get; set; }
        public string SchNo { get; set; }
        public string SchYearCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public float PendForProd { get; set; }
        public float PendPlanQty { get; set; }
        public float PlanQty { get; set; }
        public float PlanDate { get; set; }
        public float PrevProdSchQty { get; set; }
        public float TotalProdQty { get; set; }
        public float WIPStock { get; set; }
    }
}
