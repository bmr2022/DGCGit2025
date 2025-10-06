using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class OutStandingRow
    {
        public Dictionary<string, object> DynamicColumns { get; set; } = new();
    }

    public class OutStandingModel
    {
        public string Flag { get; set; }
        public string ReportType { get; set; }
        public string PartyName { get; set; }
        public string GroupName { get; set; }
        public string FromDate { get; set; }
        public string TillDate { get; set; }
        public int currYearcode { get; set; }
        public string Mode { get; set; }
        public string ShowOnlyApprovedBill { get; set; }
        public string LedgerDescription { get; set; }
        public int AccountCode {  get; set; }
        public string VoucherNo { get; set; }
        public string VoucherDate { get; set; }
        public int VoucherYearCode {  get; set; }
        public string VoucherType { get; set; }
        public string DrAmt { get; set; }
        public string CrAmt { get; set; }
        public string BillAmt { get; set; }
        public string PendingAmt { get; set; }
        public string DueDate { get; set; }
        public string OverDueDays { get; set; }
        public string TotBalanceAmt { get; set; }
        public string AccEntryId { get; set; }
        public string AccYearCode { get; set; }
        public int DocEntryId {  get; set; }
        public string SalesPersonName { get; set; }
        public bool IsVisible {  get; set; }
        public bool ShowZeroBal {  get; set; }
        public IList<OutStandingRow> OutStandingRow { get; set; }
        public List<string> ColumnNames { get; set; } = new(); // for dynamic header rendering
        public IList<OutStandingModel> OutStandingGrid { get; set; }
        public List<OutStandingPopUpData> OutStandingPopUpDatas { get; set; }   
        public string[] AccountNameList {  get; set; }
    }
    public class OutStandingPopUpData
    {
        public string VoucherNo { get; set; }
        public string VoucherDate { get; set; }
        public string BillOrInvoiceNo {  get; set; }
        public decimal DrAmt {  get; set; }
        public decimal CrAmt {  get; set; }
        public string VoucherType {  get; set; }
        public string NewRefNo {  get; set; }
        public int DocEntryId { get; set; }
        public int AccEntryId {  set; get; }
        public int AccYearCode { get; set; }
        public int AgainstAccEntryId {  get; set; }
        public int AgainstAccYearCode {  get; set; }
    }
}
