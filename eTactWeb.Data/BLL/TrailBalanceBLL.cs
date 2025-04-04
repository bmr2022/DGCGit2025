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
    public class TrailBalanceBLL:ITrailBalance
    {
        private TrailBalanceDAL _TrailBalanceDAL;
        private readonly IDataLogic _DataLogicDAL;

        public TrailBalanceBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _TrailBalanceDAL = new TrailBalanceDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<TrailBalanceModel> GetTrailBalanceDetailsData(string FromDate, string ToDate, string EntryByMachine,string ReportType)
        {
            return await _TrailBalanceDAL.GetTrailBalanceDetailsData(FromDate, ToDate, EntryByMachine, ReportType);
        }
    }
}
