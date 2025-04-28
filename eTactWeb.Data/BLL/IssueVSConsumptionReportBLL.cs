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

namespace eTactWeb.Data.BLL
{
    public class IssueVSConsumptionReportBLL:IIssueVSConsumptionReport
    {
        private IssueVSConsumptionReportDAL _IssueVSConsumptionReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public IssueVSConsumptionReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _IssueVSConsumptionReportDAL = new IssueVSConsumptionReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<IssueVSConsumptionReportModel> GetDetailData()
        {
            return await _IssueVSConsumptionReportDAL.GetDetailData();
        }
    }
}
