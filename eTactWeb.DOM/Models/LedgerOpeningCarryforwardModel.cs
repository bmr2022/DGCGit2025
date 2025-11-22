using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTactWeb.DOM.Models
{
    [Serializable]
    public class LedgerOpeningCarryforwardModel : LedgerOpeningCarryforwardDashBoardGridModel
    {
        public int LoggedInFinYear { get; set; }
        public int CarryForwardClosing { get; set; }
        public DateTime? ActualTransferDate { get; set; }
        public string FromDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate { get; set; }
        public string ToDate1 { get; set; }
        public string Searchbox { get; set; }
    }

    [Serializable]
    public class LedgerOpeningCarryforwardDashBoardGridModel
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
        public int ActualTransferBy { get; set; }
        public int EntryByEmpId { get; set; }
        public string? EntryByEmployee { get; set; }
        public string? UpdatedByEmployee { get; set; }

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

        public IList<LedgerOpeningCarryforwardDashBoardGridModel>? LedgerOpeningCarryforwardDashBoardGrid { get; set; }
    }
}
