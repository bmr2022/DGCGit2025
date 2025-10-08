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

        public async Task<ResponseResult> GetPartyName(string outstandingType, string TillDate, int? GroupCode)
        {
            return await _OutStandingDAL.GetPartyName( outstandingType,TillDate, GroupCode);
        }
        public async Task<ResponseResult> GetGroupName(string outstandingType, string TillDate)
        {
            return await _OutStandingDAL.GetGroupName(outstandingType, TillDate);
        }
        public async Task<ResponseResult> GetVoucherNo(int CurrentYear)
        {
            return await _OutStandingDAL.GetVoucherNo(CurrentYear);
        }
        public async Task<ResponseResult> GetVoucherType(int CurrentYear)
        {
            return await _OutStandingDAL.GetVoucherType(CurrentYear);
        }
        public async Task<OutStandingModel> GetVoucherList(int AccountCode, string VoucherNo, string VoucherType, int VoucherYearCode, int CurrentYear)
        {
            return await _OutStandingDAL.GetVoucherList(AccountCode,VoucherNo, VoucherType, VoucherYearCode, CurrentYear);
        }

        public async Task<OutStandingModel> GetDetailsData(string outstandingType, string TillDate,int? GroupName,string[] AccountNameList,int AccountCode,string ShowOnlyApprovedBill,bool ShowZeroBal, string VoucherNo, string VoucherType)
        {
            return await _OutStandingDAL.GetDetailsData(outstandingType, TillDate, GroupName, AccountNameList,AccountCode, ShowOnlyApprovedBill, ShowZeroBal, VoucherNo,VoucherType);
        }
    }
}
