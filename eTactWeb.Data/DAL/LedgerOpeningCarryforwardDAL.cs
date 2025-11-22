using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office.Word;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class LedgerOpeningCarryforwardDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public LedgerOpeningCarryforwardDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FILLFINYEAR()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFINYEAR"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("NewFinYearCarryForwardLedgerBalance", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> CarryforwardLedgerbalance(LedgerOpeningCarryforwardModel model)
        {
            var _ResponseResult = new ResponseResult();
            
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Ledgerbalance"));
                SqlParams.Add(new SqlParameter("@OpeningyearCode", model.CarryForwardClosing));
                SqlParams.Add(new SqlParameter("@actualTRansferDate", model.ActualTransferDate));
                SqlParams.Add(new SqlParameter("@ActualTransferBy", model.ActualTransferBy));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("NewFinYearCarryForwardLedgerBalance", SqlParams);
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
