using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class RecChallanReportModel:TimeStamp
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string RCMChallanNo { get; set; }
        public string RetNonRetChallan { get; set; }
        public string GateNo { get; set; }
        public string? GateDate { get; set; }
        public string BillChallan { get; set; }
        public string ChallanNo { get; set; }
        public string? ChallanDate { get; set; }
        public string PartyName { get; set; }
        public string ChallanType { get; set; }
        public string MRNNo { get; set; }
        public string? MRNDate { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public decimal GateQty { get; set; }
        public decimal RecQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string IssueChallanNo { get; set; }
        public int IssueChallanYearCode { get; set; }
        public decimal IssuedQty { get; set; }
        public string Remark { get; set; }
        public string RecInStore { get; set; }
        public string DocTypeCode { get; set; }
        public string DocName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public IList<RecChallanReportModel> RecChallanReportGrid { get; set; }
    }
}
