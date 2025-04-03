using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class MRPDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }

        public MRPDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
           
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> PendingMRPData(PendingMRP model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "MRP"));
                SqlParams.Add(new SqlParameter("@Month", model.Month));
                SqlParams.Add(new SqlParameter("@yearcode", model.YearCode));
                SqlParams.Add(new SqlParameter("@accountcode", model.accountcode));
                SqlParams.Add(new SqlParameter("@SONO", model.sono));
                SqlParams.Add(new SqlParameter("@SOYearcode", model.SOYearCode));
                SqlParams.Add(new SqlParameter("@schNo", model.ScheduleNo));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PendingSOForMRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
        public async Task<ResponseResult> GetStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "GetStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRPGENERATION", SqlParams);
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
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "GetWorkCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRPGENERATION", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
        public async Task<ResponseResult> IsCheckMonthWiseData(int Month, int Year)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "IsCheckMonthWiseData"));
                SqlParams.Add(new SqlParameter("@FirstMonth", Month));
                SqlParams.Add(new SqlParameter("@YearCode", Year));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRP", SqlParams);
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
        public async Task<ResponseResult> SaveMRPDetail(MRPMain model, DataTable MRPGrid, DataTable MRPSOGrid, DataTable MRPFGGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                DateTime entDt = new DateTime();
                DateTime MRPDt = new DateTime();
                DateTime EffDt = new DateTime();
                DateTime MrpRevDt = new DateTime();
                DateTime MRPfirstDt = new DateTime();

                entDt = ParseDate(model.Entry_Date);
                MRPDt = ParseDate(model.MRPDate);
                EffDt = ParseDate(model.EffectiveFromDate);
                MrpRevDt = ParseDate(model.MRPREvDate);
                MRPfirstDt = ParseDate(model.MrpFirstDate);


                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    //SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy == 0 ? 0 : model.LastUpdatedBy));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@MRPNO", model.MRPNo));
                SqlParams.Add(new SqlParameter("@MRPDate", MRPDt == default ? string.Empty : MRPDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@MRPRevNo", model.MRPRevNo));
                SqlParams.Add(new SqlParameter("@CreatedByEmpId", model.CreatedByEmpId));
                SqlParams.Add(new SqlParameter("@MRPREvDate", MrpRevDt == default ? string.Empty : MrpRevDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@FirstMonth", model.FirstMonth == 0 ? 0 : model.FirstMonth));
                SqlParams.Add(new SqlParameter("@EffectiveFromDate", EffDt == default ? string.Empty : EffDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@MrpFirstDate", MRPfirstDt == default ? string.Empty : MRPfirstDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@MrpComplete", model.MrpComplete ?? string.Empty));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy == 0 ? 0 : model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.MachineNo));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy == 0 ? 0 : model.LastUpdatedBy));

                SqlParams.Add(new SqlParameter("@DTSSGrid", MRPGrid));
                SqlParams.Add(new SqlParameter("@DTSSGridSODetail", MRPSOGrid));
                SqlParams.Add(new SqlParameter("@DTSSGridFGRMDetail", MRPFGGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRP", SqlParams);
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
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRPGENERATION", SqlParams);
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Pending MRP"));                

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
        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string MRPNo, string EntryByMachineName, int CreatedByEmpId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@Entryid", ID));
                SqlParams.Add(new SqlParameter("@yearcode", YC));
                SqlParams.Add(new SqlParameter("@MRPNO", MRPNo));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
                SqlParams.Add(new SqlParameter("@CreatedByEmpId", CreatedByEmpId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetDashboardData(MRPDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime StDt = new DateTime();
                StDt = ParseDate(model.FromDate);
                DateTime EndDt = new DateTime();
                EndDt = ParseDate(model.ToDate);
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@MRPNo", model.MRPNo));
                SqlParams.Add(new SqlParameter("@FromDate", StDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", EndDt.ToString("yyyy/MM/dd")));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<MRPMain> GetMRPDetailData(PendingMRP model)
        {
            DataSet? oDataSet = new DataSet();
            var Mainmodel = new MRPMain();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_MRPGENERATION", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("Flag", "GetDATA");
                    oCmd.Parameters.AddWithValue("@so_no", model.sono);
                    oCmd.Parameters.AddWithValue("@so_yearcode", model.SOYearCode);
                    oCmd.Parameters.AddWithValue("@Consider_Stock_Stores", model.StoreId);
                    oCmd.Parameters.AddWithValue("@Consider_WIP", model.WcId);
                    oCmd.Parameters.AddWithValue("@Sch_entryid", model.ScheduleNo);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    int seqNo = 1;
                    Mainmodel.MRPGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                         select new MRPDetail
                                         {
                                            // SeqNo = seqNo++,
                                             MRPEntryId = model.MRPEntryId,
                                             MRPNo = model.MRPNo,
                                             Month = model.Month,
                                             ForMonthYear = model.ForMonthYear,
                                             FGItemCode = Convert.ToInt32(row["FinishedItem"]),
                                             FGPartCode = row["FGPartCode"].ToString(),
                                             FGItemName = row["FGItemName"].ToString(),
                                             RMItemCode = Convert.ToInt32(row["RMitemCode"]),
                                             RMPartCode = row["ReqPartCode"].ToString(),
                                             RMItemName = row["ReqItemDesc"].ToString(),
                                             ReqQty = Convert.ToInt32(row["ReqQty"]),
                                             Unit = row["Unit"].ToString(),
                                             CurrMonthQty = Convert.ToInt32(row["CurrentStock"]),
                                             WIPStock = Convert.ToInt32(row["WIP"]),
                                             TotalStock = Convert.ToInt32(row["TotalStock"]),
                                             RecorderLvl = Convert.ToInt32(row["ReorderLvl"]),
                                             OrderQtyInclPrevPOQty = Convert.ToInt32(row["OrderQty"]),
                                             PORate = Convert.ToInt32(row["LastPORate"]),
                                             AllocatedQty = Convert.ToInt32(row["AllocatedQty"]),
                                             ConsumedQty = Convert.ToInt32(row["ConsumedQty"]),
                                             AccountName = row["AccountName"].ToString(),
                                             PrevOrderQty = Convert.ToInt32(row["POQty"]),
                                             IIndMonthQty = Convert.ToInt32(row["2ndMonthReqQty"]),
                                             IIrdMonthQty = Convert.ToInt32(row["3rdMonthReqQty"]),
                                             IIndMonthActualReqQty = Convert.ToInt32(row["2ndMonthActualReqQty"]),
                                             IIrdMonthActualReqQty = Convert.ToInt32(row["3rdMonthActualReqQty"]),

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
            return Mainmodel;
        }

        //public async Task<MRPMain> GetMRPDetailData(PendingMRP model)
        //{
        //    var Mainmodel = new MRPMain();
        //    try
        //    {
        //        var Date = DateTime.Now;

        //        var SqlParams = new List<dynamic>();
        //        SqlParams.Add(new SqlParameter("Flag", "GetDATA"));
        //        SqlParams.Add(new SqlParameter("@so_no", model.sono));
        //        SqlParams.Add(new SqlParameter("@so_yearcode", model.SOYearCode));
        //        SqlParams.Add(new SqlParameter("@Consider_Stock_Stores", model.StoreId));
        //        SqlParams.Add(new SqlParameter("@Consider_WIP", model.WcId));
        //        SqlParams.Add(new SqlParameter("@Sch_entryid", model.ScheduleNo));
        //        var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRPGENERATION", SqlParams);

        //        if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
        //        {
        //            var MainList = MRPDetailList(model.MRPEntryId, model.MRPNo, model.Month, model.ForMonthYear, ResponseResult.Result, ref model);
        //            Mainmodel.MRPGrid = MainList;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return Mainmodel;

        //}

        public async Task<MRPMain> GetMRPFGRMDetailData(PendingMRP model)
        {
            var Mainmodel = new MRPMain();
            try
            {
                var Date = DateTime.Now;

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "GetFGRMDATA"));
                SqlParams.Add(new SqlParameter("@so_no", model.sono));
                SqlParams.Add(new SqlParameter("@so_yearcode", model.SOYearCode));
                SqlParams.Add(new SqlParameter("@Consider_Stock_Stores", model.StoreId));
                SqlParams.Add(new SqlParameter("@Consider_WIP", model.WcId));
                SqlParams.Add(new SqlParameter("@Sch_entryid", model.ScheduleNo));
                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRPGENERATION", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    var MainList = MRPFGRMDetailList(ResponseResult.Result, ref model);
                    Mainmodel.MRPFGRMGrid = MainList;
                }

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Mainmodel;

        }

        internal async Task<MRPMain> GetViewByID(int ID, string Mode, int YC)
        {
            var model = new MRPMain();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRP", SqlParams);

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

        private static MRPMain PrepareView(DataSet DS, ref MRPMain? model, string Mode)
        {
            var ItemList = new List<MRPDetail>();
            var SOList = new List<MRPSaleOrderDetail>();
            var FGList = new List<MRPFDRMDetail>();
            DS.Tables[0].TableName = "VMRPMAINDETAIL";
            //DS.Tables[1].TableName = "VMRPMAINDETAIL";
            DS.Tables[1].TableName = "VMRPSaleOrderDetail";
            DS.Tables[2].TableName = "MrpFGRMDetail";

            int cnt = 1;

            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.Entry_Date = DS.Tables[0].Rows[0]["Entrydate"].ToString();
            model.MRPNo = DS.Tables[0].Rows[0]["MRPNo"].ToString();
            model.MRPDate = DS.Tables[0].Rows[0]["MRPDate"].ToString();
            model.MRPRevNo = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPRevNo"].ToString());
            model.CreatedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedByEmpId"]);
            model.CreatedByEmpName = DS.Tables[0].Rows[0]["CreatedByEmpName"].ToString();
            model.MRPREvDate = DS.Tables[0].Rows[0]["MRPREvDate"].ToString();
            model.FirstMonth = Convert.ToInt32(DS.Tables[0].Rows[0]["ForMonth"].ToString());
            model.EffectiveFromDate = DS.Tables[0].Rows[0]["EffectiveFromDate"].ToString();
            model.MrpFirstDate = DS.Tables[0].Rows[0]["MrpFirstDate"].ToString();
            model.MrpComplete = DS.Tables[0].Rows[0]["MrpComplete"].ToString();
            model.UID = DS.Tables[0].Rows[0]["UID"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.ActualEnteredDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["CreatedByEmpName"].ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]);
            model.MachineNo = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();

            if (Mode == "U" || Mode == "V")
            {
                if (DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString() != "")
                {
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdatedByEmpName = DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString();
                    model.LastUpdatedDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString().Split(" ")[0];
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new MRPDetail
                    {
                        SeqNo = cnt++,
                        FGItemCode = Convert.ToInt32(row["FGItemCode"]),
                        FGItemName = row["FGItemName"].ToString(),
                        FGPartCode = row["FGPartCode"].ToString(),
                        RMItemCode = Convert.ToInt32(row["RMItemCode"]),
                        RMItemName = row["RMItemName"].ToString(),
                        RMPartCode = row["RMPartCode"].ToString(),
                        ReqQty = Convert.ToInt32(row["ReqQty"]),
                        CurrMonthQty = Convert.ToInt32(row["CurrMonthOrderQty"]),
                        IIndMonthQty = Convert.ToInt32(row["IIndMonthQty"]),
                        IIrdMonthQty = Convert.ToInt32(row["IIIrdMonthQty"]),
                        StoreStock = Convert.ToInt32(row["StoreStock"]),
                        WIPStock = Convert.ToInt32(row["WIPStock"]),
                        TotalStock = Convert.ToInt32(row["TotalStock"]),
                        MinLevel = Convert.ToInt32(row["MinLevel"]),
                        RecorderLvl = Convert.ToInt32(row["ReorderLvl"]),
                        Unit = row["Unit"].ToString(),
                        AltUnit = row["AltUnit"].ToString(),
                        AllocatedQty = Convert.ToInt32(row["AllocatedQty"]),
                        ConsumedQty = Convert.ToInt32(row["ConsumedQty"].ToString()),
                        PORate = Convert.ToInt32(row["PORate"]),
                        LeadTime = Convert.ToInt32(row["LeadTime"]),
                        BOMExist = row["BOMExist"].ToString(),
                        POExist = row["POExist"].ToString(),
                        MaxPORate = Convert.ToInt32(row["MaxPORate"]),
                        AccountName = row["AccountName"].ToString(),
                        AccountCode = Convert.ToInt32(row["AccountCode"]),
                    });
                }
                model.MRPGrid = ItemList;
            }

            int cntt = 1;

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    SOList.Add(new MRPSaleOrderDetail
                    {
                        SeqNo = cntt++,
                        SONo = row["SONo"].ToString(),
                        SOYearCode = Convert.ToInt32(row["SOYearCode"]),
                        SODAte = row["SODAte"].ToString(),
                        ScheduleNo = row["ScheduleNo"].ToString(),
                        SchYearCode = Convert.ToInt32(row["SchYearCode"]),
                        AccountCode = Convert.ToInt32(row["AccountCode"]),
                        AccountName = row["AccountName"].ToString(),
                        Months = row["Months"].ToString(),
                        MonthYear = Convert.ToInt32(row["MonthYear"]),
                        BOMExist = row["BOMExist"].ToString(),
                    });
                }
                model.MRPSOGrid = SOList;
            }

            int cntFG = 1;

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    FGList.Add(new MRPFDRMDetail
                    {
                        SeqNo = cntFG++,
                        FGPartCode = row["FGPartCode"].ToString(),
                        FGItemName = row["FGItemName"].ToString(),
                        RMPartCode = row["RMPartCode"].ToString(),
                        RMItemName = row["RMItemName"].ToString(),
                        BOMNo = Convert.ToInt32(row["BOMNo"]),
                        CurrMonthOrderQty = Convert.ToInt32(row["CurrMonthOrderQty"]),
                        BOMEffDate = row["BOMEffDate"].ToString(),
                        BOMQty = Convert.ToInt32(row["BOMQty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        AllocatedQty = Convert.ToInt32(row["AllocatedQty"]),
                        FGQty = Convert.ToInt32(row["FGQty"]),
                        OrderQtyInclPrevPOQty = Convert.ToInt32(row["OrderQtyInclPrevPOQty"]),
                    });
                }
                model.MRPFGRMGrid = FGList;
            }

            return model;
        }

        private static List<MRPDetail> MRPDetailList(int MRPEntryID, string MRPNo, int Month, int ForMonthYear, DataSet DS, ref PendingMRP? model)
        {
            var ItemList = new List<MRPDetail>();
            DS.Tables[0].TableName = "MRPMain";

            int cnt = 1;


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new MRPDetail
                    {
                        SeqNo = cnt++,
                        MRPEntryId = MRPEntryID,
                        MRPNo = MRPNo,
                        Month = Month,
                        ForMonthYear = ForMonthYear,
                        FGItemCode = Convert.ToInt32(row["FinishedItem"]),
                        FGPartCode = row["FGPartCode"].ToString(),
                        FGItemName = row["FGItemName"].ToString(),
                        RMItemCode = Convert.ToInt32(row["RMitemCode"]),
                        RMPartCode = row["ReqPartCode"].ToString(),
                        RMItemName = row["ReqItemDesc"].ToString(),
                        ReqQty = Convert.ToInt32(row["ReqQty"]),
                        Unit = row["Unit"].ToString(),
                        CurrMonthQty = Convert.ToInt32(row["CurrentStock"]),
                        WIPStock = Convert.ToInt32(row["WIP"]),
                        TotalStock = Convert.ToInt32(row["TotalStock"]),
                        RecorderLvl = Convert.ToInt32(row["ReorderLvl"]),
                        OrderQtyInclPrevPOQty = Convert.ToInt32(row["OrderQty"]),
                        PORate = Convert.ToInt32(row["LastPORate"]),
                        AllocatedQty = Convert.ToInt32(row["AllocatedQty"]),
                        ConsumedQty = Convert.ToInt32(row["ConsumedQty"]),
                        AccountName = row["AccountName"].ToString(),
                        PrevOrderQty = Convert.ToInt32(row["POQty"].ToString()),
                        IIndMonthQty = Convert.ToInt32(row["2ndMonthReqQty"]),
                        IIrdMonthQty = Convert.ToInt32(row["3rdMonthReqQty"]),
                        IIndMonthActualReqQty = Convert.ToInt32(row["2ndMonthActualReqQty"]),
                        IIrdMonthActualReqQty = Convert.ToInt32(row["3rdMonthActualReqQty"]),
                    });
                }
            }

            return ItemList;
        }

        private static List<MRPFDRMDetail> MRPFGRMDetailList(DataSet DS, ref PendingMRP? model)
        {
            var ItemList = new List<MRPFDRMDetail>();
            DS.Tables[0].TableName = "MRPMain";

            int cnt = 1;


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new MRPFDRMDetail
                    {
                        SeqNo = cnt++,
                        FGPartCode = row["FGPartCode"].ToString(),
                        FGItemName = row["FGItemName"].ToString(),
                        FGQty = Convert.ToInt32(row["FGQty"]),
                        RMPartCode = row["RMPartCode"].ToString(),
                        RMItemName = row["RMItemName"].ToString(),
                        BOMQty = Convert.ToInt32(row["BOMQty"]),
                        CurrMonthOrderQty = Convert.ToInt32(row["CurreMonthOrderQty"]),
                        OrderQtyInclPrevPOQty = Convert.ToInt32(row["OrderQtyInclPrevPOQty"]),
                        AltUnit = row["AlternateUnit"].ToString(),
                        AllocatedQty = Convert.ToInt32(row["allocatedqty"]),
                        BOMEffDate = row["Effectivedate"].ToString(),
                        BOMNo = Convert.ToInt32(row["bomno"]),
                        FGItemCode = Convert.ToInt32(row["finishitemcode"]),
                        RMItemCode = Convert.ToInt32(row["RAW_ItemCode"]),
                    });
                }
            }

            return ItemList;
        }


    }
}
