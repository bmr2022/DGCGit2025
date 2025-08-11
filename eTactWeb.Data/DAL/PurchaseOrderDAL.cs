using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Common = eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL;

public class PurchaseOrderDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private readonly ConnectionStringService _connectionStringService;

    public PurchaseOrderDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        _IDataLogic = iDataLogic;
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
    }
    public async Task<string> GetItemServiceFORPO(string ItemService)
    {
        var JsonString = string.Empty;
        // throw new NotImplementedException();
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GetItemServiceFORPO"));
            //SqlParams.Add(new SqlParameter("@POTypeServItem", ItemSErv));
            SqlParams.Add(new SqlParameter("@POTypeServItem", ItemService));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
            JsonString = JsonConvert.SerializeObject(_ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return JsonString;
    }

    public async Task<ResponseResult> GetFormRights(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "PO"));

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

    public async Task<ResponseResult> GetFormRightsAmm(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Purchase Order Amendment"));

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

    public async Task<ResponseResult> GetPendQty(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@POno", PONo));
            SqlParams.Add(new SqlParameter("@POYearcode", POYearCode));
            SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SchNo", SchNo));
            SqlParams.Add(new SqlParameter("@SchYearCode", SchYearCode));
            SqlParams.Add(new SqlParameter("@Flag", Flag));
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SPGetPendPOQty", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<ResponseResult> PoallowtoprintWithoutApproval()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PoallowtoprintWithoutApproval"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }


    public async Task<ResponseResult> FillEntryandPONumber(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<ResponseResult> getOldRate(int EntryId, int YearCode, int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "getOldRate"));
            SqlParams.Add(new SqlParameter("@EntryId", EntryId));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@SearchItemCode", ItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillIndentDetail(string itemName, string partCode, int itemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillIndentDetail"));
            SqlParams.Add(new SqlParameter("@itemName", itemName));
            SqlParams.Add(new SqlParameter("@partcode", partCode));
            SqlParams.Add(new SqlParameter("@ItemCode", itemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPendingIndentAgainstPurchaseorder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillItems(string Type, string ShowAllItem, string SearchItemCode,string SearchPartCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetItemsOnAssets"));
            SqlParams.Add(new SqlParameter("@AType", Type));
            SqlParams.Add(new SqlParameter("@showAllItem", ShowAllItem));
            SqlParams.Add(new SqlParameter("@SearchItemCode", SearchItemCode ?? ""));
            SqlParams.Add(new SqlParameter("@SearchPartCode", SearchPartCode ?? ""));
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckLockYear(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "CheckLockYear"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@Module", "purchase"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillCurrency(string Ctrl)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLCURRENCY"));
            SqlParams.Add(new SqlParameter("@Ctrl", Ctrl));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GetDropDownList", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillPONumber(int YearCode, string Ordertype, string POdate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime podt = DateTime.ParseExact(POdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            SqlParams.Add(new SqlParameter("@Flag", "NewPO"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@orderType", Ordertype));
            SqlParams.Add(new SqlParameter("@PODate", podt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
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
    public async Task<ResponseResult> GetQuotData(string Flag, string QuotNo)
    {
        var Result = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@QuotNo", QuotNo));

            Result = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return Result;
    }

    internal async Task<ResponseResult> DeleteByID(int ID, int YearCode,int createdBy,string entryByMachineName, string Flag)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@CreatedBy", createdBy));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", entryByMachineName));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetExchangeRate(string Currency)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Date", DateTime.Today));
            SqlParams.Add(new SqlParameter("@Currency", Currency));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("getExchangeRate", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetGstRegister(string Flag, int Code)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@AccountCode", Code));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> NewAmmEntryId(int PoAmendYearCode)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GetNewAmmEntry"));
            SqlParams.Add(new SqlParameter("@POAmendYearcode", PoAmendYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillVendors()
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "BindVendoreName"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetAllPartyName(string CTRL)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "CREDITORDEBTORLIST"));
            SqlParams.Add(new SqlParameter("@CTRL", CTRL));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GetDropDownList", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    internal async Task<PODashBoard> GetDashBoardData()
    {
        var DashBoardData = new PODashBoard();
        var SqlParams = new List<dynamic>();

        try
        {
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var firstDt = CommonFunc.ParseFormattedDate(firstDayOfMonth.ToString("dd/MM/yyyy"));
            var endDt = CommonFunc.ParseFormattedDate(now.ToString("dd/MM/yyyy"));
            SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
            SqlParams.Add(new SqlParameter("@StartDate", firstDt));
            SqlParams.Add(new SqlParameter("@EndDate", endDt));
            var Result = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            if (Result?.Result is DataTable dt && dt.Rows.Count > 0)
            {
                var oDT = dt.DefaultView.ToTable(true, "EntryID", "AmmNo", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                    "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                    "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm", "EnteredBy", "UpdatedByName");

                oDT.TableName = "PODASHBOARD";
                DashBoardData.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return DashBoardData;
    }

    internal async Task<object?> GetMrpYear(string Flag, string MRPNo)
    {
        object Result = null;

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@MRPNo", MRPNo));

            Result = await _IDataLogic.ExecuteScalar("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return Result;
    }

    public async Task<PODashBoard> GetSearchData(PODashBoard model)
    {
        try
        {
            var SqlParams = new List<dynamic>();
            string flag = "";
            if (model.AmmType == "newAmm")
            {
                flag = "PURCHAMMDASHBOARD";
            }
            else if (model.AmmType == "updAmm")
            {
                flag = "UPDPOAMMDASHBOARD";
            }

            else
            {
                flag = "SEARCH";
            }
            SqlParams.Add(new SqlParameter("@Flag", flag));
            SqlParams.Add(new SqlParameter("@OrderType", model.OrderType));
            SqlParams.Add(new SqlParameter("@POType", model.POType));
            SqlParams.Add(new SqlParameter("@POFor", model.POFor));
            SqlParams.Add(new SqlParameter("@PONo", model.PONo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
            SqlParams.Add(new SqlParameter("@Branch", model.CC));
            string format = "yyyy-MM-dd HH:mm:ss.fff";
            //DateTime StartDate = new DateTime();
            //DateTime EndDate = new DateTime();
            //StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
           var StartDate = CommonFunc.ParseFormattedDate(model.FromDate);
           var EndDate = CommonFunc.ParseFormattedDate(model.ToDate);
            
            SqlParams.Add(new SqlParameter("@StartDate", StartDate));
            SqlParams.Add(new SqlParameter("@EndDate", EndDate));
            //SqlParams.Add(new SqlParameter("@StartDate", Convert.ToDateTime(model.FromDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));
            //SqlParams.Add(new SqlParameter("@StartDate", "2024-11-08 00:00:00.000"));
            //SqlParams.Add(new SqlParameter("@EndDate", Convert.ToDateTime(model.ToDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)));
            //SqlParams.Add(new SqlParameter("@EndDate", "2024-11-08 00:00:00.000"));
            SqlParams.Add(new SqlParameter("@VendorName", model.VendorName));

            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            if (((DataTable)ResponseResult.Result).Rows.Count > 0)
            {

                var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "PONo", "POType", "POFor", "WEF", "FOC", "Currency",
                    "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "YearCode", "Approved",
                  "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt",
                  "Approval1Levelapproved", "ApproveAmm", "AmmNo", "EnteredBy", "UpdatedByName", "DeliveryTerms", "ApproveddByEmp", "ApproveddByLevel1Emp",
                  "EntryByMachineName","POComplete");
                oDT.TableName = "PODASHBOARD";


                model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);

                //if (model.SummaryDetail == "Summary")
                //{
                //    var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                //        "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                //      "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm", "AmmNo", "EnteredBy", "UpdatedByName");
                //    oDT.TableName = "PODASHBOARDMain";
                //    model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
                //}
                //else
                //{
                //    var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "AmmNo", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                //         "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                //          "SeqNo", "ItemCode", "HSNNo", "POQty", "Unit", "AltPOQty", "AltUnit", "TolLimitPercentage", "TolLimitQty", "UnitRate", "Rate", "RateInOtherCurr",
                //         "DiscPer", "DiscRs", "Amount", "AdditionalRate", "OldRate", "Remark", "Description", "PendQty", "PendAltQty", "Process", "PkgStd", "AmmendmentNo",
                //         "AmmendmentDate", "AmmendmentReason", "FirstMonthTentQty"
                //         , "SecMonthTentQty", "SizeDetail", "Colour", "CostCenter", "Active", "RateApplicableOnUnit",
                //       "CreatedBy", "UpdatedBy", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm");
                //    oDT.TableName = "PODASHBOARD";
                //    model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
                //}

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
    public async Task<PODashBoard> GetDetailData(PODashBoard model)
    {
        DataSet? oDataSet = new DataSet();
        var model1 = new PODashBoard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_PurchaseOrder", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD");
                oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                oCmd.Parameters.AddWithValue("@VendorName", model.VendorName);
                oCmd.Parameters.AddWithValue("@POType", model.POType);
                oCmd.Parameters.AddWithValue("@POFor", model.POFor);
                oCmd.Parameters.AddWithValue("@PartCode", model.PartCode);
                oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                oCmd.Parameters.AddWithValue("@PONo", model.PONo);
                oCmd.Parameters.AddWithValue("@branch", model.CC);
                oCmd.Parameters.AddWithValue("@StartDate", CommonFunc.ParseFormattedDate(model.FromDate));
                oCmd.Parameters.AddWithValue("@EndDate", CommonFunc.ParseFormattedDate(model.ToDate));


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
                model1.PODashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                      select new PODashBoard
                                      {
                                          PONo = dr["PONo"].ToString(),
                                          EnteredBy = dr["EnteredBy"].ToString(),
                                          UpdatedByName = dr["UpdatedByName"].ToString(),
                                          PODate = dr["PODate"].ToString(),
                                          WEF = dr["WEF"].ToString(),
                                          POCloseDate = dr["POCloseDate"].ToString(),
                                          OrderType = dr["OrderType"].ToString(),
                                          POType = dr["POType"].ToString(),
                                          POFor = dr["POFor"].ToString(),
                                          QuotNo = Convert.ToInt32(dr["QuotNo"].ToString()),
                                          QuotYear = Convert.ToInt32(dr["QuotYear"].ToString()),
                                          BasicAmount = Convert.ToSingle(dr["BasicAmount"].ToString()),
                                          NetAmount = Convert.ToSingle(dr["NetAmount"].ToString()),
                                          Approved = dr["Approved"].ToString(),
                                          ApproveAmm = dr["ApproveAmm"].ToString(),
                                          Approval1Levelapproved = dr["Approval1Levelapproved"].ToString(),
                                          VendorAddress = dr["VendorAddress"].ToString(),
                                          HsnNo = dr["HSNNo"].ToString(),
                                          POQty = dr["POQty"].ToString(),
                                          Unit = dr["Unit"].ToString(),
                                          AltPOQty = dr["AltPOQty"].ToString(),
                                          AltUnit = dr["AltUnit"].ToString(),
                                          Rate = dr["Rate"].ToString(),
                                          RateInOtherCurr = dr["RateInOtherCurr"].ToString(),
                                          DiscPer = dr["DiscPer"].ToString(),
                                          DiscRs = dr["DiscRs"].ToString(),
                                          Amount = dr["Amount"].ToString(),
                                          PendQty = Convert.ToDecimal(dr["PendQty"]),
                                          PendAltQty = Convert.ToDecimal(dr["PendAltQty"]),
                                          VendorName = dr["VendorName"].ToString(),
                                          PartCode = dr["PartCode"].ToString(),
                                          UnitRate = dr["UnitRate"].ToString(),
                                          ItemName = dr["ItemName"].ToString(),
                                          EntryID = Convert.ToInt16(dr["Entryid"]),
                                          YearCode = Convert.ToInt16(dr["YearCode"]),

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
        return model1;
    }
    public async Task<PODashBoard> GetSearchCompData(PODashBoard model)
    {
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "POAMMCOMPLETED"));
            SqlParams.Add(new SqlParameter("@SummaryDetail", model.SummaryDetail));
            SqlParams.Add(new SqlParameter("@OrderType", model.OrderType));
            SqlParams.Add(new SqlParameter("@POType", model.POType));
            SqlParams.Add(new SqlParameter("@POFor", model.POFor));
            SqlParams.Add(new SqlParameter("@PONo", model.PONo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
            SqlParams.Add(new SqlParameter("@Branch", model.CC));
            SqlParams.Add(new SqlParameter("@StartDate", CommonFunc.ParseFormattedDate(model.FromDate)));
            SqlParams.Add(new SqlParameter("@EndDate", CommonFunc.ParseFormattedDate(model.ToDate)));
            SqlParams.Add(new SqlParameter("@VendorName", model.VendorName));

            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
            if (ResponseResult != null && ResponseResult.Result != null)
            {
                var dataTable = ResponseResult.Result as DataTable;
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    if (model.SummaryDetail == "Detail")
                    {
                        var oDT = ResponseResult.Result.DefaultView.ToTable(true, "VendorName", "VendorAddress", "EntryDate", "PONo", "OrderNo", "AmmNo", "AmmEffDate", "PODate",
                        "WEF", "POCloseDate", "OrderType", "POType", "POFor", "POTypeServItem", "Currency", "ItemName", "rate", "OldRate",
                         "POQty", "PendQty", "unit", "DiscPer", "DiscRs", "Amount", "AltPOQty", "PendAltQty", "AltUnit", "AdditionalRate", "HSNNo", "TolLimitQty",
                         "TolLimitPercentage", "Description", "orderamt", "OrderNetAmt", "ApproveddByEmp", "ApprovedDate", "ActualEntryBY", "ApproveAmm", "EntryID", "YearCode",
                         "Branch", "DeliveryTerms", "PaymentTerms", "UpdatedBy", "UpdatedOn","POComplete");

                        //var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "AmmNo", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                        //"POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                        // "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm");

                        oDT.TableName = "POCompletedDetail";
                        model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);

                    }
                    else
                    {
                        var oDT = ResponseResult.Result.DefaultView.ToTable(true, "VendorName", "VendorAddress", "EntryDate", "PONo", "OrderNo", "AmmNo", "AmmEffDate", "PODate",
                            "WEF", "POCloseDate", "OrderType", "POType", "POFor", "POTypeServItem", "Currency", "orderamt", "OrderNetAmt",
                             "ApproveddByEmp", "ApprovedDate", "ActualEntryBY", "ApproveAmm", "EntryID", "YearCode",
                             "Branch", "DeliveryTerms", "PaymentTerms", "UpdatedBy", "UpdatedOn");

                        oDT.TableName = "POCompletedSummary";
                        model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
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

        return model;
    }

    public async Task<PurchaseOrderModel> GetViewByID(int ID, int YC, string Flag)
    {
        var oDataSet = new DataSet();
        var MainModel = new PurchaseOrderModel();
        var _ItemList = new List<POItemDetail>();
        var _TaxList = new List<TaxModel>();
        var _PendingIndentDetail = new List<PendingIndentDetailModel>();
        var SqlParams = new List<dynamic>();
        var listObject = new List<DeliverySchedule>();

        try
        {
            SqlParams.Add(new SqlParameter("@Flag", "ViewByID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));

            var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);

            if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
            {
                oDataSet = ResponseResult.Result;
                oDataSet.Tables[0].TableName = "POMain";
                oDataSet.Tables[1].TableName = "ItemDetailGrid";
                oDataSet.Tables[2].TableName = "POTaxDetail";
                oDataSet.Tables[3].TableName = "PODeliverySchedule";
                oDataSet.Tables[4].TableName = "POPendingIndent";

                if (oDataSet.Tables[3] != null && oDataSet.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[3].Rows)
                    {
                        listObject.Add(new DeliverySchedule
                        {
                            AltQty = Convert.ToInt32(row["AltQty"]),
                            Qty = Convert.ToInt32(row["Qty"]),
                            DPartCode = Convert.ToInt32(row["ItemCode"]),
                            Date = string.IsNullOrEmpty(row["DeliveryDate"].ToString()) ? "" : row["DeliveryDate"].ToString(),
                            Remarks = row["Remark"].ToString(),
                            Days = Convert.ToInt32(row["Days"]),
                        });
                    }
                    MainModel.DeliveryScheduleList = listObject;
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    MainModel.EntryID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["POEntryID"]);
                    MainModel.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["POYearCode"]);
                    MainModel.EntryDate = oDataSet.Tables[0].Rows[0]["EntryDate"].ToString();
                    //MainModel.EntryDate = CommonFunc.FormatToDDMMYYYY(oDataSet.Tables[0].Rows[0]["EntryDate"].ToString());
                    MainModel.Branch = oDataSet.Tables[0].Rows[0]["CC"].ToString();
                    MainModel.POFor = oDataSet.Tables[0].Rows[0]["POFor"].ToString();
                    MainModel.POPrefix = oDataSet.Tables[0].Rows[0]["POPrefix"].ToString();
                    MainModel.POType = oDataSet.Tables[0].Rows[0]["POType"].ToString();
                    MainModel.Currency = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CurrencyID"]);
                    MainModel.AccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AccountCode"]);
                    MainModel.VendorAddress = oDataSet.Tables[0].Rows[0]["VendorAddress"].ToString();
                    MainModel.ShippingAddress = oDataSet.Tables[0].Rows[0]["ShippingAddress"].ToString();
                    MainModel.OrderType = oDataSet.Tables[0].Rows[0]["OrderType"].ToString();
                    MainModel.OrderNo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["OrderNo"]);
                    MainModel.PONo = oDataSet.Tables[0].Rows[0]["PONo"].ToString();
                    MainModel.PODate = oDataSet.Tables[0].Rows[0]["PODate"].ToString();
                    //MainModel.PODate = CommonFunc.FormatToDDMMYYYY(oDataSet.Tables[0].Rows[0]["PODate"].ToString());
                    MainModel.WEF = oDataSet.Tables[0].Rows[0]["WEF"].ToString();
                    MainModel.POCloseDate = oDataSet.Tables[0].Rows[0]["POCloseDate"].ToString();
                    MainModel.QuotNo = oDataSet.Tables[0].Rows[0]["QuotNo"].ToString();
                    MainModel.QuotYear = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["QuotYear"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["QuotYear"]);
                    MainModel.QuotDate = oDataSet.Tables[0].Rows[0]["QuotDate"].ToString();
                    MainModel.MRPNo = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["MRPNo"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRPNo"]);
                    MainModel.MRPYearCode = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["MRPYearCode"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRPYearCode"]);
                    MainModel.RefNo = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["RefNo"].ToString()) ? "" : oDataSet.Tables[0].Rows[0]["RefNo"].ToString();
                    MainModel.ShipTo = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ShipTo"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ShipTo"]);
                    MainModel.AmmDate = oDataSet.Tables[0].Rows[0]["AmmEffDate"].ToString();
                    MainModel.FreightPaidBy = oDataSet.Tables[0].Rows[0]["FreightPaidBy"].ToString();
                    MainModel.DeliverySch = oDataSet.Tables[0].Rows[0]["DeliverySch"].ToString();
                    MainModel.DeliveryTerms = oDataSet.Tables[0].Rows[0]["DeliveryTerms"].ToString();
                    MainModel.PreparedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["PreparedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PreparedBy"]);
                    MainModel.PORemark = oDataSet.Tables[0].Rows[0]["Remark"].ToString();
                    MainModel.ItemNetAmount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["BasicAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["BasicAmount"]);
                    MainModel.NetTotal = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["NetAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["NetAmount"]);
                    MainModel.PackingChg = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["PackingChg"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PackingChg"]);
                    MainModel.PackingType = oDataSet.Tables[0].Rows[0]["PackingType"].ToString();
                    MainModel.PaymentDays = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["PaymentDays"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PaymentDays"]);
                    MainModel.PaymentTerms = oDataSet.Tables[0].Rows[0]["PaymentTerms"].ToString();
                    MainModel.ShipmentTerm = oDataSet.Tables[0].Rows[0]["ShipmentTerm"].ToString();
                    MainModel.Clause = oDataSet.Tables[0].Rows[0]["Clause"].ToString();
                    MainModel.Warrenty = oDataSet.Tables[0].Rows[0]["Warrenty"].ToString();
                    MainModel.Inspection = oDataSet.Tables[0].Rows[0]["Inspection"].ToString(); 
                    MainModel.Installation = oDataSet.Tables[0].Rows[0]["Installation"].ToString();
                    MainModel.PriceBasis = oDataSet.Tables[0].Rows[0]["PriceBasis"].ToString();
                    MainModel.ModeOFTransport = oDataSet.Tables[0].Rows[0]["ModeOFTransport"].ToString();
                    MainModel.InsurancePaidBy = oDataSet.Tables[0].Rows[0]["InsurancePaidBy"].ToString();
                    MainModel.LDClause = oDataSet.Tables[0].Rows[0]["LDClause"].ToString();
                    MainModel.ItemsForDepartment = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ItemsForDepartmentID"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ItemsForDepartmentID"]);
                    MainModel.ResposibleEmplforQC = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ResposibleEmplforQC"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ResposibleEmplforQC"]);
                    MainModel.PathOfFile1URL = oDataSet.Tables[0].Rows[0]["PathOfFile1"].ToString();
                    MainModel.PathOfFile2URL = oDataSet.Tables[0].Rows[0]["PathOfFile2"].ToString();
                    MainModel.PathOfFile3URL = oDataSet.Tables[0].Rows[0]["PathOfFile3"].ToString();
                    MainModel.TotalDiscountPercentage = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["TotalDiscountPercent"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["TotalDiscountPercent"]);
                    MainModel.TotalAmtAftrDiscount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"]);
                    MainModel.TotalRoundOff = oDataSet.Tables[0].Rows[0]["TotalRoundOff"].ToString();
                    MainModel.CreatedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CreatedBy"].ToString());
                    MainModel.CretaedByName = oDataSet.Tables[0].Rows[0]["UserName"].ToString();
                    if (Flag == "POA")
                    {
                        MainModel.AmmNo = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["POAmmNo"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["POAmmNo"].ToString());
                        MainModel.Approved = oDataSet.Tables[0].Rows[0]["Approved"].ToString();
                        MainModel.ApprovedDate = oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString();
                        MainModel.Approvedby = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        MainModel.AmmApproved = oDataSet.Tables[0].Rows[0]["Approved"].ToString();
                        MainModel.AmmApprovedDate = oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString();
                        MainModel.AmmApprovedby = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        MainModel.FirstLvlApproved = oDataSet.Tables[0].Rows[0]["Approved"].ToString();
                        MainModel.FirstLvlApprovedDate = oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString();
                        MainModel.FirstLvlApprovedby = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    }
                    else
                    {
                        MainModel.AmmNo = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["AmmNo"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AmmNo"].ToString());
                    }
                    MainModel.CreatedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CreatedBy"]);
                    MainModel.CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["CreatedOn"]);
                    if (!string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["UpdatedBy"].ToString()))
                    {
                        MainModel.UpdatedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["UpdatedBy"]);
                        MainModel.UpdatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["UpdatedOn"]);
                    }
                }
                int cnt = 1;
                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[1].Rows)
                    {
                        _ItemList.Add(new POItemDetail
                        {
                            SeqNo = cnt++,
                            AltPOQty = Convert.ToDecimal(row["AltPOQty"]),
                            AltUnit = row["AltUnit"].ToString(),
                            AmendmentDate = string.IsNullOrEmpty(row["AmmendmentDate"].ToString()) ? "" : row["AmmendmentDate"].ToString(),
                            AmendmentNo = Convert.ToInt32(row["AmmendmentNo"]),
                            AmendmentReason = string.IsNullOrEmpty(row["AmmendmentReason"].ToString()) ? "" : row["AmmendmentReason"].ToString(),
                            Amount = Convert.ToDecimal(row["Amount"]),
                            Color = row["Colour"].ToString(),
                            Description = row["Description"].ToString(),
                            DiscPer = Convert.ToDecimal(row["DiscPer"]),
                            DiscRs = Convert.ToDecimal(row["DiscRs"]),
                            HSNNo = Convert.ToInt32(row["HSNNo"]),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            ItemText = row["ItemText"].ToString(),
                            OtherRateCurr = Convert.ToInt32(row["RateInOtherCurr"]),
                            AdditionalRate = Convert.ToInt32(row["RateInOtherCurr"]),
                            PartCode = Convert.ToInt32(row["ItemCode"]),
                            PartText = row["PartText"].ToString(),
                            POQty = Convert.ToDecimal(row["POQty"]),
                            Rate = Convert.ToDecimal(row["Rate"]),
                            OldRate = Convert.ToDecimal(row["OldRate"]),
                            Unit = row["Unit"].ToString(),
                            UnitRate = row["UnitRate"].ToString(),
                            PIRemark = row["Remark"].ToString(),
                            DeliveryDate= string.IsNullOrEmpty(row["DeliveryDate"].ToString()) ? "" : row["DeliveryDate"].ToString(),
                            DeliveryScheduleList = listObject.Where(x => x.DPartCode == Convert.ToInt32(row["ItemCode"])).ToList(),
                        });
                    }

                    MainModel.ItemDetailGrid = _ItemList;
                    MainModel.ItemNetAmount = decimal.Parse(MainModel.ItemDetailGrid.Sum(x => x.Amount).ToString("#.#0"));
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[2].Rows)
                    {
                        _TaxList.Add(new TaxModel
                        {
                            TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                            TxType = row["Type"].ToString(),
                            TxPartCode = Convert.ToInt32(row["ItemCode"]),
                            TxPartName = row["PartName"].ToString(),
                            TxItemCode = Convert.ToInt32(row["ItemCode"]),
                            TxItemName = row["ItemName"].ToString(),
                            TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                            TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                            TxAdInTxable = row["AddInTaxable"].ToString(),
                            TxRoundOff = row["RoundOff"].ToString(),
                            TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                            TxTaxTypeName = row["TaxType"].ToString(),
                            TxAccountCode = Convert.ToInt32(row["TaxAccountCode"]),
                            TxAccountName = row["TaxName"].ToString(),
                            TxAmount = Convert.ToDecimal(row["Amount"]),
                            TxRefundable = row["TaxRefundable"].ToString(),
                            TxRemark = row["Remarks"].ToString(),
                        });
                    }
                    MainModel.TaxDetailGridd = _TaxList;
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[4].Rows)
                    {
                        _PendingIndentDetail.Add(new PendingIndentDetailModel
                        {
                            IndentNo = row["IndentNo"]?.ToString(),
                            IndentDate = row["IndentDate"]?.ToString(),
                            IndentYearCode = row["IndentYearCode"]?.ToString(),
                            IndentEntryId = row["IndentEntryId"]?.ToString(),
                            PONO = row["PONo"]?.ToString(),
                            POEntryId = row["POEntryID"]?.ToString(),
                            POYearCode = row["POYearCode"]?.ToString(),
                            POAccountCode = row["POAccountCode"]?.ToString(),
                            Qty = row["Qty"]?.ToString(),
                            AltQty = row["AltQty"]?.ToString(),
                            Unit = row["Unit"]?.ToString(),
                            AltUnit = row["AltUnit"]?.ToString(),
                            ReqDate = row["RequiredDate"]?.ToString(),
                            PODate = row["PoDate"]?.ToString(),
                            ItemCode = row["ItemCode"]?.ToString()
                        });
                    }
                    MainModel.PendingIndentDetailGrid = _PendingIndentDetail;
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
        return MainModel;
    }

    public async Task<PODashBoard> GetAmmDashboardData()
    {
        var model = new PODashBoard();

        try
        {
            var endDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PURCHAMMDASHBOARD"));
            SqlParams.Add(new SqlParameter("@OrderType", ""));
            SqlParams.Add(new SqlParameter("@POType", ""));
            SqlParams.Add(new SqlParameter("@POFor", ""));
            SqlParams.Add(new SqlParameter("@PONo", ""));
            SqlParams.Add(new SqlParameter("@ItemName", ""));
            SqlParams.Add(new SqlParameter("@PartCode", ""));
            SqlParams.Add(new SqlParameter("@Branch", ""));
            SqlParams.Add(new SqlParameter("@StartDate", "01/jan/2023"));
            SqlParams.Add(new SqlParameter("@EndDate", endDt));
            SqlParams.Add(new SqlParameter("@VendorName", ""));

            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            model.SummaryDetail = "Summary";
            if (((DataTable)ResponseResult.Result).Rows.Count > 0)
            {
                if (model.SummaryDetail == "Summary")
                {
                    var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "AmmNo", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                        "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                      "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm");
                    oDT.TableName = "PODASHBOARDMain";
                    model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
                }


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
    public async Task<PODashBoard> GetAmmCompletedData(string summaryDetail)
    {
        var model = new PODashBoard();

        try
        {
            var endDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
            var SqlParams = new List<dynamic>();
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var firstDt=CommonFunc.ParseFormattedDate(firstDayOfMonth.ToString("dd/MM/yyyy"));
            SqlParams.Add(new SqlParameter("@Flag", "POAMMCOMPLETED"));
            SqlParams.Add(new SqlParameter("@SummaryDetail", summaryDetail));
            SqlParams.Add(new SqlParameter("@OrderType", ""));
            SqlParams.Add(new SqlParameter("@POType", ""));
            SqlParams.Add(new SqlParameter("@POFor", ""));
            SqlParams.Add(new SqlParameter("@PONo", ""));
            SqlParams.Add(new SqlParameter("@ItemName", ""));
            SqlParams.Add(new SqlParameter("@PartCode", ""));
            SqlParams.Add(new SqlParameter("@Branch", ""));
            SqlParams.Add(new SqlParameter("@StartDate", firstDt));
            SqlParams.Add(new SqlParameter("@EndDate", endDt));
            SqlParams.Add(new SqlParameter("@VendorName", ""));

            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            if (((DataTable)ResponseResult.Result).Rows.Count > 0)
            {
                //var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "AmmNo", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                //    "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                //  "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm");
                //oDT.TableName = "PODASHBOARD";

                var oDT = ResponseResult.Result.DefaultView.ToTable(true, "VendorName", "VendorAddress", "EntryDate", "PONo", "OrderNo", "AmmNo", "AmmEffDate", "PODate",
                    "WEF", "POCloseDate", "OrderType", "POType", "POFor", "POTypeServItem", "Currency", "orderamt", "OrderNetAmt",
                     "ApproveddByEmp", "ApprovedDate", "ActualEntryBY", "ApproveAmm", "EntryID", "YearCode",
                     "Branch", "DeliveryTerms", "PaymentTerms", "UpdatedBy", "UpdatedOn");

                oDT.TableName = "POCompletedSummary";
                model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
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

    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
            SqlParams.Add(new SqlParameter("@PartCode", PartCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetReportName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<PODashBoard> GetUpdAmmData(PODashBoard model)
    {

        try
        {
            //DateTime StartDate = new DateTime();
            //DateTime EndDate = new DateTime();
            //StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
           
           var StartDate = CommonFunc.ParseFormattedDate(model.FromDate);
           var EndDate = CommonFunc.ParseFormattedDate(model.ToDate);
           
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "UPDPOAMMDASHBOARD"));
            SqlParams.Add(new SqlParameter("@OrderType", model.OrderType));
            SqlParams.Add(new SqlParameter("@POType", model.POType));
            SqlParams.Add(new SqlParameter("@POFor", model.POFor));
            SqlParams.Add(new SqlParameter("@PONo", model.PONo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
            SqlParams.Add(new SqlParameter("@Branch", model.PartCode));
            SqlParams.Add(new SqlParameter("@StartDate", StartDate));
            SqlParams.Add(new SqlParameter("@EndDate", EndDate));
            SqlParams.Add(new SqlParameter("@VendorName", model.VendorName));

            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            if (((DataTable)ResponseResult.Result).Rows.Count > 0)
            {
                var oDT = ResponseResult.Result.DefaultView.ToTable(true, "EntryID", "PONo", "POType", "POFor", "YearCode", "FOC", "Currency",
                    "POTypeServItem", "CC", "OrderType", "PaymentTerms", "POCloseDate", "PODate", "VendorName", "VendorAddress", "WEF", "Approved",
                  "CreatedBy", "UpdatedBy", "Active", "UpdatedOn", "CreatedOn", "OrderAmt", "OrderNetAmt", "Approval1Levelapproved", "ApproveAmm");
                oDT.TableName = "PODASHBOARD";

                model.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);
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
    public async Task<PurchaseOrderModel> GetViewPOCcompletedByID(int ID, int YearCode, string PONO, string Flag)
    {
        var oDataSet = new DataSet();
        var model = new PurchaseOrderModel();
        var _ItemList = new List<POItemDetail>();
        var _TaxList = new List<TaxModel>();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand oCmd = new SqlCommand("Sp_PurchaseOrder", myConnection))
                {
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@EntryID", ID);
                    oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                    oCmd.Parameters.AddWithValue("@PONo", PONO);
                    await myConnection.OpenAsync();
                    using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                    oDataAdapter.Fill(oDataSet);
                    oDataSet.Tables[0].TableName = "POAmmMain";
                    oDataSet.Tables[1].TableName = "POAmmDetail";
                    oDataSet.Tables[2].TableName = "POAmendmentTaxDetail";
                    oDataSet.Tables[3].TableName = "PODelSch";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var poApprovalDetail = CommonFunc.DataRowToClassSafe<PurchaseOrderModel>(row);

                            model = poApprovalDetail;
                        }

                    }

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[1].Rows)
                        {

                            var poDetail = CommonFunc.DataRowToClass<POItemDetail>(row);
                            _ItemList.Add(poDetail);
                        }
                    }
                    model.ItemDetailGrid = _ItemList;
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[2].Rows)
                    {
                        _TaxList.Add(new TaxModel
                        {
                            TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                            TxType = row["Type"].ToString(),
                            TxPartCode = Convert.ToInt32(row["Item_Code"]),
                            TxPartName = row["PartName"].ToString(),
                            TxItemCode = Convert.ToInt32(row["Item_Code"]),
                            TxItemName = row["ItemName"].ToString(),
                            TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                            TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                            TxAdInTxable = row["AddInTaxable"].ToString(),
                            TxRoundOff = row["RoundOff"].ToString(),
                            TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                            TxTaxTypeName = row["TaxType"].ToString(),
                            TxAccountCode = Convert.ToInt32(row["TaxAccount_Code"]),
                            TxAccountName = row["TaxName"].ToString(),
                            TxAmount = Convert.ToDecimal(row["Amount"]),
                            TxRefundable = row["TaxRefundable"].ToString(),
                            TxRemark = row["Remarks"].ToString(),
                        });
                    }
                    model.TaxDetailGridd = _TaxList;
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

    public async Task<ResponseResult> SavePurchaseOrder(DataTable ItemDetailDT, DataTable DelieveryScheduleDT, DataTable TaxDetailDT, DataTable IndentDetailDT, PurchaseOrderModel model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            var EntryDt = model.EntryDate;
           var PoDt = model.PODate;
           var WEFDate = model.WEF;
           var PoCloseDt =model.POCloseDate;
            var AmmDt = model.AmmDate;
            var RefDt =model.RefDate;
            var QuotDt = model.QuotDate;
            SqlParams.Add(new SqlParameter("@EntryDate", EntryDt == default ? string.Empty : CommonFunc.ParseFormattedDate( EntryDt)));
            SqlParams.Add(new SqlParameter("@PODate", PoDt == default ? string.Empty : CommonFunc.ParseFormattedDate(PoDt)));
            SqlParams.Add(new SqlParameter("@WEF", WEFDate == default ? string.Empty : CommonFunc.ParseFormattedDate(WEFDate)));
            SqlParams.Add(new SqlParameter("@POCloseDate", PoCloseDt == default ? string.Empty : CommonFunc.ParseFormattedDate(PoCloseDt)));
            SqlParams.Add(new SqlParameter("@AmmEffDate", AmmDt == default ? string.Empty : CommonFunc.ParseFormattedDate(AmmDt)));
            SqlParams.Add(new SqlParameter("@RefDate", RefDt == default ? string.Empty : CommonFunc.ParseFormattedDate( RefDt)));

            SqlParams.Add(new SqlParameter("@Flag", model.Mode == "COPY" ? "INSERT" : model.Mode));
            SqlParams.Add(new SqlParameter("@ID", model.ID));
            if (model.Mode == "INSERT")
                SqlParams.Add(new SqlParameter("@EntryID", 0));
            else
                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
            if (model.Mode == "POA")
            {
                SqlParams.Add(new SqlParameter("@POAmendYearcode", model.AmmYearCode));

                //AmmAppDate = ParseDate(model.AmmApprovedDate);
                //AppDate = ParseDate(model.ApprovedDate);
                //FirstLvlAppDate = ParseDate(model.FirstLvlApprovedDate);

                var AmmAppDate = model.AmmApprovedDate != null ? Common.CommonFunc.ParseFormattedDate(DateTime.Today.ToString()) : string.Empty;
                var AppDate = model.ApprovedDate != null ? Common.CommonFunc.ParseFormattedDate(model.ApprovedDate) : string.Empty;
                var FirstLvlAppDate = model.FirstLvlApprovedDate != null ? Common.CommonFunc.ParseFormattedDate(model.FirstLvlApprovedDate) : string.Empty;

                SqlParams.Add(new SqlParameter("@ApprovedDate", AppDate == default ? string.Empty : CommonFunc.ParseFormattedDate(AppDate)));
                SqlParams.Add(new SqlParameter("@AmendApproveDate", AmmAppDate == default ? string.Empty : AmmAppDate));
                SqlParams.Add(new SqlParameter("@Approval1LevelDate", AmmAppDate == default ? string.Empty : AmmAppDate));

                SqlParams.Add(new SqlParameter("@Approved", model.Approved));
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.Approvedby));
                SqlParams.Add(new SqlParameter("@AmendApproved", model.AmmApproved));
                SqlParams.Add(new SqlParameter("@AmendApprovedBy", model.AmmApprovedby));
                SqlParams.Add(new SqlParameter("@Approval1Levelapproved", model.AmmApproved));
                SqlParams.Add(new SqlParameter("@Approval1Levelby", model.AmmApprovedby));
            }

            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@ShippingAddress", model.ShippingAddress));
            SqlParams.Add(new SqlParameter("@POPrefix", model.POPrefix));
            SqlParams.Add(new SqlParameter("@PONo", model.PONo));
            SqlParams.Add(new SqlParameter("@OrderNo", model.OrderNo));
            SqlParams.Add(new SqlParameter("@OrderType", model.OrderType));
            SqlParams.Add(new SqlParameter("@POType", model.POType));
            SqlParams.Add(new SqlParameter("@POFor", model.POFor));
            SqlParams.Add(new SqlParameter("@POTypeServItem", model.POTypeServItem));
            SqlParams.Add(new SqlParameter("@AmmNo", model.AmmNo));
            SqlParams.Add(new SqlParameter("@RefNo", model.RefNo));

            SqlParams.Add(new SqlParameter("@QuotNo", model.QuotNo));
            SqlParams.Add(new SqlParameter("@QuotYear", model.QuotYear));
            SqlParams.Add(new SqlParameter("@ShipTo", model.ShipTo));
            SqlParams.Add(new SqlParameter("@VendorAddress", model.VendorAddress));
            SqlParams.Add(new SqlParameter("@MRPNO", model.MRPNo));
            SqlParams.Add(new SqlParameter("@MRPYearCode", model.MRPYearCode));
            SqlParams.Add(new SqlParameter("@FreightPaidBy", model.FreightPaidBy));
            SqlParams.Add(new SqlParameter("@InsurancePaidBy", model.InsurancePaidBy));
            SqlParams.Add(new SqlParameter("@ModeOFTransport", model.ModeOFTransport));
            SqlParams.Add(new SqlParameter("@Remark", model.PORemark));
            SqlParams.Add(new SqlParameter("@DeliverySch", model.DeliverySch));
            SqlParams.Add(new SqlParameter("@PackingChg", model.PackingChg));
            SqlParams.Add(new SqlParameter("@DeliveryTerms", model.DeliveryTerms));
            SqlParams.Add(new SqlParameter("@PreparedBy", model.PreparedBy));
            SqlParams.Add(new SqlParameter("@Discount", model.Discount));
            SqlParams.Add(new SqlParameter("@GSTIncludedONRate", model.GSTIncludedONRate));
            SqlParams.Add(new SqlParameter("@PackingType", model.PackingType));
            SqlParams.Add(new SqlParameter("@CurrencyID", model.Currency));
            SqlParams.Add(new SqlParameter("@TotalDiscountPercent", model.TotalDiscountPercentage));
            SqlParams.Add(new SqlParameter("@TotalDiscountAmount", model.TotalAmtAftrDiscount));
            SqlParams.Add(new SqlParameter("@TotalRoundOff", model.TotalRoundOff));
            SqlParams.Add(new SqlParameter("@BasicAmount", model.ItemNetAmount));
            SqlParams.Add(new SqlParameter("@NetAmount", model.NetTotal));
            SqlParams.Add(new SqlParameter("@PaymentTerms", model.PaymentTerms));
            SqlParams.Add(new SqlParameter("@PaymentDays", model.PaymentDays));
            SqlParams.Add(new SqlParameter("@Pricebasis", model.PriceBasis));
            SqlParams.Add(new SqlParameter("@FOC", model.FOC));
            SqlParams.Add(new SqlParameter("@Installation", model.Installation));
            SqlParams.Add(new SqlParameter("@Inspection", model.Inspection));
            SqlParams.Add(new SqlParameter("@Warrenty", model.Warrenty));
            SqlParams.Add(new SqlParameter("@Clause", model.Clause));
            SqlParams.Add(new SqlParameter("@ManuTrade", model.ManufTrade));
            SqlParams.Add(new SqlParameter("@ShipmentTerm", model.ShipmentTerm));
            SqlParams.Add(new SqlParameter("@LDClause", model.LDClause));
            SqlParams.Add(new SqlParameter("@ItemsForDepartmentID", model.ItemsForDepartment));
            SqlParams.Add(new SqlParameter("@PathOfFile1", model.PathOfFile1URL));
            SqlParams.Add(new SqlParameter("@PathOfFile2", model.PathOfFile2URL));
            SqlParams.Add(new SqlParameter("@PathOfFile3", model.PathOfFile3URL));
            SqlParams.Add(new SqlParameter("@PaymentPartyAccountCode", model.PaymentPartyAccountCode));
            SqlParams.Add(new SqlParameter("@PaymentPartyAccountDetail", model.PaymentPartyAccountDetail));
            SqlParams.Add(new SqlParameter("@Branch", model.Branch));
            SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@RespEmpForQC", model.ResposibleEmplforQC));
            SqlParams.Add(new SqlParameter("@DTItemGrid", ItemDetailDT));
            SqlParams.Add(new SqlParameter("@DTSchedule", DelieveryScheduleDT));
            SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));
            SqlParams.Add(new SqlParameter("@dtIndent", IndentDetailDT));
            if (model.Mode == "UPDATE")
            {
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
            }
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
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