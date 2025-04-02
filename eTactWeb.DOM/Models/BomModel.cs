using System.ComponentModel.DataAnnotations;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class BomDashboard
    {
        public DataTable? DTDashboard { get; set; }
        public string? FGItemName { get; set; }
        public int? FGItemCode { get; set; }
        public IList<TextValue>? FGItemNameList { get; set; }
        public string? FGPartCode { get; set; }
        public IList<TextValue>? FGPartCodeList { get; set; }
        public string? RMItemName { get; set; }
        public IList<TextValue>? RMItemNameList { get; set; }
        public string? RMPartCode { get; set; }
        public IList<TextValue>? RMPartCodeList { get; set; }
        public string? BomRevNo { get; set; }
        public string? DashboardType { get; set; }
    }

    public class BomModel : TimeStamp
    {
      public int ? BMNo { get; set; }
        public string? AICName1 { get; set; }

        public string? AICName2 { get; set; }

        public int AltItemCode1 { get; set; }

        public int AltItemCode2 { get; set; }

        public string? AltItemName1 { get; set; }

        public string? AltItemName2 { get; set; }

        public decimal AltQty1 { get; set; }

        public decimal AltQty2 { get; set; }

        public string? ApprovedBy { get; set; }

        public IList<TextValue>? ApprovedByList { get; set; }

        public IList<BomModel>? BomList { get; set; }

        [Required]
        public string? BOMName { get; set; }

        [Required]
        public int BomNo { get; set; }

        [Required]
        public decimal BomQty { get; set; }

        public decimal BurnQty { get; set; }

        public string? CC { get; set; }

        public IList<TextValue>? CodeList { get; set; }
        public bool CopyBom { get; set; }
        public string? DirectProcess { get; set; }

        public IList<SelectListItem> DirectProcessList { get; set; } = new List<SelectListItem>()
            {
                new() { Value = "Direct", Text = "Direct" },
                new() { Value = "Process", Text = "Process" },
            };

        [Required]
        public string? EffectiveDate { get; set; }

        [Required]
        public string? EntryDate { get; set; }
        public string? CustJwAdjustmentMandatory { get; set; }
        public string? Location { get; set; }
        public string? MPNNo { get; set; }
        public string? CustJWmandatory { get; set; }
        public bool FG1 { get; set; }
        public IList<TextValue>? FG1CodeList { get; set; }
        public IList<TextValue>? FG1NameList { get; set; }
        public bool FG2 { get; set; }
        public bool FG3 { get; set; }

        [Required]
        public string? FinishedItemName { get; set; }

        [Required]
        public int FinishItemCode { get; set; }

        public decimal GrossWt { get; set; }
        public string? ICName { get; set; }
        public string? IssueToJOBwork { get; set; }
        public int ItemCode { get; set; }
        public string? ItemName { get; set; }
        public IList<TextValue>? NameList { get; set; }
        public decimal NetWt { get; set; }
        public string? PkgItem { get; set; }
        public decimal Qty { get; set; }
        public string? RecFrmCustJobwork { get; set; }
        public string? Remark { get; set; }
        public int RunnerItemCode { get; set; }
        public decimal RunnerQty { get; set; }
        public decimal Scrap { get; set; }
        public int SeqNo { get; set; }
        public string? UID { get; set; }
        public string? Unit { get; set; }
        public string? UsedStageId { get; set; }
        public IList<TextValue>? UsedStageList { get; set; }
        public int YearCode { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? FGPartCodeBack { get; set; }
        public string? FGItemNameBack { get; set; }
        public string? RMPartCodeBack { get; set; }
        public string? RMItemNameBack { get; set; }
        public string? BomRevNoBack { get; set; }
        public string? SummaryDetailBack { get; set; }
        public string? GlobalSearchBack { get; set; }
        public IList<SelectListItem> YesNo { get; set; } = new List<SelectListItem>()
            {
                new() { Value = "Y", Text = "Yes" },
                new() { Value = "N", Text = "No" },
            };

        public IList<BomViewModel> ExcelDataList { get; set; }
    }
    public class BomViewModel // Excel Data
    {
        public int SeqNo { get; set; }
        public string? FGPartCode { get; set; }
        public string? FGItemName { get; set; }
        public int FGItemCode { get; set; }
        public string? CustJwAdjustmentMandatory { get; set; }
        public string? RMPartCode { get; set; }
        public string? RMItemName { get; set; }
        public int RMItemCode { get; set; }
        public string? BomName { get; set; }
        public int BomNo { get; set; }
        public decimal? RMQty { get; set; }
        public string? RMUnit { get; set; }
        public string? Location { get; set; }
        public string? MPNNumber { get; set; }
        public string? CustJWmandatory { get; set; }
        public string? AltPartCode1 { get; set; }
        public int? AltItemCode1 { get; set; }
        public string? AltPartCode2 { get; set; }
        public int? AltItemCode2 { get; set; }
        public decimal? AltQty1 { get; set; }
        public decimal? AltQty2 { get; set; }
        public decimal? Scrap { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string? Remark { get; set; }
        public string? ConstraintExists { get; set; }
    }

    public class ImportBomData
    {
        public int SeqNo { get; set; }
        public string FGPartCode { get; set; }
        public string RMPartCode { get; set; }
        public float BomQty { get; set; }
        public string   ScrapPartCode { get; set; }
        public string   ByProdPartCode { get; set; }
    }
}