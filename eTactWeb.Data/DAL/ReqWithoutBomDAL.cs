using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class ReqWithoutBomDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        private readonly ConnectionStringService _connectionStringService;
        public ReqWithoutBomDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
                SqlParams.Add(new SqlParameter("@EmployeeId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "General"));
                SqlParams.Add(new SqlParameter("@SubMenu", "Requisition Without BOM"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetProjectNo()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetProjectNo"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_RequisitionWithoutBOM", SqlParams);
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
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "BranchList";
                    _ResponseResult.Result.Tables[1].TableName = "ProjectList";
                    _ResponseResult.Result.Tables[2].TableName = "DepartmentList";
                    _ResponseResult.Result.Tables[3].TableName = "CostCenterList";
                    _ResponseResult.Result.Tables[4].TableName = "EmployeeList";
                    _ResponseResult.Result.Tables[5].TableName = "MachineList";
                    _ResponseResult.Result.Tables[6].TableName = "StoreList";

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

        public async Task<ResponseResult> FillItems(string TF)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLITEM"));
                if (TF == "T")
                {
                    SqlParams.Add(new SqlParameter("@SHowAllItem", "Y"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@SHowAllItem", "N"));

                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@ALtQty", AltQty));
                SqlParams.Add(new SqlParameter("@UnitQty", UnitQty));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("AltUnitConversion", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(string TF)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPartCode"));
                if (TF == "T")
                {
                    SqlParams.Add(new SqlParameter("@SHowAllItem", "Y"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@SHowAllItem", "N"));

                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkOrder()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLWorkOrder"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLWorkCenter"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Store)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", Store));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Today));



                _ResponseResult = await _IDataLogic.ExecuteDataSet("GETSTORETotalSTOCK", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<RWBDashboard> GetDashboardData(string REQNo, string WCName, string WONO, string DepName, string PartCode, string ItemName, string BranchName, string Fromdate, string Todate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new RWBDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_RequisitionWithoutBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@REQNo", REQNo);
                    oCmd.Parameters.AddWithValue("@WCName", WCName);
                    oCmd.Parameters.AddWithValue("@WONo", WONO);
                    oCmd.Parameters.AddWithValue("@DepName", DepName);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@itemname", ItemName);
                    oCmd.Parameters.AddWithValue("@branchname", BranchName);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));



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
                    model.ReqMainDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new RWBDashboard
                                              {
                                                  //,,,,,
                                                  REQNo = dr["REQNo"].ToString(),
                                                  ReqDate = dr["ReqDate"].ToString(),
                                                  EntryDate = dr["EntryDate"].ToString(),
                                                  WorkCenter = dr["WorkCenter"].ToString(),
                                                  WONo = dr["WONO"].ToString(),
                                                  BranchName = dr["BranchName"].ToString(),
                                                  Reason = dr["Reason"].ToString(),
                                                  Cancel = dr["Cancel"].ToString(),
                                                  MachName = dr["MachName"].ToString(),
                                                  WOYearcode = dr["WOYearcode"].ToString(),
                                                  EntryId = Convert.ToInt32(dr["EntryId"]),
                                                  YearCode = Convert.ToInt32(dr["YearCode"]),
                                                  TotalReqQty = Convert.ToDecimal(dr["TotalReqQty"]),
                                                  TotalPendQty = Convert.ToDecimal(dr["TotalPendQty"]),
                                                  Completed = dr["Completed"].ToString(),
                                                  CreatedBy=dr["CreatedByName"].ToString(),
                                                  UpdatedBy=dr["UpdatedByName"].ToString()
                                              }).ToList();
                }
                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

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
        public async Task<RWBDashboard> GetDetailData(string REQNo, string WCName, string WONO, string DepName, string PartCode, string ItemName, string BranchName, string Fromdate, string Todate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new RWBDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_RequisitionWithoutBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", "GetDetailDashboardData");
                    oCmd.Parameters.AddWithValue("@REQNo", REQNo);
                    oCmd.Parameters.AddWithValue("@WCName", WCName);
                    oCmd.Parameters.AddWithValue("@WONo", WONO);
                    oCmd.Parameters.AddWithValue("@DepName", DepName);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@itemname", ItemName);
                    oCmd.Parameters.AddWithValue("@branchname", BranchName);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));



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
                    model.ReqMainDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new RWBDashboard
                                              {
                                                  //string.IsNullOrEmpty(row["DeliveryDate"].ToString()) ? "" : row["DeliveryDate"].ToString()
                                                  //,,,,,
                                                  REQNo = dr["REQNo"].ToString(),
                                                  ReqDate = dr["ReqDate"].ToString(),
                                                  EntryDate = dr["EntryDate"].ToString(),
                                                  WorkCenter = dr["WorkCenter"].ToString(),
                                                  WONo = dr["WONO"].ToString(),
                                                  BranchName = dr["BranchName"].ToString(),
                                                  Reason = dr["Reason"].ToString(),
                                                  Cancel = dr["Cancel"].ToString(),
                                                  MachName = dr["MachName"].ToString(),
                                                  WOYearcode = dr["WOYearcode"].ToString(),
                                                  EntryId = string.IsNullOrEmpty(dr["EntryId"].ToString()) ? 0 : Convert.ToInt32(dr["EntryId"].ToString()),
                                                  YearCode = string.IsNullOrEmpty(dr["YearCode"].ToString()) ? 0 : Convert.ToInt32(dr["YearCode"].ToString()),
                                                  TotalReqQty = string.IsNullOrEmpty(dr["TotalReqQty"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalReqQty"]),
                                                  TotalPendQty = string.IsNullOrEmpty(dr["TotalPendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalPendQty"]),
                                                  Completed = dr["Completed"].ToString(),
                                                  ItemName = dr["Item_Name"].ToString(),
                                                  PartCode = dr["PartCode"].ToString(),
                                                  Unit = dr["Unit"].ToString(),
                                                  AltUnit = dr["AltUnit"].ToString(),
                                                  Location = dr["ItemLocation"].ToString(),
                                                  BinNo = dr["ItemBinRackNo"].ToString(),
                                                  Qty = string.IsNullOrEmpty(dr["Qty"].ToString()) ? 0 : Convert.ToSingle(dr["Qty"]),
                                                  AltQty = string.IsNullOrEmpty(dr["AltQty"].ToString()) ? 0 :  Convert.ToSingle(dr["AltQty"]),
                                                  PendQty = string.IsNullOrEmpty(dr["PendQty"].ToString()) ? 0 : Convert.ToSingle(dr["PendQty"]),
                                              }).ToList();
                }
                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

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

        internal async Task<ResponseResult> DeleteByID(int ID, int YearCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_RequisitionWithoutBOM", SqlParams);
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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<ResponseResult> SaveRequisition(RequisitionWithoutBOMModel model, DataTable ReqGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                var entryDt="";
                var reqDt = "";
                var expDt = "";
                var woDt = "";
                entryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                //reqDt = ParseDate(model.ReqDate);
                reqDt= CommonFunc.ParseFormattedDate(model.ReqDate);
                expDt = CommonFunc.ParseFormattedDate(model.ExpectedDate);
                woDt = CommonFunc.ParseFormattedDate(model.WODate);
               var cancleDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
               var approveDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));


                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));

                //SqlParams.Add(new SqlParameter("@YearCode", DateTime.Now.Year)); 
                SqlParams.Add(new SqlParameter("@EntryDate", entryDt == default ? string.Empty : entryDt));
                SqlParams.Add(new SqlParameter("@REQNo", model.REQNo));
                SqlParams.Add(new SqlParameter("@ReqDate", reqDt == default ? string.Empty : reqDt));
                SqlParams.Add(new SqlParameter("@ReqTime", DateTime.Today));
                SqlParams.Add(new SqlParameter("@FromDepartmentId", model.FromDepartmentId));
                SqlParams.Add(new SqlParameter("@WONo", model.WONo));
                SqlParams.Add(new SqlParameter("@WOYearcode", model.WoYearCode));
                SqlParams.Add(new SqlParameter("@WODate", woDt == default ? string.Empty : woDt));
                SqlParams.Add(new SqlParameter("@MachineId", model.MachineId));
                SqlParams.Add(new SqlParameter("@WorkcenterId", model.WorkCenterId));
                SqlParams.Add(new SqlParameter("@Remarks", model.Remarks));
                SqlParams.Add(new SqlParameter("@ReqReason", model.ReqReason));
                SqlParams.Add(new SqlParameter("@UID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@Completed", "N"));
                SqlParams.Add(new SqlParameter("@Cancel", "N"));
                SqlParams.Add(new SqlParameter("@CancelDate", cancleDt));
                SqlParams.Add(new SqlParameter("@CancelReason", model.CancelReason));
                SqlParams.Add(new SqlParameter("@EneterdBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@RequByEmpId", model.RequByEmpId));
                SqlParams.Add(new SqlParameter("@NeedApproval", model.NeedApproval));
                SqlParams.Add(new SqlParameter("@Approved", model.Approved));
                SqlParams.Add(new SqlParameter("@ApproveDate", approveDt));
                SqlParams.Add(new SqlParameter("@ApproveBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@LineRejEntryId", model.LineRejEntryId));
                SqlParams.Add(new SqlParameter("@LineRejYearCode", model.LineRejYearCode));
                SqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

                SqlParams.Add(new SqlParameter("@DTItemGrid", ReqGrid));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        internal async Task<RequisitionWithoutBOMModel> GetViewByID(int ID, int YearCode)
        {
            var model = new RequisitionWithoutBOMModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);

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

        private static RequisitionWithoutBOMModel PrepareView(DataSet DS, ref RequisitionWithoutBOMModel? model)
        {
            var ItemGrid = new List<RequisitionDetail>();
            DS.Tables[0].TableName = "ReqMainDetail";
            //DS.Tables[1].TableName = "ReqDetail";
            int cnt = 1;
            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"]);
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"]);
            model.Prefix = DS.Tables[0].Rows[0]["Prefix"].ToString();
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.ReqDate = DS.Tables[0].Rows[0]["ReqDate"].ToString();
            model.CC = DS.Tables[0].Rows[0]["BranchName"].ToString();
            model.FromDepartmentId = Convert.ToInt32(DS.Tables[0].Rows[0]["FromDepartmentId"]);
            model.WONo = DS.Tables[0].Rows[0]["WONo"].ToString();
            model.WoYearCode = 2023;
            model.WODate = DS.Tables[0].Rows[0]["WODate"].ToString();
            model.MachineId = Convert.ToInt32(DS.Tables[0].Rows[0]["MachineId"]);
            model.WorkCenterId = Convert.ToInt32(DS.Tables[0].Rows[0]["WorkcenterId"]);
            model.RequByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["RequByEmpId"]);
            model.REQNo = DS.Tables[0].Rows[0]["REQNo"].ToString();
            model.ReqReason = DS.Tables[0].Rows[0]["ReqReason"].ToString();
            model.Remarks = DS.Tables[0].Rows[0]["Remarks"].ToString();
            model.ReqReason = DS.Tables[0].Rows[0]["ReqReason"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"]);
            model.Completed = DS.Tables[0].Rows[0]["Completed"].ToString();
            model.Cancel = DS.Tables[0].Rows[0]["Cancel"].ToString();
            model.CancelDate = DS.Tables[0].Rows[0]["CancelDate"].ToString();
            model.CancelReason = DS.Tables[0].Rows[0]["CancelReason"].ToString();
            model.NeedApproval = DS.Tables[0].Rows[0]["NeedApproval"].ToString();
            model.Approved = DS.Tables[0].Rows[0]["Approved"].ToString();
            model.ApproveDate = DS.Tables[0].Rows[0]["ApproveDate"].ToString();
            model.ApproveBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ApproveBy"]);
            model.LineRejEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["LineRejEntryId"]);
            model.LineRejYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["LineRejYearCode"]);
            model.EntryByMachineName = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();

            //CreatedBy
            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedBy"].ToString());
            model.CreatedByName = DS.Tables[0].Rows[0]["CreatedByName"].ToString();
            model.CreatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["CreatedOn"]);

            //updatedBy
            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationdate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationdate"]);
            }


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemGrid.Add(new RequisitionDetail
                    {
                        SeqNo = cnt++,
                        ItemCode = string.IsNullOrEmpty(row["ItemCode"].ToString()) ? 0 : Convert.ToInt32(row["ItemCode"]),
                        ItemName = row["Item_Name"].ToString(),
                        PartCode = row["PartCode"].ToString(),
                        Unit = row["Unit"].ToString(),
                        StoreId = string.IsNullOrEmpty(row["StoreId"].ToString()) ? 0 : Convert.ToInt32(row["StoreId"]),
                        StoreName = row["Store_Name"].ToString(),
                        Qty = string.IsNullOrEmpty(row["Qty"].ToString()) ? 0 : Convert.ToDecimal(row["Qty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        AltQty = string.IsNullOrEmpty(row["AltQty"].ToString()) ? 0 : Convert.ToDecimal(row["AltQty"]),
                        ItemModel = row["ItemModel"].ToString(),
                        ExpectedDate = row["ExpectedDate"].ToString(),
                        Remark = row["Remark"].ToString(),
                        PendQty = string.IsNullOrEmpty(row["PendQty"].ToString()) ? 0 : Convert.ToDecimal(row["PendQty"]),
                        PendAltQty = string.IsNullOrEmpty(row["PendAltQty"].ToString()) ? 0 : Convert.ToDecimal(row["PendAltQty"]),
                        TotalStock = string.IsNullOrEmpty(row["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(row["TotalStock"]),
                        Cancle = row["Cancel"].ToString(),
                        ProjectNo = row["ProjectNo"].ToString(),
                        ProjectYearCode = string.IsNullOrEmpty(row["ProjectYearCode"].ToString()) ? 0 : Convert.ToInt32(row["ProjectYearCode"]),
                        CostCenterId = string.IsNullOrEmpty(row["CostCenterId"].ToString()) ? 0 : Convert.ToInt32(row["CostCenterId"]),
                        CostCenterName = row["CostCenterName"].ToString(),
                        ItemLocation = row["ItemLocation"].ToString(),
                        ItemBinRackNo = row["ItemBinRackNo"].ToString(),
                        ItemSize = row["ItemSize"].ToString(),
                    });
                }
                model.ReqDetailGrid = ItemGrid;
            }

            return model;
        }
        
        public async Task<ResponseResult> GetDashboardData(string Fromdate, string Todate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (Flag == "True")
                {
                    //DateTime FromDt = DateTime.Parse(Fromdate, CultureInfo.InvariantCulture);
                    //DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    //DateTime fromDT = new DateTime();
                    //fromDT = ParseDate(Fromdate);

                    //DateTime toDT = new DateTime();
                    //toDT = ParseDate(Todate);


                    var fromDT = CommonFunc.ParseFormattedDate(Fromdate);
                    var toDT = CommonFunc.ParseFormattedDate(Todate);
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDT == default ? string.Empty : fromDT));
                    SqlParams.Add(new SqlParameter("@ToDate", toDT == default ? string.Empty : toDT));
                }
                else
                {
                    DateTime fromDT = new DateTime();
                    fromDT = ParseDate(Fromdate);

                    DateTime toDT = new DateTime();
                    toDT = ParseDate(Todate);
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDT == default ? string.Empty : fromDT));
                    SqlParams.Add(new SqlParameter("@ToDate", toDT == default ? string.Empty : toDT));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetNewEntry(string Flag, int yearCode, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_RequisitionWithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDept()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDEPT"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_RequisitionWithoutBOM", SqlParams);
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
