using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface  IDiscountCustomerCategoryMaster
    {
		Task<ResponseResult> FillEntryID(int YearCode);
		Task<ResponseResult> FillDiscountCategory();
		Task<ResponseResult> GetFormRights(int userID);
		Task<ResponseResult> SaveDiscountCustomerCategoryMaster(DiscountCustomerCategoryMasterModel model);
        Task<ResponseResult> GetDashboardData(DiscountCustomerCategoryMasterModel model);
        Task<DiscountCustomerCategoryMasterModel> GetDashboardDetailData(string FromDate, string ToDate);

    }
}
