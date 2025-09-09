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
using static eTactWeb.Data.Common.CommonFunc;
using System.Reflection;

namespace eTactWeb.Data.DAL
{
    public class TranferFromWorkCenterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public TranferFromWorkCenterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);

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
                SqlParams.Add(new SqlParameter("@MainMenu", "Transfer from WC to WC"));
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
        public async Task<ResponseResult> ChkWIPStockBeforeSaving(int WcId,string TransferMatEntryDate,int TransferMatYearCode,int TransferMatEntryId, DataTable TransferGrid,string Mode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ChkWIPStockBeforeSaving"));
                SqlParams.Add(new SqlParameter("@WCId", WcId));
                SqlParams.Add(new SqlParameter("@TransferMatEntrydate", ParseFormattedDate(TransferMatEntryDate)));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", TransferMatYearCode));
                SqlParams.Add(new SqlParameter("@TransferMatEntryId", TransferMatEntryId));
                SqlParams.Add(new SqlParameter("@DTItemGrid", TransferGrid));
                SqlParams.Add(new SqlParameter("@Mode", Mode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<TransferFromWorkCenterModel> GetViewByID(int ID, int YearCode)
        {
            var model = new TransferFromWorkCenterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@TransferMatEntryId", ID));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_TransferMaterialFromWc", SqlParams);

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
        private static TransferFromWorkCenterModel PrepareView(DataSet DS, ref TransferFromWorkCenterModel? model)
        {
            var TransferFromWorkCenterDetail = new List<TransferFromWorkCenterDetail>();
            DS.Tables[0].TableName = "TransferFromWc";
            DS.Tables[1].TableName = "TransferFromWcDetail";
            int cnt = 0;
            model.TransferMatEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["TransferMatEntryId"].ToString());
            model.TransferMatYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["TransferMatYearCode"].ToString());
            model.TransferMatEntrydate = DS.Tables[0].Rows[0]["TransferMatEntrydate"].ToString();
            model.TransferMatSlipNo = DS.Tables[0].Rows[0]["TransferMatSlipNo"].ToString();
            model.TransferMatSlipDate = DS.Tables[0].Rows[0]["TransferMatSlipDate"].ToString();
            model.PRODSTATUSProdUnProdRej = DS.Tables[0].Rows[0]["PRODSTATUSProdUnProdRej"].ToString();
            model.IssueToStoreWC=DS.Tables[0].Rows[0]["IssueToStoreWC"].ToString();
            model.IssueFromWCid=Convert.ToInt32(DS.Tables[0].Rows[0]["IssueFromWCid"].ToString());
            model.IssueTOWCid = Convert.ToInt32(DS.Tables[0].Rows[0]["IssueTOWCid"].ToString());
            model.IssueToStoreId=Convert.ToInt32(DS.Tables[0].Rows[0]["IssueToStoreId"].ToString());
            model.IssuedByEmp=Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedByEmp"].ToString());
            model.RecByEmpId=Convert.ToInt32(DS.Tables[0].Rows[0]["RecByEmpId"].ToString());
            model.Remark=DS.Tables[0].Rows[0]["Remark"].ToString();
            model.PendingToRecByStore=DS.Tables[0].Rows[0]["PendingToRecByStore"].ToString();
            model.Uid=Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.CC=DS.Tables[0].Rows[0]["CC"].ToString();
            model.EntryByMachineNo=DS.Tables[0].Rows[0]["EntryByMachineNo"].ToString();
            model.ActualEnteredBy=Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmpid"].ToString());
            model.ActualEntrydate=string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);
            model.UpdatedBy=Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
            model.UpdatedOn= string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);
            }
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    TransferFromWorkCenterDetail.Add(new TransferFromWorkCenterDetail
                    {
                        TransferMatEntryId = Convert.ToInt32(row["TransferMatEntryId"].ToString()),
                        TransferMatYearCode = Convert.ToInt32(row["TransferMatYearCode"].ToString()),
                        SeqNo = Convert.ToInt32(row["seqno"].ToString()),
                        ProdEntryId = Convert.ToInt32(row["ProdEntryid"].ToString()),
                        ProdSlipNo=row["ProdSlipNo"].ToString(),
                        ProdEntryYearCode = Convert.ToInt32(row["ProdYearCode"].ToString()),
                        ProdDate = row["ProdEntryDate"].ToString(),
                        ProdPlanNo = row["ProdPlanNo"].ToString(),
                        ProdPlanYearCode = Convert.ToInt32(row["ProdPlanYearCode"].ToString()),
                        ProdSchNo = row["ProdSchNo"].ToString(),
                        ProdSchYearCode = Convert.ToInt32(row["ProdSchYearCode"].ToString()),
                        ParentProdSchNo = row["ParentProdSchNo"].ToString(),
                        ParentProdSchYearCode =Convert.ToInt32(row["ParentProdSchYearCode"].ToString()),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        TransferQty = Convert.ToDecimal(row["TransferQty"].ToString()),
                        QcOkQty = Convert.ToDecimal(row["QCOkQty"].ToString()),
                        ProdQty = Convert.ToDecimal(row["ProdQty"].ToString()),
                        Unit = row["Unit"].ToString(),
                        AltTransferQty = Convert.ToDecimal(row["AltTransferQty"].ToString()),
                        ProcessName=row["ProcessName"].ToString(),
                        AltUnit = row["AltUnit"].ToString(),
                        Remark = row["Remark"].ToString(),
                        PendingToAcknowledge = row["PendingToAcknowledge"].ToString(),
                        PendingQtyToAcknowledge = Convert.ToDecimal(row["PendingQtyToAcknowledge"].ToString()),
                        ItemSize = row["ItemSize"].ToString(),
                        ItemColor=row["ItemColor"].ToString(),
                        InProcessQcSlipNo = row["InProcQCSlipNo"].ToString(),
                        InProcessQcEntryId = Convert.ToInt32(row["InProcQCEntryId"].ToString()),
                        QcCleaningDate =row["QCClearingDate"].ToString(),
                        InProcessQcYearCode=Convert.ToInt32(row["InProcQCYearCode"]),
                        ProcessId=Convert.ToInt32(row["ProcessId"].ToString()),
                        TotalStock=Convert.ToDecimal(row["TotalStock"].ToString()),
                        BatchNo=row["BatchNo"].ToString(),
                        UniqueBatchNo=row["uniquebatchno"].ToString(),
                        BatchStock=Convert.ToDecimal(row["BatchStock"].ToString()),
                        ReceivedByStoreQty=Convert.ToDecimal(row["ReceivedByStoreQty"].ToString()),
                        ReceivedCompleted=row["ReceivedCompleted"].ToString(),
                        ReceivedByEmpId=Convert.ToInt32(row["ReceivedByEmpId"].ToString()),
                        Rate=Convert.ToDecimal(row["Rate"].ToString()),
                        ItemWeight=Convert.ToDecimal(row["ItemWeight"].ToString()),
                    });
                }
                model.ItemDetailGrid = TransferFromWorkCenterDetail.OrderBy(x=>x.SeqNo).ToList();
            }
            return model;
        }
        public async Task<ResponseResult> FillEntryandGate(string Flag, int yearCode, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", yearCode));
                SqlParams.Add(new SqlParameter("@TransferMatEntryDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> BindEmpList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BindEmpList"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProcessName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BindProcessName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FillWorkCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName(int TransferMatYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemname"));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", TransferMatYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem, string SearchItemCode, string SearchPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemsOnAssets"));
                
                SqlParams.Add(new SqlParameter("@showAllItem", ShowAllItem));
                SqlParams.Add(new SqlParameter("@SearchItemCode", SearchItemCode ?? ""));
                SqlParams.Add(new SqlParameter("@SearchPartCode", SearchPartCode ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(int TransferMatYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetPartCode"));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", TransferMatYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                string TransDt = CommonFunc.ParseFormattedDate(TransDate);
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@transDate", TransDt));
                SqlParams.Add(new SqlParameter("@WCID", WcId));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));

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
        public async Task<ResponseResult> GetWorkCenterTotalStock(string Flag, int ItemCode, int WcId, string TillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@WCID", WcId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", ParseFormattedDate(TillDate)));
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
        public async Task<ResponseResult> GetWorkCenterQty(string SPNAme, int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@WCID", WcId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", ParseFormattedDate(TillDate)));
                SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo));


                Result = await _IDataLogic.ExecuteDataTable("GETWIPSTOCKBATCHWISE", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetUnit(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Unit"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
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
        public async Task<ResponseResult> CheckEditOrDelete(int TransferEntryId, int TransferYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
                SqlParams.Add(new SqlParameter("@TransferMatEntryId", TransferEntryId));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", TransferYearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTransferFromWorkCenter(TransferFromWorkCenterModel model, DataTable TransferGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                var transDt = "";
                var transSlipDt = "";
                var actualDt = DateTime.Now.ToString("dd/MM/yyyy");
                var lastupdationDt = DateTime.Now.ToString("dd/MM/yyyy");

                transDt = CommonFunc.ParseFormattedDate(model.TransferMatEntrydate);
                transSlipDt= CommonFunc.ParseFormattedDate(model.TransferMatSlipDate);
                actualDt= CommonFunc.ParseFormattedDate(actualDt);
                lastupdationDt= CommonFunc.ParseFormattedDate(lastupdationDt);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", lastupdationDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@TransferMatEntryId", model.TransferMatEntryId==0 ? 0 : model.TransferMatEntryId));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", model.TransferMatYearCode == 0 ? 0 : model.TransferMatYearCode));
                SqlParams.Add(new SqlParameter("@TransferMatEntrydate", transDt));
                SqlParams.Add(new SqlParameter("@TransferMatSlipNo", model.TransferMatSlipNo ?? ""));
                SqlParams.Add(new SqlParameter("@TransferMatSlipDate", transSlipDt));
                SqlParams.Add(new SqlParameter("@PRODSTATUSProdUnProdRej", model.PRODSTATUSProdUnProdRej ?? ""));
                SqlParams.Add(new SqlParameter("@IssueToStoreWC", model.IssueToStoreWC ?? ""));
                SqlParams.Add(new SqlParameter("@IssueFromWCid", model.IssueFromWCid == 0 ? 0 : model.IssueFromWCid));
                SqlParams.Add(new SqlParameter("@IssueTOWCid", model.IssueTOWCid == 0 ? 0 : model.IssueTOWCid));
                SqlParams.Add(new SqlParameter("@IssueToStoreId", model.IssueToStoreId == 0 ? 0.0 : model.IssueToStoreId));
                SqlParams.Add(new SqlParameter("@IssuedByEmp", model.IssuedByEmp== 0 ? 0.0 : model.IssuedByEmp));
                SqlParams.Add(new SqlParameter("@RecByEmpId", model.RecByEmpId == 0 ? 0.0 : model.RecByEmpId));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? ""));
                SqlParams.Add(new SqlParameter("@PendingToRecByStore", model.PendingToRecByStore ?? ""));
                SqlParams.Add(new SqlParameter("@UID", model.Uid == 0 ? 0.0 : model.Uid));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@EntryByMachineNo", model.EntryByMachineNo ?? ""));
                SqlParams.Add(new SqlParameter("@ActualEntryByEmpid", model.ActualEnteredBy == 0 ? 0.0 : model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualDt));

                SqlParams.Add(new SqlParameter("@DTItemGrid", TransferGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //var currentDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var firstDateOfMonthh = CommonFunc.ParseFormattedDate(firstDateOfMonth.ToString()); 
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", firstDateOfMonthh));
                SqlParams.Add(new SqlParameter("@ToDate", currentDate.ToString("yyyy/MM/dd")));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<TransferFromWorkCenterDashboard> GetDashboardData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new TransferFromWorkCenterDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_TransferMaterialFromWc", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
                    oCmd.Parameters.AddWithValue("@TransferMatSlipNo", TransferMatSlipNo);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@transferFromWorkcenter", TransferFromWC);
                    oCmd.Parameters.AddWithValue("@transferToWorkcenter", TransferToWC);
                    oCmd.Parameters.AddWithValue("@ToStoreNmae", TransferToStore);
                    oCmd.Parameters.AddWithValue("@ProdSlipNo", ProdSlipNo);
                    oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.TransferFromWorkCenterDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new TransferFromDashboard
                                                {
                                                    TransferMatSlipNo = dr["TransferMatSlipNo"].ToString(),
                                                    TransferMatEntrydate = dr["TransferMatEntrydate"].ToString().Split(" ")[0],
                                                    TransferMatSlipDate = dr["TransferMatSlipDate"].ToString().Split(" ")[0],
                                                    IssueToStoreWC =dr["IssueToStoreWC"].ToString(),
                                                    TransferFromWC =dr["TransferFromWC"].ToString(),
                                                    TransferToWC =dr["TransferToWC"].ToString(),
                                                    TransferToStore =dr["TransferToStore"].ToString(),
                                                    Remark =dr["Remark"].ToString(),
                                                    PendingToRecByStore =dr["PendingToRecByStore"].ToString(),
                                                    IssuedByEmpName =dr["IssuedByEmpName"].ToString(),
                                                    ActualEntryByEmpName =dr["ActualEntryByEmpName"].ToString(),
                                                    UID = Convert.ToInt32(dr["UID"]),
                                                    CC=dr["CC"].ToString(),
                                                    EntryByMachineNo=dr["EntryByMachineNo"].ToString(),
                                                    ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                    UpdatedByEmpName=dr["UpdatedByEmpName"].ToString(),
                                                    LastUpdationDate=dr["LastUpdationDate"].ToString().Split(" ")[0],
                                                    TransferMatEntryId = Convert.ToInt32(dr["TransferMatEntryId"]),
                                                    TransferMatYearCode = Convert.ToInt32(dr["TransferMatYearCode"]),
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
        public async Task<TransferFromWorkCenterDashboard> GetDashboardDetailData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new TransferFromWorkCenterDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_TransferMaterialFromWc", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
                    oCmd.Parameters.AddWithValue("@TransferMatSlipNo", TransferMatSlipNo);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@transferFromWorkcenter", TransferFromWC);
                    oCmd.Parameters.AddWithValue("@transferToWorkcenter", TransferToWC);
                    oCmd.Parameters.AddWithValue("@ToStoreNmae", TransferToStore);
                    oCmd.Parameters.AddWithValue("@ProdSlipNo", ProdSlipNo);
                    oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.TransferFromWorkCenterDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new TransferFromDashboard
                                                {
                                                    TransferMatSlipNo = dr["TransferMatSlipNo"].ToString(),
                                                    TransferMatEntrydate = dr["TransferMatEntrydate"].ToString().Split(" ")[0],
                                                    TransferMatSlipDate = dr["TransferMatSlipDate"].ToString().Split(" ")[0],
                                                    IssueToStoreWC =dr["IssueToStoreWC"].ToString(),
                                                    TransferFromWC =dr["TransferFromWC"].ToString(),
                                                    TransferToWC =dr["TransferToWC"].ToString(),
                                                    TransferToStore =dr["TransferToStore"].ToString(),
                                                    PartCode=dr["PartCode"].ToString(),
                                                    ItemName=dr["ItemName"].ToString(),
                                                    TransferQty = Convert.ToDecimal(dr["TransferQty"]),
                                                    QCOkQty = Convert.ToDecimal(dr["QCOkQty"]),
                                                    ProdQty = Convert.ToDecimal(dr["ProdQty"]),
                                                    Unit = dr["Unit"].ToString(),
                                                    AltTransferQty = Convert.ToDecimal(dr["AltTransferQty"]),
                                                    AltUnit = dr["AltUnit"].ToString(),
                                                    TotalStock = Convert.ToDecimal(dr["TotalStock"]),
                                                    BatchNo = dr["BatchNo"].ToString(),
                                                    uniquebatchno = dr["uniquebatchno"].ToString(),
                                                    BatchStock = Convert.ToDecimal(dr["BatchStock"]),
                                                    Rate = Convert.ToDecimal(dr["Rate"]),
                                                    ItemWeight = Convert.ToDecimal(dr["ItemWeight"]),
                                                    ItemSize = dr["ItemSize"].ToString(),
                                                    ItemColor = dr["ItemColor"].ToString(),
                                                    ItemRecCompleted = dr["ItemRecCompleted"].ToString(),
                                                    PendingQtyToAcknowledge = Convert.ToDecimal(dr["PendingQtyToAcknowledge"]),
                                                    ItemRemark = dr["ItemRemark"].ToString(),
                                                    ProdEntryId=Convert.ToInt32(dr["ProdEntryId"]),
                                                    ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                    ProdYearCode=Convert.ToInt32(dr["ProdYearCode"]),
                                                    ProdEntryDate = dr["ProdEntryDate"].ToString(),
                                                    ProdPlanNo = dr["ProdPlanNo"].ToString(),
                                                    ProdPlanYearCode=Convert.ToInt32(dr["ProdPlanYearCode"]),
                                                    ProdSchNo = dr["ProdSchNo"].ToString(),
                                                    ProdSchYearCode=Convert.ToInt32(dr["ProdSchYearCode"]),
                                                    IssuedByEmpName =dr["IssuedByEmpName"].ToString(),
                                                    ActualEntryByEmpName =dr["ActualEntryByEmpName"].ToString(),
                                                    UID = Convert.ToInt32(dr["UID"]),
                                                    CC=dr["CC"].ToString(),
                                                    EntryByMachineNo=dr["EntryByMachineNo"].ToString(),
                                                    ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                    UpdatedByEmpName=dr["UpdatedByEmpName"].ToString(),
                                                    LastUpdationDate=dr["LastUpdationDate"].ToString().Split(" ")[0],
                                                    TransferMatEntryId = Convert.ToInt32(dr["TransferMatEntryId"]),
                                                    TransferMatYearCode = Convert.ToInt32(dr["TransferMatYearCode"]),
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
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate,int EmpID)
        {
            var _ResponseResult = new ResponseResult();
            var entrydt = ParseDate(EntryDate);
            string formattedEntryDate = entrydt.ToString("yyyy-MM-dd");
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@TransferMatEntryId", ID));
                SqlParams.Add(new SqlParameter("@TransferMatYearCode", YC));
                SqlParams.Add(new SqlParameter("@cc", CC));
                SqlParams.Add(new SqlParameter("@EntryByMachineNo", EntryByMachineName));
                SqlParams.Add(new SqlParameter("@TransferMatEntrydate", entrydt));
                SqlParams.Add(new SqlParameter("@EnteredEMPID", EmpID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<TransferFromWorkCenterModel> selectMultipleItem(int WCID, string FromDate, string ToDate, string PartCode)
        {
            var resultList = new TransferFromWorkCenterModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportWIPstockRegister", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    

                    command.Parameters.AddWithValue("@Flag", "BATCHWISESTOCKSUMMARY");
                    command.Parameters.AddWithValue("@FormName", "TransferFromWC");
                  
                    command.Parameters.AddWithValue("@WCID", WCID);
                    command.Parameters.AddWithValue("@ToDate", ParseFormattedDate(ToDate));
                    command.Parameters.AddWithValue("@FromDate", ParseFormattedDate(FromDate));
                    command.Parameters.AddWithValue("@PartCode", PartCode);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.ItemDetailGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                      select new TransferFromWorkCenterDetail
                                                      {
                                                          ItemCode = row["item_code"] == DBNull.Value ? 0 : Convert.ToInt32(row["item_code"]),

                                                          PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                          ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),

                                                          BatchNo = row["batchno"] == DBNull.Value ? string.Empty : row["batchno"].ToString(),
                                                          UniqueBatchNo = row["uniquebatchno"] == DBNull.Value ? string.Empty : row["uniquebatchno"].ToString(),


                                                          Unit = row["unit"] == DBNull.Value ? string.Empty : row["unit"].ToString(),
                                                          AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                          
                                                          
                                                          BatchStock = row["BatchStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["BatchStock"]),
                                                          TotalStock = row["TotalStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalStock"]),
                                                          TransferQty = row["BatchStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["BatchStock"]),


                                                      }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data.", ex);
            }

            return resultList;
        }

    }
}
