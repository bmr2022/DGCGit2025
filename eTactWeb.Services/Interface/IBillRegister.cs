using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBillRegister
    {
        Task<ResponseResult> FillVendoreList(string FromDate, string ToDate);
        Task<ResponseResult> FillDocList(string FromDate, string ToDate);
        Task<ResponseResult> FillPONOList(string FromDate, string ToDate);
        Task<ResponseResult> FillSchNOList(string FromDate, string ToDate);
        Task<ResponseResult> FillHsnNOList(string FromDate, string ToDate);
        Task<ResponseResult> FillGstNOList(string FromDate, string ToDate);
        Task<ResponseResult> FillInvoiceNOList(string FromDate, string ToDate);
        Task<ResponseResult> FillItemName(string FromDate, string ToDate);
        Task<ResponseResult> FillPartCode(string FromDate, string ToDate);
        Task<BillRegisterModel> GetRegisterData(string ReportType, string FromDate, string ToDate, string DocumentName, string PONO, string SchNo,string HsnNo,string GstNo,string InvoiceNo,string PurchaseBill,string PurchaseRejection,string DebitNote,string CreditNote,string SaleRejection, string PartCode, string ItemName, string VendorName, int ForFinYear);
    }
}
