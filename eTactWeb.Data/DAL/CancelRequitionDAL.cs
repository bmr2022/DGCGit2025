using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CancelRequitionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
     
        public CancelRequitionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> FillItemName(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
                SqlParams.Add(new SqlParameter("@Fromdate", Fromdate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));
                SqlParams.Add(new SqlParameter("@PendCanceledReq", PendCancelReq));
                SqlParams.Add(new SqlParameter("@RequtionType", RequitionType));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCancelrequisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillReqNo(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillReqNo"));
                SqlParams.Add(new SqlParameter("@Fromdate", Fromdate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));
                SqlParams.Add(new SqlParameter("@PendCanceledReq", PendCancelReq));
                SqlParams.Add(new SqlParameter("@RequtionType", RequitionType));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCancelrequisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartcode(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
                SqlParams.Add(new SqlParameter("@Fromdate", Fromdate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));
                SqlParams.Add(new SqlParameter("@PendCanceledReq", PendCancelReq));
                SqlParams.Add(new SqlParameter("@RequtionType", RequitionType));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCancelrequisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<CancelRequitionModel> GetSearchData(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new CancelRequitionModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPCancelrequisition", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@Fromdate", Fromdate);
                    oCmd.Parameters.AddWithValue("@Todate", ToDate);
                    oCmd.Parameters.AddWithValue("@PendCanceledReq", PendCancelReq);
                    oCmd.Parameters.AddWithValue("@RequtionType", RequitionType);
                    oCmd.Parameters.AddWithValue("@ItemCode", ItemCode);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (RequitionType== "RequisitionWithoutBOM")
                {
                    if (ReportType == "FillREquisitionSummary")
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.CancelRequitionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new CancelRequitionModel
                                              {
                                                  ReqNo = dr["REQNo"] != DBNull.Value ? Convert.ToString(dr["REQNo"]) : string.Empty,
                                                  ReqDate = dr["ReqDate"] != DBNull.Value ? Convert.ToString(dr["ReqDate"]) : string.Empty,
                                                  Workcenter = dr["Workcenter"] != DBNull.Value ? Convert.ToString(dr["Workcenter"]) : string.Empty,
                                                  Department = dr["Department"] != DBNull.Value ? Convert.ToString(dr["Department"]) : string.Empty,
                                                  Remarks = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                  ReqByEmp = dr["ReqByEmp"] != DBNull.Value ? Convert.ToString(dr["ReqByEmp"]) : string.Empty,
                                                  EntryByEmp = dr["EntryByEmp"] != DBNull.Value ? Convert.ToString(dr["EntryByEmp"]) : string.Empty,
                                                  Branch = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                                  Completed = dr["Completed"] != DBNull.Value ? Convert.ToString(dr["Completed"]) : string.Empty,
                                                  EntryByMachineName = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                                  Cancel = dr["Cancel"] != DBNull.Value ? Convert.ToString(dr["Cancel"]) : string.Empty,
                                                  CancelReason = dr["CancelReason"] != DBNull.Value ? Convert.ToString(dr["CancelReason"]) : string.Empty,
                                                  ReqEntryId = dr["ReqEntryId"] != DBNull.Value ? Convert.ToInt32(dr["ReqEntryId"]) : 0,
                                                  ReqYearCode = dr["ReqYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ReqYearCode"]) : 0

                                              }).ToList();
                        }
                    }
                    else
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.CancelRequitionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                         select new CancelRequitionModel
                                                         {
                                                             ReqNo = dr["REQNo"] != DBNull.Value ? Convert.ToString(dr["REQNo"]) : string.Empty,
                                                             ReqDate = dr["ReqDate"] != DBNull.Value ? Convert.ToString(dr["ReqDate"]) : string.Empty,
                                                             Workcenter = dr["Workcenter"] != DBNull.Value ? Convert.ToString(dr["Workcenter"]) : string.Empty,
                                                             Department = dr["Department"] != DBNull.Value ? Convert.ToString(dr["Department"]) : string.Empty,
                                                             SeqNo = dr["SeqNo"] != DBNull.Value ? Convert.ToInt32(dr["SeqNo"]) : 0,
                                                             PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                                             ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                             ReqQty = dr["ReqQty"] != DBNull.Value ? Convert.ToDecimal(dr["ReqQty"]) : 0,
                                                             ExpectedDate = dr["ExpectedDate"] != DBNull.Value ? Convert.ToString(dr["ExpectedDate"]) : string.Empty,
                                                             PendQty = dr["PendQty"] != DBNull.Value ? Convert.ToDecimal(dr["PendQty"]) : 0,
                                                             Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                             Stock = dr["Stock"] != DBNull.Value ? Convert.ToDecimal(dr["Stock"]) : 0,
                                                             StoreName = dr["StoreName"] != DBNull.Value ? Convert.ToString(dr["StoreName"]) : string.Empty,
                                                             ItemRemark = dr["ItemRemark"] != DBNull.Value ? Convert.ToString(dr["ItemRemark"]) : string.Empty,
                                                             ItemCanceled = dr["ItemCanceled"] != DBNull.Value ? Convert.ToString(dr["ItemCanceled"]) : string.Empty,
                                                             ItemLocation = dr["ItemLocation"] != DBNull.Value ? Convert.ToString(dr["ItemLocation"]) : string.Empty,
                                                             ItemBinRackNo = dr["ItemBinRackNo"] != DBNull.Value ? Convert.ToString(dr["ItemBinRackNo"]) : string.Empty,
                                                             Remarks = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                             ReqByEmp = dr["ReqByEmp"] != DBNull.Value ? Convert.ToString(dr["ReqByEmp"]) : string.Empty,
                                                             EntryByEmp = dr["EntryByEmp"] != DBNull.Value ? Convert.ToString(dr["EntryByEmp"]) : string.Empty,
                                                             Branch = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                                             Completed = dr["Completed"] != DBNull.Value ? Convert.ToString(dr["Completed"]) : string.Empty,
                                                             EntryByMachineName = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                                             Cancel = dr["Cancel"] != DBNull.Value ? Convert.ToString(dr["Cancel"]) : string.Empty,
                                                             CancelReason = dr["CancelReason"] != DBNull.Value ? Convert.ToString(dr["CancelReason"]) : string.Empty,
                                                             ReqEntryId = dr["ReqEntryId"] != DBNull.Value ? Convert.ToInt32(dr["ReqEntryId"]) : 0,
                                                             ReqYearCode = dr["ReqYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ReqYearCode"]) : 0,
                                                             ItemCode = dr["ItemCode"] != DBNull.Value ? Convert.ToInt32(dr["ItemCode"]) : 0

                                                         }).ToList();
                        }
                    }
                }
                 if (RequitionType== "RequisitionWithBOM")
                {
                    if (ReportType == "FillREquisitionSummary")
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.CancelRequitionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new CancelRequitionModel
                                              {
                                                  ReqNo = dr["REQNo"] != DBNull.Value ? Convert.ToString(dr["REQNo"]) : string.Empty,
                                                  ReqDate = dr["ReqDate"] != DBNull.Value ? Convert.ToString(dr["ReqDate"]) : string.Empty,
                                                  Workcenter = dr["Workcenter"] != DBNull.Value ? Convert.ToString(dr["Workcenter"]) : string.Empty,
                                                  Department = dr["Department"] != DBNull.Value ? Convert.ToString(dr["Department"]) : string.Empty,
                                                  Remarks = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                  ReqByEmp = dr["ReqByEmp"] != DBNull.Value ? Convert.ToString(dr["ReqByEmp"]) : string.Empty,
                                                  EntryByEmp = dr["EntryByEmp"] != DBNull.Value ? Convert.ToString(dr["EntryByEmp"]) : string.Empty,
                                                  Branch = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                                  Completed = dr["Completed"] != DBNull.Value ? Convert.ToString(dr["Completed"]) : string.Empty,
                                                  EntryByMachineName = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                                  Cancel = dr["Cancel"] != DBNull.Value ? Convert.ToString(dr["Cancel"]) : string.Empty,
                                                  CancelReason = dr["CancelReason"] != DBNull.Value ? Convert.ToString(dr["CancelReason"]) : string.Empty,
                                                  ReqEntryId = dr["ReqThrBOMEntryId"] != DBNull.Value ? Convert.ToInt32(dr["ReqThrBOMEntryId"]) : 0,
                                                  ReqYearCode = dr["ReqThrBOMYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ReqThrBOMYearCode"]) : 0

                                              }).ToList();
                        }
                    }
                    else
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.CancelRequitionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                         select new CancelRequitionModel
                                                         {
                                                             ReqNo = dr["REQNo"] != DBNull.Value ? Convert.ToString(dr["REQNo"]) : string.Empty,
                                                             ReqDate = dr["ReqDate"] != DBNull.Value ? Convert.ToString(dr["ReqDate"]) : string.Empty,
                                                             Workcenter = dr["Workcenter"] != DBNull.Value ? Convert.ToString(dr["Workcenter"]) : string.Empty,
                                                             Department = dr["Department"] != DBNull.Value ? Convert.ToString(dr["Department"]) : string.Empty,
                                                             SeqNo = dr["SeqNo"] != DBNull.Value ? Convert.ToInt32(dr["SeqNo"]) : 0,
                                                             PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                                             ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                             ReqQty = dr["ReqQty"] != DBNull.Value ? Convert.ToDecimal(dr["ReqQty"]) : 0,
                                                             ExpectedDate = dr["ExpectedDate"] != DBNull.Value ? Convert.ToString(dr["ExpectedDate"]) : string.Empty,
                                                             PendQty = dr["PendQty"] != DBNull.Value ? Convert.ToDecimal(dr["PendQty"]) : 0,
                                                             Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                             Stock = dr["Stock"] != DBNull.Value ? Convert.ToDecimal(dr["Stock"]) : 0,
                                                             StoreName = dr["StoreName"] != DBNull.Value ? Convert.ToString(dr["StoreName"]) : string.Empty,
                                                             ItemRemark = dr["ItemRemark"] != DBNull.Value ? Convert.ToString(dr["ItemRemark"]) : string.Empty,
                                                             ItemCanceled = dr["ItemCanceled"] != DBNull.Value ? Convert.ToString(dr["ItemCanceled"]) : string.Empty,
                                                             ItemLocation = dr["ItemLocation"] != DBNull.Value ? Convert.ToString(dr["ItemLocation"]) : string.Empty,
                                                             ItemBinRackNo = dr["ItemBinRackNo"] != DBNull.Value ? Convert.ToString(dr["ItemBinRackNo"]) : string.Empty,
                                                             Remarks = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                             ReqByEmp = dr["ReqByEmp"] != DBNull.Value ? Convert.ToString(dr["ReqByEmp"]) : string.Empty,
                                                             EntryByEmp = dr["EntryByEmp"] != DBNull.Value ? Convert.ToString(dr["EntryByEmp"]) : string.Empty,
                                                             Branch = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                                             Completed = dr["Completed"] != DBNull.Value ? Convert.ToString(dr["Completed"]) : string.Empty,
                                                             EntryByMachineName = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                                             Cancel = dr["Cancel"] != DBNull.Value ? Convert.ToString(dr["Cancel"]) : string.Empty,
                                                             CancelReason = dr["CancelReason"] != DBNull.Value ? Convert.ToString(dr["CancelReason"]) : string.Empty,
                                                             ReqEntryId = dr["ReqThrBOMEntryId"] != DBNull.Value ? Convert.ToInt32(dr["ReqThrBOMEntryId"]) : 0,
                                                             ReqYearCode = dr["ReqThrBOMYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ReqThrBOMYearCode"]) : 0

                                                         }).ToList();
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
        public async Task<ResponseResult> UpdateCompleteRequisitionMultiple(CancelRequisitionRequest request)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                // Convert List to DataTable for SQL
                DataTable ReqList = new DataTable();
                ReqList.Columns.Add("REQEntryId", typeof(long));
                ReqList.Columns.Add("ReqYearCode", typeof(long));
                ReqList.Columns.Add("ReqTYpeThrBomWithoutBOM", typeof(string));
                ReqList.Columns.Add("Reqno", typeof(string));
                ReqList.Columns.Add("CancelReason", typeof(string));
                ReqList.Columns.Add("ItemCode", typeof(long));
                foreach (var r in request.Requisitions)
                {
                    ReqList.Rows.Add(r.ReqEntryId, r.ReqYearCode,r.RequitionType,r.ReqNo, r.CancelReason,r.ItemCode);
                }

                // Prepare SQL parameters
                var SqlParams = new List<dynamic>();
                if (request.ReportType== "FillREquisitionDetail"&&request.RequitionType== "RequisitionWithoutBOM")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UpdateCompleteRequisitionWithoutDetail"));
                }
                else if (request.ReportType == "FillREquisitionSummary" && request.RequitionType == "RequisitionWithoutBOM")
                {

                    SqlParams.Add(new SqlParameter("@Flag", "UpdateCompleteRequisitionWithout"));
                }
                else if (request.ReportType == "FillREquisitionSummary" && request.RequitionType == "RequisitionWithBOM")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UpdateRequisitionWithBom"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UpdateRequisitionWithBomDetail"));
                }

                SqlParams.Add(new SqlParameter("@dt", ReqList));
                SqlParams.Add(new SqlParameter("@PendCanceledReq", request.PendCancelReq));
                SqlParams.Add(new SqlParameter("@Fromdate", request.FromDate));
                SqlParams.Add(new SqlParameter("@Todate", request.ToDate));
                SqlParams.Add(new SqlParameter("@RequtionType", request.RequitionType));

                // Call your stored procedure (just like SaveControlPlan)
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCancelrequisition", SqlParams);
                if (_ResponseResult.Result != null)
                {
                    var dt = _ResponseResult.Result as DataTable; // cast dynamic to DataTable
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];

                        // Use the new column names from SQL
                        bool isSuccess = dt.Columns.Contains("IsSuccess") && row["IsSuccess"].ToString() == "1";
                        string message = dt.Columns.Contains("Message") ? row["Message"].ToString() : "No message";

                        _ResponseResult.IsSuccess = isSuccess;
                        _ResponseResult.StatusText = message;
                        _ResponseResult.Message = message;
                    }
                }



            }
            catch (Exception ex)
            {
                _ResponseResult.IsSuccess = false;
                _ResponseResult.Message = ex.Message;
            }

            return _ResponseResult;
        }


    }
}
