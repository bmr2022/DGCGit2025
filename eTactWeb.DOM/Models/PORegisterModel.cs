using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PORegisterModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartyName { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? PONO { get; set; }
        public string? SchNo { get; set; }
        public string? OrderType { get; set; }
        public string? POFor { get; set; }
        public string ItemGroup { get; set; }
        public string? ItemType { get; set; }
        public string? ReportMode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string showOnlyCompletedPO { get; set; }
        public string showClosedPO { get; set; }
        public string showOnlyActivePO { get; set; }
        public IList<PORegisterDetail>? PORegisterDetails { get; set; }
    }

    public class PORegisterDetail
    {
        public string Account_name { get; set; }
        public string DOMESTICIMPORT { get; set; }
        public string PONO { get; set; }
        public int POYearCode { get; set; }
        public int HSNNO { get; set; }
        public string PODate { get; set; }
        public string POtype { get; set; }
        public string POclosedate { get; set; }
        public string poAmmeffdate { get; set; }
        public string ordertype { get; set; }        
        public string POFOR { get; set; }
        public string SchNO { get; set; }
        public string Schdate { get; set; }
        public int SchYear { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string Currency { get; set; }
        public float PORate { get; set; }
        public float POAmount { get; set; }
        public float GSTAmt { get; set; }
        public float GSTPer { get; set; }
        public string POQty { get; set; }
        public string MRNNo { get; set; }
        public string MRNDATE { get; set; }
        public string GateNo { get; set; }
        public int GateYearCode { get; set; }
        public int POEntryId { get; set; }
        public string GateDate { get; set; }
        public string BillQty { get; set; }
        public string RECQty { get; set; }
        public string RecValue { get; set; }
        public string AcceptedQty { get; set; }
        public string InvNo { get; set; }
        public string INVDate { get; set; }
        public string MIRNo { get; set; }
        public string MIRDate { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchno { get; set; }
        public string ItemGroup { get; set; }
        public string ItemCategory { get; set; }


        //summary
        public string Vendor { get; set; }        
        public string ScheduleEffectiveFromDate { get; set; }        
        public string ScheduleEffectiveTillDate { get; set; }                   
        public string POEffDate { get; set; }
        public string POEndDate { get; set; }
        public string POValue { get; set; }
        public string QCOKQty { get; set; }
        public string PendQty { get; set; }
        public string MinLevel { get; set; }
        public string unit { get; set; }
        public string Rate { get; set; }
        public string SchQty { get; set; }
        public string DisPer { get; set; }
        public string DisAmt { get; set; }
        public string ItemAmount { get; set; }
        public string Ammendmentno { get; set; }
        public string Ammendmentdate { get; set; }
        public string Assrate { get; set; }
        public string Deliveryterms { get; set; }
        public string deliveryDate { get; set; }
        public string Ammno { get; set; }
        public string AmmDate { get; set; }
        public string Minlvldays { get; set; }

        //SUMMRATEING

        public string BalQty { get; set; }
        public string BalValue { get; set; }
        public string Rating { get; set; }               
        public string PartyName { get; set; }



    }
}
