using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PartCodePartyWiseDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        //private readonly IConfiguration configuration;
        private readonly DataSet oDataSet = new();

        private readonly DataTable oDataTable = new();
        private dynamic? _ResponseResult;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;  

        public PartCodePartyWiseDAL(IConfiguration configuration, IDataLogic dataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLITEMS"));
                SqlParams.Add(new SqlParameter("@ShowAll", "Y"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PartCodePartyWise", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(string Type, string ShowAllItem)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPARTCODE"));
                SqlParams.Add(new SqlParameter("@ShowAll", "Y"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PartCodePartyWise", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillAccountName(string Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLAccount"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PartCodePartyWise", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<PartCodePartyWiseModel> GetListForUpdate(int ItemCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new PartCodePartyWiseModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_PartCodePartyWise", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "GetListForUpdate");
                    oCmd.Parameters.AddWithValue("@itemcode", ItemCode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    int count = 1;
                    model.ItemDetailGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                            select new PartCodePartyWiseItemDetail
                                            {
                                                AccountName = dr["VendCustName"].ToString(),
                                                VendCustPartCode =dr["VendCustPartCode"].ToString(),
                                                VendCustitemname =dr["VendCustitemname"].ToString(),
                                                ItemRate = Convert.ToDecimal(dr["ItemRate"]),
                                                BusinessPercentage = Convert.ToDecimal(dr["BusinessPercentage"]),
                                                MOQ = Convert.ToInt32(dr["moq"]),
                                                LeadTimeInDays = Convert.ToInt32(dr["LeadTimeInDays"]),
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
        public async Task<ResponseResult> DeleteByID(int ItemCode, string EntryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PartCodePartyWise", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<PartCodePartyWiseModel> GetViewByID(int ItemCode)
        {
            var model = new PartCodePartyWiseModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@itemcode", ItemCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PartCodePartyWise", SqlParams);

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
        private static PartCodePartyWiseModel PrepareView(DataSet DS, ref PartCodePartyWiseModel? model)
        {
            var PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
            DS.Tables[0].TableName = "PartCodePartyWise";
            int cnt = 0;
            model.ItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCode"].ToString());
            model.ActualEnteredBy=Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.EntryByMachineName=DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();
            model.ActualEnteredByName=DS.Tables[0].Rows[0]["ActualEntryByEmployeeName"].ToString();
            model.PartCode=DS.Tables[0].Rows[0]["PartCode"].ToString();
            model.ItemName=DS.Tables[0].Rows[0]["ItemName"].ToString();
            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    PartCodePartyWiseItemDetail.Add(new PartCodePartyWiseItemDetail
                    {
                        AccountName = row["VendCustName"].ToString(),
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        VendCustPartCode=row["VendCustPartCode"].ToString(),
                        VendCustitemname = row["VendCustitemname"].ToString(),
                        ItemRate = Convert.ToDecimal(row["ItemRate"].ToString()),
                        BusinessPercentage = Convert.ToDecimal(row["BusinessPercentage"].ToString()),
                        MOQ =Convert.ToDecimal(row["moq"].ToString()),
                        LeadTimeInDays = Convert.ToInt32(row["LeadTimeInDays"].ToString()),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        AccountCode = Convert.ToInt32(row["VendCustAccountCode"].ToString()),

                    });
                }
                model.ItemDetailGrid = PartCodePartyWiseItemDetail.OrderBy(x => x.SeqNo).ToList();
            }
            return model;
        }
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Bom"));

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
        public async Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                SqlParams.Add(new SqlParameter("@FGPartCode", FGPartCode));
                SqlParams.Add(new SqlParameter("@RMPartCode", RMPartCode));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Bom", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<DataTable> CheckDupeConstraint()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "CheckDupeConstraint");
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
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
                oDataTable.Dispose();
            }
            return oDataTable;
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

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PartCodePartyWise", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<PartCodePartyWiseDashboard> GetDashboardData(string ItemName, string PartCode, string AccountName, string CustvendPartCode, string CustvendItemName, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new PartCodePartyWiseDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_PartCodePartyWise", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@VendCustName", AccountName);
                    oCmd.Parameters.AddWithValue("@VendCustPartCode", CustvendPartCode);
                    oCmd.Parameters.AddWithValue("@VendCustItemName", CustvendItemName);
                    oCmd.Parameters.AddWithValue("@SubMenu", DashboardType);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.PartCodePartyDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                    select new PartCodePartyWiseDashboard
                                                    {
                                                        PartCode = dr["PartCode"].ToString(),
                                                        ItemName = dr["ItemName"].ToString(),
                                                        ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                                        AccountName = dr["VendCustName"].ToString(),
                                                        VendCustPartCode =dr["VendCustPartCode"].ToString(),
                                                        VendCustitemname =dr["VendCustitemname"].ToString(),
                                                        ItemRate = Convert.ToDecimal(dr["ItemRate"]),
                                                        BusinessPercentage = Convert.ToDecimal(dr["BusinessPercentage"]),
                                                        MOQ = Convert.ToInt32(dr["moq"]),
                                                        LeadTimeInDays = Convert.ToInt32(dr["LeadTimeInDays"]),
                                                        ActualEnteredByName =dr["ActualEntryByEmployeeName"].ToString(),
                                                        EntryByMachineName =dr["EntryByMachineName"].ToString(),
                                                    }).OrderBy(a => a.SeqNo).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }

        public string GetUnit(int Itemcode, string Flag)
        {
            object _Unit = "";
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", Itemcode);
                        myConnection.Open();
                        _Unit = oCmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _Unit.ToString();
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
        public async Task<ResponseResult> SavePartCodePartWise(PartCodePartyWiseModel model, DataTable DT)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                SqlParams.Add(new SqlParameter("@dtgrid", DT));
                SqlParams.Add(new SqlParameter("@Itemcode", model.ItemCode));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PartCodePartyWise", SqlParams);
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