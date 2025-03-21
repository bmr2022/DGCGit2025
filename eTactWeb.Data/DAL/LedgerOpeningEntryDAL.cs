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
    public class LedgerOpeningEntryDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        //private readonly IConfiguration configuration;

        public LedgerOpeningEntryDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetGroupByAccountCode(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetLedgerGroup"));
                SqlParams.Add(new SqlParameter("@AccountCode",  AccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetAllGroupName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGroup"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetAmountAndType (int AccountCode, int OpeningForYear, string ActualEntryDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetLedgerOpening"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ParseFormattedDate(ActualEntryDate)));
                SqlParams.Add(new SqlParameter("@CloseYearcode", OpeningForYear));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetLedgersByGroup(string groupAccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillLedger"));
                SqlParams.Add(new SqlParameter("@GroupAccountCode", groupAccountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetLedgersALLGroup()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillLedger"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
       
        public async Task<ResponseResult> SaveWorkOrderProcess(LedgerOpeningEntryModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime entDt = DateTime.Now;

                //DateTime? entDt = string.IsNullOrEmpty(model.ActualEntryDate)
                //         ? (DateTime?)null
                //         : ParseDate(model.ActualEntryDate);

                if (model.Mode == "U" || model.Mode == "V" && model.AccountCode != 0)
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
                    SqlParams.Add(new SqlParameter("@Updationdate", DateTime.Today));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                    SqlParams.Add(new SqlParameter("@GroupAccountCode", model.GroupAccountCode));
                    //SqlParams.Add(new SqlParameter("@DrCr", string.IsNullOrEmpty(model.DrCr) ? "" : model.DrCr));
                    //SqlParams.Add(new SqlParameter("@Amount", model.Amount == 0 ? 0 : model.Amount));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode == 0 ? 0 : model.AccountCode));
                    SqlParams.Add(new SqlParameter("@GroupAccountCode", model.GroupAccountCode == 0 ? 0 : model.GroupAccountCode));
                    //SqlParams.Add(new SqlParameter("@GroupAccountCode", model.GroupCode == 0 ? 0 : model.GroupCode));
                }

                SqlParams.Add(new SqlParameter("@CloseYearcode", model.ClosingYearCode == 0 ? 0 : model.ClosingYearCode));
                SqlParams.Add(new SqlParameter("@EntryByMachine", string.IsNullOrEmpty(model.EntryByMachine) ? "" : model.EntryByMachine));
                SqlParams.Add(new SqlParameter("@Amount", model.PreviousAmount == 0 ? 0 : model.PreviousAmount));
                SqlParams.Add(new SqlParameter("@DrCr", string.IsNullOrEmpty(model.DrCr) ? "" : model.DrCr));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", (object)entDt));
                SqlParams.Add(new SqlParameter("@PreviousAmount", model.PreviousAmount == 0 ? 0 : model.PreviousAmount));
                SqlParams.Add(new SqlParameter("@EntryByEmpId", model.EntryByEmpId == 0 ? 0 : model.EntryByEmpId));
               
                SqlParams.Add(new SqlParameter("@CC", model.CC == null ? "" : model.CC));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);

            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
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
        public async Task<LedgerOpeningEntryModel> GetViewByID(int AccountCode)
        {
            var model = new LedgerOpeningEntryModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPLedgerOpeningEntry", SqlParams);

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
        private static LedgerOpeningEntryModel PrepareView(DataSet DS, ref LedgerOpeningEntryModel? model)
        {
            try
            {
                var ItemList = new List<LedgerOpeningEntryModel>();
                DS.Tables[0].TableName = "SSMain";
                int cnt = 0;
                model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
                model.GroupAccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ParentAccountCode"].ToString());
                model.ClosingYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ClosingYearCode"].ToString());
                model.DrCr = DS.Tables[0].Rows[0]["DrCr"].ToString();
                model.Amount = Convert.ToInt32(DS.Tables[0].Rows[0]["Amount"].ToString());
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.EntryByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryByEmpId"].ToString());
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                //model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]) : (DateTime?)null;
                model.UpdatedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedByEmpId"].ToString());
                model.Updationdate = DS.Tables[0].Rows[0]["Updationdate"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                //model.PreviousAmount = Convert.ToInt32(DS.Tables[0].Rows[0]["PreviousAmount"].ToString());

                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new LedgerOpeningEntryModel
                        {
                            //SrNO = Convert.ToInt32(row["SrNO"].ToString()),
                            AccountCode = Convert.ToInt32(row["AccountCode"].ToString()),
                            GroupAccountCode = Convert.ToInt32(row["ParentAccountCode"].ToString()),
                            ClosingYearCode = Convert.ToInt32(row["ClosingYearCode"].ToString()),
                            DrCr = DS.Tables[0].Rows[0]["DrCr"].ToString(),
                            Amount = Convert.ToInt32(row["Amount"].ToString()),
                            CC = DS.Tables[0].Rows[0]["CC"].ToString(),
                            EntryByEmpId = Convert.ToInt32(row["EntryByEmpId"].ToString()),
                             ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString(),
                            //ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]): (DateTime?)null,
                            UpdatedByEmpId = Convert.ToInt32(row["UpdatedByEmpId"].ToString()),
                            Updationdate = DS.Tables[0].Rows[0]["Updationdate"].ToString(),
                            EntryByMachine = row["EntryByMachine"].ToString(),
                            //PreviousAmount = Convert.ToInt32(row["PreviousAmount"].ToString()),
                        });
                    }
                    model.LedgerOpeningEntryGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));

                _ResponseResult = await _IDataLogic .ExecuteDataSet("AccSPLedgerOpeningEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<LedgerOpeningEntryDashBoardGridModel> GetDashboardDetailData(string GroupName,string LedgerName,float PreviousAmount, string DrCr)
        {
            DataSet? oDataSet = new DataSet();
            var model = new LedgerOpeningEntryDashBoardGridModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSPLedgerOpeningEntry", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@GroupName", string.IsNullOrEmpty(GroupName) ? DBNull.Value : GroupName);
                    oCmd.Parameters.AddWithValue("@LedgerName", string.IsNullOrEmpty(LedgerName) ? DBNull.Value : LedgerName);

                    oCmd.Parameters.AddWithValue("@Amount", PreviousAmount);
                    oCmd.Parameters.AddWithValue("@DrCr", DrCr);
                    
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.LedgerOpeningEntryDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new LedgerOpeningEntryDashBoardGridModel
                                                       {
                                                           GroupName = dr.IsNull("GroupName") ? string.Empty : dr["GroupName"].ToString(),
                                                           LedgerName = dr.IsNull("LedgerName") ? string.Empty : dr["LedgerName"].ToString(),
                                                           ClosingYearCode = dr.IsNull("ClosingYearCode") ? 0 : Convert.ToInt32(dr["ClosingYearCode"]),
                                                           AccountCode = dr.IsNull("AccountCode") ? 0 : Convert.ToInt32(dr["AccountCode"]),
                                                           DrCr = dr.IsNull("DrCr") ? string.Empty : dr["DrCr"].ToString(),
                                                           Amount = dr.IsNull("Amount") ? 0 : Convert.ToInt32(dr["Amount"]),
                                                           CC = dr.IsNull("CC") ? string.Empty : dr["CC"].ToString(),
                                                           EntryByEmployee = dr.IsNull("EntryByEmployee") ? string.Empty : dr["EntryByEmployee"].ToString(),
                                                           ActualEntryDate = dr.IsNull("ActualEntryDate") ? string.Empty : Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd-MM-yyyy"),
                                                           GroupAccountCode = dr.IsNull("GroupAccountCode") ? 0 : Convert.ToInt32(dr["GroupAccountCode"]),
                                                           UpdatedByEmployee = dr.IsNull("UpdatedByEmployee") ? string.Empty : dr["UpdatedByEmployee"].ToString(),
                                                           Updationdate = dr.IsNull("Updationdate") ? string.Empty : Convert.ToDateTime(dr["Updationdate"]).ToString("dd-MM-yyyy"),
                                                           EntryByMachine = dr.IsNull("EntryByMachine") ? string.Empty : dr["EntryByMachine"].ToString()


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
        public async Task<ResponseResult> DeleteByID(int AC, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@AccountCode", AC));
                SqlParams.Add(new SqlParameter("@CloseYearcode", YC));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPLedgerOpeningEntry", SqlParams);
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
