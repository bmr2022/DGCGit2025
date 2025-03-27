using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class MaterialConversionModel:TimeStamp
    {

        public int EntryId { get; set; }
        public int SrNO { get; set; }
        public string EntryDate { get; set; }
        public int OpeningYearCode { get; set; }
        public string SlipNo { get; set; }
        public string SlipDate { get; set; }
        public string StoreName { get; set; }
        public int StoreId { get; set; }
        public int WcId { get; set; }
        public string IssueToStoreWC { get; set; }
        public string WorkCenterName { get; set; }
        public string AltWorkCenterName { get; set; }
        public string WONO { get; set; }
        public int WOYear { get; set; }
        public string RsNo { get; set; }
        public string RsDescription{ get; set; }
        public int RsYear { get; set; }
        public string ParentRsNo { get; set; }
        public string ParentDescription { get; set; }
        public string OriginalPartCode { get; set; }
        public string OriginalItemName { get; set; }
        public bool ShowAllItem { get; set; }
        public string Unit { get; set; }
        public string FromStore { get; set; }
        public float StockQty { get; set; }
        public float Qty { get; set; }
        public float Rate { get; set; }
        public string AltPartCode { get; set; }
        public string AltItemName { get; set; }
        public bool ShowAllAltItem { get; set; }
        public string ToStore { get; set; }
        public float Stock { get; set; }
        public float AltQty { get; set; }
        public string Remark { get; set; }

        //Main
        public int MatConvYearCode { get; set; }
        public string MatConvSlipNo { get; set; }
        public string MatConvSlipDate { get; set; }
        public string StoreWorkcenter { get; set; }
        public string Remarks { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedByEmpName { get; set; }
        public string Uid { get; set; }
        public string cc { get; set; }
        public int ActualEntryByEmpid { get; set; }
        public string ActualEntryDate { get; set; }
        public int? UpdatedByEmpId { get; set; }
        public string? UpdatedByEmpName { get; set; }
        public string? UpdationDate { get; set; }
        public string EntryByMachine { get; set; }

        //Detail
       
        public string OriginalItemCode { get; set; }
        
        public decimal OriginalQty { get; set; }
        public string AltItemCode { get; set; }
        public string AltUnit { get; set; }
        public decimal AltOriginalQty { get; set; }
        public int OriginalStoreId { get; set; }
        public int AltStoreId { get; set; }
        public int OrginalWCID { get; set; }
        public int AltWCID { get; set; }
        public string AltStoreName { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public decimal BatchStock { get; set; }
        public decimal TotalStock { get; set; }
        public decimal AltStock { get; set; }
        public string PlanNo { get; set; }
        public int PlanYearCode { get; set; }
        public string PlanDate { get; set; }
        public string ProdSchNo { get; set; }
        public int ProdSchYearCode { get; set; }
        public string ProdSchDatetime { get; set; }
        public decimal OrigItemRate { get; set; }
        public IList<MaterialConversionModel> MaterialConversionGrid { get; set; }

        //DashBoard
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Searchbox { get; set; }
        public string ReportType { get; set; }

       
    }
}
