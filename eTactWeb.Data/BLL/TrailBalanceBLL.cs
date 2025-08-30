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
    public class TrailBalanceBLL:ITrailBalance
    {
        private TrailBalanceDAL _TrailBalanceDAL;
        private readonly IDataLogic _DataLogicDAL;

        public TrailBalanceBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _TrailBalanceDAL = new TrailBalanceDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<TrailBalanceModel> GetTrailBalanceDetailsData(string FromDate, string ToDate, int? TrailBalanceGroupCode, int? ParentAccountCode, string ReportType)
        {
            return await _TrailBalanceDAL.GetTrailBalanceDetailsData(FromDate, ToDate, TrailBalanceGroupCode,ParentAccountCode, ReportType);
        }
        public async Task<ResponseResult> FillGroupList(string FromDate, string ToDate)
        {
            return await _TrailBalanceDAL.FillGroupList(FromDate, ToDate);
        }
        public async Task<ResponseResult> FillParentGroupList(string FromDate, string ToDate, int? GroupCode)
        {
            return await _TrailBalanceDAL.FillParentGroupList(FromDate,ToDate,GroupCode);
        }
        public async Task<ResponseResult> FillAccountList(string FromDate, string ToDate, int? GroupCode, int? ParentAccountCode)
        {
            return await _TrailBalanceDAL.FillAccountList(FromDate, ToDate,GroupCode,ParentAccountCode);
        }
    }
}
