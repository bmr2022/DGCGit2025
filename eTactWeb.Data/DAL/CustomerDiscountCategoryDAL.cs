using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CustomerDiscountCategoryDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public CustomerDiscountCategoryDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }


        public async Task<ResponseResult> SaveCustomerDiscountCategory(CustomerDiscountCategoryModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", model.Mode == "U" ? "Update" : "Insert"),
            new SqlParameter("@CategoryId", model.CategoryId > 0 ? model.CategoryId : (object)DBNull.Value),
            new SqlParameter("@DiscountCategoryName", string.IsNullOrEmpty(model.DiscountCategoryName) ? DBNull.Value : model.DiscountCategoryName)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("[SPCustomerDiscountCategory]", SqlParams);
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> CheckCategoryExists(string categoryName)
        {
            var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "CheckPartyExists"),
            new SqlParameter("@DiscountCategoryName", categoryName)
        };

            return await _IDataLogic.ExecuteDataTable("[SPCustomerDiscountCategory]", sqlParams);
        }

        public async Task<IEnumerable<CustomerDiscountCategoryModel>> GetDashboard()
        {
            var sqlParams = new List<dynamic> { new SqlParameter("@Flag", "Dashboard") };
            var response = await _IDataLogic.ExecuteDataTable("[SPCustomerDiscountCategory]", sqlParams);

            var result = new List<CustomerDiscountCategoryModel>();
            if (response.Result is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new CustomerDiscountCategoryModel
                    {
                        CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : 0,
                        DiscountCategoryName = row["DiscountCategoryName"] != DBNull.Value ? row["DiscountCategoryName"].ToString() : string.Empty
                    });
                }
            }
            return result;
        }


        public async Task<CustomerDiscountCategoryModel> GetById(long id)
        {
            var sqlParams = new List<dynamic>
    {
        new SqlParameter("@Flag", "ViewByID"),
        new SqlParameter("@CategoryId", id)
    };

            var response = await _IDataLogic.ExecuteDataTable("[SPCustomerDiscountCategory]", sqlParams);

            if (response.Result is DataTable dt && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new CustomerDiscountCategoryModel
                {
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : 0,
                    DiscountCategoryName = row["DiscountCategoryName"] != DBNull.Value ? row["DiscountCategoryName"].ToString() : string.Empty
                };
            }

            return null;
        }



        public async Task<ResponseResult> Delete(long id)
        {
            var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DeleteByID"),
            new SqlParameter("@CategoryId", id)
        };
            return await _IDataLogic.ExecuteDataTable("[SPCustomerDiscountCategory]", sqlParams);
        }

    }

}
