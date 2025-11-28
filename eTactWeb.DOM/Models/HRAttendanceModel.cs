using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

/// <inheritdoc/>

[Serializable()]
public class HRAttendanceModel
{
}

public class HRAListDataModel
{
    public int? HRAListDataSeqNo { get; set; }
    public string? AttandanceDate { get; set; }
    public string? PartyName { get; set; }
    public IList<TextValue>? PartyNameList { get; set; }
    public string? MRNNo { get; set; }
    public IList<TextValue>? MRNNoList { get; set; }
    public int? MRNYearCode { get; set; }
    public DateTime? MRNEntryDate { get; set; }
    public string? PONO { get; set; }
    public IList<TextValue>? PONOList { get; set; }
    public string? InvoiceNo { get; set; }
    public IList<TextValue>? InvoiceNoList { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? GateNo { get; set; }
    public IList<TextValue>? GateNoList { get; set; }
    public DateTime? GateDate { get; set; }
    public string? DocumentName { get; set; }
    public IList<TextValue>? DocumentNameList { get; set; }
    public string? ItemName { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }
    public string? PartCode { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? DashboardType { get; set; }
    public string? CheckQc { get; set; }
    public string? QCCompleted { get; set; }
    public int? TotalMRNItemCount { get; set; }
    public int? QCtotalQty { get; set; }
    public int? ItemQCCompledCount { get; set; }
    public int? AccountCode { get; set; }
    public IList<HRAListDataModel> HRAListData { get; set; }
}