using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class BomModuleDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        //private readonly IConfiguration configuration;
        private readonly DataSet oDataSet = new();

        private readonly DataTable oDataTable = new();
        private dynamic? _ResponseResult;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public BomModuleDAL(IConfiguration configuration,IDataLogic dataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> DeleteByID(string FIC, int BMNo, string Flag)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@FinishItemCode", FIC);
                        oCmd.Parameters.AddWithValue("@BomNo", BMNo);

                        await myConnection.OpenAsync();

                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }
                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)oDataTable.Rows[0]["StatusCode"],
                                StatusText = oDataTable.Rows[0]["StatusText"].ToString(),
                                Result = oDataTable.Rows[0]["Result"].ToString()
                            };
                        }
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
                oDataTable.Dispose();
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Bom"));                

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
        public async Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                SqlParams.Add(new SqlParameter("@FGPartCode", FGPartCode));
                SqlParams.Add(new SqlParameter("@RMPartCode",RMPartCode));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Bom", SqlParams);
               
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAltItemCode(string AltPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAltItemCode"));
                SqlParams.Add(new SqlParameter("@AltPartCode", AltPartCode));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Bom", SqlParams);
               
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetByProdItemName(int MainItemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetByProdItem"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", MainItemcode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<BomModel> EditBomDetail(string FIC, int BMNo, string Flag)
        {
            BomModel? model = new BomModel();
            List<BomModel>? _List = new List<BomModel>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", FIC);
                        oCmd.Parameters.AddWithValue("@BomNo", BMNo);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            model.FinishItemCode = Convert.ToInt32(oDataTable.Rows[0]["FinishItemCode"]);
                            model.FinishedItemName = oDataTable.Rows[0]["FinishItemCode"].ToString();
                            model.BOMName = oDataTable.Rows[0]["BOMName"].ToString();
                            model.BomNo = Convert.ToInt32(oDataTable.Rows[0]["BomNo"]);
                            model.BomQty = Convert.ToDecimal(oDataTable.Rows[0]["BomQty"]);
                            model.EntryDate = oDataTable.Rows[0]["EntryDate"].ToString();
                            model.EffectiveDate = oDataTable.Rows[0]["EffectiveDate"].ToString();

                            //model.CreatedBy = Convert.ToInt32(oDataTable.Rows[0]["CreatedBy"]);
                            //model.CreatedOn = Convert.ToDateTime(oDataTable.Rows[0]["CreatedOn"]);
                            //model.Active = oDataTable.Rows[0]["Active"].ToString();

                            model.ItemCode = Convert.ToInt32(oDataTable.Rows[0]["ItemCode"]);
                            model.ItemName = oDataTable.Rows[0]["ItemCode"].ToString();
                            model.Qty = Convert.ToDecimal(oDataTable.Rows[0]["Qty"]);
                            model.Unit = oDataTable.Rows[0]["Unit"].ToString();
                            model.CreatedByName = oDataTable.Rows[0]["CreatedByName"].ToString();
                            model.CreatedByName = oDataTable.Rows[0]["CreatedByName"].ToString();
                            model.UID = oDataTable.Rows[0]["CreatedByName"].ToString();
                            model.CreatedBy = Convert.ToInt32(oDataTable.Rows[0]["CreatedBy"]);
                            model.CreatedOn = oDataTable.Rows[0]["CreatedOn"] != DBNull.Value
    ? Convert.ToDateTime(oDataTable.Rows[0]["CreatedOn"])
    : (DateTime?)null;
                            model.CC = oDataTable.Rows[0]["CC"]?.ToString();
                            model.UID = oDataTable.Rows[0]["UID"].ToString();
                            if (!string.IsNullOrEmpty(oDataTable.Rows[0]["UpdatedByName"].ToString()))
                            {
                                model.UpdatedBy = Convert.ToInt32(oDataTable.Rows[0]["UpdatedBy"]);
                                model.UpdatedByName = oDataTable.Rows[0]["UpdatedByName"]?.ToString();
                                model.UpdatedOn = Convert.ToDateTime(oDataTable.Rows[0]["UpdatedOn"]);
                            }
                            model.BomList = (from DataRow dr in oDataTable.Rows
                                             select new BomModel
                                             {
                                                 FinishItemCode = Convert.ToInt32(dr["FinishItemCode"]),
                                                 FinishedItemName = dr["FinishItemCode"].ToString(),
                                                 BOMName = dr["BOMName"].ToString(),
                                                 BomNo = Convert.ToInt32(dr["BomNo"]),
                                                 BomQty = Convert.ToDecimal(dr["BomQty"]),
                                                 EntryDate = dr["EntryDate"].ToString(),
                                                 EffectiveDate = dr["EffectiveDate"].ToString(),
                                                 SeqNo = Convert.ToInt32(dr["SeqNo"]),
                                                 ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                                 ICName = dr["ICName"].ToString(),
                                                 ItemName = dr["RMItemName"].ToString(),
                                                 Qty = Convert.ToDecimal(dr["Qty"]),
                                                 Unit = dr["Unit"].ToString(),
                                                 UsedStageId = dr["UsedStageId"].ToString(),
                                                // AltItemCode1 = Convert.ToInt32(dr["AltItemCode1"]),
                                                 AltItemCode1 = dr["AltItemCode1"] != DBNull.Value ? Convert.ToInt32(dr["AltItemCode1"]) : 0,
                                                 AICName1 = dr["AICName1"].ToString(),
                                                 AltItemName1 = dr["AltItemName1"].ToString(),
                                                 //AltQty1 = Convert.ToDecimal(dr["AltQty1"]),
                                                 AltQty1 = dr["AltQty1"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty1"]) : 0,
                                                 AltItemCode2 = dr["AltItemCode2"] != DBNull.Value ? Convert.ToInt32(dr["AltItemCode2"]) : 0,
                                                 AICName2 = dr["AICName2"].ToString(),
                                                 AltItemName2 = dr["AltItemName2"].ToString(),
                                                 //AltQty2 = Convert.ToDecimal(dr["AltQty2"]),
                                                 AltQty2 = dr["AltQty2"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty2"]) : 0,
                                                 IssueToJOBwork = dr["IssueToJOBwork"].ToString(),
                                                 DirectProcess = dr["DirectProcess"].ToString(),
                                                 RecFrmCustJobwork = dr["RecFrmCustJobwork"].ToString(),
                                                 PkgItem = dr["PkgItem"].ToString(),
                                                 Remark = dr["Remark"].ToString(),
                                                 GrossWt = dr["GrossWt"] != DBNull.Value ? Convert.ToDecimal(dr["GrossWt"]) : 0,
                                                 //GrossWt = Convert.ToDecimal(dr["GrossWt"]),
                                                 NetWt = dr["NetWt"] != DBNull.Value ? Convert.ToDecimal(dr["NetWt"]) : 0   ,
                                                 Scrap = dr["Scrap"] != DBNull.Value ? Convert.ToDecimal(dr["Scrap"]) : 0,
                                                 RunnerItemCode = dr["RunnerItemCode"] != DBNull.Value ? Convert.ToInt32(dr["RunnerItemCode"]) : 0,
                                                 RunnerQty = dr["RunnerQty"] != DBNull.Value ? Convert.ToDecimal(dr["RunnerQty"]) : 0,
                                                 BurnQty = dr["BurnQty"] != DBNull.Value ? Convert.ToDecimal(dr["BurnQty"]) : 0,
                                                 Location = dr["Location"].ToString(),
                                                 MPNNo = dr["MPNNo"].ToString(),
                                                 CustJWmandatory = dr["CustJWmandatory"].ToString(),
                                                 ByprodItemCode1 = dr["ByprodItemCode1"] != DBNull.Value ? Convert.ToInt32(dr["ByprodItemCode1"]) : 0,
                                                 ByProdQty1 = dr["ByprodItemcQty1"] != DBNull.Value ? Convert.ToDecimal(dr["ByprodItemcQty1"]) : 0,
                                                 ByprodItemCode2 = dr["ByprodItemcode2"] != DBNull.Value ? Convert.ToInt32(dr["ByprodItemcode2"]) : 0,
                                                 ByProdQty2 = dr["ByprodItemcQty2"] != DBNull.Value ? Convert.ToDecimal(dr["ByprodItemcQty2"]) : 0,
                                                 ByprodItemName1 = dr["ByProdItem1"] != DBNull.Value ? dr["ByProdItem1"].ToString() : string.Empty,
                                                 ByprodItemName2 = dr["ByProdItem2"] != DBNull.Value ? dr["ByProdItem2"].ToString() : string.Empty,
                                                 Byprodpartcode1 = dr["ByprodPartCode1"] != DBNull.Value ? dr["ByprodPartCode1"].ToString() : string.Empty,
                                                 Byprodpartcode2 = dr["ByprodPartCode2"] != DBNull.Value ? dr["ByprodPartCode2"].ToString() : string.Empty,


                                             }).ToList();
                        }
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
                oDataTable.Dispose();
            }
            return model;
        }

        public async Task<DataTable> EditBomSeq(BomModel model)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                        oCmd.Parameters.AddWithValue("@ID", model.ItemCode);
                        oCmd.Parameters.AddWithValue("@BomNo", model.BomNo);
                        oCmd.Parameters.AddWithValue("@BomQty", model.SeqNo);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }
                        if (oDataTable.Rows.Count > 0)
                        {
                            oDataTable.Rows[0]["Qty"] = (float)Convert.ToDecimal(oDataTable.Rows[0]["Qty"]);
                        }
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
                oDataTable.Dispose();
            }
            return oDataTable;
        }
        public async Task<DataTable> CheckDupeConstraint()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "CheckDupeConstraint");
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }
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
                oDataTable.Dispose();
            }
            return oDataTable;
        }

        public async Task<DataSet> GetBomDashboard(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Bom", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    //_ResponseResult.Result.Tables[0].TableName = "TaxTypeList";
                    //_ResponseResult.Result.Tables[1].TableName = "HSNList";
                    //_ResponseResult.Result.Tables[2].TableName = "ParentGroupList";
                    //_ResponseResult.Result.Tables[3].TableName = "SGSTHeadList";
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

        public DataTable GetBomDetail(string FGC, int BMNo, string Flag)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", FGC);
                        oCmd.Parameters.AddWithValue("@BomNo", BMNo);
                        myConnection.Open();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }
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
                oDataTable.Dispose();
            }
            return oDataTable;
        }
        public BomModel GetGridData(int IC, int BomNo)
        {
            var model=new BomModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "GetGridData");
                        oCmd.Parameters.AddWithValue("@FinishItemCode", IC);
                        oCmd.Parameters.AddWithValue("@BomNo", BomNo);

                        myConnection.Open();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }
                        int SeqNo = 1;
                        if (oDataTable.Rows.Count > 0)
                        {
                            model.BomList = (from DataRow dr in oDataTable.Rows
                                                             select new BomModel
                                                             {
                                                                 FinishItemCode = string.IsNullOrEmpty(dr["FinishItemCode"].ToString()) ?0: Convert.ToInt32(dr["FinishItemCode"]),
                                                                 FinishedItemName = dr["FinishItemCode"].ToString(),
                                                                 BOMName = dr["BOMName"].ToString(),
                                                                 BomNo = string.IsNullOrEmpty(dr["BomNo"].ToString()) ? 0 : Convert.ToInt32(dr["BomNo"]),
                                                                 BomQty = string.IsNullOrEmpty(dr["BomQty"].ToString()) ? 0 : Convert.ToDecimal(dr["BomQty"]),
                                                                 EntryDate = dr["EntryDate"].ToString(),
                                                                 EffectiveDate = dr["EffectiveDate"].ToString(),
                                                                 SeqNo = SeqNo++,
                                                                 ItemCode = string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["ItemCode"]),
                                                                 ICName = dr["ICName"].ToString(),
                                                                 ItemName = dr["RMItemName"].ToString(),
                                                                 Qty = string.IsNullOrEmpty(dr["Qty"].ToString()) ? 0 : Convert.ToDecimal(dr["Qty"]),
                                                                 Unit = dr["Unit"].ToString(),
                                                                 UsedStageId = dr["UsedStageId"].ToString(),
                                                                 AltItemCode1 = string.IsNullOrEmpty(dr["AltItemCode1"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode1"]),
                                                                 AICName1 = dr["AICName1"].ToString(),
                                                                 AltItemName1 = dr["AltItemName1"].ToString(),
                                                                 AltQty1 = string.IsNullOrEmpty(dr["AltQty1"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty1"]),
                                                                 AltItemCode2 = string.IsNullOrEmpty(dr["AltItemCode2"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode2"]),
                                                                 AICName2 = dr["AICName2"].ToString(),
                                                                 AltItemName2 = dr["AltItemName2"].ToString(),
                                                                 AltQty2 = string.IsNullOrEmpty(dr["AltQty2"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty2"]),
                                                                 IssueToJOBwork = dr["IssueToJOBwork"].ToString(),
                                                                 DirectProcess = dr["DirectProcess"].ToString(),
                                                                 RecFrmCustJobwork = dr["RecFrmCustJobwork"].ToString(),
                                                                 PkgItem = dr["PkgItem"].ToString(),
                                                                 Remark = dr["Remark"].ToString(),
                                                                 GrossWt = string.IsNullOrEmpty(dr["GrossWt"].ToString()) ? 0 : Convert.ToDecimal(dr["GrossWt"]),
                                                                 NetWt = string.IsNullOrEmpty(dr["NetWt"].ToString()) ? 0 : Convert.ToDecimal(dr["NetWt"]),
                                                                 Scrap = string.IsNullOrEmpty(dr["Scrap"].ToString()) ? 0 : Convert.ToDecimal(dr["Scrap"]),
                                                                 RunnerItemCode = string.IsNullOrEmpty(dr["RunnerItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["RunnerItemCode"]),
                                                                 RunnerQty = string.IsNullOrEmpty(dr["RunnerQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RunnerQty"]),
                                                                 BurnQty = string.IsNullOrEmpty(dr["BurnQty"].ToString()) ? 0 : Convert.ToDecimal(dr["BurnQty"]),
                                                             }).ToList();
                        }
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
                oDataTable.Dispose();
            }
            return model;
        }

        public int GetBomNo(int ID, string Flag)
        {
            object BomNo = 0;
            try
            {
                if (ID > 0)
                {
                    using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                    {
                        using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                        {
                            oCmd.CommandType = CommandType.StoredProcedure;
                            oCmd.Parameters.AddWithValue("@Flag", Flag);
                            oCmd.Parameters.AddWithValue("@ID", ID);
                            myConnection.Open();
                            BomNo = oCmd.ExecuteScalar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return BomNo == null ? 1 : Convert.ToInt32(BomNo);
        }

        public async Task<string> VerifyPartCode(DataTable bomDataTable)
        {
            var JsonString = string.Empty;
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CHEKIMPORTDATA"));
                SqlParams.Add(new SqlParameter("@DtChk", bomDataTable));

                _ResponseResult = await  _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
                JsonString = JsonConvert.SerializeObject(_ResponseResult.Result);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return JsonString;
        }

        public async Task<BomDashboard> GetSearchData(BomDashboard model)
        {
            DataTable oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "Search");
                        oCmd.Parameters.AddWithValue("@FGPartCode", model.FGPartCode == null ? "" : model.FGPartCode);
                        oCmd.Parameters.AddWithValue("@FGItemName", model.FGItemName == null ? "" : model.FGItemName);
                        oCmd.Parameters.AddWithValue("@RMPartCode", model.RMPartCode == null ? "" : model.RMPartCode);
                        oCmd.Parameters.AddWithValue("@RMItemName", model.RMItemName == null ? "" : model.RMItemName);
                        oCmd.Parameters.AddWithValue("@BomNo", model.BomRevNo == null ? "" : model.BomRevNo);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            model.DTDashboard = oDataTable;
                        }
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
                oDataTable.Dispose();
            }
            return model;
        }
        public async Task<BomDashboard> GetDetailSearchData(BomDashboard model)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "DetailDashboard");
                        oCmd.Parameters.AddWithValue("@FGPartCode", model.FGPartCode);
                        oCmd.Parameters.AddWithValue("@FGItemName", model.FGItemName);
                        oCmd.Parameters.AddWithValue("@RMPartCode", model.RMPartCode);
                        oCmd.Parameters.AddWithValue("@RMItemName", model.RMItemName);
                        oCmd.Parameters.AddWithValue("@BomNo", model.BomRevNo);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            model.DTDashboard = oDataTable;
                        }
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
                oDataTable.Dispose();
            }
            return model;
        }

        public string GetUnit(string IC, string Mode)
        {
            object _Unit = "";
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Mode);
                        oCmd.Parameters.AddWithValue("@ID", IC);
                        myConnection.Open();
                        _Unit = oCmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _Unit.ToString();
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
        public async Task<ResponseResult> SaveBomData(DataTable DT, BomModel model)
        {
            try
            {
                //DateTime entDt = new DateTime();
                //DateTime EffDt = new DateTime();
                //entDt = ParseDate(model.EntryDate);
                //EffDt = ParseDate(model.EffectiveDate);

                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@DTBomGrid", DT);
                    oCmd.Parameters.AddWithValue("@FinishItemCode", model.FinishItemCode);
                    oCmd.Parameters.AddWithValue("@BOMName", model.BOMName);
                    oCmd.Parameters.AddWithValue("@BomNo", model.BomNo);
                    oCmd.Parameters.AddWithValue("@BomQty", model.BomQty);
                    //oCmd.Parameters.AddWithValue("@EntryDate", entDt == default ? string.Empty : entDt);
                    //oCmd.Parameters.AddWithValue("@EffectiveDate", EffDt == default ? string.Empty : EffDt);
                    oCmd.Parameters.AddWithValue("@EntryDate", model.EntryDate);
                    oCmd.Parameters.AddWithValue("@EffectiveDate", model.EffectiveDate);
                    oCmd.Parameters.AddWithValue("@YearCode", model.YearCode);
                    oCmd.Parameters.AddWithValue("@UID", model.UID);
                    oCmd.Parameters.AddWithValue("@CC", model.CC);
                    oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    //oCmd.Parameters.AddWithValue("@ByprodItemcode1", model.ByprodItemCode1);
                    //oCmd.Parameters.AddWithValue("@ByprodItemcQty1", model.ByProdQty1);
                    //oCmd.Parameters.AddWithValue("@ByprodItemcode2", model.ByprodItemCode2);
                    //oCmd.Parameters.AddWithValue("@ByprodItemcQty2", model.ByProdQty2);

                    oCmd.Parameters.AddWithValue("@EntryByMachineName", model.EntryByMachineName);
                    if (model.Mode == "Update")
                    {
                        oCmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);

                    }

                    myConnection.Open();

                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusText = Reader["StatusText"].ToString(),
                                Result = Reader["Result"].ToString()
                            };
                        }
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
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveMultipleBomData(DataTable ItemDetailGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "INSERTMULTIPLE"));
                SqlParams.Add(new SqlParameter("@DTBomModule", ItemDetailGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal int GetBomStatus(int ItemCode, int BomNo)
        {
            object BomStatus = 0;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;

                        oCmd.Parameters.AddWithValue("@Flag", "BOMSTATUS");
                        oCmd.Parameters.AddWithValue("@FinishItemCode", ItemCode);
                        oCmd.Parameters.AddWithValue("@BomNo", BomNo);

                        myConnection.OpenAsync();
                        BomStatus = oCmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return Convert.ToInt32(BomStatus);
        }
    }
}