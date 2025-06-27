using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PurchaseMISModel
    {
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string AccountName { get; set; }
        public int Itemcode { get; set; }
        public int AccountCode { get; set; }

        public double APR { get; set; }
        public double MAY { get; set; }
        public double JUN { get; set; }
        public double JUL { get; set; }
        public double AUG { get; set; }
        public double SEP { get; set; }
        public double OCT { get; set; }
        public double NOV { get; set; }
        public double DEC { get; set; }
        public double JAN { get; set; }
        public double FEB { get; set; }
        public double MAR { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public string Mode { get; set; }
        public int YearCode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<PurchaseMISModel> PurchaseMISGrid { get; set; }
    }
}
