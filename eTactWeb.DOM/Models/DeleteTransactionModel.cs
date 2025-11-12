using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class DeleteTransactionModel : TimeStamp
    {
        public string FormName { get; set; }
        public string Action { get; set; }
      
        public long YearCode { get; set; }
        public string? EntryDate { get; set; }
        public string CC { get; set; }
        public string MachineName { get; set; }
        public long ActionByEmpId { get; set; }
        public string IPAddress { get; set; }

        public string SlipNo { get; set; }
        public string AccountName { get; set; }
        public long AccountCode { get; set; }
        public long EntryId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal BasicAmount { get; set; }
       
        public string Flag { get; set; }
    }

}
