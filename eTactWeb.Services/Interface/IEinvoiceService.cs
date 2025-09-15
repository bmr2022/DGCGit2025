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
        Task<ResponseResult> GetEwayBillDataAsync();
        Task<ResponseResult> GenerateInvoiceAsync(EInvoiceItemModel input);
        Task<ResponseResult> GenerateEwayBillAsync(string token, string irnNo, int invoiceNo, int yearCode);
        Task<ResponseResult> GenerateQRCodeAsync(string barcodeValue, int invoiceNo, int yearCode);
        Task<ResponseResult> CreateIRNAsync(string token, int manEntryId, string manInvoiceNo, int manYearCode, string saleBillType, string customerPartCode, string transporterName, string vehicleNo, string distanceKM, int EntrybyId, string MachineName, string fromname, string generateEway,string flag);

        Task<ResponseResult> CheckDuplicateIRN(int entryId, string invoiceNo, int yearCode);
        Task<ResponseResult> CancelEInvoice(string token,int SaleBillYearCode, string SaleBillNo);

        // Removed duplicate CreateIRNAsync method to resolve CS0111 error
    }

}
