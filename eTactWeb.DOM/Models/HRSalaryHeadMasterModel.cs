using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRSalaryHeadMasterModel: TimeStamp
    {
        public int SalHeadEntryId { get; set; }
        public string? SalHeadEntryDate { get; set; }
       
        public string? SalHeadEffectiveDate { get; set; }
        public string? SalaryHead { get; set; }
        public string? SalaryCode { get; set; }

        public string? ShortForm { get; set; }

        public string? TypeOfSalary { get; set; }

        public string? PartOfGrossBasic { get; set; }
        public string? CurrencyId { get; set; }
        public string? Currency { get; set; }
        public string? SalaryPaymentMode { get; set; }
        public string? PaymentMode { get; set; }
        public string? CalculationType { get; set; }
        public string? AmountOrPercentage { get; set; }

        public string? PercentageOfSalaryHeadID { get; set; }
        public string? ContributionSalaryHead { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal CalculationPercentage { get; set; }
        public string? CalculationFormula { get; set; }
        public string? RoundOffMethod { get; set; }
        public string? RoundType { get; set; }
        public string? FrequencyOfPayment { get; set; }
        public string? CarryForward { get; set; }
        public decimal ContributionPercentage { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal MaxAmountofCarryForward { get; set; }
        public string? EmployerContribution { get; set; }
        public decimal ContributionPerOfSalaryHeadId { get; set; }
       

        [Column(TypeName = "decimal(10, 4)")]
        public decimal Taxpercentage { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TaxAmount { get; set; }
        public string? DeductionMorY { get; set; }
        public string? ActiveStatus { get; set; }
        public int DisplayOrder { get; set; }
        public string? Remarks { get; set; }
        public int ActualEntryby { get; set; }
        public int LastUpdatedBy { get; set; }
        public string? LastUpdatedOn { get; set; }
        public string? EntryByMachine { get; set; }
        public decimal MinAmount { get; set; }

       
        public decimal MaxAmount { get; set; }
        public string? PartOfPayslip { get; set; }

        public IList<string>? EmpCateg { get; set; }
        public int CategoryId { get; set; }
        public int DeptId { get; set; }

        public string? ApplicableOnCategory { get; set; }
        public IList<EmpCategDetail>? EmpCategDetailList { get; set; }
        public IList<EmpCategDetail1>? EmpCategDetailList1 { get; set; }
        public IList<TextValue>? EmpCategList { get; set; }

        public IList<string>? DeptName { get; set; }
        public string? ApplicableOnDeparyment {  get; set; }
        public IList<DeptWiseCategDetail>? DeptWiseCategDetailList { get; set; }
        public IList<DeptWiseCategDetail1>? DeptWiseCategDetailList1 { get; set; }
        public IList<TextValue>? DeptWiseCategList { get; set; }

       
        public string? PFApplicable { get; set; }
        public string? ESIApplicable { get; set; }
        public string? IncomeTaxApplicable { get; set; }
        public IList<HRSalaryHeadMasterModel>? HRSalaryDashboard { get; set; }
        public string Searchbox { get; set; }

    }
}
[Serializable()]
public class EmpCategDetail
{
    //public int CategoryId { get; set; }
    public int SalHeadEntryId { get; set; }
    public string? CategoryId { get; set; }


}

[Serializable()]
public class DeptWiseCategDetail
{
    // public int DeptId { get; set; }
    public int SalHeadEntryId { get; set; }
    public string? DeptId { get; set; }

}

[Serializable()]
public class DeptWiseCategDetail1
{
     public string? DeptId { get; set; }
    
    public string? DeptName { get; set; }

}

[Serializable()]
public class EmpCategDetail1
{
    public string? CategoryId { get; set; }
    
    public string? EmpCateg { get; set; }


}


