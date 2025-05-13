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
    public class ProductionEntryReportBLL : IProductionEntryReport
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ProductionEntryReportDAL _ProductionEntryReportDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductionEntryReportBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _ProductionEntryReportDAL = new ProductionEntryReportDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<ResponseResult> FillFGPartCode(string FromDate,string ToDate)
        {
            return await _ProductionEntryReportDAL.FillFGPartCode(FromDate,ToDate);
        }
        public async Task<ResponseResult> FillFGItemName(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillFGItemName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillRMPartCode(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillRMItemName(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillRMItemName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillShiftName()
        {
            return await _ProductionEntryReportDAL.FillShiftName();
        }
        public async Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillProdSlipNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillProdPlanNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillProdSchNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillReqNo(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillReqNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillWorkCenter(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillWorkCenter(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillMachinName(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillMachinName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillOperatorName(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillOperatorName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProcess(string FromDate, string ToDate)
        {
            return await _ProductionEntryReportDAL.FillProcess(FromDate, ToDate);
        }
        public async Task<ProductionEntryReportModel> GetProductionEntryReport(string ReportType, string FromDate, string ToDate, string FGPartCode, string FGItemName, string RMPartCode, string RMItemName, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ReqNo, string WorkCenter, string MachineName, string OperatorName, string Process)
        {
            return await _ProductionEntryReportDAL.GetProductionEntryReport(ReportType , FromDate, ToDate, FGPartCode, FGItemName, RMPartCode, RMItemName, ProdSlipNo,ProdPlanNo , ProdSchNo, ReqNo,WorkCenter,MachineName,OperatorName,Process);
        }
    }
}
