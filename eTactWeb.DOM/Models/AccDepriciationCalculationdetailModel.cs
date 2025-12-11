using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class AccDepriciationCalculationdetailModel
    {
		public int DepriciationEntryId { get; set; }
		public string? IPAddress { get; set; }
        public int DepriciationYearCode { get; set; }
		public int ForClosingOfFinancialYear { get; set; }
		public string? DepriciationDate { get; set; }
		public string DepriciationSlipNo { get; set; }
		public string CC { get; set; }
		public int ActualEntryBy { get; set; }
		public string? ActualEntryDate { get; set; }
		public string? ActualEntryByEmpName { get; set; }
		public int LastupdatedBy { get; set; }
		public string? LastUpdatedByEmpName { get; set; }
		public string? LastUpdatedDate { get; set; }
		public string EntryByMachine { get; set; }
		public int UID { get; set; }
		public string BalanceSheetClosed { get; set; }
		public string Carryforwarded { get; set; }
		public string BlockedEntry { get; set; }
		public string Mode { get; set; }
		public int AssetsEntryId { get; set; }
		public string  AssetsName { get; set; }
		public string  AssetsCategoryName { get; set; }
		public int AccountCode { get; set; }
		public string AccountName { get; set; }
		public int ItemCode { get; set; }
		public string ItemName { get; set; }
		public string MainGroup { get; set; }
		public int MainGroupCode { get; set; }
		public int ParentAccountCode { get; set; }
		public string ParentAccountName { get; set; }
		public string SubGroup { get; set; }
		public string UnderGroup { get; set; }
		public string SubSubGroup { get; set; }
		public decimal OriginalNetBookValue { get; set; }
		public int PreviousYearValue { get; set; }
		public string DepreciationMethod { get; set; }
		public decimal DepreciationRate { get; set; }
		public decimal AfterDepriciationNetValue { get; set; }
		public decimal RemainingUseFullLifeInYear { get; set; }
		public string CarryForwarded { get; set; }
		public int SeqNo { get; set; }
		public string FromDate { get; set; }
		public string ToDate { get; set; }
		public string ReportType { get; set; }
		public string Searchbox { get; set; }

		public IList<AccDepriciationCalculationdetailModel> AccDepriciationCalculationdetailGrid { get; set; }
	}
}
