using eTactWeb.Data.Common;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class MirDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        public MirDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Quality"));
                SqlParams.Add(new SqlParameter("@SubMenu", "MRIR"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetOkRecStore(int ItemCode,string ShowAllStore)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillOkRecStore"));
                SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@ShowAllStore", ShowAllStore));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMIR", SqlParams);
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
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                //DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                var currDt = CommonFunc.ParseFormattedDate(currentDate.ToString("dd/MM/yyyy"));
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var firstDt = CommonFunc.ParseFormattedDate(firstDateOfMonth.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", firstDt));
                SqlParams.Add(new SqlParameter("@Todate", currDt));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal async Task<MIRQDashboard> GetSearchData(string VendorName, string MrnNo, string GateNo, string MirNo, string ItemName, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MIRQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMIR", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", "SEARCH");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MrnNo);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@MIRNo", MirNo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
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
                    model.MIRQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new MIRDashboard
                                           {
                                               VendorName = dr["VendorName"].ToString(),
                                               MIRNo = dr["MIRNo"].ToString(),
                                               MIRDate = dr["MIRDate"].ToString(),
                                               MRNNo = dr["MRNNO"].ToString(),
                                               MRNDate = dr["MRNDate"].ToString(),
                                               GateNo = dr["GateNo"].ToString(),
                                               InvNo = dr["INVNo"].ToString(),
                                               InvDate = dr["Invdate"].ToString(),
                                               MRNJWCustJW = dr["MRNJWCustJW"].ToString(),
                                               PurchaseBillBooked = dr["PurchaseBillBooked"].ToString(),
                                               MaterialIssued = dr["MaterialIssued"].ToString(),
                                               EntryId = Convert.ToInt32(dr["EntryId"]),
                                               YearCode = Convert.ToInt32(dr["YearCode"]),
                                               EnteredBy = dr["EnteredBy"].ToString(),
                                               UpdatedBy = dr["UpdatedBy"].ToString(),

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
        internal async Task<MIRQDashboard> GetDashboardDetailData(string VendorName, string MrnNo, string GateNo, string MirNo, string ItemName, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MIRQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMIR", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MrnNo);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@MIRNo", MirNo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
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
                    model.MIRQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new MIRDashboard
                                           {
                                               VendorName = dr["VendorName"].ToString(),
                                               MIRNo = dr["MIRNo"].ToString(),
                                               MIRDate = dr["MIRDate"].ToString(),
                                               MRNNo = dr["MRNNO"].ToString(),
                                               MRNDate = dr["MRNDate"].ToString(),
                                               GateNo = dr["GateNo"].ToString(),
                                               InvNo = dr["INVNo"].ToString(),
                                               InvDate = dr["Invdate"].ToString(),
                                               MRNJWCustJW = dr["MRNJWCustJW"].ToString(),
                                               PurchaseBillBooked = dr["PurchaseBillBooked"].ToString(),
                                               MaterialIssued = dr["MaterialIssued"].ToString(),
                                               EntryId = Convert.ToInt32(dr["EntryId"]),
                                               YearCode = Convert.ToInt32(dr["YearCode"]),
                                               EnteredBy = dr["EnteredBy"].ToString(),
                                               UpdatedBy = dr["UpdatedBy"].ToString(),
                                               partcode = string.IsNullOrEmpty(dr["partcode"].ToString()) ? "" : dr["partcode"].ToString(),
                                               item_name = string.IsNullOrEmpty(dr["item_name"].ToString()) ? "" : dr["item_name"].ToString(),
                                               BillQty = string.IsNullOrEmpty(dr["BillQty"].ToString()) ? 0 : Convert.ToDecimal(dr["BillQty"].ToString()),
                                               RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"].ToString()),
                                               AcceptedQty = string.IsNullOrEmpty(dr["AcceptedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AcceptedQty"].ToString()),
                                               AltAcceptedQty = string.IsNullOrEmpty(dr["AltAcceptedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltAcceptedQty"].ToString()),
                                               OkRecStore = string.IsNullOrEmpty(dr["OkRecStore"].ToString()) ? 0 : Convert.ToInt32(dr["OkRecStore"].ToString()),
                                               RejectedQty = string.IsNullOrEmpty(dr["RejectedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RejectedQty"].ToString()),
                                               AltRejectedQty = string.IsNullOrEmpty(dr["AltRejectedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRejectedQty"].ToString()),
                                               RejRecStore = string.IsNullOrEmpty(dr["RejRecStore"].ToString()) ? 0 : Convert.ToInt32(dr["RejRecStore"].ToString()),
                                               HoldQty = string.IsNullOrEmpty(dr["HoldQty"].ToString()) ? 0 : Convert.ToDecimal(dr["HoldQty"].ToString()),
                                               Reworkqty = string.IsNullOrEmpty(dr["Reworkqty"].ToString()) ? 0 : Convert.ToDecimal(dr["Reworkqty"].ToString()),
                                               DeviationQty = string.IsNullOrEmpty(dr["DeviationQty"].ToString()) ? 0 : Convert.ToDecimal(dr["DeviationQty"].ToString()),
                                               ResponsibleEmpForDeviation = string.IsNullOrEmpty(dr["ResponsibleEmpForDeviation"].ToString()) ? 0 : Convert.ToInt32(dr["ResponsibleEmpForDeviation"].ToString()),
                                               PONo = string.IsNullOrEmpty(dr["PONo"].ToString()) ? "" : dr["PONo"].ToString(),
                                               POYearCode = string.IsNullOrEmpty(dr["POYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["POYearCode"].ToString()),
                                               SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                               SchYearCode = string.IsNullOrEmpty(dr["SchYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SchYearCode"].ToString()),
                                               Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                               AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                               AltRecQty = string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                               Remarks = string.IsNullOrEmpty(dr["Remarks"].ToString()) ? "" : dr["Remarks"].ToString(),
                                               Defaulttype = string.IsNullOrEmpty(dr["Defaulttype"].ToString()) ? "" : dr["Defaulttype"].ToString(),
                                               ApprovedByEmp = string.IsNullOrEmpty(dr["ApprovedByEmp"].ToString()) ? 0 : Convert.ToInt32(dr["ApprovedByEmp"].ToString()),
                                               Color = string.IsNullOrEmpty(dr["Color"].ToString()) ? "" : dr["Color"].ToString(),
                                               ItemSize = string.IsNullOrEmpty(dr["ItemSize"].ToString()) ? "" : dr["ItemSize"].ToString(),
                                               ResponsibleFactor = string.IsNullOrEmpty(dr["ResponsibleFactor"].ToString()) ? "" : dr["ResponsibleFactor"].ToString(),
                                               SupplierBatchno = string.IsNullOrEmpty(dr["SupplierBatchno"].ToString()) ? "" : dr["SupplierBatchno"].ToString(),
                                               shelfLife = string.IsNullOrEmpty(dr["shelfLife"].ToString()) ? 0 : Convert.ToDecimal(dr["shelfLife"].ToString()),
                                               BatchNo = string.IsNullOrEmpty(dr["BatchNo"].ToString()) ? "" : dr["BatchNo"].ToString(),
                                               uniqueBatchno = string.IsNullOrEmpty(dr["uniqueBatchno"].ToString()) ? "" : dr["uniqueBatchno"].ToString(),
                                               AllowDebitNote = string.IsNullOrEmpty(dr["AllowDebitNote"].ToString()) ? "" : dr["AllowDebitNote"].ToString(),
                                               FilePath = string.IsNullOrEmpty(dr["FilePath"].ToString()) ? "" : dr["FilePath"].ToString(),
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
        public async Task<MirModel> GetViewByID(int ID, int YearCode)
        {
            var model = new MirModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMIR", SqlParams);

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
        private static MirModel PrepareView(DataSet DS, ref MirModel? model)
        {
            //            SELECT distinct   EntryId,YearCode,EntryDate,JobWorkNewRework,JWChallanNo,ChallanDate,AccountCode,  

            //DeliveryAdd,VendorStateCode,Remark,TolApprVal,        

            //UId, CC,  isnull(ActualEntryDate, EntryDate)ActualEntryDate ,ActualEnteredBy  ,VendAddress
            var ItemList = new List<MirDetail>();
            DS.Tables[0].TableName = "SSMain";
            DS.Tables[1].TableName = "SSDetail";
            int cnt = 0;
            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MIREntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MIRYearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["MIREntryDate"].ToString();
            model.MIRNo = DS.Tables[0].Rows[0]["MIRNo"].ToString();
            model.MRNEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNEntryId"].ToString());
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString().Trim();
            model.MRNNo = DS.Tables[0].Rows[0]["MRNNO"].ToString().Trim();
            model.MRNJWCustJW = DS.Tables[0].Rows[0]["MRNJWCustJW"].ToString().Trim();
            model.MIRDate = DS.Tables[0].Rows[0]["MIREntryDate"].ToString();
            model.MRNYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNYearcode"].ToString());
            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["MRNDate"].ToString()))
            {
                model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"].ToString().Trim().Split(" ")[0];
            }
            else
            {

                model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"].ToString().Trim();
            }
            model.GateNo = DS.Tables[0].Rows[0]["GateNo"].ToString().Trim();
            model.GateEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["GateEntryId"].ToString());
            model.GateYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"].ToString().Trim());
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString().Trim());
            model.AccountName = DS.Tables[0].Rows[0]["Account_Name"].ToString().Trim();
            //model.ItemServTypes = DS.Tables[0].Rows[0]["MRNNO"].ToString().Trim();
            model.INVNo = DS.Tables[0].Rows[0]["INVNo"].ToString().Trim();
            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Invdate"].ToString()))
            {
                model.INVDate = DS.Tables[0].Rows[0]["Invdate"].ToString().Trim().Split(" ")[0];
            }
            else
            {

                model.INVDate = DS.Tables[0].Rows[0]["Invdate"].ToString().Trim();
            }
            model.StoreName = DS.Tables[0].Rows[0]["Store_Name"].ToString().Trim();
            model.FromStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["FromStoreId"].ToString().Trim());
            model.ModeOfTransport = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString().Trim();
            model.EntryByMachineName = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString().Trim();

            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["CreatedByName"].ToString();
            model.CreatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new MirDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"]),
                        PartCode = row["PartCode"].ToString().Trim(),
                        ItemName = row["ItemName"].ToString().Trim(),
                        PONo = row["PONo"].ToString().Trim(),
                        POYearCode = Convert.ToInt32(row["POYearCode"].ToString()),
                        SchNo = row["SchNo"].ToString().Trim(),
                        SchYearCode = Convert.ToInt32(row["SchYearCode"].ToString()),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        Unit = row["Unit"].ToString().Trim(),
                        AltUnit = row["AltUnit"].ToString().Trim(),
                        OkRecStoreName = row["OkRecStoreName"].ToString().Trim(),
                        RejRecStoreName = row["RejRecStoreName"].ToString().Trim(),
                        HoldStoreName = row["HoldStoreName"].ToString().Trim(),
                        RewokStoreName = row["RewokStoreName"].ToString().Trim(),
                        BillQty = string.IsNullOrEmpty(row["BillQty"].ToString()) ? 0 : Convert.ToDecimal(row["BillQty"]),
                        RecQty = string.IsNullOrEmpty(row["RecQty"].ToString()) ? 0 : Convert.ToDecimal(row["RecQty"]),
                        ALtRecQty = string.IsNullOrEmpty(row["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(row["AltRecQty"]),
                        AcceptedQty = string.IsNullOrEmpty(row["AcceptedQty"].ToString()) ? 0 : Convert.ToDecimal(row["AcceptedQty"]),
                        AltAcceptedQty = string.IsNullOrEmpty(row["AltAcceptedQty"].ToString()) ? 0 : Convert.ToDecimal(row["AltAcceptedQty"]),
                        OkRecStore = string.IsNullOrEmpty(row["OkRecStore"].ToString()) ? 0 : Convert.ToInt32(row["OkRecStore"]),
                        DeviationQty = string.IsNullOrEmpty(row["DeviationQty"].ToString()) ? 0 : Convert.ToDecimal(row["DeviationQty"]),
                        ResponsibleEmpForDeviation = string.IsNullOrEmpty(row["ResponsibleEmpForDeviation"].ToString()) ? 0 : Convert.ToInt32(row["ResponsibleEmpForDeviation"]),
                        RejectedQty = string.IsNullOrEmpty(row["RejectedQty"].ToString()) ? 0 : Convert.ToDecimal(row["RejectedQty"]),
                        AltRejectedQty = string.IsNullOrEmpty(row["AltRejectedQty"].ToString()) ? 0 : Convert.ToDecimal(row["AltRejectedQty"]),
                        RejRecStore = string.IsNullOrEmpty(row["RejRecStore"].ToString()) ? 0 : Convert.ToInt32(row["RejRecStore"]),
                        Remarks = row["Remarks"].ToString().Trim(),
                        DefaultType = row["Defaulttype"].ToString().Trim(),
                        ApprovedByEmp = string.IsNullOrEmpty(row["ApprovedByEmp"].ToString()) ? 0 : Convert.ToInt32(row["ApprovedByEmp"]),
                        HoldQty = string.IsNullOrEmpty(row["HoldQty"].ToString()) ? 0 : Convert.ToDecimal(row["HoldQty"]),
                        HoldStoreId = string.IsNullOrEmpty(row["HoldStoreId"].ToString()) ? 0 : Convert.ToInt32(row["HoldStoreId"]),
                        ProcessId = string.IsNullOrEmpty(row["ProcessId"].ToString()) ? 0 : Convert.ToInt32(row["ProcessId"]),
                        ReworkQty = string.IsNullOrEmpty(row["Reworkqty"].ToString()) ? 0 : Convert.ToDecimal(row["Reworkqty"]),
                        RewokStoreId = string.IsNullOrEmpty(row["RewokStoreId"].ToString()) ? 0 : Convert.ToInt32(row["RewokStoreId"]),
                        Color = row["Color"].ToString().Trim(),
                        ItemSize = row["ItemSize"].ToString().Trim(),
                        ResponsibleFactor = row["ResponsibleFactor"].ToString().Trim(),
                        SupplierBatchNo = row["SupplierBatchNo"].ToString().Trim(),
                        ShelfLife = string.IsNullOrEmpty(row["shelfLife"].ToString()) ? 0 : Convert.ToInt32(row["shelfLife"]),
                        BatchNo = row["BatchNo"].ToString().Trim(),
                        UniqueBatchNo = row["uniqueBatchno"].ToString().Trim(),
                        AllowDebitNote = row["AllowDebitNote"].ToString().Trim(),
                        Rate = string.IsNullOrEmpty(row["Rate"].ToString()) ? 0 : Convert.ToDecimal(row["Rate"]),
                        RateInOtherCurr = string.IsNullOrEmpty(row["rateinother"].ToString()) ? 0 : Convert.ToDecimal(row["rateinother"]),
                        PODate = row["PODate"].ToString().Trim(),
                        //Itemcolor= row["ItemColor"].ToString().Trim(),
                        PathOfFile = row["FilePath"].ToString().Trim(),
                    });
                }
                model.ItemDetail = ItemList;
                model.ItemDetail = ItemList.OrderBy(x => x.SeqNo).ToList();

                model.ItemDetailGrid = (IList<MirDetail>?)ItemList;
                //model.JobDetailGrid = (IList<JobWorkIssueModel.JobWorkGridDetail>)ItemList;

            }
            return model;
        }
        internal async Task<ResponseResult> GetSearchData(MIRQDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(model.FromDate);
                var toDt = CommonFunc.ParseFormattedDate(model.ToDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
                SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
                SqlParams.Add(new SqlParameter("@PONo", model.PONo));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@StartDate", fromDt));
                SqlParams.Add(new SqlParameter("@EndDate", toDt));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
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
        public async Task<ResponseResult> AllowUpdelete(int EntryId, string YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BEFOREUPDATE"));
                SqlParams.Add(new SqlParameter("@EntryId", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckEditOrDelete(int EntryId, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
                SqlParams.Add(new SqlParameter("@EntryId", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMRNNo(string Flag, string SPName, string FromDate, string ToDate, string MRNCustJW)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@Todate", toDt));
                SqlParams.Add(new SqlParameter("@MRNJWCustJW", MRNCustJW));
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


        public async Task<ResponseResult> AddPassWord()
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                
                SqlParams.Add(new SqlParameter("@Flag", "ALLOWSHOWALLSTORE"));
               
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMRNData(string Flag, string SPName, string MRNNo, int MRNYear, int GateNo, int GateYear, int GateEntryId, string MRNCustJW)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@MRNNo", MRNNo));
                SqlParams.Add(new SqlParameter("@MRNYearCode", MRNYear));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYear));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                SqlParams.Add(new SqlParameter("@MRNJWCustJW", MRNCustJW));
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
        public async Task<MirModel> GetMIRMainItem(string Flag, string SPName, string MRNNo, int MRNYear, int GateNo, int GateYear, int GateEntryId, string MRNCustJW)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MirModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMIR", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@MRNNo", MRNNo);
                    oCmd.Parameters.AddWithValue("@MRNYearCode", MRNYear);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@GateYearCode", GateYear);
                    oCmd.Parameters.AddWithValue("@GateEntryId", GateEntryId);
                    oCmd.Parameters.AddWithValue("@MRNJWCustJW", MRNCustJW);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    int count = 1;
                    model.ItemDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                        select new MirDetail
                                        {
                                            SeqNo = string.IsNullOrEmpty(dr["SeqNo"].ToString()) ? 0 : Convert.ToInt32(dr["SeqNo"].ToString()),
                                            PONo = dr["PONo"].ToString() ?? "",
                                            POYearCode = string.IsNullOrEmpty(dr["POYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["POYearCode"].ToString()),
                                            SchNo = dr["SchNo"].ToString() ?? "",
                                            SchYearCode = string.IsNullOrEmpty(dr["SchYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SchYearCode"].ToString()),
                                            PoType = dr["PoType"].ToString() ?? "",
                                            PODate = dr["PODate"].ToString() ?? "",
                                            ItemCode = string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["ItemCode"].ToString()),
                                            ItemName = dr["Item_Name"].ToString() ?? "",
                                            PartCode = dr["PartCode"].ToString() ?? "",
                                            Unit = dr["unit"].ToString() ?? "",
                                            RateUnit = dr["RateUnit"].ToString() ?? "",
                                            AltUnit = dr["altunit"].ToString() ?? "",
                                            NoofCase = string.IsNullOrEmpty(dr["NoofCase"].ToString()) ? 0 : Convert.ToInt32(dr["NoofCase"].ToString()),
                                            BillQty = string.IsNullOrEmpty(dr["BillQty"].ToString()) ? 0 : Convert.ToDecimal(dr["BillQty"].ToString()),
                                            RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"].ToString()),
                                            AcceptedQty = string.IsNullOrEmpty(dr["AcceptedQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AcceptedQty"].ToString()),
                                            ALtRecQty = string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                            //Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                            RateInOtherCurr = string.IsNullOrEmpty(dr["RateInOtherCurr"].ToString()) ? 0 : Convert.ToDecimal(dr["RateInOtherCurr"].ToString()),
                                            Amount = string.IsNullOrEmpty(dr["Amount"].ToString()) ? 0 : Convert.ToDecimal(dr["Amount"].ToString()),
                                            PendPOQty = string.IsNullOrEmpty(dr["PendPOQty"].ToString()) ? 0 : Convert.ToDecimal(dr["PendPOQty"].ToString()),
                                            QCCompleted = dr["QCCompleted"].ToString() ?? "",
                                            RetChallanPendQty = string.IsNullOrEmpty(dr["RetChallanPendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RetChallanPendQty"].ToString()),
                                            BatchWise = dr["BatchWise"].ToString() ?? "",
                                            SaleBillNo = dr["SaleBillNo"].ToString() ?? "",
                                            SaleBillYearCode = string.IsNullOrEmpty(dr["SaleBillYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SaleBillYearCode"].ToString()),
                                            AgainstChallanNo = dr["AgainstChallanNo"].ToString() ?? "",
                                            BatchNo = dr["Batchno"].ToString() ?? "",
                                            UniqueBatchNo = dr["Uniquebatchno"].ToString() ?? "",
                                            SupplierBatchNo = dr["SupplierBatchNo"].ToString() ?? "",
                                            ShelfLife = string.IsNullOrEmpty(dr["ShelfLife"].ToString()) ? 0 : Convert.ToInt32(dr["ShelfLife"].ToString()),
                                            ItemSize = dr["ItemSize"].ToString() ?? "",
                                            Itemcolor = dr["ItemColor"].ToString() ?? "",
                                            MirTotalRows = oDataSet.Tables[0].Rows.Count
                                            //IssueToStore= dr["IssueToStore"].ToString() ?? "",
                                            //batchqty= string.IsNullOrEmpty(dr["batchqty"].ToString()) ? 0 : Convert.ToDecimal(dr["batchqty"].ToString()),
                                            //batchnotype= dr["bathnotype"].ToString() ?? "",
                                            //number= string.IsNullOrEmpty(dr["number"].ToString()) ? 0 : Convert.ToInt32(dr["number"].ToString()),
                                        }).OrderBy(x => x.SeqNo) // 👈 Ordering here
                    .ToList();
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
        public async Task<ResponseResult> GetMIRFromPend(string Flag, string SPName, string MRNNo, int MRNYear, string MRNCustJW)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@MRNNo", MRNNo));
                SqlParams.Add(new SqlParameter("@MRNYearCode", MRNYear));
                SqlParams.Add(new SqlParameter("@MRNJWCustJW", MRNCustJW));
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
        public async Task<ResponseResult> GetStore(string Flag, string SPName)
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
        public async Task<ResponseResult> GetRewStore(string Flag, string SPName)
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
        public async Task<ResponseResult> GetHoldStore(string Flag, string SPName)
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
        public async Task<ResponseResult> GetRecOkStore(int ItemCode,string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
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
        public async Task<ResponseResult> GetGateData(string Flag, string SPName, string MRNNo, string MRNYearCode, string MRNCustJW)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@MRNNo", MRNNo));
                SqlParams.Add(new SqlParameter("@MRNYearCode", MRNYearCode));
                SqlParams.Add(new SqlParameter("@MRNJWCustJW", MRNCustJW));
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
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
                SqlParams.Add(new SqlParameter("@EntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMIR", SqlParams);
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
        public async Task<ResponseResult> SaveMIR(MirModel model, DataTable MIRGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
               // DateTime mirDt = new DateTime();
               // DateTime mrnDt = new DateTime();
               // DateTime invDt = new DateTime();
                // EntryDate = new DateTime();


               var mirDt = CommonFunc.ParseFormattedDate(model.MIRDate);
               var mrnDt = CommonFunc.ParseFormattedDate(model.MRNDate);
               var invDt = CommonFunc.ParseFormattedDate(model.INVDate);
               var EntryDate = CommonFunc.ParseFormattedDate(model.EntryDate);


                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                //SqlParams.Add(new SqlParameter("@YearCode", DateTime.Now.Year));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", mirDt == default ? string.Empty : mirDt));
                SqlParams.Add(new SqlParameter("@MIRNo", model.MIRNo == null ? "" : model.MIRNo));
                SqlParams.Add(new SqlParameter("@MIRDate", mirDt == default ? string.Empty : mirDt));
                SqlParams.Add(new SqlParameter("@MRNJWCustJW", model.MRNJWCustJW == null ? "" : model.MRNJWCustJW));
                SqlParams.Add(new SqlParameter("@MRNYearcode", model.MRNYearCode));
                SqlParams.Add(new SqlParameter("@MRNNO", model.MRNNo == null ? "" : model.MRNNo));
                SqlParams.Add(new SqlParameter("@MRNDate", mrnDt == default ? string.Empty : mrnDt));
                SqlParams.Add(new SqlParameter("@GateNo", model.GateNo == null ? "" : model.GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@INVNo", model.INVNo == null ? "11" : model.INVNo));
                SqlParams.Add(new SqlParameter("@Invdate", invDt == default ? string.Empty : invDt));
                SqlParams.Add(new SqlParameter("@QcType", model.QcType == null ? "" : model.QcType));
                SqlParams.Add(new SqlParameter("@JobWorkEntryId", model.JobWorkEntryId));
                SqlParams.Add(new SqlParameter("@FromStoreId", model.FromStoreId));
                SqlParams.Add(new SqlParameter("@ModeOfTransport", model.ModeOfTransport == null ? "" : model.ModeOfTransport));
                SqlParams.Add(new SqlParameter("@HoldQC", model.HoldQC == null ? "" : model.HoldQC));
                SqlParams.Add(new SqlParameter("@EnteredEMPID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@UID", model.Uid));
                SqlParams.Add(new SqlParameter("@CC", model.CC == null ? "" : model.CC));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.CreatedOn));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName == null ? "" : model.EntryByMachineName));
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                //SqlParams.Add(new SqlParameter("@UpdatedOn", model.UpdatedOn== null ? "" : model.UpdatedOn));

                SqlParams.Add(new SqlParameter("@DTSSGrid", MIRGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMIR", SqlParams);
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
