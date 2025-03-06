using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class HRLeaveOpeningMasterModel
    {
        public int? LeaveOpnEntryId {  get; set; }
        public int? LeaveOpnYearCode {  get; set; }
        public string? EntryDate {  get; set; }
        public int? EmpId {  get; set; }
        public string? EmpName { get; set; }

        public string? EmpCode {  get; set; }
        public string? Designation { get; set; }
        public int? DesignationId {  get; set; }

        public string? Department { get; set; }
        public int? DeptId { get; set; }
        public string? Shift { get; set; }
        public int? ShiftId { get; set; }
        public int? ActualEntryBy {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public int? UpdatedBy { get;  set; }
        public string? UpdatedOn {  get; set; }
        /// <summary>
        /// ///////////
        /// </summary>
        public string? LeaveAccrualType { get; set; }

        public string? LeaveName { get; set; }
        public int LeaveId { get; set; }
        public decimal? AccrualRate { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CarriedForward { get; set; }
        public decimal? TotalLeaves { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public decimal? LeaveEncashmentAllowed { get; set; }
        public decimal? LeaveValidityPeriod { get; set; }
        public decimal? MandatoryLeaveAfter { get; set; }
        public decimal? MaxAllowedLeaves { get; set; }
        public string? Mode { get; set; }

        public IList<HRLeaveOpeningDetail> HRLeaveOpeningDetailGrid { get; set; }


    }

    [Serializable]
    public class HRLeaveOpeningDetail
    {
        public string? LeaveAccrualType { get; set; }
        public decimal? AccrualRate { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CarriedForward { get; set; }
        public decimal? TotalLeaves { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public decimal? LeaveEncashmentAllowed { get; set; }
        public decimal? LeaveValidityPeriod { get; set; }
        public decimal? MandatoryLeaveAfter { get; set; }
        public decimal? MaxAllowedLeaves { get; set; }
        public string? Mode { get; set; }
    }
}
