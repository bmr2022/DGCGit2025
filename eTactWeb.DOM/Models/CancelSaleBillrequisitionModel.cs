using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CancelSaleBillrequisitionModel : TimeStamp
    {
        public string Flag { get; set; }
        public string? IPAddress { get; set; }
        public int YearCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int CanSaleBillReqEntryid { get; set; }
        public int CanSaleBillReqYearcode { get; set; }
       
        public string? CanSaleBillReqDate { get; set; }
        public string CurrentDate { get; set; }
        public int AccountCode { get; set; }
        public string CustomerName { get; set; }
        public string SaleBillNo { get; set; }
        public long SaleBillEntryId { get; set; }
        public string? SaleBillDate { get; set; }
        public double BillAmt { get; set; }
        public double INVNetAmt { get; set; }
        public int Approvedby { get; set; }
        public string ReasonOfCancel { get; set; }
        public string CC { get; set; }
        public long uid { get; set; }
        public long SaleBillYearCode { get; set; }
        public string CanRequisitionNo { get; set; }
        public string MachineName { get; set; }
        public string Requisitionby { get; set; }


        public string CustomerNameOutput { get; set; }
        public string VoucherType { get; set; }
        public string Canceled { get; set; }
        public string? ApprovalDate { get; set; }
        public string? CancelDate { get; set; }
        public long ApprovedBy { get; set; }
        public long SaleBillYearCodeOutput { get; set; }
        public long SaleBillEntryIdOutput { get; set; }
        public string ReqNo { get; set; }
        public string Updationdate { get; set; }
        public IList<CancelSaleBillrequisitionModel> CancelSaleBillrequisitionsGrid
        {
            get;set;
        }
        //DashBoard
        public string Searchbox { get; set; }

    }
}