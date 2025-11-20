using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRAdvanceModel : TimeStamp
    {
        public int AdvanceEntryId { get; set; }
        public int AdvanceYearCode { get; set; }
        public string AdvanceSlipNo { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public int DesigId { get; set; }
        public string DesigName { get; set; }
        public int DepId { get; set; }
        public string DeptName { get; set; }
        public string DOJ { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public string AdvanceType { get; set; }
        public decimal PresentDaysinCurrMonth { get; set; }
        public decimal PresentDaysInCurrYear { get; set; }
        public int CategoryId { get; set; }
        public string RequestDate { get; set; }
        public string EntryDate { get; set; }
        public string Purpose { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public decimal PreviousPendAdvanceAmt { get; set; }
        public decimal PreviousPendLoanAmt { get; set; }
        public string? MgrApprovaldate { get; set; }
        public int? MgrApprovedbyEmpid { get; set; }
        public string? MgrApprovedbyEmpName { get; set; }
        public string? MgrApprovedbyEmpCode { get; set; }
        public int? HRApprovedbyEmpid { get; set; }
        public string? HRApprovedbyEmpName { get; set; }
        public string? HRApprovedbyEmpCode { get; set; }
        public string? HRApprovalDate { get; set; }
        public int? FinanceApprovalEmpid { get; set; }
        public string? FinanceApprovalEmpName { get; set; }
        public string? FinanceApprovalEmpCode { get; set; }
        public string? FinanceApprovalDate { get; set; }
        public int? CanceledByEmpId { get; set; }
        public string? CanceledByEmpName { get; set; }
        public string? CanceledByEmpCode { get; set; }
        public string? Canceled { get; set; }
        public string? CancelOrApprovalremarks { get; set; }
        public string? ModeOfPayment { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string PaymentVoucherNo { get; set; }
        public string PaymentVoucherType { get; set; }
        public string PaymentRemark { get; set; }
        public string RecoveryMethod { get; set; }
        public int NoofInstallment { get; set; }
        public string? StartRecoveryFromMonth { get; set; }
        public string? AutoDeductionFromSalaryYN { get; set; }
        public string? FinalRevoveryDate { get; set; }
        public string? ActualFinalRecoveryDate { get; set; }
        public string? StatusHoldCancelApproved { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string? LastUpdatedByEmpName { get; set; }
        public string? LastUpdationDate { get; set; }
        public string RequestEntryByMachine { get; set; }
        public string? ApprovedBYMachine { get; set; }
        public string? CancelByMachine { get; set; }
        public int? ActualEntryBy { get; set; }
        public string? ActualEntryByName { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }

        public string FinFromDate { get; set;}
        public string FinToDate { get; set; }
        public HRAdvanceDashboard? HRAdvanceDashboards { get; set; }
    }

    public class HRAdvanceDashboard : HRAdvanceModel
    {
        public List<HRAdvanceDashboard>? HRAdvanceDashboards { get; set; }
    }
}
