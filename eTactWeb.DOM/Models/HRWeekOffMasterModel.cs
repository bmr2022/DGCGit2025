using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRWeekOffMasterModel: TimeStamp
    {
        public int WeekoffEntryId {  get; set; }
        public int WeekoffYearCode {  get; set; }
        public string? WeekoffName {  get; set; }
        public string? WeekoffTypefixRot { get; set; }
        public int EmpCategoryId { get; set; }
        public int DeptId { get; set; }
        public string? WeekoffDays { get; set; }
        public string? MinWorkDaysRequired { get; set; }
        public string? halfdayFulldayOff { get; set; }
        public string? MaxWorkingDaysReqForWeekOff { get; set; }
        public string? OverrideForHolidays { get; set; }
        public string? CompensatoryOffApplicable { get; set; }
        public string? ExtraPayApplicable { get; set; }
        public string? Remark { get; set; }
        public string? Active { get; set; }
        public string? EffectiveFrom { get; set; }

        public int EntryByEmpId { get; set; }
        public int UpdatedbyId { get; set; }

    }
}
