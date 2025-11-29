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
    public class TracebilityReportBLL : ITracebilityReport
    {
        private TracebilityReportDAL _MISTracebilityReportDAL;
        private readonly IDataLogic _DataLogicDAL;
        public TracebilityReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _MISTracebilityReportDAL = new TracebilityReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<TracebilityReportModel> GetTracebilityReportData( string FromDate, string ToDate, string SaleBillNo)
        {
            return await _MISTracebilityReportDAL.GetTracebilityReportData( FromDate, ToDate, SaleBillNo);
        }

        public async Task<ResponseResult> FillSaleBillNoList(string FromDate, string ToDate)
        {
            return await _MISTracebilityReportDAL.FillSaleBillNoList(FromDate, ToDate);
        }
    }
}
