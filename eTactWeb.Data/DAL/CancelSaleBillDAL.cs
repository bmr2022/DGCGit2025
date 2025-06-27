using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using System.Net.Http.Headers;

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
        public async Task<ResponseResult> SaveCancelation(int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo, int CancelBy, String Cancelreason, String Canceldate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(Canceldate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "UpdateCancelSaleBillApproval"));
                SqlParams.Add(new SqlParameter("@SaleBillYearCode", SaleBillYearCode));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYearcode));
                SqlParams.Add(new SqlParameter("@CanRequisitionNo", CanRequisitionNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@CancelBy", CancelBy));
                SqlParams.Add(new SqlParameter("@Cancelreason", Cancelreason));
                SqlParams.Add(new SqlParameter("@Canceldate", fromdt));
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

        public async Task<ResponseResult> GetInvoiceDataAsync(string invoiceNo, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@SaleBillNo1", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo2", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleSaleBillYearCode", yearCode));
                SqlParams.Add(new SqlParameter("@flag", "GETIRNNO"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPIRNEInvoiceAndEwayBillData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<string> PostCancelIRNAsync(Dictionary<string, object> dictData)
        {
            string urlToPost = "https://pro.mastersindia.co/cancelEinvoice";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string jsonData = JsonConvert.SerializeObject(dictData, Formatting.Indented);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(urlToPost, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(responseString);
                    var irn = jsonResponse.SelectToken("results.message.Irn")?.ToString();
                    var date = jsonResponse.SelectToken("results.message.CancelDate")?.ToString();

                    if (string.IsNullOrWhiteSpace(irn) || string.IsNullOrWhiteSpace(date))
                    {
                        return "IRN Cancel Failed";
                    }

                    var result = new
                    {
                        CancelledIrn = irn,
                        CancelledDate = date
                    };

                    return JsonConvert.SerializeObject(result); // Return as JSON string
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<ResponseResult> CancelEInvoiceAsync(string irn, string gstin, string SaleBillNo, int SaleBillYearCode)
        {
            var result = new ResponseResult();
            try
            {
                var client1 = new HttpClient();
                var data = new Dictionary<string, object>
                {
                    { "username", "bmr_client@outlook.com" },
                    { "password", "Autotax@0214" },
                    { "client_id", "QAVEfAAvibaQdjjXPE" },
                    { "client_secret", "vXIP4fz9sZsr3Zw4wfqhtdQh" },
                    { "grant_type", "password" }
                };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client1.PostAsync("https://pro.mastersindia.co/oauth/access_token", content);
                var result1 = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(result1)?.SelectToken("access_token")?.ToString();

               var urlToPost1 = "https://pro.mastersindia.co/cancelEinvoice";
                var dictData1 = new Dictionary<string, object>
                    {
                        { "access_token", token },
                        { "user_gstin", gstin },
                        { "irn", irn },
                        { "cancel_reason", "1" },
                        { "cancel_remarks", "wrong entry" }
                    };

                string  InvoiceStatus = await PostCancelIRNAsync(dictData1);


                if (InvoiceStatus.Contains("IRN Cancel Failed"))
                {
                    result.StatusText = "Error";
                    result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    result.Result = InvoiceStatus;
                    return result;
                }
                var resultObj = JsonConvert.DeserializeObject<dynamic>(InvoiceStatus);
                string cancelledIrn = resultObj.CancelledIrn;
                string cancelledDate = resultObj.CancelledDate;

                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "UpdateIRNDetails"));
                SqlParams.Add(new SqlParameter("@InvoiceNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@cancelledirnno", cancelledIrn)); // Ensure accountCode is of type int
                SqlParams.Add(new SqlParameter("@cancelleddate", cancelledDate)); // Ensure accountCode is of type int
                SqlParams.Add(new SqlParameter("@yearcode", SaleBillYearCode));
                await _IDataLogic.ExecuteDataTable("SPEInvoiceIRNdetail", SqlParams);
                result.Result = "EInvoice Cancelled Successfully";
                return result;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

    }
}
