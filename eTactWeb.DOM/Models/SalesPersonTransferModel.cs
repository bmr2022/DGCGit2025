using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public  class SalesPersonTransferModel
    {
        public string Flag { get; set; } 
        public string Mode { get; set; } 
        public string ShowAllEmp { get; set; } 
        public string ShowAllOldEmp { get; set; } 
        public string ShowAllCust { get; set; } 
        public int SalesPersTransfYearCode { get; set; } 
        public int SalesPersTransfEntryId { get; set; }
        public string? SalesPersTransfEntryDate { get; set; }
        public string SalesPersTransfSlipNo { get; set; } 
        public string EffFrom { get; set; } 
        public int RevNo { get; set; }
        public int NewSalesEmpId { get; set; } 
        public string NewSalesEmpName { get; set; } 
        public string NewSalesCode { get; set; } 
        public int OldSalesEmpId { get; set; }
        public string OldSalesEmpName { get; set; }
        public string OldSalesCode { get; set; }
        public string? NewSalesPersEffdate { get; set; }
        public string? OldSalesPersTillDate { get; set; }
        public int EntryByEmpId { get; set; } 
        public int UpdatedByEmpId { get; set; } 
        public string UpdatedByEmpName { get; set; } 
        public string? UpdationDate { get; set; }
        public string EntryByMachine { get; set; } 
        public string CC { get; set; } 
        public int CreatedBy { get; set; } 
        public int ApprovedBy { get; set; } 
        public string ApprovedByEmpName { get; set; } 
        public string Designation { get; set; } 
        public string Department { get; set; } 
        public string EffTillDate { get; set; } 
        public string CustomerName { get; set; } 
        public string CustAddress { get; set; } 
        public string CustCity { get; set; } 
        public string CustState { get; set; } 
        public string CustStateCode { get; set; } 
        public string CustNameCode { get; set; } 
        public string Searchbox { get; set; } 
        public int seqno { get; set; } 
        public int AccountCode { get; set; } 
       
      
        public IList<SalesPersonTransferModel> SalesPersonTransferGrid { get; set; }
    }
	public class SelectedRow
	{
		public int CustNameCode { get; set; }
	}
}
