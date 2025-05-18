using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class BankReconciliationModel
    {
        public string Flag { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string chequeNo { get; set; }
        public string BankName { get; set; }
        public string NewOrEdit { get; set; }
        public string Account_Code { get; set; }
        public string Date { get; set; }
        public string Perticuler { get; set; }
        public string Type { get; set; }
        public string BankDate { get; set; }
        public string DrAmt { get; set; }
        public string? VoucherNo { get; set; }
        public string CrAmt { get; set; }
        public string ChequeNo { get; set; }
        public string entryid { get; set; }
        public string AccYearCode { get; set; }


        
        public string Mode { get; set; }

        public IList<BankReconciliationModel> BankReconciliationGrid { get; set; }
    }
}
