using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingToReceiveItemModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public string? FromWorkCenter {  get; set; }
        public string? ToStoreName {  get; set; }
        public string? ProdSlipNo {  get; set; }
        public string? ProdUnProd {  get; set; }
        public int SeqNo {  get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? FromWorkCenterList { get; set; }
        public IList<TextValue>? ProdSlipNoList { get; set; }
        public IList<TextValue>? ToStoreNameList { get; set; }
        public IList<TextValue>? ProdTypeList { get; set; }
    }
    public class DisplayPendToReceive
    {
        public int TransferMatEntryId {  get; set; }
        public int TransferMatYearCode {  get; set; }
        public int ItemCode {  get; set; }
        public string? IssueToStoreWC {  get; set; }
    }
}
