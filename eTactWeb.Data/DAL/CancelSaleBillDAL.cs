using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{ 
   public class CancelSaleBillDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CancelSaleBillDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string SaleBillNo, string CustomerName, string CanRequisitionNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PendingRequsitionForCancel"));
                SqlParams.Add(new SqlParameter("@fromdate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@todate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@CustomerName", CustomerName));
                SqlParams.Add(new SqlParameter("@CanRequisitionNo", CanRequisitionNo));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCanRequisitionNo(string CurrentDate, int accountcode, string SaleBillNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(CurrentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCanRequisitionNo"));
                SqlParams.Add(new SqlParameter("@CurrentDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerName(string CurrentDate,  string SaleBillNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(CurrentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerName"));
                SqlParams.Add(new SqlParameter("@CurrentDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleBillNo(string CurrentDate, int accountcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(CurrentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillNo"));
                SqlParams.Add(new SqlParameter("@CurrentDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<List<CancelSaleBillDetails>> ShowSaleBillDetail(int SaleBillEntryId,int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();
            var MainModel = new List<CancelSaleBillDetails>();
            try
            {
            
                SqlParams.Add(new SqlParameter("@Flag", "FillPendingForCancelInvoiceDetail"));
                SqlParams.Add(new SqlParameter("@SaleBillEntryId", SaleBillEntryId));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYearcode));
                SqlParams.Add(new SqlParameter("@SaleBillYearCode", SaleBillYearCode));
                SqlParams.Add(new SqlParameter("@CanRequisitionNo", CanRequisitionNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                var ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);
                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "POCancelMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var CancelSaleBillDetails = CommonFunc.DataRowToClass<CancelSaleBillDetails>(row);

                            MainModel.Add(CancelSaleBillDetails);
                        }


                    }


                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return MainModel;
        }
    }
}
