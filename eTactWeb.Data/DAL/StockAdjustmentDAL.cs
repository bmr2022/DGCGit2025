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
using  eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    internal class StockAdjustmentDAL
    {
        public StockAdjustmentDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }
        public async Task<ResponseResult> StockAdjBackDatePassword()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BackDatePassword"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
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
                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_StockAdjustment", SqlParams);
                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    ResponseResult.Result.Tables[0].TableName = "BranchList";

                    oDataSet = ResponseResult.Result;
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
        public async Task<ResponseResult> SaveStockAdjust(StockAdjustmentModel model, DataTable SAGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                DateTime entDt = new DateTime();
                DateTime stockDt = new DateTime();

                entDt = ParseDate(model.EntryDate);
                stockDt = ParseDate(model.StockAdjustmentDate);


                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedbyEmp", model.UpdatedByEmp));
                    SqlParams.Add(new SqlParameter("@UpdatedOn", DateTime.Now));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@SlipNo", model.SlipNo));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@StockAdjustmentDate", stockDt == default ? string.Empty : stockDt));
                SqlParams.Add(new SqlParameter("@StoreWorkcenter", model.StoreWorkCenter));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@entryByEmp", model.entryByEmp));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.MachineNo));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entDt == default ? string.Empty : entDt));

                SqlParams.Add(new SqlParameter("@DTSSGrid", SAGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_stockadjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public static string ParseFormattedDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return string.Empty;
            }
            DateTime date;
            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "" };

            if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
            {
                return date.ToString("yyyy/MM/dd");
            }
            else
            {
                return string.Empty;
            }
        }
        private string ConvertToDDMMMYYYY(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return null;

            string[] possibleFormats = { "dd/MM/yyyy", "dd-MM-yyyy" }; // ✅ Supports both formats

            if (DateTime.TryParseExact(dateStr, possibleFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("dd/MMM/yyyy");  // ✅ Convert to "dd/MMM/yyyy" format
            }
            else
            {
                Console.WriteLine($"Invalid date format: {dateStr}");
                return null;
            }
        }
        internal async Task<ResponseResult> GetDashboardData(SADashborad model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
              
                var SqlParams = new List<dynamic>();
                var Flag = "";
                if (model.SummaryDetail == "Detail")
                {
                    Flag = "DETAILDASHBOARD";
                }
                else
                {
                    Flag = "DASHBOARD";
                }
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
                SqlParams.Add(new SqlParameter("@StoreWorkcenter", model.StoreWorkcenter));
                SqlParams.Add(new SqlParameter("@StoreName", model.StoreName));
                SqlParams.Add(new SqlParameter("@WorkCenter", model.WorkCenter));
                string formattedFromDate = ConvertToDDMMMYYYY(model.FromDate);
                string formattedToDate = ConvertToDDMMMYYYY(model.ToDate);

                SqlParams.Add(new SqlParameter("@StartDate", string.IsNullOrEmpty(formattedFromDate) ? DBNull.Value : (object)formattedFromDate));
                SqlParams.Add(new SqlParameter("@EndDate", string.IsNullOrEmpty(formattedToDate) ? DBNull.Value : (object)formattedToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_stockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GETWIPotalSTOCK(int ItemCode, int WCID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@WCID", WCID));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Now));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETWIPotalSTOCK", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetWIPStockBatchWise(int ItemCode, int WCID, string uniquebatchno, string batchno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@WCID", WCID));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Now));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", uniquebatchno));
                SqlParams.Add(new SqlParameter("@BATCHNO", batchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETWIPSTOCKBATCHWISE", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItems(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemNamePartCode"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetWCName(int Wcid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetWorkCenterName"));
                SqlParams.Add(new SqlParameter("@Wcid", Wcid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetStoreName(int storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetStoreName"));
                SqlParams.Add(new SqlParameter("@Storeid", storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> DeleteByID(int ID, int YC,int entryByEmp,string EntryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@entryByEmp", entryByEmp));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<StockAdjustmentModel> GetViewByID(int ID, string Mode, int YC)
        {
            var model = new StockAdjustmentModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_stockadjustment", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareView(ResponseResult.Result, ref model, Mode);
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

        private static StockAdjustmentModel PrepareView(DataSet DS, ref StockAdjustmentModel? model, string Mode)
        {
            var ItemList = new List<StockAdjustmentDetail>();
            DS.Tables[0].TableName = "SAMain";
            DS.Tables[1].TableName = "SADetail";

            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.SlipNo = DS.Tables[0].Rows[0]["SlipNo"].ToString();
            model.StoreWorkCenter = DS.Tables[0].Rows[0]["StoreWorkCenter"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"].ToString());
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            model.entryByEmp = Convert.ToInt32(DS.Tables[0].Rows[0]["entryByEmp"].ToString());
            model.entryByEmpName = DS.Tables[0].Rows[0]["enterByEmpName"].ToString();
            model.MachineNo = DS.Tables[0].Rows[0]["MachineNo"].ToString();
            model.StockAdjustmentDate = DS.Tables[0].Rows[0]["StockAdjustmentDate"].ToString();

            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["UpdatedbyEmp"].ToString() != "")
                {
                    model.UpdatedByEmp = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedbyEmp"].ToString());
                    model.UpdatedByEmpName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
                    model.UpdatedOn = DS.Tables[0].Rows[0]["UpdatedOn"].ToString();
                }
            }


            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new StockAdjustmentDetail
                    {
                        SeqNo = cnt++,
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        Unit = row["Unit"].ToString(),
                        LotStock = Convert.ToInt32(row["LotStock"]),
                        TotalStock = Convert.ToInt32(row["TotalStock"]),
                        altUnit = row["altUnit"].ToString(),
                        AltQty = Convert.ToInt32(row["AltQty"]),
                        ActualStockQty = Convert.ToInt32(row["ActuleStockQty"]),
                        AdjQty = Convert.ToInt32(row["AdjQty"]),
                        AdjType = row["AdjType"].ToString(),
                        Storeid = Convert.ToInt32(row["storeid"]),
                        StoreName = row["Store_Name"].ToString(),
                        Wcid = Convert.ToInt32(row["wcid"].ToString()),
                        WCName = row["WorkCenter"].ToString(),
                        Rate = Convert.ToInt32(row["rate"]),
                        Amount = Convert.ToInt32(row["Amount"].ToString()),
                        batchno = row["batchno"].ToString(),
                        uniqbatchno = row["uniquebatchno"].ToString(),
                        reasonOfAdjustment = row["reasonOfAdjutment"].ToString(),
                    });
                }
                model.StockAdjustModelGrid = ItemList;
            }

            return model;
        }
        public async Task<ResponseResult> FillPartCode(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetmaxStockAdjustDate(string Flag, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> StockAdjustByFeaturesOptions(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillLotStock(int ItemCode, int Storeid, string UniqueBatchNo, string BatchNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", Storeid));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Now));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo ?? ""));
                SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETSTORESTOCKBATCHWISE", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRateAmount(int ItemCode, int YearCode, string UniqueBatchNo = "", string BatchNo = "")
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@TransDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@Batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@uniquebatchno", UniqueBatchNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GetItemRate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                SqlParams.Add(new SqlParameter("@PartCode", PartCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetStoreId(string StoreName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetStoreId"));
                SqlParams.Add(new SqlParameter("@StoreName", StoreName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetWorkCenterId(string WCName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetWorkCenterId"));
                SqlParams.Add(new SqlParameter("@WorkCenter", WCName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid)
            {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", Storeid));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Now));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETSTORETotalSTOCK", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenter(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckLastTransDate(string TransDate, int ItemCode, int StoreWC, int YearCode, string batchno, string uniquebatchno, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                var TrDt = Convert.ToDateTime(TransDate);
                var TransDt = TrDt.ToString("yyyy/MM/dd", culture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@TransDate", TransDt));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@SToreIdWCID", StoreWC));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@Batchno", batchno));
                SqlParams.Add(new SqlParameter("@uniquebatchno", uniquebatchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("CheckLastTransDate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStore(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_StockAdjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@ALtQty", 0));
                SqlParams.Add(new SqlParameter("@UnitQty", UnitQty));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AltUnitConversion", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string TrDate,string StoreName, string batchno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var finStDt = Convert.ToDateTime(FinStartDate);
                var transDt= Convert.ToDateTime(TrDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", finStDt.ToString("yyyy/MM/dd").Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@transDate", transDt.ToString("yyyy/MM/dd").Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@batchno", batchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("FillCurrentBatchINStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrentBatchINWIP(int ItemCode, int YearCode, int WCid, string batchno, string TransDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var StrTransDate = Convert.ToDateTime(TransDate);
                var Date = DateTime.Now;
                var FormattedTransDate = DateTime.Now.ToString("yyyy/MM/dd");
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@WCID", WCid));
                SqlParams.Add(new SqlParameter("@transDate", FormattedTransDate));
                SqlParams.Add(new SqlParameter("@batchno", batchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("FillCurrentBatchINWIP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DASHBOARDItemName"));
                SqlParams.Add(new SqlParameter("@StartDate", FromDate));
                SqlParams.Add(new SqlParameter("@EndDate", ToDate));
               


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_stockadjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DASHBOARDPartCode"));
                SqlParams.Add(new SqlParameter("@StartDate", FromDate));
                SqlParams.Add(new SqlParameter("@EndDate", ToDate));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_stockadjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashStoreName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DASHBOARDStoreName"));
                SqlParams.Add(new SqlParameter("@StartDate", FromDate));
                SqlParams.Add(new SqlParameter("@EndDate", ToDate));
               



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_stockadjustment", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashWorkCenter(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DASHBOARDWorkCenter"));
                SqlParams.Add(new SqlParameter("@StartDate", FromDate));
                SqlParams.Add(new SqlParameter("@EndDate", ToDate));




                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_stockadjustment", SqlParams);
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
