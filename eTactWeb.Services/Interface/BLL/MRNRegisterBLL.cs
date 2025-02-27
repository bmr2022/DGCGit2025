using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class MRNRegisterBLL : IMRNRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly MRNRegisterDAL _MRNRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MRNRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _MRNRegisterDAL = new MRNRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }
        public async Task <ResponseResult> FillDocument(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillDocument(FromDate, ToDate);
        }
        //public async Task<ResponseResult> NewEntryId(int YearCode)
        //{
        //    return await _WorkOrderDAL.NewEntryId(YearCode);
        //}
        public async Task<ResponseResult> FillGateNo(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillGateNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillMRNNo(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillMRNNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillInvoice(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillInvoice(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillItemNamePartcode(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillPONO(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillSchNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillVendor(string FromDate, string ToDate)
        {
            return await _MRNRegisterDAL.FillVendor(FromDate, ToDate);
        }

        //public async Task<MRNRegisterModel> GetMRNRegisterData(string ReportType, string FromDate, string ToDate, string gateno, string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        //{
        //    return await _MRNRegisterDAL.GetMRNRegisterData(ReportType, FromDate, ToDate, gateno, MRNno, docname, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName);

        //    // throw new NotImplementedException();
        //}

        public async Task<MRNRegisterModel> GetMRNRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno, string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            return await _MRNRegisterDAL.GetMRNRegisterData(MRNType,ReportType, FromDate, ToDate, gateno, MRNno, docname, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName);
        }
    }
}
