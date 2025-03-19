using AutoMapper.Configuration;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.DAL
{
    public class ProductionScheduleDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        public ProductionScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> DeleteByID(int ActualEntryBy, string EntryDate,int ID, int YC, int createdBy, string entryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var now = DateTime.Now;
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@yearcode", YC));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", entryByMachineName));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", ActualEntryBy));
                SqlParams.Add(new SqlParameter("@EntryDate", EntryDate.Split(" ")[0]));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionSchedule", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ProductionScheduleModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new ProductionScheduleModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@Yearcode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionSchedule", SqlParams);

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
        private static ProductionScheduleModel PrepareView(DataSet DS, ref ProductionScheduleModel? model, string Mode)
        {
            try
            {
                var ItemGrid = new List<ProductionScheduleDetail>();
                var ProdPlanGrid = new List<ProductionScheduleProdPlanDetail>();
                DS.Tables[0].TableName = "productionScheduleModel";
                DS.Tables[1].TableName = "ProdPlanDetail";
                DS.Tables[2].TableName = "BomChildDetail";
                DS.Tables[3].TableName = "BomSummary";
                int cnt = 0;

                model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
                model.EntryDate = DS.Tables[0].Rows[0]["Entrydate"]?.ToString();
                model.WCID = Convert.ToInt32(DS.Tables[0].Rows[0]["WCID"]);
                model.ProdSchNo = DS.Tables[0].Rows[0]["ProdSchNo"]?.ToString();
                model.ProdSchDate = DS.Tables[0].Rows[0]["ProdSchDate"]?.ToString();
                model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
                model.EffectiveFrom = DS.Tables[0].Rows[0]["EffectiveFrom"]?.ToString();
                model.EffectiveTo = DS.Tables[0].Rows[0]["EffectiveTo"]?.ToString();
                model.RevNo = Convert.ToInt32(DS.Tables[0].Rows[0]["RevNo"]);
                model.CC = DS.Tables[0].Rows[0]["CC"]?.ToString();
                model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"]);
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
                model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEntryByName"]?.ToString();
                model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"]);
                model.EntryByMachineName = DS.Tables[0].Rows[0]["EntryByMachineName"]?.ToString();
                model.Closed = DS.Tables[0].Rows[0]["Closed"]?.ToString();
                model.Completed = DS.Tables[0].Rows[0]["Completed"]?.ToString();
                model.ForTheMonth = Convert.ToInt32(DS.Tables[0].Rows[0]["ForTheMonth"]);
                model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
                model.ShowWOWithOrWOItem = DS.Tables[0].Rows[0]["ShowWOWithOrWOItem"]?.ToString();
                model.PlanForNoOFDays = Convert.ToInt32(DS.Tables[0].Rows[0]["PlanForNoOFDays"]);
                // model.SeqNo = cnt + 1;

                if (Mode == "U" || Mode == "V")
                {
                    if (DS.Tables[0].Rows[0]["LastUpdatedByName"].ToString() != "")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                        model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByName"].ToString();
                        model.LastUpdatedDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
                    }
                }

                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemGrid.Add(new ProductionScheduleDetail
                        {
                            //  SeqNo = Convert.ToInt32(row["SeqNo"]),
                            ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                            PartCode = row["PartCode"]?.ToString(),
                            ItemName = row["Item_Name"]?.ToString(),
                            AccountName = row["AccountName"]?.ToString(),
                            BOMNO = row["BOMNO"] != DBNull.Value ? Convert.ToInt32(row["BOMNO"]) : 0,
                            BomEffDate = row["BomEffDate"]?.ToString(),
                            SchDate = row["Schdate"]?.ToString() != string.Empty ? Convert.ToDateTime(row["Schdate"]).ToString("dd/MM/yyyy") : string.Empty,
                            ShiftID = row["ShiftID"] != DBNull.Value ? Convert.ToInt32(row["ShiftID"]) : 0,
                            ProdInWC = row["ProdInWC"] != DBNull.Value ? Convert.ToInt32(row["ProdInWC"]) : 0,
                            Qty = row["Qty"] != DBNull.Value ? Convert.ToSingle(row["Qty"]) : 0,
                            ProdPendQty = row["ProdPendQty"] != DBNull.Value ? Convert.ToSingle(row["ProdPendQty"]) : 0,
                            Originalqty = row["Originalqty"] != DBNull.Value ? Convert.ToInt32(row["Originalqty"]) : 0,
                            TotalWOQty = row["TotalWOQty"] != DBNull.Value ? Convert.ToInt32(row["TotalWOQty"]) : 0,
                            PlannedMachineid1 = row["PlannedMachineid1"] != DBNull.Value ? Convert.ToInt32(row["PlannedMachineid1"]) : 0,
                            PlannedMachineid2 = row["PlannedMachineid2"] != DBNull.Value ? Convert.ToInt32(row["PlannedMachineid2"]) : 0,
                            PlannedMachineid3 = row["PlannedMachineid3"] != DBNull.Value ? Convert.ToInt32(row["PlannedMachineid3"]) : 0,
                            WONo = row["ProdPlanNo"]?.ToString(),
                            WOYearCode = row["ProdPlanNoYearCode"] != DBNull.Value ? Convert.ToInt32(row["ProdPlanNoYearCode"]) : 0,
                            WODate = row["ProdPlanNoDate"]?.ToString(),
                            SONo = row["SONo"]?.ToString(),
                            CustOrderNo = row["CustOrderNo"]?.ToString(),
                            SOYearCode = row["SOYearCode"] != DBNull.Value ? Convert.ToInt32(row["SOYearCode"]) : 0,
                            SODate = row["SODate"]?.ToString(),
                            SubBOM = row["SubBOM"]?.ToString(),
                            InhouseJOBProd = row["InhouseJOBProd"]?.ToString(),
                            DrawingNo = row["DrawingNo"]?.ToString(),
                            ProdCompleted = row["ProdCompleted"]?.ToString(),
                            ProdCanceled = row["ProdCanceled"]?.ToString(),
                            QCMandatory = row["QCMandatory"]?.ToString(),
                            ProdSeq = row["ProdSeq"] != DBNull.Value ? Convert.ToInt32(row["ProdSeq"]) : 0,
                            AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,

                        });
                    }
                    model.ProductionScheduleDetails = ItemGrid;
                }

                if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ProdPlanGrid.Add(new ProductionScheduleProdPlanDetail
                        {
                            ProdSchNo = row["ProdSchNo"]?.ToString(),
                            PlanNo = row["PlanNo"]?.ToString(),
                            PlanNoEntryId = row["PlanNoEntryId"] != DBNull.Value ? Convert.ToInt32(row["PlanNoEntryId"]) : 0,
                            PlanNoYearCode = row["PlanNoYearCode"] != DBNull.Value ? Convert.ToInt32(row["PlanNoYearCode"]) : 0,
                            PlanNoDate = row["PlanNoDate"]?.ToString(),
                            SONO = row["SONO"]?.ToString(),
                            CustOrderNo = row["CustOrderNo"]?.ToString(),
                            SOEntryId = row["SOEntryId"] != DBNull.Value ? Convert.ToInt32(row["SOEntryId"]) : 0,
                            SOYearCode = row["SOyearCode"] != DBNull.Value ? Convert.ToInt32(row["SOyearCode"]) : 0,
                            AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,
                            WOEffectiveFrom = row["WOEffectiveFrom"]?.ToString(),
                            WOEndDate = row["WOEndDate"]?.ToString(),
                            SaleSchNo = row["SaleSchNo"]?.ToString(),
                            SaleSchYearCode = row["SaleSchYearcode"] != DBNull.Value ? Convert.ToInt32(row["SaleSchYearcode"]) : 0,
                            SaleSchDate = row["SaleSchDate"]?.ToString(),
                            SaleSchEntryId = row["SaleSchEntryId"] != DBNull.Value ? Convert.ToInt32(row["SaleSchEntryId"]) : 0,
                        });
                    }
                    model.prodPlanDetails = ProdPlanGrid;
                }

                if (model.BomDatamodel == null)
                {
                    model.BomDatamodel = new ProductionScheduleBOMData();
                }
                if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
                {
                    if (model.BomDatamodel.BomSummaries == null)
                    {
                        model.BomDatamodel.BomSummaries = new List<ProductionScheduleBomSummary>();
                    }
                    List<ProductionScheduleBomSummary> BomSummaries = new List<ProductionScheduleBomSummary>();
                    foreach (DataRow row in DS.Tables[2].Rows)
                    {
                        BomSummaries.Add(new ProductionScheduleBomSummary
                        {
                            RMItemCode = row["RMitemCode"] != DBNull.Value ? Convert.ToInt32(row["RMitemCode"]) : 0,
                            RMItemName = row["RMItemName"]?.ToString(),
                            RMPartCode = row["RMPartCode"]?.ToString(),
                            Unit = "",
                            TotalReqQty = row["RMQTY"] != DBNull.Value ? Convert.ToInt32(row["RMQTY"]) : 0,
                            PendQty = row["PendQtyToIssueFromStore"] != DBNull.Value ? Convert.ToInt32(row["PendQtyToIssueFromStore"]) : 0,
                            MainStoreStock = row["MainstoreStock"] != DBNull.Value ? Convert.ToInt32(row["MainstoreStock"]) : 0,
                            QcStoreStock = row["qcstoreStock"] != DBNull.Value ? Convert.ToInt32(row["qcstoreStock"]) : 0,
                            WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToInt32(row["WIPStock"]) : 0,
                        });
                    }
                    model.BomDatamodel.BomSummaries = BomSummaries;
                }

                if (DS.Tables.Count != 0 && DS.Tables[3].Rows.Count > 0)
                {
                    if (model.BomDatamodel.BomDetails == null)
                    {
                        model.BomDatamodel.BomDetails = new List<ProductionScheduleBomDetail>();
                    }
                    List<ProductionScheduleBomDetail> BomDetails = new List<ProductionScheduleBomDetail>();
                    foreach (DataRow row in DS.Tables[3].Rows)
                    {
                        BomDetails.Add(new ProductionScheduleBomDetail
                        {
                            FGItemCode = row["FGItemCode"] != DBNull.Value ? Convert.ToInt32(row["FGItemCode"]) : 0,
                            FGPartCode = row["FGPartCode"]?.ToString(),
                            FGItemName = row["FGItemName"]?.ToString(),
                            FGQty = row["FGQty"] != DBNull.Value ? Convert.ToInt32(row["FGQty"]) : 0,
                            RMItemCode = row["RMitemCode"] != DBNull.Value ? Convert.ToInt32(row["RMitemCode"]) : 0,
                            RMItemName = row["RMItemName"]?.ToString(),
                            RMPartCode = row["RMPartCode"]?.ToString(),
                            Unit = "",
                            ReqQty = row["RMQTY"] != DBNull.Value ? Convert.ToInt32(row["RMQTY"]) : 0,
                            PendQty = row["PendToIssueFromStore"] != DBNull.Value ? Convert.ToInt32(row["PendToIssueFromStore"]) : 0,
                            BomNo = row["BOMNO"] != DBNull.Value ? Convert.ToInt32(row["BOMNO"]) : 0,
                            BomEffDate = row["BOMEffectiveDate"]?.ToString(),
                            BomQty = row["BOMQTY"] != DBNull.Value ? Convert.ToInt32(row["BOMQTY"]) : 0
                        });
                    }
                    model.BomDatamodel.BomDetails = BomDetails;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static ProductionScheduleModel PrepareBOMDetails(DataSet DS, ref ProductionScheduleModel? model)
        {
            var BomSummary = new List<ProductionScheduleBomSummary>();
            var BomDetail = new List<ProductionScheduleBomDetail>();
            var mainModel = new ProductionScheduleBOMData();

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    BomDetail.Add(new ProductionScheduleBomDetail
                    {
                        FGItemCode = row["FGItemCode"] != DBNull.Value ? Convert.ToInt32(row["FGItemCode"]) : 0,
                        FGPartCode = row["FGPartCode"]?.ToString(),
                        FGItemName = row["FgItemName"]?.ToString(),
                        FGQty = row["FGQty"] != DBNull.Value ? Convert.ToInt32(row["FGQty"]) : 0,
                        RMItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        RMItemName = row["RMItemName"]?.ToString(),
                        RMPartCode = row["RMPartCode"]?.ToString(),
                        Unit = row["Unit"]?.ToString(),
                        ReqQty = row["ReqQty"] != DBNull.Value ? Convert.ToSingle(row["ReqQty"]) : 0,
                        PendQty = row["PendQty"] != DBNull.Value ? Convert.ToSingle(row["PendQty"]) : 0
                    });
                }
                mainModel.BomDetails = BomDetail;
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    BomSummary.Add(new ProductionScheduleBomSummary
                    {
                        //  SeqNo = Convert.ToInt32(row["SeqNo"]),
                        RMItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        RMItemName = row["RMItemName"]?.ToString(),
                        RMPartCode = row["RMPartCode"]?.ToString(),
                        Unit = row["Unit"]?.ToString(),
                        TotalReqQty = row["TotalReqQty"] == DBNull.Value ? 0 : Convert.ToSingle(row["TotalReqQty"]),
                        PendQty = row["PendQty"] != DBNull.Value ? Convert.ToInt32(row["PendQty"]) : 0,
                        MainStoreStock = row["MainStoreStock"] != DBNull.Value ? Convert.ToInt32(row["MainStoreStock"]) : 0,
                        QcStoreStock = row["QcStoreStock"] != DBNull.Value ? Convert.ToInt32(row["QcStoreStock"]) : 0,
                        WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToInt32(row["WIPStock"]) : 0

                    });
                }
                mainModel.BomSummaries = BomSummary;
            }
            model.BomDatamodel = mainModel;
            return model;
        }

        public async Task<ResponseResult> GetBomMultiLevelGrid()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETBOMMULTILEVELITEMS", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItems()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItems"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ProductionScheduleModel> PSBomDetail(int YearCode, DataTable itemGrid)
        {
            var _ResponseResult = new ResponseResult();
            ProductionScheduleModel model = new();
            try
            {
                var currentDate = eTactWeb.Data.Common.CommonFunc.ParseFormattedDate(((DateTime.Now).ToString()).Split(" ")[0]);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillBOMCHILDPART"));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@CurrentDate", currentDate));
                SqlParams.Add(new SqlParameter("@DTItemGrid", itemGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionSchedule", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareBOMDetails(_ResponseResult.Result, ref model);
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
        internal async Task<ResponseResult> GetDashboardData(string partCode, string itemName, string accountName, string FromDate, string ToDate, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime FromDate1 = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@Flag", "dashboard"));
                SqlParams.Add(new SqlParameter("@SOFromDate", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@CurrentDate", ParseFormattedDate(ToDate)));
                SqlParams.Add(new SqlParameter("@PartCode", partCode));
                SqlParams.Add(new SqlParameter("@ItemName", itemName));
                SqlParams.Add(new SqlParameter("@AccountName", accountName));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetPendWOData(string PendWoType, int YearCode, string SOEffFromDate, string CurrentDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayPendWO"));
                SqlParams.Add(new SqlParameter("@DisplayFlag", PendWoType));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@SOFromDate", SOEffFromDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMachineName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetMachineName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillWorkCenetr"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> SaveProductionSchedule(ProductionScheduleModel model, DataTable PSGrid, DataTable prodPlanDetail, DataTable bomChildDetail, DataTable bomSummaryDetail)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LatUpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdateDate", DateTime.Today));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                DateTime entDt = new DateTime();
                DateTime ProdSchDate = new DateTime();
                DateTime FromSchDt = new DateTime();
                DateTime ToSchDt = new DateTime();
                DateTime EffFromDt = new DateTime();
                DateTime EffTillDt = new DateTime();
                DateTime Revdt = new DateTime();
                DateTime Actudt = new DateTime();
                DateTime lastUpdatedDate = new DateTime();

                entDt = ParseDate(model.EntryDate);
                ProdSchDate = ParseDate(model.ProdSchDate);
                FromSchDt = ParseDate(model.FinFromDate);
                ToSchDt = ParseDate(model.FinToDate);
                EffFromDt = ParseDate(model.EffectiveFrom);
                EffTillDt = ParseDate(model.EffectiveTill);
                Revdt = ParseDate(model.RevDate);
                Actudt = ParseDate(model.ActualEntryDate);
                //lastUpdatedDate = ParseDate(model.LastUpdatedDate);

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryID));
                SqlParams.Add(new SqlParameter("@Yearcode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@WCID", model.WCID));
                SqlParams.Add(new SqlParameter("@ProdSchNo", model.ProdSchNo));
                SqlParams.Add(new SqlParameter("@ProdSchDate", ProdSchDate == default ? string.Empty : ProdSchDate));
                SqlParams.Add(new SqlParameter("@EffectiveFrom", EffFromDt == default ? string.Empty : EffFromDt));
                SqlParams.Add(new SqlParameter("@EffectiveTo", EffTillDt == default ? string.Empty : EffTillDt));
                SqlParams.Add(new SqlParameter("@RevNo", model.RevNo));
                SqlParams.Add(new SqlParameter("@Revdate", Revdt == default ? string.Empty : Revdt));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@actualEntryDate", Actudt == default ? string.Empty : Actudt));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                if (model.Mode != "U")
                {
                    //SqlParams.Add(new SqlParameter("@LastUpdateDate", lastUpdatedDate == default ? string.Empty : lastUpdatedDate));
                    SqlParams.Add(new SqlParameter("@LatUpdatedBy", model.LastUpdatedBy));
                }
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Closed", model.Closed ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Completed", model.Completed));
                SqlParams.Add(new SqlParameter("@ForTheMonth", model.ForTheMonth));
                SqlParams.Add(new SqlParameter("@Remarks", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ShowWOWithOrWOItem", model.ShowWOWithOrWOItem ?? string.Empty));
                SqlParams.Add(new SqlParameter("@FromSchDate", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ToSchDate", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ForNoofDays", model.PlanForNoOFDays));
                if (model.Mode != "U")
                {
                    SqlParams.Add(new SqlParameter("@LastUpdateDate", string.Empty));
                }
                SqlParams.Add(new SqlParameter("@DTItemGrid", PSGrid));
                SqlParams.Add(new SqlParameter("@DTWOdetail", prodPlanDetail));
                SqlParams.Add(new SqlParameter("@DTBOMChild", bomChildDetail));
                SqlParams.Add(new SqlParameter("@DTBOMChildSumm", bomSummaryDetail));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);

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
        public async Task<ProductionScheduleModel> AddPendingProdPlans(int yearCode, string schFromDate, string schTillDate, string displayFlag, int noOfDays, DataTable PendingProdPlans)
        {
            var _ResponseResult = new ResponseResult();
            var model = new ProductionScheduleModel();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemDetailIngrid"));
                SqlParams.Add(new SqlParameter("@Yearcode", yearCode));
                SqlParams.Add(new SqlParameter("@SchFromDate", schFromDate));
                SqlParams.Add(new SqlParameter("@SchTodate", schTillDate));
                SqlParams.Add(new SqlParameter("@DisplayFlag", displayFlag));
                SqlParams.Add(new SqlParameter("@ForNoofDays", noOfDays));
                SqlParams.Add(new SqlParameter("@DTgetPlanItem", PendingProdPlans));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionSchedule", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    ProductionSchDetail(_ResponseResult.Result, ref model);
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

        private static ProductionScheduleModel ProductionSchDetail(DataTable DS, ref ProductionScheduleModel? model)
        {
            var ItemGrid = new List<ProductionScheduleDetail>();
            DS.TableName = "WorkOrderDetail";
            int cnt = 0;
            try
            {
                if (DS.Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Rows)
                    {
                        ItemGrid.Add(new ProductionScheduleDetail
                        {
                            //  SeqNo = Convert.ToInt32(row["SeqNo"]),
                            PartCode = row["PartCode"]?.ToString(),
                            ItemName = row["ItemName"]?.ToString(),
                            //schqty = row["schqty"]?.ToString(),
                            TotalWOQty = row["TotalPlanQty"] != null ? Convert.ToSingle(row["TotalPlanQty"]) : 0,
                            ProdPendQty = row["PendPlanQty"] != null ? Convert.ToSingle(row["PendPlanQty"]) : 0,
                            PlannedMachineid1 = row["Machine1"] != "" ? Convert.ToInt32(row["Machine1"]) : 0,
                            PlannedMachineid2 = row["Machine2"] != "" ? Convert.ToInt32(row["Machine2"]) : 0,
                            PlannedMachineid3 = row["Machine3"] != "" ? Convert.ToInt32(row["Machine3"]) : 0,
                            //workcenter = row["WorkCenter"]?.ToString(),
                            BOMNO = row["Bomno"] != DBNull.Value ? Convert.ToInt32(row["Bomno"]) : 0,
                            BomEffDate = row["BomEffectiveDate"].ToString(),
                            InhouseJOBProd = row["InhouseJW"]?.ToString(),
                            //prodins = row["Prodinstruction"]?.ToString(),
                            QCMandatory = row["QC"]?.ToString(),
                            SONo = row["sono"]?.ToString(),
                            CustOrderNo = row["CustomerOrderNo"]?.ToString(),
                            SOYearCode = row["SOYearCode"] != null ? Convert.ToInt32(row["SOYearCode"]) : 0,
                            //DrawingNo = row["drawingNo"]?.ToString(),
                            SchDate = row["schdate"] != null ? row["schdate"].ToString() : string.Empty,
                            AccountCode = row["AccountCode"] != null ? Convert.ToInt32(row["AccountCode"]) : 0,
                            WONo = row["PlanNo"] != null ? row["PlanNo"].ToString() : string.Empty,
                            WOEntryId = row["PLanEntryId"] != null ? Convert.ToInt32(row["PLanEntryId"]) : 0,
                            WOYearCode = row["PlanYearCode"] != null ? Convert.ToInt32(row["PlanYearCode"]) : 0,
                            ItemCode = row["itemCode"] != DBNull.Value ? Convert.ToInt32(row["itemCode"]) : 0,
                            //ProdInWC = row["Workcenter"] != DBNull.Value ? Convert.ToInt32(row["Workcenter"]) : 0

                        });
                    }
                    //ItemGrid = ItemGrid.OrderBy(item => item.SeqNo).ToList();
                    model.ProductionScheduleDetails = ItemGrid;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
