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
    public class MISTracebilityReportBLL : IMISTracebilityReport
    {
        private MISTracebilityReportDAL _MISTracebilityReportDAL;
        private readonly IDataLogic _DataLogicDAL;
        public MISTracebilityReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _MISTracebilityReportDAL = new MISTracebilityReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<MISTracebilityReportModel> GetMISTracebilityReportData( string FromDate, string ToDate, string SaleBillNo)
        {
            return await _MISTracebilityReportDAL.GetMISTracebilityReportData( FromDate, ToDate, SaleBillNo);
        }

        public async Task<ResponseResult> FillSaleBillNoList(string FromDate, string ToDate)
        {
            return await _MISTracebilityReportDAL.FillSaleBillNoList(FromDate, ToDate);
        }
    }
}
