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
    public class StockRegisterBLL : IStockRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly StockRegisterDAL _StockRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StockRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _StockRegisterDAL = new StockRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }
        public async Task<StockRegisterModel> GetStockRegisterData(string FromDate, string ToDate, string PartCode,string ItemName, string ItemGroup, string ItemType,int StoreId, string ReportType,string BatchNo,string UniqueBatchNo)
        {
            return await _StockRegisterDAL.GetStockRegisterData(FromDate, ToDate,PartCode,ItemName,ItemGroup,ItemType,StoreId,ReportType,BatchNo,UniqueBatchNo);
        }
        public async Task<ResponseResult> GetAllItems()
        {
            return await _StockRegisterDAL.GetAllItems();
        }
        public async Task<ResponseResult> GetAllItemTypes()
        {
            return await _StockRegisterDAL.GetAllItemTypes();
        }
        public async Task<ResponseResult> GetAllItemGroups()
        {
            return await _StockRegisterDAL.GetAllItemGroups();
        }
        public async Task<ResponseResult> GetAllStores()
        {
            return await _StockRegisterDAL.GetAllStores();
        }
      
    }
}
