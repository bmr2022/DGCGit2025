using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class InProcessInspectionModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int YearCode { get; set; }
        public string CC { get; set; }
        public string? IPAddress { get; set; }
        public string Mode { get; set; }
        public string Entry_Date { get; set; }
        public string Control_PlanNo { get; set; }
        public string Flag { get; set; } = "";
        public int EntryId { get; set; }
        public string? EntryDate { get; set; }
        public string? EffectiveDate { get; set; }
        public string? TestingDate { get; set; }
        public int RevNo { get; set; }
        public string SlipNo { get; set; }
        public string InspTimeFrom { get; set; }
        public string InspTimeTo { get; set; }
        public int SampleSize { get; set; }
        public string ProjectDate { get; set; }
        public int ProjectYearCode { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string MRNNo { get; set; }
        public string? MRNDate { get; set; }
        public int MRNYearCode { get; set; }
        public string ProdDate { get; set; }
        public string? FML { get; set; }
        public int ProdYearCode { get; set; }
        public decimal MRNQty { get; set; }
        public decimal ProdQty { get; set; }
        public decimal InspActqty { get; set; }
        public decimal OkQty { get; set; }
        public decimal Rejqty { get; set; }
        public decimal Weight { get; set; }
        public string InpectionJudgement { get; set; }
        public string InspectedOther1 { get; set; }
        public string InspectedOther2 { get; set; }
        public string InspectedOther3 { get; set; }
        public string ProdSlipNo { get; set; }
        public int WCID { get; set; }
        public int WorkCenter { get; set; }
        public string ForInOutInprocess { get; set; } = "";
        public string Remark { get; set; } = "";
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string PartName { get; set; }
        
        public int AccountCode { get; set; }
        public int EngApprovedBy { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedByEmpName { get; set; }
        public int InspectedBy { get; set; }
        public string InspectedByEmpName { get; set; }
        public string Remarks { get; set; } = "";

        public int UId { get; set; }
        public string EntryByMachine { get; set; } = "";
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string? LastUpdationDate { get; set; }
        public int SeqNo { get; set; }
        public string Characteristic { get; set; }
        public string EvalutionMeasurmentTechnique { get; set; }
        public string SpecificationFrom { get; set; }
        public string Operator { get; set; }
        public string SpecificationTo { get; set; }
        public string FrequencyofTesting { get; set; }
        public string InspectionBy { get; set; }
        public string ControlMethod { get; set; }
        public string RejectionPlan { get; set; }

        
        public int CreatedBy { get; set; }
        public int seqNo { get; set; }
        public string ReportType { get; set; }
        public string Searchbox { get; set; }

		public string PartNo { get; set; }
		public string InspectionType { get; set; }
		public string CustomerName { get; set; }
		public string ProjectNo { get; set; }
		public string Color { get; set; }
		public string Material { get; set; }
		public int NoOfCavity { get; set; }
		public string MachineNo { get; set; }
		public string LotNo { get; set; }
		public string Shift { get; set; }
		public int ShiftID { get; set; }
		public string Time { get; set; }
		public string Date { get; set; }
		public string RevDate { get; set; }
		public int ProcessId { get; set; }
		public bool InspectedBeginingOfProd { get; set; }
		public bool InspectedAfterMoldCorrection { get; set; }
		public bool InspectedAfterMachineBreackDown { get; set; }
		public bool InspectedAfterLotChange { get; set; }
		public bool InspectedAfterMachinIdel { get; set; }
		public bool InspectedEndOfProd { get; set; }

		public int TotalRows { get; set; }
		public string Attachment1 { get; set; }
		public string Attachment2 { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<string>? Inspection { get; set; }
		
		
		
		public IList<string>? SelectedInspections { get; set; } 
		public List<InspectionOption> InspectionOptions { get; set; }
		//public IList<string>? BegingOfProduction { get; set; }
		//public IList<BegingOfProductionDetail>? BegingOfProductionDetailList { get; set; }
		//public IList<TextValue>? BegingOfProductionList { get; set; }

		public IList<InProcessInspectionDetailModel> DTSSGrid { get; set; }
    }

    public class InspectionOption
	{
		public string Key { get; set; }
		public string Text { get; set; }
	}

    public class InProcessInspectionDetailModel
    {
        public int? SeqNo { get; set; }
        public string Mode { get; set; }
        public string Remarks { get; set; }
        public int RevNo { get; set; }
        public string Material { get; set; }
        public string ForInOutInprocess { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int AccountCode { get; set; }
        public int EngApprovedBy { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsDeleted { get; set; }

		//public List<string> Samples { get; set; }
		public bool Copied { get; set; }
		public List<string>? Samples
		{
			get
			{
				return new List<string>()
		{
			InspValue1?.ToString() ?? string.Empty,
			InspValue2?.ToString() ?? string.Empty,
			InspValue3?.ToString() ?? string.Empty,
			InspValue4?.ToString() ?? string.Empty,
			InspValue5?.ToString() ?? string.Empty,
			InspValue6?.ToString() ?? string.Empty,
			InspValue7?.ToString() ?? string.Empty,
			InspValue8?.ToString() ?? string.Empty,
			InspValue9?.ToString() ?? string.Empty,
			InspValue10?.ToString() ?? string.Empty,
			InspValue11?.ToString() ?? string.Empty,
			InspValue12?.ToString() ?? string.Empty,
			InspValue13?.ToString() ?? string.Empty,
			InspValue14?.ToString() ?? string.Empty,
			InspValue15?.ToString() ?? string.Empty,
			InspValue16?.ToString() ?? string.Empty,
			InspValue17?.ToString() ?? string.Empty,
			InspValue18?.ToString() ?? string.Empty,
			InspValue19?.ToString() ?? string.Empty,
			InspValue20?.ToString() ?? string.Empty,
			InspValue21?.ToString() ?? string.Empty,
			InspValue22?.ToString() ?? string.Empty,
			InspValue23?.ToString() ?? string.Empty,
			InspValue24?.ToString() ?? string.Empty,
			InspValue25?.ToString() ?? string.Empty
		};
			}
			set
			{
				if (value == null || value.Count == 0) return;

				InspValue1 = string.IsNullOrEmpty(value[0]) ? null : value[0];
				InspValue2 = value.Count > 1 ? value[1] : null;
				InspValue3 = value.Count > 2 ? value[2] : null;
				InspValue4 = value.Count > 3 ? value[3] : null;
				InspValue5 = value.Count > 4 ? value[4] : null;
				InspValue6 = value.Count > 5 ? value[5] : null;
				InspValue7 = value.Count > 6 ? value[6] : null;
				InspValue8 = value.Count > 7 ? value[7] : null;
				InspValue9 = value.Count > 8 ? value[8] : null;
				InspValue10 = value.Count > 9 ? value[9] : null;
				InspValue11 = value.Count > 10 ? value[10] : null;
				InspValue12 = value.Count > 11 ? value[11] : null;
				InspValue13 = value.Count > 12 ? value[12] : null;
				InspValue14 = value.Count > 13 ? value[13] : null;
				InspValue15 = value.Count > 14 ? value[14] : null;
				InspValue16 = value.Count > 15 ? value[15] : null;
				InspValue17 = value.Count > 16 ? value[16] : null;
				InspValue18 = value.Count > 17 ? value[17] : null;
				InspValue19 = value.Count > 18 ? value[18] : null;
				InspValue20 = value.Count > 19 ? value[19] : null;
				InspValue21 = value.Count > 20 ? value[20] : null;
				InspValue22 = value.Count > 21 ? value[21] : null;
				InspValue23 = value.Count > 22 ? value[22] : null;
				InspValue24 = value.Count > 23 ? value[23] : null;
				InspValue25 = value.Count > 24 ? value[24] : null;
			}
		}

		//public List<string> ?Samples
		//      {
		//          get
		//          {
		//              return new List<string>()
		//      {
		//          InspValue1?.ToString() ?? string.Empty,
		//          InspValue2?.ToString() ?? string.Empty,
		//          InspValue3?.ToString() ?? string.Empty,
		//          InspValue4?.ToString() ?? string.Empty,
		//          InspValue5?.ToString() ?? string.Empty,
		//          InspValue6?.ToString() ?? string.Empty,
		//          InspValue7?.ToString() ?? string.Empty,
		//          InspValue8?.ToString() ?? string.Empty,
		//          InspValue9?.ToString() ?? string.Empty,
		//          InspValue10?.ToString() ?? string.Empty,
		//          InspValue11?.ToString() ?? string.Empty,
		//          InspValue12?.ToString() ?? string.Empty,
		//          InspValue13?.ToString() ?? string.Empty,
		//          InspValue14?.ToString() ?? string.Empty,
		//          InspValue15?.ToString() ?? string.Empty,
		//          InspValue16?.ToString() ?? string.Empty,
		//          InspValue17?.ToString() ?? string.Empty,
		//          InspValue18?.ToString() ?? string.Empty,
		//          InspValue19?.ToString() ?? string.Empty,
		//          InspValue20?.ToString() ?? string.Empty,
		//          InspValue21?.ToString() ?? string.Empty,
		//          InspValue22?.ToString() ?? string.Empty,
		//          InspValue23?.ToString() ?? string.Empty,
		//          InspValue24?.ToString() ?? string.Empty,
		//          InspValue25?.ToString() ?? string.Empty
		//      };
		//          }
		//      }

		public int UId { get; set; }
        public string EntryByMachine { get; set; } = "";
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string? LastUpdationDate { get; set; }
        public string? CC { get; set; }
        public bool? BegingOfProduction { get; set; }
        public bool? AfterMouldCorrection { get; set; }
        public bool? AfterMachineBreackDown { get; set; }
        public bool? AfterMaterialLotChange { get; set; }
        public bool? AfterMachineIdel { get; set; }
        public bool? EndOfProduction { get; set; }
        public int? InspEntryId { get; set; }
        public int? InspYearCode { get; set; }
		public string? InspValue1 { get; set; } 
		public string? InspValue2 { get; set; } 
		public string? InspValue3 { get; set; }
		public string? InspValue4 { get; set; } 
		public string? InspValue5 { get; set; } 
		public string? InspValue6 { get; set; } 
		public string? InspValue7 { get; set; } 
		public string? InspValue8 { get; set; } 
		public string? InspValue9 { get; set; } 
		public string? InspValue10 { get; set; } 
		public string? InspValue11 { get; set; } 
		public string? InspValue12 { get; set; } 
		public string? InspValue13 { get; set; } 
		public string? InspValue14 { get; set; } 
		public string? InspValue15 { get; set; } 
		public string? InspValue16 { get; set; } 
		public string? InspValue17 { get; set; } 
		public string? InspValue18 { get; set; } 
		public string? InspValue19 { get; set; } 
		public string? InspValue20 { get; set; } 
		public string? InspValue21 { get; set; }
		public string? InspValue22 { get; set; } 
		public string? InspValue23 { get; set; } 
		public string? InspValue24 { get; set; } 
		public string? InspValue25 { get; set; }
        public string Entry_Date { get; set; }
        public string? TestingDate { get; set; }
        public string SlipNo { get; set; }
        public string InspTimeFrom { get; set; }
        public string InspTimeTo { get; set; }
        public int SampleSize { get; set; }
        public string ProjectDate { get; set; }
        public int ProjectYearCode { get; set; }
        public int MachineId { get; set; }
        public string MRNNo { get; set; }
        public string? MRNDate { get; set; }
        public int MRNYearCode { get; set; }
        public string ProdDate { get; set; }
        public int ProdYearCode { get; set; }
        public decimal MRNQty { get; set; }
        public decimal ProdQty { get; set; }
        public decimal InspActqty { get; set; }
        public decimal OkQty { get; set; }
        public decimal Rejqty { get; set; }
        public decimal Weight { get; set; }
        public string LotNo { get; set; }
        public string ProdSlipNo { get; set; }
        public int NoOfCavity { get; set; }
        public string AccountName { get; set; }
        public string MachineName { get; set; }
        public string Color { get; set; }
        public string ProjectNo { get; set; }
        public string ShiftName { get; set; }
        public int ShiftId { get; set; }
        public string IncomInprocessOutgoing { get; set; }
        public string Characteristic { get; set; }
        public string EvalutionMeasurmentTechnique { get; set; }
        public string SpecificationFrom { get; set; }
        public string Operator { get; set; }
        public string SpecificationTo { get; set; }
        public string FrequencyofTesting { get; set; }
        public string InspectionBy { get; set; }
        public string ControlMethod { get; set; }
        public string RejectionPlan { get; set; }
        public string InspectionType { get; set; }
        public string Attachment1 { get; set; }
        public string Attachment2 { get; set; }
    }
}
