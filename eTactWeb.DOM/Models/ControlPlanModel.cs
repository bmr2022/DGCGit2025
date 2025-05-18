using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ControlPlanModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Yearcode { get; set; }
        public string CC { get; set; }
        public string Mode { get; set; }
        public string Entry_Date { get; set; }
        public string Control_PlanNo { get; set; }
        public string Flag { get; set; } = "";
        public int CntPlanEntryId { get; set; }
        public string? CntPlanEntryDate { get; set; }
        public string? EffectiveDate { get; set; }
        public int CntPlanYearCode { get; set; }
        public int ControlPlanNo { get; set; } 
        public string RevNo { get; set; }
        public string ForInOutInprocess { get; set; } = "";
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int AccountCode { get; set; }
        public int EngApprovedBy { get; set; }
        public int ApprovedBy { get; set; }
        public string Remarks { get; set; } = "";
     
        public int UId { get; set; }
        public string EntryByMachine { get; set; } = "";
        public int ActualEntryBy { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedBy { get; set; }
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
        public string DrawingNo { get; set; }
        public string DrawingNoImagePath { get; set; }
        public List<ControlPlanDetailModel> DTSSGrid { get; set; }
    }
    public class ControlPlanDetailModel
    {
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
        public string Remarks { get; set; }
        public string ItemimagePath { get; set; }
        public string DrawingNo { get; set; }
        public string DrawingNoImagePath { get; set; }
    }

}
