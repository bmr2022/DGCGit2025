using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ProductionEntryReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        private readonly ICommon _common;
        public ProductionEntryReportDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService, ICommon common)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _common = common;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetCompanyName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetCompanyName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillFGPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFGItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGItemName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMItemName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdSlipNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdPlanNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdSchNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillReqNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLReqNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenter(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLWorkCenter"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMachinName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLMachinName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillOperatorName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLOperatorName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProcess(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProcess"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillShiftName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillShiftName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> filltranstore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "filltranstore"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> filltranworkcenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "filltranworkcenter"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetProductionEntryReport(string ReportType, string FromDate,
    string ToDate,
    string FGPartCode,
    string FGItemName,
    string RMPartCode,
    string RMItemName,
    string ProdSlipNo,
    string ProdPlanNo,
    string ProdSchNo,
    string ReqNo,
    string WorkCenter,
    string MachineName,
    string OperatorName,
    string Process,
    string ShiftName,
    int StoreID,
    int WCID,
    string FromSlipNo,
    string ToSlipNo,
    DateTime FromTime,
    DateTime ToTime)
        {
            var parameters = new Dictionary<string, object?>
    {
        { "@ReportType", string.IsNullOrWhiteSpace(ReportType) ? string.Empty : ReportType },
        { "@FromDate", CommonFunc.ParseFormattedDate(FromDate) },
        { "@ToDate", CommonFunc.ParseFormattedDate(ToDate) },
        { "@PartCode", string.IsNullOrWhiteSpace(FGPartCode) ? string.Empty : FGPartCode },
        { "@ItemName", string.IsNullOrWhiteSpace(FGItemName) ? string.Empty : FGItemName },
        { "@RMPartCode", string.IsNullOrWhiteSpace(RMPartCode) ? string.Empty : RMPartCode },
        { "@RMItemName", string.IsNullOrWhiteSpace(RMItemName) ? string.Empty : RMItemName },
        { "@ProdSlipNo", string.IsNullOrWhiteSpace(ProdSlipNo) ? string.Empty : ProdSlipNo },
        { "@ProdPlanNo", string.IsNullOrWhiteSpace(ProdPlanNo) ? string.Empty : ProdPlanNo },
        { "@ProdSchNo", string.IsNullOrWhiteSpace(ProdSchNo) ? string.Empty : ProdSchNo },
        { "@ReqNo", string.IsNullOrWhiteSpace(ReqNo) ? string.Empty : ReqNo },
        { "@WorkcenterName", string.IsNullOrWhiteSpace(WorkCenter) ? string.Empty : WorkCenter },
        { "@processName", string.IsNullOrWhiteSpace(Process) ? string.Empty : Process },
        { "@Operator", string.IsNullOrWhiteSpace(OperatorName) ? string.Empty : OperatorName },
        { "@ShiftName", string.IsNullOrWhiteSpace(ShiftName) ? string.Empty : ShiftName },
        { "@machineName", string.IsNullOrWhiteSpace(MachineName) ? string.Empty : MachineName },
        { "@StoreID", StoreID },
        { "@WCID", WCID },
        { "@FromSlipNo", string.IsNullOrWhiteSpace(FromSlipNo) ? string.Empty : FromSlipNo },
        { "@ToSlipNo", string.IsNullOrWhiteSpace(ToSlipNo) ? string.Empty : ToSlipNo },
        { "@FromTime", FromTime.ToString("HH:mm:ss") },
        { "@ToTime", ToTime.ToString("HH:mm:ss") }
    };

            return await _common.GetDashboardData(
                "SPreportProductionEntry",
                "DASHBOARD",
                parameters
            );
        }
    }
}