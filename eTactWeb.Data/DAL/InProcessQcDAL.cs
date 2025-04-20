using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
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
    public class InProcessQcDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public InProcessQcDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        internal async Task<InProcessQc> GetViewByID(int ID, int YearCode)
        {
            var model = new InProcessQc();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@InProcQCEntryId", ID));
                SqlParams.Add(new SqlParameter("@InProcQCYearcode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }
        private static InProcessQc PrepareView(DataSet DS, ref InProcessQc? model)
        {
            var InProcessQcDetail = new List<InProcessQcDetail>();
            DS.Tables[0].TableName = "InProcessQc";
            DS.Tables[1].TableName = "InProcessQcDetail";
            int cnt = 0;
            model.InProcessEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["InProcQCEntryId"].ToString());
            model.InProcQCYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["InProcQCYearCode"].ToString());
            model.InProcQcSlipNo = DS.Tables[0].Rows[0]["InProcQCSlipNo"].ToString();
            model.QcClearedbyEmpId=Convert.ToInt32(DS.Tables[0].Rows[0]["QCClearedByEmpId"].ToString());
            model.QcCleaningDate = DS.Tables[0].Rows[0]["QCClearingDate"].ToString();
            model.EnteredbyMachineName = DS.Tables[0].Rows[0]["EnteredByMachine"].ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryById"].ToString());
            model.ActualEntrydate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.LastUpdatedBy=Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
            model.LastUpdatedDate=DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
            model.CC=DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID=Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.Materialtransfer=DS.Tables[0].Rows[0]["MaterialIstransfered"].ToString();

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedOn = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
            }
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    InProcessQcDetail.Add(new InProcessQcDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        InProcessEntryId = Convert.ToInt32(row["InProcQCEntryId"].ToString()),
                        InProcQCYearCode = Convert.ToInt32(row["InProcQCYearCode"].ToString()),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        ProdEntryId = Convert.ToInt32(row["ProdEntryid"].ToString()),
                        ProdSlipNo=row["ProdSlipNo"].ToString(),
                        ProdYearcode = Convert.ToInt32(row["ProdYearCode"].ToString()),
                        ProdDate = row["ProdEntryDate"].ToString(),
                        ProdPlanSchNo = row["ProdSchNo"].ToString(),
                        ProdPlanSchYearCode = Convert.ToInt32(row["ProdSchYearCode"].ToString()),
                        ProdPlanSchDate = row["ProdSchdate"].ToString(),
                        ProdPlanNo = row["ProdPlanNo"].ToString(),
                        ProdPlanYearCode = Convert.ToInt32(row["ProdPlanYearCode"].ToString()),
                        ProdPlanDate = row["ProdPlanDate"].ToString(),
                        Reqno = row["ReqNo"].ToString(),
                        ReqYearCode = Convert.ToInt32(row["ReqYearCode"].ToString()),
                        ReqDate = row["ReqDate"].ToString(),
                        TotProdQty = Convert.ToDecimal(row["ProdQty"].ToString()),
                        OKProdQty = Convert.ToDecimal(row["ProdOkQty"].ToString()),
                        QCOKQty = Convert.ToDecimal(row["OkQty"].ToString()),
                        QCRejQty = Convert.ToDecimal(row["RejQty"].ToString()),
                        RewQty = Convert.ToDecimal(row["RewQty"].ToString()),
                        RejReason = row["RejReason"].ToString(),
                        RewRemark = row["RewRemark"].ToString(),
                        otherdetail = (row["otherdetail"].ToString()),
                        BatchNo = row["Batchno"].ToString(),
                        UniqueBatchNo = (row["Uniquebatchno"].ToString()),
                        QcClearByEMPId = Convert.ToInt32(row["QcClearByEMPId"].ToString()),
                        ApprovedByEmpId = Convert.ToInt32(row["ApprovedByEmpId"].ToString()),
                        TransferedQty = Convert.ToDecimal(row["TransferedQty"].ToString()),
                        PendForTransfQty=Convert.ToDecimal(row["PendForTransfQty"]),
                        ToStoreOrWc = row["TransferToWcOrStore"].ToString(),
                        ToWcId=Convert.ToInt32(row["TransferToWC"].ToString()),
                        ToWorkCenter = row["ToWorkCenter"].ToString(),
                        ToStoreId=Convert.ToInt32(row["TransferToStore"].ToString()),
                        ToStoreName = row["ToStoreName"].ToString(),
                        ProcessId=Convert.ToInt32(row["ProcessId"].ToString()),
                        WcId=Convert.ToInt32(row["ProdInWC"].ToString()),
                        WorkCenter = row["WorkCenter"].ToString(),
                        MaterialIstransfered=row["MaterialIstransfered"].ToString(),
                        Sampleqtytested=Convert.ToDecimal(row["Sampleqtytested"].ToString()),
                        TestingMethod=row["TestingMethod"].ToString()
                    });
                }
                model.ItemDetailGrid = InProcessQcDetail;
            }
            return model;
        }
        public async Task<ResponseResult> FillEntryId(string Flag, int yearCode, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@InProcQCYearcode", yearCode));
                SqlParams.Add(new SqlParameter("@QCClearingDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<DataSet> BindEmpList(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InProcessQC", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "EmpList";
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
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }
        }
        public async Task<ResponseResult> SaveInprocessQc(InProcessQc model, DataTable InProcessQcGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                //DateTime QcDate = new DateTime();
                //DateTime entryDate= new DateTime();
                //DateTime updationDate=new DateTime();   

               var QcDate = CommonFunc.ParseFormattedDate(model.QcCleaningDate);
               var entryDate= CommonFunc.ParseFormattedDate(model.ActualEntrydate);
               var updationDate= CommonFunc.ParseFormattedDate(model.UpdatedOn);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedDate", updationDate));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@InProcQCEntryId", model.InProcessEntryId));
                SqlParams.Add(new SqlParameter("@InProcQCYearCode", model.InProcQCYearCode));
                SqlParams.Add(new SqlParameter("@InProcQCSlipNo", model.InProcQcSlipNo ?? ""));
                SqlParams.Add(new SqlParameter("@QCClearedByEmpId", model.QcClearedbyEmpId));
                SqlParams.Add(new SqlParameter("@QCClearingDate", QcDate));
                SqlParams.Add(new SqlParameter("@EnteredByMachine", model.EnteredbyMachineName));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@MaterialIstransfered", model.Materialtransfer ?? ""));
                SqlParams.Add(new SqlParameter("@ActualEntryById", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entryDate== default ? string.Empty : entryDate));

                SqlParams.Add(new SqlParameter("@DTRGPChalItemGrid", InProcessQcGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
             }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var currentDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                DateTime firstDateOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var firstDateOfMonthh= CommonFunc.ParseFormattedDate(firstDateOfMonth.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", firstDateOfMonthh));
                SqlParams.Add(new SqlParameter("@ToDate", currentDate));

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
        public async Task<ResponseResult> FillTransferToWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillToWorkCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTransferToStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillToStoreName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillToStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillToStoreName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRejectionreason()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillRejectionreason"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillReworkreason()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillReworkreason"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<InProcessDashboard> GetDashboardData(string FromDate, string ToDate, string QcSlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ProdSlipNo, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new InProcessDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_InProcessQC", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.InProcessDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                 select new InProcessQcDashboard    
                                                 {
                                                     InProcQCEntryId = Convert.ToInt32(dr["InProcQCEntryId"]),
                                                     InProcQCYearCode = Convert.ToInt32(dr["InProcQCYearCode"]),
                                                     InProcQCSlipNo =dr["InProcQCSlipNo"].ToString(),
                                                     QCClearingDate = dr["QCClearingDate"].ToString().Split(" ")[0],
                                                     QcClearedEmpId = Convert.ToInt32(dr["QCClearedByEmpId"]),
                                                     QcClearedEmp=dr["QcClearedByEmp"].ToString(),
                                                     EnteredByMachine=dr["EnteredByMachine"].ToString(),
                                                     ActualEntryById = Convert.ToInt32(dr["ActualEntryById"]),
                                                     ActualEmpName=dr["ActualEmpName"].ToString(),
                                                     ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                     LastUpdatedBy = Convert.ToInt32(dr["LastUpdatedBy"]),
                                                     LastUpdatedByEmp=dr["LastUpdatedByEmp"].ToString(),
                                                     LastUpdatedDate=dr["LastUpdatedDate"].ToString().Split(" ")[0],
                                                     CC=dr["CC"].ToString(),
                                                     MaterialIstransfered=dr["MaterialIstransfered"].ToString(),
                                                 }).ToList();
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
        public async Task<InProcessDashboard> GetDashboardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new InProcessDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_InProcessQC", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD");
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.InProcessDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new InProcessQcDashboard
                                                {
                                                    InProcQCEntryId = Convert.ToInt32(dr["InProcQCEntryId"]),
                                                    InProcQCYearCode = Convert.ToInt32(dr["InProcQCYearCode"]),
                                                    InProcQCSlipNo =dr["InProcQCSlipNo"].ToString(),
                                                    ItemName =dr["ItemName"].ToString(),
                                                    PartCode =dr["PartCode"].ToString(),
                                                    ProdEntryid = Convert.ToInt32(dr["ProdEntryid"]),
                                                    ProdYearCode = Convert.ToInt32(dr["ProdYearCode"]),
                                                    ProdSlipNo =dr["ProdSlipNo"].ToString(),
                                                    ProdEntryDate = dr["ProdEntryDate"].ToString().Split(" ")[0],
                                                    ProdSchNo =dr["ProdSchNo"].ToString(),
                                                    ProdSchYearCode = Convert.ToInt32(dr["ProdSchYearCode"]),
                                                    ProdSchdate = dr["ProdSchdate"].ToString().Split(" ")[0],
                                                    ProdPlanNo =dr["ProdPlanNo"].ToString(),
                                                    ProdPlanYearCode = Convert.ToInt32(dr["ProdPlanYearCode"]),
                                                    ProdPlanDate = dr["ProdPlanDate"].ToString().Split(" ")[0],
                                                    ReqNo =dr["ReqNo"].ToString(),
                                                    ReqDate = dr["ReqDate"].ToString().Split(" ")[0],
                                                    ReqYearCode = Convert.ToInt32(dr["ReqYearCode"]),
                                                    ProdQty = Convert.ToDecimal(dr["ProdQty"]),
                                                    ProdOkQty = Convert.ToDecimal(dr["ProdOkQty"]),
                                                    OkQty = Convert.ToDecimal(dr["OkQty"]),
                                                    RejQty = Convert.ToDecimal(dr["RejQty"]),
                                                    RewQty = Convert.ToDecimal(dr["RewQty"]),
                                                    EnteredByMachine=dr["EnteredByMachine"].ToString(),
                                                    RejReason=dr["RejReason"].ToString(),
                                                    RewRemark=dr["RewRemark"].ToString(),
                                                    otherdetail=dr["otherdetail"].ToString(),
                                                    Batchno=dr["Batchno"].ToString(),
                                                    Uniquebatchno=dr["Uniquebatchno"].ToString(),
                                                    TransferedQty = Convert.ToDecimal(dr["TransferedQty"]),
                                                    PendForTransfQty = Convert.ToDecimal(dr["PendForTransfQty"]),
                                                    TransfertoWorkCenter=dr["TransfertoWorkCenter"].ToString(),
                                                    TransfertoStoreName=dr["TransfertoStoreName"].ToString(),
                                                    ProcessName=dr["ProcessName"].ToString(),
                                                    WorkCenter=dr["ProdInWorkCenter"].ToString(),
                                                    Sampleqtytested = Convert.ToDecimal(dr["Sampleqtytested"]),
                                                    TestingMethod=dr["TestingMethod"].ToString(),
                                                    QCClearingDate = dr["QCClearingDate"].ToString().Split(" ")[0],
                                                    ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                    LastUpdatedDate=dr["LastUpdatedDate"].ToString().Split(" ")[0],
                                                    CC=dr["CC"].ToString(),
                                                    MaterialIstransfered=dr["MaterialIstransfered"].ToString(),
                                                }).ToList();
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
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate)
        {
            var _ResponseResult = new ResponseResult();
            var entrydt = ParseDate(EntryDate);
            string formattedEntryDate = entrydt.ToString("yyyy-MM-dd");
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@InProcQCEntryId", ID));
                SqlParams.Add(new SqlParameter("@InProcQCYearcode", YC));
                SqlParams.Add(new SqlParameter("@cc", CC));
                SqlParams.Add(new SqlParameter("@EnteredByMachine", EntryByMachineName));
                SqlParams.Add(new SqlParameter("@QCClearingDate", formattedEntryDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InProcessQC", SqlParams);
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
