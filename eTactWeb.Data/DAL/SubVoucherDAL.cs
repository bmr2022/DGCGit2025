using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
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
    public class SubVoucherDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public SubVoucherDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }

        public async Task<ResponseResult> GetMainVoucherNames()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GETMainVoucherName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetEmployeeList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "EmpNameWithCode"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GetDropDownList", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetPriceFrom()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetPriceFrom"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTableName(string MainVoucherName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetTableName"));
                SqlParams.Add(new SqlParameter("@MainVoucherName", MainVoucherName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        //public async Task<int> GetPrefixEntryIdByVoucherName(string mainVoucherName)
        //{
        //    int prefixEntryId = 0;
        //    using (var connection = new SqlConnection(DBConnectionString))
        //    {
        //        var command = new SqlCommand("AccSpSubVoucherPrefixSetting", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@Flag", "DASHBOARD");
        //        command.Parameters.AddWithValue("@MainVoucherName", mainVoucherName);

        //        connection.Open();
        //        using (var reader = command.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                prefixEntryId = Convert.ToInt32(reader["PrefixEntryId"]);
        //            }
        //        }
        //    }
        //    return prefixEntryId;
        //}
        //public async Task<ResponseResult> SaveSubVoucher(SubVoucherModel model)
        //{
        //    var _ResponseResult = new ResponseResult();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();
        //        //DateTime entDt = ParseDate(model.ActualEntryDate);

        //        DateTime? entDt = string.IsNullOrEmpty(model.ActualEntryDate)
        //                 ? (DateTime?)null
        //                 : ParseDate(model.ActualEntryDate);

        //        if (model.Mode == "U" || model.Mode == "V")
        //        {
        //            SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
        //            SqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedBy));
        //            SqlParams.Add(new SqlParameter("@Updationdate", DateTime.Today));
        //        }
        //        else
        //        {
        //            SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
        //        }

        //       // SqlParams.Add(new SqlParameter("@PrefixEntryId", model.PrefixEntryId == 0 ? 0 : model.PrefixEntryId));
        //        SqlParams.Add(new SqlParameter("@MainVoucherName", model.MainVoucherName == null ? "" : model.MainVoucherName));
        //        SqlParams.Add(new SqlParameter("@MainVoucherTableName", model.MainVoucherTableName == null ? "" : model.MainVoucherTableName));
        //        SqlParams.Add(new SqlParameter("@SubVoucherName ", model.SubVoucherName == null ? "" : model.SubVoucherName));
        //        SqlParams.Add(new SqlParameter("@VoucherRotationType", model.VoucherRotationType == null ? "" : model.VoucherRotationType));
        //        SqlParams.Add(new SqlParameter("@StartSubVouchDiffSeries", model.StartSubVouchDiffSeries == null ? "" : model.StartSubVouchDiffSeries));
        //        SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy == 0 ? 0 : model.ActualEntryBy));
        //        SqlParams.Add(new SqlParameter("@ActualEntryDate", entDt.HasValue ? (object)entDt.Value : DBNull.Value));
        //        SqlParams.Add(new SqlParameter("@SubVouchPrefix", model.SubVouchPrefix == null ? "" : model.SubVoucherName));
        //        SqlParams.Add(new SqlParameter("@FromYearPrefix", model.FromYearPrefix == null ? "" : model.FromYearPrefix));
        //        SqlParams.Add(new SqlParameter("@ToYearPreFix", model.ToYearPreFix == null ? "" : model.ToYearPreFix));
        //        SqlParams.Add(new SqlParameter("@MonthPrefix", model.MonthPrefix == null ? "" : model.MonthPrefix));
        //        SqlParams.Add(new SqlParameter("@DayPrefix", model.DayPrefix == null ? "" : model.DayPrefix));
        //        SqlParams.Add(new SqlParameter("@SeparatorApplicable", model.SeparatorApplicable == null ? "" : model.SeparatorApplicable));
        //        SqlParams.Add(new SqlParameter("@Separator", model.Separator == null ? "" : model.Separator));
        //        SqlParams.Add(new SqlParameter("@TotalLength", model.TotalLength == 0 ? 0 : model.TotalLength));
        //        SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine == null ? "" : model.EntryByMachine));
        //        var prefixEntryIdParam = new SqlParameter("@PrefixEntryId", SqlDbType.Int)
        //        {
        //            Direction = ParameterDirection.Output
        //        };
        //        SqlParams.Add(prefixEntryIdParam);

        //        _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);

        //        // Check the status
        //        if (_ResponseResult.StatusCode != HttpStatusCode.OK)
        //        {
        //            _ResponseResult.StatusText = "Error";
        //            return _ResponseResult;
        //        }

        //        // Get the PrefixEntryId value from the output parameter
        //        int prefixEntryId = (int)prefixEntryIdParam.Value;
        //        _ResponseResult.Result = new { PrefixEntryId = prefixEntryId };

        //        _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);


        //    }
        //    catch (Exception ex)
        //    {
        //        _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
        //        _ResponseResult.StatusText = "Error";
        //        _ResponseResult.Result = new { ex.Message, ex.StackTrace };
        //    }
        //    return _ResponseResult;
        //}
        public async Task<SubVoucherModel> GetViewByID(int PrefixEntryId,string MainVoucherName, string MainVoucherTableName)
        {
            var model = new SubVoucherModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@MainVoucherName", MainVoucherName));
                SqlParams.Add(new SqlParameter("@MainVoucherTableName", MainVoucherTableName));
                SqlParams.Add(new SqlParameter("@PrefixEntryId", PrefixEntryId));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSubVoucherPrefixSetting", SqlParams);

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
        private static SubVoucherModel PrepareView(DataSet DS, ref SubVoucherModel? model)
        {
            try
            {
                var ItemList = new List<SubVoucherModel>();
                DS.Tables[0].TableName = "SSMain";
                int cnt = 0;
                model.PrefixEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["PrefixEntryId"].ToString());
                model.MainVoucherName =DS.Tables[0].Rows[0]["MainVoucherName"].ToString();
                model.MainVoucherTableName = DS.Tables[0].Rows[0]["MainVoucherTableName"].ToString();
                model.SubVoucherName = DS.Tables[0].Rows[0]["SubVoucherName"].ToString();
                model.VoucherRotationType = DS.Tables[0].Rows[0]["VoucherRotationType"].ToString();
                model.StartSubVouchDiffSeries = DS.Tables[0].Rows[0]["StartSubVouchDiffSeries"].ToString();
                model.SubVouchPrefix = DS.Tables[0].Rows[0]["SubVouchPrefix"].ToString();
                model.FromYearPrefix = DS.Tables[0].Rows[0]["FromYearPrefix"].ToString();
                model.ToYearPreFix = DS.Tables[0].Rows[0]["ToYearPreFix"].ToString();
                model.MonthPrefix = DS.Tables[0].Rows[0]["MonthPrefix"].ToString();
                model.DayPrefix = DS.Tables[0].Rows[0]["DayPrefix"].ToString();
                model.SeparatorApplicable = DS.Tables[0].Rows[0]["SeparatorApplicable"].ToString();
                model.Separator = DS.Tables[0].Rows[0]["Separator"].ToString();
                model.TotalLength = Convert.ToInt32(DS.Tables[0].Rows[0]["TotalLength"].ToString());
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                model.GetPriceFrom = DS.Tables[0].Rows[0]["GetPriceFrom"].ToString();
                model.SelectedEmployeeIds = DS.Tables[0].Rows[0]["UserRightsList"].ToString();
                model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
                //model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"].ToString());
                //model.UpdationDate = DS.Tables[0].Rows[0]["UpdationDate"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                //model.PreviousAmount = Convert.ToInt32(DS.Tables[0].Rows[0]["PreviousAmount"].ToString());

                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new SubVoucherModel
                        {
                            //SrNO = Convert.ToInt32(row["SrNO"].ToString()),
                            PrefixEntryId = Convert.ToInt32(row["PrefixEntryId"].ToString()),
                            MainVoucherName = row["MainVoucherName"].ToString(),
                            MainVoucherTableName = row["MainVoucherTableName"].ToString(),
                            SubVoucherName = row["SubVoucherName"].ToString(),
                            VoucherRotationType = row["VoucherRotationType"].ToString(),
                            StartSubVouchDiffSeries = row["StartSubVouchDiffSeries"].ToString(),
                            SubVouchPrefix = row["SubVouchPrefix"].ToString(),
                            FromYearPrefix = row["FromYearPrefix"].ToString(),
                            ToYearPreFix = row["ToYearPreFix"].ToString(),
                            MonthPrefix = row["MonthPrefix"].ToString(),
                            DayPrefix = row["DayPrefix"].ToString(),
                            SeparatorApplicable = row["SeparatorApplicable"].ToString(),
                            Separator = row["Separator"].ToString(),
                            TotalLength = Convert.ToInt32(row["TotalLength"].ToString()),
                            ActualEntryBy = Convert.ToInt32(row["ActualEntryBy"].ToString()),
                            ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString(),
                            //UpdatedBy = Convert.ToInt32(row["UpdatedBy"].ToString()),
                            //UpdationDate = DS.Tables[0].Rows[0]["UpdationDate"].ToString(),
                            EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString(),
                        });
                    }
                    model.SubVoucherGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> SaveSubVoucher(SubVoucherModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime? entDt = string.IsNullOrEmpty(model.ActualEntryDate)
                         ? (DateTime?)null
                         : ParseDate(model.ActualEntryDate);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@Updationdate", DateTime.Today));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@PrefixEntryId", model.PrefixEntryId == 0 ? 0 : model.PrefixEntryId));
                SqlParams.Add(new SqlParameter("@MainVoucherName", model.MainVoucherName ?? ""));
                SqlParams.Add(new SqlParameter("@MainVoucherTableName", model.MainVoucherTableName ?? ""));
                SqlParams.Add(new SqlParameter("@SubVoucherName", model.SubVoucherName ?? ""));
                SqlParams.Add(new SqlParameter("@VoucherRotationType", model.VoucherRotationType ?? ""));
                SqlParams.Add(new SqlParameter("@StartSubVouchDiffSeries", model.StartSubVouchDiffSeries ?? ""));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy == 0 ? 0 : model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entDt.HasValue ? (object)entDt.Value : DBNull.Value));
                SqlParams.Add(new SqlParameter("@SubVouchPrefix", model.SubVouchPrefix ?? ""));
                SqlParams.Add(new SqlParameter("@FromYearPrefix", model.FromYearPrefix ?? ""));
                SqlParams.Add(new SqlParameter("@ToYearPreFix", model.ToYearPreFix ?? ""));
                SqlParams.Add(new SqlParameter("@MonthPrefix", model.MonthPrefix ?? ""));
                SqlParams.Add(new SqlParameter("@DayPrefix", model.DayPrefix ?? ""));
                SqlParams.Add(new SqlParameter("@SeparatorApplicable", model.SeparatorApplicable ?? ""));
                SqlParams.Add(new SqlParameter("@Separator", model.Separator ?? ""));
                SqlParams.Add(new SqlParameter("@TotalLength", model.TotalLength == 0 ? 0 : model.TotalLength));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? ""));
                SqlParams.Add(new SqlParameter("@UserRightsList", model.SelectedEmployeeIds ?? ""));
                SqlParams.Add(new SqlParameter("@GetPriceFrom", model.GetPriceFrom ?? ""));
                SqlParams.Add(new SqlParameter("@VoucherInvoice", "Voucher"));

               
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);

               
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }
            return _ResponseResult;
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

                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSubVoucherPrefixSetting", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<SubVoucherDashBoardGridModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new SubVoucherDashBoardGridModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpSubVoucherPrefixSetting", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.SubVoucherDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                             select new SubVoucherDashBoardGridModel
                                                             {
                                                                 PrefixEntryId = Convert.ToInt32(dr["PrefixEntryId"]),
                                                                 MainVoucherName = dr["MainVoucherName"].ToString(),
                                                                 MainVoucherTableName = dr["MainVoucherTableName"].ToString(),
                                                                 SubVoucherName = dr["SubVoucherName"].ToString(),
                                                                 VoucherRotationType = dr["VoucherRotationType"].ToString(),
                                                                 StartSubVouchDiffSeries = dr["StartSubVouchDiffSeries"].ToString(),
                                                                 SubVouchPrefix = dr["SubVouchPrefix"].ToString(),
                                                                 FromYearPrefix = dr["FromYearPrefix"].ToString(),
                                                                 ToYearPreFix = dr["ToYearPreFix"].ToString(),
                                                                 MonthPrefix = dr["MonthPrefix"].ToString(),
                                                                 DayPrefix = dr["DayPrefix"].ToString(),
                                                                 SeparatorApplicable = dr["SeparatorApplicable"].ToString(),
                                                                 Separator = dr["Separator"].ToString(),
                                                                 ActualEntryBy = dr["ActualEntryBy"].ToString(),
                                                                 //EntryByEmpName = dr["ActualEntryBy"].ToString(),
                                                                 TotalLength = Convert.ToInt32(dr["TotalLength"]),
                                                                 ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                                 UpdatedBy = dr["UpdatedBy"].ToString(),
                                                                 UpdationDate = dr["UpdationDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["UpdationDate"]) : null,
                                                                 EntryByMachine = dr["EntryByMachine"].ToString(),

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
        public async Task<ResponseResult> UpdateSubVoucherPrefixSetting(SubVoucherDashBoardModel model)
        {
            var response = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "UPDATE"),
            new SqlParameter("@MainVoucherName", model.MainVoucherName),
            new SqlParameter("@MainVoucherTableName", model.MainVoucherTableName),
            new SqlParameter("@SubVoucherName", model.SubVoucherName),
            new SqlParameter("@VoucherRotationType", model.VoucherRotationType),
            new SqlParameter("@StartSubVouchDiffSeries", model.StartSubVouchDiffSeries),
            new SqlParameter("@SubVouchPrefix", model.SubVouchPrefix),
            new SqlParameter("@FromYearPrefix", model.FromYearPrefix),
            new SqlParameter("@ToYearPrefix", model.ToYearPreFix),
            new SqlParameter("@MonthPrefix", model.MonthPrefix),
            new SqlParameter("@DayPrefix", model.DayPrefix),
            new SqlParameter("@SeparatorApplicable", model.SeparatorApplicable),
            new SqlParameter("@Separator", model.Separator),
            new SqlParameter("@TotalLength", model.TotalLength),
            new SqlParameter("@ActualEntryBy", model.ActualEntryBy),
            new SqlParameter("@ActualEntryDate", model.ActualEntryDate),
            new SqlParameter("@UpdatedBy", model.UpdatedBy),
            new SqlParameter("@UpdationDate", model.UpdationDate),
            new SqlParameter("@EntryByMachine", model.EntryByMachine),
            new SqlParameter("@PrefixEntryId", model.PrefixEntryId)
        };

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSubVoucherPrefixSetting", SqlParams);
                if (_ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    response.StatusText = "Success";
                }
                else
                {
                    response.StatusText = "Unsuccess";
                }
            }
            catch (Exception ex)
            {
                response.StatusText = "Error: " + ex.Message;
            }

            return response;
        }
        public async Task<ResponseResult> DeleteByID(string MainVoucherName, string MainVoucherTableName,string StartSubVouchDiffSeries,int ActualEntryBy,int UpdatedBy,int PrefixEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@MainVoucherName", MainVoucherName));
                SqlParams.Add(new SqlParameter("@MainVoucherTableName", MainVoucherTableName));
                SqlParams.Add(new SqlParameter("@StartSubVouchDiffSeries", StartSubVouchDiffSeries));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", ActualEntryBy));
                SqlParams.Add(new SqlParameter("@UpdatedBy", UpdatedBy));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSubVoucherPrefixSetting", SqlParams);
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
    }
}
