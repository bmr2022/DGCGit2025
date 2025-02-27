using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public  class ProdPlanStatusModel
    {
        public string Flag { get; set; }
        public string Mode { get; set; }
        public DateTime? FromPlanDate { get; set; }
        public DateTime? ToPlanDate { get; set; }
        public string ProdPlanNo { get; set; }
        public string CustomerName { get; set; }
        public string FGPartCode { get; set; }
        public string FGItemName { get; set; }
        public string ProdSchNo { get; set; }
        public string prodEntryNo { get; set; }
        public string QcSlipNo { get; set; }

        // ProdPlanDetail Table
        public long PlanNoEntryId { get; set; }
        public long PlanNoYearCode { get; set; }
        public string ProdPlanNoDetail { get; set; }
        public DateTime ProdPlanDate { get; set; }
        public long AccountCode { get; set; }
        public string SONO { get; set; }
        public DateTime? SODATE { get; set; }
        public DateTime? SOCloseDate { get; set; }
       
        public string CustomerOrderNo { get; set; }
        public string SchNo { get; set; }
        public DateTime? SCHDATE { get; set; }
        public long ItemCode { get; set; }
        public string OrderQty { get; set; }
        public string ProdPlanDeactive { get; set; }
        public string OrderType { get; set; }
        public double ProdPlanQty { get; set; }
        public long WCID { get; set; }
        public string DrawingNo { get; set; }
        public double WIPStock { get; set; }
        public string ProdInst1 { get; set; }
        public string ProdInst2 { get; set; }
        public long FGStoreId { get; set; }
        public long BomNo { get; set; }
        public DateTime? BomEffectiveDate { get; set; }
        public double ProdPlanPendQty { get; set; }
        public string DeactivateWo { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public DateTime? DODeactivate { get; set; }
        public string CloseWo { get; set; }
        public string ProdPlanClosed { get; set; }
        public DateTime? CloseDate { get; set; }
        public string WoStatus { get; set; }
        public string PlanStatus { get; set; }
        public string RemarkForProduction { get; set; }
        public string OtherInstruction { get; set; }

        // ProductionScheduleItemQty Table
        public long ProdSchEntryId { get; set; }
        public long ProdSchYearCode { get; set; }
        public double ProdSchQty { get; set; }
        public double ProdQty { get; set; }
        public string ProdSchNoDetail { get; set; }
        public DateTime? ProdSchDate { get; set; }
        public double SaleOrderQty { get; set; }

        // ProductionEntry Table
        public string NewProdRework { get; set; }
        public long FGItemCode { get; set; }
        public double FGProdQty { get; set; }
        public double FGOKQty { get; set; }

        // InProcessQC Table
        public double OKProdQty { get; set; }
        public double RejQty { get; set; }
        public double PendForQc { get; set; }
        public double RewQty { get; set; }

        // Output Fields
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string PlanNo { get; set; }
        public double PendQtyForProd { get; set; }
        public double QCOkQty { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public IList<ProdPlanStatusModel> ProdPlanStatusGrid { get; set; }
    }
}
