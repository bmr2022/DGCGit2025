using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class PendingToReceiveItemDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public PendingToReceiveItemDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetDataForPendingReceiveItem(string Flag, string FromDate, string ToDate,string partcode,string itemname, string slipno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //SqlParams.Add(new SqlParameter("@StartDate", StartDate));
                //SqlParams.Add(new SqlParameter("@EndDate", EndDate));
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", Flag));
                //SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                //SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@ToDate", toDt));
                SqlParams.Add(new SqlParameter("@ItemName", itemname));
                SqlParams.Add(new SqlParameter("@PartCode", partcode));
                SqlParams.Add(new SqlParameter("@TransferMatSlipNo", slipno));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<DataSet> BindItem(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ItemList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<DataSet> BindPartCode(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ItemList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<DataSet> BindWorkCenter(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "WorkCenterList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<DataSet> BindProdSlipNo(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ProdSlipNoList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<DataSet> BindStoreName(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "StoreNameList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<DataSet> BindProdType(string Flag, string FromDate, string ToDate)
        {
            var oDataSet = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FlagForPendingExisting", "PendingToRec"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ProdTypeList";
                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<ResponseResult> GetDataReceiveItem(DataTable DisplayPendReceiveItem)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayFromPendtoNewEntry"));
                SqlParams.Add(new SqlParameter("@PendingTransferEntyryList", DisplayPendReceiveItem));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
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
