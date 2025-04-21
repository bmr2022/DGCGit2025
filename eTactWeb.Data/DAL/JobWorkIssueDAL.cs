using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;
 
using static eTactWeb.DOM.Models.JobWorkIssueModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class JobWorkIssueDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

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
        public JobWorkIssueDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Job Work Issue"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkIssue", SqlParams);
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

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkIssue", SqlParams);
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
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkIssue", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "BranchList";
                    _ResponseResult.Result.Tables[1].TableName = "VendorList";
                    _ResponseResult.Result.Tables[2].TableName = "ProcessList";
                    _ResponseResult.Result.Tables[3].TableName = "EmployeeList";
                    _ResponseResult.Result.Tables[4].TableName = "StoreList";
                    _ResponseResult.Result.Tables[5].TableName = "ScrapList";

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
        public async Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetPrevQty"));
                SqlParams.Add(new SqlParameter("@entryid", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@uniquebatchno", uniqueBatchNo));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@TransDate", TillDate));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@BatchNo", BatchNo));
                SqlParams.Add(new SqlParameter("@uniquebatchno", UniqueBatchNo));

                Result = await _IDataLogic.ExecuteDataTable("GetItemRate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", CommonFunc.ParseFormattedDate(Constants.FYStartDate.ToString("dd/MM/yyyy"))));
                SqlParams.Add(new SqlParameter("@Todate", CommonFunc.ParseFormattedDate(Constants.FYEndDate.ToString("dd/MM/yyyy"))));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<JWIssQDashboard> GetDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new JWIssQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_JobworkIssue", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
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
                    model.JWIssQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new JWIssueDashboard
                                             {
                                                 ChallanNo = dr["ChallanNo"].ToString(),
                                                 ChallanDate = dr["ChallanDate"].ToString(),
                                                 VendorName = dr["VendorName"].ToString(),
                                                 DeliveryAdd = dr["DeliveryAdd"].ToString(),
                                                 VendorStateCode = dr["VendorStateCode"].ToString(),
                                                 BOMIND = dr["BOMIND"].ToString(),
                                                 Closed = dr["Closed"].ToString(),
                                                 CompletlyReceive = dr["CompletlyReceive"].ToString(),
                                                 Remarks = dr["Remarks"].ToString(),
                                                 TolApprVal = Convert.ToDecimal(dr["TolApprVal"].ToString()),
                                                 TotalWt = Convert.ToDecimal(dr["TotalWt"]),
                                                 EntryId = dr["EntryId"].ToString(),
                                                 YearCode = Convert.ToInt32(dr["YearCode"]),
                                                 EnteredBy = dr["EnteredBy"].ToString(),
                                                 UpdatedBy = dr["UpdatedBy"].ToString(),
                                                 EntryDate = dr["EntryDate"].ToString(),
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
        public async Task<JWIssQDashboard> GetDetailDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new JWIssQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_JobworkIssue", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "DETAILSEARCH");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
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
                    model.JWIssQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new JWIssueDashboard
                                             {
                                                 ChallanNo = dr["ChallanNo"].ToString(),
                                                 ChallanDate = dr["ChallanDate"].ToString(),
                                                 VendorName = dr["VendorName"].ToString(),
                                                 DeliveryAdd = dr["DeliveryAdd"].ToString(),
                                                 VendorStateCode = dr["VendorStateCode"].ToString(),
                                                 BOMIND = dr["BOMIND"].ToString(),
                                                 Closed = dr["Closed"].ToString(),
                                                 CompletlyReceive = dr["CompletlyReceive"].ToString(),
                                                 Remarks = dr["Remarks"].ToString(),
                                                 TolApprVal = Convert.ToDecimal(dr["TolApprVal"].ToString()),
                                                 TotalWt = Convert.ToDecimal(dr["TotalWt"]),
                                                 EntryId = dr["EntryId"].ToString(),
                                                 YearCode = Convert.ToInt32(dr["YearCode"]),
                                                 EnteredBy = dr["EnteredBy"].ToString(),                                                 
                                                 UpdatedBy = dr["UpdatedBy"].ToString(),
                                                 ItemName = dr["item_name"].ToString(),
                                                 PartCode = dr["partcode"].ToString(),
                                                 Types = dr["Types"].ToString(),
                                                 TimeOfRemoval = dr["timeofremoval"].ToString(),
                                                 JobWorkNewRework = dr["JobWorkNewRework"].ToString(),
                                                 TransporterName = dr["transpoterName"].ToString(),
                                                 VehicleNo = dr["vehicleno"].ToString(),
                                                 DispatchTo = dr["Dispatchto"].ToString(),
                                                 CC = dr["cc"].ToString(),
                                                 HsnNo = dr["HSNNO"].ToString(),
                                                 Unit = dr["unit"].ToString(),
                                                 Store = dr["Store"].ToString(),
                                                 StageDescription = dr["StageDescription"].ToString(),
                                                 ScrapPartCode = dr["ScrapPartCode"].ToString(),
                                                 ScrapItemName = dr["ScrapitemName"].ToString(),
                                                 AltUnit = dr["altUnit"].ToString(),
                                                 IssQty = string.IsNullOrEmpty(dr["IssQty"].ToString()) ? 0 : Convert.ToSingle(dr["IssQty"].ToString()),
                                                 Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToSingle(dr["Rate"].ToString()),
                                                 PurchasePrice = string.IsNullOrEmpty(dr["PurchasePrice"].ToString()) ? 0 : Convert.ToSingle(dr["PurchasePrice"].ToString()),
                                                 PendQty = string.IsNullOrEmpty(dr["pendqty"].ToString()) ? 0 : Convert.ToSingle(dr["pendqty"].ToString()),
                                                 RecScrapQty = string.IsNullOrEmpty(dr["RecScrapQty"].ToString()) ? 0 : Convert.ToSingle(dr["RecScrapQty"].ToString()),
                                                 AltQty = string.IsNullOrEmpty(dr["AltQty"].ToString()) ? 0 : Convert.ToSingle(dr["AltQty"].ToString()),
                                                 PendAltQty = string.IsNullOrEmpty(dr["PendAltQty"].ToString()) ? 0 : Convert.ToSingle(dr["PendAltQty"].ToString()),
                                                 EntryDate = dr["EntryDate"].ToString(),
                                                 UpdatedDate = dr["UpdatedDate"].ToString(),
                                                 EntryByMachineName = dr["EntryByMachineName"].ToString(),
                                                 RecQty= Convert.ToDecimal(dr["RecQty"].ToString()),
                                                 RecItemName= dr["RecItemName"].ToString()

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
        public async Task<JobWorkIssueModel> GetViewByID(int ID, int YearCode)
        {
            var model = new JobWorkIssueModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkIssue", SqlParams);

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
        private static JobWorkIssueModel PrepareView(DataSet DS, ref JobWorkIssueModel? model)
        {
            var ItemList = new List<JobWorkGridDetail>();
            var TaxList = new List<TaxModel>();
            DS.Tables[0].TableName = "SSMain";
            DS.Tables[1].TableName = "SSDetail";
            DS.Tables[2].TableName = "TaxDetail";
            int cnt = 1;


            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.JWChallanNo = DS.Tables[0].Rows[0]["JWChallanNo"].ToString().Trim();
            model.JobWorkNewRework = DS.Tables[0].Rows[0]["JobWorkNewRework"].ToString().Trim();
            model.ChallanDate = DS.Tables[0].Rows[0]["ChallanDate"].ToString().Trim();
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
            model.DeliveryAdd = DS.Tables[0].Rows[0]["DeliveryAdd"].ToString();
            model.VendorStateCode = DS.Tables[0].Rows[0]["VendorStateCode"].ToString();
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString().Trim();
            model.TolApprVal = Convert.ToDecimal(DS.Tables[0].Rows[0]["TolApprVal"]);
            model.TotalWt = Convert.ToInt32(DS.Tables[0].Rows[0]["TotalWt"]);
            model.BomStatus = DS.Tables[0].Rows[0]["BomStatus"].ToString();
            model.Types = DS.Tables[0].Rows[0]["Types"].ToString();
            model.GstType = DS.Tables[0].Rows[0]["Gsttype"].ToString().Trim();
            model.EnteredByEmpid = Convert.ToInt32(DS.Tables[0].Rows[0]["EnterdByEmpid"]);
            model.CompletlyReceive = DS.Tables[0].Rows[0]["CompletlyReceive"].ToString();
            model.timeofremoval = DS.Tables[0].Rows[0]["timeofremoval"].ToString();
            model.Processdays = Convert.ToInt16(DS.Tables[0].Rows[0]["Processdays"]);
            model.TransporterName = DS.Tables[0].Rows[0]["transpoterName"].ToString();
            model.Distance = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["Distance"].ToString()) ? 0 : Convert.ToSingle(DS.Tables[0].Rows[0]["Distance"]);
            model.VehicleNo = DS.Tables[0].Rows[0]["vehicleno"].ToString();
            model.DispatchTo = DS.Tables[0].Rows[0]["Dispatchto"].ToString().Trim();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]);
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString().Trim();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());

            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.CreatedByName = DS.Tables[0].Rows[0]["CreatedByName"].ToString();
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
                    ItemList.Add(new JobWorkGridDetail
                    {
                        SeqNo = Convert.ToInt32(row["seqno"]),
                        SeqForBatch = Convert.ToInt32(row["seqno"]),
                        PartCode = row["partcode"].ToString().Trim(),
                        ItemName = row["ItemNamePartCode"].ToString().Trim(),
                        PONO = row["PONo"].ToString().Trim(),
                        POYear = Convert.ToInt32(row["PoYear"].ToString()),
                        SchNo = row["schno"].ToString().Trim(),
                        SchYear = Convert.ToInt32(row["SchYear"]),
                        SchDate = row["SchDate"].ToString(),
                        OtherInstruction = row["OtherInstruction"].ToString(),
                        PODate = row["PODate"].ToString(),
                        Unit = row["unit"].ToString().Trim(),
                        Rate = Convert.ToDecimal(row["Rate"].ToString()),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        HSNNo = Convert.ToInt32(row["HSNNO"].ToString()),
                        IssQty = Convert.ToDecimal(row["IssQty"].ToString()),
                        Amount = Convert.ToDecimal(row["Amount"].ToString()),
                        RemarkDetail = row["RemarkDetail"].ToString().Trim(),
                        PurchasePrice = Convert.ToDecimal(row["PurchasePrice"]),
                        ProcessId = Convert.ToInt32(row["ProcessId"].ToString()),
                        BatchNo = row["BatchNo"].ToString().Trim(),
                        UniqueBatchNo = row["uniquebatchno"].ToString().Trim(),
                        StockQty = Convert.ToDecimal(row["StockQty"]),
                        BatchStockQty = Convert.ToDecimal(row["BatchStockQty"]),
                        StoreId = Convert.ToInt32(row["StoreId"].ToString().Trim()),
                        StoreName = row["Store"].ToString().Trim(),
                        Closed = row["Closed"].ToString(),
                        pendqty = Convert.ToDecimal(row["pendqty"]),
                        RecScrapCode = Convert.ToInt32(row["RecSrapCode"]),
                        RecScrapPartCode = row["ScrapPartCode"].ToString().Trim(),
                        RecScrapItemName = row["ScrapItemNamePartCode"].ToString(),
                        RecScrapQty = Convert.ToDecimal(row["RecScrapQty"].ToString()),
                        AltQTy = Convert.ToDecimal(row["AltQty"]),
                        AltUnit = row["altUnit"].ToString().Trim(),
                        PendAltQty = Convert.ToDecimal(row["PendAltQty"]),
                        PendScrapQty = Convert.ToDecimal(row["PendScrapQty"]),
                        RecItemCode = string.IsNullOrEmpty(row["RecItemCode"].ToString()) ? 0 : Convert.ToInt32(row["RecItemCode"].ToString()),
                        RecItemName = string.IsNullOrEmpty(row["RecItemName"].ToString()) ? "" : row["RecItemName"].ToString(),
                        RecPartCode = string.IsNullOrEmpty(row["RecPartCode"].ToString()) ? "" : row["RecPartCode"].ToString(),
                        RecQty = string.IsNullOrEmpty(row["RecQty"].ToString()) ? 0 : Convert.ToDecimal(row["RecQty"].ToString()),
                    });
                }
                model.JobDetailGrid = ItemList;
                model.JobDetailGrid = ItemList.OrderBy(item => item.ItemCode).ThenBy(item => item.SeqNo).ToList();

            }

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    TaxList.Add(new TaxModel
                    {
                        TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                        TxType = row["Type"].ToString(),
                        TxItemCode = Convert.ToInt32(row["ItemCode"]),
                        TxTaxType = Convert.ToInt32(row["taxtypeid"]),
                        TxAccountName =row["TxAccountName"].ToString(),
                        TxAccountCode = Convert.ToInt32(row["TaxAccountCode"]),
                        TxItemName = row["ItemName"].ToString(),
                        TxPartName = row["PartCode"].ToString(),
                        TxTaxTypeName = row["TaxType"].ToString(),
                        //tx= row["Type"].ToString(),
                        TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                        TxAdInTxable = row["AddInTaxable"].ToString(),
                        TxRoundOff = row["RoundOff"].ToString(),
                        TxAmount = Convert.ToDecimal(row["Amount"]),
                        TxRefundable = row["TaxRefundable"].ToString(),
                        TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                        TxRemark = row["Remarks"].ToString()
                    });
                }
                model.TaxDetailGridd = TaxList;
            }

            return model;
        }
        internal async Task<ResponseResult> GetSearchData(JWIssQDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(model.FromDate);
                var toDt = CommonFunc.ParseFormattedDate(model.ToDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
                SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
                SqlParams.Add(new SqlParameter("@PONo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@StartDate",fromDt));
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
        public async Task<ResponseResult> GetStoreTotalStock(string Flag, int ItemCode, int StoreId, string TillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime ChallanDt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", ChallanDt.ToString("yyyy/MM/dd")));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(Flag, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo, string FinFromDate)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime TransDt = DateTime.ParseExact(TransDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@transDate", TransDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinFromDate));

                Result = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetAllItems(string Flag, string SH,string Types)
        {

            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                if (SH == "T")
                {
                    SqlParams.Add(new SqlParameter("@Type", "T"));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Type", "F"));

                }
                SqlParams.Add(new SqlParameter("@itemServAsstes", Types));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetStockQty(string SPNAme, int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime tilldt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", tilldt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo));


                Result = await _IDataLogic.ExecuteDataTable("GETSTORESTOCKBATCHWISE", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillItemsBom(string Flag, string BomStatus,string Types)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@BomStatus", BomStatus));
                SqlParams.Add(new SqlParameter("@itemServAsstes", Types));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillEntryandJWNo(int YearCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillAdditionalFields(string Flag, int AccountCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int POYear, int ItemCode, string ChallanDate)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime ChallanDt = DateTime.ParseExact(ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@PONO", PONO));
                SqlParams.Add(new SqlParameter("@POYearCode", POYear));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@jobworkchallanDate", ChallanDt.ToString("yyyy/MM/dd")));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetAddressDetails(int Code)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAddress"));
                SqlParams.Add(new SqlParameter("@ID", Code));

                Result = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillVendors()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillAccountName"));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        
        public async Task<ResponseResult> FillPartCode(string bomStatus)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
                SqlParams.Add(new SqlParameter("@BomStatus", bomStatus));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        
        public async Task<ResponseResult> FillItem(string bomStatus)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillItem"));
                SqlParams.Add(new SqlParameter("@BomStatus", bomStatus));


                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        
        public async Task<ResponseResult> FillProcess()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillProcess"));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillEmployee()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillEmployee"));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillStore()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));

                Result = await _IDataLogic.ExecuteDataTable("SP_JobworkIssue", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
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
        public async Task<ResponseResult> SaveJobWorkIssue(JobWorkIssueModel model, DataTable JWGrid, DataTable TaxDetailGridd)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var timeofremovalDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                var actentDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                var SqlParams = new List<dynamic>();
                var Types = 'S';
                var BomsStatus = 'I';
                if (model.Types == "Items")
                {
                    Types = 'I';
                }
                if (model.BomStatus == "BOM")
                {
                    BomsStatus = 'B';
                }
                var entDt = "";
                if (model.EntryDate != null)
                {
                    entDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                    //entDt = DateTime.ParseExact(model.EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                //DateTime challanDt = new DateTime();
                var challanDt = "";
                if (model.ChallanDate != null)
                {
                    challanDt = CommonFunc.ParseFormattedDate(model.ChallanDate);
                    //challanDt = DateTime.ParseExact(model.ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                }
                else
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate",  entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@JobWorkNewRework", model.JobWorkNewRework));
                SqlParams.Add(new SqlParameter("@JWChallanNo", model.JWChallanNo));
                SqlParams.Add(new SqlParameter("@ChallanDate", challanDt == default ? string.Empty : challanDt));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@DeliveryAdd", model.DeliveryAdd));
                SqlParams.Add(new SqlParameter("@VendorStateCode", model.VendorStateCode ?? ""));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? ""));
                SqlParams.Add(new SqlParameter("@TolApprVal", model.TolApprVal));
                SqlParams.Add(new SqlParameter("@TotalWt", model.TotalWt));
                SqlParams.Add(new SqlParameter("@BomStatus", BomsStatus));
                SqlParams.Add(new SqlParameter("@Types", Types));
                SqlParams.Add(new SqlParameter("@Gsttype", model.GstType ?? ""));
                SqlParams.Add(new SqlParameter("@EnterdByEmpid", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@timeofremoval", timeofremovalDt));
                SqlParams.Add(new SqlParameter("@Processdays", model.Processdays));
                SqlParams.Add(new SqlParameter("@transpoterName", model.TransporterName ?? ""));
                SqlParams.Add(new SqlParameter("@vehicleno", model.VehicleNo ?? ""));
                SqlParams.Add(new SqlParameter("@Dispatchto", model.DispatchTo ?? ""));
                SqlParams.Add(new SqlParameter("@UId", model.UID));
                SqlParams.Add(new SqlParameter("@NetAmount", model.NetTotal));
                SqlParams.Add(new SqlParameter("@TotalAmount", model.TotalAmtAftrDiscount));
                SqlParams.Add(new SqlParameter("@TotalGstAmt",model.TotalAmtAftrDiscount));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actentDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@Distance", model.Distance));
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EnterByMachineName));

                SqlParams.Add(new SqlParameter("@DTItemGrid", JWGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailGridd));


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
    }
}
