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
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class StockRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public StockRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<StockRegisterModel> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int StoreId, string ReportType, string BatchNo, string UniqueBatchNo)
        {
            DataSet? oDataSet = new DataSet();
            var model = new StockRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportSTockRegisterForAllStore", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout=240
                    };
                    if (ReportType == "SHOWALLSTORESTOCK" || ReportType == "SHOWALLSTORE+WIPSTOCK" || ReportType == "SHOWALLSTORE+WIPSTOCK+JOBWORK")
                    {

                        oCmd = new SqlCommand("SPReportSTockRegisterForAllStore", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 240
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SPReportSTockRegister", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 240
                        };
                    }
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@GroupName", ItemGroup);
                    oCmd.Parameters.AddWithValue("@CatName", ItemType);
                    oCmd.Parameters.AddWithValue("@Storeid", StoreId);
                    oCmd.Parameters.AddWithValue("@BatchNo", BatchNo);
                    oCmd.Parameters.AddWithValue("@Uniquebatchno", UniqueBatchNo);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "STOCKDETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         TransactionType = string.IsNullOrEmpty(dr["TRansactionType"].ToString()) ? "" : dr["TRansactionType"].ToString(),
                                                         TransDate = string.IsNullOrEmpty(dr["TRansDAte"].ToString()) ? "" : dr["TRansDAte"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         TransSlipNo = string.IsNullOrEmpty(dr["TrasMatSlipNo"].ToString()) ? "" : dr["TrasMatSlipNo"].ToString(),
                                                         OpnStk = Convert.ToDecimal(dr["OpnStk"].ToString()),
                                                         RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                         IssQty = Convert.ToDecimal(dr["IssQty"].ToString()),
                                                         TotStk = Convert.ToDecimal(dr["TotStk"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         MinLevel = Convert.ToDecimal(dr["Minimum_Level"]),
                                                         AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                         AltStock = Convert.ToInt32(dr["AltStock"]),
                                                         BillNo = string.IsNullOrEmpty(dr["BillNo"].ToString()) ? "" : dr["BillNo"].ToString().Split(" ")[0],
                                                         BillDate = string.IsNullOrEmpty(dr["BillDate"].ToString()) ? "" : dr["BillDate"].ToString(),
                                                         PartyName = string.IsNullOrEmpty(dr["PartyName"].ToString()) ? "" : dr["PartyName"].ToString(),
                                                         MRNNo = string.IsNullOrEmpty(dr["MRNNo"].ToString()) ? "" : dr["MRNNo"].ToString(),
                                                         Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amount = Convert.ToDecimal(dr["Rate"].ToString()) * Convert.ToDecimal(dr["TotStk"].ToString()),
                                                         BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                         UniquebatchNo = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                         EntryId = Convert.ToInt32(dr["entryid"].ToString()),
                                                         AltRecQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                         AltIssQty = Convert.ToDecimal(dr["AltIssQty"].ToString()),
                                                         GroupName = string.IsNullOrEmpty(dr["Group_name"].ToString()) ? "" : dr["Group_name"].ToString(),
                                                         SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                         //package = string.IsNullOrEmpty(dr["package"].ToString()) ? "" : dr["package"].ToString()
                                                     }).ToList();
                    }
                }
                else if (ReportType == "STOCKSUMMARY"|| ReportType== "STOCKWITHZEROINVENTORY")//done(stocksummary,Zeroinventory,balance)
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         OpnStk = Convert.ToDecimal(dr["OpnStk"].ToString()),
                                                         RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                         IssQty = Convert.ToDecimal(dr["IssQty"].ToString()),
                                                         TotStk = Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                         StdPacking = Convert.ToDecimal(dr["StdPacking"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         MinLevel = Convert.ToDecimal(dr["MInLvl"]),
                                                         AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                         AltStock = Convert.ToInt32(dr["AltStock"]),
                                                         AvgRate = Convert.ToDecimal(dr["AvgRate"]),
                                                         Amount = Convert.ToDecimal(dr["AvgRate"]) * Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                         GroupName = string.IsNullOrEmpty(dr["Group_name"].ToString()) ? "" : dr["Group_name"].ToString(),
                                                         BinNo = string.IsNullOrEmpty(dr["BinNo"].ToString()) ? "" : dr["BinNo"].ToString(),
                                                         MaximumLevel = Convert.ToDecimal(dr["MaximumLevel"]),
                                                         ReorderLevel = Convert.ToDecimal(dr["ReorderLevel"]),
                                                         SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "SHOWALLSTORESTOCK")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         Total = string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                         Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amount = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()) * (string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString())),
                                                         ItemType = string.IsNullOrEmpty(dr["ITEMTYPE"].ToString()) ? "" : dr["ITEMTYPE"].ToString(),
                                                         ItemGroup = string.IsNullOrEmpty(dr["itemgroup"].ToString()) ? "" : dr["itemgroup"].ToString(),
                                                         MAINSTORE = string.IsNullOrEmpty(dr["MAIN STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["MAIN STORE"].ToString()),
                                                         //REJSTORE = string.IsNullOrEmpty(dr["REJ STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["REJ STORE"].ToString()),
                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "SHOWONLYRECDATA")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         TransactionType = string.IsNullOrEmpty(dr["TRansactionType"].ToString()) ? "" : dr["TRansactionType"].ToString(),
                                                         TransDate = string.IsNullOrEmpty(dr["TRansDAte"].ToString()) ? "" : dr["TRansDAte"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         BillNo = string.IsNullOrEmpty(dr["BillNo"].ToString()) ? "" : dr["BillNo"].ToString().Split(" ")[0],
                                                         BillDate = string.IsNullOrEmpty(dr["BillDate"].ToString()) ? "" : dr["BillDate"].ToString(),
                                                         PartyName = string.IsNullOrEmpty(dr["PartyName"].ToString()) ? "" : dr["PartyName"].ToString(),
                                                         MRNNo = string.IsNullOrEmpty(dr["MRNNo"].ToString()) ? "" : dr["MRNNo"].ToString(),
                                                         Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amount = Convert.ToDecimal(dr["Rate"].ToString()) * Convert.ToDecimal(dr["RecQty"].ToString()),
                                                         BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                         UniquebatchNo = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                         EntryId = Convert.ToInt32(dr["entryid"].ToString()),
                                                         AltRecQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                         GroupName = string.IsNullOrEmpty(dr["Group_name"].ToString()) ? "" : dr["Group_name"].ToString(),
                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "SHOWONLYISSUEDATA")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         TransactionType = string.IsNullOrEmpty(dr["TRansactionType"].ToString()) ? "" : dr["TRansactionType"].ToString(),
                                                         TransDate = string.IsNullOrEmpty(dr["TRansDAte"].ToString()) ? "" : dr["TRansDAte"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         IssQty = string.IsNullOrEmpty(dr["IssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["IssQty"]),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         MinLevel = string.IsNullOrEmpty(dr["Minimum_Level"].ToString()) ? 0 : Convert.ToDecimal(dr["Minimum_Level"]),
                                                         AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                         BillNo = string.IsNullOrEmpty(dr["BillNo"].ToString()) ? "" : dr["BillNo"].ToString().Split(" ")[0],
                                                         BillDate = string.IsNullOrEmpty(dr["BillDate"].ToString()) ? "" : dr["BillDate"].ToString(),
                                                         PartyName = string.IsNullOrEmpty(dr["PartyName"].ToString()) ? "" : dr["PartyName"].ToString(),
                                                         MRNNo = string.IsNullOrEmpty(dr["MRNNo"].ToString()) ? "" : dr["MRNNo"].ToString(),
                                                         Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"]) * (string.IsNullOrEmpty(dr["IssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["IssQty"])),
                                                         BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                         UniquebatchNo = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                         EntryId = string.IsNullOrEmpty(dr["entryid"].ToString()) ? 0 : Convert.ToInt32(dr["entryid"]),
                                                         AltRecQty = string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRecQty"]),
                                                         AltIssQty = string.IsNullOrEmpty(dr["AltIssQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltIssQty"]),
                                                         GroupName = string.IsNullOrEmpty(dr["Group_name"].ToString()) ? "" : dr["Group_name"].ToString(),
                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "BATCHWISESTOCKSUMMARY") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         OpnStk = Convert.ToDecimal(dr["OpnStk"].ToString()),
                                                         RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                         IssQty = Convert.ToDecimal(dr["IssQty"].ToString()),
                                                         TotStk = Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                         BatchStock = Convert.ToDecimal(dr["BatchStock"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         MinLevel = Convert.ToDecimal(dr["MInLvl"]),
                                                         AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                         AltStock = Convert.ToInt32(dr["AltStock"]),
                                                         AvgRate = Convert.ToInt32(dr["AvgRate"]),
                                                         Amount = Convert.ToInt32(dr["AvgRate"]) * Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                         SeqNum = Convert.ToInt32(dr["seqnum"]),
                                                         GroupName = string.IsNullOrEmpty(dr["Group_name"].ToString()) ? "" : dr["Group_name"].ToString(),
                                                         StdPacking = Convert.ToDecimal(dr["StdPacking"]),
                                                         MaximumLevel = Convert.ToDecimal(dr["MaximumLevel"]),
                                                         ReorderLevel = Convert.ToDecimal(dr["ReorderLevel"]),
                                                         BinNo = string.IsNullOrEmpty(dr["BinNo"].ToString()) ? "" : dr["BinNo"].ToString(),
                                                         FromDate = string.IsNullOrEmpty(dr["FromDate"].ToString()) ? "" : dr["FromDate"].ToString(),
                                                         ToDate = string.IsNullOrEmpty(dr["ToDate"].ToString()) ? "" : dr["ToDate"].ToString(),
                                                         PartyName = string.IsNullOrEmpty(dr["CompanyName"].ToString()) ? "" : dr["CompanyName"].ToString(),
                                                         Address = string.IsNullOrEmpty(dr["Address"].ToString()) ? "" : dr["Address"].ToString(),
                                                         Address2 = string.IsNullOrEmpty(dr["Address2"].ToString()) ? "" : dr["Address2"].ToString(),
                                                         BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                         UniquebatchNo = string.IsNullOrEmpty(dr["uniquebatchno"].ToString()) ? "" : dr["uniquebatchno"].ToString(),
                                                         SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "SHOWALLSTORE+WIPSTOCK")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         Total = string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                         Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amount = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()) * (string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString())),
                                                         ItemType = string.IsNullOrEmpty(dr["ITEMTYPE"].ToString()) ? "" : dr["ITEMTYPE"].ToString(),
                                                         ItemGroup = string.IsNullOrEmpty(dr["itemgroup"].ToString()) ? "" : dr["itemgroup"].ToString(),
                                                         MAINSTORE = string.IsNullOrEmpty(dr["MAIN STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["MAIN STORE"].ToString()),
                                                         //REJSTORE = string.IsNullOrEmpty(dr["REJ STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["REJ STORE"].ToString()),
                                                         QCSTORE = string.IsNullOrEmpty(dr["QC STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["QC STORE"].ToString()),
                                                         Assembly = string.IsNullOrEmpty(dr["Assembly"].ToString()) ? 0 : Convert.ToDecimal(dr["Assembly"].ToString()),
                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "SHOWALLSTORE+WIPSTOCK+JOBWORK")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         Total = string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                         Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amount = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()) * (string.IsNullOrEmpty(dr["Total"].ToString()) ? 0 : Convert.ToDecimal(dr["Total"].ToString())),
                                                         ItemType = string.IsNullOrEmpty(dr["ITEMTYPE"].ToString()) ? "" : dr["ITEMTYPE"].ToString(),
                                                         ItemGroup = string.IsNullOrEmpty(dr["itemgroup"].ToString()) ? "" : dr["itemgroup"].ToString(),
                                                         MAINSTORE = string.IsNullOrEmpty(dr["MAIN STORE"].ToString()) ? 0 : Convert.ToDecimal(dr["MAIN STORE"].ToString()),
                                                         Assembly = string.IsNullOrEmpty(dr["Assembly"].ToString()) ? 0 : Convert.ToDecimal(dr["Assembly"].ToString()),
                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType == "NegativeStock")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.StockRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new StockRegisterDetail
                                                     {
                                                         StoreName = string.IsNullOrEmpty(dr["StoreName"].ToString()) ? "" : dr["StoreName"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         UniquebatchNo = string.IsNullOrEmpty(dr["UniqueBatchNo"].ToString()) ? "" : dr["UniqueBatchNo"].ToString(),
                                                         BatchNo = string.IsNullOrEmpty(dr["BatchNo"].ToString()) ? "" : dr["BatchNo"].ToString(),
                                                         ItemCode = string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["ItemCode"].ToString()),

                                                         StoreId = string.IsNullOrEmpty(dr["StoreId"].ToString()) ? 0 : Convert.ToInt32(dr["StoreId"].ToString()),
                                                         TotStk = string.IsNullOrEmpty(dr["Stock"].ToString()) ? 0 : Convert.ToDecimal(dr["Stock"].ToString()),


                                                         //SeqNo = Convert.ToInt32(dr["seqnum"].ToString()),
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
        public async Task<ResponseResult> FillItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GETALLITEMS"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSTockRegister", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSTockRegister", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSTockRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllStores()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GETALLSTORE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSTockRegister", SqlParams);
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
