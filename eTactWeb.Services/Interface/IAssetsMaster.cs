using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface  IAssetsMaster
    {

		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillCategoryName();
		Task<ResponseResult> FillCustoidianEmpName();
		Task<ResponseResult> FillCostCenterName();
		Task<ResponseResult> FillDepartmentName();
		Task<ResponseResult> FillParentAccountName();
		Task<ResponseResult> FillEntryId(int YearCode,string EntryDate);
		Task<ResponseResult> FillParentGoupDetail(int ParentAccountCode);
		Task<ResponseResult> SaveAssetsMaster(AssetsMasterModel model);
        Task<ResponseResult> GetDashboardData(AssetsMasterModel model);
        Task<AssetsMasterModel> GetDashboardDetailData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int EntryId, int YearCode);
        Task<AssetsMasterModel> GetViewByID(int ID, int YC);
    }
}
