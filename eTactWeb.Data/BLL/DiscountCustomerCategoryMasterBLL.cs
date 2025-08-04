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
    public class DiscountCustomerCategoryMasterBLL:IDiscountCustomerCategoryMaster
    {
        private DiscountCustomerCategoryMasterDAL _DiscountCustomerCategoryMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public DiscountCustomerCategoryMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _DiscountCustomerCategoryMasterDAL = new DiscountCustomerCategoryMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
		public async Task<ResponseResult> GetFormRights(int userID)
		{
			return await _DiscountCustomerCategoryMasterDAL.GetFormRights(userID);
		}
		public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			return await _DiscountCustomerCategoryMasterDAL.FillEntryID(YearCode);
		}
		public async Task<ResponseResult> FillDiscountCategory()
		{
			return await _DiscountCustomerCategoryMasterDAL.FillDiscountCategory();
		}
		public async Task<ResponseResult> SaveDiscountCustomerCategoryMaster(DiscountCustomerCategoryMasterModel model)
		{
			return await _DiscountCustomerCategoryMasterDAL.SaveDiscountCustomerCategoryMaster(model);
		}

        public async Task<ResponseResult> GetDashboardData(DiscountCustomerCategoryMasterModel model)
        {
            return await _DiscountCustomerCategoryMasterDAL.GetDashboardData(model);
        }
        public async Task<DiscountCustomerCategoryMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
        {
            return await _DiscountCustomerCategoryMasterDAL.GetDashboardDetailData(FromDate, ToDate);
        }
    }
}
