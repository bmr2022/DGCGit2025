using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PartyItemGroupDiscountModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int GroupCode { get; set; }
        public int SeqNo { get; set; }
        public decimal SaleDiscount { get; set; }
        public decimal PurchaseDiscount { get; set; }
        public int DiscCategoryEntryId { get; set; }
        public string Flag { get; set; } 
        public int PartyWIseGrpDiscEntryId { get; set; } 
        public int EntryId { get; set; } 
        public int AccountCode { get; set; }
        public string CategoryName { get; set; }
        public int CategoryCode { get; set; }
        public int CategoryId { get; set; }
        public string ReportType { get; set; } 

        public long EntryBy { get; set; } 
        public string? EntryDate { get; set; }
        public int UpdatedBy { get; set; } 
        public string AccountName { get; set; } 

        public string? Updateddate { get; set; }
        public string EntryByMachine { get; set; }
        public string CC { get; set; } 
        public string DiscCategoryCode { get; set; } 

        public string? Fromdate { get; set; }
        public string? ToDate { get; set; }

   
        public int ActualEntryByEmpId { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedbyEmpId { get; set; }
        public string LastUpdatedbyEmpName { get; set; }
        public string? LastupDationDate { get; set; }
        public int Uid { get; set; }
        public int ApprovedByEmpId { get; set; }
        public string ApprovedByEmpName { get; set; }
        public string Mode { get; set; }
        public int YearCode { get; set; }

        public IList<PartyItemGroupDiscountModel> PartyItemGroupDiscountGrid { get; set; }
    }
}
