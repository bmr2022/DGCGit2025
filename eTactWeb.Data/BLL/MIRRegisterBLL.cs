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
    public class MIRRegisterBLL : IMIRRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly MIRRegisterDAL _MIRRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MIRRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _MIRRegisterDAL = new MIRRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }
        public async Task <ResponseResult> FillMIRNo(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillMIRNo(FromDate, ToDate);
        }
        //public async Task<ResponseResult> NewEntryId(int YearCode)
        //{
        //    return await _WorkOrderDAL.NewEntryId(YearCode);
        //}
        public async Task<ResponseResult> FillGateNo(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillGateNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillMRNNo(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillMRNNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillInvoice(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillInvoice(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemName(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillItemName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemPartcode(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillItemPartcode(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillPONO(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillSchNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillVendor(string FromDate, string ToDate)
        {
            return await _MIRRegisterDAL.FillVendor(FromDate, ToDate);
        }
         
        public async Task<MIRRegisterModel> GetRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno, string MRNno, string MIRNo, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            return await _MIRRegisterDAL.GetRegisterData(MRNType,ReportType, FromDate, ToDate, gateno, MRNno, MIRNo, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName);
        }
    }
}
