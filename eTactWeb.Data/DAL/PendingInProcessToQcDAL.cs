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
    public class PendingInProcessToQcDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public PendingInProcessToQcDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<DataSet> BindItem(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
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
        public async Task<DataSet> BindProdSlip(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ProdList";
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
        public async Task<DataSet> BindWorkCenter(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
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
        public async Task<ResponseResult> GetDataForPendingMRN(string Flag, string FromDate, string ToDate, string PartCode, string ItemName, string ProdSlipNo, string WorkCenter, string GlobalSearch)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ProdSlipNo", ProdSlipNo == null ? "" : ProdSlipNo));
                SqlParams.Add(new SqlParameter("@partcode", PartCode == null ? "" : PartCode));
                SqlParams.Add(new SqlParameter("@ItemName", ItemName == null ? "" : ItemName));
                SqlParams.Add(new SqlParameter("@Workcenter", WorkCenter == null ? "" : WorkCenter));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDataforQc(DataTable DisplayPendForQC,string Flag, string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@DisplayPendForQC", DisplayPendForQC));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
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
