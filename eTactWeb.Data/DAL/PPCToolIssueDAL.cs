using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PPCToolIssueDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IConfiguration _configuration;
        private readonly ConnectionStringService _connectionStringService;
        private const string SP_NAME = "PPCSPToolIssueMainDetail";

        public PPCToolIssueDAL(IConfiguration configuration, IDataLogic dataLogic, ConnectionStringService connectionStringService)
        {
            _configuration = configuration;
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
        }

        // 1️⃣ NEW ENTRY
        public async Task<ResponseResult> GetNewEntry(string Flag, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolIssueYearCode", yearCode));
                SqlParams.Add(new SqlParameter("@ToolIssueDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> FillDepartmentList(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillToolList(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        // 3️⃣ FILL TOOL BARCODE
        public async Task<ResponseResult> FillToolBarCode(string Flag, long ToolEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillToolSerialNo(string Flag, long ToolEntryId, string Barcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@barcode", Barcode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillToolBarCodeDetail(string Flag, long ToolEntryId, string Barcode, string SerialNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@barcode", Barcode));
                SqlParams.Add(new SqlParameter("@SerialNo", SerialNo));


                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        // 4️⃣ FILL PROD PLAN
        public async Task<ResponseResult> FillProdPlan(string Flag, long ToolEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        // 5️⃣ FILL PROD PLAN YEAR CODE
        public async Task<ResponseResult> FillProdPlanYearCode(string Flag, long ToolEntryId, string ProdPlanNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillProdPlanDate(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                SqlParams.Add(new SqlParameter("@ProdPlanYearCode", ProdPlanYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        // 6️⃣ FILL PROD PLAN SCHEDULE
        public async Task<ResponseResult> FillProdPlanSchedule(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolIssueEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                SqlParams.Add(new SqlParameter("@ProdPlanYearCode", ProdPlanYearCode));
                SqlParams.Add(new SqlParameter("@PlanNoEntryId", PlanNoEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> FillProdPlanScheduleYearCode(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                SqlParams.Add(new SqlParameter("@ProdPlanYearCode", ProdPlanYearCode));
                SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillProdPlanScheduleDetail(string Flag, long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo, long ProdSchYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ToolEntryId", ToolEntryId));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                SqlParams.Add(new SqlParameter("@ProdPlanYearCode", ProdPlanYearCode));
                SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
                SqlParams.Add(new SqlParameter("@ProdSchYearCode", ProdSchYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        // 7️⃣ FILL MACHINE LIST
        public async Task<ResponseResult> FillMachineList(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillIssuedByEmpList(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        // 8️⃣ INSERT TOOL ISSUE
        public async Task<ResponseResult> InsertToolIssue(PPCToolIssueMainModel model, DataTable ToolGrid)
           {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                // Convert dates
                var issueDate = model.ToolIssueDate ?? DateTime.Now;
                var entryDate = model.ToolIssueEntryDate ?? DateTime.Now;
                var actualEntryDate = model.ActualEntryDate ?? DateTime.Now;
                var lastUpdatedDate = model.LastUpdatedDate ?? DateTime.Now;

                // INSERT or UPDATE
                if (model.Mode == "U")
                    SqlParams.Add(new SqlParameter("@flag", "UPDATE"));
                else
                    SqlParams.Add(new SqlParameter("@flag", "INSERT"));

                // Main parameters
                SqlParams.Add(new SqlParameter("@ToolIssueEntryId", model.ToolIssueEntryId));
                SqlParams.Add(new SqlParameter("@ToolIssueYearCode", model.ToolIssueYearCode));
                SqlParams.Add(new SqlParameter("@ToolIssueDate", issueDate));
                SqlParams.Add(new SqlParameter("@ToolIssueEntryDate", entryDate));
                SqlParams.Add(new SqlParameter("@ToolIssueSlipNo", model.ToolIssueSlipNo ?? ""));
                SqlParams.Add(new SqlParameter("@IssueToDepartmentId", model.IssueToDepartmentId));
                SqlParams.Add(new SqlParameter("@IssuedByEmpId", model.IssuedByEmpId));
                SqlParams.Add(new SqlParameter("@ApprovedByEmpId", model.ApprovedByEmpId));
                SqlParams.Add(new SqlParameter("@ReceivedByEmpId", model.ReceivedByEmpId));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? ""));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualEntryDate));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedDate", lastUpdatedDate));
                SqlParams.Add(new SqlParameter("@pendingStatus", model.pendingStatus ?? ""));

                // Add TVP
                var tvpParam = new SqlParameter("@dt", SqlDbType.Structured)
                {
                    TypeName = "dbo.Type_PPCToolIssueDetail",
                    Value = ToolGrid
                };
                SqlParams.Add(tvpParam);

                // Execute SP
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
    }
}
