using eTactWeb.DOM.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AlternateItemMasterModel: AlternateItemMasterGridModel
    {
        public int FGItemCode { get; set; }
        public int MainItemCode { get; set; }
        public int ItemCode { get; set; }
        public string MainPartCode { get; set; }
        public string AltPartCode { get; set; }
        public string AlternatePartCode { get; set; }
        public int AlternateItemCode { get; set; }
        public int ApprovedBy { get; set; }
        public float MatchingFactorPercentage { get; set; }
        public string effectivedate { get; set; }
        public int ActualEntryByEmp { get; set; }
        public string ActualEntryDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public int UpdatedByEmp { get; set; }
        public string UpdationDate { get; set; }
        public string MachineName { get; set; }
        public IList<AlternateItemMasterGridModel>? AlternateItemMasterGrid { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string MainItemName { get; set; }
        public string MainPartyName { get; set; }
        public string AltItemName { get; set; }
        public string AltPartyName { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdatedByEmpName { get; set; }
        public int SrNO { get; set; }
        public int EntryByempId { get; set; }
        public int YearCode { get; set; }
        public string EffectiveDate { get; set; }

    }
    public class AlternateItemMasterGridModel: TimeStamp
    {
        public int FGItemCode { get; set; }
        public int MainItemCode { get; set; }
        public int ItemCode { get; set; }
        public string MainPartCode { get; set; }
        public string AltPartCode { get; set; }
        public string AltItemName { get; set; }
        public string MainItemName { get; set; }
        public int AlternateItemCode { get; set; }
        public int ApprovedBy { get; set; }
        public float MatchingFactorPercentage { get; set; }
        public string effectivedate { get; set; }
        public int ActualEntryByEmp { get; set; }
        public string ActualEntryDate { get; set; }
        public int UpdatedByEmp { get; set; }
        public string UpdationDate { get; set; }
        public string MachineName { get; set; }
        public int SrNO { get; set; }
        //public IList<AlternateItemMasterGridModel>? AlternateItemMasterGrid { get; set; }
    }
    public class AlternateItemMasterDashBoardModel : AlternateItemMasterDashBoardGridModel
    {
        public string FromDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate { get; set; }
        public string ToDate1 { get; set; }
        public string Searchbox { get; set; }
        public int FGItemCode { get; set; }
        public int MainItemCode { get; set; }
        public int ItemCode { get; set; }
        public string MainPartCode { get; set; }
        public int AltPartCode { get; set; }
        public int AlternateItemCode { get; set; }
        public int ApprovedBy { get; set; }
        public float MatchingFactorPercentage { get; set; }
        public string effectivedate { get; set; }
        public int ActualEntryByEmp { get; set; }
        public string ActualEntryDate { get; set; }
        public string UpdateByEmpName { get; set; }
        public int UpdatedByEmp { get; set; }
        public int EntryByempId { get; set; }
        public string UpdationDate { get; set; }
        public string MachineName { get; set; }
        public string Mode { get; set; }
        public IList<AlternateItemMasterDashBoardModel>? AlternateItemMasterDashBoardGrid { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string MainItemName { get; set; }
        public string MainPartyName { get; set; }
        public string AltItemName { get; set; }
        public string AltPartyName { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdatedByEmpName { get; set; }
        public int SrNO { get; set; }
        public int YearCode { get; set; }
        public string EffectiveDate { get; set; }
        public string AlternatePartCode { get; set; }
        public string ActualEntryBy { get; set; }
        public string LastUpdatedBy { get; set; }

    }
    public class AlternateItemMasterDashBoardGridModel : TimeStamp
    {
        public string FromDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate { get; set; }
        public string ToDate1 { get; set; }
        public string Searchbox { get; set; }
        public int FGItemCode { get; set; }
        public int MainItemCode { get; set; }
        public int ItemCode { get; set; }
        public string MainPartCode { get; set; }
        public int AltPartCode { get; set; }
        public int AlternateItemCode { get; set; }
        public int ApprovedBy { get; set; }
        public float MatchingFactorPercentage { get; set; }
        public string effectivedate { get; set; }
        public int ActualEntryByEmp { get; set; }
        public string ActualEntryDate { get; set; }
        public string UpdateByEmpName { get; set; }
        public int UpdatedByEmp { get; set; }
        public string UpdationDate { get; set; }
        public string MachineName { get; set; }
        // public IList<AlternateItemMasterGridModel>? AlternateItemMasterGrid { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string MainItemName { get; set; }
        public string MainPartyName { get; set; }
        public string AltItemName { get; set; }
        public string AltPartyName { get; set; }
        public string EntryByEmpName { get; set; }
        public string UpdatedByEmpName { get; set; }
        public int SrNO { get; set; }
        public int YearCode { get; set; }
        public string EffectiveDate { get; set; }

    }
}
