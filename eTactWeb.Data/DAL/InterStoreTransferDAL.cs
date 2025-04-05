using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    internal class InterStoreTransferDAL
    {
     private readonly ConnectionStringService _connectionStringService;
        public InterStoreTransferDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Inter Store Transfer"));
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

        public async Task<ResponseResult> NewEntryId(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "LoadFromStore"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string UniqueBatchNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetPrevQty"));
                SqlParams.Add(new SqlParameter("@EntryID", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy,string MachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@EntryDate", EntryDate));
                SqlParams.Add(new SqlParameter("@ActulEntryBy", ActualEntryBy));
                SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<InterStoreTransferModel> GetViewByID(int ID, string Mode, int YC)
        {
            var model = new InterStoreTransferModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InterStoreTransferMainDetail", SqlParams);

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

        private static InterStoreTransferModel PrepareView(DataSet DS, ref InterStoreTransferModel? model, string Mode)
        {
            var ItemList = new List<InterStoreTransferDetail>();
            DS.Tables[0].TableName = "VInterStoreTransferMainDetail";

            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Yearcode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.SlipNo = DS.Tables[0].Rows[0]["SlipNo"].ToString();
            model.SlipDate = DS.Tables[0].Rows[0]["SlipDate"].ToString();
            model.IssueToStoreWC = DS.Tables[0].Rows[0]["IssueToStoreWC"].ToString();
            model.FromStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["FromStoreId"]);
            model.FromStoreName = DS.Tables[0].Rows[0]["FromStoreName"].ToString();
            model.ToStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["ToStoreId"].ToString());
            model.ToStoreName = DS.Tables[0].Rows[0]["ToStorename"].ToString();
            model.ToWCID = Convert.ToInt32(DS.Tables[0].Rows[0]["ToWCID"]);
            model.ToWCName = DS.Tables[0].Rows[0]["ToWCName"].ToString();
            model.IssuedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedBy"]);
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            //model.ActualEntryBy = DS.Tables[0].Rows[0]["StockAdjustmentDate"].ToString();
            model.ActualEntryByName = DS.Tables[0].Rows[0]["EnteredBy"].ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.TransferReason = DS.Tables[0].Rows[0]["TransferReason"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]);
            model.MachineName = DS.Tables[0].Rows[0]["TransferReason"].ToString();

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdateddBy"].ToString();
                model.LastUpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatetionDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdatetionDate"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new InterStoreTransferDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"]),
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        TotalStockQty = Convert.ToSingle(row["TotalStockQty"]),
                        LotStockQty = Convert.ToSingle(row["LotStockQty"]),
                        Qty = Convert.ToSingle(row["Qty"]),
                        Unit = row["Unit"].ToString(),
                        AltQty = Convert.ToSingle(row["AltQty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        Rate = Convert.ToSingle(row["Rate"]),
                        BatchNo = row["Batchno"].ToString(),
                        UniqueBatchNo = row["Uniquebatchno"].ToString(),
                        ReasonOfTransfer = row["ReasonOfTransfer"].ToString(),
                        RecStoreStock = Convert.ToSingle(row["RecStoreStock"].ToString())
                    });
                }
                ItemList = ItemList.OrderBy(item => item.SeqNo).ToList();
                model.InterStoreDetails = ItemList;
            }

            return model;
        }
        public async Task<ResponseResult> GetUnitAltUnit(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetUnitAltUnit"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillLoadToStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "LoadTOStore"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                var Date = DateTime.Now;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                SqlParams.Add(new SqlParameter("@transDate", Date));
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

        internal async Task<ResponseResult> GetDashboardData(ISTDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                SqlParams.Add(new SqlParameter("@SlipNo", model.SlipNo));
                SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@ToStoreName", model.ToStorename));
                SqlParams.Add(new SqlParameter("@ToWCName", model.ToWCName));
                SqlParams.Add(new SqlParameter("@Batchno", model.Batchno));
                SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_InterStoreTransferMainDetail", SqlParams);
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

        public async Task<ResponseResult> SaveInterStore(InterStoreTransferModel model, DataTable ISTGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                DateTime entDt = new DateTime();
                DateTime SADt = new DateTime();

                entDt = ParseDate(model.EntryDate);
                SADt = ParseDate(model.SlipDate);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdatetionDate", DateTime.Now));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@SlipNo", model.SlipNo));
                SqlParams.Add(new SqlParameter("@SlipDate", SADt == default ? string.Empty : SADt));
                SqlParams.Add(new SqlParameter("@IssueToStoreWC", model.IssueToStoreWC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@FromStoreId", model.FromStoreId));
                SqlParams.Add(new SqlParameter("@ToStoreId", model.ToStoreId));
                SqlParams.Add(new SqlParameter("@ToWCID", model.ToWCID));
                SqlParams.Add(new SqlParameter("@IssuedBy", model.IssuedBy));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ActulEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", eTactWeb.Data.Common.CommonFunc.ParseFormattedDate( DateTime.Now.ToString())));
                SqlParams.Add(new SqlParameter("@TransferReason", model.TransferReason ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@MAchineName", model.MachineName));

                SqlParams.Add(new SqlParameter("@DTItemGrid", ISTGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillLoadTOWorkcenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "LoadTOWorkcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetAllowBackDate()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAllowBackDate"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(string ShowAllItems)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPartCode"));
                SqlParams.Add(new SqlParameter("@SHowAllItem", ShowAllItems));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckIssuedTransStock(int ItemCode, int YearCode, int EntryId, string TransDate, string TransNo, int Storeid, string batchno, string uniquebatchno, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var TransDt = ParseDate(TransDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@Entryid", EntryId));
                SqlParams.Add(new SqlParameter("@TransDate", TransDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@TransNo", TransNo));
                SqlParams.Add(new SqlParameter("@storeid", Storeid));
                SqlParams.Add(new SqlParameter("@batchno", batchno));
                SqlParams.Add(new SqlParameter("@uniquebatchno", uniquebatchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPGetissuedTransStock", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItems(string ShowAllItems)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLITEM"));
                SqlParams.Add(new SqlParameter("@SHowAllItem", ShowAllItems));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_InterStoreTransferMainDetail", SqlParams);
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