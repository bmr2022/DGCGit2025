using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class POApprovalPolicyModel
    {
        public string Flag { get; set; }
        public string Mode { get; set; }
        public string EntryDate { get; set; }
        public int YearCode { get; set; }

        public int POApprovalEntryId { get; set; }

        public string POTYPE { get; set; }
        public int ItemGroupId { get; set; }
        public int ItemCategoryId { get; set; }
        public int Itemcode { get; set; }

        public decimal FromAmt { get; set; }
        public decimal ToAmt { get; set; }

        public string FirstApprovalRequired { get; set; }           
        public string FinalApprovalRequired1 { get; set; }       
        public string OnlyDirectorApproval { get; set; }          
        public string ONLY1stApprovalRequired { get; set; }   
        public string OnlyFinalApprovalRequired { get; set; }     
        public string All3ApprovalRequired { get; set; }           

   
        public int EmpidForFirstApproval1 { get; set; }
        public int EmpidForFirstApproval2 { get; set; }
        public int EmpidForFirstApproval3 { get; set; }

        public int EmpidForFinalApproval1 { get; set; }
        public int EmpidForFinalApproval2 { get; set; }
        public int EmpidForFinalApproval3 { get; set; }

        public int EmpidForMgmtApproval1 { get; set; }
        public int EmpidForMgmtApproval12 { get; set; }
        public int EmpidForMgmtApproval13 { get; set; }

      
        public string EntryByMachine { get; set; }
        public int ActualEntryByEmpId { get; set; }
        public string ActualEntryDate { get; set; }
        public string ActualEntryByName { get; set; }

        public int LastUpdatedByEmpId { get; set; }
        public string LastUpdatedDate { get; set; }
        public string LastUpdatedByName { get; set; }

        public string CC { get; set; }

        public string GroupName { get; set; }
        public string CatName { get; set; }
        public string ItemName { get; set; }

      
        public string StatusText { get; set; }
        public int StatusCode { get; set; }
        public string Result { get; set; }

      
        public int GroupCode { get; set; }
        public int CatId { get; set; }

        public int EntryId { get; set; }
        public string Type_Item { get; set; }

        public string PartCode { get; set; }
        public int ItemCode{ get; set; }

        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
