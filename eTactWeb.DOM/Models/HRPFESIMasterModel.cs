using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRPFESIMasterModel: TimeStamp
    {
        public int EntryId {  get; set; }
        public string? SchemeType { get; set; }
        public string? EffectiveFrom { get; set; }

       
        /// ///////////////
        

        public decimal? EmployeePFDeducionPer { get; set; }
        public decimal? EmployerPFContributionPer { get; set; }
        public decimal? EmployerEPSContributionPer { get; set; }
        public decimal? PFWageLimit { get; set; }
        public int? MinServicePeriodForPF { get; set; }
       
        public string? PFAccountNumberFormat { get; set; }

       
        /// ////////
       
        public decimal? EmployeeESIContributionPer { get; set; }
        public decimal? EmployerESIContributionPer { get; set; }
        public decimal? ESIWageLimit { get; set; }

        public string? ESICode { get; set; }
        public string? ESIDispensary { get; set; }
        
        /// /////
       
        public string? TaxDeductionApplicable { get; set; }
        public string? VoluntaryPFAllowed { get; set; }
        public int? PFWithdrawalLockPeriod { get; set; }
        public string? ESIBenefitsCovered { get; set; }

        

        public IList<string>? ExemptedCategories { get; set; }
        public IList<ExemptedCategoriesDetail>? ExemptedCategoriesDetailList { get; set; }
        public IList<TextValue>? ExemptedCategoriesList { get; set; }





        public string? ComplianceStatus { get; set; }
        
        /// /////
       
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string? Active { get; set; }
        public string? CreatedOn { get; set; }
        public string? UpdatedOn { get; set; }
        public string? EntryByMachine { get; set; }



    }
}

[Serializable()]
public class ExemptedCategoriesDetail
{
    //public int CategoryId { get; set; }
    public int? EntryId { get; set; }
    public string? CategoryId { get; set; }
    public string? EmpCateg { get; set; }


}
