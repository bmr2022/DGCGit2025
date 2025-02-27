using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PrimaryAccountGroupMasterModel: TimeStamp
    {
        public int Account_Code { get; set; }
        public int EnteredEMPID { get; set; }
        public string EntryByMachineName { get; set; }
        public string Account_Name { get; set; }
        public int Parent_Account_Code { get; set; }
        public string ParentAccountName { get; set; }
       // public string Parent_Account_Code { get; set; }
        public string Main_Group { get; set; }
        public float Account_Type { get; set; }
        public string SubGroup { get; set; }
        public int SubSubGroup { get; set; }
        public string UnderGroup { get; set; }
        public string Balance_sheet_Category { get; set; }
        public string Allow_sub_ledger { get; set; }
        public int TB_Seq { get; set; }
        public int P_L_Seq { get; set; }
        public int B_S_Seq { get; set; }
        public int Annexture_No { get; set; }
        public string Deb_Code { get; set; }
        public string Active { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public IList<PrimaryAccountGroupMasterModel>? PrimaryAccountGroupMasterGrid { get; set; }
    }
    public class PrimaryAccountGroupMasterDashBoardModel: TimeStamp
    {
        public int Account_Code { get; set; }
        public int EnteredEMPID { get; set; }
        public string EntryByMachineName { get; set; }
        public string Account_Name { get; set; }
        public string ParentAccountName { get; set; }
        public int Parent_Account_Code { get; set; }
       // public string Parent_Account_Code { get; set; }
        public string Main_Group { get; set; }
        public float Account_Type { get; set; }
        public string SubGroup { get; set; }
        public int SubSubGroup { get; set; }
        public string UnderGroup { get; set; }
        public string Balance_sheet_Category { get; set; }
        public string Allow_sub_ledger { get; set; }
        public int TB_Seq { get; set; }
        public int P_L_Seq { get; set; }
        public int B_S_Seq { get; set; }
        public int Annexture_No { get; set; }
        public string Deb_Code { get; set; }
        public string Active { get; set; }
        public string Searchbox { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public IList<PrimaryAccountGroupMasterDashBoardModel>? PrimaryAccountGroupMasterDashBoardGrid { get; set; }
    }
    }
