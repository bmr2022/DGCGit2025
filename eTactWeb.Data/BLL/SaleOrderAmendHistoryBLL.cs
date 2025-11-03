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

    public class SaleOrderAmendHistoryBLL : ISaleOrderAmendHistory
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly SaleOrderAmendHistoryDAL _SaleOrderAmendHistoryDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SaleOrderAmendHistoryBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _SaleOrderAmendHistoryDAL = new SaleOrderAmendHistoryDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<SaleOrderAmendHistoryModel> SaleOrderAmendHistory(
         string reportType,
         string flag,
         string dashboardFlag,
         string fromDate,
         string toDate,
         int accountCode,
         string poNo,
         string partCode,
         int itemCode,
         string soNo)
        {
            return await _SaleOrderAmendHistoryDAL.SaleOrderAmendHistory(
                reportType, flag, dashboardFlag, fromDate, toDate,
                accountCode, poNo, partCode, itemCode, soNo);
        }

        //public async Task<SaleOrderAmendHistoryModel> GetGateRegisterData(string ReportType, string FromDate, string ToDate, string gateno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        //{
        //    return await _SaleOrderAmendHistoryDAL.Sa(ReportType, FromDate, ToDate, gateno, docname, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName);
        //}
        public async Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate, int AccountCode, string SOno)
        {
            return await _SaleOrderAmendHistoryDAL.FillItemNamePartcode(FromDate, ToDate, AccountCode, SOno);
        }
        public async Task<ResponseResult> FillAccountCode(string FromDate, string ToDate, string partCode, string ItemCode, string SOno)
        {
            return await _SaleOrderAmendHistoryDAL.FillAccountCode(FromDate, ToDate, partCode, ItemCode,SOno);
        }
        public async Task<ResponseResult> FillSONO(string FromDate, string ToDate, int AccountCode)
        {
            return await _SaleOrderAmendHistoryDAL.FillSONO(FromDate, ToDate, AccountCode);
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate, int AccountCode)
        {
            return await _SaleOrderAmendHistoryDAL.FillPONO(FromDate, ToDate, AccountCode);
        }

    }
}
