using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class SaleOrderRegisterModel
    {
        public int SOEntryID { get; set; }
        public int SOYearCode { get; set; }
        public string CustOrderNo { get; set; }
        public string SONo { get; set; }
        public string? SODate { get; set; }
        public string OrderType { get; set; }
        public string SOType { get; set; }
        public string SOFor { get; set; }
        public string? WEF { get; set; }
        public string? SOCloseDate { get; set; }
        public int CurrencyID { get; set; }
        public int SOAmmNo { get; set; }
        public string? SOAmmEffDate { get; set; }
        public int AccountCode { get; set; }
        public string DeliveryAddress { get; set; }
        public int ConsigneeAccountCode { get; set; }
        public string ConsigneeAddress { get; set; }
        public decimal OrderAmt { get; set; }
        public decimal OrderNetAmt { get; set; }
        public string FreightPaidBy { get; set; }
        public string InsuApplicable { get; set; }
        public string ModeTransport { get; set; }
        public string Remark { get; set; }
        public string? SOConfirmDate { get; set; }
        public string SoComplete { get; set; }
        public string Approved { get; set; }
        public string? ApprovedDate { get; set; }
        public string? DeActiveDate { get; set; }
        public string EntryByMachineName { get; set; }
        public string SchNo { get; set; }
        public string? SchDate { get; set; }
        public int SchYearCode { get; set; }
        public string TentConfirm { get; set; }
        public string? SchEffectFrom { get; set; }
        public string? SchEffTill { get; set; }
        public string SchClosed { get; set; }
        public string SchCompleted { get; set; }
        public string SchActive { get; set; }
        public long ItemCode { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public long ItemAmendNo { get; set; }
        public string AmendmentReason { get; set; }
        public decimal PendQty { get; set; }
        public string? DeliveryDate { get; set; }
        public string CustomerLocation { get; set; }
        public long SchAmndNo { get; set; }
        public string? SchAmenddate { get; set; }
        public string ApprovedByEmp { get; set; }
        public string DeActiveByEmp { get; set; }
        public string CustomerName { get; set; }

        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string ConsigneeName { get; set; }
        public string SalesPersonName { get; set; }
        public int SalesPersonCode { get; set; }
        public string SalesEmailId { get; set; }
        public string SalesMobileNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Mode { get; set; }
        public string ReportType { get; set; }
        public string Order_Schedule { get; set; }
        public int Itemcode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<SaleOrderRegisterModel> saleOrderRegisterGrid { get; set; } 
    }
}
