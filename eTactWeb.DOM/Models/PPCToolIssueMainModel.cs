using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
	public class PPCToolIssueMainModel:TimeStamp
	{
		public long ToolIssueEntryId { get; set; }
		public long ToolIssueYearCode { get; set; }
		public DateTime? ToolIssueDate { get; set; }
		public DateTime? ToolIssueEntryDate { get; set; }
		public string ToolIssueSlipNo { get; set; }
		public long IssueToDepartmentId { get; set; }
		public long IssuedByEmpId { get; set; }
		public long ApprovedByEmpId { get; set; }
		public long ReceivedByEmpId { get; set; }
		public string CC { get; set; }
		public long UID { get; set; }
		public string EntryByMachine { get; set; }
		public DateTime? ActualEntryDate { get; set; }
		public long ActualEntryBy { get; set; }
		public long LastUpdatedBy { get; set; }
		public DateTime? LastUpdatedDate { get; set; }
		public string pendingStatus { get; set; }

		// Dropdown lists (optional)
		public IList<TextValue>? DepartmentList { get; set; }
		public IList<TextValue>? EmployeeList { get; set; }
		public IList<TextValue>? MachineList { get; set; }

		// Child table
		public IList<PPCToolIssueDetailModel> ToolIssueDetails { get; set; } = new List<PPCToolIssueDetailModel>();

		// UI/Utility
		public string Mode { get; set; } // e.g. "NEWENTRY" | "INSERT" | "UPDATE"
	}

	public class PPCToolIssueDetailModel
	{
		public long ToolIssueEntryId { get; set; }
		public long ToolIssueYearCode { get; set; }
		public long ToolEntryId { get; set; }
        public string ToolName { get; set; }
        public long ToolYearCode { get; set; }
		public string BarCode { get; set; }
		public string SerialNo { get; set; }
		public string ToolType { get; set; }
		public long? IssueQty { get; set; }
		public string Unit { get; set; }
		public string CurrentLocation { get; set; }
		public string IssueToLocation { get; set; }
		public string ToolCondition { get; set; }
		public DateTime? LastCalibrationDate { get; set; }
		public DateTime? NextCalibrationDueDate { get; set; }
		public long? RemainingToolLifeInMonths { get; set; }
		public long? ProdPlanEntryId { get; set; }
		public string ProdPlanNo { get; set; }
		public DateTime? ProdPlandate { get; set; }
		public long? ProdPlanYearCode { get; set; }
		public long? ProdSchEntryId { get; set; }
		public string ProdSchNo { get; set; }
		public DateTime? ProdSchDate { get; set; }
		public long? ProdSchYearCode { get; set; }
		public long? ForMachineId { get; set; }
		public string SpecialInstruction { get; set; }
		public string WillBeConsumedOrReturned { get; set; }
		public string PendingStatus { get; set; }
		public long? PendingQty { get; set; }
		public long? SeqNo { get; set; }

    }

}
