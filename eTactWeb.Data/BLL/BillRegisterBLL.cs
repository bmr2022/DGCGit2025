using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class BillRegisterBLL:IBillRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly BillRegisterDAL _BillRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _BillRegisterDAL = new BillRegisterDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<ResponseResult> FillVendoreList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillVendoreList(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillDocList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillDocList(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillPONOList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillPONOList(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillSchNOList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillSchNOList(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillHsnNOList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillHsnNOList(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillGstNOList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillGstNOList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillInvoiceNOList(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillInvoiceNOList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemName(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillItemName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillPartCode(string FromDate, string ToDate)
        {
            return await _BillRegisterDAL.FillPartCode(FromDate, ToDate);
        }
        

        public async Task<BillRegisterModel> GetRegisterData(string ReportType, string FromDate, string ToDate, string DocumentName, string PONO, string SchNo, string HsnNo, string GstNo, string InvoiceNo, string PurchaseBill, string PurchaseRejection, string DebitNote, string CreditNote, string SaleRejection, string PartCode, string ItemName, string VendorName, int ForFinYear)
        {
            return await _BillRegisterDAL.GetRegisterData( ReportType,  FromDate,  ToDate,  DocumentName,  PONO,  SchNo,  HsnNo,  GstNo,  InvoiceNo,  PurchaseBill,  PurchaseRejection,  DebitNote,  CreditNote,  SaleRejection,  PartCode,  ItemName,  VendorName,  ForFinYear);
        }

    }
}
