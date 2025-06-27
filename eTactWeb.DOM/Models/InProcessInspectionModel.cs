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
        public int Yearcode { get; set; }
        public string CC { get; set; }
        public string Mode { get; set; }
        public string Entry_Date { get; set; }
        public string Control_PlanNo { get; set; }
        public string Flag { get; set; } = "";
        public int EntryId { get; set; }
        public string? EntryDate { get; set; }
        public string? EffectiveDate { get; set; }
        public string? TestingDate { get; set; }
        public int YearCode { get; set; }

        public string RevNo { get; set; }
        public string SlipNo { get; set; }
        public string InspTimeFrom { get; set; }
        public string InspTimeTo { get; set; }
        public string SampleSize { get; set; }
        public string ProjectDate { get; set; }
        public int MachineId { get; set; }
        public string MRNNo { get; set; }
        public string MRNDate { get; set; }
        public int MRNYearCode { get; set; }
        public string ProdDate { get; set; }
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

        public string ItemimagePath { get; set; }
        public bool CopyControlPlan { get; set; }
        public string DrawingNo { get; set; }
        public string DrawingNoImagePath { get; set; }
        public string ImageURL { get; set; }
        public IFormFile? UploadImage { get; set; }
        public IFormFile? ItemImage { get; set; }
        public string? ItemImageURL { get; set; }
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
		public string NoOfCavity { get; set; }
		public string MachineNo { get; set; }
		public string LotNo { get; set; }
		public string Shift { get; set; }
		public int ShiftID { get; set; }
		public string Time { get; set; }
		public string Date { get; set; }
		public string RevDate { get; set; }
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
        public int SeqNo { get; set; }
        public string Mode { get; set; }
        public string? CntPlanEntryDate { get; set; }
        public int CntPlanEntryId { get; set; }
        public int CntPlanYearCode { get; set; }
        public string Characteristic { get; set; }
        public string EvalutionMeasurmentTechnique { get; set; }
        public string SpecificationFrom { get; set; }
        public string Operator { get; set; }
        public string SpecificationTo { get; set; }
        public string FrequencyofTesting { get; set; }
        public string InspectionBy { get; set; }
        public string ControlMethod { get; set; }
        public string RejectionPlan { get; set; }
        public string Remarks { get; set; }
        public string ItemimagePath { get; set; }
        public string DrawingNo { get; set; }
        public string DrawingNoImagePath { get; set; }
        public string ImageURL { get; set; }
        public string Control_PlanNo { get; set; }
        public string RevNo { get; set; }
        public string ForInOutInprocess { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int AccountCode { get; set; }
        public int EngApprovedBy { get; set; }
        public int ApprovedBy { get; set; }


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

    }
}
