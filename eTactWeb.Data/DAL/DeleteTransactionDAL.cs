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
    public class DeleteTransactionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IConfiguration _configuration;
        private readonly ConnectionStringService _connectionStringService;
        private const string SP_NAME = "AdminSPDeleteTransactions";

        public DeleteTransactionDAL(IConfiguration configuration, IDataLogic dataLogic, ConnectionStringService connectionStringService)
        {
            _configuration = configuration;
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
        }

        public async Task<ResponseResult> GetModuleName(string Flag)
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

        public async Task<ResponseResult> GetFormData(string Flag, string ModuleName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ModuleName", ModuleName));
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


        public async Task<ResponseResult> GetSlipNoData(string Flag,string MainTableName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@MainTableName", MainTableName));
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

        public async Task<ResponseResult> InsertAndDeleteTransaction(DeleteTransactionModel model)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>
        {                       
            new SqlParameter("@Flag", model.Flag),
            new SqlParameter("@FormName", model.FormName),
            new SqlParameter("@Action", model.Action),
            new SqlParameter("@SlipNo", model.SlipNo),
            new SqlParameter("@newSlipNo", model.newSlipNo),
            new SqlParameter("@EntryDate", CommonFunc.ParseFormattedDate( model.EntryDate) ?? (object)DBNull.Value),
            new SqlParameter("@YearCode", model.YearCode),
            new SqlParameter("@EntryId", model.EntryId),
            new SqlParameter("@AccountCode", model.AccountCode),
            new SqlParameter("@NetAmount", model.NetAmount),
            new SqlParameter("@BasicAmount", model.BasicAmount),
            new SqlParameter("@CC", model.CC),
            new SqlParameter("@MachineName", model.MachineName),
            new SqlParameter("@ActionByEmpId", model.ActionByEmpId),
            new SqlParameter("@IPAddress", model.IPAddress),
            new SqlParameter("@MainTableName", model.FormName),

        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Message = ex.Message;
            }

            return _ResponseResult;
        }




        public async Task<ResponseResult> UpdateExistingSlipNo(DeleteTransactionModel model)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", model.Flag),
            new SqlParameter("@FormName", model.FormName),
            new SqlParameter("@Action", model.Action),
            new SqlParameter("@SlipNo", model.SlipNo),
            new SqlParameter("@newSlipNo", model.newSlipNo),
            new SqlParameter("@EntryDate", CommonFunc.ParseFormattedDate( model.EntryDate) ?? (object)DBNull.Value),
            new SqlParameter("@YearCode", model.YearCode),
            new SqlParameter("@EntryId", model.EntryId),
            new SqlParameter("@AccountCode", model.AccountCode),
            new SqlParameter("@NetAmount", model.NetAmount),
            new SqlParameter("@BasicAmount", model.BasicAmount),
            new SqlParameter("@CC", model.CC),
            new SqlParameter("@MachineName", model.MachineName),
            new SqlParameter("@ActionByEmpId", model.ActionByEmpId),
            new SqlParameter("@IPAddress", model.IPAddress),
            new SqlParameter("@MainTableName", model.FormName),

        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable(SP_NAME, SqlParams);
            }

            catch (Exception ex)
            {
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Message = ex.Message;
            }

            return _ResponseResult;
        }


    }
}
