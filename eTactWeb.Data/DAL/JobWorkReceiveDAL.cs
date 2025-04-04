using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
//using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Net;

using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class JobWorkReceiveDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public JobWorkReceiveDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Job Work Receive"));
               // SqlParams.Add(new SqlParameter("@SubMenu", "Job Work receive"));

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
        public async Task<DataSet> BindBranch(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMIR", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "BranchList";
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
        public async Task<ResponseResult> FillNewEntry(int YearCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetGateNo(string Flag, string SPName, string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@Todate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@YearCode", null));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                //SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateItemData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                //SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetEmployeeList(string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetProcessList(string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetProcessUnit(string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetBomRevNo(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETBOMREVNO", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> ViewDetailSection(int yearCode,int entryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYIDDeatilSection"));
                SqlParams.Add(new SqlParameter("@EntryID", entryId));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPopUpChallanData(int AccountCode, int YearCode, string FromDate, string ToDate, int RecItemCode, int BomRevNo, string BomRevDate, string BOMIND, string BillChallanDate, string JobType, string ProdUnProd)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                DateTime fromdt = DateTime.ParseExact("01/04/2022", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime billDt = DateTime.ParseExact(BillChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime bomrevDate = new DateTime();
                if (BomRevDate != null)
                {
                    bomrevDate = DateTime.ParseExact(BomRevDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //SqlParams.Add(new SqlParameter("@RevDate", bomrevDate.ToString("yyyy/MM/dd")));
                    SqlParams.Add(new SqlParameter("@RevDate", DateTime.Today));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@RevDate", ""));
                }



                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@Year", YearCode));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@Todate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@RecItemCode", RecItemCode));
                SqlParams.Add(new SqlParameter("@BOMRevNo", BomRevNo));
                SqlParams.Add(new SqlParameter("@BOMINd", BOMIND));
                SqlParams.Add(new SqlParameter("@billchallandate", billDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@JobType", JobType));
                SqlParams.Add(new SqlParameter("@prodUnProd", ProdUnProd));
                //SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SelPendJobWorkChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }


            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPopUpData(string Flag, int AccountCode, int IssYear, string FinYearFromDate, string billchallandate, string prodUnProd, string BOMINd, int RMItemCode, string RMPartcode, string RMItemNAme, string ACCOUNTNAME, int Processid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime FinFromDt = new DateTime();
                DateTime BillChallanDt = new DateTime();

                FinFromDt = ParseDate(FinYearFromDate);
                BillChallanDt = ParseDate(billchallandate);

                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@IssYear", IssYear));
                //SqlParams.Add(new SqlParameter("@FinYearFromDate", FinYearFromDate));
                //SqlParams.Add(new SqlParameter("@billchallandate", billchallandate));
                SqlParams.Add(new SqlParameter("@FinYearFromDate", FinFromDt == default ? string.Empty : FinFromDt));
                SqlParams.Add(new SqlParameter("@billchallandate", FinFromDt == default ? string.Empty : BillChallanDt));

                SqlParams.Add(new SqlParameter("@prodUnProd", prodUnProd));
                SqlParams.Add(new SqlParameter("@BOMINd", BOMINd));
                SqlParams.Add(new SqlParameter("@RMItemCode", RMItemCode));
                SqlParams.Add(new SqlParameter("@RMPartcode", RMPartcode));
                SqlParams.Add(new SqlParameter("@RMItemNAme", RMItemNAme));
                SqlParams.Add(new SqlParameter("@ACCOUNTNAME", ACCOUNTNAME));
                SqlParams.Add(new SqlParameter("@Processid", Processid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("getPendJobWorkChallanList", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal async Task<ResponseResult> GetAdjustedChallan(int AccountCode, int YearCode, string FinYearFromDate, string billchallandate, string GateNo, int GateYearCode, DataTable DTTItemGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime FinFromDt = new DateTime();
                DateTime BillChallanDt = new DateTime();

                FinFromDt = ParseDate(FinYearFromDate);
                BillChallanDt = ParseDate(billchallandate);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@yearCode", YearCode));
                //SqlParams.Add(new SqlParameter("@FromFinStartDate", FinYearFromDate));
                //SqlParams.Add(new SqlParameter("@billchallandate", billchallandate));
                SqlParams.Add(new SqlParameter("@FromFinStartDate", FinFromDt == default ? string.Empty : FinFromDt));
                SqlParams.Add(new SqlParameter("@billchallandate", BillChallanDt == default ? string.Empty : BillChallanDt));

                SqlParams.Add(new SqlParameter("@Gateno", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearcode", GateYearCode));
                SqlParams.Add(new SqlParameter("@DTTItemGrid", DTTItemGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpVenJobworkAdjustedChallan", SqlParams);
                //if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                //{
                //    PrepareViewforChallanAdjust(_ResponseResult.Result, ref model);
                //}
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBomValidated(int RecItemCode, int BomRevNo, string BomRevDate, int RecQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();


                SqlParams.Add(new SqlParameter("@FGItem", RecItemCode));
                SqlParams.Add(new SqlParameter("@BOMREVNo", BomRevNo));
                SqlParams.Add(new SqlParameter("@FGQty", RecQty));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpVerifyBOMChild", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }


            return _ResponseResult;
        }

        //public static string ParseDate(string dateString)
        //{
        //    if (string.IsNullOrEmpty(dateString))
        //    {
        //        return string.Empty;
        //    }

        //    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        //    {
        //        return parsedDate.ToString("yyyy/MM/dd");
        //    }
        //    else
        //    {
        //        if (DateTime.TryParse(dateString, out DateTime nonFormattedDate))
        //        {
        //            return nonFormattedDate.ToString("yyyy/MM/dd");
        //        }

        //        return string.Empty;
        //    }
        //}
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

        internal async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (Flag == "True")
                {
                    //DateTime FromDt = DateTime.Parse(Fromdate, CultureInfo.InvariantCulture);
                    //DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", Todate));
                }
                else
                {
                    DateTime FromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", FromDt.ToString("yyyy/MM/dd")));
                    SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<JWReceiveDashboard> GetDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName, string BranchName, string InvNo, string Fromdate, string Todate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new JWReceiveDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_JobworkRec", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    DateTime FromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@branchname", BranchName);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@MRNNo", MRNNo);
                    oCmd.Parameters.AddWithValue("@InvNo", InvNo);
                    oCmd.Parameters.AddWithValue("@FromDate", FromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", todt.ToString("yyyy/MM/dd"));


                    //oCmd.Parameters.AddWithValue("@ItemCategory", model.ItemCategory);
                    // oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.JWRecQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new JWReceiveDashboard
                                             {
                                                 VendorName = dr["VendorName"].ToString(),
                                                 GateNo = dr["GateNo"].ToString(),
                                                 GateDate = dr["GateDate"].ToString(),
                                                 MRNNo = dr["MRNNo"].ToString(),
                                                 MRNDate = dr["MRNDate"].ToString(),
                                                 InvNo = dr["InvNo"].ToString(),
                                                 InvDate = dr["InvDate"].ToString(),
                                                 QCCompleted = dr["QCCompleted"].ToString(),
                                                 TotalAmt = Convert.ToDecimal(dr["TotalAmt"]),
                                                 NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                                 PurchaseBillPosted = dr["PurchaseBillPosted"].ToString(),
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 YearCode = Convert.ToInt32(dr["YearCode"]),
                                                 GateYearCode = Convert.ToInt32(dr["GateYearCode"]),
                                                 BranchName = dr["BranchName"].ToString(),
                                                 EnteredBy = dr["EnteredBy"].ToString(),
                                                 UpdatedBy = dr["UpdatedBy"].ToString()
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

        public async Task<JWReceiveDashboard> GetDetailDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName, string BranchName, string InvNo, string Fromdate, string Todate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new JWReceiveDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_JobworkRec", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    DateTime FromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "DetailSearch");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@branchname", BranchName);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@MRNNo", MRNNo);
                    oCmd.Parameters.AddWithValue("@InvNo", InvNo);
                    oCmd.Parameters.AddWithValue("@FromDate", FromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", todt.ToString("yyyy/MM/dd"));


                    //oCmd.Parameters.AddWithValue("@ItemCategory", model.ItemCategory);
                    // oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.JWRecQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new JWReceiveDashboard
                                             {
                                                 VendorName = dr["VendorName"].ToString(),
                                                 GateNo = dr["GateNo"].ToString(),
                                                 GateDate = dr["GateDate"].ToString(),
                                                 MRNNo = dr["MRNNo"].ToString(),
                                                 MRNDate = dr["MRNDate"].ToString(),
                                                 InvNo = dr["InvNo"].ToString(),
                                                 InvDate = dr["InvDate"].ToString(),
                                                 QCCompleted = dr["QCCompleted"].ToString(),
                                                 TotalAmt = Convert.ToDecimal(dr["TotalAmt"]),
                                                 NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                                 PurchaseBillPosted = dr["PurchaseBillPosted"].ToString(),
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 YearCode = Convert.ToInt32(dr["YearCode"]),
                                                 GateYearCode = Convert.ToInt32(dr["GateYearCode"]),
                                                 BranchName = dr["BranchName"].ToString(),
                                                 EnteredBy = dr["EnteredBy"].ToString(),
                                                 UpdatedBy = dr["UpdatedBy"].ToString(),
                                                 ItemName = dr["ItemName"].ToString(),
                                                 PartCode = dr["PartCode"].ToString(),
                                                 Unit = dr["Unit"].ToString(),
                                                 BillQty = dr["BillQty"].ToString(),
                                                 RecQty = dr["RecQty"].ToString(),
                                                 Amount = dr["Amount"].ToString(),
                                                 ProducedUnProd = dr["ProducedUnprod"].ToString(),
                                                 BomRevNo = dr["BomRevNo"].ToString(),
                                                 PONo = dr["PONo"].ToString(),
                                                 POYearCode = dr["POYearCode"].ToString(),
                                                 PODate = dr["PODate"].ToString(),
                                                 BatchNo = dr["BatchNo"].ToString(),
                                                 UniqueBatchNo = dr["UniqueBatchNo"].ToString(),
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
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "JWREC"));
                SqlParams.Add(new SqlParameter("@MainBOMItem", ItemCode));
                SqlParams.Add(new SqlParameter("@PFGQTY", WOQty));
                SqlParams.Add(new SqlParameter("@PBOMREVNO", BomRevNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpDisplayBomDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> DeleteByID(int ID, int YearCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<JobWorkReceiveModel> GetViewByID(int ID, int YearCode)
        {
            var model = new JobWorkReceiveModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkRec", SqlParams);

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

        private static JobWorkReceiveModel PrepareView(DataSet DS, ref JobWorkReceiveModel? model)
        {
            try
            {
                var ChallanGrid = new List<JobWorkReceiveDetail>();
                var ItemGrid = new List<JobWorkReceiveItemDetail>();
                DS.Tables[0].TableName = "SSMain";
                DS.Tables[1].TableName = "SSDetail";
                DS.Tables[2].TableName = "SSChallanTable";
                int cnt = 0;
                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
                model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
                model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
                model.GateNo = DS.Tables[0].Rows[0]["GateNo"].ToString();
                model.GateYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"].ToString());
                model.AccountName = DS.Tables[0].Rows[0]["VendorName"].ToString();
                model.InvNo = DS.Tables[0].Rows[0]["InvNo"].ToString();
                model.InvDate = DS.Tables[0].Rows[0]["InvDate"].ToString();
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.MRNNo = DS.Tables[0].Rows[0]["MRNNo"].ToString();
                model.GateDate = DS.Tables[0].Rows[0]["GateDate"].ToString();
                model.RecStoreid = Convert.ToInt32(DS.Tables[0].Rows[0]["RecStoreId"].ToString());
                model.RecInStore = DS.Tables[0].Rows[0]["RecStore"].ToString();
                model.QCCheck = DS.Tables[0].Rows[0]["CheckQc"].ToString();
                model.TypesBOMIND = DS.Tables[0].Rows[0]["BOMINd"].ToString();

                if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ItemGrid.Add(new JobWorkReceiveItemDetail
                        {
                            SeqNo = cnt + 1,
                            ItemCode = Convert.ToInt32(row["Item_Code"]),
                            ItemName = row["Item_Name"].ToString(),
                            PartCode = row["PartCode"].ToString(),
                            Unit = row["Unit"].ToString(),
                            BillQty = Convert.ToDecimal(row["BillQty"]),
                            RecQty = Convert.ToDecimal(row["RecQty"]),
                            JWRate = Convert.ToDecimal(row["JWRate"]),
                            Amount = Convert.ToDecimal(row["Amount"]),
                            Remark = row["Remark"].ToString(),
                            ProducedUnprod = row["ProducedUnProd"].ToString(),
                            ProcessId = Convert.ToInt32(row["ProcessId"]),
                            Adjusted = row["Adjusted"].ToString(),
                            BomRevNo = Convert.ToInt32(row["ProcessId"]),
                            BomRevDate = row["BOMRevDate"].ToString(),
                            NoOfCase = Convert.ToInt32(row["NoOfCase"]),
                            QCCompleted = row["QCCompleted"].ToString(),
                            PONO = row["PONo"].ToString(),
                            POYearCode = Convert.ToInt32(row["POYearCode"]),
                            PODate = row["PODate"].ToString(),
                            SchNo = row["SchNo"].ToString(),
                            SchYearCode = row["SchYearCode"].ToString(),
                            SchDate = row["SchDate"].ToString(),
                            POType = row["POType"].ToString(),
                            BomInd = row["BOMINd"].ToString(),
                            BatchNo = row["BatchNo"].ToString(),
                            UniqueBatchNo = row["Uniquebatchno"].ToString(),
                            BatchWise = row["batchwise"].ToString(),
                            JWRateUnit = row["JWRateUnit"].ToString(),
                            ProcessUnitQty = Convert.ToDecimal(row["ProcessUnitQty"].ToString()),
                            ProcessUnit = row["ProcessUnit"].ToString(),
                            POAmendNo = Convert.ToInt32(row["POAmendNo"])
                        });
                    }
                    model.JobWorkReceiveGrid = ItemGrid;
                }

                if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[2].Rows)
                    {
                        ChallanGrid.Add(new JobWorkReceiveDetail
                        {
                            SeqNo = cnt + 1,
                            EntryIdIssJw = Convert.ToInt32(row["VendJWIssEntryId"]),
                            YearCodeIssJw = Convert.ToInt32(row["vendjwrecYearCode"]),
                            IssChallanNo = row["IssChallanNo"].ToString(),
                            IssChallanDate = row["IssChallanDate"].ToString(),
                            AccountCode = Convert.ToInt32(row["AccountCode"]),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            IssItemName = row["RMItemName"].ToString(),
                            IssPartCode = row["RMpartCode"].ToString(),
                            EntryIdRecJw = Convert.ToInt32(row["vendjwrecEntryID"]),
                            YearCodeRecJw = Convert.ToInt32(row["vendjwrecYearCode"]),
                            FinishItemCode = Convert.ToInt32(row["FinishItemCode"]),
                            FinishItemName = row["FGItemName"].ToString(),
                            FinishPartCode = row["FGpartCode"].ToString(),
                            AdjQty = Convert.ToDecimal(row["AdjQty"]),
                            CC = row["CC"].ToString(),
                            AdjFormType = row["AdjFormType"].ToString(),
                            TillDate = row["TillDate"].ToString(),
                            TotRecQTy = Convert.ToDecimal(row["TotRecQty"]),
                            PendQty = Convert.ToDecimal(row["PendQty"]),
                            BOMQty = Convert.ToDecimal(row["BOMQty"]),
                            BOMrevno = Convert.ToInt32(row["BOMrevno"].ToString()),
                            BOMRevDate = row["BOMRevDate"].ToString(),
                            ProcessId = Convert.ToInt32(row["ProcessId"]),
                            BOMInd = row["BOMInd"].ToString(),
                            RecQty = Convert.ToDecimal(row["RecQty"]),
                            TotaladjQty = Convert.ToDecimal(row["TotadjQty"]),
                            TotalRecQty = Convert.ToDecimal(row["TotalRecQty"]),
                            TotalIssuedQty = Convert.ToDecimal(row["TotalIssuedQty"]),
                            RunnerItemCode = Convert.ToInt32(row["RunnerItemCode"]),
                            ScrapItemCode = Convert.ToInt32(row["ScrapItemCode"]),
                            IdealScrapQty = Convert.ToDecimal(row["IdealScrapQty"]),
                            IssuedScrapQty = Convert.ToDecimal(row["IssuedScrapQty"]),
                            PreRecChallanNo = row["PreRecChallanNo"].ToString(),
                            ScrapqtyagainstRcvQty = Convert.ToDecimal(row["ScrapqtyagainstRcvqty"]),
                            IssuedBatchNO = row["Issuedbatchno"].ToString(),
                            IssuedUniqueBatchNo = row["Issueduniquebatchno"].ToString(),
                            Recbatchno = row["Recbatchno"].ToString(),
                            Recuniquebatchno = row["Recuniquebatchno"].ToString(),
                            ScrapAdjusted = row["ScrapAdjusted"].ToString()
                        });
                    }
                    model.ItemDetailGrid = ChallanGrid;
                }

                //model.BiltyDate = DateTime.ParseExact(DS.Tables[0].Rows[0]["BiltyDate"].ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

                //model.EntryDate = DateTime.ParseExact(DS.Tables[0].Rows[0]["EntryDate"].ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //private static JobWorkReceiveModel PrepareViewforChallanAdjust(DataTable DS, ref JobWorkReceiveModel? model)
        //{
        //    var ItemGrid = new List<JobWorkReceiveDetail>();
        //    DS.TableName = "SSMain";
        //    DS.TableName = "SSDetail";
        //    int cnt = 0;
        //    //model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
        //    //model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());

        //    //model.GateNo = DS.Tables[0].Rows[0]["GateNo"].ToString();
        //    //model.GateYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"].ToString());
        //    //model.AccountName = DS.Tables[0].Rows[0]["VendorName"].ToString();
        //    //model.InvNo = DS.Tables[0].Rows[0]["InvNo"].ToString();
        //    //model.InvDate = DS.Tables[0].Rows[0]["InvDate"].ToString();
        //    //model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
        //    //model.MRNNo = DS.Tables[0].Rows[0]["MRNNo"].ToString();
        //    //model.GateDate = DS.Tables[0].Rows[0]["GateDate"].ToString();
        //    //model.RecStoreid = Convert.ToInt32(DS.Tables[0].Rows[0]["RecStoreId"].ToString());
        //    //model.RecInStore = DS.Tables[0].Rows[0]["RecStore"].ToString();
        //    //model.QCCheck = DS.Tables[0].Rows[0]["CheckQc"].ToString();

        //    if ( DS.Rows.Count > 0)
        //    {
        //        foreach (DataRow row in DS.Rows)
        //        {
        //            ItemGrid.Add(new JobWorkReceiveDetail
        //            {
        //                SeqNo = cnt + 1,
        //                EntryIdIssJw = Convert.ToInt32(row["EntryIdIssJw"]),
        //                IssChallanNo = row["IssJWChallanNo"].ToString(),
        //                IssYearCode = Convert.ToInt32(row["IssYearCode"]),
        //                IssChallanDate = row["ChallanDate"].ToString(),
        //                IssPartCode = row["IssPartCode"].ToString(),
        //                IssItemName = row["IssItemName"].ToString(),
        //                ItemCode = Convert.ToInt32(row["IssItemCode"]),
        //                BOMrevno = Convert.ToInt32(row["BomNo"]),
        //                BOMRevDate = row["BOMDate"].ToString(),
        //                BOMInd = row["BomStatus"].ToString(),
        //                PendQty = Convert.ToDecimal(row["PendQty"]),
        //                FinishPartCode = row["FinishPartcode"].ToString(),
        //                FinishItemName = row["FinishItemName"].ToString(),
        //                FinishItemCode = Convert.ToInt32(row["RecItemCode"]),
        //                BOMQty = Convert.ToDecimal(row["bomqty"]),
        //                Through = row["through"].ToString(),
        //                RecQty = Convert.ToDecimal(row["QtyToBerec"]),
        //                AdjQty = Convert.ToDecimal(row["ActualAdjQty"]),
        //                IssuedBatchNO = row["batchno"].ToString(),
        //                IssuedUniqueBatchNo = row["uniquebatchno"].ToString(),
        //            });
        //        }
        //        model.ItemDetailGrid = ItemGrid;
        //    }


        //    //model.BiltyDate = DateTime.ParseExact(DS.Tables[0].Rows[0]["BiltyDate"].ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

        //    //model.EntryDate = DateTime.ParseExact(DS.Tables[0].Rows[0]["EntryDate"].ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

        //    return model;
        //}
        internal async Task<ResponseResult> SaveJobReceive(JobWorkReceiveModel model, DataTable JWRGrid, DataTable ChallanGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //entDt = ParseDate(model.EntryDate);
                //bilDt = ParseDate(model.BiltyDate);
                //invDt = ParseDate(model.InvoiceDate);


                DateTime GateDt = new DateTime();
                DateTime entDt = new DateTime();
                DateTime invDt = new DateTime();
                entDt = ParseDate(model.EntryDate);
                GateDt = ParseDate(model.GateDate);

                invDt = ParseDate(model.InvDate);

                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                var currentDate = eTactWeb.Data.Common.CommonFunc.ParseFormattedDate((DateTime.Now).ToString());

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));

                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@InvNo", model.InvNo));
                //SqlParams.Add(new SqlParameter("@InvDate", model.InvDate));
                SqlParams.Add(new SqlParameter("@InvDate", invDt == default ? string.Empty : invDt));

                SqlParams.Add(new SqlParameter("@MrnNo", model.MRNNo));
                SqlParams.Add(new SqlParameter("@GateNo", model.GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
                //SqlParams.Add(new SqlParameter("@GateDate", model.GateDate));
                SqlParams.Add(new SqlParameter("@GateDate", GateDt == default ? string.Empty : GateDt));

                SqlParams.Add(new SqlParameter("@RecStoreid", model.RecStoreid));
                SqlParams.Add(new SqlParameter("@Through", "Y"));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@UID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@QCCheck", model.QCCheck));
                SqlParams.Add(new SqlParameter("@DocumentTypeBillChal", model.DocumentTypeBillChal));
                SqlParams.Add(new SqlParameter("@JobTypeJwRW", model.JobTypeJwRW));
                SqlParams.Add(new SqlParameter("@PurBillNo", model.PurBillNo));
                SqlParams.Add(new SqlParameter("@PurBillYear", model.PurBillYear));
                SqlParams.Add(new SqlParameter("@Adjusted", model.Adjusted));
                SqlParams.Add(new SqlParameter("@QCCompleted", model.QCCompleted));
                SqlParams.Add(new SqlParameter("@BLOCKSHORTAGEDEBITNOTE", model.BlockShortageDebitNote));
                SqlParams.Add(new SqlParameter("@PurchaseBillPosted", model.PurchaseBillPosted));
                SqlParams.Add(new SqlParameter("@TypesBOMIND", model.TypesBOMIND));
                SqlParams.Add(new SqlParameter("@Empid", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@FOC", "N"));
                SqlParams.Add(new SqlParameter("@TotalAmt", model.TotalAmt));
                SqlParams.Add(new SqlParameter("@NetAmt", model.NetAmt));
                SqlParams.Add(new SqlParameter("@EnteredEMPID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", currentDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@UpdatedOn", currentDate));

                SqlParams.Add(new SqlParameter("@DTSSGrid", JWRGrid));
                SqlParams.Add(new SqlParameter("@DTSSGridAdjust", ChallanGrid));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkRec", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetFeatureOption(string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
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
