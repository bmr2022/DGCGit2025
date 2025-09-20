using eTactWeb.Data.Common;
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
    public class GateEntryRegisterBLL : IGateEntryRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly GateEntryRegisterDAL _GateEntryRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GateEntryRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _GateEntryRegisterDAL = new GateEntryRegisterDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task <ResponseResult> FillDocument(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillDocument(FromDate, ToDate);
        }
        //public async Task<ResponseResult> NewEntryId(int YearCode)
        //{
        //    return await _WorkOrderDAL.NewEntryId(YearCode);
        //}
        public async Task<ResponseResult> FillGateNo(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillGateNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillInvoice(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillInvoice(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillItemNamePartcode(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillPONO(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillSchNo(FromDate, ToDate);
        }
          public async Task<ResponseResult> GetFeatureOption()
        {
            return await _GateEntryRegisterDAL.GetFeatureOption();
        } 
        
        public async Task<ResponseResult> FillVendor(string FromDate, string ToDate)
        {
            return await _GateEntryRegisterDAL.FillVendor(FromDate, ToDate);
        }
        public async Task<GateEntryRegisterModel> GetGateRegisterData(string ReportType, string FromDate, string ToDate, string gateno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName, int RecUnit)
         {
            return await _GateEntryRegisterDAL.GetGateRegisterData(ReportType, FromDate, ToDate, gateno, docname, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName   ,RecUnit);
        }
        //public async Task<ResponseResult> GetAllItems()
        //{
        //    return await _GateEntryRegisterDAL.GetAllItems();
        //}


    }
}
