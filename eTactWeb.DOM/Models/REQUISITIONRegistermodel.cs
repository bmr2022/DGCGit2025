using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class REQUISITIONRegistermodel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? ReqType { get; set; }
        public string? ReqNo { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? FromStore { get; set; }
        public string? ToWorkCentrer { get; set; }
        public int ReqYearCode { get; set; }
        public string? ReportMode { get; set; }
        public IList<REQUISITIONRegisterDetail>? REQUISITIONRegisterDetails { get; set; }
    }
}
public class REQUISITIONRegisterDetail
{
   public string REQNo { get; set; }
   public string ReqDate { get; set; }
   public string PartCode { get; set; }
   public string ItemName { get; set; }
   public decimal Qty { get; set;}
   public decimal Rate { get; set;}
   public decimal PendQty { get; set;}
   public decimal AltQty { get; set;}
   public decimal TotalStock { get; set;}
   public string ProdPlanNo { get; set;}
   public string PlanDate { get; set;}
   public string FromWORKCENTER { get; set; }
   public string Remark { get;set; }
   public string Completed { get; set; }
   public string Cancel { get; set; }
   public string FGPartCode { get; set; }
   public string FGItemName { get; set; }
   public decimal FGReqQty { get; set; }
   public decimal FGPendQty { get; set; }
   public decimal AltQtyFGALTQTY { get; set; }
   public decimal FGSTOCK { get; set; }
   public int BOMNO { get; set; }
   public string BOMrevDate { get; set; }
   public string RMPartCode { get; set; }
   public string RMItemName { get; set;}
   public float RMReqQty { get; set; }
   public string RMUnit { get; set; }

}
