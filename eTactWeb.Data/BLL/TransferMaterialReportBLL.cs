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
    public class TransferMaterialReportBLL : ITransferMaterialReport
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly TransferMaterialReportDAL _TransferMaterialReportDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransferMaterialReportBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _TransferMaterialReportDAL = new TransferMaterialReportDAL(configuration, iDataLogic, _httpContextAccessor,connectionStringService);
        }
        public async Task<ResponseResult> FillFromWorkCenter(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillFromWorkCenter(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillToWorkCenter(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillToWorkCenter(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillToStore(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillToStore(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillPartCode(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillPartCode(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItemName(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillItemName(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillTransferSlipNo(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillTransferSlipNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillProdSlipNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillProdPlanNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillProdSchNo(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillProcessName(string FromDate, string ToDate)
        {
            return await _TransferMaterialReportDAL.FillProcessName(FromDate, ToDate);
        }
        public async Task<TransferMaterialReportModel> GetTransferMaterialReport(string ReportType, string FromDate, string ToDate, string FromWorkCenter, string ToWorkCenter, string Tostore, string PartCode, string ItemName, string TransferSlipNo, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ProcessName)
        {
            return await _TransferMaterialReportDAL.GetTransferMaterialReport(ReportType, FromDate, ToDate, FromWorkCenter, ToWorkCenter, Tostore, PartCode, ItemName, TransferSlipNo, ProdSlipNo, ProdPlanNo, ProdSchNo, ProcessName);
        }
    }
}
