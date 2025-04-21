using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class IssueThrBOMDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public IssueThrBOMDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Material to Issue Thr BOM"));
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

        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
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
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);
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
        public async Task<ResponseResult> FillProjectNo()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetProjectDetails"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);
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

        public async Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                DateTime issueDate = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                SqlParams.Add(new SqlParameter("@TILL_DATE", issueDate.ToString("yyyy/MM/dd")));
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

        public async Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, int ReqyearCode, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            //DateTime reqDt = new DateTime();
            //reqDt= ParseDate(ReqyearCode);

            try
            {
                var SqlParams = new List<dynamic>();


                SqlParams.Add(new SqlParameter("@Flag", "CheckReqBeforeSave"));
                SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
                SqlParams.Add(new SqlParameter("@ReqYearCode", ReqyearCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
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
                //DateTime issueDate = DateTime.ParseExact(IssuedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime issueDate = DateTime.Today;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@transDate", CommonFunc.ParseFormattedDate( issueDate.ToString())));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@FinStartDate", CommonFunc.ParseFormattedDate(FinStartDate)));

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
        public async Task<ResponseResult> GetAllowBatch()
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAllowBatch"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFGDataList(string Reqno, int ReqYC)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FGDataList"));
                SqlParams.Add(new SqlParameter("@reqno", Reqno));
                SqlParams.Add(new SqlParameter("@reqYearcode", ReqYC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("GetDataForRequitionThroughBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                DateTime issueDate = DateTime.ParseExact(TransDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@Flag", "STORE"));
                SqlParams.Add(new SqlParameter("@UniqueBatchNo", UniqBatchNo));
                SqlParams.Add(new SqlParameter("@Year_code", YearCode));
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
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "IsStockable"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
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
        public async Task<ResponseResult> SaveIssueThrBom(IssueThrBom model, DataTable RMGrid, DataTable FGGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Clear();
                //DateTime entDt = new DateTime();
                //DateTime ReqDate = new DateTime();
                //DateTime issDate = new DateTime();
                //DateTime woDate = new DateTime();

               var woDate = "";
               var entDt = CommonFunc.ParseFormattedDate(model.EntryDate);
               var ReqDate = CommonFunc.ParseFormattedDate(model.ReqDate);
               var issDate = CommonFunc.ParseFormattedDate(model.IssueDate);
               var upDt = CommonFunc.ParseFormattedDate(model.LastUpdationDate.ToString("dd/MM/yyyy"));
                var jobCardDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                if (model.WoDate != null) 
                {
                   woDate = CommonFunc.ParseFormattedDate(model.WoDate);
                }
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastupdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@IssueSlipNo", model.IssueSlipNo));
                SqlParams.Add(new SqlParameter("@IssueDate", issDate == default ? string.Empty : issDate));
                SqlParams.Add(new SqlParameter("@WONO", model.WONO));
                SqlParams.Add(new SqlParameter("@WOYearCode", model.WOYearCode));
                if( model.WoDate != null)
                {
                    SqlParams.Add(new SqlParameter("@WOdate", woDate == default ? string.Empty : woDate));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@WOdate", model.WoDate == default ? string.Empty : model.WoDate));
                }
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@WCID", model.WCID));
                SqlParams.Add(new SqlParameter("@ReqNo", model.ReqNo));
                SqlParams.Add(new SqlParameter("@ReqYearCode", model.ReqyearCode));
                SqlParams.Add(new SqlParameter("@ReqDate", ReqDate == default ? string.Empty : ReqDate));
                SqlParams.Add(new SqlParameter("@jobCardNo", model.JobCardNo));
                SqlParams.Add(new SqlParameter("@JObCardYearcode", model.JobYearCode));
                SqlParams.Add(new SqlParameter("@JobCardDate", jobCardDt));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@UID", model.Uid));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@Machinecode", model.MachineCode));
                SqlParams.Add(new SqlParameter("@IssuedByEmpCode", model.IssuedByEmpCode));
                SqlParams.Add(new SqlParameter("@RecByEmpCode", model.RecByEmpCode));


                SqlParams.Add(new SqlParameter("@DTItemGrid", RMGrid));
                SqlParams.Add(new SqlParameter("@DTMainGrid", FGGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<IList<TextValue>> GetEmployeeList()
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "BINDRecByEmployee");

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["EmpNameCode"].ToString(),
                                Value = Reader["Emp_Id"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        internal async Task<IssueThrBom> GetViewByID(int ID, int YearCode)
        {
            var model = new IssueThrBom();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);

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
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "IssueThrBom"));
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
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueWithBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        } 
        
        public async Task<IssueThrBomMainDashboard> FGDetailData(string FromDate, string Todate, string Flag = "", string DashboardType = "FGSUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "")
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueThrBomMainDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);                   
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARDGRID");
                    oCmd.Parameters.AddWithValue("@DashBoardSearchType", DashboardType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@REQNo", ReqNo);                                                                                                                   
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.IssueThrBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new IssueThrBomMainDashboard
                                              {
                                                  IssueSlipno = dr["IssueSlipNo"].ToString(),
                                                  IssueDate = dr["IssueDate"].ToString(),                                                                                                     
                                                  ReqNo = dr["ReqNo"].ToString(),                                                
                                                  ReqDate = dr["ReqDate"].ToString(),                                                
                                                  ReqYearCode = Convert.ToInt32(dr["ReqYearCode"].ToString()),
                                                  WONO = dr["WONO"].ToString(),                                                
                                                  WODate = dr["WODate"].ToString(),
                                                  FGPartCode = dr["FGPARTCODE"].ToString(),  
                                                  FGItemName = dr["FGITEMNAME"].ToString(),                                                                                                    
                                                  IssueQty = Convert.ToSingle(dr["FGQty"].ToString()),
                                                  EntryId = Convert.ToInt32(dr["EntryId"].ToString()),                                                
                                                  YearCode = Convert.ToInt32(dr["YearCode"].ToString()),
                                                  jobCardNo = dr["jobCardNo"].ToString(),
                                                  JobcardDate = dr["JobcardDate"].ToString(),
                                                  RMRemark = dr["Remark"].ToString(),                                                
                                                  ActENterdByEmpName = dr["ActENterdByEmpName"].ToString(),
                                                  ActENterdByEmpCode = dr["ActENterdByEmpCode"].ToString(),                                                
                                                  ActualEntryDate = dr["ActualEntryDate"].ToString(),                                                
                                                  UpdatedEmpName = dr["UpdatedEmpName"].ToString(),                                                
                                                  UpdatedByEmpcode = dr["UpdatedByEmpcode"].ToString(),                                                                                                                                            
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
        
        public async Task<IssueThrBomMainDashboard> RMDetailData(string FromDate, string Todate, string WCName, string PartCode, string ItemName, string Flag = "", string DashboardType = "RMDETAIL", string IssueSlipNo = "", string ReqNo = "", string GlobalSearch = "", string FGPartCode = "", string FGItemName = "")
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueThrBomMainDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);                   
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARDGRID");
                    oCmd.Parameters.AddWithValue("@DashBoardSearchType", DashboardType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@REQNo", ReqNo);                                                  
                    oCmd.Parameters.AddWithValue("@wcname", WCName);                                                  
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);                                                                                                                   
                    oCmd.Parameters.AddWithValue("@Item_name", ItemName);                                                                                                                   
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.IssueThrBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new IssueThrBomMainDashboard
                                              {
                                                  IssueSlipno = dr["IssueSlipNo"].ToString(),
                                                  IssueDate = dr["IssueDate"].ToString(),                                                                                                     
                                                  ReqNo = dr["ReqNo"].ToString(),                                                
                                                  ReqDate = dr["ReqDate"].ToString(),     
                                                  RMItemName = dr["RMItem"].ToString(),     
                                                  RMPartCode = dr["RMPartCode"].ToString(),     
                                                  ReqQty = Convert.ToSingle(dr["ReqQty"].ToString()),     
                                                  IssueQty = Convert.ToSingle(dr["IssueQty"].ToString()),     
                                                  rmUnit   = dr["rmUnit"].ToString(),
                                                  PendQty = Convert.ToSingle(dr["PendQty"].ToString()),
                                                  AltReqQty = Convert.ToSingle(dr["AltReqQty"].ToString()),
                                                  AltIssueQty = Convert.ToSingle(dr["AltIssueQty"].ToString()),
                                                  AltUnit = dr["AltUnit"].ToString(),
                                                  BatchNo = dr["BatchNo"].ToString(),
                                                  uniquebatchNo = dr["uniquebatchNo"].ToString(),
                                                  lotStock = Convert.ToSingle(dr["lotStock"].ToString()),
                                                  TotalStock = Convert.ToSingle(dr["TotalStock"].ToString()),
                                                  IssuedAlternateItem = dr["IssuedAlternateItem"].ToString(),
                                                  WorkCenter = dr["WorkCenter"].ToString(),
                                                  jobCardNo = dr["jobCardNo"].ToString(),
                                                  JobcardDate = dr["JobcardDate"].ToString(),
                                                  OrginalItemName = dr["OrginalItemName"].ToString(),
                                                  OriginalPartCode = dr["OriginalPartCode"].ToString(),
                                                  FGPartCode = dr["FGPARTCODE"].ToString(),
                                                  FGItemName = dr["FGITEMNAME"].ToString(),
                                                  RMRemark = dr["RMRemark"].ToString(),
                                                  itemsize = dr["itemsize"].ToString(),
                                                  itemcolor = dr["itemcolor"].ToString(),  
                                                  ReqYearCode = Convert.ToInt32(dr["ReqYearCode"].ToString()),
                                                  EntryId = Convert.ToInt32(dr["EntryId"].ToString()),
                                                  YearCode = Convert.ToInt32(dr["YearCode"].ToString()),                                                                                                                                        
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
        public async Task<IssueThrBomMainDashboard> SummaryData(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "")

        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueThrBomMainDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);                   
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARDGRID");
                    oCmd.Parameters.AddWithValue("@DashBoardSearchType", DashboardType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@REQNo", ReqNo);                                                  
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.IssueThrBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new IssueThrBomMainDashboard
                                              {
                                                  IssueSlipno = dr["IssueSlipNo"].ToString(),
                                                  IssueDate = dr["IssueDate"].ToString(),                                                                                                     
                                                  ReqNo = dr["ReqNo"].ToString(),                                                
                                                  ReqDate = dr["ReqDate"].ToString(),                                                
                                                  ReqYearCode = Convert.ToInt32(dr["ReqYearCode"].ToString()),
                                                  WONO = dr["WONO"].ToString(),                                                
                                                  WODate = dr["WODate"].ToString(),
                                                  EntryId = Convert.ToInt32(dr["EntryId"].ToString()),                                                                                            
                                                  YearCode = Convert.ToInt32(dr["YearCode"].ToString()),                                                  
                                                  jobCardNo = dr["jobCardNo"].ToString(),
                                                  JobcardDate = dr["JobcardDate"].ToString(),
                                                  RMRemark = dr["Remark"].ToString(),                                                
                                                  ActENterdByEmpName = dr["ActENterdByEmpName"].ToString(),
                                                  ActENterdByEmpCode = dr["ActENterdByEmpCode"].ToString(),                                                
                                                  ActualEntryDate = dr["ActualEntryDate"].ToString(),                                                
                                                  UpdatedEmpName = dr["UpdatedEmpName"].ToString(),                                                
                                                  UpdatedByEmpcode = dr["UpdatedByEmpcode"].ToString(),                                                                                                                                            
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


        private static IssueThrBom PrepareView(DataSet DS, ref IssueThrBom? model)
        {
            var ItemGrid = new List<IssueThrBomDetail>();
            var ItemGrid1 = new List<IssueThrBomFGData>();
            DS.Tables[0].TableName = "IssueThrBOM";
            DS.Tables[1].TableName = "IssueThrBomDetail";
            DS.Tables[2].TableName = "IssThrBomFGDetail";
            int cnt = 1;
            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["IssWithBOMEntryId"]);
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["IssWithBOMYearCode"]);
            model.EntryDate = DS.Tables[0].Rows[0]["IssWithBOMEntryDate"].ToString();            
            model.IssueSlipNo = DS.Tables[0].Rows[0]["IssWithBOMIssueSlipNo"].ToString();
            model.IssueDate = DS.Tables[0].Rows[0]["IssWithBOMIssueDate"].ToString();
            model.WONO = DS.Tables[0].Rows[0]["WONO"].ToString();
            model.WOYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["WOYearCode"].ToString());
            model.WODate = DS.Tables[0].Rows[0]["WOdate"].ToString();
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            model.ReqNo = DS.Tables[0].Rows[0]["ReqNo"].ToString();
            model.ReqyearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ReqYearCode"]);
            model.ReqDate = DS.Tables[0].Rows[0]["ReqDate"].ToString();
            model.JobCardNo = DS.Tables[0].Rows[0]["jobCardNo"].ToString();
            model.JobYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["JObCardYearcode"].ToString());
            model.JobCardDate = DS.Tables[0].Rows[0]["JobCardDate"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            model.ActualEntrydate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntrydate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntrydate"]);
            model.LastupdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
            model.LastupdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString();
            model.LastUpdationDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);
            model.MachineCode = DS.Tables[0].Rows[0]["Machinecode"].ToString();
            model.IssuedByEmpCode =Convert.ToInt32(DS.Tables[0].Rows[0]["IssuedByEmpCode"]);
            model.IssuedByEmpName = DS.Tables[0].Rows[0]["IssuedByEmpName"].ToString();
            model.RecByEmpCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RecByEmpCode"]);
            model.RecByEmpCodeName = DS.Tables[0].Rows[0]["RecByEmpCodeName"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());        
            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.LastupdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmpName"].ToString();
                model.LastupdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.LastUpdationDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemGrid.Add(new IssueThrBomDetail
                    {
                        seqno = cnt++,
                        ReqNo1 = row["ReqNo"].ToString(),
                        ReqDate1 = row["ReqDate"].ToString(),
                        ReqyearCode1 = Convert.ToInt32(row["ReqYearCode"].ToString()),
                        EntryId = Convert.ToInt32(row["EntryID"].ToString()),
                        YearCode = Convert.ToInt32(row["YearCode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemCode"].ToString()),
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        IssuedDate = row["IssueDate"].ToString(),
                        ItemName = row["RMITEMNAME"].ToString(),                        
                        PartCode = row["RMPARTCODE"].ToString(),
                        ReqQty = Convert.ToDecimal(row["ReqQty"]),
                        AltReqQty = Convert.ToDecimal(row["AltReqQty"]),
                        StoreId = Convert.ToInt32(row["StoreId"]),
                        WCId = Convert.ToInt32(row["wcid"]),
                        StoreName = row["StoreName"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        uniqueBatchNo = row["uniqueBatchNo"].ToString(),
                        IssueQty = Convert.ToDecimal(row["IssueQty"]),
                        AltIssueQty = Convert.ToDecimal(row["AltIssueQty"]),
                        PendQty = Convert.ToDecimal(row["PendQty"]),
                        Unit = row["rmunit"].ToString(),
                        LotStock = Convert.ToDecimal(row["LotStock"]),
                        TotalStock = Convert.ToDecimal(row["TotalStock"]),
                        AltQty = Convert.ToDecimal(row["AltQty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        Rate = Convert.ToDecimal(row["Rate"]),
                        Remark = row["Remark"].ToString(),
                        WorkCenter = row["WorkCenter"].ToString(),
                        AltItemCode = Convert.ToInt32(row["AltItemCode"]),
                        CostCenterId = Convert.ToInt32(row["CostCenterId"]),
                        ItemSize = row["ItemSize"].ToString(),
                        ItemColor = row["ItemColor"].ToString(),
                        StdPacking = Convert.ToSingle(row["STDPkg"].ToString()),
                        IssuedAlternateItem = row["IssuedAlternateItem"].ToString(),
                        OriginalItemCode = Convert.ToInt32(row["OriginalItemCode"].ToString()),                        
                        //ProjectNo = row["ProjectNo"].ToString(),
                        //ProjectYearCode = Convert.ToInt32(row["ProjectYearcode"]),
                        WONO = row["WONO"].ToString(),
                        WOYearCode = Convert.ToInt32(row["WOYearCode"].ToString()),
                        WoDate = row["WOdate"].ToString(),
                        MachineCodee = row["Machinecode"].ToString(),
                        //WipStock = row["ProjectNo"].ToString(),
                    });
                }
                model.ItemDetailGrid = ItemGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    ItemGrid1.Add(new IssueThrBomFGData
                    {
                        Seqno = cnt++,
                        EntryId = Convert.ToInt32(row["EntryID"].ToString()),
                        YearCode = Convert.ToInt32(row["YearCode"].ToString()),
                        EntryDate = row["ReqNo"].ToString(),
                        WONO = row["WONO"].ToString(),
                        WOYearCode = Convert.ToInt32(row["WOYearCode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemCode"].ToString()),
                        FGItemName = row["FGITEMNAME"].ToString(),
                        FGPartCode = row["FGPARTCODE"].ToString(),
                        Unit = row["Unit"].ToString(),
                        FGQty = Convert.ToDecimal(row["FGQty"]),
                        BOMNO = Convert.ToInt32(row["BOMNO"].ToString()),
                        BOMDate = row["BOMDate"].ToString(),
                        FGStockINStore =  Convert.ToInt32(row["FGStockInStore"].ToString()),
                        //IssueFromStoreID =  Convert.ToInt32(row["Unit"].ToString()),
                        //Remark = row["Remark"].ToString(),
                        WCID =  Convert.ToInt32(row["WCID"].ToString()),
                        ReqNo = row["ReqNo"].ToString(),
                        ReqYearCode = Convert.ToInt32(row["ReqYearCode"].ToString()),
                        ReqDate = row["ReqDate"].ToString(),
                        //ProjectNo = row["ProjectNo"].ToString(),
                        //ProjectYearCode = Convert.ToInt32(row["ProjectYearcode"]),
                    });
                }
                model.FGItemDetailGrid = ItemGrid1;
            }
            return model;
        }
        public async Task<IssueThrBomDashboard> GetSearchData(string DashboardType, string FromDate, string ToDate, string IssueSlipNo, string ReqNo, string WCName, string ItemName, string PartCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new IssueThrBomDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_IssueWithBOM", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@REQNo", ReqNo);
                    oCmd.Parameters.AddWithValue("@WCName", WCName);

                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@itemname", ItemName);
                    oCmd.Parameters.AddWithValue("@IssueSlipNo", IssueSlipNo);
                    oCmd.Parameters.AddWithValue("@DashBoardSearchType", DashboardType);
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
                    //                    ,Rm.RMPartCode 'RMPartCode',
                    //                    rm.ReqQty, rm.IssueQty , rm.rmUnit, rm.PendQty , rm.AltReqQty , rm.AltIssueQty , rm.AltUnit, rm.BatchNo, rm.uniquebatchNo ,
                    //rm.lotStock, Rm.TotalStock,Rm.IssuedAlternateItem, 
                    //rm.WorkCenter ,   rm.jobCardNo, rm.JobcardDate,  rm.OrginalItemName, OriginalPartCode,   FGPARTCODE,FGITEMNAME,
                    //rm.RMRemark,rm.itemsize, rm.itemcolor ,ReqYearCode, EntryId,YearCode
                    model.IssueThrBOMDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new IssueThrBomMainDashboard
                                                  {
                                                      //,,,,,
                                                      ReqNo = dr["REQNo"].ToString(),
                                                      ReqDate = dr["ReqDate"].ToString(),
                                                      IssueSlipno = dr["IssueSlipno"].ToString(),
                                                      IssueDate = dr["IssueDate"].ToString(),
                                                      RMItemName = dr["RMItem"].ToString(),
                                                      RMPartCode = dr["RMPartCode"].ToString(),
                                                      ReqQty = Convert.ToSingle(dr["ReqQty"]),
                                                      IssueQty = Convert.ToSingle(dr["IssueQty"]),
                                                      rmUnit = dr["rmUnit"].ToString(),
                                                      PendQty = Convert.ToSingle(dr["PendQty"]),
                                                      AltReqQty = Convert.ToSingle(dr["AltReqQty"]),
                                                      AltIssueQty = Convert.ToSingle(dr["AltIssueQty"]),
                                                      AltUnit = dr["AltUnit"].ToString(),
                                                      BatchNo = dr["BatchNo"].ToString(),
                                                      uniquebatchNo = dr["uniquebatchNo"].ToString(),
                                                      lotStock = Convert.ToSingle(dr["lotStock"]),
                                                      TotalStock = Convert.ToSingle(dr["TotalStock"]),
                                                      IssuedAlternateItem = dr["IssuedAlternateItem"].ToString(),
                                                      WorkCenter = dr["WorkCenter"].ToString(),
                                                      jobCardNo = dr["jobCardNo"].ToString(),
                                                      JobcardDate = dr["JobcardDate"].ToString(),
                                                      OrginalItemName = dr["OrginalItemName"].ToString(),
                                                      OriginalPartCode = dr["OriginalPartCode"].ToString(),
                                                      FGPartCode = dr["FGPartCode"].ToString(),
                                                      FGItemName = dr["FGITEMNAME"].ToString(),
                                                      RMRemark = dr["RMRemark"].ToString(),
                                                      itemsize = dr["itemsize"].ToString(),
                                                      itemcolor = dr["itemcolor"].ToString(),
                                                      ReqYearCode = Convert.ToInt32(dr["ReqYearCode"]),
                                                      EntryId = Convert.ToInt32(dr["EntryId"]),
                                                      YearCode = Convert.ToInt32(dr["EntryId"]),


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

        public async Task<ResponseResult> GetDashboardData(string Fromdate, string Todate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (Flag == "True")
                {
                    //DateTime FromDt = DateTime.Parse(Fromdate, CultureInfo.InvariantCulture);
                    //DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(Fromdate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                }
                else
                {
                    //DateTime FromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime todt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(Fromdate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string DashboardType, string IssueSlipNo, string ReqNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime frmDt = new DateTime();
                //DateTime toDt = new DateTime();

               var frmDt = CommonFunc.ParseFormattedDate(FromDate);
               var toDt = CommonFunc.ParseFormattedDate(ToDate);

                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                SqlParams.Add(new SqlParameter("@FromDate", frmDt == default ? string.Empty : frmDt));
                SqlParams.Add(new SqlParameter("@ToDate", toDt == default ? string.Empty : toDt));
                SqlParams.Add(new SqlParameter("@IssueSlipNo", IssueSlipNo));
                SqlParams.Add(new SqlParameter("@REQNo", ReqNo));
                SqlParams.Add(new SqlParameter("@DashBoardSearchType", DashboardType));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);
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
