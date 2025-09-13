using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ScheduleCalibrationModel
    {
        public class CalibrationScheduleModel
        {

            public int CalibSchEntryId { get; set; }
            public int CalibSchYearCode { get; set; }
            public int ToolEntryId { get; set; }
            public string ToolOrMold { get; set; }
            public string? ScheduleDate { get; set; }
            public string? ActualScheduleDate { get; set; }
            public string FrequencyMQYN { get; set; }
            public int? ResponsibleEmpId { get; set; }
            public string CalibInhouseThirdPart { get; set; }   
            public int? CalibrationAgencyId { get; set; }
            public string StatusSCO { get; set; }
            public string Remark { get; set; }
            public string OtherInstruction { get; set; }
            public string CalibSchNo { get; set; }
            public string? CalibSchEntryDate { get; set; }
            public int? CalibScheduledByEmpId { get; set; }
            public int? ActualEntryBy { get; set; }
            public string? ActualEntryDate { get; set; }
            public int? LastUpdatedBy { get; set; }
            public string? LastUpdationDate { get; set; }
            public string CC { get; set; }
            public string UID { get; set; }
            public string EntryByMachine { get; set; }
            public IList<CalibrationScheduleModel> CalibrationScheduleGrid { get; set; }
        }
    }
}
