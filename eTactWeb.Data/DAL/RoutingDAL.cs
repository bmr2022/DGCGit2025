using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;

namespace eTactWeb.Data.DAL
{
    public class RoutingDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public RoutingDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Routing", SqlParams);
                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    ResponseResult.Result.Tables[0].TableName = "BranchList";
                    ResponseResult.Result.Tables[1].TableName = "StageList";
                    ResponseResult.Result.Tables[2].TableName = "EmpList";
                    ResponseResult.Result.Tables[4].TableName = "MachineList";
                    ResponseResult.Result.Tables[3].TableName = "WorkCenterList";
                    ResponseResult.Result.Tables[3].TableName = "TransfertoWCList";

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
        public async Task<ResponseResult> FillStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillItems(string Flag)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));

                Result = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> AlreadyExistItems(string Flag)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));

                Result = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }

        public async Task<ResponseResult> FillSubItems(string Flag)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));

                Result = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<RoutingModel> GetAllDataItemWise(string Flag, int ItemCode)
        {
            var Result = new ResponseResult();
            var model = new RoutingModel();
            DataSet DS = new DataSet();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                Result = await _IDataLogic.ExecuteDataSet("SP_Routing", SqlParams);

                if (Result.Result != null && Result.StatusCode == HttpStatusCode.OK && Result.StatusText == "Success")
                {
                    //PrepareView(Result.Result, ref model);
                    model.EntryId = Convert.ToInt32(Result.Result.Tables[0].Rows[0]["EntryId"]);
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

        internal async Task<ResponseResult> SaveRouting(RoutingModel model, DataTable RoutingGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
                SqlParams.Add(new SqlParameter("@RevNo", model.RevNo));
                SqlParams.Add(new SqlParameter("@RevDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@actualEntryDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@LastUpdateDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@LatUpdatedBy", model.LastUpdateBy));

                SqlParams.Add(new SqlParameter("@DTItemGrid", RoutingGrid));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<RoutingGridDashBoard> GetDashboardData(string SummaryDetail, string PartCode, string ItemName, string Stage, string WorkCenter, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new RoutingGridDashBoard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_Routing", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    DateTime FromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@Stage", Stage);
                    oCmd.Parameters.AddWithValue("@WorkCenter", WorkCenter);
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
                        model.RoutingGrid = (from DataRow distinctRow in oDataSet.Tables[0].Rows
                                             group distinctRow by Convert.ToInt32(distinctRow["EntryId"]) into grouped
                                             select grouped.First() into dr
                                             select new RoutingGridDashBoard
                                             {
                                                 StoreID = Convert.ToInt32(dr["TransferToStoreId"]),
                                                 StoreName = dr["TransferToStore"].ToString(),
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 EntryDate = dr["EntryDate"].ToString(),
                                                 ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                                 PartCode = dr["PartCode"].ToString(),
                                                 ItemName = dr["ItemName"].ToString(),
                                                 RouteNo = dr["RouteNo"].ToString(),
                                                 RevNo = Convert.ToInt32(dr["RevNo"]),
                                                 RevDate = dr["RevDate"].ToString(),
                                                 CC = dr["CC"].ToString(),
                                                 UID = Convert.ToInt32(dr["UID"]),
                                                 ActualEntryDate = dr["actualEntryDate"].ToString(),
                                                 ActualEntryByEmpName = dr["ActualEntryByEmpName"].ToString(),
                                                 ActualEntryBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                 LastUpdateDate = dr["LastUpdateDate"].ToString(),
                                                 LastUpdatedByEmpName = dr["LAstUpdatedByEmpName"].ToString(),
                                                 LatUpdatedBy = Convert.ToInt32(dr["LatUpdatedBy"]),

                                             }).ToList();
                    }
                }
                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.RoutingGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new RoutingGridDashBoard
                                             {
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 EntryDate = dr["EntryDate"].ToString(),
                                                 ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                                 PartCode = dr["PartCode"].ToString(),
                                                 ItemName = dr["ItemName"].ToString(),
                                                 StageID = Convert.ToInt32(dr["StageID"]),
                                                 StageDescription = dr["StageDescription"].ToString(),
                                                 MachineGroupID = Convert.ToInt32(dr["MachineGroupID"]),
                                                 MachGroup = dr["MachGroup"].ToString(),
                                                 WorkCenterID = Convert.ToInt32(dr["WorkCenterID"]),
                                                 WorkCenterDescription = dr["WorkCenterDescription"].ToString(),
                                                 TransferToWCID = Convert.ToInt32(dr["TransferToWCID"]),
                                                 TransferWCName = dr["TransferWCName"].ToString(),
                                                 StoreID = Convert.ToInt32(dr["TransferToStoreId"]),
                                                 StoreName = dr["TransferToStore"].ToString(),
                                                 InitialSetupTime = Convert.ToSingle(dr["InitialSetupTime"]),
                                                 LeadTime = Convert.ToSingle(dr["LeadTime"]),
                                                 LeadTimeType = dr["LeadTimeType"].ToString(),
                                                 LeadTimeInMin = Convert.ToSingle(dr["LeadTimeInMin"]),
                                                 Subitemcode = Convert.ToInt32(dr["Subitemcode"]),
                                                 SubitemName = dr["SubItemName"].ToString(),
                                                 SubPartCode = dr["SubPartCode"].ToString(),
                                                 Remark = dr["Remark"].ToString(),
                                                 MandatoryOptionalProcess = dr["MandatoryOptionalProcess"].ToString(),
                                                 NoOfWorkers = Convert.ToInt32(dr["NoOfWorkers"]),
                                                 LaboursCost = Convert.ToInt32(dr["LaboursCost"]),
                                                 ProdCost = Convert.ToSingle(dr["ProdCost"]),
                                                 RouteNo = dr["RouteNo"].ToString(),
                                                 RevNo = Convert.ToInt32(dr["RevNo"]),
                                                 RevDate = dr["RevDate"].ToString(),
                                                 CC = dr["CC"].ToString(),
                                                 UID = Convert.ToInt32(dr["UID"]),
                                                 ActualEntryDate = dr["actualEntryDate"].ToString(),
                                                 ActualEntryByEmpName = dr["ActualEntryByEmpName"].ToString(),
                                                 ActualEntryBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                 LastUpdateDate = dr["LastUpdateDate"].ToString(),
                                                 LastUpdatedByEmpName = dr["LAstUpdatedByEmpName"].ToString(),
                                                 LatUpdatedBy = Convert.ToInt32(dr["LatUpdatedBy"]),

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
        public async Task<ResponseResult> GetNewEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Routing", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFormRights(int ID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", ID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Routing"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Item Group"));

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
        internal async Task<RoutingModel> GetViewByID(int ID, string Mode)
        {
            var model = new RoutingModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Routing", SqlParams);

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
        private static RoutingModel PrepareView(DataSet DS, ref RoutingModel? model, string Mode)
        {
            var ItemGrid = new List<RoutingDetail>();
            DS.Tables[0].TableName = "VRouteMainDetail";
            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.ItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCode"].ToString());
            model.PartCode = DS.Tables[0].Rows[0]["PartCode"].ToString();
            model.ItemName = DS.Tables[0].Rows[0]["ItemName"].ToString();
            model.RouteNo = DS.Tables[0].Rows[0]["RouteNo"].ToString();
            model.RevNo = Convert.ToInt32(DS.Tables[0].Rows[0]["RevNo"].ToString());
            model.RevDate = DS.Tables[0].Rows[0]["Revdate"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.ActualEntryDate = DS.Tables[0].Rows[0]["actualEntryDate"].ToString();
            model.ActualEntryByEmpName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["LAstUpdatedByEmpName"].ToString() != "")
                {
                    model.LastUpdateDate = DS.Tables[0].Rows[0]["LastUpdateDate"].ToString();
                    model.LastUpdateByEmpName = DS.Tables[0].Rows[0]["LAstUpdatedByEmpName"].ToString();
                    model.LastUpdateBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LatUpdatedBy"].ToString());
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemGrid.Add(new RoutingDetail
                    {
                        SequenceNo = Convert.ToInt32(row["SequenceNo"]),
                        RevNo = Convert.ToInt32(row["RevNo"]),
                        StageID = Convert.ToInt32(row["StageID"].ToString()),
                        Stage = row["StageDescription"].ToString(),
                        MachineGroupID = Convert.ToInt32(row["MachineGroupID"]),
                        MachineGroup = row["MachGroup"].ToString(),
                        WorkCenterID = Convert.ToInt32(row["WorkCenterID"]),
                        WorkCenter = row["WorkCenterDescription"].ToString(),
                        TransferToWCID = Convert.ToInt32(row["TransferToWCID"]),
                        TransferToWC = row["TransferWCName"].ToString(),
                        QCRequired = row["NeedQc"].ToString(),
                        IntialSetupTime = (float)Math.Round(Convert.ToSingle(row["InitialSetupTime"]), 2),
                        LeadTime = Convert.ToSingle(row["LeadTime"].ToString()),
                        LeadTimeType = row["LeadTimeType"].ToString(),
                        LeadTimeInMin = Convert.ToDecimal(row["LeadTimeInMin"]),
                        SubItemCode = Convert.ToInt32(row["SubItemCode"].ToString()),
                        SubPartCode = row["SubPartCode"].ToString(),
                        SubItemName = row["SubItemName"].ToString(),
                        Remark = row["Remark"].ToString(),
                        MandatoryOptionalProcess = row["MandatoryOptionalProcess"].ToString(),
                        NoOfWorkers = Convert.ToInt32(row["NoOfWorkers"]),
                        LaboursCost = Convert.ToInt32(row["LaboursCost"]),
                        ProdCost = Convert.ToSingle(row["ProdCost"]),
                        StoreID = Convert.ToInt32(row["TransferToStoreId"]),
                        StoreName = row["TransferToStore"].ToString(),

                    });
                }
                ItemGrid = ItemGrid.OrderBy(item => item.SequenceNo).ToList();
                model.RoutingDetailGrid = ItemGrid;
            }


            return model;
        }
    }
}