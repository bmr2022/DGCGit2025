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
    public class PrimaryAccountGroupMasterBLL:IPrimaryAccountGroupMaster
    {
        private PrimaryAccountGroupMasterDAL _PrimaryAccountGroupMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public PrimaryAccountGroupMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _PrimaryAccountGroupMasterDAL = new PrimaryAccountGroupMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _PrimaryAccountGroupMasterDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetParentGroup()
        {
            return await _PrimaryAccountGroupMasterDAL.GetParentGroup();
        }
        public async Task<ResponseResult> GetAccountGroupDetailsByParentCode(int parentAccountCode)
        {
            return await _PrimaryAccountGroupMasterDAL.GetAccountGroupDetailsByParentCode( parentAccountCode);
        }
        public async Task<ResponseResult> SavePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model)
        {
            return await _PrimaryAccountGroupMasterDAL.SavePrimaryAccountGroupMaster(model);
        } 
        public async Task<ResponseResult> UpdatePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model)
        {
            return await _PrimaryAccountGroupMasterDAL.UpdatePrimaryAccountGroupMaster(model);
        } 
      
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _PrimaryAccountGroupMasterDAL.GetDashboardData();
        }
        public async Task<PrimaryAccountGroupMasterDashBoardModel> GetDashboardDetailData(string Account_Name, string ParentAccountName)
        {
            return await _PrimaryAccountGroupMasterDAL.GetDashboardDetailData( Account_Name,  ParentAccountName);
        }
        public async Task<PrimaryAccountGroupMasterModel> GetViewByID(int accountCode)
        {
            return await _PrimaryAccountGroupMasterDAL.GetViewByID(accountCode);
        }
        public async Task<ResponseResult> DeleteByID(int AccountCode)
        {
            return await _PrimaryAccountGroupMasterDAL.DeleteByID(AccountCode);
        }
    }
}
