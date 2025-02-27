using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class IndentRegisterModel
    {
        public string Flag { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PartCode { get; set; }
        public string IndentNo { get; set; }
        public string ItemName { get; set; }
        public string ReportType { get; set; }
        public int YearCode { get; set; }
        public int AccountCode { get; set; }
        public int ParentAccountCode { get; set; }
        public string Mode { get; set; }

        public string IndentDate { get; set; }
        public string itemservice { get; set; }
        public string ItemNameOnly { get; set; }
        public string DeptName { get; set; }
        public string IndentorName { get; set; }
        public string CreatedByName { get; set; }
        public string MachineNo { get; set; }
        public string LastUpdationDate { get; set; }
        public string LastUpdatedName { get; set; }
        public string BOMtem { get; set; }
        public string BOMPartCode { get; set; }
        public string BOMIND { get; set; }
        public string BOMQty { get; set; }
        public string BOMRevNo { get; set; }
        public string Approved { get; set; }
        public string IndentCompleted { get; set; }
        public string canceled { get; set; }
        public string closed { get; set; }
        public string CC { get; set; }
        public string ApprovedDate { get; set; }

        public string MRPNO { get; set; }
        public string MRPEntryId { get; set; }
        public string MRPyearcode { get; set; }


        public string Specification { get; set; }
        public string ItemDescription { get; set; }
        public string IndentQty { get; set; }
        public string Unit { get; set; }
        public string PendQtyForPO { get; set; }
        public string ReqDate { get; set; }
        public string IndentRemark { get; set; }

        public string Model { get; set; }

        public string Size { get; set; }
        public string Color { get; set; }
        public string ItemRemark { get; set; }
        public string Approvalue { get; set; }
        public string AltQty { get; set; }
        public string AltUnit { get; set; }
        public string StoreName { get; set; }
        public string TotalStock { get; set; }
        public string Account_Name { get; set; }
        public string Account_Name2 { get; set; }
        public string TotalPOQty { get; set; }
        public string PendPOQty { get; set; }
        public string POVendorName { get; set; }
        public string PONO { get; set; }
        public string PODate { get; set; }

        

        public IList<IndentRegisterModel> IndentRegisterGrid { get; set; }
    }
}
