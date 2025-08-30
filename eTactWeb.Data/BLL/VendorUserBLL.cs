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
        public async Task<ResponseResult> SaveVendorUser(VendorUserModel model)
        {
            return await _vendorUserDAL.SaveVendorUser(model);
        }
    }
}
