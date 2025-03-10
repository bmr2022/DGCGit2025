using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRLeaveOpeningMasterModel: HRLeaveOpeningDetail
    {
        public int? LeaveOpnEntryId {  get; set; }
        public int? LeaveOpnYearCode {  get; set; }
        public string? EntryDate {  get; set; }
        public int? EmpId {  get; set; }
        public string? EmpName { get; set; }

        public string? EmpCode {  get; set; }
        public string? Designation { get; set; }
        public int? DesignationId {  get; set; }
        public int? SrNO { get; set; }

        public string? Department { get; set; }
        public int? DeptId { get; set; }
        public string? Shift { get; set; }
        public int? ShiftId { get; set; }
        public int? ActualEntryBy {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public int? UpdatedBy { get;  set; }
        public string? UpdatedOn {  get; set; }
        public string? EntryByMachine {  get; set; }

        public IList<HRLeaveOpeningDetail>? HRLeaveOpeningDetailGrid { get; set; }
        /// <summary>
        /// ///////////
        /// </summary>
        /// 



    }

    [Serializable]
    public class HRLeaveOpeningDetail: TimeStamp
    {
        public int? SrNO { get; set; }
        public int? LeaveOpnEntryId { get; set; }
        public int? EmpId { get; set; }
        public int? LeaveOpnYearCode { get; set; }
        public string? LeaveAccrualType { get; set; }

        public string? LeaveName { get; set; }
        public int? LeaveId { get; set; }
        public decimal? AccrualRate { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CarriedForward { get; set; }
        public decimal? TotalLeaves { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public string? LeaveEncashmentAllowed { get; set; }
        public string? LeaveValidityPeriod { get; set; }
        public int? MandatoryLeaveAfter { get; set; }
        public decimal? MaxAllowedLeaves { get; set; }
        public string? Mode { get; set; }

        
    }

    public class HRLeaveOpeningDashBoardModel : TimeStamp
    {


        public int? SrNO { get; set; }
        public int? LeaveOpnEntryId { get; set; }
        public string? EmpName { get; set; }
        public int? EmpId { get; set; }
        public int? LeaveOpnYearCode { get; set; }
        public string? LeaveAccrualType { get; set; }

        public string? LeaveName { get; set; }
        public int? LeaveId { get; set; }
        public decimal? AccrualRate { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CarriedForward { get; set; }
        public decimal? TotalLeaves { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public string? LeaveEncashmentAllowed { get; set; }
        public string? LeaveValidityPeriod { get; set; }
        public int? MandatoryLeaveAfter { get; set; }
        public decimal? MaxAllowedLeaves { get; set; }
        public string? Mode { get; set; }
        public string? EmpCode { get; set; }
        public string? Designation { get; set; }
        public int? DesignationId { get; set; }

        public string? Department { get; set; }
        public int? DeptId { get; set; }
        public string? Shift { get; set; }
        public int? ShiftId { get; set; }
        public int? ActualEntryBy { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? EntryByEmp { get; set; }
        public string? UpdatedByEmp { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedOn { get; set; }
        public string? EntryByMachine { get; set; }
        public string? ReportType { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Searchbox { get; set; }
        public List<HRLeaveOpeningDashBoardModel>? HRLeaveOpeningDashBoardDetail { get; set; }

    }
}
