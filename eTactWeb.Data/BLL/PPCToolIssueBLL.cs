using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PPCToolIssueBLL : IPPCToolIssue
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly PPCToolIssueDAL _PPCToolIssueDAL;

        public PPCToolIssueBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _PPCToolIssueDAL = new PPCToolIssueDAL(configuration, iDataLogic, connectionStringService);
        }

        // 1️⃣ NEW ENTRY
        public async Task<ResponseResult> GetNewEntry(string Flag, int YearCode)
        {
            return await _PPCToolIssueDAL.GetNewEntry(Flag, YearCode);
        }
        public async Task<ResponseResult> FillDepartmentList(string Flag)
        {
            return await _PPCToolIssueDAL.FillDepartmentList(Flag);
        }

        // 2️⃣ FILL TOOL LIST
        public async Task<ResponseResult> FillToolList(string Flag)
        {
            return await _PPCToolIssueDAL.FillToolList(Flag);
        }

        // 3️⃣ FILL TOOL BARCODE
        public async Task<ResponseResult> FillToolBarCode(string Flag, long ToolEntryId)
        {
            return await _PPCToolIssueDAL.FillToolBarCode(Flag, ToolEntryId);
        }

        public async Task<ResponseResult> FillToolSerialNo(string Flag, long ToolEntryId, string Barcode)
        {
            return await _PPCToolIssueDAL.FillToolSerialNo(Flag, ToolEntryId,Barcode);
        }
        public async Task<ResponseResult> FillToolBarCodeDetail(string Flag, long ToolEntryId, string Barcode, string SerialNo)
        {
            return await _PPCToolIssueDAL.FillToolBarCodeDetail(Flag, ToolEntryId, Barcode,SerialNo);
        }
        // 4️⃣ FILL PROD PLAN
        public async Task<ResponseResult> FillProdPlan(string Flag, long ToolEntryId)
        {
            return await _PPCToolIssueDAL.FillProdPlan(Flag, ToolEntryId);
        }

        // 5️⃣ FILL PROD PLAN YEAR CODE
        public async Task<ResponseResult> FillProdPlanYearCode(string Flag, long ToolEntryId, string ProdPlanNo)
        {
            return await _PPCToolIssueDAL.FillProdPlanYearCode(Flag, ToolEntryId, ProdPlanNo);
        }
        public async Task<ResponseResult> FillProdPlanDate(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode)
        {
            return await _PPCToolIssueDAL.FillProdPlanDate(Flag, ToolEntryId, ProdPlanNo,ProdPlanYearCode);
        }

        // 6️⃣ FILL PROD PLAN SCHEDULE
        public async Task<ResponseResult> FillProdPlanSchedule(string Flag, long ToolIssueEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId)
        {
            return await _PPCToolIssueDAL.FillProdPlanSchedule(Flag, ToolIssueEntryId, ProdPlanNo, ProdPlanYearCode, PlanNoEntryId);
        }

        // 7️⃣ FILL MACHINE LIST
        public async Task<ResponseResult> FillMachineList(string Flag)
        {
            return await _PPCToolIssueDAL.FillMachineList(Flag);
        }
        public async Task<ResponseResult> FillIssuedByEmpList(string Flag)
        {
            return await _PPCToolIssueDAL.FillIssuedByEmpList(Flag);
        }
        // 8️⃣ INSERT TOOL ISSUE
        public async Task<ResponseResult> InsertToolIssue(PPCToolIssueMainModel model, DataTable ToolGrid)
        {
            return await _PPCToolIssueDAL.InsertToolIssue(model, ToolGrid);
        }
    }
}
        