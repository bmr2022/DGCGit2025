using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class StockValuationModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CurrentDate { get; set; }
        public string Flag { get; set; } 
        public string GroupName { get; set; } 
        public string CatName { get; set; } 
        public string ItemName { get; set; } 
        public string PartCode { get; set; } 
        public string StoreName { get; set; } 
        public long StoreId { get; set; }
        public string WorkCenter { get; set; } 
        public long WCID { get; set; } 
        public char ConsiderWIPStock { get; set; }
        public char OtherStockStk { get; set; } 
        public char OtherWCStk { get; set; } 
        public string CC { get; set; } 

        //ItemStock
        public long ItemCode { get; set; }
        public float OpeningStock { get; set; }
        public float OpenRate { get; set; }
        public float OpeningValue { get; set; }
        public float StockQty { get; set; }
        public float IssueQty { get; set; }
        public float ClosingStock { get; set; }
        public float Rate { get; set; }
        public float ClosingValue { get; set; }
        public float RecQty { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; } 
        public float WIPRecQty { get; set; }
        public float WIPIssQty { get; set; }
        public float WIPStock { get; set; }

        //ItemTable
        public string ItemGroup { get; set; } 
        public string ItemCat { get; set; } 
        public string ReportType { get; set; } 

        public IList<StockValuationModel> StockValuationGrid { get; set; }
    }
}
