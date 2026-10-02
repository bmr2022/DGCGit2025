using DocumentFormat.OpenXml.Spreadsheet;
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
        private readonly ICommon _common;

        private readonly ConnectionStringService _connectionStringService;

        public BomModuleDAL(IConfiguration configuration, IDataLogic dataLogic, ConnectionStringService connectionStringService, ICommon common)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _common = common;
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
        public async Task<ResponseResult> GetIndustryType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetIndustryType"));


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
        public async Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode, string RunnerPartCode, string AltPartCode1, string AltPartCode2, string AltPartCode3, string AltPartCode4, string AltPartCode5)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                SqlParams.Add(new SqlParameter("@FGPartCode", FGPartCode));
                SqlParams.Add(new SqlParameter("@RMPartCode", RMPartCode));
                SqlParams.Add(new SqlParameter("@RunnerPartCode", RunnerPartCode));
                SqlParams.Add(new SqlParameter("@AltPartCode1", AltPartCode1));
                SqlParams.Add(new SqlParameter("@AltPartCode2", AltPartCode2));
                SqlParams.Add(new SqlParameter("@AltPartCode3", AltPartCode3));
                SqlParams.Add(new SqlParameter("@AltPartCode4", AltPartCode4));
                SqlParams.Add(new SqlParameter("@AltPartCode5", AltPartCode5));

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
        public async Task<ResponseResult> IsBomExists(string FGPartCode, string RMPartCode, int BomNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CHECKDUPLICATE"));
                SqlParams.Add(new SqlParameter("@FGPartCode", FGPartCode));
                SqlParams.Add(new SqlParameter("@RMPartCode", RMPartCode));
                SqlParams.Add(new SqlParameter("@BomNo", BomNo));

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
                            model.FinishItemCodeText = (oDataTable.Rows[0]["FinishPartCode"]).ToString();
                            model.FinishedItemNameText = oDataTable.Rows[0]["FinishItemName"].ToString();
                            model.BOMName = oDataTable.Rows[0]["BOMName"].ToString();
                            model.BomNo = Convert.ToInt32(oDataTable.Rows[0]["BomNo"]);
                            //model.BomQty = Convert.ToDecimal(oDataTable.Rows[0]["BomQty"]);
                            model.BomQty = oDataTable.Rows[0]["BomQty"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["BomQty"]);
                            model.EntryDate = oDataTable.Rows[0]["EntryDate"].ToString();
                            model.EffectiveDate = oDataTable.Rows[0]["EffectiveDate"].ToString();
                            model.Ldash = oDataTable.Rows[0]["Ldash"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["Ldash"]);
                            model.Adash = oDataTable.Rows[0]["Adash"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["Adash"]);
                            model.Bdash = oDataTable.Rows[0]["Bdash"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["Bdash"]);
                            model.LValue = oDataTable.Rows[0]["LValue"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["LValue"]);
                            model.AValue = oDataTable.Rows[0]["AValue"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["AValue"]);
                            model.BValue = oDataTable.Rows[0]["BValue"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["BValue"]);
                            model.DeltaE = oDataTable.Rows[0]["DeltaE"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["DeltaE"]);
                            model.TotalAmount = oDataTable.Rows[0]["TotalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["TotalAmount"]);
                            model.MaterialCost = oDataTable.Rows[0]["MaterialCost"] == DBNull.Value ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["MaterialCost"]);

                            //model.CreatedBy = Convert.ToInt32(oDataTable.Rows[0]["CreatedBy"]);
                            //model.CreatedOn = Convert.ToDateTime(oDataTable.Rows[0]["CreatedOn"]);
                            //model.Active = oDataTable.Rows[0]["Active"].ToString();

                            model.ItemCode = Convert.ToInt32(oDataTable.Rows[0]["ItemCode"]);
                            model.ItemName = oDataTable.Rows[0]["ItemCode"].ToString();
                            model.ItemNameText = oDataTable.Rows[0]["ItemName"].ToString();
                            model.PartCodeText = oDataTable.Rows[0]["PartCode"].ToString();
                            model.TotalRmQtyForTotBomQty = string.IsNullOrEmpty(oDataTable.Rows[0]["TotalRmQtyForTotBomQty"].ToString()) ? 0 : Convert.ToDecimal(oDataTable.Rows[0]["TotalRmQtyForTotBomQty"]);
                            model.Qty = Convert.ToDecimal(oDataTable.Rows[0]["Qty"]);
                            model.Unit = oDataTable.Rows[0]["Unit"].ToString();
                            model.CreatedByName = oDataTable.Rows[0]["CreatedByName"].ToString();
                            model.CreatedByName = oDataTable.Rows[0]["CreatedByName"].ToString();
                            model.UID = oDataTable.Rows[0]["CreatedByName"].ToString();
                            //model.CreatedBy = Convert.ToInt32(oDataTable.Rows[0]["CreatedBy"]);
                            model.CreatedBy = oDataTable.Rows[0]["CreatedBy"] != DBNull.Value ? Convert.ToInt32(oDataTable.Rows[0]["CreatedBy"]) : 0;
                            model.CreatedOn = oDataTable.Rows[0]["CreatedOn"] != DBNull.Value
    ? Convert.ToDateTime(oDataTable.Rows[0]["CreatedOn"])
    : (DateTime?)null;
                            model.CC = oDataTable.Rows[0]["CC"]?.ToString();
                            model.UID = oDataTable.Rows[0]["UID"].ToString();
                            model.Location = oDataTable.Rows[0]["Location"].ToString();
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
                                                 grade = dr["grade"].ToString(),
                                                 BomNo = Convert.ToInt32(dr["BomNo"]),
                                                 //grade = dr["grade"] != DBNull.Value ? dr["grade"].ToString() : string.Empty,
                                                 //BomQty = Convert.ToDecimal(dr["BomQty"]),
                                                 BomQty = dr["BomQty"] != DBNull.Value ? Convert.ToDecimal(dr["BomQty"]) : 0,
                                                 EntryDate = dr["EntryDate"].ToString(),
                                                 EffectiveDate = dr["EffectiveDate"].ToString(),
                                                 Ldash = dr["Ldash"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Ldash"]),
                                                 Adash = dr["Adash"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Adash"]),
                                                 Bdash = dr["Bdash"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Bdash"]),
                                                 LValue = dr["LValue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["LValue"]),
                                                 AValue = dr["AValue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AValue"]),
                                                 BValue = dr["BValue"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["BValue"]),
                                                 DeltaE = dr["DeltaE"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DeltaE"]),
                                                 TotalRmQtyForTotBomQty = string.IsNullOrEmpty(dr["TotalRmQtyForTotBomQty"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalRmQtyForTotBomQty"]),
                                                 //SeqNo = Convert.ToInt32(dr["SeqNo"]),
                                                 SeqNo = oDataTable.Rows.IndexOf(dr) + 1,
                                                 ItemCode = Convert.ToInt32(dr["ItemCode"]),
                                                 ICName = dr["ICName"].ToString(),
                                                 ItemName = dr["RMItemName"].ToString(),
                                                 //Qty = Convert.ToDecimal(dr["Qty"]),
                                                 Qty = dr["Qty"] != DBNull.Value ? Convert.ToDecimal(dr["Qty"]) : 0,
                                                 Unit = dr["Unit"].ToString(),
                                                 Rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Rate"]),
                                                 Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]),
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
                                                 //Gra = dr["Grade"].ToString(),
                                                 //grade= dr["grade"] != DBNull.Value ? dr["grade"].ToString() : string.Empty,
                                                 //AltQty2 = Convert.ToDecimal(dr["AltQty2"]),
                                                 AltQty2 = dr["AltQty2"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty2"]) : 0,



                                                 AltItemCode3 = dr["AltItemCode3"] != DBNull.Value ? Convert.ToInt32(dr["AltItemCode3"]) : 0,
                                                 AICName3 = dr["AICName3"].ToString(),
                                                 AltItemName3 = dr["AltItemName3"].ToString(),

                                                 AltQty3 = dr["AltQty3"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty3"]) : 0,



                                                 AltItemCode4 = dr["AltItemCode4"] != DBNull.Value ? Convert.ToInt32(dr["AltItemCode4"]) : 0,
                                                 AICName4 = dr["AICName4"].ToString(),
                                                 AltItemName4 = dr["AltItemName4"].ToString(),

                                                 AltQty4 = dr["AltQty4"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty4"]) : 0,

                                                 AltItemCode5 = dr["AltItemCode5"] != DBNull.Value ? Convert.ToInt32(dr["AltItemCode5"]) : 0,
                                                 AICName5 = dr["AICName5"].ToString(),
                                                 AltItemName5 = dr["AltItemName5"].ToString(),

                                                 AltQty5 = dr["AltQty5"] != DBNull.Value ? Convert.ToDecimal(dr["AltQty5"]) : 0,

                                                 IssueToJOBwork = dr["IssueToJOBwork"].ToString(),
                                                 DirectProcess = dr["DirectProcess"].ToString(),
                                                 RecFrmCustJobwork = dr["RecFrmCustJobwork"].ToString(),
                                                 PkgItem = dr["PkgItem"].ToString(),
                                                 Remark = dr["Remark"].ToString(),
                                                 GrossWt = dr["GrossWt"] != DBNull.Value ? Convert.ToDecimal(dr["GrossWt"]) : 0,
                                                 //GrossWt = Convert.ToDecimal(dr["GrossWt"]),
                                                 NetWt = dr["NetWt"] != DBNull.Value ? Convert.ToDecimal(dr["NetWt"]) : 0,
                                                 Scrap = dr["Scrap"] != DBNull.Value ? Convert.ToDecimal(dr["Scrap"]) : 0,
                                                 RunnerItemCode = dr["RunnerItemCode"] != DBNull.Value ? Convert.ToInt32(dr["RunnerItemCode"]) : 0,
                                                 RunnerQty = dr["RunnerQty"] != DBNull.Value ? Convert.ToDecimal(dr["RunnerQty"]) : 0,
                                                 BurnQty = dr["BurnQty"] != DBNull.Value ? Convert.ToDecimal(dr["BurnQty"]) : 0,
                                                 Location = dr["Location"].ToString(),
                                                 RunnerPartCode = dr["RunnerPartCode"].ToString(),
                                                 MPNNo = dr["MPNNo"].ToString(),
                                                 CustJWmandatory = dr["CustJWmandatory"].ToString(),
                                                 CustJwAdjustmentMandatory = dr["CustJwAdjustmentMandatory"].ToString(),
                                                 ByprodItemCode1 = dr["ByprodItemCode1"] != DBNull.Value ? Convert.ToInt32(dr["ByprodItemCode1"]) : 0,
                                                 ByProdQty1 = dr["ByprodItemcQty1"] != DBNull.Value ? Convert.ToDecimal(dr["ByprodItemcQty1"]) : 0,
                                                 ByprodItemCode2 = dr["ByprodItemcode2"] != DBNull.Value ? Convert.ToInt32(dr["ByprodItemcode2"]) : 0,
                                                 ByProdQty2 = dr["ByprodItemcQty2"] != DBNull.Value ? Convert.ToDecimal(dr["ByprodItemcQty2"]) : 0,
                                                 ByprodItemName1 = dr["ByProdItem1"] != DBNull.Value ? dr["ByProdItem1"].ToString() : string.Empty,
                                                 ByprodItemName2 = dr["ByProdItem2"] != DBNull.Value ? dr["ByProdItem2"].ToString() : string.Empty,
                                                 Byprodpartcode1 = dr["ByprodPartCode1"] != DBNull.Value ? dr["ByprodPartCode1"].ToString() : string.Empty,
                                                 Byprodpartcode2 = dr["ByprodPartCode2"] != DBNull.Value ? dr["ByprodPartCode2"].ToString() : string.Empty,
                                                 Dia = dr["Dia"] != DBNull.Value ? dr["Dia"].ToString() : string.Empty,

                                                 thickness = dr["thickness"] != DBNull.Value ? Convert.ToDecimal(dr["thickness"]) : 0,
                                                 width = dr["width"] != DBNull.Value ? Convert.ToDecimal(dr["width"]) : 0,
                                                 length = dr["length"] != DBNull.Value ? Convert.ToDecimal(dr["length"]) : 0,



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
            var model = new BomModel();
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
                                                 FinishItemCode = string.IsNullOrEmpty(dr["FinishItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["FinishItemCode"]),
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
                                                 TotalRmQtyForTotBomQty = string.IsNullOrEmpty(dr["TotalRmQtyForTotBomQty"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalRmQtyForTotBomQty"]),
                                                 Qty = string.IsNullOrEmpty(dr["Qty"].ToString()) ? 0 : Convert.ToDecimal(dr["Qty"]),
                                                 Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"]),
                                                 Unit = dr["Unit"].ToString(),
                                                 grade = dr["grade"].ToString(),
                                                 Location = dr["Location"].ToString(),

                                                 UsedStageId = dr["UsedStageId"].ToString(),
                                                 AltItemCode1 = string.IsNullOrEmpty(dr["AltItemCode1"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode1"]),
                                                 AICName1 = dr["AICName1"].ToString(),
                                                 AltItemName1 = dr["AltItemName1"].ToString(),
                                                 AltQty1 = string.IsNullOrEmpty(dr["AltQty1"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty1"]),
                                                 AltItemCode2 = string.IsNullOrEmpty(dr["AltItemCode2"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode2"]),
                                                 AICName2 = dr["AICName2"].ToString(),
                                                 AltItemName2 = dr["AltItemName2"].ToString(),
                                                 AltQty2 = string.IsNullOrEmpty(dr["AltQty2"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty2"]),


                                                 AltItemCode3 = string.IsNullOrEmpty(dr["AltItemCode3"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode3"]),
                                                 AICName3 = dr["AICName3"].ToString(),
                                                 AltItemName3 = dr["AltItemName3"].ToString(),
                                                 AltQty3 = string.IsNullOrEmpty(dr["AltQty3"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty3"]),
                                                 AltItemCode4 = string.IsNullOrEmpty(dr["AltItemCode4"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode4"]),
                                                 AICName4 = dr["AICName4"].ToString(),
                                                 AltItemName4 = dr["AltItemName4"].ToString(),
                                                 AltQty4 = string.IsNullOrEmpty(dr["AltQty4"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty4"]),

                                                 AltItemCode5 = string.IsNullOrEmpty(dr["AltItemCode5"].ToString()) ? 0 : Convert.ToInt32(dr["AltItemCode5"]),
                                                 AICName5 = dr["AICName5"].ToString(),
                                                 AltItemName5 = dr["AltItemName5"].ToString(),
                                                 AltQty5 = string.IsNullOrEmpty(dr["AltQty5"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty5"]),
                                                 IssueToJOBwork = dr["IssueToJOBwork"].ToString(),
                                                 DirectProcess = dr["DirectProcess"].ToString(),
                                                 RecFrmCustJobwork = dr["RecFrmCustJobwork"].ToString(),
                                                 PkgItem = dr["PkgItem"].ToString(),
                                                 Remark = dr["Remark"].ToString(),
                                                 RunnerPartCode = dr["RunnerPartCode"].ToString(),
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
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
        public async Task<ResponseResult> GetSearchData(BomDashboard model, int userID)
        {
            DataTable oDataTable = new DataTable();

            var flag = model.DashboardType;

            var parameters = new Dictionary<string, object>
      {
       { "@FGPartCode", model.FGPartCode == null ? "" : model.FGPartCode },
       { "@FGItemName",  model.FGItemName == null ? "" : model.FGItemName},
       { "@RMPartCode",model.RMPartCode == null ? "" : model.RMPartCode},
       { "@RMItemName", model.RMItemName == null ? "" : model.RMItemName},
       { "@BomNo", model.BomRevNo == null ? 0 : model.BomRevNo},
       { "@CreatedBy", userID},
       { "@DeltaE", 0},

      };

            return await _common.GetDashboardData(
                "SP_Bom",
                flag,
                parameters
            );



        }
        //public async Task<BomDashboard> GetSearchData(BomDashboard model, int userID)
        //{
        //    DataTable oDataTable = new DataTable();

        //    try
        //    {
        //        using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
        //        {
        //            using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
        //            {
        //                oCmd.CommandType = CommandType.StoredProcedure;
        //                oCmd.Parameters.AddWithValue("@Flag", "Search");
        //                oCmd.Parameters.AddWithValue("@FGPartCode", model.FGPartCode == null ? "" : model.FGPartCode);
        //                oCmd.Parameters.AddWithValue("@FGItemName", model.FGItemName == null ? "" : model.FGItemName);
        //                oCmd.Parameters.AddWithValue("@RMPartCode", model.RMPartCode == null ? "" : model.RMPartCode);
        //                oCmd.Parameters.AddWithValue("@RMItemName", model.RMItemName == null ? "" : model.RMItemName);
        //                oCmd.Parameters.AddWithValue("@BomNo", model.BomRevNo == null ? "" : model.BomRevNo);
        //                oCmd.Parameters.AddWithValue("@CreatedBy", userID);
        //                oCmd.Parameters.AddWithValue("@DeltaE", 0);
        //                await myConnection.OpenAsync();
        //                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
        //                {
        //                    oDataAdapter.Fill(oDataTable);
        //                }

        //                if (oDataTable.Rows.Count > 0)
        //                {
        //                    model.DTDashboard = oDataTable;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }
        //    finally
        //    {
        //        oDataTable.Dispose();
        //    }
        //    return model;
        //}
        public async Task<BomDashboard> GetDetailSearchData(BomDashboard model)
        {
            try
            {
                var flag = "DetailDashboard";

                var parameters = new Dictionary<string, object>
                  {
                   { "@FGPartCode", model.FGPartCode == null ? "" : model.FGPartCode },
                   { "@FGItemName",  model.FGItemName == null ? "" : model.FGItemName},
                   { "@RMPartCode",model.RMPartCode == null ? "" : model.RMPartCode},
                   { "@RMItemName", model.RMItemName == null ? "" : model.RMItemName},
                   { "@BomNo", model.BomRevNo == null ? "" : model.BomRevNo},

                   { "@DeltaE", 0},
                   { "@SummDetail", "Summary" }
                  };
                var result = await _common.GetDashboardData(
               "SP_Bom",
               flag,
               parameters
           );

                if (oDataTable.Rows.Count > 0)
                {
                    model.DTDashboard = result.Result;
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
        //public async Task<BomDashboard> GetDetailSearchData(BomDashboard model)
        //{
        //    try
        //    {
        //        using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
        //        {
        //            using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
        //            {
        //                oCmd.CommandType = CommandType.StoredProcedure;
        //                oCmd.Parameters.AddWithValue("@Flag", "DetailDashboard");
        //                oCmd.Parameters.AddWithValue("@FGPartCode", model.FGPartCode);
        //                oCmd.Parameters.AddWithValue("@FGItemName", model.FGItemName);
        //                oCmd.Parameters.AddWithValue("@RMPartCode", model.RMPartCode);
        //                oCmd.Parameters.AddWithValue("@RMItemName", model.RMItemName);
        //                oCmd.Parameters.AddWithValue("@BomNo", model.BomRevNo);
        //                oCmd.Parameters.AddWithValue("@DeltaE", 0);
        //                await myConnection.OpenAsync();
        //                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
        //                {
        //                    oDataAdapter.Fill(oDataTable);
        //                }

        //                if (oDataTable.Rows.Count > 0)
        //                {
        //                    model.DTDashboard = oDataTable;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }
        //    finally
        //    {
        //        oDataTable.Dispose();
        //    }
        //    return model;
        //}
        public async Task<ResponseResult> GetFGPartCodeList(string SearchFGPartCode, string CTRL)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SEARCHFGPARTCODELIST"));
                SqlParams.Add(new SqlParameter("@SearchFGPartCode", SearchFGPartCode ?? ""));
                SqlParams.Add(new SqlParameter("@CTRL", CTRL));
                Result = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetFGItemNameList(string SearchFGItemName, string CTRL)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SEARCHFGPARTCODELIST"));
                SqlParams.Add(new SqlParameter("@SearchFGItemName", SearchFGItemName ?? ""));
                SqlParams.Add(new SqlParameter("@CTRL", CTRL));
                Result = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetRMItemNameList(string SearchRMItemName, string CTRL)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SEARCHRMITEMNAMELIST"));
                SqlParams.Add(new SqlParameter("@SearchRMItemName", SearchRMItemName ?? ""));
                SqlParams.Add(new SqlParameter("@CTRL", CTRL));
                Result = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetRMPartCodeList(string SearchRMPartcode, string CTRL)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SEARCHRMPARTCODELIST"));
                SqlParams.Add(new SqlParameter("@CTRL", CTRL));
                SqlParams.Add(new SqlParameter("@SearchRMPartCode", SearchRMPartcode ?? ""));
                Result = await _IDataLogic.ExecuteDataTable("SP_Bom", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
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
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode == "U" ? "UPDATE" : "Insert");
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
                    oCmd.Parameters.AddWithValue("@Ldash", model.Ldash);
                    oCmd.Parameters.AddWithValue("@Adash", model.Adash);
                    oCmd.Parameters.AddWithValue("@Bdash", model.Bdash);
                    oCmd.Parameters.AddWithValue("@LValue", model.LValue);
                    oCmd.Parameters.AddWithValue("@AValue", model.AValue);
                    oCmd.Parameters.AddWithValue("@BValue", model.BValue);
                    oCmd.Parameters.AddWithValue("@DeltaE", model.DeltaE);
                    oCmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                    oCmd.Parameters.AddWithValue("@MaterialCost", model.MaterialCost);

                    oCmd.Parameters.AddWithValue("@EntryByMachineName", model.EntryByMachineName);
                    oCmd.Parameters.AddWithValue("@IPAddress", model.IPAddress);
                    if (model.Mode == "U")
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
                                //StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusCode = (HttpStatusCode)Convert.ToInt32(Reader["StatusCode"]),
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
        public async Task<ResponseResult> FillItems(string Flag, string SearchPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                if (Flag == "SEARCHFGPARTCODELIST")
                {
                    SqlParams.Add(new SqlParameter("@SearchFGPartCode", SearchPartCode));

                }
                else if (Flag == "SEARCHFGITEMCODELIST")
                {
                    SqlParams.Add(new SqlParameter("@SearchFGItemName", SearchPartCode));
                }
                else if (Flag == "SEARCHRMPARTCODELIST")
                {
                    SqlParams.Add(new SqlParameter("@SearchRMPartCode", SearchPartCode));

                }
                else if (Flag == "SEARCHRMITEMNAMELIST")
                {
                    SqlParams.Add(new SqlParameter("@SearchRMItemName", SearchPartCode));

                }
                SqlParams.Add(new SqlParameter("@CTRL", "T"));

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


        public async Task<ResponseResult> ChangeBomRMQtySameAsGrossWeight()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ChangeBomRMQtySameAsGrossWeight"));

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

        public async Task<ResponseResult> AllowChangeFGBomQty()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "AllowChangeFGBomQty"));

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

        public async Task<BomModel> GetItemData(string FGPartCode, string FGItemName, string RMPartCode, string RMItemName)
        {
            var resultList = new BomModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SP_Bom", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    ;

                    command.Parameters.AddWithValue("@Flag", "GetAllDataForUpdate");
                    command.Parameters.AddWithValue("@FGPartCode", FGPartCode);
                    command.Parameters.AddWithValue("@FGItemName", FGItemName);
                    command.Parameters.AddWithValue("@RMPartCode", RMPartCode);
                    command.Parameters.AddWithValue("@RMItemName", RMItemName);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.ALLDATAForUpdate = (from DataRow row in oDataSet.Tables[0].Rows
                                                   select new BomViewModel
                                                   {
                                                       FGItemCode = row["FGItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["FGItemCode"]),
                                                       RMItemCode = row["RMItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["RMItemCode"]),

                                                       FGPartCode = row["FGPartCode"] == DBNull.Value ? string.Empty : row["FGPartCode"].ToString(),
                                                       FGItemName = row["FGItemName"] == DBNull.Value ? string.Empty : row["FGItemName"].ToString(),
                                                       RMPartCode = row["RMPartCode"] == DBNull.Value ? string.Empty : row["RMPartCode"].ToString(),
                                                       RMItemName = row["RMItemName"] == DBNull.Value ? string.Empty : row["RMItemName"].ToString(),

                                                       RMQty = row["RMQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["RMQty"]),
                                                       BomNo = row["BomNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["BomNo"]),
                                                       Scrap = row["Scrap"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Scrap"]),
                                                       GrossWeight = row["GrossWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["GrossWeight"]),
                                                       NetWeight = row["NetWeight"] == DBNull.Value ? 0 : Convert.ToDecimal(row["NetWeight"]),
                                                       BurnQty = row["BurnQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["BurnQty"]),


                                                       Remark = row["Remark"] == DBNull.Value ? string.Empty : row["Remark"].ToString(),


                                                   }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data.", ex);
            }

            return resultList;
        }

        public async Task<ResponseResult> UpdateMultipleBOMData(DataTable ItemDetailGrid, int oldItemCode, string ApplyType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "UpdateMultipleBOMData"));
                SqlParams.Add(new SqlParameter("@DTBomModule", ItemDetailGrid));
                SqlParams.Add(new SqlParameter("@oldItemCode", oldItemCode));
                SqlParams.Add(new SqlParameter("@ApplyType", ApplyType));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_BOM", SqlParams);
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