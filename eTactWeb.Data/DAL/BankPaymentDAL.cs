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
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.DAL
{
    public class BankPaymentDAL
    {

        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public BankPaymentDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
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
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime vchDt = new DateTime();
                vchDt = ParseDate(VoucherDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetLedgerBalance"));
                SqlParams.Add(new SqlParameter("@Yearcode", OpeningYearCode));
                SqlParams.Add(new SqlParameter("@Accountcode", AccountCode));
                SqlParams.Add(new SqlParameter("@VoucherDate", ParseDate(VoucherDate)));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillLedger"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@type", Type));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SUBVOUCHER"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetLedgerType"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillIntrument"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillModeofAdjust"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillPONO( string accountcode, string VoucherDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "SHOWPOListFORADVANCEPAYMENT"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@ModOfAdjutment", "Advance"));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@VoucherDate", VoucherDate));
                

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetPODetail(int PONO, string accountcode, string VoucherDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "SHOWPOYearFORADVANCEPAYMENT"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@ModOfAdjutment", "Advance"));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@VoucherDate", VoucherDate));
                SqlParams.Add(new SqlParameter("@PONO", PONO));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> GetPODate(int PONO, string accountcode, string VoucherDate, string POYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "SHOWPODateFORADVANCEPAYMENT"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@ModOfAdjutment", "Advance"));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@VoucherDate", VoucherDate));
                SqlParams.Add(new SqlParameter("@POYearCode", POYearCode));
                SqlParams.Add(new SqlParameter("@PONO", PONO));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillCostCenterName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCostcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NEWENTRY"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@VoucherDate", ParseFormattedDate(VoucherDate))); 
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrency()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CURRENCY"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveBankPayment(BankPaymentModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                DateTime entryDate = new DateTime();
                DateTime actualEntryDate = new DateTime();
                DateTime voucherDate = new DateTime();
                DateTime InsDate = new DateTime();

                entryDate = ParseDate(model.EntryDate);
                actualEntryDate = ParseDate(model.ActualEntryDate);
                model.BankRECO = DateTime.Now.ToString();
                voucherDate = ParseDate(model.VoucherDate);
                InsDate = ParseDate(model.InsDate);

                var sqlParams = new List<dynamic>();
                sqlParams.Add(new SqlParameter("@voucherNo", model.VoucherNo));
                if (model.Mode == "U" || model.Mode == "V")
                {
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                sqlParams.Add(new SqlParameter("@BooktrnsEntryId", model.AccEntryId));
                sqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                sqlParams.Add(new SqlParameter("@EntryDate", entryDate));
                sqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                sqlParams.Add(new SqlParameter("@entrybymachine", model.EntryByMachine));
                sqlParams.Add(new SqlParameter("@VoucherDate", voucherDate));
                sqlParams.Add(new SqlParameter("@Subvoucher", "Bank-Payment"));
                sqlParams.Add(new SqlParameter("@ActualEntryDate", actualEntryDate));
                sqlParams.Add(new SqlParameter("@InstrumentNo", model.InsNo));
                sqlParams.Add(new SqlParameter("@intrument", model.Intrument));
                sqlParams.Add(new SqlParameter("@intrumentdate", InsDate));
                sqlParams.Add(new SqlParameter("@cc", model.CC));
                sqlParams.Add(new SqlParameter("@DTbooktrans", GIGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", sqlParams);

            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            var responseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@summDetail", "Summary"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@fromdate", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@todate", ParseFormattedDate(ToDate)));
                responseResult = await _IDataLogic.ExecuteDataSet("AccSpVoucherEntry", SqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<BankPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new BankPaymentModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpVoucherEntry", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@summDetail", "Detail");
                    oCmd.Parameters.AddWithValue("@VoucherType", "Bank-Payment");
                    oCmd.Parameters.AddWithValue("@fromdate", ParseFormattedDate(FromDate));
                    oCmd.Parameters.AddWithValue("@todate", ParseFormattedDate(ToDate));

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.BankPaymentGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new BankPaymentModel
                                             {
                                                 LedgerName = dr["LedgerName"] != DBNull.Value ? dr["LedgerName"].ToString() : string.Empty,
                                                 //AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                                 VoucherNo = dr["VoucherNo"] != DBNull.Value ? dr["VoucherNo"].ToString() : string.Empty,
                                                 VoucherDate = dr["VchDate"] != DBNull.Value ? dr["VchDate"].ToString() : string.Empty,
                                                 DrAmt = dr["DrAmt"] != DBNull.Value ? Convert.ToDecimal(dr["DrAmt"]) : 0,
                                                 CrAmt = dr["CrAmt"] != DBNull.Value ? Convert.ToDecimal(dr["CrAmt"]) : 0,
                                                 VoucherType = dr["VoucherType"] != DBNull.Value ? dr["VoucherType"].ToString() : string.Empty,
                                                 //ActualEntryBy = dr["ActualEntryByEmp"] != DBNull.Value ? dr["ActualEntryByEmp"].ToString() : string.Empty,
                                                 ActualEntryBy = dr["ActualEntryByEmp"] != DBNull.Value ? dr["ActualEntryByEmp"].ToString() : string.Empty,
                                                 ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                 UpdatedOn = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["LastUpdatedDate"]) : (DateTime?)null,
                                                 //UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedBy"]) : 0,
                                                 UpdatedByEmp = dr["UpdatedByEmp"] != DBNull.Value ? dr["UpdatedByEmp"].ToString() : string.Empty,
                                                 EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? dr["EntryByMachine"].ToString() : string.Empty,
                                                 SubVoucherName = dr["SubVoucherName"] != DBNull.Value ? dr["SubVoucherName"].ToString() : string.Empty,
                                                 AccEntryId = dr["AccEntryId"] != DBNull.Value ? Convert.ToInt32(dr["AccEntryId"]) : 0,
                                                 YearCode = dr["AccYearCode"] != DBNull.Value ? Convert.ToInt32(dr["AccYearCode"]) : 0,
                                                 EntryDate = dr["EntryDate"] != DBNull.Value ? dr["EntryDate"].ToString() : string.Empty

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
        public async Task<BankPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new BankPaymentModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpVoucherEntry", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@summDetail", "Summary");
                    oCmd.Parameters.AddWithValue("@VoucherType", "Bank-Payment");
                    oCmd.Parameters.AddWithValue("@fromdate", ParseFormattedDate(FromDate));
                    oCmd.Parameters.AddWithValue("@todate", ParseFormattedDate(ToDate));

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.BankPaymentGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new BankPaymentModel
                                             {
                                                 AccEntryId = dr["AccEntryId"] != DBNull.Value ? Convert.ToInt32(dr["AccEntryId"]) : 0,
                                                 YearCode = dr["AccYearCode"] != DBNull.Value ? Convert.ToInt32(dr["AccYearCode"]) : 0,
                                                 EntryDate = dr["EntryDate"] != DBNull.Value ? dr["EntryDate"].ToString() : string.Empty,
                                                 SubVoucherName = dr["SubVoucherName"] != DBNull.Value ? dr["SubVoucherName"].ToString() : string.Empty,
                                                 VoucherNo = dr["VoucherNo"] != DBNull.Value ? dr["VoucherNo"].ToString() : string.Empty,
                                                 VoucherDocDate = dr["VoucherDocdate"] != DBNull.Value ? dr["VoucherDocdate"].ToString() : string.Empty,
                                                 Currency = dr["Currency"] != DBNull.Value ? dr["Currency"].ToString() : string.Empty,
                                                 VoucherType = dr["Vouchertype"] != DBNull.Value ? dr["Vouchertype"].ToString() : string.Empty,
                                                 UID = dr["uid"] != DBNull.Value ? Convert.ToInt32(dr["uid"]) : 0,
                                                 ActualEntryby = dr["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryBy"]) : 0,
                                                 ActualEntryBy = dr["ActualEntryByEmp"] != DBNull.Value ? dr["ActualEntryByEmp"].ToString() : string.Empty,
                                                 ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                 UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedBy"]) : 0,
                                                 UpdatedByEmp = dr["UpdatedByEmp"] != DBNull.Value ? dr["UpdatedByEmp"].ToString() : string.Empty,
                                                 UpdatedOn = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["LastUpdatedDate"]) : (DateTime?)null,
                                                 EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? dr["EntryByMachine"].ToString() : string.Empty,
                                                 CC = dr["CC"] != DBNull.Value ? dr["CC"].ToString() : string.Empty,

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
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
                SqlParams.Add(new SqlParameter("@AccEntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", ActualEntryBy));
                SqlParams.Add(new SqlParameter("@EntryByMachine", EntryByMachine));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ParseFormattedDate(ActualEntryDate)));
                SqlParams.Add(new SqlParameter("@Vouchertype", "Bank-Payment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
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
        public async Task<BankPaymentModel> PopUpForPendingVouchers(PopUpDataTable DataTable)
        {
            DataSet? oDataSet = new DataSet();
            var model = new BankPaymentModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpPopupForPendingVouchersToBeAdjusted", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@VoucherType", DataTable.VoucherType);
                    oCmd.Parameters.AddWithValue("@Flag", DataTable.Flag);
                    oCmd.Parameters.AddWithValue("@summDetail", DataTable.SumDetail);
                    oCmd.Parameters.AddWithValue("@Accountcode", DataTable.AccountCode);
                    oCmd.Parameters.AddWithValue("@yearcode", DataTable.YearCode);
                    oCmd.Parameters.AddWithValue("@VoucherDate", ParseFormattedDate(DataTable.VoucherDate));
                    oCmd.Parameters.AddWithValue("@AccEntryid", DataTable.AccEntryId);
                    oCmd.Parameters.AddWithValue("@Accyearcode", DataTable.AccYearCode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    int count = 1;
                    model.BankPaymentGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new BankPaymentModel
                                             {
                                                 SeqNo = count++,
                                                 YearCode = Convert.ToInt32(dr["YearCode"].ToString()),
                                                 VoucherType = dr["VoucherType"].ToString() ?? "",
                                                 VoucherNo = dr["VoucherNo"].ToString() ?? "",
                                                 InVoiceNo = dr["InvoiceNo"].ToString() ?? "",
                                                 VoucherDate = dr["VoucherDate"].ToString() ?? "",
                                                 DueDate = dr["DueDate"].ToString() ?? "",
                                                 Balance = string.IsNullOrEmpty(dr["BalanceAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["BalanceAmt"].ToString()),
                                                 DRCR = dr["DrCrType"].ToString() ?? "",
                                                 AdjustmentAmt = string.IsNullOrEmpty(dr["AdjAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["AdjAmt"].ToString()),
                                                 Adjusted = dr["Adjusted"].ToString() ?? "",
                                                 DrAmt = string.IsNullOrEmpty(dr["DrAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["DrAmt"].ToString()),
                                                 CrAmt = string.IsNullOrEmpty(dr["CrAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["CrAmt"].ToString()),
                                                 PendBillAmt = string.IsNullOrEmpty(dr["BillNetAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["BillNetAmt"].ToString()),
                                                 NewrefNo = dr["NewrefNo"].ToString() ?? "",
                                                 ModeOfAdjustment = dr["ModOfAdjust"].ToString() ?? "",
                                                 AccEntryId = string.IsNullOrEmpty(dr["AccEntryid"].ToString()) ? 0 : Convert.ToInt32(dr["AccEntryid"].ToString()),
                                                 ActualDrCr = dr["ActualDRCRType"].ToString() ?? ""
                                             }).ToList();
                }
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

            return model;
        }
        public async Task<BankPaymentModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            var model = new BankPaymentModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@VoucherType", "Bank-Payment"));
                SqlParams.Add(new SqlParameter("@BooktrnsEntryId", ID));
                SqlParams.Add(new SqlParameter("@yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@voucherNo", VoucherNo));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpVoucherEntry", SqlParams);

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
        private static BankPaymentModel PrepareView(DataSet DS, ref BankPaymentModel? model)
        {
            var ItemList = new List<BankPaymentModel>();
            DS.Tables[0].TableName = "SSMain";
            DS.Tables[1].TableName = "SSDetail";
            int cnt = 1;
            model.AccEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["AccEntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccYearCode"].ToString());
           
            if (DateTime.TryParse(DS.Tables[0].Rows[0]["EntryDate"].ToString(), out DateTime EntryDate))
                model.EntryDate = EntryDate.ToString("dd/MM/yyyy");
            if (DateTime.TryParse(DS.Tables[0].Rows[0]["VoucherDocdate"].ToString(), out DateTime VoucherDocdate))
                model.VoucherDate = VoucherDocdate.ToString("dd/MM/yyyy");
            model.SubVoucherName = DS.Tables[0].Rows[0]["SubVoucherName"].ToString();
            model.VoucherNo = DS.Tables[0].Rows[0]["VoucherNo"].ToString();
            model.VoucherDocDate = DS.Tables[0].Rows[0]["VoucherDocdate"].ToString();
            model.Currency = DS.Tables[0].Rows[0]["Currency"].ToString();
            model.CurrencyId = Convert.ToInt32(DS.Tables[0].Rows[0]["CurrencyId"].ToString());
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["uid"].ToString());
            model.ActualEntryby = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            model.ActualEntryBy = DS.Tables[0].Rows[0]["ActualEntryBy"].ToString();
            if (DateTime.TryParse(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString(), out DateTime actualEntryDate))
                model.ActualEntryDate = actualEntryDate.ToString("dd/MM/yyyy");
            model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.DrAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["DrAmt"].ToString());
            model.CrAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["CrAmt"].ToString());

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()))
            {
                model.UpdatedByEmp = DS.Tables[0].Rows[0]["UpdatedByEmp"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdatedDate"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new BankPaymentModel
                    {
                        SrNO = Convert.ToInt32(row["seqno"].ToString()),
                        AccEntryId = Convert.ToInt32(row["AccEntryId"].ToString()),
                        YearCode = Convert.ToInt32(row["AccYearCode"].ToString()),
                        CostCenterId = Convert.ToInt32(row["costcenterid"].ToString()),
                        CostCenterName = row["CostCenterName"].ToString(),
                        VoucherNo = row["VoucherNo"].ToString(),
                        VoucherDocDate = row["VoucherDocdate"].ToString(),
                        SubVoucherName = row["SubVoucherName"].ToString(),
                        Currency = row["Currency"].ToString(),
                        CurrentValue = Convert.ToDecimal(row["CurrentValue"].ToString()),
                        LedgerName = row["LedgerName"].ToString(),
                        AccountCode = Convert.ToInt32(row["Accountcode"].ToString()),
                        VoucherType = row["Vouchertype"].ToString(),
                        VoucherDocNo = row["VoucherDocNo"].ToString(),
                        EntryByMachine = row["EntryByMachine"].ToString(),
                        BillVouchNo = row["BillVouchNo"].ToString(),
                        DrAmt = Convert.ToDecimal(row["drAmt"].ToString()),
                        CrAmt = Convert.ToDecimal(row["CrAmt"].ToString()),
                        AdjustmentAmt = Convert.ToDecimal(row["AmountInOtherCurr"].ToString()),
                        AdjustmentAmtOthCur = Convert.ToDouble(row["AmountInOtherCurr"].ToString()),
                        ChequeDate = row["chequeDate"].ToString(),
                        ModeOfAdjustment = row["ModeOfAdjustment"].ToString(),
                        AgainstVoucherEntryId = Convert.ToInt32(row["AgainstVoucherEntryId"].ToString()),
                        AgainstVoucherRefNo = row["againstVoucherRefNo"].ToString(),
                        AgainstVoucherType = row["AgainstVoucherType"].ToString(),
                        AgainstVoucherNo = row["AgainstVoucherNo"].ToString(),
                        AgainstVoucheryearCode = Convert.ToInt32(row["AgainstVoucheryearcode"].ToString()),
                        ChequeClearDate = row["chequeClearDate"].ToString(),
                        ChequePrintAC = row["ChequePrintAC"].ToString(),
                        InsNo = row["instrumentno"].ToString(),
                        Intrument = row["instrument"].ToString(),
                        InsDate = DateTime.TryParse(row["instrumentdate"].ToString(), out DateTime insDate) ? insDate.ToString("dd/MM/yyyy") : row["instrumentdate"].ToString(),
                        PONo = row["PONo"].ToString(),
                        PoDate = DateTime.TryParse(row["PoDate"].ToString(), out DateTime poDate) ? poDate.ToString("dd/MM/yyyy") : row["PoDate"].ToString(),
                        POYear = Convert.ToInt32(row["POYear"].ToString()),
                        SONo = Convert.ToInt32(row["SONo"].ToString()),
                        SOYear = Convert.ToInt32(row["SOYear"].ToString()),
                        CustOrderNo = row["CustOrderNo"].ToString(),
                        NameOnCheque = row["NameOnCheque"].ToString(),
                        AccountNarration = row["AccountNarration"].ToString(),
                        Description = row["Description"].ToString(),
                        VoucherRemark = row["VoucherRemark"].ToString(),
                        ActualEntryby = Convert.ToInt32(row["ActualEntryBy"].ToString()),
                        BankType = row["UnderGroup"].ToString(),
                        DRCR = row["DRCRTYPE"].ToString(),
                        Balance =  Convert.ToDecimal(row["BalanceAmt"].ToString()),
                        Type = row["CRDDR"].ToString(),

                    });
                }
                model.BankPaymentGrid = ItemList.OrderBy(x => x.SrNO).ToList();
            }

            return model;
        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ChkBillAmtVsAgainstrefAmt"));
                SqlParams.Add(new SqlParameter("@VoucherDate", VoucherDate));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@AgainstVoucherYearcode", AgainstVoucherYearCode));
                SqlParams.Add(new SqlParameter("@AgainstVoucherEntryid", AgainstVoucherEntryId));
                SqlParams.Add(new SqlParameter("@AgainstVoucherNo", AgainstVoucherNo));
                SqlParams.Add(new SqlParameter("@Accountcode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
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

