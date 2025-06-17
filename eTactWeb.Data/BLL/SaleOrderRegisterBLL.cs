using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class SaleOrderRegisterBLL: ISaleOrderRegister
    {
        private SaleOrderRegisterDAL _SaleOrderRegisterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public SaleOrderRegisterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _SaleOrderRegisterDAL = new SaleOrderRegisterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillPartCode(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillPartCode(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillSaleOrderNo(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillSaleOrderNo(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillCustOrderNo(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillCustOrderNo(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillSchNo(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillCustomerName(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillCustomerName(FromDate, ToDate);
        }
         public async Task<ResponseResult> FillSalesPerson(string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.FillSalesPerson(FromDate, ToDate);
        }
        public async Task<SaleOrderRegisterModel> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate)
        {
            return await _SaleOrderRegisterDAL.GetSaleOrderDetailsData(OrderSchedule, ReportType, PartCode, ItemName, Sono, CustOrderNo, CustomerName, SalesPersonName, SchNo, FromDate, ToDate);
        }
    }
}
