using eTactWeb.Data.BLL;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PendingSaleRejectionDAL
    {
        private readonly ConnectionStringService _connectionStringService;
        public PendingSaleRejectionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        public async Task<ResponseResult> PendingMRNForSaleRejection(string fromDate, string toDate, string mrnNo, string gateNo, string customerName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                 fromDate = CommonFunc.ParseFormattedDate(fromDate);
                 toDate = CommonFunc.ParseFormattedDate(toDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PendingMRNForSaleRejection"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                SqlParams.Add(new SqlParameter("@MRNNO", mrnNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateNo", gateNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CustomerName", customerName ?? string.Empty));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMRNNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPendingMRN"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                SqlParams.Add(new SqlParameter("@MRNNO", mrnNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateNo", gateNo ?? string.Empty));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> FillGateNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPendingGateNo"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                SqlParams.Add(new SqlParameter("@MRNNO", mrnNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateNo", gateNo ?? string.Empty));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerInvNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPendingInvoice"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                SqlParams.Add(new SqlParameter("@MRNNO", mrnNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateNo", gateNo ?? string.Empty));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartyName(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPendingPartyName"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
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
