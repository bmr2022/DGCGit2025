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
    public class LedgerOpeningEntryModel : LedgerOpeningEntryGridModel
    {
        public int SrNO { get; set; }
        public int AccountCode { get; set; }
        public int GroupAccountCode { get; set; }
        public int ParentAccountCode { get; set; }
        public int OpeningForYear { get; set; }
        public string DrCr { get; set; }
        public float Amount { get; set; }
        public string CC { get; set; }
        public int EntryByEmpId { get; set; }
        public string? EntryByEmpName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string? UpdateByEmpName { get; set; }
        public string Updationdate { get; set; }
        public string EntryByMachine { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0.")]
        public float PreviousAmount { get; set; }

        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? Mode { get; set; }
        public string? Account_Name { get; set; }
        public string? GroupName { get; set; }
        public int GroupCode { get; set; }
        public string? LedgerName { get; set; }
        public int? CreatedBy { get; set; }
        public int? ClosingYearCode { get; set; }
        public string? FromDateBack { get; set; }
        public string? ToDateBack { get; set; }
        public string? GroupNameBack { get; set; }
        public string? LedgerNameBack { get; set; }
        public string? OpeningForYearBack { get; set; }
        public string? PreviousAmountBack { get; set; }
        public string? DrCrBack { get; set; }
        public string? GlobalSearchBack { get; set; }

        public IList<LedgerOpeningEntryModel>? LedgerOpeningEntryGrid { get; set; }
    }
    public class LedgerOpeningEntryGridModel : TimeStamp
    {
        public int AccountCode { get; set; }
        public int GroupAccountCode { get; set; }
        public int SrNO { get; set; }
        public int ParentAccountCode { get; set; }
        public int OpeningForYear { get; set; }
        public string DrCr { get; set; }
        public float Amount { get; set; }
        public string CC { get; set; }
        public int EntryByEmpId { get; set; }
        public string ActualEntryDate { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string Updationdate { get; set; }
        public string EntryByMachine { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0.")]
        public float PreviousAmount { get; set; }

        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? Mode { get; set; }
        public string? Account_Name { get; set; }
        public string? GroupName { get; set; }
        public string? LedgerName { get; set; }
        public int? CreatedBy { get; set; }
        public int? ClosingYearCode { get; set; }

    }
    [Serializable]
    public class LedgerOpeningEntryDashBoardModel : LedgerOpeningEntryDashBoardGridModel
    {
        public int SrNO { get; set; }
        public int AccountCode { get; set; }
        public int GroupAccountCode { get; set; }
        public int ParentAccountCode { get; set; }
        public int OpeningForYear { get; set; }
        public string DrCr { get; set; }
        public float Amount { get; set; }
        public string CC { get; set; }
        public int EntryByEmpId { get; set; }
        public string ActualEntryDate { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string Updationdate { get; set; }
        public string EntryByMachine { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0.")]
        public float PreviousAmount { get; set; }

        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? Mode { get; set; }
        public string? Account_Name { get; set; }
        public string? GroupName { get; set; }
        public string? LedgerName { get; set; }
        public int? CreatedBy { get; set; }
        public string FromDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate { get; set; }
        public string ToDate1 { get; set; }
        public string Searchbox { get; set; }
        public int? ClosingYearCode { get; set; }

        //  public IList<LedgerOpeningEntryDashBoardGridModel>? LedgerOpeningEntryDashBoardGrid { get; set; }
    }
    [Serializable]
    public class LedgerOpeningEntryDashBoardGridModel
    {
        public int SrNO { get; set; }
        public int ID { get; set; }
        public int AccountCode { get; set; }
        public int GroupAccountCode { get; set; }
        public int ParentAccountCode { get; set; }
        public int OpeningForYear { get; set; }
        public string DrCr { get; set; }
        public float Amount { get; set; }
        public string CC { get; set; }
        public int EntryByEmpId { get; set; }
        public string ActualEntryDate { get; set; }
        public string EntryByEmployee { get; set; }
        public string UpdatedByEmployee { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string Updationdate { get; set; }
        public string EntryByMachine { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0.")]
        public float PreviousAmount { get; set; }

        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? Mode { get; set; }
        public string? Account_Name { get; set; }
        public string? GroupName { get; set; }
        public string? LedgerName { get; set; }
        public int? CreatedBy { get; set; }
        public int? ClosingYearCode { get; set; }

        public IList<LedgerOpeningEntryDashBoardGridModel>? LedgerOpeningEntryDashBoardGrid { get; set; }
    }
}
