using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
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
    public class WIPStockRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public WIPStockRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> FillItems( string SearchItemCode, string SearchPartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "fillitem"));
               
                SqlParams.Add(new SqlParameter("@ItemName", SearchItemCode ?? ""));
                SqlParams.Add(new SqlParameter("@PartCode", SearchPartCode ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPReportWIPstockRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<WIPStockRegisterModel> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int WCID, string ReportType, string BatchNo, string UniqueBatchNo, string WorkCenter)
        {
            DataSet? oDataSet = new DataSet();
            var model = new WIPStockRegisterModel();
            var _WIPstockDetail = new List<WIPStockRegisterDetail>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportSTockRegisterForAllStore", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType == "SHOWALLSTORESTOCK" || ReportType == "SHOWALLSTORE+WIPSTOCK" || ReportType == "SHOWALLSTORE+WIPSTOCK+JOBWORK")
                    {

                        oCmd = new SqlCommand("SPReportSTockRegisterForAllStore", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SPReportWIPstockRegister", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    if (ReportType != "WIPSTOCKDETAILFORTRANSFER")
                    { 
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    }
                    oCmd.Parameters.AddWithValue("@PartCode", string.IsNullOrEmpty(PartCode) ? DBNull.Value : (object)PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName","");
                    oCmd.Parameters.AddWithValue("@GroupName", string.IsNullOrEmpty(ItemGroup) ? DBNull.Value : (object)ItemGroup);
                    oCmd.Parameters.AddWithValue("@CatName", string.IsNullOrEmpty(ItemType) ? DBNull.Value : (object)ItemType);
                    oCmd.Parameters.AddWithValue("@WCId", WCID); // int — 0 is fine
                    oCmd.Parameters.AddWithValue("@BatchNo", string.IsNullOrEmpty(BatchNo) ? DBNull.Value : (object)BatchNo);
                    oCmd.Parameters.AddWithValue("@Uniquebatchno", string.IsNullOrEmpty(UniqueBatchNo) ? DBNull.Value : (object)UniqueBatchNo);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "WIPSTOCKDETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.WIPStockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new WIPStockRegisterDetail
                                                        {
                                                            SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
															//itemCode = Convert.ToInt32(dr["itemCode"].ToString()),
															//WCID = Convert.ToInt32(dr["WCID"].ToString()),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            TransactionType = string.IsNullOrEmpty(dr["TRansactionType"].ToString()) ? "" : dr["TRansactionType"].ToString(),
                                                            TransDate = string.IsNullOrEmpty(dr["TRansDAte"].ToString()) ? "" : dr["TRansDAte"].ToString(),
                                                            BillNo = string.IsNullOrEmpty(dr["TransactionNo"].ToString()) ? "" : dr["TransactionNo"].ToString(),
                                                            FromWc = string.IsNullOrEmpty(dr["FromWC"].ToString()) ? "" : dr["FromWC"].ToString(),
                                                            FromStore = string.IsNullOrEmpty(dr["FromStore"].ToString()) ? "" : dr["FromStore"].ToString(),
                                                            BillDate = string.IsNullOrEmpty(dr["SlipDate"].ToString()) ? "" : dr["SlipDate"].ToString(),
                                                            BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                            UniquebatchNo = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                            ToWC = string.IsNullOrEmpty(dr["ToWC"].ToString()) ? "" : dr["ToWC"].ToString(),
                                                            ToStore = string.IsNullOrEmpty(dr["ToStore"].ToString()) ? "" : dr["ToStore"].ToString(),
                                                            WorkCenterName = string.IsNullOrEmpty(dr["WorkCenterName"].ToString()) ? "" : dr["WorkCenterName"].ToString(),
                                                            OpnStk = string.IsNullOrEmpty(dr["OpnStk"].ToString()) ? 0 : Convert.ToDecimal(dr["OpnStk"].ToString()),
                                                            IssQty = string.IsNullOrEmpty(dr["IssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["IssQty"].ToString()),
                                                            RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"].ToString()),
                                                            TotStk = string.IsNullOrEmpty(dr["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                            AltIssQty = string.IsNullOrEmpty(dr["AltIssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltIssQty"].ToString()),
                                                            AltRecQty = string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                            AltStock = string.IsNullOrEmpty(dr["AltStock"].ToString()) ? 0 : Convert.ToDecimal(dr["AltStock"].ToString()),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                            AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                            //ItemType = string.IsNullOrEmpty(dr["ITEMTYPE"].ToString()) ? "" : dr["ITEMTYPE"].ToString(),
                                                            ItemGroup = string.IsNullOrEmpty(dr["itemgroup"].ToString()) ? "" : dr["itemgroup"].ToString(),
                                                            Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                            Amount = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()) * (string.IsNullOrEmpty(dr["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalStock"].ToString())),
                                                            EntryId = string.IsNullOrEmpty(dr["entryId"].ToString()) ? 0 : Convert.ToInt16(dr["entryId"].ToString()),

                                                        }).ToList();
                    }
                    
                }
                if (ReportType == "WIPSTOCKDETAILFORTRANSFER") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.WIPStockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new WIPStockRegisterDetail
                                                        {
                                                            //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                            itemCode = Convert.ToInt32(dr["itemCode"].ToString()),
                                                            WCID = Convert.ToInt32(dr["WCID"].ToString()),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                            UniquebatchNo = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                            WorkCenterName = string.IsNullOrEmpty(dr["WorkCenterName"].ToString()) ? "" : dr["WorkCenterName"].ToString(),
                                                            TotStk = string.IsNullOrEmpty(dr["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),

                                                        }).ToList();
                    }

                }
                else if (ReportType == "WIPSTOCKSUMMARY")//done(stocksummary,Zeroinventory,balance)
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.WIPStockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new WIPStockRegisterDetail
                                                        {
                                                            SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            WorkCenterName = string.IsNullOrEmpty(dr["WorkCenterName"].ToString()) ? "" : dr["WorkCenterName"].ToString(),
                                                            OpnStk = string.IsNullOrEmpty(dr["OpnStk"].ToString()) ? 0 : Convert.ToDecimal(dr["OpnStk"].ToString()),
                                                            IssQty = string.IsNullOrEmpty(dr["IssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["IssQty"].ToString()),
                                                            RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"].ToString()),
                                                            TotStk = string.IsNullOrEmpty(dr["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                            MinLevel = string.IsNullOrEmpty(dr["MInLvl"].ToString()) ? 0 : Convert.ToDecimal(dr["MInLvl"].ToString()),
                                                            MaximumLevel = string.IsNullOrEmpty(dr["MaximumLevel"].ToString()) ? 0 : Convert.ToDecimal(dr["MaximumLevel"].ToString()),
                                                            AltStock = string.IsNullOrEmpty(dr["AltStock"].ToString()) ? 0 : Convert.ToDecimal(dr["AltStock"].ToString()),
                                                            AvgRate = string.IsNullOrEmpty(dr["AvgRate"].ToString()) ? 0 : Convert.ToDecimal(dr["AvgRate"].ToString()),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                            AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                            //ItemType = string.IsNullOrEmpty(dr["ITEMTYPE"].ToString()) ? "" : dr["ITEMTYPE"].ToString(),
                                                            ItemGroup = string.IsNullOrEmpty(dr["itemgroup"].ToString()) ? "" : dr["itemgroup"].ToString(),
                                                            Rate = string.IsNullOrEmpty(dr["AvgRate"].ToString()) ? 0 : Convert.ToDecimal(dr["AvgRate"].ToString()),
                                                            Amount = string.IsNullOrEmpty(dr["AvgRate"].ToString()) ? 0 : Convert.ToDecimal(dr["AvgRate"].ToString()) * (string.IsNullOrEmpty(dr["TotalStock"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalStock"].ToString())),

                                                        }).ToList();
                    }
                       
                    
                }

                 else if (ReportType == "SHOWONLYRECDATA")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<WIPStockRegisterDetail>(row);
                            _WIPstockDetail.Add(poDetail);
                        }
                        model.WIPStockRegisterDetail = _WIPstockDetail;
                    }
                }
                else if (ReportType == "SHOWONLYISSUEDATA")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {   
                            var poDetail = CommonFunc.DataRowToClass<WIPStockRegisterDetail>(row);
                            _WIPstockDetail.Add(poDetail);
                        }
                        model.WIPStockRegisterDetail = _WIPstockDetail;
                    }
                }
                else if (ReportType == "BATCHWISESTOCKSUMMARY") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<WIPStockRegisterDetail>(row);
                            _WIPstockDetail.Add(poDetail);
                        }
                        model.WIPStockRegisterDetail = _WIPstockDetail;
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
        public async Task<ResponseResult> GetAllWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAllWorkCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportWIPstockRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllItemGroups()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GETITEMGROUP"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportWIPstockRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllItemTypes()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GETITEMTYPE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportWIPstockRegister", SqlParams);
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
