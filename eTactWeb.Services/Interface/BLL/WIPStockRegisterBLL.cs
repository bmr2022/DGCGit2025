using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class WIPStockRegisterBLL : IWIPStockRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly WIPStockRegisterDAL _WIPStockRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WIPStockRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _WIPStockRegisterDAL = new WIPStockRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }

        public async Task<ResponseResult> GetAllWorkCenter()
        {
            return await _WIPStockRegisterDAL.GetAllWorkCenter();
        }
        public async Task<WIPStockRegisterModel> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int WCID, string ReportType, string BatchNo, string UniqueBatchNo, string WorkCenter)
        {
            return await _WIPStockRegisterDAL.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, WCID, ReportType, BatchNo, UniqueBatchNo, WorkCenter);
        }
        public async Task<ResponseResult> GetAllItemGroups()
        {
            return await _WIPStockRegisterDAL.GetAllItemGroups();
        }
        public async Task<ResponseResult> GetAllItemTypes()
        {
            return await _WIPStockRegisterDAL.GetAllItemTypes();
        }

    }
}
