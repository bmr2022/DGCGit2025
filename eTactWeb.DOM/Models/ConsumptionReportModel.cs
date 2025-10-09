using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ConsumptionReportModel: TimeStamp
    {
        public int Itemcode { get; set; }
        public string FGPartCode { get; set; }
        public string ProdDate { get; set; }
        public string FGunit { get; set; }
        public string FGItemName { get; set; }
        public string RMPartCode { get; set; }
        public string RMItemName { get; set; }
        public string StoreName { get; set; }
        public int Storeid { get; set; }
        public string WorkCenterName { get; set; }
        public int WorkCenterId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal OpeningWIP { get; set; }
        public string Unit { get; set; }
        public decimal POQty { get; set; }
        public decimal AltPOQty { get; set; }
        public string AltUnit { get; set; }
        public decimal ConsumedRMQty { get; set; }
        public string ConsumedRMUnit { get; set; }
        public decimal FGOKQty { get; set; }
        public decimal FGProdQty { get; set; }
        public decimal ClosingStock { get; set; }
        public decimal WIPClosingStock { get; set; }
        public int RMItemCode { get; set; }
        public string ReportType { get; set; }
        public int YearCode { get; set; }
        public decimal RecFromMRN { get; set; }
        public decimal RecFromJW { get; set; }
        public decimal ReturnFromShopFloor { get; set; }
        public decimal RecMaterialConv { get; set; }
        public decimal InterStoreTransfer { get; set; }
        public decimal OtherRec { get; set; }
        public decimal TotalRec { get; set; }
        public decimal IssuedToShopFloor { get; set; }
        public decimal IssuedViaChallan { get; set; }
        public decimal IssMaterialConv { get; set; }
        public decimal ReturnViaRejection { get; set; }
        public decimal OtherIssue { get; set; }
        public string PONo { get; set; }
        public string PODate { get; set; }
        public int AccountCode { get; set; }
        public string VendorName { get; set; }
        public string SchNo { get; set; }
        public string SchDate { get; set; }

        public decimal IssuedToShopFloorAgstPlan { get; set; }
        public decimal IssuedToShopFloorAgstBOMReq { get; set; }
        public decimal IssuedToShopFloorAgstIndReq { get; set; }
        public decimal IssFromInterStoreTransf { get; set; }
        public decimal IssViaMaterialConv { get; set; }
        public decimal IssStockAdjustment { get; set; }
        public decimal IssueOther { get; set; }
        public decimal TotalIssue { get; set; }
        public IList<ConsumptionReportModel>? ConsumptionReportGrid { get; set; }
    }
}
