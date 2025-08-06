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
    public class OutStandingBLL:IOutStanding
    {
        private OutStandingDAL _OutStandingDAL;
        private readonly IDataLogic _DataLogicDAL;

        public OutStandingBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _OutStandingDAL = new OutStandingDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetPartyName(string outstandingType, string TillDate)
        {
            return await _OutStandingDAL.GetPartyName( outstandingType,TillDate);
        }
        public async Task<ResponseResult> GetGroupName(string outstandingType, string TillDate)
        {
            return await _OutStandingDAL.GetGroupName(outstandingType, TillDate);
        }

        public async Task<OutStandingModel> GetDetailsData(string outstandingType, string TillDate,string GroupName,string[] AccountNameList,int AccountCode,string ShowOnlyApprovedBill)
        {
            return await _OutStandingDAL.GetDetailsData(outstandingType, TillDate, GroupName, AccountNameList,AccountCode, ShowOnlyApprovedBill);
        }
    }
}
