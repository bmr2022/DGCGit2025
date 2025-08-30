using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ToolMoldMasterModel
    {
		public int ToolEntryId { get; set; }
		public int AccountCode { get; set; }
		public string EntryDate { get; set; }
		public string ToolOrMold { get; set; }
		public string ToolCode { get; set; }
		public string ToolName { get; set; }
		public int ItemCode { get; set; }
		public string ItemName { get; set; }
		public int YearCode { get; set; }

		public int ToolCateogryId { get; set; }
		public string ConsiderAsFixedAssets { get; set; }
		public string ConsiderInInvetory { get; set; }
		public int ParentAccountCode { get; set; }
		public string ParentAccountName { get; set; }
		public string MainGroup { get; set; }
		public string SubGroup { get; set; }
		public string UnderGroup { get; set; }
		public int SubSubGroup { get; set; }
		public int CostCenterId { get; set; }
		public string CostCenterName { get; set; }
		public int Fiscalyear { get; set; }

		// Vendor detail
		public int VendoreAccountCode { get; set; }
		public string VendoreAccountName { get; set; }
		public string PONO { get; set; }
		public string PODate { get; set; }
		public int POYear { get; set; }
		public string InvoiceNo { get; set; }
		public string InvoiceDate { get; set; }
		public int InvoiceYearCode { get; set; }
		public decimal NetBookValue { get; set; }
		public decimal PurchaseValue { get; set; }
		public decimal ResidualValue { get; set; }

		// Depreciation Detail
		public string DepreciationMethod { get; set; }
		public decimal DepreciationRate { get; set; }
		public decimal DepriciationAmt { get; set; }

		public string CountryOfOrigin { get; set; }
		public string FirstAqusitionOn { get; set; }
		public decimal OriginalValue { get; set; }
		public string CapatalizationDate { get; set; }

		// Identification
		public string BarCode { get; set; }
		public string SerialNo { get; set; }

		// Usage & Life
		public string LocationOfInsallation { get; set; }
		public int ForDepartmentId { get; set; }
		public int DepartmentName { get; set; }
		public int ExpectedLife { get; set; }

		// Calibration Detail
		public string CalibrationRequired { get; set; }
		public int CalibrationFrequencyInMonth { get; set; }
		public string LastCalibrationDate { get; set; }
		public string NextCalibrationDate { get; set; }
		public int? CalibrationAgencyId { get; set; }
		public string LastCalibrationCertificateNo { get; set; }
		public string CalibrationResultPassFail { get; set; }
		public string TolrenceRange { get; set; }
		public string CalibrationRemark { get; set; }

		public string Technician { get; set; }
		public string TechnicialcontactNo { get; set; }
		public string TechEmployeeName { get; set; }
		public string CustoidianEmpName { get; set; }
		public int CustoidianEmpId { get; set; }

		// Other detail
		public string InsuranceCompany { get; set; }
		public decimal InsuredAmount { get; set; }
		public string InsuranceDetail { get; set; }
		public string CC { get; set; }
		public string UID { get; set; }
		public int ActualEntryBy { get; set; }
		public string ActualEntryDate { get; set; }
		public string ActualEntryByEmpName { get; set; }
		public int? LastupdatedBy { get; set; }
		public string LastUpdatedDate { get; set; }
		public string LastUpdatedbyEmpName { get; set; }
		public string EntryByMachine { get; set; }
		public string Mode { get; set; }
		public string FromDate { get; set; }
		public string ToDate { get; set; }
		public string Searchbox { get; set; }
		public IList<ToolMoldMasterModel> ToolMoldGrid { get; set; }
	}
}
