using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ScheduleCalibrationModel
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
            public string? ActualEntryByEmpName { get; set; }
            public string? ActualEntryDate { get; set; }
            public int? LastUpdatedBy { get; set; }
            public string? UpdatedByEmpName { get; set; }
            public string? LastUpdationDate { get; set; }
            public string CC { get; set; }
            public string UID { get; set; }
            public string EntryByMachine { get; set; }
            public string Mode { get; set; }
            public IList<ScheduleCalibrationModel> CalibrationScheduleGrid { get; set; }
        
    }
    public class PendingScheduleCalibrationModel
    {
		public string ToolName { get; set; }
		public string ToolCode { get; set; }
		public string ItemName { get; set; }
		public int ItemCode { get; set; }
		public string PartCode { get; set; }
		public string? LastCalibrationDate { get; set; }
		public string? NextCalibrationDate { get; set; }
		public int CalibrationAgencyId { get; set; }
		public string CalibrationAgencyName { get; set; }
		public string FromDate { get; set; }
		public string ToDate { get; set; }
		public int seqno { get; set; }
		public int TotalRecords { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string LastCalibrationCertificateNo { get; set; }      // ID as int
		public string CalibrationResultPassFail { get; set; }
		public string TolrenceRange { get; set; }
		public string CalibrationRemark { get; set; }
		public string TechEmployeeName { get; set; }
		public string Technician { get; set; }
		public string TechnicialcontactNo { get; set; }
		public int CustoidianEmpId { get; set; }
		public IList<PendingScheduleCalibrationModel> ?PendingScheduleCalibrationGrid { get; set; }
	}
}
