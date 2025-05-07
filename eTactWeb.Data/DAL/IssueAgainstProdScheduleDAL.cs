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

namespace eTactWeb.Data.DAL
{
    public class IssueAgainstProdScheduleDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        public IssueAgainstProdScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic)
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
                SqlParams.Add(new SqlParameter("@IssAgtProdSchEntryDate", DateTime.Today));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "BINDWorkcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillIssueByEmp()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BINDIssueByEmployee"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRecByEmp()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BINDRecByEmployee"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
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
        public async Task<ResponseResult> DisplayRoutingDetail(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Showrouting"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<IssueAgainstProdSchedule> GetViewByID(int ID, int YearCode,string ProdSchSlipNo)
        {
            var model = new IssueAgainstProdSchedule();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@EntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@IssAgtProdSchSlipNo", ProdSchSlipNo));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueAgainstProdSchedule", SqlParams);

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
        private static IssueAgainstProdSchedule PrepareView(DataSet DS, ref IssueAgainstProdSchedule? model)
        {
            var IssueAgainstProdScheduleDetail = new List<IssueAgainstProdScheduleDetail>();
            DS.Tables[0].TableName = "IssueAgainstPlanSchMain";
            DS.Tables[1].TableName = "IssueAgainstPlanSchDetail";
            int cnt = 0;
            model.IssAgtProdSchEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["IssAgtProdSchEntryId"].ToString());
            model.IssAgtProdSchYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["IssAgtProdSchYearCode"].ToString());
            model.IssAgtProdSchEntryDate = DS.Tables[0].Rows[0]["IssAgtProdSchEntryDate"].ToString();
            model.IssAgtProdSchSlipNo = DS.Tables[0].Rows[0]["IssAgtProdSchSlipNo"].ToString();
            model.IssAgtProdSchSlipDate = DS.Tables[0].Rows[0]["IssAgtProdSchSlipDate"].ToString();
            model.IssuedByEmpName = DS.Tables[0].Rows[0]["IssuedByEmp"].ToString();
            model.IssuedByEmpId=Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedByEmpId"].ToString());
            model.ReceivedByEmpName = DS.Tables[0].Rows[0]["ReceivedByEmp"].ToString();
            model.ReceivedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ReceivedByEmpId"].ToString());
            model.Acknowledged = DS.Tables[0].Rows[0]["AcknowledgedByProd"].ToString();
            model.AckByEmpId=Convert.ToInt32(DS.Tables[0].Rows[0]["AckByEmpId"].ToString());
            model.ActualEntryByName=DS.Tables[0].Rows[0]["ActualEntryByEmp"].ToString();
            model.AckDate=DS.Tables[0].Rows[0]["AckDate"].ToString();
            model.ActualEntryDate=DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.ActualEntryById=Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            model.LastUpdatedById=Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
            model.LastUpdatedDate=DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
            model.LastUpdatedByName=DS.Tables[0].Rows[0]["UpdatedByEmp"].ToString();
            model.CC=DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID=Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.MachineName=DS.Tables[0].Rows[0]["MachineName"].ToString();
            model.StoreId=Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedFromStoreId"].ToString());
            model.StoreName=DS.Tables[0].Rows[0]["IssueFromStore"].ToString();
           
            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedByEmp"].ToString()))
            {
                model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmp"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.LastUpdatedDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
            }
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    IssueAgainstProdScheduleDetail.Add(new IssueAgainstProdScheduleDetail
                    {
                        seqno = Convert.ToInt32(row["SeqNo"].ToString()),
                        WCId = Convert.ToInt32(row["Wcid"].ToString()),
                        WorkCenter=row["WorkCenter"].ToString(),
                        ProdPlanNo = row["ProdPlanNo"].ToString(),
                        ProdPlanYearcode = Convert.ToInt32(row["PlanNoYearCode"].ToString()),
                        ProdPlanEntryId = Convert.ToInt32(row["PlanNoEntryId"].ToString()),
                        ProdPlanDate = row["PlanDate"].ToString(),
                        ProdPlanFGItemCode = Convert.ToInt32(row["PRODPlanFGItemCode"].ToString()),
                        ProdSchNo = row["ProdSchNo"].ToString(),
                        ProdSchEntryId = Convert.ToInt32(row["ProdSchEntryId"].ToString()),
                        ProdSchYearcode =Convert.ToInt32(row["ProdSchYearCode"].ToString()),
                        ProdSchDate = row["ProdSchDate"].ToString(),
                        ParentProdSchNo = row["ParentProdSchNo"].ToString(),
                        ParentProdSchEntryId = Convert.ToInt32(row["ParentProdSchEntryId"].ToString()),
                        ParentProdSchYearCode = Convert.ToInt32(row["ParentProdSchYearCode"].ToString()),
                        ParentProdSchDate = row["ParentProdSchDate"].ToString(),
                        PRODSCHFGItemCode = Convert.ToInt32(row["PRODSCHFGItemCode"].ToString()),
                        FGItemName = row["FGItemName"].ToString(),
                        FGPartCode = row["FGPartCode"].ToString(),
                        IssueItemCode = Convert.ToInt32(row["IssueItemCode"].ToString()),
                        RMItemName = row["ItemName"].ToString(),
                        RMPartCode = row["PartCode"].ToString(),
                        StoreId = Convert.ToInt32(row["StoreId"].ToString()),
                        StoreName=row["IssueFromStore"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo=row["UniqueBatchno"].ToString(),
                        ToatlStock = Convert.ToDecimal(row["TotalStock"].ToString()),
                        BatchStock = Convert.ToDecimal(row["BatchStock"].ToString()),
                        WIPStock = Convert.ToDecimal(row["WIPStock"].ToString()),
                        StdPacking = Convert.ToDecimal(row["StdPkg"].ToString()),
                        MaxIssueQty = Convert.ToDecimal(row["MaxIssueQtyAllowed"].ToString()),
                        IssueQty = Convert.ToDecimal(row["IssueQty"].ToString()),
                        Unit = row["Unit"].ToString(),
                        AltQty = Convert.ToDecimal(row["AltIssueQty"].ToString()),
                        AltUnit = row["Altunit"].ToString(),
                        PrevissueQty=Convert.ToDecimal(row["PrevissueQty"]),
                        PreIssueAltQty = Convert.ToDecimal(row["PreIssueAltQty"].ToString()),
                        BomNo = Convert.ToInt32(row["BOMNo"].ToString()),
                        BomQty=Convert.ToDecimal(row["BomQty"].ToString()),
                        WIPMaxQty=Convert.ToDecimal(row["WIPMaxQty"].ToString()),
                        InProcessQCEntryId=Convert.ToInt32(row["InProcessQCEntryId"].ToString()),
                        InprocessQCSlipNo = row["InprocessQCSlipNo"].ToString(),
                        InProcessQCYearCode=Convert.ToInt32(row["InProcessQCYearCode"].ToString()),
                        InProcessQCDate =row["InProcessQCDate"].ToString(),
                        ItemHasSubBom=row["ItemHasSubBom"].ToString(),
                        MaterialIsIssuedDirectlyFrmWC=row["MaterialIsIssuedDirectlyFrmWC"].ToString(),
                        IssuedFromWC=Convert.ToInt32(row["IssuedFromWC"].ToString()),
                        IssueFrmWCSlipNo = row["IssueFrmWCSlipNo"].ToString(),
                        IssueFrmWCYearCode=Convert.ToInt32(row["IssueFrmWCYearCode"].ToString()),
                        IssueFrmWCDate = row["IssueFrmWCDate"].ToString(),
                        IssueFrmQCorTransferForm = row["IssueFrmQCorTransferForm"].ToString(),
                        Remark = row["ItemRemark"].ToString(),
                        otherdetail1 = row["otherdetail1"].ToString(),
                        otherdetail2 = row["otherdetail2"].ToString(),
                        ItemRate=Convert.ToDecimal(row["ItemRate"].ToString()),
                        //ReceivedByEmpId=Convert.ToInt32(row["ReceivedByEmpId"].ToString()),
                        //Rate=Convert.ToDecimal(row["Rate"].ToString()),
                        //ItemWeight=Convert.ToDecimal(row["ItemWeight"].ToString()),
                    });
                }
                model.ItemDetailGrid = IssueAgainstProdScheduleDetail;
            }
            return model;
        }
        public async Task<ResponseResult> SaveIssueAgainstProductionSchedule(IssueAgainstProdSchedule model, DataTable IssueAgainstProductionScheduleGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                //DateTime IssEntryDate = new DateTime();
                //DateTime IssSlipDate = new DateTime();
                //DateTime ackDate = new DateTime();
                //DateTime entryDate = new DateTime();
                //DateTime lastDate = new DateTime();
               
                var IssEntryDate = CommonFunc.ParseFormattedDate(model.IssAgtProdSchEntryDate);
                var IssSlipDate = CommonFunc.ParseFormattedDate(model.IssAgtProdSchSlipDate);
                var ackDate = CommonFunc.ParseFormattedDate(model.AckDate);
                var entryDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                var lastDate = CommonFunc.ParseFormattedDate(model.LastUpdatedDate);

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.UpdatedBy == 0 ? 0.0 : model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdatedDate", lastDate));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@Entryid", model.IssAgtProdSchEntryId == 0 ? 0 : model.IssAgtProdSchEntryId));
                SqlParams.Add(new SqlParameter("@Yearcode", model.IssAgtProdSchYearCode == 0 ? 0 : model.IssAgtProdSchYearCode));
                SqlParams.Add(new SqlParameter("@IssAgainstEntrydate", IssEntryDate));
                SqlParams.Add(new SqlParameter("@IssAgainstIssueSlipNo", model.IssAgtProdSchSlipNo ?? ""));
                SqlParams.Add(new SqlParameter("@IssAgtProdSchEntryDate", IssSlipDate));
                SqlParams.Add(new SqlParameter("@IssuedBYEmpId", model.IssuedByEmpId == 0 ? 0 : model.IssuedByEmpId));
                SqlParams.Add(new SqlParameter("@IssuedFromStoreId", model.StoreId == 0 ? 0 : model.StoreId));
                SqlParams.Add(new SqlParameter("@ReceivedBy", model.ReceivedByEmpId == 0 ? 0.0 : model.ReceivedByEmpId));
                SqlParams.Add(new SqlParameter("@Acknowledged", model.Acknowledged));
                SqlParams.Add(new SqlParameter("@AckByEmpID", model.AckByEmpId == 0 ? 0.0 : model.AckByEmpId));
                SqlParams.Add(new SqlParameter("@AckDate", ackDate));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entryDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEntryById == 0 ? 0.0 : model.ActualEntryById));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.MachineName ?? ""));
                SqlParams.Add(new SqlParameter("@UID", model.UID == 0 ? 0.0 : model.UID));

                SqlParams.Add(new SqlParameter("@dt", IssueAgainstProductionScheduleGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "IsStockable"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty,int BomNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "IssueAgtProdSch"));
                SqlParams.Add(new SqlParameter("@MainBOMItem", ItemCode));
                SqlParams.Add(new SqlParameter("@PFGQTY", WOQty));
                SqlParams.Add(new SqlParameter("@PBOMREVNO", BomNo));
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
        public async Task<ResponseResult> GetSummaryDetail(string FromDate, string ToDate, string IssAgtProdSchSlipNo, string IssueFromStore, string PartCode, string ItemName, string ProdPlanNo, string ProdSchNo, string DashboardType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@ToDate", toDt));
                SqlParams.Add(new SqlParameter("@IssAgtProdSchSlipNo", IssAgtProdSchSlipNo));
                SqlParams.Add(new SqlParameter("@IssueFromStore", IssueFromStore));
                SqlParams.Add(new SqlParameter("@Partcode", PartCode));
                SqlParams.Add(new SqlParameter("@ItemName", ItemName));
                SqlParams.Add(new SqlParameter("@ProdPlanNo", ProdPlanNo));
                SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
                SqlParams.Add(new SqlParameter("@SummDetail", DashboardType));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueAgainstProdSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<IssueAgainstProdScheduleDashboard> GetSearchData(string FromDate, string ToDate, string IssAgtProdSchSlipNo, string IssueFromStore, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueAgainstProdScheduleDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueAgainstProdSchedule", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@IssAgtProdSchSlipNo", IssAgtProdSchSlipNo);
                    oCmd.Parameters.AddWithValue("@IssueFromStore", IssueFromStore);
                    oCmd.Parameters.AddWithValue("@Partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                    oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (DashboardType == "SUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.IssueAgainstProdScheduleDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                   select new IssueAgainstProductionScheduleDashboard
                                                                   {
                                                                       IssAgtProdSchEntryId=Convert.ToInt32(dr["IssAgtProdSchEntryId"]),
                                                                       IssAgtProdSchYearCode = Convert.ToInt32(dr["IssAgtProdSchYearCode"]),
                                                                       IssAgtProdSchEntryDate = dr["IssAgtProdSchEntryDate"].ToString().Split(" ")[0],
                                                                       IssAgtProdSchSlipNo = dr["IssAgtProdSchSlipNo"].ToString(),
                                                                       IssAgtProdSchSlipDate = dr["IssAgtProdSchSlipDate"].ToString().Split(" ")[0],
                                                                       IssuedByEmpId = Convert.ToInt32(dr["IssuedByEmpId"]),
                                                                       IssuedByEmp =dr["IssuedByEmp"].ToString(),
                                                                       ReceivedByEmpId = Convert.ToInt32(dr["ReceivedByEmpId"]),
                                                                       ReceivedByEmp =dr["ReceivedByEmp"].ToString(),
                                                                       AcknowledgedByProd =dr["AcknowledgedByProd"].ToString(),
                                                                       AckDate = dr["AckDate"].ToString().Split(" ")[0],
                                                                       ActualEntryDate = dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                                       ActualEntryBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                                       ActualEntryByEmp = dr["ActualEntryByEmp"].ToString(),
                                                                       UpdatedByEmp =dr["UpdatedByEmp"].ToString(),
                                                                       LastUpdatedBy = Convert.ToInt32(dr["LastUpdatedBy"]),
                                                                       LastUpdatedDate=dr["LastUpdatedDate"].ToString().Split(" ")[0],
                                                                       CC=dr["CC"].ToString(),
                                                                       MachineName =dr["MachineName"].ToString(),
                                                                       UID = Convert.ToInt32(dr["UID"]),
                                                                       IssueFromStore =dr["IssueFromStore"].ToString(),
                                                                   }).ToList();
                    }
                }
                if (DashboardType == "DETAIL")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.IssueAgainstProdScheduleDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                   select new IssueAgainstProductionScheduleDashboard
                                                                   {
                                                                       IssAgtProdSchEntryId=Convert.ToInt32(dr["IssAgtProdSchEntryId"]),
                                                                       IssAgtProdSchYearCode = Convert.ToInt32(dr["IssAgtProdSchYearCode"]),
                                                                       WorkCenter = dr["WorkCenter"].ToString(),
                                                                       ProdPlanNo = dr["ProdPlanNo"].ToString(),
                                                                       IssAgtProdSchEntryDate = dr["IssAgtProdSchEntryDate"].ToString().Split(" ")[0],
                                                                       IssAgtProdSchSlipNo = dr["IssAgtProdSchSlipNo"].ToString(),
                                                                       PlanNoYearCode = Convert.ToInt32(dr["PlanNoYearCode"]),
                                                                       PlanNoEntryId = Convert.ToInt32(dr["PlanNoEntryId"]),
                                                                       PlanDate = dr["PlanDate"].ToString().Split(" ")[0],
                                                                       PRODPlanFGItemCode = Convert.ToInt32(dr["PRODPlanFGItemCode"]),
                                                                       PRODSCHFGItemCode = Convert.ToInt32(dr["PRODSCHFGItemCode"]),
                                                                       FGItemName = dr["FGItemName"].ToString().Split(" ")[0],
                                                                       FGPartCode = dr["FGPartCode"].ToString().Split(" ")[0],
                                                                       ProdSchNo =dr["ProdSchNo"].ToString(),
                                                                       ProdSchEntryId = Convert.ToInt32(dr["ProdSchEntryId"]),
                                                                       ProdSchYearCode = Convert.ToInt32(dr["ProdSchYearCode"]),
                                                                       ProdSchDate = dr["ProdSchDate"].ToString().Split(" ")[0],
                                                                       ParentProdSchNo =dr["ParentProdSchNo"].ToString(),
                                                                       ParentProdSchEntryId = Convert.ToInt32(dr["ParentProdSchEntryId"]),
                                                                       ParentProdSchYearCode = Convert.ToInt32(dr["ParentProdSchYearCode"]),
                                                                       ParentProdSchDate = dr["ParentProdSchDate"].ToString().Split(" ")[0],
                                                                       PartCode =dr["PartCode"].ToString(),
                                                                       ItemName =dr["ItemName"].ToString(),
                                                                       IssueItemCode = Convert.ToInt32(dr["IssueItemCode"]),                                                                       BatchNo =dr["BatchNo"].ToString(),
                                                                       UniqueBatchno =dr["UniqueBatchno"].ToString(),
                                                                       TotalStock = Convert.ToDecimal(dr["TotalStock"]),
                                                                       BatchStock = Convert.ToDecimal(dr["BatchStock"]),
                                                                       WIPStock = Convert.ToDecimal(dr["WIPStock"]),
                                                                       StdPkg = Convert.ToDecimal(dr["StdPkg"]),
                                                                       MaxIssueQtyAllowed = Convert.ToDecimal(dr["MaxIssueQtyAllowed"]),
                                                                       IssueQty = Convert.ToDecimal(dr["IssueQty"]),
                                                                       Unit =dr["Unit"].ToString(),
                                                                       AltIssueQty = Convert.ToDecimal(dr["AltIssueQty"]),
                                                                       Altunit =dr["Altunit"].ToString(),
                                                                       PrevissueQty = Convert.ToDecimal(dr["PrevissueQty"]),
                                                                       PreIssueAltQty = Convert.ToDecimal(dr["PreIssueAltQty"]),
                                                                       BOMNo = Convert.ToInt32(dr["BOMNo"]),
                                                                       BomQty = Convert.ToDecimal(dr["BomQty"]),
                                                                       WIPMaxQty = Convert.ToDecimal(dr["WIPMaxQty"]),
                                                                       InProcessQCEntryId = Convert.ToInt32(dr["InProcessQCEntryId"]),
                                                                       InprocessQCSlipNo=dr["InprocessQCSlipNo"].ToString(),
                                                                       InProcessQCYearCode = Convert.ToInt32(dr["InProcessQCYearCode"]),
                                                                       InProcessQCDate=dr["InProcessQCDate"].ToString().Split(" ")[0],
                                                                       ItemHasSubBom=dr["ItemHasSubBom"].ToString(),
                                                                       MaterialIsIssuedDirectlyFrmWC=dr["MaterialIsIssuedDirectlyFrmWC"].ToString(),
                                                                       IssAgtProdSchSlipDate=dr["IssAgtProdSchSlipDate"].ToString().Split(" ")[0],
                                                                       IssuedByEmpId = Convert.ToInt32(dr["IssuedByEmpId"]),
                                                                       IssuedByEmp =dr["IssuedByEmp"].ToString(),
                                                                       ReceivedByEmpId = Convert.ToInt32(dr["ReceivedByEmpId"]),
                                                                       ReceivedByEmp =dr["ReceivedByEmp"].ToString(),
                                                                       AcknowledgedByProd =dr["AcknowledgedByProd"].ToString(),
                                                                       AckDate = dr["AckDate"].ToString().Split(" ")[0],
                                                                       ActualEntryDate = dr["ActualEntryDate"].ToString().Split(" ")[0],
                                                                       ActualEntryBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                                       ActualEntryByEmp = dr["ActualEntryByEmp"].ToString(),
                                                                       LastUpdatedBy = Convert.ToInt32(dr["LastUpdatedBy"]),
                                                                       UpdatedByEmp =dr["UpdatedByEmp"].ToString(),
                                                                       LastUpdatedDate=dr["LastUpdatedDate"].ToString().Split(" ")[0],
                                                                       CC=dr["CC"].ToString(),
                                                                       MachineName =dr["MachineName"].ToString(),
                                                                       UID = Convert.ToInt32(dr["UID"]),
                                                                       IssueFromStore =dr["IssueFromStore"].ToString()
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
        public async Task<ResponseResult> FillBatchNo(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime issueDate = DateTime.ParseExact(IssuedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime issueDate = DateTime.Today;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@transDate", ParseFormattedDate(IssuedDate)));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@FinStartDate", ParseFormattedDate(FinStartDate)));

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
        public async Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                //DateTime issueDate = DateTime.ParseExact(TillDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime issueDate = DateTime.Today;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@storeid", StoreId));
                SqlParams.Add(new SqlParameter("@IssueDate", issueDate.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@uniqbatchno", UniqBatchNo));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("ItemTotStkAndBatchStock", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string ProdSchSlipNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DeletedById"));
                SqlParams.Add(new SqlParameter("@EntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@IssAgtProdSchSlipNo", ProdSchSlipNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueAgainstProdSchedule", SqlParams);
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
