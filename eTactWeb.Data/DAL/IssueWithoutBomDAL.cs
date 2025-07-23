using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class IssueWithoutBomDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }

        public IssueWithoutBomDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Material to Issue WO BOM"));
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
        public async Task<ResponseResult> GetDashboardData(string Fromdate, string Todate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var frmDt = CommonFunc.ParseFormattedDate(Fromdate);
                var toDt = CommonFunc.ParseFormattedDate(Todate);
                var SqlParams = new List<dynamic>();
                if (Flag == "True")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", frmDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", frmDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillBranch()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Branch"));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GETDepartMent(string ReqNo, int ReqYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GETDepartMent"));
                SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
                SqlParams.Add(new SqlParameter("@ReqYearCode", ReqYearCode));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProjectNo()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetProjectDetails"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "BINDSTORE"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<DataSet> FillEmployee(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {

                    _ResponseResult.Result.Tables[0].TableName = "EmployeeList";


                    oDataSet = _ResponseResult.Result;
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

        public async Task<ResponseResult> FillDept()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "IssuedByDepartment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<IssueWithoutBom> GetViewByID(int ID, int YearCode)
        {
            var model = new IssueWithoutBom();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);

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

        private static IssueWithoutBom PrepareView(DataSet DS, ref IssueWithoutBom? model)
        {
            try
            {
                var ItemGrid = new List<IssueWithoutBomDetail>();
                DS.Tables[0].TableName = "IssWOBomMain";
                DS.Tables[1].TableName = "IssWOBomDetail";
                int cnt = 1;
                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["IssWOBOMEntryId"]);
                model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["IssWOBOMYearCode"]);
                model.EntryDate = DS.Tables[0].Rows[0]["IssWOBOMEntryDate"].ToString();
                model.PreFix = DS.Tables[0].Rows[0]["PreFix"].ToString();
                model.IssueSlipNo = DS.Tables[0].Rows[0]["IssWOBOMSlipNo"].ToString();
                model.IssueDate = DS.Tables[0].Rows[0]["IssWOBOMIssueDate"].ToString();
                model.IssuedByEmpCode = Convert.ToInt32((DS.Tables[0].Rows[0]["IssuedByEmpCode"]));
                model.IssuedByEmpName = DS.Tables[0].Rows[0]["IssuedByEmpCodeName"].ToString();
                model.IssuedByDepCode = Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedByDepCode"]);
                model.RecDepCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RecDepCode"]);
                model.RecDept = DS.Tables[0].Rows[0]["RecDepCodeName"].ToString();
                model.RecByEmpCode = Convert.ToInt32((DS.Tables[0].Rows[0]["RecByEmpCode"]));
                model.MachineCode = DS.Tables[0].Rows[0]["Machinecode"].ToString();
                model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
                model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]);
               
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.ReqNo = DS.Tables[0].Rows[0]["ReqNo"].ToString();
                model.ReqyearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ReqYearCode"]);
                model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
                model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
                model.ActualEntrydate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);

                if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
                {
                    model.LastupdatedByName = DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString();
                    model.LastupdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                    model.LastUpdationDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);
                }

                if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ItemGrid.Add(new IssueWithoutBomDetail
                        {
                            seqno = cnt++,
                            //ReqNo = row["ReqNo"].ToString(),
                            //ReqDate = row["ReqDate"].ToString(),
                            //ReqyearCode = Convert.ToInt32(row["ReqYearCode"]),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            ItemName = row["Item_Name"].ToString(),
                            PartCode = row["PartCode"].ToString(),
                            ReqQty = Convert.ToDecimal(row["ReqQty"]),
                            StoreId = Convert.ToInt32(row["StoreId"]),
                            BatchNo = row["BatchNo"].ToString(),
                            uniqueBatchNo = row["uniqueBatchNo"].ToString(),
                            IssueQty = Convert.ToDecimal(row["IssueQty"]),
                            Unit = row["Unit"].ToString(),
                            LotStock = Convert.ToDecimal(row["LotStock"]),
                            TotalStock = Convert.ToDecimal(row["TotalStock"]),
                            AltQty = Convert.ToDecimal(row["AltQty"]),
                            AltUnit = row["AltUnit"].ToString(),
                            Rate = Convert.ToDecimal(row["Rate"]),
                            Remark = row["Remark"].ToString(),
                            WorkCenter = row["WorkCenterDescription"].ToString(),
                            AltItemCode = Convert.ToInt32(row["AltItemCode"]),
                            WCId = Convert.ToInt32(row["WCId"]),
                            CostCenterId = Convert.ToInt32(row["CostCenterId"]),
                            ItemSize = row["ItemSize"].ToString(),
                            ItemColor = row["ItemColor"].ToString(),
                            ProjectNo = row["ProjectNo"].ToString(),
                            ProjectYearCode = Convert.ToInt32(row["ProjectYearcode"]),
                            StoreName = row["Store_Name"].ToString(),
                            OriginalItemCode = Convert.ToInt32(row["OriginalitemCode"]),
                            StdPacking = Convert.ToSingle(row["StdPacking"]),
                            ReqNo1 = row["ReqNo"].ToString(),
                            ReqDate1 = row["ReqDate"].ToString(),
                            ReqyearCode1 = row["ReqYearCode"].ToString(),


                        });
                    }
                    model.ItemDetailGrid = ItemGrid;
                }

            }
            catch (Exception ex)
            {
                var Message = ex.Message;
            }
            return model;

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
        public async Task<ResponseResult> SaveIssueWithoutBom(IssueWithoutBom model, DataTable MRGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime entDt = new DateTime();
                //DateTime ReqDate = new DateTime();
                //DateTime issDate = new DateTime();

                var entDt =ParseFormattedDate(model.EntryDate);
                var ReqDate = ParseFormattedDate(model.ReqDate);
                var issDate = ParseFormattedDate(model.IssueDate);
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastupdatedBy));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@ReqNo", model.ReqNo));
                SqlParams.Add(new SqlParameter("@ReqYearCode", model.ReqyearCode));
                SqlParams.Add(new SqlParameter("@ReqDate", ReqDate == default ? string.Empty : ReqDate));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@PreFix", model.PreFix));
                SqlParams.Add(new SqlParameter("@IssueSlipNo", model.IssueSlipNo));
                SqlParams.Add(new SqlParameter("@IssueDate", issDate == default ? string.Empty : issDate));
                SqlParams.Add(new SqlParameter("@IssuedByEmpCode", model.IssuedByEmpCode));
                SqlParams.Add(new SqlParameter("@IssuedByDepCode", model.IssuedByDepCode));
                SqlParams.Add(new SqlParameter("@RecDepCode", model.RecDepCode));
                SqlParams.Add(new SqlParameter("@RecByEmpCode", model.RecByEmpCode));
                SqlParams.Add(new SqlParameter("@Machinecode", model.MachineCode == default ? string.Empty : model.MachineCode));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark == default ? string.Empty : model.Remark));
                SqlParams.Add(new SqlParameter("@Uid", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));


                SqlParams.Add(new SqlParameter("@DTItemGrid", MRGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<IssueWOBomMainDashboard> GetSearchData(string REQNo, string ReqDate, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueWOBomMainDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithoutBomM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@REQNo", REQNo);
                    oCmd.Parameters.AddWithValue("@ReqDate", ReqDate);
                    oCmd.Parameters.AddWithValue("@wcname", WorkCenter);
                    oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@IssueDate", IssueDate);
                    oCmd.Parameters.AddWithValue("@ReqYearCode", ReqYearCode);
                    oCmd.Parameters.AddWithValue("@FromDate", ParseFormattedDate(FromDate));
                    oCmd.Parameters.AddWithValue("@ToDate", ParseFormattedDate(ToDate));

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.IssueWOBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                 select new IssueWOBomMainDashboard
                                                 {
                                                     //,,,,,
                                                     EntryId = Convert.ToInt32(dr["EntryId"]),
                                                     YearCode = Convert.ToInt32(dr["YearCode"]),
                                                     ReqNo = dr["ReqNo"].ToString(),
                                                     ReqDate = dr["ReqDate"].ToString(),
                                                     ReqYearCode = Convert.ToInt32(dr["ReqYearCode"]),
                                                     IssueSlipNo = dr["IssueSlipNo"].ToString(),
                                                     IssueDate = dr["IssueDate"].ToString(),
                                                     WorkCenterDescription = dr["WorkCenterDescription"].ToString(),
                                                     // ActualEnteredBy = Convert.ToInt32(dr["ActualEnteredBy"]),
                                                     //MachineCode = dr["Machinecode"].ToString(),
                                                 }).ToList();
                }
                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

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


        public async Task<IssueWOBomMainDashboard> GetDetailData(string REQNo, string ReqDate, string PartCode, string Item_Name, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueWOBomMainDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithoutBomM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "SearchDETAIL");
                    oCmd.Parameters.AddWithValue("@REQNo", REQNo);
                    oCmd.Parameters.AddWithValue("@ReqDate", ReqDate);
                    oCmd.Parameters.AddWithValue("@wcname", WorkCenter);
                    oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                    oCmd.Parameters.AddWithValue("@ReqYearCode", ReqYearCode);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@Item_name", Item_Name);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@IssueDate", IssueDate);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);



                    //oCmd.Parameters.AddWithValue("@ItemCategory", model.ItemCategory);
                    // oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.IssueWOBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                 select new IssueWOBomMainDashboard
                                                 {
                                                     //,,,,,
                                                     //EntryId = Convert.ToInt32(dr["EntryId"]),
                                                     YearCode = Convert.ToInt32(dr["YearCode"]),
                                                     ReqNo = dr["ReqNo"].ToString(),
                                                     ReqDate = dr["ReqDate"].ToString(),
                                                     ReqYearCode = Convert.ToInt32(dr["ReqYearCode"]),
                                                     Item_Name = dr["Item_Name"].ToString(),
                                                     PartCode = dr["PartCode"].ToString(),
                                                     IssueSlipNo = dr["IssueSlipNo"].ToString(),
                                                     IssueDate = dr["IssueDate"].ToString(),
                                                     WorkCenterDescription = dr["WorkCenterDescription"].ToString(),
                                                 }).ToList();
                }
                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

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


        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEnteredBy, string EntryByMachine)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@Machinecode", EntryByMachine));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDataForDelete(int ID, int YearCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetDataForDelete"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckLastTransDate(long ItemCode, string BatchNo, string UniqBatchNo)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@TransDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@SToreIdWCID", 2));
                SqlParams.Add(new SqlParameter("@YearCode", DateTime.Today.Year));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@uniquebatchno", UniqBatchNo));

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
        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@ALtQty", AltQty));
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
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "IsStockable"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllowBatch()
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAllowBatch"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetIssueScanFeature()
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetIssueScanFeature"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                //DateTime issueDate = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", ParseFormattedDate(TillDate)));
                SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqBatchNo));

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
        public async Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, string ReqDate, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            DateTime reqDt = new DateTime();
            reqDt = ParseDate(ReqDate);

            try
            {
                var SqlParams = new List<dynamic>();


                SqlParams.Add(new SqlParameter("@Flag", "CheckReqBeforeSave"));
                SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
                SqlParams.Add(new SqlParameter("@ReqDate", reqDt == default ? string.Empty : reqDt));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                DateTime issueDate = DateTime.Today;

                SqlParams.Add(new SqlParameter("@UniqueBatchNo", UniqBatchNo));
                SqlParams.Add(new SqlParameter("@Year_Code", YearCode));
                SqlParams.Add(new SqlParameter("@TransDate", issueDate.ToString("yyyy/MM/dd")));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("GetItemDetailFromUniqueBatch", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {

                var SqlParams = new List<dynamic>();
                //DateTime issuedDate = DateTime.Today;
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

                DateTime issueDate = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

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

        public async Task<ResponseResult> GetReqQtyForScan(string ReqNo, int ReqYearCode, string ReqDate, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetReqForScan"));
                SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
                SqlParams.Add(new SqlParameter("@ReqYearCode", ReqYearCode));
                SqlParams.Add(new SqlParameter("@ReqDate", ReqDate));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithoutBomM", SqlParams);
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
