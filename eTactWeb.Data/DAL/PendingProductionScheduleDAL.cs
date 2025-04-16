using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
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
    public class PendingProductionScheduleDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public PendingProductionScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> FillStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "StoreName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Workcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BindItemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BindPartCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPendingProdPlanNo()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PendingProdPlan"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPendingProdPlanYearCode(string ProdPlanNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PendingProdPlanYearCode"));
                SqlParams.Add(new SqlParameter("@planno", ProdPlanNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdScheduleNo(string ProPlanNo,int ProdPlanYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillProdScheduleNo"));
                SqlParams.Add(new SqlParameter("@planno", ProPlanNo));
                SqlParams.Add(new SqlParameter("@PlanNoYearCode", ProdPlanYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDataForPendingProductionSchedule(string Flag, string FromDate, string ToDate, int StoreId, int YearCode, string GlobalSearch,string ProdSchNo, int WcId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
               
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ShowPendingProdSchDetail"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDt));
                SqlParams.Add(new SqlParameter("@ToDate", toDt));
                SqlParams.Add(new SqlParameter("@storeid", StoreId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
                SqlParams.Add(new SqlParameter("@WCID", WcId));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueAgainstProdSchedule", SqlParams);
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
