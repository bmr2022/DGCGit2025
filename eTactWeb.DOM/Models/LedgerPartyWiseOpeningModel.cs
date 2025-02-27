using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public  class LedgerPartyWiseOpeningModel: LedgerPartyWiseOpeningDetailModel
    {
        public string Flag { get; set; }
        public int EntryId { get; set; }
        public int AccBookTransEntryId { get; set; }
        public int AccBookTransYearCode { get; set; }
        public string EntryDate { get; set; }
        public int  ActualEntryByEmp { get; set; }
        public string EntryByEmpName { get; set; }
        public string LedgerName { get; set; }
        public bool ShowAllParty { get; set; }
       // public string Balance { get; set; }
        public float Balance { get; set; }
        public string DRCR { get; set; }
        public string? BillNo { get; set; }
        public int BillYear { get; set; }
        public string BillDate { get; set; }
        public string DueDate { get; set; }
        public float BillNetAmt { get; set; }
        public float PendAmt { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public double OpeningAmt { get; set; }
        public long AccountCode { get; set; }
        public long OpeningYearCode { get; set; }
        public long UpdatedBy { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string EntryByMachine { get; set; } 
        public long Uid { get; set; }
        public long ActualEntryBy { get; set; }
        public DateTime? ActualEntryDate { get; set; }
        public string? LedgerOpnEntryDate { get; set; }
        public string Searchbox { get; set; }
        public int UpdatedByEmp { get; set; }
        public string LastUpdatedBy { get; set; }
        public string UpdationDate { get; set; }
        public List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetails { get; set; } 
    }

    public class LedgerPartyWiseOpeningDetailModel : TimeStamp
    {
        public string InvoiceNo { get; set; } 
        public int SrNO { get; set; } 
        public DateTime? InvoiceDate { get; set; }
        public double InvNetAmt { get; set; }
        public double InvPendAmt { get; set; }
        public string DrCrType { get; set; } 
        public string TransactionType { get; set; }
        public string? DueDate { get; set; }
        public string CC { get; set; } 
        public string LedgerName { get; set; } 
        public string FromDate { get; set; } 
        public string ToDate { get; set; } 
        public long ActualEntryBy { get; set; }
        public string? ActualEntryDate { get; set; }
        public long UpdatedBy { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string EntryByMachine { get; set; } 
        public string AccountNarration { get; set; }
        public string? BillNo { get; set; }
        public int BillYear { get; set; }
        public int EntryId { get; set; }
        public string US { get; set; }
        public string Adjustment { get; set; }
        public string BillDate { get; set; }
        public double BillNetAmt { get; set; }
        public double PendAmt { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public double OpeningAmt { get; set; }
        public long AccountCode { get; set; }
        public int AccBookTransEntryId { get; set; }
        public int AccBookTransYearCode { get; set; }
        public long OpeningYearCode { get; set; }
    } 
    public class LedgerPartyWiseOpeningDashBoardModel : TimeStamp
    {
        public string InvoiceNo { get; set; } 
        public int SrNO { get; set; } 
        public string? InvoiceDate { get; set; }
        public string? AccountName { get; set; }
        public double InvNetAmt { get; set; }
        public double InvPendAmt { get; set; }
        public string DrCrType { get; set; } 
        public string TransactionType { get; set; }
        public string? DueDate { get; set; }
        public string CC { get; set; } 
        public string LedgerName { get; set; } 
        public string FromDate { get; set; } 
        public string ToDate { get; set; } 
        public long ActualEntryBy { get; set; }
        public string? ActualEntryDate { get; set; }
        public long UpdatedBy { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string EntryByMachine { get; set; } 
        public string AccountNarration { get; set; }
        public string? BillNo { get; set; }
        public int BillYear { get; set; }
        public int EntryId { get; set; }
        public string US { get; set; }
        public string Adjustment { get; set; }
        public string BillDate { get; set; }
        public decimal BillNetAmt { get; set; }
        public decimal PendAmt { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public string Searchbox { get; set; }
        public double OpeningAmt { get; set; }
        public long AccountCode { get; set; }
        public int AccBookTransEntryId { get; set; }
        public int AccBookTransYearCode { get; set; }
        public long OpeningYearCode { get; set; }
        public List<LedgerPartyWiseOpeningDashBoardModel> LedgerPartyWiseOpeningDashBoardDetail { get; set; }

    }
}

