using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class PORegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IDataReader? Reader;

        public PORegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PORegisterModel> GetPORegisterData(string FromDate, string ToDate, string ReportType,int YearCode, string Partyname, string partcode, string itemName, string POno, string SchNo, string OrderType, string POFor, string ItemType, string ItemGroup)
        {
            DataSet? oDataSet = new DataSet();
            var model = new PORegisterModel();
            var _PODetail = new List<PORegisterDetail>();
            var _ResponseResult = new ResponseResult(); 
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportPurchaseOrderSchedule", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@Yearcode", YearCode);
                    oCmd.Parameters.AddWithValue("@fromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@todate", toDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@Partyname", Partyname == null ? "" : Partyname);
                    oCmd.Parameters.AddWithValue("@partcode", partcode == null ? "" : partcode);
                    oCmd.Parameters.AddWithValue("@itemName", itemName == null ? "" : itemName);
                    oCmd.Parameters.AddWithValue("@POno", POno == null ? "" : POno);
                    oCmd.Parameters.AddWithValue("@SchNo", SchNo == null ? "" : SchNo);
                    oCmd.Parameters.AddWithValue("@OrderType", OrderType == null ? "" : OrderType);
                    oCmd.Parameters.AddWithValue("@POFor", POFor == null ? "" : POFor);
                    oCmd.Parameters.AddWithValue("@ItemType", ItemType == null ? "" : ItemType);
                    oCmd.Parameters.AddWithValue("@ItemGroup", ItemGroup == null ? "" : ItemGroup);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }

                    if (ReportType == "LISTOFPO") //list of po
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    } 
                    if (ReportType == "LISTOFSCHEDULESUMMARY") //list of summary
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    } 
                    if (ReportType == "LISTOFSCHEDULE") //list of sehedule
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    } 
                    if (ReportType == "DETAIL") //PURCHASE ORDER DETAIL
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    }
                    if (ReportType == "SUMM") //PURCHASE ORDER SUMMARY
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    }
                    if (ReportType == "SUMMRATEING") //SUMMARY RATING
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    }
                    if (ReportType == "CONSOLIDATED") //Consolidated
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    }
                    if (ReportType == "PARTYWISECONSOLIDATED") //party WISE CONSOLIDATED
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
                        }
                    }
                    if (ReportType == "ITEMWISECONSOLIDATED") //item wise consolidated
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                var poDetail = CommonFunc.DataRowToClass<PORegisterDetail>(row);
                                _PODetail.Add(poDetail);
                            }
                            model.PORegisterDetails = _PODetail;
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
