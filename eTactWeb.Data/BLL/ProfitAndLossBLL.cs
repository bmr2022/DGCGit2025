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
    public class ProfitAndLossBLL : IProfitAndLoss
    {
        private ProfitAndLossDAL _ProfitAndLossDAL;
        private readonly IDataLogic _DataLogicDAL;
        public ProfitAndLossBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ProfitAndLossDAL = new ProfitAndLossDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ProfitAndLossModel> GetProfitAndLossData(string FromDate, string ToDate, string Flag, string ReportType, string ShowOpening,  string ShowRecordWithZeroAmt)
        {
            return await _ProfitAndLossDAL.GetProfitAndLossData(FromDate, ToDate, Flag, ReportType, ShowOpening, ShowRecordWithZeroAmt);
        }
    }
}
