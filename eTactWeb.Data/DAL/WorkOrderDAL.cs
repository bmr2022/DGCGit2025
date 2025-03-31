using eTactWeb.Data.Common;
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

namespace eTactWeb.Data
{
    public class WorkOrderDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public WorkOrderDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetSaleOrderData(string Flag, string SPName, int YearCode, string WODate, string EffFrom, string EffTill)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var WODt = ParseDate(WODate);
                var EffFromDt = ParseDate(EffFrom);
                var EffTillDt = ParseDate(EffTill);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@WODate", (WODt).ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EffectiveFrom", (EffFromDt).ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EffectiveTill", (EffTillDt).ToString("yyyy/MM/dd")));
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
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Production Plan"));
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


        public async Task<ResponseResult> GetStoreList(string Flag, string SPName)
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
        public async Task<ResponseResult> GetTotalStockList(int store, int Itemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", Itemcode));
                SqlParams.Add(new SqlParameter("@STORE_ID", store));
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
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBomNo(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomNo"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "WORKORDER"));
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
        public async Task<ResponseResult> GetBomName(int BomNo, int FinishedItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomName"));
                SqlParams.Add(new SqlParameter("@BomNo", BomNo));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishedItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetEffDate(int BomNo,int FinishedItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomEffDate"));
                SqlParams.Add(new SqlParameter("@BomNo", BomNo));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishedItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMachineGroupName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetMachineGroupName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMachineName(int MachGroupId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetMachineName"));
                SqlParams.Add(new SqlParameter("@MachGroupId", MachGroupId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
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

        internal async Task<ResponseResult> SaveWorkOrder(WorkOrderModel model, DataTable WOGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdateBy));
                    SqlParams.Add(new SqlParameter("@LastUpdatedDate", DateTime.Today));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                DateTime entDt = new DateTime();
                DateTime EffDt = new DateTime();
                DateTime EffTillDt = new DateTime();
                DateTime Wodt = new DateTime();
                DateTime Appdt = new DateTime();
                DateTime closedt = new DateTime();
                DateTime deacdt = new DateTime();
                DateTime Actudt = new DateTime();
                DateTime Worevdt = new DateTime();

                entDt = ParseDate(model.Entrydate);
                EffDt = ParseDate(model.EffectiveFrom);
                EffTillDt = ParseDate(model.EffectiveTill);
                Wodt = ParseDate(model.WODate);
                Appdt = ParseDate(model.ApprovedDate);
                closedt = ParseDate(model.closedate);
                deacdt = ParseDate(model.DeactivateDate);
                Actudt = ParseDate(model.ActualEntryDate);
                Worevdt = ParseDate(model.WorevDate);


                SqlParams.Add(new SqlParameter("@EntryId", model.Entryid));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@Yearcode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EffectiveFrom", EffDt == default ? string.Empty : EffDt));
                SqlParams.Add(new SqlParameter("@EffectiveTill", EffTillDt == default ? string.Empty : EffTillDt));
                SqlParams.Add(new SqlParameter("@ForMonth", DateTime.Now));
                SqlParams.Add(new SqlParameter("@WONO", model.WONO));
                SqlParams.Add(new SqlParameter("@WODate", Wodt == default ? string.Empty : Wodt));
                SqlParams.Add(new SqlParameter("@workcenterId", model.workcenterId));
                SqlParams.Add(new SqlParameter("@WoStataus", model.WoStatus ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Remarkproductsupplystage", model.Remarkproductsupplystage ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RemarkForProduction", model.RemarkForProduction ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RemarkForRouting", model.RemarkForRouting ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RemarkForPacking", model.RemarkForPacking ?? string.Empty));
                SqlParams.Add(new SqlParameter("@otherInstruction", model.otherInstruction ?? string.Empty));
                SqlParams.Add(new SqlParameter("@BillingStatus", model.BillingStatus ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PendForRouteSheet", model.PendForRouteSheet ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Approved", model.Approved ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                SqlParams.Add(new SqlParameter("@ApprovedDate", Appdt == default ? string.Empty : Appdt));
                SqlParams.Add(new SqlParameter("@CloseWo", model.CloseWo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CloseBy", model.CloseBy));
                SqlParams.Add(new SqlParameter("@closedate", closedt == default ? string.Empty : closedt));
                SqlParams.Add(new SqlParameter("@DeactivateWo", model.DeactivateWo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DeactivateBy", model.DeactivateBy));
                SqlParams.Add(new SqlParameter("@Deactivatedate", deacdt == default ? string.Empty : deacdt));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", Actudt == default ? string.Empty : Actudt));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));

                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@MachineName", model.MachineName));
                SqlParams.Add(new SqlParameter("@WORevNo", model.WorevNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@WORevDate", Worevdt == default ? string.Empty : Worevdt));

                SqlParams.Add(new SqlParameter("@DTItemGrid", WOGrid));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@Entryid", ID));
                SqlParams.Add(new SqlParameter("@Yearcode", YC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<WorkOrderModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new WorkOrderModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@Yearcode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_WorkOrder", SqlParams);

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
        private static WorkOrderModel PrepareView(DataSet DS, ref WorkOrderModel? model, string Mode)
        {
            var ItemGrid = new List<WorkOrderDetail>();
            DS.Tables[0].TableName = "WorkOrderMain";
            DS.Tables[1].TableName = "WorkOrderDetail";
            int cnt = 0;

            model.Entryid = Convert.ToInt32(DS.Tables[0].Rows[0]["Entryid"].ToString());
            model.Entrydate = DS.Tables[0].Rows[0]["Entrydate"].ToString();
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Yearcode"].ToString());
            model.EffectiveFrom = DS.Tables[0].Rows[0]["EffectiveFrom"].ToString();
            model.EffectiveTill = DS.Tables[0].Rows[0]["EffectiveTill"].ToString();
            model.ForMonth = DS.Tables[0].Rows[0]["ForMonth"].ToString();
            //model.WONO = Convert.ToInt32(DS.Tables[0].Rows[0]["WONO"].ToString());
            model.WONO = DS.Tables[0].Rows[0]["WONO"].ToString();
            model.WODate = DS.Tables[0].Rows[0]["WODate"].ToString();
            model.workcenterId = Convert.ToInt32(DS.Tables[0].Rows[0]["workcenterId"].ToString());
            model.WoStatus = DS.Tables[0].Rows[0]["WoStataus"].ToString();
            model.Remarkproductsupplystage = DS.Tables[0].Rows[0]["Remarkproductsupplystage"].ToString();
            model.RemarkForProduction = DS.Tables[0].Rows[0]["RemarkForProduction"].ToString();
            model.RemarkForRouting = DS.Tables[0].Rows[0]["RemarkForRouting"].ToString();
            model.RemarkForPacking = DS.Tables[0].Rows[0]["RemarkForPacking"].ToString();
            model.otherInstruction = DS.Tables[0].Rows[0]["otherInstruction"].ToString();
            model.BillingStatus = DS.Tables[0].Rows[0]["BillingStatus"].ToString();
            model.PendForRouteSheet = DS.Tables[0].Rows[0]["PendForRouteSheet"].ToString();
            model.Approved = DS.Tables[0].Rows[0]["Approved"].ToString();
            model.ApprovedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ApprovedBy"].ToString());
            model.ApprovedDate = DS.Tables[0].Rows[0]["ApprovedDate"].ToString();
            model.CloseWo = DS.Tables[0].Rows[0]["CloseWo"].ToString();
            model.CloseBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CloseBy"].ToString());
            model.closedate = DS.Tables[0].Rows[0]["closedate"].ToString();
            model.DeactivateWo = DS.Tables[0].Rows[0]["DeactivateWo"].ToString();
            model.DeactivateBy = Convert.ToInt32(DS.Tables[0].Rows[0]["DeactivateBy"].ToString());
            model.DeactivateDate = DS.Tables[0].Rows[0]["Deactivatedate"].ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            model.ActualEntryByEmpName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"].ToString());
            model.WorevNo = DS.Tables[0].Rows[0]["WORevNo"].ToString();
            model.WORevDate = DS.Tables[0].Rows[0]["WORevDate"].ToString();
           // model.SeqNo = cnt + 1;
            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString() != "")
                {
                    model.LastUpdateBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdateByEmpName = DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString();
                    model.LastUpdateDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemGrid.Add(new WorkOrderDetail
                    {
                      //  SeqNo = Convert.ToInt32(row["SeqNo"]),
                        Accountcode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,
                        AccountName = row["Account_Name"] != DBNull.Value ? row["Account_Name"].ToString() : string.Empty,
                        SONO = row["SONO"] != DBNull.Value ? row["SONO"].ToString() : string.Empty,
                        CustomerOrderNo = row["CustomerOrderNo"] != DBNull.Value ? row["CustomerOrderNo"].ToString() : string.Empty,
                        SOYearCode = row["SOYearCode"] != DBNull.Value ? Convert.ToInt32(row["SOYearCode"]) : 0,
                        SODATE = row["SODATE"] != DBNull.Value ? row["SODATE"].ToString() : string.Empty,
                        SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                        SchYearcode = row["SchYearcode"] != DBNull.Value ? Convert.ToInt32(row["SchYearcode"]) : 0,
                        SCHDATE = row["SCHDATE"] != DBNull.Value ? row["SCHDATE"].ToString() : string.Empty,
                        Itemcode = row["Itemcode"] != DBNull.Value ? Convert.ToInt32(row["Itemcode"]) : 0,
                        COLOR = row["COLOR"].ToString(),
                        OrderQty = row["OrderQty"] != DBNull.Value ? Convert.ToSingle(row["OrderQty"]) : 0f,
                        PendRoutSheetQTy = row["PendRoutSheetQTy"] != DBNull.Value ? Convert.ToSingle(row["PendRoutSheetQTy"]) : 0f,
                        PendProdQty = row["PendProdQty"] != DBNull.Value ? Convert.ToSingle(row["PendProdQty"]) : 0f,
                        WOQty = row["WOQty"] != DBNull.Value ? Convert.ToSingle(row["WOQty"]) : 0f,
                        FGStock = row["FGStock"] != DBNull.Value ? Convert.ToSingle(row["FGStock"]) : 0f,
                        WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToSingle(row["WIPStock"]) : 0f,
                        drawingNo = row["drawingNo"] != DBNull.Value ? row["drawingNo"].ToString() : string.Empty,
                        ProdInst1 = row["ProdInst1"] != DBNull.Value ? row["ProdInst1"].ToString() : string.Empty,
                        ProdInst2 = row["ProdInst2"] != DBNull.Value ? row["ProdInst2"].ToString() : string.Empty,
                        SOInstruction = row["SOInstruction"] != DBNull.Value ? row["SOInstruction"].ToString() : string.Empty,
                        PkgInstruction = row["PkgInstruction"] != DBNull.Value ? row["PkgInstruction"].ToString() : string.Empty,
                        PendingRouteForSheet = row["PendingRouteForSheet"] != DBNull.Value ? row["PendingRouteForSheet"].ToString() : string.Empty,
                        RouteSheetNo = row["RouteSheetNo"] != DBNull.Value ? row["RouteSheetNo"].ToString() : string.Empty,
                        RouteSheetYearCode = row["RouteSheetYearCode"] != DBNull.Value ? Convert.ToInt32(row["RouteSheetYearCode"]) : 0,
                        RouteSheetEntryNo = row["RouteSheetEntryNo"] != DBNull.Value ? Convert.ToInt32(row["RouteSheetEntryNo"]) : 0,
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        WorevNo = row["WorevNo"].ToString(),
                        PrevWOQty = row["PrevWOQty"] != DBNull.Value ? Convert.ToSingle(row["PrevWOQty"]) : 0,
                        ItemActive = row["ItemActive"] != DBNull.Value ? row["ItemActive"].ToString() : string.Empty,
                        FGStoreId = row["FGStoreId"] != DBNull.Value ? Convert.ToInt32(row["FGStoreId"]) : 0,
                        FGStore = row["FGStoreName"] != DBNull.Value ? row["FGStoreName"].ToString() : string.Empty,
                        Bomno = row["Bomno"] != DBNull.Value ? Convert.ToInt32(row["Bomno"]) : 0,
                        BomName = row["BomName"] != DBNull.Value ? row["BomName"].ToString() : string.Empty,
                        BomEffectiveDate = row["BomEffectiveDate"] != DBNull.Value ? row["BomEffectiveDate"].ToString() : string.Empty,
                        MainBomSubBom = row["MainBomSubBom"] != DBNull.Value ? row["MainBomSubBom"].ToString() : string.Empty,
                        MachineGroupId = row["MachineGroupID"] != DBNull.Value ? Convert.ToInt32(row["MachineGroupID"]) : 0,
                        MachineGroupName = row["MachGroup"] != DBNull.Value ? row["MachGroup"].ToString() : string.Empty,
                        PrefMachineId1 = row["PrefMachineId1"] != DBNull.Value ? Convert.ToInt32(row["PrefMachineId1"]) : 0,
                        PrefMachineId2 = row["PrefMachineId2"] != DBNull.Value ? Convert.ToInt32(row["PrefMachineId2"]) : 0,
                        PrefMachineId3 = row["PrefMachineId3"] != DBNull.Value ? Convert.ToInt32(row["PrefMachineId3"]) : 0,
                        PrefMachineName1 = row["MachineName1"] != DBNull.Value ? row["MachineName1"].ToString() : string.Empty,
                        PrefMachineName2 = row["MachineName2"] != DBNull.Value ? row["MachineName2"].ToString() : string.Empty,
                        PrefMachineName3 = row["MachineName3"] != DBNull.Value ? row["MachineName3"].ToString() : string.Empty,
                        OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                        OrderWEF = row["OrderWEF"] != DBNull.Value ? row["OrderWEF"].ToString() : string.Empty,
                        AmendNo = row["AmendNo"] != DBNull.Value ? Convert.ToInt32(row["AmendNo"]) : 0,
                        AmendEffDate = row["AmendEffDate"] != DBNull.Value ? row["AmendEffDate"].ToString() : string.Empty,
                        Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                        AltUnit = row["AltUnit"] != DBNull.Value ? row["AltUnit"].ToString() : string.Empty,
                        AltOrderQty = row["AltOrderQty"] != DBNull.Value ? Convert.ToSingle(row["AltOrderQty"]) : 0,
                        SOCloseDate = row["SOCloseDate"] != DBNull.Value ? row["FGStoreName"].ToString() : string.Empty,
                        StoreId = row["StoreId"] != DBNull.Value ? Convert.ToInt32(row["StoreId"]) : 0,
                        StoreName = row["StoreName"] != DBNull.Value ? row["StoreName"].ToString() : string.Empty,
                        StoreStock = row["StoreStock"] != DBNull.Value ? Convert.ToSingle(row["StoreStock"]) : 0,
                        ApproxStartDate = row["ApproxStartDate"] != DBNull.Value ? row["ApproxStartDate"].ToString() : string.Empty,
                        ApproxEndDate = row["ApproxEndDate"] != DBNull.Value ? row["ApproxEndDate"].ToString() : string.Empty,
                        SchEffTillDate = row["SchEffTillDate"] != DBNull.Value ? row["SchEffTillDate"].ToString() : string.Empty,
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToSingle(row["AltQty"]) : 0,
                        ItemDescription = row["ItemDescription"] != DBNull.Value ? row["ItemDescription"].ToString() : string.Empty
                        //WIPStoreId = row["WIPStoreId"] != DBNull.Value ? Convert.ToInt32(row["WIPStoreId"]) : 0

                    });
                } 
                if (DS.Tables[1].Rows[0]["RouteSheetDate"] != DBNull.Value)
                {
                    model.RouteSheetDate = DS.Tables[1].Rows[0]["RouteSheetDate"].ToString();
                }
                else
                {
                    model.RouteSheetDate = "";
                }
                if (DS.Tables[1].Rows[0]["WORevDate"] != DBNull.Value)
                {
                    model.WORevDate = DS.Tables[1].Rows[0]["WORevDate"].ToString();
                }
                else
                {
                    model.WORevDate = "";
                }
                ItemGrid = ItemGrid.OrderBy(item => item.SeqNo).ToList();
                model.WorkDetailGrid = ItemGrid;
            }


            return model;
        }
        internal async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                SqlParams.Add(new SqlParameter("@FromDate", ParseDate(FromDate)));
                SqlParams.Add(new SqlParameter("@Todate", ParseDate(ToDate)));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_WorkOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<WorkOrderGridDashboard> GetDashboardData(string SummaryDetail, string WONO, string CC, string SONO, string SchNo, string AccountName, string PartCode, string ItemName, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new WorkOrderGridDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_WorkOrder", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    DateTime FromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@WONO", WONO);
                    oCmd.Parameters.AddWithValue("@SOno", SONO);
                    oCmd.Parameters.AddWithValue("@SchNo", SchNo);
                    oCmd.Parameters.AddWithValue("@Accountname", AccountName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
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
                if (SummaryDetail == "Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.WorkOrderGrid = (from DataRow distinctRow in oDataSet.Tables[0].Rows
                                               group distinctRow by Convert.ToInt32(distinctRow["Entryid"]) into grouped
                                               select grouped.First() into dr
                                               select new WorkOrderGridDashboard
                                               {
                                                   Entryid = Convert.ToInt32(dr["Entryid"]),
                                                   YearCode = Convert.ToInt32(dr["Yearcode"]),
                                                   WONO = dr["WONO"].ToString(),
                                                   WODate = dr["WODate"].ToString(),
                                                   EffectiveFrom = dr["EffectiveFrom"].ToString(),
                                                   EffectiveTill = dr["EffectiveTill"].ToString(),
                                                   EntryDate = dr["Entrydate"].ToString(),
                                                   RemarkForProduction = dr["RemarkForProduction"].ToString(),
                                                   RemarkProductSupplyStage = dr["Remarkproductsupplystage"].ToString(),
                                                   RemarkForPacking = dr["RemarkForPacking"].ToString(),
                                                   RemarkForRouting = dr["RemarkForRouting"].ToString(),
                                                   OtherInstruction = dr["otherInstruction"].ToString(),
                                                   BillingStatus = dr["BillingStatus"].ToString(),
                                                   PendRouteSheet = dr["PendForRouteSheet"].ToString(),
                                                   Approved = dr["Approved"].ToString(),
                                                   ApprovedBy = Convert.ToInt32(dr["ApprovedBy"].ToString()),
                                                   ApprovedDate = dr["ApprovedDate"].ToString(),
                                                   WoStataus = dr["WoStataus"].ToString(),
                                                   CloseWo = dr["CloseWo"].ToString(),
                                                   WorevNo = dr["WorevNo"].ToString(),
                                                   WORevDate = dr["WORevDate"].ToString(),
                                                   ActualEntryBy = dr["ActualEntryByEmpName"].ToString(),
                                                   ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                   LastUpdatedBy = dr["LastUpdatedByEmpName"].ToString(),
                                                   LastUpdatedDate = dr["LastUpdatedDate"].ToString(),
                                                   MachineName = dr["MachineName"].ToString()
                                               }).ToList();
                    }
                }
                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.WorkOrderGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new WorkOrderGridDashboard
                                               {
                                                   Entryid = Convert.ToInt32(dr["Entryid"]),
                                                   YearCode = Convert.ToInt32(dr["Yearcode"]),
                                                   WONO = dr["WONO"].ToString(),
                                                   WODate = dr["WODate"].ToString(),
                                                   WorkCenterName = dr["WODate"].ToString(),
                                                   EffectiveFrom = dr["EffectiveFrom"].ToString(),
                                                   EffectiveTill = dr["EffectiveTill"].ToString(),
                                                   AccountName = dr["Account_Name"].ToString(),
                                                   SONO = dr["SONO"].ToString(),
                                                   SOYearCode = Convert.ToInt32(dr["SOYearCode"]),
                                                   SchNo = dr["SchNo"].ToString(),
                                                   SchYearCode = Convert.ToInt32(dr["SchYearcode"]),
                                                   SchDate = dr["SCHDATE"].ToString(),
                                                   PartCode = dr["PartCode"].ToString(),
                                                   ItemName = dr["ItemName"].ToString(),
                                                   COLOR = dr["COLOR"].ToString(),
                                                   OrderQty = Convert.ToSingle(dr["OrderQty"]),
                                                   PendRouteSheetQTy = Convert.ToSingle(dr["PendRoutSheetQTy"]),
                                                   PendProdQty = Convert.ToSingle(dr["PendProdQty"]),
                                                   WOQty = Convert.ToSingle(dr["WOQty"]),
                                                   FGStock = Convert.ToSingle(dr["FGStock"]),
                                                   WIPStock = Convert.ToSingle(dr["WIPStock"]),
                                                   drawingNo = dr["drawingNo"].ToString(),
                                                   ProdInst1 = dr["ProdInst1"].ToString(),
                                                   ProdInst2 = dr["ProdInst2"].ToString(),
                                                   SOInstruction = dr["SOInstruction"].ToString(),
                                                   PkgInstruction = dr["PkgInstruction"].ToString(),
                                                   RouteSheetNo = dr["RouteSheetNo"].ToString(),
                                                   RouteSheetYearCode = dr["RouteSheetYearCode"].ToString(),
                                                   RouteSheetDate = dr["RouteSheetDate"].ToString(),
                                                   PrevWoQty = Convert.ToSingle(dr["PrevWOQty"]),
                                                   EntryDate = dr["Entrydate"].ToString(),
                                                   RemarkForProduction = dr["RemarkForProduction"].ToString(),
                                                   RemarkProductSupplyStage = dr["Remarkproductsupplystage"].ToString(),
                                                   RemarkForPacking = dr["RemarkForPacking"].ToString(),
                                                   RemarkForRouting = dr["RemarkForRouting"].ToString(),
                                                   OtherInstruction = dr["otherInstruction"].ToString(),
                                                   BillingStatus = dr["BillingStatus"].ToString(),
                                                   PendRouteSheet = dr["PendForRouteSheet"].ToString(),
                                                   Approved = dr["Approved"].ToString(),
                                                   ApprovedBy = Convert.ToInt32(dr["ApprovedBy"].ToString()),
                                                   ApprovedDate = dr["ApprovedDate"].ToString(),
                                                   WoStataus = dr["WoStataus"].ToString(),
                                                   CloseWo = dr["CloseWo"].ToString(),
                                                   WorevNo = dr["WorevNo"].ToString(),
                                                   WORevDate = dr["WORevDate"].ToString(),
                                                   ActualEntryBy = dr["ActualEntryByEmpName"].ToString(),
                                                   ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                   LastUpdatedBy = dr["LastUpdatedByEmpName"].ToString(),
                                                   LastUpdatedDate = dr["LastUpdatedDate"].ToString(),
                                                   FGStoreName = dr["FGStoreName"].ToString(),
                                                   WIPStoreName = dr["WIPStoreName"].ToString(),
                                                   MachineName = dr["MachineName"].ToString()
                                               }).ToList();
                    }
                }
                model.SummaryDetail = SummaryDetail;

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

    }
}