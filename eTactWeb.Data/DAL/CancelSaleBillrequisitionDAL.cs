using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class CancelSaleBillrequisitionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        //private readonly IConfiguration configuration;

        public CancelSaleBillrequisitionDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
       
        public async Task<ResponseResult> SaveCancelSaleBillRequisition(CancelSaleBillrequisitionModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqEntryid", model.CanSaleBillReqEntryid > 0 ? model.CanSaleBillReqEntryid : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", model.CanSaleBillReqYearcode > 0 ? model.CanSaleBillReqYearcode : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqDate", string.IsNullOrEmpty(model.CanSaleBillReqDate) ? DBNull.Value : model.CanSaleBillReqDate));
                    SqlParams.Add(new SqlParameter("@CanRequisitionNo", string.IsNullOrEmpty(model.CanRequisitionNo) ? DBNull.Value : model.CanRequisitionNo));
                    SqlParams.Add(new SqlParameter("@Accountcode", model.AccountCode == 0 ? 0 : model.AccountCode));
                    SqlParams.Add(new SqlParameter("@SaleBillEntryId", model.SaleBillEntryId > 0 ? model.SaleBillEntryId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@SaleBillNo", string.IsNullOrEmpty(model.SaleBillNo) ? DBNull.Value : model.SaleBillNo));
                    SqlParams.Add(new SqlParameter("@SaleBillYearCode", model.SaleBillYearCode > 0 ? model.SaleBillYearCode : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@SaleBillDate", string.IsNullOrEmpty(model.SaleBillDate) ? DBNull.Value : model.SaleBillDate));
                    SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt > 0 ? model.BillAmt : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@INVNetAmt", model.INVNetAmt > 0 ? model.INVNetAmt : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Reasonofcancel", string.IsNullOrEmpty(model.ReasonOfCancel) ? DBNull.Value : model.ReasonOfCancel));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@uid", model.uid > 0 ? model.uid : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@MachineName", string.IsNullOrEmpty(model.MachineName) ? DBNull.Value : model.MachineName));
                   
                }

                else
                {


                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqEntryid", model.CanSaleBillReqEntryid > 0 ? model.CanSaleBillReqEntryid : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", model.CanSaleBillReqYearcode > 0 ? model.CanSaleBillReqYearcode : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CanSaleBillReqDate", string.IsNullOrEmpty(model.CanSaleBillReqDate) ? DBNull.Value : model.CanSaleBillReqDate));
                    SqlParams.Add(new SqlParameter("@CanRequisitionNo", string.IsNullOrEmpty(model.CanRequisitionNo) ? DBNull.Value : model.CanRequisitionNo));
                    SqlParams.Add(new SqlParameter("@Accountcode", model.AccountCode == 0 ? 0 : model.AccountCode));
                    SqlParams.Add(new SqlParameter("@SaleBillEntryId", model.SaleBillEntryId > 0 ? model.SaleBillEntryId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@SaleBillNo", string.IsNullOrEmpty(model.SaleBillNo) ? DBNull.Value : model.SaleBillNo));
                    SqlParams.Add(new SqlParameter("@SaleBillYearCode", model.SaleBillYearCode > 0 ? model.SaleBillYearCode : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@SaleBillDate", string.IsNullOrEmpty(model.SaleBillDate) ? DBNull.Value : model.SaleBillDate));
                    SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt > 0 ? model.BillAmt : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@INVNetAmt", model.INVNetAmt > 0 ? model.INVNetAmt : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Reasonofcancel", string.IsNullOrEmpty(model.ReasonOfCancel) ? DBNull.Value : model.ReasonOfCancel));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@uid", model.uid > 0 ? model.uid : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@MachineName", string.IsNullOrEmpty(model.MachineName) ? DBNull.Value : model.MachineName));
                }
                // Call the stored procedure with the provided parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillRequisition", SqlParams);
            }
            catch (Exception ex)
            {
                // Handle exceptions and prepare the error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> FillSaleBillNo(string CurrentDate,string SaleBillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillNo"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCanSaleBillReqEntryid(string CurrentDate,string SaleBillDate,int CanSaleBillReqYearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYearcode));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCanRequisitionNo(string CurrentDate,string SaleBillDate,int CanSaleBillReqYearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYearcode));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleBillNoYear(string CurrentDate,string SaleBillDate,string  SaleBillNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillNoYear"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerName(string CurrentDate,string SaleBillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerName"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleBillNoDate(string CurrentDate, string SaleBillNo, string SaleBillYearCode,string SaleBillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillNoDate"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@SaleBillYearCode", SaleBillYearCode));
                SqlParams.Add(new SqlParameter("@SaleBillDate", SaleBillDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));

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
        public async Task<CancelSaleBillrequisitionModel> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new CancelSaleBillrequisitionModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSp_CancelSalebillREquisition", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@todate", ToDate);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.CancelSaleBillrequisitionsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                             select new CancelSaleBillrequisitionModel
                                                             {
                                                                 CanSaleBillReqEntryid = Convert.ToInt32(dr["CanSaleBillReqEntryid"]),
                                                                 CanSaleBillReqYearcode = Convert.ToInt32(dr["CanSaleBillReqYearcode"]),
                                                                 CanSaleBillReqDate = dr["CanSaleBillReqDate"].ToString(),
                                                                 CanRequisitionNo = dr["CanRequisitionNo"].ToString(),
                                                                 AccountCode = Convert.ToInt32(dr["AccountCode"]),
                                                                 CustomerName = dr["CustomerName"].ToString(),
                                                                 SaleBillNo = dr["SaleBillNo"].ToString(),
                                                                 SaleBillYearCode = Convert.ToInt32(dr["SaleBillYearCode"]),
                                                                 SaleBillEntryId = Convert.ToInt32(dr["SaleBillEntryId"]),
                                                                 SaleBillDate = dr["SaleBillDate"].ToString(),
                                                                 BillAmt = Convert.ToInt32(dr["BillAmt"]),
                                                                 INVNetAmt = Convert.ToInt32(dr["INVNetAmt"]),
                                                                 ReasonOfCancel = dr["Reasonofcancel"].ToString(),
                                                                 Approvedby = Convert.ToInt32(dr["Approvedby"]),
                                                                 CC = dr["CC"].ToString(),
                                                                 uid = Convert.ToInt32(dr["uid"]),
                                                                 Canceled = dr["Canceled"].ToString(),
                                                                 VoucherType = dr["VoucherType"].ToString(),
                                                                 ApprovalDate = dr["ApprovalDate"].ToString(),
                                                                 CancelDate = dr["CancelDate"].ToString(),
                                                                 MachineName = dr["MachineName"].ToString(),
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
        public async Task<ResponseResult> DeleteByID(int ID, string CanReqNo, string MachineName, int CanSaleBillReqYC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@MachineName", MachineName));
                SqlParams.Add(new SqlParameter("@CanRequisitionNo", CanReqNo));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYC));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSp_CancelSalebillREquisition", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                // Optional: log the error using a logger
                Console.WriteLine($"Error: {Error.Message}");
                Console.WriteLine($"Source: {Error.Source}");
            }

            return _ResponseResult;
        }
        public async Task<CancelSaleBillrequisitionModel> GetViewByID(string CanRequisitionNo,int CanSaleBillReqYearcode)
        {
            var model = new CancelSaleBillrequisitionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@CanRequisitionNo", CanRequisitionNo));
                SqlParams.Add(new SqlParameter("@CanSaleBillReqYearcode", CanSaleBillReqYearcode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSp_CancelSalebillREquisition", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    //PrepareView(_ResponseResult.Result, ref model);
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
    }
}
