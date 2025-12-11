using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRHolidaysMasterModel: TimeStamp
    {
        public int HolidayId {  get; set; }
        public string? IPAddress { get; set; }
        public int HolidayYear {  get; set; }
        public string HolidayName { get; set; }

        public string? EffectiveFrom { get; set; }
        public string HolidayEffTill { get; set; }
        public string? HolidayType { get; set; }
        public string? Country {  get; set; }
        public int? StateId {  get; set; }
        public string? State {  get; set; }
        public string? StateName { get; set; }
        public string? Branch { get; set; }

        public IList<string>? EmployeeCategory { get; set; }
        public IList<HolidayEmployeeCategoryDetail>? EmployeeCategoryDetailList { get; set; }
        public IList<TextValue>? EmployeeCategoryList { get; set; }

        public string? ApplicableOnCategory {  get; set; }
        public string? ApplicableOnDepartment {  get; set; }
        public IList<string>? Department { get; set; }
        public IList<HoliDayDepartmentDetail>? DepartmentDetailList { get; set; }
        public IList<TextValue>? DepartmentList { get; set; }


        public int? EmpCategoryId {  get; set; }
        public int? DeptId {  get; set; }
        public int? ShiftId {  get; set; }
        public string? OverrideWeekOff {  get; set; }
        public string? CompensatoryOffAllowed { get; set; }
        public string? PaidHoliday { get; set; }
        public string? HalfDayFullDay { get; set; }
        
        public string? Active { get; set; }
        public string? Remark { get; set; }
        public int? CreatedBy { get; set; }

        public string? CreatedByEmp { get; set; }
        public string? UpdatedByEmp { get; set; }
        public string? CreatedOn { get; set; }
        public string? EntryDate { get; set; }
        public string? UpdatedOn { get; set; }


        public int? UpdatedBy { get; set; }

        public IList<HRHolidaysMasterModel>? HRHolidayDashboard { get; set; }
        public string Searchbox { get; set; }
        public string EntryByMachine {  get; set; }
        public string FromDate {  get; set; }
        public string ToDate { get; set; }
        
    }
}

[Serializable()]
public class HolidayEmployeeCategoryDetail
{
    public int? HolidayEntryId { get; set; }
    public int? HolidayYear { get; set; }
    public string? Country { get; set; }
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public string? CategoryId { get; set; }
}

[Serializable()]
public class HoliDayDepartmentDetail
{
    public int? HolidayEntryId { get; set; }
    public int? HolidayYear { get; set; }
    public string? Country { get; set; }
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public string? DeptId { get; set; }
}

