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
    public class REQUISITIONRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public REQUISITIONRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<REQUISITIONRegistermodel> GetREQUISITIONRegisterData(string Flag,string ReqType,string fromDate, string ToDate, string REQNo, string Partcode, string ItemName, string FromstoreId, string Toworkcenter,int ReqYearcode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new REQUISITIONRegistermodel();
            var _REQDetail = new List<REQUISITIONRegisterDetail>();
            var _ResponseResult = new ResponseResult();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportRequisitionRegister", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(fromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@flag", Flag);
                    oCmd.Parameters.AddWithValue("@ReqType", ReqType);
                    oCmd.Parameters.AddWithValue("@REQNo", REQNo == null ? "" : REQNo);
                    oCmd.Parameters.AddWithValue("@fromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@Partcode", Partcode == null ? "" : ItemName);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName == null ? "" : Partcode);
                    oCmd.Parameters.AddWithValue("@FromstoreId", FromstoreId);
                    oCmd.Parameters.AddWithValue("@Toworkcenter", Toworkcenter );
                    oCmd.Parameters.AddWithValue("@ReqYearcode", ReqYearcode );

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                    if (ReqType=="REQUISITIONWITHOUTBOM")
                    {
                        if (Flag == "REQUISITIONWITHOUTBOMLIST")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var REQDetail = CommonFunc.DataRowToClass<REQUISITIONRegisterDetail>(row);
                                    _REQDetail.Add(REQDetail);
                                }
                                model.REQUISITIONRegisterDetails = _REQDetail;
                            }
                        }
                    }else if (ReqType=="REQUISITIONWITHBOM")
                    {
                        if (Flag == "REQUISITIONWITHBOMLIST" || Flag=="REQUISITIONWITHBOMWITHCHILPARTDETAIL")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var REQDetail = CommonFunc.DataRowToClass<REQUISITIONRegisterDetail>(row);
                                    _REQDetail.Add(REQDetail);
                                }
                                model.REQUISITIONRegisterDetails = _REQDetail;
                            }
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
            return model;
        }
    }
}
