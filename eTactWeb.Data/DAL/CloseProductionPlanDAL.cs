using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CloseProductionPlanDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CloseProductionPlanDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetOpenItemName(int EmpId, int ActualEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayOpenItemName"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@ActualEntryId", ActualEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCloseOpenProdPlanProdSch", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> GetOpenPlanNo(int EmpId, int ActualEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayOpenPlanNo"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@ActualEntryId", ActualEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCloseOpenProdPlanProdSch", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> GetCloseItemName(int EmpId, int ActualEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayCloseItemName"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@ActualEntryId", ActualEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCloseOpenProdPlanProdSch", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> GetClosePlanNo(int EmpId, int ActualEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayClosePlanNo"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@ActualEntryId", ActualEntryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCloseOpenProdPlanProdSch", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveCloseProductionPlan(CloseProductionPlanModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var sqlParams = new List<dynamic>();
                    sqlParams.Add(new SqlParameter("@Flag", "Insert"));
                    sqlParams.Add(new SqlParameter("@CloseOpen", model.CloseOpen));
                    sqlParams.Add(new SqlParameter("@Approvedby", model.EmpId));
                    sqlParams.Add(new SqlParameter("@ApprovalDate", model.EntryDate));
                sqlParams.Add(new SqlParameter("@dt", GIGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCloseOpenProdPlanProdSch", sqlParams);

            }
            catch (Exception ex)
            {
                // Set error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<CloseProductionPlanModel> GetGridDetailData(int EmpId, string ActualEntryByEmpName, string ReportType,string FromDate,string ToDate,string CloseOpen)
        {

            DataSet? oDataSet = new DataSet();
            var model = new CloseProductionPlanModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPCloseOpenProdPlanProdSch", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType == "SUMMARY")
                    {

                    oCmd.Parameters.AddWithValue("@Flag", "DisplayOpenProdPlanListInSummary");
                    }
                    if (ReportType == "DETAIL")
                    {

                        oCmd.Parameters.AddWithValue("@Flag", "DisplayOpenProdPlanListInDetail");
                    }
                    oCmd.Parameters.AddWithValue("@EmpId", EmpId);
                    oCmd.Parameters.AddWithValue("@ActualEntryByEmpName", ActualEntryByEmpName);
                    oCmd.Parameters.AddWithValue("@FromDate", FromDate);
                    oCmd.Parameters.AddWithValue("@ToDate", ToDate);
                    oCmd.Parameters.AddWithValue("@CloseOpen", CloseOpen);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (ReportType == "SUMMARY")
                    {
                        model.CloseProductionPlanGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                         select new CloseProductionPlanModel
                                                         {
                                                             WONO = dr["WONO"].ToString(),
                                                             WODate = DateTime.Parse(dr["WODate"].ToString()).ToString("dd/MM/yyyy"),
                                                             CloseOpen = dr["WoStataus"].ToString(),
                                                             EntryDate = dr["EntryDate"].ToString(),
                                                             EffectiveFrom = dr["EffectiveFrom"].ToString(),
                                                             EffectiveTill = DateTime.Parse(dr["EffectiveTill"].ToString()).ToString("dd/MM/yyyy"),
                                                             ForMonth = dr["ForMonth"].ToString(),
                                                             RemarkProductSupplyStage = dr["Remarkproductsupplystage"].ToString(),
                                                             RemarkForProduction = dr["RemarkForProduction"].ToString(),
                                                             Approved = dr["Approved"].ToString(),
                                                             ApprovedDate = DateTime.Parse(dr["ApprovedDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             CloseWO = dr["CloseWo"].ToString(),
                                                             CloseDate = dr["CloseDate"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["CloseDate"].ToString())
    ? null
    : DateTime.Parse(dr["CloseDate"].ToString()).ToString("dd/MM/yyyy"),

                                                             DeactivateWO = dr["DeactivateWo"].ToString(),
                                                             DeactivateDate = DateTime.Parse(dr["Deactivatedate"].ToString()).ToString("dd/MM/yyyy"),
                                                             MachineName = dr["MachineName"].ToString(),
                                                             RemarkForRouting = dr["RemarkForRouting"].ToString(),
                                                             RemarkForPacking = dr["RemarkForPacking"].ToString(),
                                                             OtherInstruction = dr["OtherInstruction"].ToString(),
                                                             BillingStatus = dr["BillingStatus"].ToString(),
                                                             PendForRouteSheet = dr["PendForRouteSheet"].ToString(),
                                                             ActualEntryDate = DateTime.Parse(dr["ActualEntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             ActualEntryByEmpName = dr["ActualEntryByEmpName"].ToString(),
                                                             LastUpdatedByEmpName = dr["LastUpdatedByEmpName"].ToString(),
                                                             LastUpdatedDate = DateTime.Parse(dr["LastUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             CC = dr["cc"].ToString(),
                                                             UID = dr["uid"].ToString(),
                                                             EntryId = Convert.ToInt32(dr["Entryid"]),
                                                             YearCode = Convert.ToInt32(dr["Yearcode"])
                                                         }).ToList();
                    }
                    else if (ReportType == "DETAIL")
                    {
                        model.CloseProductionPlanGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                         select new CloseProductionPlanModel
                                                         {
                                                             WONO = dr["WONO"].ToString(),
                                                             WODate = DateTime.Parse(dr["WODate"].ToString()).ToString("dd/MM/yyyy"),
                                                             CloseOpen = dr["WoStataus"].ToString(),
                                                             AccountName = dr["Account_Name"].ToString(),
                                                             ItemName = dr["ItemName"].ToString(),
                                                             PartCode = dr["PartCode"].ToString(),
                                                             WOQty = Convert.ToInt32(dr["WOQty"]),
                                                             FGStock = Convert.ToInt32(dr["FGStock"]),
                                                             WIPStock = Convert.ToInt32(dr["WIPStock"]),
                                                             PendRoutSheetQty = Convert.ToInt32(dr["PendRoutSheetQTy"]),
                                                             PendProdQty = Convert.ToInt32(dr["PendProdQty"]),
                                                             SONO = dr["SONO"].ToString(),
                                                             CustomerOrderNo = dr["CustomerOrderNo"].ToString(),
                                                             SODate = DateTime.Parse(dr["SODATE"].ToString()).ToString("dd/MM/yyyy"),
                                                             SchNo = dr["SchNo"].ToString(),
                                                             SchDate = DateTime.Parse(dr["SCHDATE"].ToString()).ToString("dd/MM/yyyy"),
                                                             Color = dr["COLOR"].ToString(),
                                                             OrderQty = Convert.ToInt32(dr["OrderQty"]),
                                                             Unit = dr["Unit"].ToString(),
                                                             AltUnit = dr["AltUnit"].ToString(),
                                                             AltOrderQty = Convert.ToInt32(dr["AltOrderQty"]),
                                                             AltQty = Convert.ToInt32(dr["AltQty"]),
                                                             ApprovedDate = DateTime.Parse(dr["ApprovedDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             EffectiveFrom = DateTime.Parse(dr["EffectiveFrom"].ToString()).ToString("dd/MM/yyyy"),
                                                             EffectiveTill = DateTime.Parse(dr["EffectiveTill"].ToString()).ToString("dd/MM/yyyy"),
                                                             ForMonth = dr["ForMonth"].ToString(),
                                                             CloseWO = dr["CloseWo"].ToString(),
                                                             CloseDate = dr["CloseDate"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["CloseDate"].ToString())
    ? null
    : DateTime.Parse(dr["CloseDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             CloseBy = dr["CloseBy"].ToString(),
                                                             RemarkProductSupplyStage = dr["Remarkproductsupplystage"].ToString(),
                                                             RemarkForProduction = dr["RemarkForProduction"].ToString(),
                                                             Approved = dr["Approved"].ToString(),
                                                             DrawingNo = dr["drawingNo"].ToString(),
                                                             ProdInst1 = dr["ProdInst1"].ToString(),
                                                             ProdInst2 = dr["ProdInst2"].ToString(),
                                                             SOInstruction = dr["SOInstruction"].ToString(),
                                                             PkgInstruction = dr["PkgInstruction"].ToString(),
                                                             PendingRouteForSheet = dr["PendingRouteForSheet"].ToString(),
                                                             RouteSheetNo = dr["RouteSheetNo"].ToString(),
                                                             RouteSheetDate = DateTime.Parse(dr["RouteSheetDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             OrderType = dr["OrderType"].ToString(),
                                                             OrderWEF = dr["OrderWEF"].ToString(),
                                                             SOCloseDate = DateTime.Parse(dr["SOCloseDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             SchEffTillDate = DateTime.Parse(dr["SchEffTillDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             StoreStock = Convert.ToInt32(dr["StoreStock"]),
                                                             ItemDescription = dr["ItemDescription"].ToString(),
                                                             MachineName = dr["MachineName"].ToString(),
                                                             RemarkForRouting = dr["RemarkForRouting"].ToString(),
                                                             RemarkForPacking = dr["RemarkForPacking"].ToString(),
                                                             OtherInstruction = dr["OtherInstruction"].ToString(),
                                                             PendForRouteSheet = dr["PendForRouteSheet"].ToString(),
                                                             DeactivateWO = dr["DeactivateWo"].ToString(),
                                                             DeactivateDate = DateTime.Parse(dr["Deactivatedate"].ToString()).ToString("dd/MM/yyyy"),
                                                             ActualEntryDate = DateTime.Parse(dr["ActualEntryDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             ActualEntryByEmpName = dr["ActualEntryByEmpName"].ToString(),
                                                             LastUpdatedByEmpName = dr["LastUpdatedByEmpName"].ToString(),
                                                             LastUpdatedDate = DateTime.Parse(dr["LastUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             ApproxStartDate = DateTime.Parse(dr["ApproxStartDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             ApproxEndDate = DateTime.Parse(dr["ApproxEndDate"].ToString()).ToString("dd/MM/yyyy"),
                                                             EntryId = Convert.ToInt32(dr["Entryid"]),
                                                             EntryDate = DateTime.Parse(dr["Entrydate"].ToString()).ToString("dd/MM/yyyy"),
                                                             YearCode = Convert.ToInt32(dr["Yearcode"])

                                                         }).ToList();
                    }
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

    }
}
