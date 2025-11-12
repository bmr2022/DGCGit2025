using eTactWeb.DOM.Models;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPPCToolIssue
    {
        Task<ResponseResult> GetNewEntry(string Flag, int YearCode);
        Task<ResponseResult> FillDepartmentList(string Flag);
        Task<ResponseResult> FillToolList(string Flag);
        Task<ResponseResult> FillToolBarCode(string Flag, long ToolEntryId);
        Task<ResponseResult> FillToolSerialNo(string Flag, long ToolEntryId,string Barcode);
        Task<ResponseResult> FillToolBarCodeDetail(string Flag, long ToolEntryId, string Barcode, string SerialNo);
        Task<ResponseResult> FillProdPlan(string Flag, long ToolEntryId);
        Task<ResponseResult> FillProdPlanYearCode(string Flag, long ToolEntryId, string ProdPlanNo);
        Task<ResponseResult> FillProdPlanDate(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode);
        Task<ResponseResult> FillProdPlanSchedule(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId);
        Task<ResponseResult> FillProdPlanScheduleYearCode(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo);
        Task<ResponseResult> FillProdPlanScheduleDetail(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo, long ProdSchYearCode);
        Task<ResponseResult> FillMachineList(string Flag);
        Task<ResponseResult> FillIssuedByEmpList(string Flag);
        Task<ResponseResult> InsertToolIssue(PPCToolIssueMainModel model, DataTable ToolGrid);
    }
}
