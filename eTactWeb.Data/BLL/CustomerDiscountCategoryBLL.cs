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
    public class CustomerDiscountCategoryBLL : ICustomerDiscountCategory
    {
        private CustomerDiscountCategoryDAL _CustomerDiscountCategoryDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CustomerDiscountCategoryBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _CustomerDiscountCategoryDAL = new CustomerDiscountCategoryDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> SaveCustomerDiscountCategory(CustomerDiscountCategoryModel model)
        {
            return await _CustomerDiscountCategoryDAL.SaveCustomerDiscountCategory(model);
        }

        public async Task<ResponseResult> CheckCategoryExists(string categoryName)
        {
            return await _CustomerDiscountCategoryDAL.CheckCategoryExists(categoryName);
        }
        public async Task<IEnumerable<CustomerDiscountCategoryModel>> GetDashboard()
        {
            return await _CustomerDiscountCategoryDAL.GetDashboard();
        }

        public async Task<CustomerDiscountCategoryModel> GetById(long id)
        {
            return await _CustomerDiscountCategoryDAL.GetById(id);
        }

        public async Task<ResponseResult> Delete(long id)
        {
            return await _CustomerDiscountCategoryDAL.Delete(id);
        }
    }
}

