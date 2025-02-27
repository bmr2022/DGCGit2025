using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class RCRegisterModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartyName { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? IssueChallanNo { get; set; }
        public string? recChallanno { get; set; }
        public string? IssueChallanType { get; set; }
        public string? RGPNRGP { get; set; }
        public string? ReportMode { get; set; }
        public IList<RCRegisterDetail>? RCRegisterDetails { get; set; }
    }

    public class RCRegisterDetail
    {
        public int AccountCode { get; set; }
        public string IssueChallanNo { get; set; }
        public string IssueChallanDate { get; set; }
        public string ChallanType { get; set; }
        public string RGPNRGP { get; set; }
        public string EntryTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string GSTType { get; set; }
        public string Completed { get; set; }
        public string FullyReturned { get; set; }
        public string Vehicleno { get; set; }
        public string remark { get; set; }
        public string Transporter { get; set; }
        public string TransportMode { get; set; }
        public string distance { get; set; }
        public string ewaybillno { get; set; }
        public string closechallan { get; set; }
        public string CloseChallandate { get; set; }
        public int YearCode { get; set; }
        public int EntryId { get; set; }
        public int ItemCode { get; set; }
        public string BatchNo { get; set; }
        public string uniquebatchno { get; set; }
        public string PONO { get; set; }
        public string PODate { get; set; }
        public int SeqNo { get; set; }
        public int HSNNO { get; set; }
        public string unit { get; set; }
        public float rate { get; set; }
        public float Amount { get; set; }
        public float PurchasePrice { get; set; }
        public string purpose { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public float IsschallanQty { get; set; }
        public float PendQty { get; set; }
        public float ActualPendQty { get; set; }
        public string ItemRemark { get; set; }
        public string Account_Name { get; set; }
        public string PartCode { get; set; }
        public string Item_Name { get; set; }
        public string GateNo { get; set; }
        public string GateDate { get; set; }
        public string MRNNO { get; set; }
        public string MRNDate { get; set; }
        public string BillChallan { get; set; }
        public float RecQty { get; set; }
        public float GateQty { get; set; }
        public float IssuedQty { get; set; }
        public string Produced { get; set; }
        public string  AgainstChallanNo{ get; set; }
        public int  AgainstChallanYearCode{ get; set; }
        public int  AgainstChallanEntryId{ get; set; }
        public string entrybymachine { get; set; }
        public string DocTypeDesc { get; set; }
        public int MRNYear { get; set; }
        public int GateYear { get; set; }
        public string RecChallanNo { get; set; }
        public string RecChallanDate { get; set; }
        
    }
}
