using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICustomerDiscountCategory
    {
        Task<ResponseResult> SaveCustomerDiscountCategory(CustomerDiscountCategoryModel model);
        Task<ResponseResult> CheckCategoryExists(string categoryName);
        Task<IEnumerable<CustomerDiscountCategoryModel>> GetDashboard();
        Task<CustomerDiscountCategoryModel> GetById(long id);
        Task<ResponseResult> Delete(long id);
    }
}
