using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CustomerJWRDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;


        public CustomerJWRDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Customer Jobwork Receive"));
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

        public async Task<ResponseResult> GetGateNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@Flag", "PENDINGGATEFORMRN"));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@Todate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@YearCode", null));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GATEMAINDATA"));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GATEMAINITEM"));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkReceiveMainDetail", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "EmployeeList";

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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<ResponseResult> SaveCustJWR(CustomerJobWorkReceiveModel model, DataTable MRGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime entDt = new DateTime();
                //DateTime gatedate = new DateTime();
                //DateTime challanDt = new DateTime();

               var entDt = CommonFunc.ParseFormattedDate(model.EntryDate);
               var gatedate = CommonFunc.ParseFormattedDate(model.GateDate);
               var challanDt = CommonFunc.ParseFormattedDate(model.ChallanDate);
               var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
               var actentDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));



                if (model.Mode == "U")
                {

                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@updatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@updatedon", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@ChallanDate", challanDt == default ? string.Empty : challanDt));
                SqlParams.Add(new SqlParameter("@GateNo", model.GateNo));
                SqlParams.Add(new SqlParameter("@GateDate", gatedate == default ? string.Empty : gatedate));
                SqlParams.Add(new SqlParameter("@MRNNO", model.MRNNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountID));
                SqlParams.Add(new SqlParameter("@JobworkType", model.JobWorkType));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@RecievedBy", model.ReceivedBy));
                SqlParams.Add(new SqlParameter("@Complete", model.Complete));
                SqlParams.Add(new SqlParameter("@Closed", model.Closed));
                SqlParams.Add(new SqlParameter("@QcCheck", model.QcCheck));
                SqlParams.Add(new SqlParameter("@QcCompleted", model.QcCompleted));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actentDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));
                SqlParams.Add(new SqlParameter("@StoreId", model.RecStoreId));
                SqlParams.Add(new SqlParameter("@RecStoreid", model.RecStoreId));


                SqlParams.Add(new SqlParameter("@DTSSGrid", MRGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string VendorName, string ChallanNO, string PartCode, string ItemName, string MrnNO)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromdt = CommonFunc.ParseFormattedDate(FromDate);
                var todt = CommonFunc.ParseFormattedDate(ToDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt));
                SqlParams.Add(new SqlParameter("@Todate", todt));
                SqlParams.Add(new SqlParameter("@Accountname", VendorName));
                SqlParams.Add(new SqlParameter("@ChallanNo", ChallanNO));
                SqlParams.Add(new SqlParameter("@Partcode", PartCode));
                SqlParams.Add(new SqlParameter("@ItemName", ItemName));
                SqlParams.Add(new SqlParameter("@MRNno", MrnNO));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<CustomerJWRQDashboard> GetSearchData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new CustomerJWRQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_CustomerJobworkReceiveMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@AccountName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MrnNo);
                    oCmd.Parameters.AddWithValue("@ChallanNo", ChallanNo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
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
                    model.CustomerJWRQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new CustomerJWRDashboard
                                                   {
                                                       VendorName = dr["VendorName"].ToString(),
                                                       MrnNo = dr["MrnNo"].ToString(),
                                                       MrnDate = dr["MrnDate"].ToString(),
                                                       GateNo = dr["GateNo"].ToString(),
                                                       GateDate = dr["GateDate"].ToString(),
                                                       ChallanNo = dr["ChallanNo"].ToString(),
                                                       ChallanDate = dr["ChallanDate"].ToString(),
                                                       EntryId = Convert.ToInt32(dr["EntryId"].ToString()),
                                                       YearCode = Convert.ToInt32(dr["YearCode"].ToString()),
                                                       EntryBy = dr["EntryBy"].ToString(),
                                                       ReceByEmp = dr["ReceByEmp"].ToString(),
                                                       LastUpdatedByEmp = dr["LastUpdatedByEmp"].ToString(),
                                                       JobworkType = dr["JobworkType"].ToString(),
                                                       CC = dr["CC"].ToString(),
                                                       UID = dr["UID"].ToString(),
                                                       GateYearCode = Convert.ToInt32(dr["GateYearCode"].ToString()),
                                                       Remark = dr["Remark"].ToString(),
                                                       MRNQCCompleted = dr["MRNQCCompleted"].ToString(),
                                                       Complete = dr["Complete"].ToString(),
                                                       Closed = dr["Closed"].ToString(),
                                                       ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                       UpdatedOn = dr["UpdatedOn"].ToString(),
                                                       EntryByMachineName = dr["EntryByMachineName"].ToString(),
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
        public async Task<CustomerJWRQDashboard> GetSearchDetailData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new CustomerJWRQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_CustomerJobworkReceiveMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo

                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "SEARCHDETAIL");
                    oCmd.Parameters.AddWithValue("@AccountName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MrnNo);
                    oCmd.Parameters.AddWithValue("@ChallanNo", ChallanNo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
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
                    model.CustomerJWRQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new CustomerJWRDashboard
                                                   {
                                                       VendorName = dr["VendorName"].ToString(),
                                                       MrnNo = dr["MrnNo"].ToString(),
                                                       MrnDate = dr["MrnDate"].ToString(),
                                                       GateNo = dr["GateNo"].ToString(),
                                                       GateDate = dr["GateDate"].ToString(),
                                                       ChallanNo = dr["ChallanNo"].ToString(),
                                                       ChallanDate = dr["ChallanDate"].ToString(),
                                                       EntryId = string.IsNullOrEmpty(dr["EntryId"].ToString()) ? 0 : Convert.ToInt32(dr["EntryId"].ToString()),
                                                       YearCode = string.IsNullOrEmpty(dr["YearCode"].ToString()) ? 0 : Convert.ToInt32(dr["YearCode"].ToString()),
                                                       EntryBy = dr["EntryBy"].ToString(),
                                                       ReceByEmp = dr["ReceByEmp"].ToString(),
                                                       LastUpdatedByEmp = dr["LastUpdatedByEmp"].ToString(),
                                                       JobworkType = dr["JobworkType"].ToString(),
                                                       CC = dr["CC"].ToString(),
                                                       UID = dr["UID"].ToString(),
                                                       GateYearCode = string.IsNullOrEmpty(dr["GateYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["GateYearCode"].ToString()),
                                                       Remark = dr["Remark"].ToString(),
                                                       MRNQCCompleted = dr["MRNQCCompleted"].ToString(),
                                                       Complete = dr["Complete"].ToString(),
                                                       Closed = dr["Closed"].ToString(),
                                                       ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                       UpdatedOn = dr["UpdatedOn"].ToString(),
                                                       EntryByMachineName = dr["EntryByMachineName"].ToString(),
                                                       RecPartCode = dr["RecPartCode"].ToString(),
                                                       RecItemName = dr["RecItemName"].ToString(),
                                                       Billqty = string.IsNullOrEmpty(dr["Billqty"].ToString()) ? 0 : Convert.ToDecimal(dr["Billqty"].ToString()),
                                                       RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       Unit = dr["Unit"].ToString(),
                                                       RecAltQty = string.IsNullOrEmpty(dr["RecAltQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecAltQty"].ToString()),
                                                       AltUnit = dr["AltUnit"].ToString(),
                                                       Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                       Amount = string.IsNullOrEmpty(dr["Amount"].ToString()) ? 0 : Convert.ToDecimal(dr["Amount"].ToString()),
                                                       ShortExcessQty = string.IsNullOrEmpty(dr["ShortExcessQty"].ToString()) ? 0 : Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                                       ItemRemark = dr["ItemRemark"].ToString(),
                                                       Purpose = dr["Purpose"].ToString(),
                                                       FinishedQty = string.IsNullOrEmpty(dr["FinishedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["FinishedQty"].ToString()),
                                                       PendQty = string.IsNullOrEmpty(dr["PendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["PendQty"].ToString()),
                                                       RecScrap = dr["RecScrap"].ToString(),
                                                       AllowedRejPer = dr["AllowedRejPer"].ToString(),
                                                       ProcessId = string.IsNullOrEmpty(dr["ProcessId"].ToString()) ? 0 : Convert.ToInt32(dr["ProcessId"].ToString()),
                                                       Color = dr["Color"].ToString(),
                                                       ItemQcCompleted = dr["ItemQcCompleted"].ToString(),
                                                       CustBatchno = dr["CustBatchno"].ToString(),
                                                       batchno = dr["batchno"].ToString(),
                                                       UniqueBatchNo = dr["UniqueBatchNo"].ToString(),
                                                       SoNo = dr["SoNo"].ToString(),
                                                       SoYearCode = string.IsNullOrEmpty(dr["SoYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SoYearCode"].ToString()),
                                                       CustOrderno = dr["CustOrderno"].ToString(),
                                                       SOSchNo = dr["SOSchNo"].ToString(),
                                                       SOSchYearCode = string.IsNullOrEmpty(dr["SOSchYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SOSchYearCode"].ToString()),
                                                       SODate = dr["SODate"].ToString(),
                                                       SchDate = dr["SchDate"].ToString(),
                                                       bomno = dr["bomno"].ToString(),
                                                       BomName = dr["BomName"].ToString(),
                                                       BomDate = dr["BomDate"].ToString(),
                                                       INDBOM = dr["INDBOM"].ToString(),
                                                       FGPartCode = dr["FGPartCode"].ToString(),
                                                       FGItemName = dr["FGItemName"].ToString()

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
        public async Task<CustomerJobWorkReceiveModel> GetViewByID(int ID, int YearCode)
        {
            var model = new CustomerJobWorkReceiveModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_CustomerJobworkReceiveMainDetail", SqlParams);

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
        public async Task<IList<TextValue>> GetEmployeeList()
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_CustomerJobworkReceiveMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "BindEmpData");

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["EmpNameCode"].ToString(),
                                Value = Reader["Emp_Id"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        public async Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
                SqlParams.Add(new SqlParameter("@MRNNo", MRNNo));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        private static CustomerJobWorkReceiveModel PrepareView(DataSet DS, ref CustomerJobWorkReceiveModel? model)
        {


            var ItemList = new List<CustomerJobWorkReceiveDetail>();
            DS.Tables[0].TableName = "CJWRMain";
            DS.Tables[1].TableName = "CJWRDetail";
            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["CustJWRecEntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["CustJWRecYearCode"].ToString());
            model.EntryDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CustJWRecEntryDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["CustJWRecEntryDate"].ToString();
            model.ChallanNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CustJWRecChallanNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["CustJWRecChallanNo"].ToString();
            model.ChallanDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CustJWRecChallanDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["CustJWRecChallanDate"].ToString();
            model.GateNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["GateNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["GateNo"].ToString();
            model.GateDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["GateDate"].ToString()) ? "" : DS.Tables[0].Rows[0]["GateDate"].ToString();
            model.MRNNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["MRNNO"].ToString()) ? "" : DS.Tables[0].Rows[0]["MRNNO"].ToString();
            model.GateYearCode = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["GateYearCode"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"].ToString());
            model.AccountID = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["AccountCode"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
            model.AccountCode = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["AccountName"].ToString()) ? "" : DS.Tables[0].Rows[0]["AccountName"].ToString();
            model.JobWorkType = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["JobworkType"].ToString()) ? "" : DS.Tables[0].Rows[0]["JobworkType"].ToString();
            model.UID = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UID"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.CC = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CC"].ToString()) ? "" : DS.Tables[0].Rows[0]["CC"].ToString();
            model.Remark = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Remark"].ToString()) ? "" : DS.Tables[0].Rows[0]["Remark"].ToString();
            model.ReceivedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["RecievedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["RecievedBy"].ToString());
            model.Complete = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Complete"].ToString()) ? "" : DS.Tables[0].Rows[0]["Complete"].ToString();
            model.Closed = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Closed"].ToString()) ? "" : DS.Tables[0].Rows[0]["Closed"].ToString();
            model.QcCheck = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["QcCheck"].ToString()) ? "" : DS.Tables[0].Rows[0]["QcCheck"].ToString();
            model.QcCompleted = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["QcCompleted"].ToString()) ? "" : DS.Tables[0].Rows[0]["QcCompleted"].ToString();
            model.CreatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);
            model.CreatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.CreatedByName = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CreatedByName"].ToString()) ? "" : DS.Tables[0].Rows[0]["CreatedByName"].ToString();
            model.EntryByMachineName = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["EntryByMachineName"].ToString()) ? "" : DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();
            model.RecStoreName = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["StoreName"].ToString()) ? "" : DS.Tables[0].Rows[0]["StoreName"].ToString();
            model.RecStoreId = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["RecStoreId"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["RecStoreId"].ToString());


            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedByEmp"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmp"].ToString();
                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new CustomerJobWorkReceiveDetail
                    {
                        SeqNo = cnt++,
                        RecitemCode = string.IsNullOrEmpty(row["RecItemCode"].ToString()) ? 0 : Convert.ToInt32(row["RecItemCode"].ToString()),
                        FinishItemCode = string.IsNullOrEmpty(row["FinishedItemCode"].ToString()) ? 0 : Convert.ToInt32(row["FinishedItemCode"].ToString()),
                        SoYearCode = string.IsNullOrEmpty(row["SoYearCode"].ToString()) ? 0 : Convert.ToInt32(row["SoYearCode"].ToString()),
                        ProcessId = string.IsNullOrEmpty(row["ProcessId"].ToString()) ? 0 : Convert.ToInt32(row["ProcessId"].ToString()),//name?
                        BomNo = string.IsNullOrEmpty(row["bomno"].ToString()) ? 0 : Convert.ToInt32(row["bomno"].ToString()),
                        SOSchYearCode = string.IsNullOrEmpty(row["SOSchYearCode"].ToString()) ? 0 : Convert.ToInt32(row["SOSchYearCode"].ToString()),
                        BillQty = string.IsNullOrEmpty(row["Billqty"].ToString()) ? 0 : Convert.ToSingle(row["Billqty"].ToString()),
                        RecQty = string.IsNullOrEmpty(row["RecQty"].ToString()) ? 0 : Convert.ToSingle(row["RecQty"].ToString()),
                        RecAltQty = string.IsNullOrEmpty(row["RecAltQty"].ToString()) ? 0 : Convert.ToSingle(row["RecAltQty"].ToString()),
                        Rate = string.IsNullOrEmpty(row["Rate"].ToString()) ? 0 : Convert.ToSingle(row["Rate"].ToString()),
                        Amount = string.IsNullOrEmpty(row["Amount"].ToString()) ? 0 : Convert.ToSingle(row["Amount"].ToString()),
                        FinsihedQty = string.IsNullOrEmpty(row["FinishedQty"].ToString()) ? 0 : Convert.ToSingle(row["FinishedQty"].ToString()),
                        PendQty = string.IsNullOrEmpty(row["PendQty"].ToString()) ? 0 : Convert.ToSingle(row["PendQty"].ToString()),
                        RecScrap = string.IsNullOrEmpty(row["RecScrap"].ToString()) ? 0 : Convert.ToSingle(row["RecScrap"].ToString()),
                        AllowedRejPer = string.IsNullOrEmpty(row["AllowedRejPer"].ToString()) ? 0 : Convert.ToSingle(row["AllowedRejPer"].ToString()),
                        ShortExcessQty = string.IsNullOrEmpty(row["ShortExcessQty"].ToString()) ? 0 : Convert.ToSingle(row["ShortExcessQty"].ToString()),
                        Unit = string.IsNullOrEmpty(row["Unit"].ToString()) ? "" : row["Unit"].ToString(),
                        AltUnit = string.IsNullOrEmpty(row["AltUnit"].ToString()) ? "" : row["AltUnit"].ToString(),
                        Remark = string.IsNullOrEmpty(row["Remark"].ToString()) ? "" : row["Remark"].ToString(),
                        Purpose = string.IsNullOrEmpty(row["Purpose"].ToString()) ? "" : row["Purpose"].ToString(),
                        Color = string.IsNullOrEmpty(row["Color"].ToString()) ? "" : row["Color"].ToString(),
                        QcCompleted = string.IsNullOrEmpty(row["QcCompleted"].ToString()) ? "" : row["QcCompleted"].ToString(),
                        CustbatchNo = string.IsNullOrEmpty(row["CustBatchno"].ToString()) ? "" : row["CustBatchno"].ToString(),
                        BatchNo = string.IsNullOrEmpty(row["batchno"].ToString()) ? "" : row["batchno"].ToString(),
                        UniqueBatchNo = string.IsNullOrEmpty(row["UniqueBatchNo"].ToString()) ? "" : row["UniqueBatchNo"].ToString(),
                        SoNo = string.IsNullOrEmpty(row["SoNo"].ToString()) ? "" : row["SoNo"].ToString(),
                        CustOrderno = string.IsNullOrEmpty(row["CustOrderno"].ToString()) ? "" : row["CustOrderno"].ToString(),
                        SOSchNo = string.IsNullOrEmpty(row["SOSchNo"].ToString()) ? "" : row["SOSchNo"].ToString(),
                        SoDate = string.IsNullOrEmpty(row["SODate"].ToString()) ? "" : row["SODate"].ToString(),
                        Schdate = string.IsNullOrEmpty(row["SchDate"].ToString()) ? "" : row["SchDate"].ToString(),
                        BomName = string.IsNullOrEmpty(row["BomName"].ToString()) ? "" : row["BomName"].ToString(),
                        BomDate = string.IsNullOrEmpty(row["BomDate"].ToString()) ? "" : row["BomDate"].ToString(),
                        INDBOM = string.IsNullOrEmpty(row["INDBOM"].ToString()) ? "" : row["INDBOM"].ToString(),
                        RecItemName = string.IsNullOrEmpty(row["RECItemPartcode"].ToString()) ? "" : row["RECItemPartcode"].ToString(),
                        RecPartCode = string.IsNullOrEmpty(row["RecPartCode"].ToString()) ? "" : row["RecPartCode"].ToString(),
                        FinishItemName = string.IsNullOrEmpty(row["FGItemName"].ToString()) ? "" : row["FGItemName"].ToString(),
                        FinishPartCode = string.IsNullOrEmpty(row["FGPartCode"].ToString()) ? "" : row["FGPartCode"].ToString(),
                    });
                }
                model.CustomerJWRGrid = ItemList;
            }

            return model;
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string EntryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CustomerJobworkReceiveMainDetail", SqlParams);
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
