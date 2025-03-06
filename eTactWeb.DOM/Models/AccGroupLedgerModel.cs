using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccGroupLedgerModel: TimeStamp
    {
        public long TBSeq { get; set; }
        public int TrailBalanceGroupCode { get; set; }
        public string TrailBalanceGroupName { get; set; }
        public string ParentGroupName { get; set; }
        public int ParentAccountCode { get; set; }
        public string SubGroupParent { get; set; }
        public string UnderGroup { get; set; }
        public string MainGroup { get; set; }
        public string AccountName { get; set; }
       
        public decimal OpnDr { get; set; }
        public decimal OpnCr { get; set; }
        public decimal GroupOpnDr { get; set; }
        public decimal GroupOpnCr { get; set; }
        public decimal TotalOpening { get; set; }
        public decimal TotalGroupOpening { get; set; }
        public decimal CurrDrAmt { get; set; }
        public decimal CurrCrAmt { get; set; }
        public decimal GroupCurrCrAmt { get; set; }
        public decimal GroupCurrDrAmt { get; set; }
        public decimal NetCurrentAmt { get; set; }
        public decimal NetAmt { get; set; }
        public decimal GroupNetAmt { get; set; }
        public decimal CurrDrTotal { get; set; }
        public decimal CurrCrTotal { get; set; }
        public int SeqNo { get; set; }
        public long TrailBalanceGroupId { get; set; }
        public string GroupLedger { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string EntryByMachine { get; set; }
        public string ReportType { get; set; }
        public string GroupName { get; set; }
        public int Account_Code { get; set; }
        public IList<AccGroupLedgerModel> AccGroupLedgerGrid { get; set; }
    }
}
