using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class DiscountCustomerCategoryMasterModel
    {
        public string Flag { get; set; }
        public int DiscountCustCatEntryId { get; set; }
        public string DiscountCatSlipNo { get; set; } 
        public int DiscountCustCatYearCode { get; set; } 
        public string DiscountCategory { get; set; }
        public string? EffectiveFromDate { get; set; } 
        public decimal MinDiscountPer { get; set; } 
        public decimal MaxDiscountPer { get; set; }
        public string ApplicableMonthlyYearlyAfterEachSale { get; set; } 
        public string ApplicableOnAdvancePayment { get; set; } 
        public decimal MinmumAdvancePaymentPercent { get; set; }
        public string CategoryActive { get; set; } 
        public string EntryByMachine { get; set; } 
        public int ActualEntryByEmpId { get; set; } 
        public string ActualEntryByEmpName { get; set; } 
        public string? ActualEntryDate { get; set; } 
        public int LastUpdatedbyEmpId { get; set; } 
        public string LastUpdatedbyEmpName { get; set; } 
        public string? LastupDationDate { get; set; } 
        public string CC { get; set; } 
        public int Uid { get; set; } 
        public int ApprovedByEmpId { get; set; }
        public string ApprovedByEmpName { get; set; }
        public string Mode { get; set; }
        public string DiscountCatSlipDate { get; set; }
        public string PartCode { get; set; }
        public int ItemCode { get; set; }
        public IList<DiscountCustomerCategoryMasterModel> DiscountCustomerCategoryMasterGrid { get; set; }
    }
}
