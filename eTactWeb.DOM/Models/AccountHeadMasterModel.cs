using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccountHeadMasterModel : TimeStamp
    {
        public int Account_Code { get; set; }
        public string Account_Name { get; set; }
        public int Parent_Account_Code { get; set; }
        public string Main_Group { get; set; }
        public int OpeningForYear { get; set; }
        public string? EntryByMachine { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? CC { get; set; }
        public int EntryByEmpId { get; set; }
        public int UpdatedByEmpId { get; set; }
        public string? Updationdate { get; set; }
        public decimal Amount { get; set; }
        public float Account_Type { get; set; }
        public string SubGroup { get; set; }
        public string UnderGroup { get; set; }
        public string Balance_sheet_Category { get; set; }
        public string Allow_sub_ledger { get; set; }
        public int TB_Seq { get; set; }
        public int P_L_Seq { get; set; }
        public int B_S_Seq { get; set; }
        public int Annexture_No { get; set; }
        public string Deb_Code { get; set; }
        public string Active { get; set; }
        public string? GroupName {  get; set; }
        public int AccountCode {  get; set; }
        public string? LedgerName {  get; set; }
        public int GroupAccountCode {  get; set; }
        public decimal PreviousAmount {  get; set; }
        public string? DrCr {  get; set; }
        public string? EntryByEmpName {  get; set; }
        public string? UpdateByEmpName { get; set; }
        public string? FromDateBack {  get; set; }
        public string? ToDateBack { get; set; }
        public string? GroupNameBack {  get; set; }
        public string? LedgerNameBack {  get; set; }
        public string? OpeningForYearBack { get; set; }
        public string? PreviousAmountBack {  get; set; }
        public string? DrCrBack { get; set; }
        public string? GlobalSearchBack { get; set; }

    }
}
