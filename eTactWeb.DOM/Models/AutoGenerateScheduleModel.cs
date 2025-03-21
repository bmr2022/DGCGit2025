using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AutoGenerateScheduleModel:TimeStamp
    {
        public int  MrpEntryId { get; set; }
        public int MrpYearCode { get; set; }
        public string CC { get; set; }
        public int MrpNo { get; set; }
        public int UID { get; set; }
        public int YearCode { get; set; }
        public int AccountCode { get; set; }
        public int SoEntryId { get; set; }
        public string Sono { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string VendorName { get; set; }
        public int VendSchOty { get; set; }
        public string MachineName { get; set; }
        public string CustomerOrderNo { get; set; }
        public long SchEntryId { get; set; }
        public string SchNo { get; set; }
        public long SchYearCode { get; set; }
        public string SchEffectivedate { get; set; }
        public long SoItemCode { get; set; }
        public string Unit { get; set; }
        public double SoQty { get; set; }
        public string DeliveryDate { get; set; }
        public int BusinessPer { get; set; }
        public int VendCustAccCode  { get; set; }
        public int NoOfVend  { get; set; }
        public string PONO  { get; set; }
        public string PoDate  { get; set; }
        public string ScheduleDate { get; set; }
        public int SchAmendNo { get; set; }
        public string ScheduleEffectiveFromDate { get; set; }
        public string ScheduleEffectiveTillDate { get; set; }
        public int MRPNO { get; set; }
        public int MRPNoYearCode { get; set; }
        public int MRPEntry_Id { get; set; }
        public int PoYearCode { get; set; }
        public float PoRate  { get; set; }
        public long RmItemCode { get; set; }
        public double BomQty { get; set; }
        public double RmReqQty { get; set; }
        public double ActualReqQty { get; set; }
        public double ReqRmQty { get; set; }
        public double StockQty { get; set; }
        public double WipStockQty { get; set; }
        public string OrderDate { get; set; }
        public double FirstMonth { get; set; }
        public double SecondMonth { get; set; }
        public double ItemRate { get; set; }
        public double RemainingStock { get; set; }
        public double NewOrderQty { get; set; }
        public string ReportType { get; set; }
        public IList<AutoGenerateScheduleModel> AutoGenerateScheduleGrid { get; set; }
    }
}
