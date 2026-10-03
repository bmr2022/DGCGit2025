using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CommonBLL : ICommon
    {
        private CommonDAL _CommonDAL;
        private readonly IDataLogic _IDataLogic;

        public CommonBLL(IConfiguration config, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CommonDAL = new CommonDAL(config, iDataLogic, connectionStringService);
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName)
        {
            return await _CommonDAL.CheckFinYearBeforeSave(YearCode, Date, DateName);
        }
        public async Task<ResponseResult> FillReportTypes(string TableName, string MainReportType)
        {
            return await _CommonDAL.FillReportTypes(TableName, MainReportType);
        }
        public async Task<ResponseResult> GetDashboardData(string spName, string flag, Dictionary<string, object> parameters)
        {
            return await _CommonDAL.GetDashboardData(spName, flag, parameters);
        }
        public async Task<ResponseResult> GetUnit(int ItemCode)
        {
            return await _CommonDAL.GetUnit(ItemCode);
        }
        public async Task<ResponseResult> CheckRoundOff(string unit)
        {
            return await _CommonDAL.CheckRoundOff(unit);
        }
        public async Task<ResponseResult> GetCredential()
        {
            return await _CommonDAL.GetCredential();
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno)
        {
            return await _CommonDAL.FillCurrentBatchINStore(ItemCode, YearCode, FinStartDate, StoreName, batchno);
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, string FinStartDate, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            return await _CommonDAL.GetBatchNumber(SPName, StoreId, StoreName, FinStartDate, ItemCode, TransDate, YearCode, BatchNo);
        }
        public List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(dict);
            }

            return list;
        }
        public async Task<AccessTokenResponse> GetAccessTokenAsync()
        {
            var client = new HttpClient();

            var Credentialdetails = await GetCredential();

            string username = "", password = "", client_id = "",
                   client_secret = "", grant_type = "password",
                   gstin = "";

            if (Credentialdetails.Result != null && Credentialdetails.Result.Rows.Count > 0)
            {
                var row = Credentialdetails.Result.Rows[0];

                username = row["username"].ToString();
                password = row["password"].ToString();
                client_id = row["client_id"].ToString();
                client_secret = row["client_secret"].ToString();
                grant_type = row["grant_type"].ToString();
                gstin = row["gstin"].ToString();
            }

            var data = new Dictionary<string, object>
            {
                { "username", username },
                { "password", password },
                { "client_id", client_id },
                { "client_secret", client_secret },
                { "grant_type", grant_type }
            };

            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://pro.mastersindia.co/oauth/access_token",
                content);

            var result = await response.Content.ReadAsStringAsync();

            var token = JObject.Parse(result)?
                            .SelectToken("access_token")?
                            .ToString();

            return new AccessTokenResponse
            {
                AccessToken = token,
                Gstin = gstin
            };
        }
        public async Task<DataTable> GetInvoiceDetailToExportInXml(string FromDate, string ToDate, string InvoiceNo, int AccountCode, string InvoiceType)
        {
            return await _CommonDAL.GetInvoiceDetailToExportInXml(FromDate, ToDate, InvoiceNo, AccountCode, InvoiceType);
        }
        public async Task<ResponseResult> GetFeatureOptions()
        {
            return await _CommonDAL.GetFeatureOptions();
        }
        public async Task<ResponseResult> GetFormRights(int userId, int menuId)
        {
            return await _CommonDAL.GetFormRights(userId, menuId);
        }
    }
}
