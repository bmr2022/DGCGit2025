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
    public class ConsumptionReportBLL:IConsumptionReport
    {
        private ConsumptionReportDAL _ConsumptionReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ConsumptionReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ConsumptionReportDAL = new ConsumptionReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillFGItemName()
        {
            return await _ConsumptionReportDAL.FillFGItemName();
        }
         public async Task<ResponseResult> FillFGPartCode()
        {
            return await _ConsumptionReportDAL.FillFGPartCode();
        }
         public async Task<ResponseResult> FillRMItemName()
        {
            return await _ConsumptionReportDAL.FillRMItemName();
        }

        public async Task<ResponseResult> FillRMPartCode()
        {
            return await _ConsumptionReportDAL.FillRMPartCode();
        }
        public async Task<ResponseResult> FillStoreName()
        {
            return await _ConsumptionReportDAL.FillStoreName();
        }
        public async Task<ResponseResult> FillWorkCenterName()
        {
            return await _ConsumptionReportDAL.FillWorkCenterName();
        }
        public async Task<ConsumptionReportModel> GetConsumptionDetailsData(string fromDate, string toDate, int WorkCenterid, string ReportType, int FGItemCode, int RMItemCode, int Storeid, string GroupName, string ItemCateg)
        {
            return await _ConsumptionReportDAL.GetConsumptionDetailsData(fromDate, toDate, WorkCenterid, ReportType, FGItemCode, RMItemCode, Storeid,  GroupName,  ItemCateg);
        }

        public async Task<DataSet> GetCategory()
        {
            return await _ConsumptionReportDAL.GetCategory();
        }
        public async Task<DataSet> GetGroupName()
        {
            return await _ConsumptionReportDAL.GetGroupName();
        }

    }
}
