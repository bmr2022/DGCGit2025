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

namespace eTactWeb.Data.DAL
{
    public class ReceiveItemDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        public ReceiveItemDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> FillEntryId(string Flag, int yearCode, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveMaterialInStore", SqlParams);
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
        internal async Task<ReceiveItemModel> GetViewByID(int ID, int YearCode)
        {
            var model = new ReceiveItemModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@yearcode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);

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
        private static ReceiveItemModel PrepareView(DataSet DS, ref ReceiveItemModel? model)
        {
            var ReceiveItemDetail = new List<ReceiveItemDetail>();
            DS.Tables[0].TableName = "ReceiveItem";
            int cnt = 0;
            model.RecMatEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["RecMatEntryId"].ToString());
            model.RecMatYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RecMatYearCode"].ToString());
            model.RecMatEntryDate = DS.Tables[0].Rows[0]["RecMatEntryDate"].ToString();
            model.RecMatSlipNo = DS.Tables[0].Rows[0]["RecMatSlipNo"].ToString();
            model.RecMatSlipDate=DS.Tables[0].Rows[0]["RecMatSlipDate"].ToString();
            model.RecMatEntryDate=DS.Tables[0].Rows[0]["ReceiveDate"].ToString();
            model.EnteredbyMachineName = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmpid"].ToString());
            model.ActualEntrydate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.CC=DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID=Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.DepID=Convert.ToInt32(DS.Tables[0].Rows[0]["FromDepID"].ToString());

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedByEmpId"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"].ToString();
                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedByEmpId"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedByEmpId"]);
                model.UpdatedOn = DS.Tables[0].Rows[0]["UpdationDate"].ToString();
            }
            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ReceiveItemDetail.Add(new ReceiveItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        MaterialType=row["MaterialType"].ToString(),
                        FromDepWorkCenter = row["FromDepWorkCenter"].ToString(),
                        WCID = Convert.ToInt32(row["FromWCID"].ToString()),
                        FromWorkcenter = row["FromWorkCenter"].ToString(),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        RecQty = Convert.ToDecimal(row["ActualRecQtyInStr"].ToString()),
                        AltTransferQty = Convert.ToDecimal(row["ActualTransferQtyFrmWC"].ToString()),
                        Unit = row["Unit"].ToString(),
                        Qty = Convert.ToInt32(row["AltQty"].ToString()),
                        AltUnit = row["AltUnit"].ToString(),
                        Itemremark = row["Remark"].ToString(),
                        RecStoreId = Convert.ToInt32(row["RecInStore"].ToString()),
                        RecInStore = row["StoreName"].ToString(),
                        ProdEntryId = Convert.ToInt32(row["Prodentryid"].ToString()),
                        ProdYearCode = Convert.ToInt32(row["ProdyearCode"].ToString()),
                        ProdEntryDate = row["ProdDateAndTime"].ToString(),
                        PlanNoEntryId = Convert.ToInt32(row["PlanNoEntryId"].ToString()),
                        ProdPlanNo = row["ProdPlanNo"].ToString(),
                        ProdPlanYearCode = Convert.ToInt32(row["ProdPlanYearCode"].ToString()),
                        ProdSchEntryId = Convert.ToInt32(row["ProdSchEntryId"].ToString()),
                        ProdSchNo = row["ProdSchNo"].ToString(),
                        ProdSchYearCode = Convert.ToInt32(row["ProdSchYearCode"].ToString()),
                        InProcQCSlipNo = (row["InProcQCSlipNo"].ToString()),
                        InProcQCEntryId = Convert.ToInt32(row["InProcQCEntryId"].ToString()),
                        InProcQCYearCode=Convert.ToInt32(row["InProcQCYearCode"]),
                        ProdQty=Convert.ToDecimal(row["ProdQty"].ToString()),
                        RejQty=Convert.ToDecimal(row["RejQty"].ToString()),
                        QCOkQty=Convert.ToDecimal(row["QCOkQty"].ToString()),
                        BatchNo = row["Batchno"].ToString(),
                        uniquebatchno = (row["UniqueBatchno"].ToString()),
                        TransferMatEntryId=Convert.ToInt32(row["TransferMatEntryId"].ToString()),
                        TransferMatYearCode=Convert.ToInt32(row["TransferMatYearCode"].ToString()),
                        TransferMatSlipNo=row["TransferMatSlipNo"].ToString()
                    });
                }
                model.ItemDetailGrid = ReceiveItemDetail;
            }
            return model;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", firstDateOfMonth.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", currentDate.ToString("yyyy/MM/dd")));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveMaterialInStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ReceiveItemDashboard> GetDashboardData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new ReceiveItemDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ReceiveMaterialInStore", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
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
                    model.ReceiveItemDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new ReceiveItemDetailDashboard
                                                {
                                                    RecMatEntryId = Convert.ToInt32(dr["RecMatEntryId"]),
                                                    RecMatYearCode = Convert.ToInt32(dr["RecMatYearCode"]),
                                                    RecMatEntryDate = dr["RecMatEntryDate"].ToString().Split(" ")[0],
                                                    RecMatSlipNo =dr["RecMatSlipNo"].ToString(),
                                                    RecMatSlipDate=dr["RecMatSlipDate"].ToString().Split(" ")[0],
                                                    CC=dr["CC"].ToString(),
                                                    ActualEntryByEmpid=Convert.ToInt32(dr["ActualEntryByEmpid"]),
                                                    ActualEntryByname=dr["ActualEntryByname"].ToString(),
                                                    ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                    UpdatedByName=dr["UpdatedByName"].ToString(),
                                                    UpdationDate=dr["UpdationDate"].ToString().Split(" ")[0],
                                                    EntryByMachine=dr["EntryByMachine"].ToString(),
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
        public async Task<ReceiveItemDashboard> GetDashboardDetailData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new ReceiveItemDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ReceiveMaterialInStore", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
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
                    model.ReceiveItemDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new ReceiveItemDetailDashboard
                                                {
                                                    RecMatEntryId = Convert.ToInt32(dr["RecMatEntryId"]),
                                                    RecMatYearCode = Convert.ToInt32(dr["RecMatYearCode"]),
                                                    RecMatEntryDate = dr["RecMatEntryDate"].ToString().Split(" ")[0],
                                                    RecMatSlipNo =dr["RecMatSlipNo"].ToString(),
                                                    RecMatSlipDate=dr["RecMatSlipDate"].ToString().Split(" ")[0],
                                                    ReceiveDate =dr["ReceiveDate"].ToString().Split(" ")[0],
                                                    MaterialType = dr["MaterialType"].ToString(),
                                                    FromDepWorkCenter =dr["FromDepWorkCenter"].ToString(),
                                                    ItemName =dr["ItemName"].ToString(),
                                                    PartCode =dr["PartCode"].ToString(),
                                                    ActualRecQtyInStr = Convert.ToDecimal(dr["ActualRecQtyInStr"]),
                                                    ActualTransferQtyFrmWC = Convert.ToDecimal(dr["ActualTransferQtyFrmWC"]),
                                                    Unit = dr["Unit"].ToString(),
                                                    AltQty = Convert.ToDecimal(dr["AltQty"]),
                                                    AltUnit =dr["AltUnit"].ToString(),
                                                    QCOkQty = Convert.ToDecimal(dr["QCOkQty"]),
                                                    Remark = dr["Remark"].ToString(),
                                                    StoreName =dr["StoreName"].ToString(),
                                                    CC = dr["CC"].ToString(),
                                                    Prodentryid = Convert.ToInt32(dr["Prodentryid"]),
                                                    ProdyearCode = Convert.ToInt32(dr["ProdyearCode"]),
                                                    ProdDateAndTime = dr["ProdDateAndTime"].ToString(),
                                                    PlanNoEntryId = Convert.ToInt32(dr["PlanNoEntryId"]),
                                                    ProdPlanNo=dr["ProdPlanNo"].ToString(),
                                                    ProdPlanYearCode = Convert.ToInt32(dr["ProdPlanYearCode"]),
                                                    ProdSchEntryId = Convert.ToInt32(dr["ProdSchEntryId"]),
                                                    ProdSchNo=dr["ProdSchNo"].ToString(),
                                                    ProdSchYearCode = Convert.ToInt32(dr["ProdSchYearCode"]),
                                                    InProcQCSlipNo=dr["InProcQCSlipNo"].ToString(),
                                                    InProcQCEntryId = Convert.ToInt32(dr["InProcQCEntryId"]),
                                                    InProcQCYearCode = Convert.ToInt32(dr["InProcQCYearCode"]),
                                                    ProdQty = Convert.ToDecimal(dr["ProdQty"]),
                                                    RejQty = Convert.ToDecimal(dr["RejQty"]),
                                                    Batchno=dr["Batchno"].ToString(),
                                                    UniqueBatchno=dr["UniqueBatchno"].ToString(),
                                                    TransferMatEntryId = Convert.ToInt32(dr["TransferMatEntryId"]),
                                                    TransferMatYearCode = Convert.ToInt32(dr["TransferMatYearCode"]),
                                                    TransferMatSlipNo=dr["TransferMatSlipNo"].ToString(),
                                                    UID = Convert.ToInt32(dr["UID"]),
                                                    ActualEntryByEmpid = Convert.ToInt32(dr["ActualEntryByEmpid"]),
                                                    ActualEntryByname=dr["ActualEntryByname"].ToString(),
                                                    ActualEntryDate=dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                    UpdatedByEmpId = Convert.ToInt32(dr["UpdatedByEmpId"]),
                                                    UpdatedByName=dr["UpdatedByName"].ToString(),
                                                    UpdationDate=dr["UpdationDate"].ToString().Split(" ")[0],
                                                    EntryByMachine=dr["EntryByMachine"].ToString()
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
        public async Task<ResponseResult> BindDepartmentList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DepartmentNameList"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveMaterialInStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveInprocessQc(ReceiveItemModel model, DataTable ReceiveItemDetail)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime entryDate = new DateTime();
                //DateTime updationDate = new DateTime();
                //DateTime recmatDate=new DateTime();

                var entryDate =CommonFunc.ParseFormattedDate(model.RecMatEntryDate);
                var updationDate=CommonFunc.ParseFormattedDate(model.UpdatedOn);
                var recmatDate=CommonFunc.ParseFormattedDate(model.RecMatSlipDate);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@updatedByEmpid", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@updationdate", updationDate));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryID", model.RecMatEntryId));
                SqlParams.Add(new SqlParameter("@yearcode", model.RecMatYearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entryDate));
                SqlParams.Add(new SqlParameter("@DepId", model.DepID));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EnteredbyMachineName));
                SqlParams.Add(new SqlParameter("@ActualEntrydate", entryDate));
                SqlParams.Add(new SqlParameter("@RecMatSlipNo", model.RecMatSlipNo));
                SqlParams.Add(new SqlParameter("@Uid", model.UID));
                SqlParams.Add(new SqlParameter("@RecMatSlipDate", recmatDate));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@ReceiveDate", entryDate));
                

                SqlParams.Add(new SqlParameter("@DTSSGrid", ReceiveItemDetail));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveMaterialInStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string RecMatSlipNo, string EntryByMachineName, int ActualEntryBy)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@yearcode", YC));
                SqlParams.Add(new SqlParameter("@RecMatSlipNo", RecMatSlipNo));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", ActualEntryBy));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveMaterialInStore", SqlParams);
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
