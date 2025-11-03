using eTactWeb.DOM.Models;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPPCToolIssue
    {
        Task<ResponseResult> GetNewEntry(string Flag, int YearCode);
        Task<ResponseResult> FillToolList(string Flag);
        Task<ResponseResult> FillToolBarCode(string Flag, long ToolIssueEntryId);
        Task<ResponseResult> FillProdPlan(string Flag, long ToolIssueEntryId);
        Task<ResponseResult> FillProdPlanYearCode(string Flag, long ToolIssueEntryId, string ProdPlanNo);
        Task<ResponseResult> FillProdPlanSchedule(string Flag, long ToolIssueEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId);
        Task<ResponseResult> FillMachineList(string Flag);
        Task<ResponseResult> InsertToolIssue(PPCToolIssueMainModel model, string Flag);
    }
}
