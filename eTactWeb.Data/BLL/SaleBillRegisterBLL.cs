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
    public class SaleBillRegisterBLL : ISaleBillRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly SaleBillRegisterDAL _SaleBillRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommon _common;

        public SaleBillRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService, ICommon common)
        {
            _DataLogicDAL = iDataLogic;
            _SaleBillRegisterDAL = new SaleBillRegisterDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService, common);
            _common = common;
        }
        public async Task<ResponseResult> FillItemNamePartcodeList(string FromDate, string ToDate, string PartCode, string ItemName)
        {
            return await _SaleBillRegisterDAL.FillItemNamePartcodeList(FromDate, ToDate, PartCode, ItemName);
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate, string SearchText)//2
        {
            return await _SaleBillRegisterDAL.FillSchNo(FromDate, ToDate, SearchText);
        }
        //3
        public async Task<ResponseResult> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, int itemCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO, int AccountCode, int yearcode,int ItemParentGroup)
        {
            return await _SaleBillRegisterDAL.GetSaleBillRegisterData(ReportType, FromDate, ToDate, docname, SONo, Schno, itemCode, ItemName, SaleBillNo, CustomerName, HSNNO, GSTNO, AccountCode, yearcode, ItemParentGroup);
        }
        public async Task<ResponseResult> FillCustomerList(string FromDate, string ToDate, string SearchText)//4
        {
            return await _SaleBillRegisterDAL.FillCustomerList(FromDate, ToDate, SearchText);
        }
        public async Task<ResponseResult> FillItemGroupList(string FromDate, string ToDate, string SearchText)//4
        {
            return await _SaleBillRegisterDAL.FillItemGroupList(FromDate, ToDate, SearchText);
        }
        public async Task<ResponseResult> FillDocumentList(string FromDate, string ToDate)//5
        {
            return await _SaleBillRegisterDAL.FillDocumentList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillGSTNo(string FromDate, string ToDate, string SearchText)//6
        {
            return await _SaleBillRegisterDAL.FillGSTNo(FromDate, ToDate, SearchText);
        }
        public async Task<ResponseResult> FillHSNNo(string FromDate, string ToDate, string SearchText)//7
        {
            return await _SaleBillRegisterDAL.FillHSNNo(FromDate, ToDate, SearchText);
        }
        public async Task<ResponseResult> FillSaleBillList(string FromDate, string ToDate, string SearchText)//8
        {
            return await _SaleBillRegisterDAL.FillSaleBillList(FromDate, ToDate, SearchText);
        }
        public async Task<ResponseResult> FillSONO(string FromDate, string ToDate, string SearchText)//9
        {
            return await _SaleBillRegisterDAL.FillSONO(FromDate, ToDate, SearchText);
        }
    }
}
