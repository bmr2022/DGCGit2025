using eTactWeb.DOM.Models.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    [Serializable]
    public  class SubVoucherModel: TimeStamp
    {
        public int PrefixEntryId { get; set; }
        public string MainVoucherName { get; set; }
        public List<SelectListItem> MainVoucherNameList { get; set; }
        public string MainVoucherTableName { get; set; }
        public string SubVoucherName { get; set; }
        public string VoucherRotationType { get; set; }
        public string StartSubVouchDiffSeries { get; set; }
        [StringLength(4, ErrorMessage = "Sub Voucher Prefix must be less than or equal to 4 characters.")]
        public string SubVouchPrefix { get; set; }
        public string FromYearPrefix { get; set; }
        public string ToYearPreFix { get; set; }
        public string MonthPrefix { get; set; }
        public string DayPrefix { get; set; }
        public string SeparatorApplicable { get; set; }
        public string Separator { get; set; }
        [Range(0, 10, ErrorMessage = "Length must be between 0 and 10.")]
        public int TotalLength { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryDate { get; set; }
        public string VoucherInvoice { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdationDate { get; set; }
        public string EntryByMachine { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string Mode { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdateByEmpName { get; set; }
        public IList<SubVoucherModel>? SubVoucherGrid { get; set; }
        //Back
        public int PrefixEntryIdBack { get; set; }
        public string MainVoucherNameBack { get; set; }
        public List<SelectListItem> MainVoucherNameListBack { get; set; }
        public string MainVoucherTableNameBack { get; set; }
        public string SubVoucherNameBack { get; set; }
        public string VoucherRotationTypeBack { get; set; }
        public string StartSubVouchDiffSeriesBack    { get; set; }

        public string SubVouchPrefixBack { get; set; }
        public string FromYearPrefixBack { get; set; }
        public string ToYearPreFixBack { get; set; }
        public string MonthPrefixBack { get; set; }
        public string DayPrefixBack { get; set; }
        public string SeparatorApplicableBack { get; set; }
        public string SeparatorBack { get; set; }
        public int TotalLengthBack { get; set; }
        public string ActualEntryByBack { get; set; }
        public string ActualEntryDateBack { get; set; }
        public string UpdatedByBack { get; set; }
        public DateTime? UpdationDateBack { get; set; }
        public string EntryByMachineBack { get; set; }
    }
    [Serializable]
    public class SubVoucherDashBoardModel : SubVoucherDashBoardGridModel
    {
        public int PrefixEntryId { get; set; }
        public string MainVoucherName { get; set; }
        public List<SelectListItem> MainVoucherNameList { get; set; }
        public string MainVoucherTableName { get; set; }
        public string SubVoucherName { get; set; }
        public string VoucherRotationType { get; set; }
        public string StartSubVouchDiffSeries { get; set; }
        [StringLength(4, ErrorMessage = "Sub Voucher Prefix must be less than or equal to 4 characters.")]
        public string SubVouchPrefix { get; set; }
        public string FromYearPrefix { get; set; }
        public string ToYearPreFix { get; set; }
        public string MonthPrefix { get; set; }
        public string DayPrefix { get; set; }
        public string SeparatorApplicable { get; set; }
        public string Separator { get; set; }
        [Range(0, 10, ErrorMessage = "Length must be between 0 and 10.")]
        public int TotalLength { get; set; }
        public string ActualEntryBy { get; set; }
        public string ActualEntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdationDate { get; set; }
        public string EntryByMachine { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string Mode { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdateByEmpName { get; set; }
        public string Searchbox { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }

       
    }
    [Serializable]
    public class SubVoucherDashBoardGridModel : TimeStamp
    {
        public int PrefixEntryId { get; set; }
        public string MainVoucherName { get; set; }
        public List<SelectListItem> MainVoucherNameList { get; set; }
        public string MainVoucherTableName { get; set; }
        public string SubVoucherName { get; set; }
        public string VoucherRotationType { get; set; }
        public string StartSubVouchDiffSeries { get; set; }
        [StringLength(4, ErrorMessage = "Sub Voucher Prefix must be less than or equal to 4 characters.")]
        public string SubVouchPrefix { get; set; }
        public string FromYearPrefix { get; set; }
        public string ToYearPreFix { get; set; }
        public string MonthPrefix { get; set; }
        public string DayPrefix { get; set; }
        public string SeparatorApplicable { get; set; }
        public string Separator { get; set; }
        [Range(0, 10, ErrorMessage = "Length must be between 0 and 10.")]
        public int TotalLength { get; set; }
        public string ActualEntryBy { get; set; }
        public string ActualEntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdationDate { get; set; }
        public string EntryByMachine { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string Mode { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdateByEmpName { get; set; }
        public string Searchbox { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public int EntryByEmpCode { get; set; }

        public IList<SubVoucherDashBoardGridModel>? SubVoucherDashBoardGrid{ get; set; }
    }
}
