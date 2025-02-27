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
    public class BOMReportBLL:IBOMReport
    {
        private BOMReportDAL _BOMReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public BOMReportBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _BOMReportDAL = new BOMReportDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetBOMTree()
        {
            return await _BOMReportDAL.GetBOMTree();
        }
        public async Task<ResponseResult> GetDirectBOMTree()
        {
            return await _BOMReportDAL.GetDirectBOMTree();
        }
        public async Task<ResponseResult> GetBOMStockTree()
        {
            return await _BOMReportDAL.GetBOMStockTree();
        } 
        public async Task<ResponseResult> FillFinishItemName()
        {
            return await _BOMReportDAL.FillFinishItemName();
        }
        public async Task<ResponseResult> FillFinishPartCode()
        {
            return await _BOMReportDAL.FillFinishPartCode();
        }  
        public async Task<ResponseResult> FillRMItemName()
        {
            return await _BOMReportDAL.FillRMItemName();
        } 
        public async Task<ResponseResult> FillRMPartCode()
        {
            return await _BOMReportDAL.FillRMPartCode();
        } 
        public async Task<ResponseResult> FillStoreName()
        {   
            return await _BOMReportDAL.FillStoreName();
        } 
        public async Task<ResponseResult> FillWorkCenterName()
        {
            return await _BOMReportDAL.FillWorkCenterName();
        } 
        public async Task<BOMReportModel> GetBomTreeDetailsData(string fromDate, string toDate, int Yearcode, string ReportType, string FGPartCode, string RMPartCode, int Storeid)
        {
            return await _BOMReportDAL.GetBomTreeDetailsData( fromDate,toDate, Yearcode, ReportType, FGPartCode, RMPartCode, Storeid);
        }
    }
}
