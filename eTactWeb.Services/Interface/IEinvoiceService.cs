using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IEinvoiceService
    {
       
        Task<string> GetAccessTokenAsync();
        Task<ResponseResult> GenerateInvoiceAsync(EInvoiceItemModel input);
        Task<ResponseResult> GenerateEwayBillAsync(string token, string irnNo, int invoiceNo, int yearCode);
        Task<ResponseResult> GenerateQRCodeAsync(string barcodeValue, int invoiceNo, int yearCode);
        Task<ResponseResult> CreateIRNAsync(string token, int manEntryId, string manInvoiceNo, int manYearCode, string saleBillType, string customerPartCode);

        Task<ResponseResult> CheckDuplicateIRN(int entryId, string invoiceNo, int yearCode);
    }

}
