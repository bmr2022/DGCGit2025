using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
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
    public class SubVoucherBLL:ISubVoucher
    {
        private SubVoucherDAL _SubVoucherDAL;
        private readonly IDataLogic _DataLogicDAL;

        public SubVoucherBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _SubVoucherDAL = new SubVoucherDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetMainVoucherNames()
        {
            return await _SubVoucherDAL.GetMainVoucherNames();
        }

        public async Task<ResponseResult> GetEmployeeList()
        {
            return await _SubVoucherDAL.GetEmployeeList();
        }
        public async Task<ResponseResult> GetTableName(string MainVoucherName)
        {
            return await _SubVoucherDAL.GetTableName(MainVoucherName);
        }
        //public async Task<int> GetPrefixEntryIdByVoucherName(string mainVoucherName)
        //{
        //       return await _SubVoucherDAL.GetPrefixEntryIdByVoucherName(mainVoucherName);
        //}
        public async Task<ResponseResult> SaveSubVoucher(SubVoucherModel model)
        {
            return await _SubVoucherDAL.SaveSubVoucher(model);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _SubVoucherDAL.GetDashboardData();
        }
        public async Task<SubVoucherDashBoardGridModel> GetDashboardDetailData()
        {
            return await _SubVoucherDAL.GetDashboardDetailData();
        }
        public async Task<ResponseResult> UpdateSubVoucherPrefixSetting(SubVoucherDashBoardModel model)
        {
            return await _SubVoucherDAL.UpdateSubVoucherPrefixSetting(model);
        }
        public async Task<SubVoucherModel> GetViewByID(int PrefixEntryId,string MainVoucherName, string MainVoucherTableName)
        {
            return await _SubVoucherDAL.GetViewByID( PrefixEntryId, MainVoucherName, MainVoucherTableName);
        }
        public async Task<ResponseResult> DeleteByID(string MainVoucherName, string MainVoucherTableName, string StartSubVouchDiffSeries, int ActualEntryBy, int UpdatedBy, int PrefixEntryId)
        {
            return await _SubVoucherDAL.DeleteByID( MainVoucherName,  MainVoucherTableName,  StartSubVouchDiffSeries,  ActualEntryBy,  UpdatedBy,  PrefixEntryId);
        }
    }
}
