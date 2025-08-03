using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
		public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			return await _DiscountCustomerCategoryMasterDAL.FillEntryID(YearCode);
		}
	}
}
