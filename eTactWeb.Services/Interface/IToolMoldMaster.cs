using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IToolMoldMaster
    {
		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillCategoryName();
		Task<ResponseResult> FillCustoidianEmpName();
		Task<ResponseResult> FillCostCenterName();
		Task<ResponseResult> FillDepartmentName();
		Task<ResponseResult> FillParentAccountName();
		Task<ResponseResult> FillEntryId(int YearCode, string EntryDate);
		Task<ResponseResult> FillParentGoupDetail(int ParentAccountCode);
		Task<ResponseResult> SaveToolMoldMaster(ToolMoldMasterModel model);
        Task<ResponseResult> GetDashboardData(ToolMoldMasterModel model);
        Task<ToolMoldMasterModel> GetDashboardDetailData(string FromDate, string ToDate, string ToolName);
    }
}
