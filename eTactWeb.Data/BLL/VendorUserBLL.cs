using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class VendorUserBLL : IVendorMater
    {
        private VendorUserDAL _vendorUserDAL;
        private readonly IDataLogic _DataLogicDAL;

        public VendorUserBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _vendorUserDAL = new VendorUserDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            return await _vendorUserDAL.FillEntryId();
        }
        public async Task<ResponseResult> FillVendorList(string isShowAll)
        {
            return await _vendorUserDAL.FillVendorList(isShowAll);
        }
        public async Task<ResponseResult> CheckUserDuplication(int userId)
        {
            return await _vendorUserDAL.CheckUserDuplication(userId);
        }
        public async Task<ResponseResult> SaveVendorUser(VendorUserModel model)
        {
            return await _vendorUserDAL.SaveVendorUser(model);
        }
        public async Task<VendorUserModel> GetViewByID(int ID,string mode)
        {
            return await _vendorUserDAL.GetViewByID(ID,mode);
        }
        public async Task<ResponseResult> DeleteByID(int userEntryId, int accountCode,int userId, string entryByMachineName, int actualEntryBy, string actualEntryDate)
        {
            return await _vendorUserDAL.DeleteByID(userEntryId,accountCode,userId,entryByMachineName,actualEntryBy,actualEntryDate);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _vendorUserDAL.GetDashboardData();
        }
    }
}
