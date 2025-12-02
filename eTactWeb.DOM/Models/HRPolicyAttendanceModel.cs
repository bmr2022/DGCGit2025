using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRPolicyAttendanceModel : TimeStamp
    {
        public int PolicyId { get; set; }
        public string? PolicyName { get; set; }
        public string? IsActive { get; set; }
        public string? HalfDayApplicableAfterLateMin { get; set; }
        [Range(0, 180, ErrorMessage = "Max Grace Minutes cannot be more than 180.")]
        public int MaxGraceMinforlate { get; set; }
        public int FullDayAbsentIfPresentLessthenMin { get; set; }
        public string? OverTimeEligibleYN { get; set; }
        public int OverTimeRateXBasicRate { get; set; }
        public string? OverTimeApplicableAfterExtraHours { get; set; }
        public string? WeekOffFixedRotate { get; set; }
        [Range(1, 7, ErrorMessage = "Days must be between 1 and 7.")]
        public int MinNoOfPresentDaysreqForWeekOff { get; set; }
        public int HalfDayCountIfLeftBeforeShiftTime { get; set; }
        public string? FlexibleInOutTimingAllowed { get; set; }
        public string? SandwitchWeekoffPolicyApplicable { get; set; }
        public string? SandWitchHolidayPolicyApplicable { get; set; }
        public int MaxLateIncomingAllowed { get; set; }
        public int MaxEarlyLeavingAllowed { get; set; }
        public string? LateComingEarlyGoingCountSame { get; set; }
        public int MaxNoOfHalfDayInMonth { get; set; }
        public int MaxNoOfhourinAWeek { get; set; }
    }
}
