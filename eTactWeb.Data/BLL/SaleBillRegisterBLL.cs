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

        public SaleBillRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _SaleBillRegisterDAL = new SaleBillRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }
         public async Task<ResponseResult> FillItemNamePartcodeList(string FromDate, string ToDate)
        {
            return await _SaleBillRegisterDAL.FillItemNamePartcodeList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)//2
        {
            return await _SaleBillRegisterDAL.FillSchNo(FromDate, ToDate);
        }
        //3
        public async Task<SaleBillRegisterModel> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, string PartCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO)
        {
            return await _SaleBillRegisterDAL.GetSaleBillRegisterData(ReportType, FromDate, ToDate, docname, SONo, Schno, PartCode,ItemName, SaleBillNo, CustomerName, HSNNO,   GSTNO);
        }
        public async Task<ResponseResult> FillCustomerList(string FromDate, string ToDate)//4
        {
            return await _SaleBillRegisterDAL.FillCustomerList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillDocumentList(string FromDate, string ToDate)//5
        {
            return await _SaleBillRegisterDAL.FillDocumentList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillGSTNo(string FromDate, string ToDate)//6
        {
            return await _SaleBillRegisterDAL.FillGSTNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillHSNNo(string FromDate, string ToDate)//7
        {
            return await _SaleBillRegisterDAL.FillHSNNo(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillSaleBillList(string FromDate, string ToDate)//8
        {
            return await _SaleBillRegisterDAL.FillSaleBillList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillSONO(string FromDate, string ToDate)//9
        {
            return await _SaleBillRegisterDAL.FillSONO(FromDate, ToDate);
        }
    }
}
