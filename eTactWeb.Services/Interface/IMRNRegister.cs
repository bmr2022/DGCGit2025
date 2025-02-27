using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMRNRegister
    { 
        Task<MRNRegisterModel> GetMRNRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno, string MRNno,string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName);
 

        Task<ResponseResult> FillGateNo(string FromDate, string ToDate);
        Task<ResponseResult> FillMRNNo(string FromDate, string ToDate);

        Task<ResponseResult> FillDocument(string FromDate, string ToDate);
        Task<ResponseResult> FillVendor(string FromDate, string ToDate);
        Task<ResponseResult> FillInvoice(string FromDate, string ToDate);
        Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate);
        Task<ResponseResult> FillPONO(string FromDate, string ToDate);
        Task<ResponseResult> FillSchNo(string FromDate, string ToDate);


    }
}
