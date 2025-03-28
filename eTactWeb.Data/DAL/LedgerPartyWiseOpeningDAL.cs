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
    public class LedgerPartyWiseOpeningDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public LedgerPartyWiseOpeningDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<LedgerPartyWiseOpeningModel> GetAllDataAccountCodeWise(int OpeningYearCode, int AccountCode)
        {
            var Result = new ResponseResult();
            var model = new LedgerPartyWiseOpeningModel();
            DataSet DS = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetExistingDataForAccountCode"));
                SqlParams.Add(new SqlParameter("@OpeningYearCode", OpeningYearCode));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

                Result = await _IDataLogic.ExecuteDataSet("AccSPLedgerBillWiseOpening", SqlParams);

                if (Result.Result != null && Result.StatusCode == HttpStatusCode.OK && Result.StatusText == "Success")
                {
                    //PrepareView(Result.Result, ref model);
                    model.EntryId = Convert.ToInt32(Result.Result.Tables[0].Rows[0]["LedgerOpnEntryId"]);
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
        public async Task<ResponseResult> FillLedgerName(int OpeningYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillLedger"));
                SqlParams.Add(new SqlParameter("@OpeningYearCode", OpeningYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillAccountNameForDashBoard()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillAccountNameForDashBoard"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInvoiceForDashBoard()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillInvoiceForDashBoard"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetOpeningAmt(int OpeningYearCode, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetOpeningAmt"));
                SqlParams.Add(new SqlParameter("@OpeningYearCode", OpeningYearCode));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDueDate(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetCreditDays"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveLedgerPartyWiseOpening(LedgerPartyWiseOpeningModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var sqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@LedgerOpnEntryId", model.EntryId));
                    sqlParams.Add(new SqlParameter("@OpeningYearCode", model.OpeningYearCode));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
                    sqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                    sqlParams.Add(new SqlParameter("@LedgerOpnEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@OpeningAmt", model.Balance));
                    sqlParams.Add(new SqlParameter("@LastUpdatedDate", model.UpdationDate));
                    sqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedByEmp));
                    //sqlParams.Add(new SqlParameter("@SeqNo", model.SrNO));
                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    //sqlParams.Add(new SqlParameter("@LedgerOpnEntryId", model.EntryId));
                    sqlParams.Add(new SqlParameter("@OpeningYearCode", model.OpeningYearCode));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
                    sqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                    sqlParams.Add(new SqlParameter("@LedgerOpnEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@OpeningAmt", model.Balance));
                    //sqlParams.Add(new SqlParameter("@SeqNo", model.SrNO));
                    //sqlParams.Add(new SqlParameter("@AccBookTransEntryId", model.AccBookTransEntryId));
                    //sqlParams.Add(new SqlParameter("@OpeningYearCode", model.AccBookTransYearCode));
                    //sqlParams.Add(new SqlParameter("@OpeningAmt", model.Balance));
                    //sqlParams.Add(new SqlParameter("@InvoiceDate", model.EntryDate));
                    //sqlParams.Add(new SqlParameter("@InvoiceNo", model.BillNo)); 
                    //sqlParams.Add(new SqlParameter("@InvoiceDate", model.BillDate));
                    //sqlParams.Add(new SqlParameter("@InvoiceDate", model.BillYear));
                    //sqlParams.Add(new SqlParameter("@InvNetAmt", model.BillNetAmt));
                    //sqlParams.Add(new SqlParameter("@InvPendAmt", model.PendAmt));
                    //sqlParams.Add(new SqlParameter("@DrCrType", model.DrCrType));
                    //sqlParams.Add(new SqlParameter("@TransactionType", model.TransactionType));
                    //sqlParams.Add(new SqlParameter("@DueDate", model.DueDate));
                    //sqlParams.Add(new SqlParameter("@CC", model.CC));
                    //sqlParams.Add(new SqlParameter("@AccountNarration", model.AccountNarration));
                    //sqlParams.Add(new SqlParameter("@Unit", model.Unit));
                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
                }


                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", sqlParams);

            }
            catch (Exception ex)
            {
                // Set error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@Flag", "Dashboard")
                };

                responseResult = await _IDataLogic.ExecuteDataSet("AccSPLedgerBillWiseOpening", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<LedgerPartyWiseOpeningDashBoardModel> GetDashboardDetailData(string LedgerName, string BillNo)
        {
            DataSet? oDataSet = new DataSet();
            var model = new LedgerPartyWiseOpeningDashBoardModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSPLedgerBillWiseOpening", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
                    oCmd.Parameters.AddWithValue("@InvoiceNo", string.IsNullOrEmpty(BillNo) ? DBNull.Value : BillNo);
                    oCmd.Parameters.AddWithValue("@AccountName", string.IsNullOrEmpty(LedgerName) ? DBNull.Value : LedgerName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.LedgerPartyWiseOpeningDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                   select new LedgerPartyWiseOpeningDashBoardModel
                                                                   {
                                                                       EntryId = Convert.ToInt32(dr["LedgerOpnEntryId"]),
                                                                       OpeningYearCode = Convert.ToInt32(dr["LedgerOpnYearCode"]),
                                                                       AccBookTransEntryId = Convert.ToInt32(dr["AccBookTransEntryId"]),
                                                                       AccBookTransYearCode = Convert.ToInt32(dr["AccBookTransYearCode"]),
                                                                       AccountCode = Convert.ToInt32(dr["AccountCode"].ToString()),
                                                                       LedgerName = dr["Account_Name"].ToString(),
                                                                       OpeningAmt = Convert.ToDouble(dr["OpeningAmt"]),
                                                                       BillNo = dr["InvoiceNo"].ToString(),
                                                                       BillDate = dr.IsNull("InvoiceDate") ? string.Empty : Convert.ToDateTime(dr["InvoiceDate"]).ToString("dd-MM-yyyy"),
                                                                       BillNetAmt = Convert.ToDecimal(dr["InvNetAmt"]),
                                                                       PendAmt = Convert.ToDecimal(dr["InvPendAmt"]),
                                                                       Type = dr["DrCrType"].ToString(),
                                                                       TransactionType = dr["TransactionType"].ToString(),
                                                                       DueDate = dr["DueDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["DueDate"]).ToString("dd-MM-yyyy"),
                                                                       CC = dr["CC"].ToString(),
                                                                       ActualEntryBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                                       ActualEntryDate = dr["ActualEntryDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd-MM-yyyy"),
                                                                       UpdatedBy = Convert.ToInt32(dr["UpdatedBy"]),
                                                                       LastUpdatedDate = dr["LastUpdatedDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["LastUpdatedDate"]).ToString("dd-MM-yyyy"),
                                                                       EntryByMachine = dr["EntryByMachine"].ToString(),
                                                                       AccountNarration = dr["AccountNarration"].ToString(),
                                                                       Unit = dr["SaveUpdate"].ToString()
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

		public async Task<ResponseResult> FillEntryId()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}
			return _ResponseResult;
		}
		public async Task<LedgerPartyWiseOpeningModel> GetViewByID(int OpeningYearCode, int LedgerOpnEntryId)
        {
            var model = new LedgerPartyWiseOpeningModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByAccountCode"));
                SqlParams.Add(new SqlParameter("@OpeningYearCode", OpeningYearCode));
                SqlParams.Add(new SqlParameter("@LedgerOpnEntryId", LedgerOpnEntryId));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPLedgerBillWiseOpening", SqlParams);

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
        private static LedgerPartyWiseOpeningModel PrepareView(DataSet DS, ref LedgerPartyWiseOpeningModel? model)
        {
            try
            {
                var ItemList = new List<LedgerPartyWiseOpeningDetailModel>();
                DS.Tables[0].TableName = "AccLedgerBillWiseOpening";
                int cnt = 0;

                //model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["LedgerOpnEntryId"].ToString());
                //model.OpeningYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["LedgerOpnYearCode"].ToString());
                //model.ActualEntryDate = DS.Tables[0].Rows[0]["LedgerOpnEntryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(DS.Tables[0].Rows[0]["LedgerOpnEntryDate"]);
                //model.Balance = Convert.ToInt32(DS.Tables[0].Rows[0]["OpeningAmt"].ToString());
                //model.LedgerName = DS.Tables[0].Rows[0]["Account_Name"].ToString();
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new LedgerPartyWiseOpeningDetailModel
                        {
                            EntryId = DS.Tables[0].Rows[0]["LedgerOpnEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LedgerOpnEntryId"]),
                            BillYear = DS.Tables[0].Rows[0]["LedgerOpnYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LedgerOpnYearCode"]),
                            AccBookTransEntryId = DS.Tables[0].Rows[0]["AccBookTransEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["AccBookTransEntryId"]),
                            AccBookTransYearCode = DS.Tables[0].Rows[0]["AccBookTransYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["AccBookTransYearCode"]),
                            AccountCode = DS.Tables[0].Rows[0]["AccountCode"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]),
                            LedgerName = DS.Tables[0].Rows[0]["Account_Name"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["Account_Name"].ToString(),
                            OpeningAmt = DS.Tables[0].Rows[0]["OpeningAmt"] == DBNull.Value ? 0.0 : Convert.ToDouble(DS.Tables[0].Rows[0]["OpeningAmt"]),
                            BillNo = DS.Tables[0].Rows[0]["InvoiceNo"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["InvoiceNo"].ToString(),
                            BillDate = DS.Tables[0].Rows[0]["InvoiceDate"] == DBNull.Value ? null : Convert.ToDateTime(DS.Tables[0].Rows[0]["InvoiceDate"]).ToString("dd-MM-yyyy"),
                            BillNetAmt = DS.Tables[0].Rows[0]["InvNetAmt"] == DBNull.Value ? 0.0 : Convert.ToDouble(DS.Tables[0].Rows[0]["InvNetAmt"]),
                            PendAmt = DS.Tables[0].Rows[0]["InvPendAmt"] == DBNull.Value ? 0.0 : Convert.ToDouble(DS.Tables[0].Rows[0]["InvPendAmt"]),
                            Type = DS.Tables[0].Rows[0]["DrCrType"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["DrCrType"].ToString(),
                            TransactionType = DS.Tables[0].Rows[0]["TransactionType"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["TransactionType"].ToString(),
                            DueDate = DS.Tables[0].Rows[0]["DueDate"] == DBNull.Value ? null : Convert.ToDateTime(DS.Tables[0].Rows[0]["DueDate"]).ToString("dd-MM-yyyy"),
                            CC = DS.Tables[0].Rows[0]["CC"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["CC"].ToString(),
                            ActualEntryBy = DS.Tables[0].Rows[0]["ActualEntryBy"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"]),
                            ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"] == DBNull.Value ? null : DS.Tables[0].Rows[0]["ActualEntryDate"].ToString(),
                            UpdatedBy = DS.Tables[0].Rows[0]["UpdatedBy"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]),
                            LastUpdatedDate = DS.Tables[0].Rows[0]["LastUpdatedDate"] == DBNull.Value ? null : DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString(),
                            EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["EntryByMachine"].ToString(),
                            AccountNarration = DS.Tables[0].Rows[0]["AccountNarration"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["AccountNarration"].ToString(),
                            Unit = DS.Tables[0].Rows[0]["SaveUpdate"] == DBNull.Value ? string.Empty : DS.Tables[0].Rows[0]["SaveUpdate"].ToString(),
                            SrNO = DS.Tables[0].Rows[0]["SeqNo"] == DBNull.Value ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SeqNo"])


                        });
                    }
                    model.LedgerPartyWiseOpeningDetails = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> DeleteByID(string EntryByMachine, int OpeningYearCode, int LedgerOpnEntryId, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryByMachine", EntryByMachine));
                SqlParams.Add(new SqlParameter("@OpeningYearCode", OpeningYearCode));
                SqlParams.Add(new SqlParameter("@LedgerOpnEntryId", LedgerOpnEntryId));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerBillWiseOpening", SqlParams);
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
