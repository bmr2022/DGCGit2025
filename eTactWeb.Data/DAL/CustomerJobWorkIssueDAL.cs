using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
 
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class CustomerJobWorkIssueDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;


        public CustomerJobWorkIssueDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {

            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Customer Jobwork Issue"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Sale Order"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> GetPopUpData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, string prodUnProd, string BOMINd, int RMItemCode, string Partcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //DateTime FinFromDt = new DateTime();
                //DateTime BillChallanDt = new DateTime();

                //FinFromDt = ParseDate(FinYearFromDate);
                //BillChallanDt = ParseDate(billchallandate);
                var entDt = CommonFunc.ParseFormattedDate(EntryDate);
                var challanDt = CommonFunc.ParseFormattedDate(ChallanDate);
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "JOBWORKISSUESUMMARY"));
                SqlParams.Add(new SqlParameter("@IssYear", YearCode));
                SqlParams.Add(new SqlParameter("@FinYearFromDate", entDt));
                SqlParams.Add(new SqlParameter("@billchallandate", challanDt));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("getPendCustomerJobWorkChallanList", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
     
        public async Task<ResponseResult> GetAdjustedChallanDetailsData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, DataTable DTTItemGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var entDt = CommonFunc.ParseFormattedDate(EntryDate);
                var challanDt = CommonFunc.ParseFormattedDate(ChallanDate);
                var SqlParams = new List<dynamic>();
                //cmd.Parameters.AddWithValue("@Flag", "JOBWORKISSUESUMMARY");
                SqlParams.Add(new SqlParameter("@yearCode", YearCode));
                SqlParams.Add(new SqlParameter("@FromFinStartDate", entDt));
                SqlParams.Add(new SqlParameter("@billchallandate", challanDt));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@DTTItemGrid", DTTItemGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpCustomerJobworkAdjustedChallanInGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;

        }


        public async Task<ResponseResult> GetNewEntry(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetCustomers(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerList"));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDistanceData(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetDistance"));
                SqlParams.Add(new SqlParameter("@accountcode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCustomerDetails(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerDetail"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSoNoDetails(string ChallanDate, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime challanDt = new DateTime();

                challanDt = ParseDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@Flag", "FILLSONO"));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", challanDt));
                SqlParams.Add(new SqlParameter("@Accountcode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSOSchedule(string ChallanDate, int AccountCode, int SoNo, int SoNoYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime challanDt = new DateTime();

                challanDt = ParseDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@Flag", "FILLSOSchedule"));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", challanDt));
                SqlParams.Add(new SqlParameter("@Accountcode", AccountCode));
                SqlParams.Add(new SqlParameter("@sOno", SoNo));
                SqlParams.Add(new SqlParameter("@soYearCode", SoNoYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSoNoYearCode(int AccountCode, int SoNo, string ChallanDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime challanDt = new DateTime();

                challanDt = ParseDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@Flag", "FillSOYearCode"));
                SqlParams.Add(new SqlParameter("@SOno", SoNo));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", challanDt));
                SqlParams.Add(new SqlParameter("@accountcode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustOrderNo(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime challanDt = new DateTime();

                challanDt = ParseDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@Flag", "FILLCustomerOrderAndSPDate"));
                SqlParams.Add(new SqlParameter("@SOno", SoNo));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", challanDt));
                SqlParams.Add(new SqlParameter("@accountcode", AccountCode));
                SqlParams.Add(new SqlParameter("@SoYearCode", SoNOYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillScheduleNoAndYear(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime challanDt = new DateTime();

                challanDt = ParseDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@Flag", "FILLSOSchedule"));
                SqlParams.Add(new SqlParameter("@sOno", SoNo));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", challanDt));
                SqlParams.Add(new SqlParameter("@Accountcode", AccountCode));
                SqlParams.Add(new SqlParameter("@soYearCode", SoNOYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> FillPartCodes(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartCodeList"));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemList(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemList"));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetStores()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BINDSTORE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
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

        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@Reportype", "SUMMARY"));
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@Todate", toDt));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<CustJWIssQDashboard> GetDashboardDetailsData(string FromDate, string ToDate, string ReportType)
        {
           
                DataSet? oDataSet = new DataSet();
                var model = new CustJWIssQDashboard();
                try
                {
                    using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                    {
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    SqlCommand oCmd = new SqlCommand("SP_CustomerJobworkIssueMainDetail", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                        oCmd.Parameters.AddWithValue("@Reportype", ReportType);
                        oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                        oCmd.Parameters.AddWithValue("@Todate", toDt);

                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataSet);
                        }
                    }
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        if (ReportType == "SUMMARY")
                        {
                            model.CustJWIssQDashboardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                      select new CustJWIssQDashboard
                                                                      {
                                                                          EntryDate = DateTime.Parse(dr["EntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          ChallanNo = dr["ChallanNo"].ToString(),
                                                                          ChallanDate = DateTime.Parse(dr["ChallanDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          CustomerName = dr["CustomerName"].ToString(),
                                                                          Account_Code = Convert.ToInt32(dr["AccountCode"]),
                                                                          CustomerAddress = dr["CustomerAddress"].ToString(),
                                                                          State = dr["CustState"].ToString(),
                                                                          StateCode = dr["CustStateCode"].ToString(),
                                                                          GSTType = dr["GstType"].ToString(),
                                                                          JWType = dr["JobWorkType"].ToString(),
                                                                          TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                                                                          NetAmount = Convert.ToDecimal(dr["NetAmount"]),
                                                                          EWayBill = dr["EWayBillNo"].ToString(),
                                                                          EntryMachineName = dr["EntryByMachineName"].ToString(),
                                                                          IssuedByEmpName = dr["IssuedByEmpName"].ToString(),
                                                                          VehicleNo = dr["VehicleNo"].ToString(),
                                                                          TimeOfRemovel = dr["TimeOfRemoval"].ToString(),
                                                                          DispatchThrough = dr["DispatchThrough"].ToString(),
                                                                          DispatchTo = dr["DispatchTo"].ToString(),
                                                                          EntryByEmp = dr["EnterdByEmp"].ToString(),
                                                                          ActualEntryDate = DateTime.Parse(dr["ActualEntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          UpdatedByEmp = dr["UpdatedByEmpName"].ToString(),
                                                                          UpdatedOn = DateTime.Parse(dr["UpdatedOn"].ToString()).ToString("dd/MM/yyyy"),
                                                                          Remark = dr["Remark"].ToString(),
                                                                          CC = dr["CC"].ToString(),
                                                                          UID = Convert.ToInt32(dr["UID"].ToString()),
                                                                          BillNo = dr["BillNo"].ToString(),
                                                                          BillYearCode = Convert.ToInt32(dr["BillyearCode"].ToString()),
                                                                          CustJwIssEntryId = Convert.ToInt32(dr["CustJwIssEntryid"]),
                                                                          CustJwIssYearCode = Convert.ToInt32(dr["CustJwIssYearCode"].ToString())
                                                                      }).ToList();
                        }
                        else if (ReportType == "DETAIL")
                        {
                             model.CustJWIssQDashboardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                      select new CustJWIssQDashboard
                                                                      {
                                                                          EntryDate = DateTime.Parse(dr["EntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          ChallanNo = dr["ChallanNo"].ToString(),
                                                                          ChallanDate = DateTime.Parse(dr["ChallanDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          Account_Code = Convert.ToInt32(dr["AccountCode"]),
                                                                          CustomerName = dr["CustomerName"].ToString(),
                                                                          CustomerAddress = dr["CustomerAddress"].ToString(),
                                                                          State = dr["CustState"].ToString(),
                                                                          StateCode = dr["CustStateCode"].ToString(),
                                                                          GSTType = dr["GSTType"].ToString(),
                                                                          JWType = dr["JobWorkType"].ToString(),
                                                                          BOMInd = string.IsNullOrEmpty(dr["BOMInd"]?.ToString()) ? ' ' : dr["BOMInd"].ToString()[0],

                                                                          ProduceUnproduce = dr["producedUnproduced"].ToString(),
                                                                          BOMNO = dr["BOMNO"].ToString(),
                                                                          PartCode = dr["PartCode"].ToString(),
                                                                          ItemName = dr["ItemName"].ToString(),
                                                                          HSNNo = dr["HSNNO"].ToString(),
                                                                          Qty = Convert.ToDecimal(dr["Qty"]),
                                                                          Unit = dr["Unit"].ToString(),
                                                                          Rate = Convert.ToDecimal(dr["Rate"]),
                                                                          //ItemAmount = Convert.ToDecimal(dr["Amount"]),
                                                                          Process = dr["Process"].ToString(),
                                                                          SONO = Convert.ToInt32(dr["SONO"]),
                                                                          CustOrderNo = dr["CustOrderNo"].ToString(),
                                                                          SoDate = DateTime.Parse(dr["SODate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          //SchNo = Convert.ToInt32(dr["SchNo"]),
                                                                          SchNo = dr["SchNo"].ToString(),
                                                                          SchDate = DateTime.Parse(dr["SchDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          SchYearcode = Convert.ToInt32(dr["SchYearcode"]),
                                                                          BatchNo = dr["Batchno"].ToString(),
                                                                          UNiqueBatchNo = dr["uniquebatchno"].ToString(),
                                                                          BatchStock = Convert.ToInt32(dr["BatchStock"]),
                                                                          TotalStock = Convert.ToInt32(dr["TotalStock"]),
                                                                          TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                                                                          NetAmount = Convert.ToDecimal(dr["NetAmount"]),
                                                                          NoOfCases = Convert.ToInt32(dr["NoofCase"]),
                                                                          //EWayBillNo = dr["EWayBillNo"].ToString(),
                                                                          EWayBill = dr["EWayBillNo"].ToString(),
                                                                          EntryMachineName = dr["EntryByMachineName"].ToString(),
                                                                          IssuedByEmpName = dr["IssuedByEmpName"].ToString(),
                                                                          TransporterName = dr["transporter"].ToString(),
                                                                          VehicleNo = dr["VehicleNo"].ToString(),
                                                                          TimeOfRemovel = DateTime.Parse(dr["TimeOfRemoval"].ToString()).ToString("dd/MM/yyyy"),
                                                                          DispatchThrough = dr["DispatchThrough"].ToString(),
                                                                          DispatchTo = dr["DispatchTo"].ToString(),
                                                                          EntryByEmp = dr["EnterdByEmp"].ToString(),
                                                                          ActualEntryDate = DateTime.Parse(dr["ActualEntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                                          UpdatedByEmp = dr["UpdatedByEmpName"].ToString(),
                                                                          UpdatedOn = DateTime.Parse(dr["UpdatedOn"].ToString()).ToString("dd/MM/yyyy"),
                                                                          Remark = dr["Remark"].ToString(),
                                                                          CC = dr["CC"].ToString(),
                                                                          UID = Convert.ToInt32(dr["UID"].ToString()),
                                                                          BillNo = dr["BillNo"].ToString(),
                                                                          BillYearCode = Convert.ToInt32(dr["BillyearCode"].ToString()),
                                                                          CustJwIssEntryId = Convert.ToInt32(dr["CustJwIssEntryid"]),
                                                                          CustJwIssYearCode = Convert.ToInt32(dr["CustJwIssYearCode"].ToString())


                                                                      }).ToList();
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
        public async Task<ResponseResult> DeleteByID(int CustJwIssEntryId, int CustJwIssYearCode, string EntryMachineName, int EntryById, string ActualEntryDate, int Account_Code)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime parsedDate;
                string formattedDate = ActualEntryDate; // Default value

                if (DateTime.TryParseExact(ActualEntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    formattedDate = parsedDate.ToString("dd/MMM/yyyy"); // Convert to "31/Jan/2025"
                }

                SqlParams.Add(new SqlParameter("@Flag", "Delete"));
                SqlParams.Add(new SqlParameter("@CustJwIssEntryid", CustJwIssEntryId));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", CustJwIssYearCode));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryMachineName));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", EntryById));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", formattedDate));
                SqlParams.Add(new SqlParameter("@AccountCode", Account_Code));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        //public async Task<ResponseResult> GetDashboardSummaryData()
        //{
        //    var _ResponseResult = new ResponseResult();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();
        //        SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
        //        SqlParams.Add(new SqlParameter("@Reportype", "SUMMARY"));
        //        SqlParams.Add(new SqlParameter("@FromDate", Constants.FYStartDate));
        //        SqlParams.Add(new SqlParameter("@Todate", Constants.FYEndDate));

        //        _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkIssueMainDetail", SqlParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }
        //    return _ResponseResult;
        //}
        public async Task<ResponseResult> SaveCustomerJWI(CustomerJobWorkIssueModel model, DataTable JWIGrid, DataTable ChallanGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATEInChallan"));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERTInChallan"));
                }
                //DateTime entryDt = new DateTime();
                //DateTime challanDt = new DateTime();
                //DateTime dtEntry = new DateTime();
                //DateTime RemovalTime = new DateTime();

                var entryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                var challanDt = CommonFunc.ParseFormattedDate(model.ChallanDate);
                var dtEntry = CommonFunc.ParseFormattedDate(model.DateEntry);
                var RemovalTime = CommonFunc.ParseFormattedDate(model.EntryTime);

                SqlParams.Add(new SqlParameter("@CustJwIssEntryid", model.EntryId));
                SqlParams.Add(new SqlParameter("@CustJwIssYearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@CustJwIssEntryDate", model.EntryDate));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanNo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@CustJwIssChallanBillDate", model.ChallanDate));
                SqlParams.Add(new SqlParameter("@AccountCode", model.Account_Code));
                SqlParams.Add(new SqlParameter("@CustomerAddress", model.CustomerAddress));
                SqlParams.Add(new SqlParameter("@CustState", model.State));
                SqlParams.Add(new SqlParameter("@CustStateCode", model.StateCode));
                SqlParams.Add(new SqlParameter("@JobWorkType", model.JWType));
                SqlParams.Add(new SqlParameter("@CC", model.Branch));
                //SqlParams.Add(new SqlParameter("@BillNo", model.EntryBill));  // Empty BillNo
                SqlParams.Add(new SqlParameter("@transporter", model.TransporterName));
                SqlParams.Add(new SqlParameter("@VehicleNo", model.VehicleNo));
                SqlParams.Add(new SqlParameter("@TimeOfRemoval", model.EntryDate));
                SqlParams.Add(new SqlParameter("@DispatchThrough", model.DispatchFrom));
                SqlParams.Add(new SqlParameter("@DispatchTo", model.DispatchTo));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.EntryDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.EntryById));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryMachineName));
                //SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@UpdatedOn", model.UpdatedOn ?? ""));
                SqlParams.Add(new SqlParameter("@EWayBillNo", model.EWayBill));
                SqlParams.Add(new SqlParameter("@TotalAmount", model.TotalAmount));
                SqlParams.Add(new SqlParameter("@GSTType", model.GSTType));
                SqlParams.Add(new SqlParameter("@DeptFromID", model.DeptFromID));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@IssuedBy", model.IssuedByEmpName));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                //SqlParams.Add(new SqlParameter("@BillyearCode", model.YearCode)); 
                SqlParams.Add(new SqlParameter("@NetAmount", model.NetAmount));
                SqlParams.Add(new SqlParameter("@DTCustJwIss", JWIGrid));
                SqlParams.Add(new SqlParameter("@DTSSGridAdjust", ChallanGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkIssueMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        //public async Task<CustomerJobWorkIssueModel> GetViewByID(int ID, int YearCode)
        //{
        //    var model = new CustomerJobWorkIssueModel();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();

        //        SqlParams.Add(new SqlParameter("@flag", "ViewById"));
        //        SqlParams.Add(new SqlParameter("@CustJwIssEntryid", ID));
        //        SqlParams.Add(new SqlParameter("@CustJwIssYearCode", YearCode));
        //        var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkIssueMainDetail", SqlParams);

        //        if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
        //        {
        //            PrepareView(_ResponseResult.Result, ref model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return model;
        //}
        public async Task<CustomerJobWorkIssueModel> GetViewByID(int ID, int YearCode)
        {
            var model = new CustomerJobWorkIssueModel();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "ViewById"),
            new SqlParameter("@CustJwIssEntryid", ID),
            new SqlParameter("@CustJwIssYearCode", YearCode)
        };

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkIssueMainDetail", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var ds = (DataSet)_ResponseResult.Result;

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var row = ds.Tables[0].Rows[0];

                        model.CustJwIssEntryId = Convert.ToInt32(row["CustJwIssEntryid"]);
                        model.CustJwIssYearCode = Convert.ToInt32(row["CustJwIssYearCode"]);
                        model.EntryDate = Convert.ToDateTime(row["CustJwIssEntryDate"]).ToString("yyyy-MM-dd");
                        model.ChallanNo = row["ChallanNo"]?.ToString();
                        model.ChallanDate = Convert.ToDateTime(row["ChallanDate"]).ToString("yyyy-MM-dd");
                        model.Account_Code = Convert.ToInt32(row["AccountCode"]);
                        model.CustomerName = row["CustomerName"]?.ToString();
                        model.CustomerAddress = row["CustomerAddress"]?.ToString();
                        model.CustomerState = row["CustState"]?.ToString();
                        model.StateCode = row["CustStateCode"]?.ToString();
                        model.GSTType = row["GSTType"]?.ToString();
                        model.JWType = row["JobWorkType"]?.ToString();
                        model.DeptFromID = Convert.ToInt32(row["DeptFromID"]);
                        model.Remark = row["Remark"]?.ToString();
                        model.CC = row["CC"]?.ToString();
                        model.UID = Convert.ToInt32(row["UID"]);
                        model.BillNo = row["BillNo"]?.ToString();
                        model.BillYearCode = Convert.ToInt32(row["BillyearCode"]);
                        model.TransporterName = row["transporter"]?.ToString();
                        model.VehicleNo = row["VehicleNo"]?.ToString();
                        model.TimeOfRemovel = row["TimeOfRemoval"]?.ToString();
                        model.DispatchThrough = row["DispatchThrough"]?.ToString();
                        model.DispatchTo = row["DispatchTo"]?.ToString();
                        model.ActualEntryDate = Convert.ToDateTime(row["ActualEntryDate"]).ToString("yyyy-MM-dd");
                        model.UpdatedByEmp = row["UpdatedByEmpName"]?.ToString();
                        model.LastUpdatedByDate = Convert.ToDateTime(row["UpdatedOn"]).ToString("yyyy-MM-dd");
                        model.EWayBill = row["EWayBillNo"]?.ToString();
                        model.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                        model.NetAmount = Convert.ToDecimal(row["NetAmount"]);
                    }

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        model.CustJWIDetailGrid = new List<CustomerJobWorkIssueDetail>();

                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            var detail = new CustomerJobWorkIssueDetail
                            {
                                SEQNo = Convert.ToInt32(dr["SEQNo"]),
                                ProduceUnproduce = dr["producedUnproduced"]?.ToString(),
                                GridSONO = dr["SONO"]?.ToString(),
                                CustOrderNo = dr["CustOrderNo"]?.ToString(),
                                SOYear = Convert.ToInt32(dr["SOyearCode"]),
                                SoDate = dr["SODate"]?.ToString(),
                                SOAmmNo = dr["SOAmmNo"]?.ToString(),
                                SOAmmDate = dr["SOAmmDate"]?.ToString(),
                                SchNo = dr["SchNo"]?.ToString(),
                                SchYearcode = dr["SchYearcode"] as int?,
                                SchDate = dr["SchDate"]?.ToString(),
                                ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                PartCode = dr["PartCode"]?.ToString(),
                                ItemName = dr["ItemName"]?.ToString(),
                                BatchNo = dr["Batchno"]?.ToString(),
                                UNiqueBatchNo = dr["uniquebatchno"]?.ToString(),
                                ProcessId = Convert.ToInt32(dr["processid"]),
                                StoreId = Convert.ToInt32(dr["StoreId"]),
                                StoreName = dr["StoreName"]?.ToString(),
                                Qty = Convert.ToDecimal(dr["Qty"]),
                                Unit = dr["Unit"]?.ToString(),
                                PendQty = dr["PendQty"] as int?,
                                ChallanQty = dr["ChallanQty"] as int?,
                                NoOfCases = Convert.ToInt32(dr["NoofCase"]),
                                Rate = Convert.ToDecimal(dr["Rate"]),
                                ItemAmount = Convert.ToInt32(dr["Amount"]),
                                Discountper = Convert.ToInt32(dr["DiscountPer"]),
                                DiscountAmt = Convert.ToInt32(dr["DiscountAmt"]),
                                BatchStock = Convert.ToInt32(dr["BatchStock"]),
                                TotalStock = Convert.ToInt32(dr["TotalStock"]),
                                ItemSize = dr["ItemSize"]?.ToString(),
                                PacketsDetail = dr["PacketsDetail"]?.ToString(),
                                OtherDetail = dr["OtherDetail"]?.ToString(),
                                HSNNo = dr["HSNNO"]?.ToString(),
                                BOMInd = Convert.ToChar(dr["BOMInd"]),
                                ChallanAdjustRate = Convert.ToInt32(dr["ChallanAdjustedRate"]),
                                StdPacking = Convert.ToInt32(dr["StdPacking"]),
                                color = dr["color"]?.ToString(),
                                BOMNO = Convert.ToInt32(dr["BOMNO"]),
                                BOMname = dr["BOMname"]?.ToString(),
                                Bomdate = dr["Bomdate"]?.ToString(),
                                AltUnit = dr["AltUnit"]?.ToString(),
                                AltQty = Convert.ToInt32(dr["AltQty"]),
                            };

                            model.CustJWIDetailGrid.Add(detail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log or return error
                Console.WriteLine($"Error in GetViewByID: {ex.Message}");
            }

            return model;
        }

        private static CustomerJobWorkIssueModel PrepareView(DataSet DS, ref CustomerJobWorkIssueModel? MainModel)
        {
            try
            {
                var ItemList = new List<CustomerJobWorkIssueDetail>();
                DS.Tables[0].TableName = "CustomerJobWorkIssue";
                int cnt = 0;

                MainModel.EntryDate = DateTime.Parse(DS.Tables[0].Rows[0]["EntryDate"].ToString()).ToString("dd/MM/yyyy");
                MainModel.ChallanNo = DS.Tables[0].Rows[0]["ChallanNo"].ToString();
                MainModel.ChallanDate = DateTime.Parse(DS.Tables[0].Rows[0]["ChallanDate"].ToString()).ToString("dd/MM/yyyy");
                MainModel.CustomerName = DS.Tables[0].Rows[0]["CustomerName"].ToString();
                MainModel.CustomerAddress = DS.Tables[0].Rows[0]["CustomerAddress"].ToString();
                MainModel.State = DS.Tables[0].Rows[0]["CustState"].ToString();
                MainModel.StateCode = DS.Tables[0].Rows[0]["CustStateCode"].ToString();
                MainModel.GSTType = DS.Tables[0].Rows[0]["GstType"].ToString();
                MainModel.JWType = DS.Tables[0].Rows[0]["JobWorkType"].ToString();
                MainModel.TotalAmount = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalAmount"]);
                MainModel.NetAmount = Convert.ToDecimal(DS.Tables[0].Rows[0]["NetAmount"]);
                MainModel.EWayBill = DS.Tables[0].Rows[0]["EWayBillNo"].ToString();
                MainModel.EntryMachineName = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();
                MainModel.IssuedByEmpName = DS.Tables[0].Rows[0]["IssuedByEmpName"].ToString();
                MainModel.VehicleNo = DS.Tables[0].Rows[0]["VehicleNo"].ToString();
                MainModel.TimeOfRemovel = DS.Tables[0].Rows[0]["TimeOfRemoval"].ToString();
                MainModel.DispatchThrough = DS.Tables[0].Rows[0]["DispatchThrough"].ToString();
                MainModel.DispatchTo = DS.Tables[0].Rows[0]["DispatchTo"].ToString();
                MainModel.EntryByEmp = DS.Tables[0].Rows[0]["EnterdByEmp"].ToString();
                MainModel.ActualEntryDate = DateTime.Parse(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()).ToString("dd/MM/yyyy");
                MainModel.UpdatedByEmp = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
                MainModel.UpdatedOn = DateTime.Parse(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()).ToString("dd/MM/yyyy");
                MainModel.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
                MainModel.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                MainModel.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
                MainModel.BillNo = DS.Tables[0].Rows[0]["BillNo"].ToString();
                MainModel.BillYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["BillyearCode"].ToString());
                MainModel.CustJwIssEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["CustJwIssEntryid"]);
                MainModel.CustJwIssYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["CustJwIssYearCode"].ToString());


                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new CustomerJobWorkIssueModel
                        {
                            SEQNo = DS.Tables[0].Rows[0]["SEQNo"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SEQNo"]),
                            CustJwIssEntryId = DS.Tables[0].Rows[0]["CustJwIssEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["CustJwIssEntryId"]),
                            CustJwIssYearCode = DS.Tables[0].Rows[0]["CustJwIssYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["CustJwIssYearCode"]),
                            ProduceUnproduce = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ProduceUnproduce"].ToString()) ? "" : DS.Tables[0].Rows[0]["ProduceUnproduce"].ToString(),
                            SONO = DS.Tables[0].Rows[0]["SONO"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SONO"]),
                            CustOrderNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CustOrderNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["CustOrderNo"].ToString(),
                            SOYear = DS.Tables[0].Rows[0]["SOYear"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SOYear"]),
                            SoDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SoDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["SoDate"].ToString(),
                            SOAmmNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SOAmmNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["SOAmmNo"].ToString(),
                            SOAmmDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SOAmmDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["SOAmmDate"].ToString(),
                            SchNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SchNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["SchNo"].ToString(),
                            SchYearcode = DS.Tables[0].Rows[0]["SchYearcode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SchYearcode"]),
                            SchDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SchDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["SchDate"].ToString(),
                            ItemCode = DS.Tables[0].Rows[0]["ItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCode"]),
                            BatchNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["BatchNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["BatchNo"].ToString(),
                            UNiqueBatchNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UNiqueBatchNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["UNiqueBatchNo"].ToString(),
                            ProcessId = DS.Tables[0].Rows[0]["ProcessId"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ProcessId"]),
                            StoreId = DS.Tables[0].Rows[0]["StoreId"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["StoreId"]),
                            Qty = DS.Tables[0].Rows[0]["Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["Qty"]),
                            Unit = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Unit"].ToString()) ? "" : DS.Tables[0].Rows[0]["Unit"].ToString(),
                            PendQty = DS.Tables[0].Rows[0]["PendQty"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["PendQty"]),
                            NoOfCases = DS.Tables[0].Rows[0]["NoOfCases"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["NoOfCases"]),
                            Rate = DS.Tables[0].Rows[0]["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["Rate"]),
                            ItemAmount = DS.Tables[0].Rows[0]["ItemAmount"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ItemAmount"]),
                            Discountper = DS.Tables[0].Rows[0]["Discountper"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["Discountper"]),
                            DiscountAmt = DS.Tables[0].Rows[0]["DiscountAmt"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["DiscountAmt"]),
                            BatchStock = DS.Tables[0].Rows[0]["BatchStock"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["BatchStock"]),
                            TotalStock = DS.Tables[0].Rows[0]["TotalStock"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["TotalStock"]),
                            ItemSize = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ItemSize"].ToString()) ? "" : DS.Tables[0].Rows[0]["ItemSize"].ToString(),
                            PacketsDetail = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["PacketsDetail"].ToString()) ? "" : DS.Tables[0].Rows[0]["PacketsDetail"].ToString(),
                            OtherDetail = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["OtherDetail"].ToString()) ? "" : DS.Tables[0].Rows[0]["OtherDetail"].ToString(),
                            HSNNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["HSNNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["HSNNo"].ToString(),
                            BOMInd = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["BOMInd"]?.ToString()) ? ' ' : DS.Tables[0].Rows[0]["BOMInd"].ToString()[0],

                            ChallanAdjustRate = DS.Tables[0].Rows[0]["ChallanAdjustRate"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ChallanAdjustRate"]),
                            StdPacking = DS.Tables[0].Rows[0]["StdPacking"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["StdPacking"]),
                            color = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["color"].ToString()) ? "" : DS.Tables[0].Rows[0]["color"].ToString(),
                            BOMNO = DS.Tables[0].Rows[0]["BOMNO"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["BOMNO"]),
                            BOMname = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["BOMname"].ToString()) ? "" : DS.Tables[0].Rows[0]["BOMname"].ToString(),
                            Bomdate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Bomdate"].ToString()) ? "" : DS.Tables[0].Rows[0]["Bomdate"].ToString(),
                            AltUnit = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["AltUnit"].ToString()) ? "" : DS.Tables[0].Rows[0]["AltUnit"].ToString(),
                            AltQty = DS.Tables[0].Rows[0]["AltQty"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["AltQty"]),

                        });
                    }
                    MainModel.CustJWIDetailGrid = ItemList;
                }
                return MainModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
