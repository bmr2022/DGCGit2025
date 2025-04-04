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
    public class InventoryAgingReportBLL:IInventoryAgingReport
    {
        private InventoryAgingReportDAL _InventoryAgingReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public InventoryAgingReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _InventoryAgingReportDAL = new InventoryAgingReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillRMItemName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            return await _InventoryAgingReportDAL.FillRMItemName( FromDate,  ToDate,  CurrentDate,  Storeid);
        }

        public async Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            return await _InventoryAgingReportDAL.FillRMPartCode( FromDate,  ToDate,  CurrentDate,  Storeid);
        }
        public async Task<ResponseResult> FillStoreName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            return await _InventoryAgingReportDAL.FillStoreName( FromDate,  ToDate,  CurrentDate,  Storeid);
        }
        public async Task<ResponseResult> FillWorkCenterName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            return await _InventoryAgingReportDAL.FillWorkCenterName( FromDate,  ToDate,  CurrentDate,  Storeid);
        }
        public async Task<InventoryAgingReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate, string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid)
        {
            return await _InventoryAgingReportDAL.GetInventoryAgingReportDetailsData(fromDate, toDate, CurrentDate, WorkCenterid, ReportType, RMItemCode, Storeid);
        }
    }
}
