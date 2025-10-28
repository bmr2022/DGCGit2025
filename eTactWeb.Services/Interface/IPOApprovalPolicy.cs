using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPOApprovalPolicy
    {
		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillPartCode();
		Task<ResponseResult> FillEmpName();
		Task<ResponseResult> FillCatName();
		Task<ResponseResult> FillGroupName();
		Task<ResponseResult> FillEntryID();
		Task<ResponseResult> SavePOApprovalPolicy(POApprovalPolicyModel model);

        Task<ResponseResult> GetDashboardData(POApprovalPolicyModel model);
        Task<POApprovalPolicyModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType);
        Task<ResponseResult> DeleteByID(int EntryId, string EntryDate, int EntryByempId);
        Task<POApprovalPolicyModel> GetViewByID(int ID, int YC, string FromDate, string TODate);
    }
}
