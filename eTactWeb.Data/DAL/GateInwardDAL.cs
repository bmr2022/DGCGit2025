using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.GateInwardModel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using common = eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL;

public class GateInwardDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ConnectionStringService _connectionStringService;
    private IDataReader? Reader;

    public GateInwardDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
    {
        _IDataLogic = iDataLogic;
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        _httpContextAccessor = httpContextAccessor;

        //DBConnectionString = configuration.GetConnectionString("eTactDB");
    }
    public async Task<ResponseResult> DeleteByID(int ID, int YC,int ActualEnteredBy,string EntryByMachineName,string gateno, string IPAddress)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));
            SqlParams.Add(new SqlParameter("@ActualEnteredBy", ActualEnteredBy));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
            SqlParams.Add(new SqlParameter("@gateno", gateno));
            SqlParams.Add(new SqlParameter("@IPAddress", IPAddress));


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillEntryandGate(string Flag, int yearCode, string SPName)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", yearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
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
            SqlParams.Add(new SqlParameter("@MainMenu", "Gate Inward"));

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
    public async Task<ResponseResult> GetDashboardData()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime currentDate = DateTime.Today;
            DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var firstDt = CommonFunc.ParseFormattedDate(firstDateOfMonth.ToString("dd/MM/yyyy"));
            var currDt= CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
            SqlParams.Add(new SqlParameter("@FromDate", firstDt));
            SqlParams.Add(new SqlParameter("@ToDate", currDt));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetFeatureOption()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }


	public async Task<PendingGateInwardDashboard> GetPendingGateEntryVPDetailData(int AccountCode,string InvoiceNo)
	{
		DataSet? oDataSet = new DataSet();
		var model = new PendingGateInwardDashboard();
		try
		{
			using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
			{
				SqlCommand oCmd = new SqlCommand("SP_GateMainDetail", myConnection)
				{
					CommandType = CommandType.StoredProcedure
				};
				
				{ oCmd.Parameters.AddWithValue("@Flag", "PendVendorPortalSalebillForGateEntryDisplayData"); }
				

				oCmd.Parameters.AddWithValue("@AccountCode", AccountCode);
				
				oCmd.Parameters.AddWithValue("@VPSaleBillNo", InvoiceNo);
				await myConnection.OpenAsync();
				using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
				{
					oDataAdapter.Fill(oDataSet);
				}
			}
			//if (DashboardType == "Summary")
			//{
			
				if (oDataSet.Tables.Count > 0 && oDataSet.Tables[1].Rows.Count > 0)
				{
					model.PendingGateEntryDashboard = (from DataRow dr in oDataSet.Tables[1].Rows
													   select new PendingGateInwardDashboard
													   {
														   PONo = dr["PONo"].ToString(),
														   PoYear = string.IsNullOrEmpty(dr["POYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["POYearCode"]),
														   PODate = dr["PODate"].ToString(),
														   
														   SchNo = dr["SchNo"].ToString(),
														   SchYearCode = string.IsNullOrEmpty(dr["SchYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SchYearCode"]),
														   SeqNo = string.IsNullOrEmpty(dr["SeqNo"].ToString()) ? 0 : Convert.ToInt32(dr["SeqNo"]),
														   seqno = string.IsNullOrEmpty(dr["SeqNo"].ToString()) ? 0 : Convert.ToInt32(dr["SeqNo"]),
														   //SchDate = dr["SchDate"].ToString(),
														   //SchEntryId = string.IsNullOrEmpty(dr["SchEntryId"].ToString()) ? 0 : Convert.ToInt64(dr["SchEntryId"]),
														   PartCode = dr["PartCode"].ToString(),
														   ItemName = dr["ItemName"].ToString(),
														   ItemCode = string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["ItemCode"]),
														   Unit = dr["Unit"].ToString(),
														   Qty = string.IsNullOrEmpty(dr["Qty"].ToString()) ? 0 : Convert.ToSingle(dr["Qty"]),
														   Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToSingle(dr["Rate"]),
														   AltQty = string.IsNullOrEmpty(dr["AltQty"].ToString()) ? 0 : Convert.ToSingle(dr["AltQty"]),
														   AltUnit = dr["AltUnit"].ToString(),
														   PendPOQty = string.IsNullOrEmpty(dr["PendPOQty"].ToString()) ? 0 : Convert.ToSingle(dr["PendPOQty"]),
														   AltPendQty = string.IsNullOrEmpty(dr["AltPendQty"].ToString()) ? 0 : Convert.ToSingle(dr["AltPendQty"]),
														   SaleBillNo = dr["SaleBillNo"].ToString(),
														   SaleBillYearCode = string.IsNullOrEmpty(dr["SaleBillYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SaleBillYearCode"]),
														   SaleBillQty = string.IsNullOrEmpty(dr["SaleBillQty"].ToString()) ? 0 : Convert.ToDecimal(dr["SaleBillQty"]),
														   Remarks = dr["Remarks"].ToString(),
														   AgainstChallanNo = dr["AgainstChallanNo"].ToString(),
														   ChallanQty = string.IsNullOrEmpty(dr["ChallanQty"].ToString()) ? 0 : Convert.ToDecimal(dr["ChallanQty"]),
														   ProcessId = string.IsNullOrEmpty(dr["ProcessId"].ToString()) ? 0 : Convert.ToInt32(dr["ProcessId"]),
														   Size = dr["Size"].ToString(),
														   Color = dr["Color"].ToString(),
														   SupplierBatchNo = dr["SupplierBatchNo"].ToString(),
														   ShelfLife = string.IsNullOrEmpty(dr["AltPendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["ShelfLife"]),
														   POType = dr["POType"].ToString(),
														   AgainstChallanYearCode = string.IsNullOrEmpty(dr["AgainstChallanYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["AgainstChallanYearCode"]),
														   NoOfBoxes = string.IsNullOrEmpty(dr["NoOfBoxes"].ToString()) ? 0 : Convert.ToInt32(dr["NoOfBoxes"]),
														   //unitrate = dr["UnitRate"].ToString()


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




	public async Task<PendingGateInwardDashboard>  GetPendingGateEntryDashboardData(int AccountCode,int docTypeId, string PoNo, int PoYearCode, int ItemCode,
  string FromDate, string ToDate,string Partcode,string ItemName,string GetDataFrom,string Invoiceno)
    {
        DataSet? oDataSet = new DataSet();
        var model = new PendingGateInwardDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_GateMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                if (GetDataFrom == "PendingPO")
                { oCmd.Parameters.AddWithValue("@Flag", "PendingGateEntryDashBoard"); }
                else
                { oCmd.Parameters.AddWithValue("@Flag", "PendVendorPortalSalebillForGateEntrySumm");
					oCmd.Parameters.AddWithValue("@VPSaleBillNo", Invoiceno);
				}


                oCmd.Parameters.AddWithValue("@AccountCode", AccountCode);
                oCmd.Parameters.AddWithValue("@docTypeId", docTypeId);
                oCmd.Parameters.AddWithValue("@PONo", PoNo);
                oCmd.Parameters.AddWithValue("@POYearCode", PoYearCode);
                oCmd.Parameters.AddWithValue("@itemcode", ItemCode);
                oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                oCmd.Parameters.AddWithValue("@ToDate", toDt);
                oCmd.Parameters.AddWithValue("@PartCode", Partcode);
                oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            //if (DashboardType == "Summary")
            //{
            if (GetDataFrom == "PendingPO")
            {
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.PendingGateEntryDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new PendingGateInwardDashboard
                                                       {
                                                         
                                                           ItemCode = string.IsNullOrEmpty(dr["itemcode"].ToString()) ? 0 : Convert.ToInt32(dr["itemcode"]),
                                                           PartCode = dr["PartCode"].ToString(),
                                                           ItemName = dr["ItemName"].ToString(),
                                                           PONo = dr["PONo"].ToString(),
                                                           POQty = string.IsNullOrEmpty(dr["POQty"].ToString()) ? 0 : Convert.ToSingle(dr["POQty"]),
                                                           PendQty = string.IsNullOrEmpty(dr["PendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["PendQty"]),
                                                           Unit = dr["Unit"].ToString(),
                                                           AltPOQty = string.IsNullOrEmpty(dr["AltPOQty"].ToString()) ? 0 : Convert.ToSingle(dr["AltPOQty"]),
                                                           AltUnit = dr["AltUnit"].ToString(),
                                                           Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToSingle(dr["Rate"]),
                                                           POTypeServItem = dr["POTypeServItem"].ToString(),
                                                           SchNo = dr["ScheduleNo"].ToString(),
                                                           POType = dr["POType"].ToString(),
                                                           VendorName = dr["Account_name"].ToString(),
                                                           PODate = dr["podate"].ToString(),
                                                           SODate = dr["schdate"].ToString(),
                                                           PoYear = string.IsNullOrEmpty(dr["PoYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["PoYearCode"]),
                                                           SoYearCode = string.IsNullOrEmpty(dr["SoYearcode"].ToString()) ? 0 : Convert.ToInt32(dr["SoYearcode"])

                                                       }).ToList();
                }
            }
            else {

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.PendingGateEntryDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new PendingGateInwardDashboard
                                                       {
                                                           Invoiceno = dr["InvoiceNo"].ToString(),
                                                           InvoiceDate = dr["InvoiceDate"].ToString(),
                                                           VendorName = dr["VendorName"].ToString(),
                                                           Transporter = dr["Transporter"].ToString(),
                                                           Truck = dr["Truck"].ToString(),
                                                           DocName = dr["DocName"].ToString(),
                                                           DriverName = dr["DriverName"].ToString(),
                                                           Remarks = dr["Remark"].ToString(),
                                                           POTypeServItem = dr["POTypeServItem"].ToString(),
                                                           TareWeight = string.IsNullOrEmpty(dr["TareWeight"].ToString()) ? 0 : Convert.ToSingle(dr["TareWeight"]),
                                                           GrossWeight = string.IsNullOrEmpty(dr["GrossWeight"].ToString()) ? 0 : Convert.ToSingle(dr["GrossWeight"]),
                                                           NetWeight = string.IsNullOrEmpty(dr["NetWeight"].ToString()) ? 0 : Convert.ToSingle(dr["NetWeight"]),
                                                           address = dr["Address"].ToString(),
                                                           ModeOfTransport = dr["ModeOfTransport"].ToString(),
                                                           CC = dr["CC"].ToString(),
                                                           ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                           EntryByMachineName = dr["EntryByMachineName"].ToString(),
                                                           LastUpdatedDate = dr["LastUpdatedDate"].ToString(),
                                                           Gateno = dr["GateEntryno"].ToString(),
                                                           entryId = string.IsNullOrEmpty(dr["GateEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["GateEntryId"]),
                                                           yearcode = string.IsNullOrEmpty(dr["GateEntryYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["GateEntryYearCode"]),
                                                           GDate = dr["EntryDate"].ToString(),
                                                           VPSaleBillEntryId = string.IsNullOrEmpty(dr["VPSaleBillEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["VPSaleBillEntryId"]),
                                                           VPSaleBillYearCode = string.IsNullOrEmpty(dr["VPSaleBillYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["VPSaleBillYearCode"]),
                                                           AccountCode = string.IsNullOrEmpty(dr["AccountCode"].ToString()) ? 0 : Convert.ToInt32(dr["AccountCode"]),
                                                           DocTypeId = string.IsNullOrEmpty(dr["DocTypeId"].ToString()) ? 0 : Convert.ToInt32(dr["DocTypeId"])

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
    public async Task<ResponseResult> GetSearchData(GateDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        var fromDt = CommonFunc.ParseFormattedDate(model.FromDate);
        var toDt = CommonFunc.ParseFormattedDate(model.ToDate);
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
            SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
            SqlParams.Add(new SqlParameter("@PONo", model.POTypeServItem));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@StartDate", fromDt));
            SqlParams.Add(new SqlParameter("@EndDate", toDt));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
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
    public async Task<ResponseResult> SaveGateInward(GateInwardModel model, DataTable GIGrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {

            var SqlParams = new List<dynamic>();
            
           var entDt = common.CommonFunc.ParseFormattedDate(model.EntryDate);
           var bilDt = common.CommonFunc.ParseFormattedDate(model.BiltyDate);
           var invDt = common.CommonFunc.ParseFormattedDate(model.InvoiceDate);
           var  updDt = common.CommonFunc.ParseFormattedDate(model.UpdatedDate);

            //DateTime Invoicedt = DateTime.ParseExact(model.InvoiceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (model.Mode == "U" || model.Mode == "V")
            {
               
                SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                
            }
            else
            {
                SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
               //model.GateNo = _IDataLogic.GetEntryID("GateMain", Constants.FinincialYear, "GateNo").ToString();
                model.GateNo = _IDataLogic.GetEntryID("GateMain", Constants.FinincialYear, "GateNo","Gateyearcode").ToString();
            }

            SqlParams.Add(new SqlParameter("@EntryID", model.EntryId));
            SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@GateNo", model.GateNo ?? ""));
            SqlParams.Add(new SqlParameter("@CompGateNo", model.CompGateNo ?? ""));
            SqlParams.Add(new SqlParameter("@GDate", entDt == default ? string.Empty : entDt));
            SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
            SqlParams.Add(new SqlParameter("@EntryTime", model.EntryTime ?? ""));
            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@InvoiceDate", invDt == default ? string.Empty : invDt));
            SqlParams.Add(new SqlParameter("@Invoiceno", model.Invoiceno ?? ""));
            SqlParams.Add(new SqlParameter("@Transporter", model.Transporter ?? ""));
            SqlParams.Add(new SqlParameter("@Truck", model.Truck ?? ""));
            SqlParams.Add(new SqlParameter("@docTypeId", model.docTypeId));
            SqlParams.Add(new SqlParameter("@DriverName", model.DriverName ?? ""));
            SqlParams.Add(new SqlParameter("@BiltyNo", model.BiltyNo ?? ""));
            SqlParams.Add(new SqlParameter("@BiltyDate", bilDt == default ? string.Empty : bilDt));
            SqlParams.Add(new SqlParameter("@PaymentMode", model.PaymentMode ?? ""));
            SqlParams.Add(new SqlParameter("@RefNo", model.RefNo ?? ""));
            SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? ""));
            SqlParams.Add(new SqlParameter("@RecUnit", model.RecUnit));
            SqlParams.Add(new SqlParameter("@Uid", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@ItemService", model.Types ?? ""));
            SqlParams.Add(new SqlParameter("@TareWeight", model.TareWeight == null ? 0.0 : model.TareWeight));
            SqlParams.Add(new SqlParameter("@GrossWeight", model.GrossWeight == null ? 0.0 : model.GrossWeight));
            SqlParams.Add(new SqlParameter("@NetWeight", model.NetWeight == null ? 0.0 : model.NetWeight));
            SqlParams.Add(new SqlParameter("@address", model.Address ?? ""));
            SqlParams.Add(new SqlParameter("@ShowPOTillDate", bilDt == default ? string.Empty : bilDt));
            SqlParams.Add(new SqlParameter("@ModeOfTransport", model.ModeOfTransport ?? ""));
            SqlParams.Add(new SqlParameter("@PreparedByEmpId", model.PreparedByEmpId));
            SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
            SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntrybyMachineName ?? ""));
            SqlParams.Add(new SqlParameter("@lastUpdatedBy", model.UpdatedBy));
            SqlParams.Add(new SqlParameter("@VPSaleBillEntryId", model.VPSaleBillEntryId));
            SqlParams.Add(new SqlParameter("@lastupdated", updDt == default ? string.Empty : updDt));
            SqlParams.Add(new SqlParameter("@IPAddress", model.IPAddress));


            SqlParams.Add(new SqlParameter("@DTSSGrid", GIGrid));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<GateInwardModel> GetEwayBillDataforPo(GateInwardModel model, DataTable GIGrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
           var invDt = common.CommonFunc.ParseFormattedDate(model.InvoiceDate);
            SqlParams.Add(new SqlParameter("@Flag", "GetEwayBillDataforPo"));
            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@InvoiceDate", invDt == default ? string.Empty : invDt));
            SqlParams.Add(new SqlParameter("@DTSSGrid", GIGrid));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
            if (_ResponseResult?.Result != null)
            {
                var dt = _ResponseResult.Result as DataTable;
                if (dt != null)
                {
                    model.ItemDetailGrid = dt.AsEnumerable().Select(row => new GateInwardItemDetail
                    {
                        PoNo = row.Table.Columns.Contains("PoNo") ? row["PoNo"].ToString() : string.Empty,
                        PoYear = row.Table.Columns.Contains("PoYear") && row["PoYear"] != DBNull.Value ? Convert.ToInt32(row["PoYear"]) : 0,
                        PoEntryId = row.Table.Columns.Contains("PoEntryId") && row["PoEntryId"] != DBNull.Value ? Convert.ToInt32(row["PoEntryId"]) : 0,
                        PoDate = row.Table.Columns.Contains("PoDate") && row["PoDate"] != DBNull.Value ? (row["PoDate"]).ToString() : string.Empty,
                        POType = row.Table.Columns.Contains("POType") ? row["POType"].ToString() : string.Empty,
                        ItemCode = row.Table.Columns.Contains("ItemCode") && row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        PartCode = row.Table.Columns.Contains("PartCode") ? row["PartCode"].ToString() : string.Empty,
                        ItemName = row.Table.Columns.Contains("ItemName") ? row["ItemName"].ToString() : string.Empty,
                        Unit = row.Table.Columns.Contains("Unit") ? row["Unit"].ToString() : string.Empty,
                        UnitRate = row.Table.Columns.Contains("UnitRate") ? row["UnitRate"].ToString() : string.Empty,
                        Qty = row.Table.Columns.Contains("Qty") && row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0,
                        Rate = row.Table.Columns.Contains("Rate") && row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                        AltUnit = row.Table.Columns.Contains("AltUnit") ? row["AltUnit"].ToString() : string.Empty,
                        AltQty = row.Table.Columns.Contains("AltQty") && row["AltQty"] != DBNull.Value ? Convert.ToDecimal(row["AltQty"]) : 0,
                        PendQty = row.Table.Columns.Contains("PendQty") && row["PendQty"] != DBNull.Value ? Convert.ToDecimal(row["PendQty"]) : 0,
                        AltPendQty = row.Table.Columns.Contains("AltPendQty") && row["AltPendQty"] != DBNull.Value ? Convert.ToSingle(row["AltPendQty"].ToString()) : 0,
                        SchNo = row.Table.Columns.Contains("SchNo") ? row["SchNo"].ToString() : string.Empty,
                        SchYearCode = row.Table.Columns.Contains("SchYearCode") && row["SchYearCode"] != DBNull.Value ? Convert.ToInt32(row["SchYearCode"]) : 0,
                        SchEntryId = row.Table.Columns.Contains("SchEntryId") && row["SchEntryId"] != DBNull.Value ? Convert.ToInt32(row["SchEntryId"]) : 0,
                        SchDate = row.Table.Columns.Contains("SchDate") && row["SchDate"] != DBNull.Value ? (row["SchDate"]).ToString() : string.Empty
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
        return model;
    }
    public async Task<GateInwardDashboard> GetDashboardData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate,string DashboardType)
    {
        DataSet? oDataSet = new DataSet();
        var model = new GateInwardDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_GateMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                oCmd.Parameters.AddWithValue("@Flag", "Search");
                oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                oCmd.Parameters.AddWithValue("@GateNo", Gateno);
                oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                oCmd.Parameters.AddWithValue("@DocType", DocName);
                oCmd.Parameters.AddWithValue("@PONo", PONO);
                oCmd.Parameters.AddWithValue("@ScheduleNo", ScheduleNo);
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
            //if (DashboardType == "Summary")
            //{
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.GateDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new GateInwardDashboard
                                           {
                                               Gateno = dr["GateNo"].ToString(),
                                               GDate = dr["GDate"].ToString().Split(" ")[0],
                                               VendorName = dr["VendorName"].ToString(),
                                               address = dr["address"].ToString(),
                                               Invoiceno = dr["Invoiceno"].ToString(),
                                               InvoiceDate = dr["InvoiceDate"].ToString().Split(" ")[0],
                                               DocName = dr["DocName"].ToString(),
                                               CompGateNo = dr["CompGateNo"].ToString(),
                                               POTypeServItem = dr["POTypeServItem"].ToString(),
                                               entryId = dr["entryId"].ToString(),
                                               yearcode = Convert.ToInt32(dr["yearcode"]),
                                               MrnNo = dr["MRNNO"].ToString(),
                                               MRNYEARCODE = Convert.ToInt32(dr["MRNYEARCODE"]),
                                               MRNDate = dr["MRNDate"].ToString().Split(" ")[0],
                                               EnteredBy = dr["EnteredBy"].ToString(),
                                               UpdatedBy = dr["UpdatedBy"].ToString()
                                           }).ToList();
                }
            //}
            //else
            //{
            //    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            //    {
            //        model.GateDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
            //                               select new GateInwardDashboard
            //                               {
            //                                   Gateno = dr["GateNo"].ToString(),
            //                                   GDate = dr["GDate"].ToString().Split(" ")[0],
            //                                   VendorName = dr["VendorName"].ToString(),
            //                                   address = dr["address"].ToString(),
            //                                   Invoiceno = dr["Invoiceno"].ToString(),
            //                                   InvoiceDate = dr["InvoiceDate"].ToString().Split(" ")[0],
            //                                   DocName = dr["DocName"].ToString(),
            //                                   CompGateNo = dr["CompGateNo"].ToString(),
            //                                   POTypeServItem = dr["POTypeServItem"].ToString(),
            //                                   entryId = dr["entryId"].ToString(),
            //                                   yearcode = Convert.ToInt32(dr["yearcode"]),
            //                                   MrnNo = dr["MRNNO"].ToString(),
            //                                   MRNYEARCODE = Convert.ToInt32(dr["MRNYEARCODE"]),
            //                                   MRNDate = dr["MRNDate"].ToString().Split(" ")[0],
            //                                   EnteredBy = dr["EnteredBy"].ToString(),
            //                                   UpdatedBy = dr["UpdatedBy"].ToString(),
            //                                   SaleBillNo = dr["SaleBillNo"].ToString(),
            //                                   SaleBillQty = Convert.ToInt32(dr["SaleBillQty"]),
            //                                   SaleBillYearCode = Convert.ToInt32(dr["SaleBillYearCode"]),
            //                                   AgainstChallanNo = dr["AgainstChallanNo"].ToString(),
            //                                   ChallanQty = Convert.ToDecimal(dr["ChallanQty"]),
            //                                   SupplierBatchNo = dr["SupplierBatchNo"].ToString(),
            //                                   ShelfLife = Convert.ToDecimal(dr["ShelfLife"]),
            //                                   Remarks = dr["Remarks"].ToString(),
            //                                   ProcessName = dr["ProcessName"].ToString()
            //                               }).ToList();
            //    }
            //}
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
    public async Task<GateInwardDashboard> GetDashboardDetailData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate)
    {
        DataSet? oDataSet = new DataSet();
        var model = new GateInwardDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_GateMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD");
                oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                oCmd.Parameters.AddWithValue("@GateNo", Gateno);
                oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                oCmd.Parameters.AddWithValue("@DocType", DocName);
                oCmd.Parameters.AddWithValue("@PONo", PONO);
                oCmd.Parameters.AddWithValue("@ScheduleNo", ScheduleNo);
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
                model.GateDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                       select new GateInwardDashboard
                                       {
                                           Gateno = dr["GateNo"].ToString(),
                                           GDate = dr["GDate"].ToString().Split(" ")[0],
                                           VendorName = dr["VendorName"].ToString(),
                                           address = dr["address"].ToString(),
                                           Invoiceno = dr["Invoiceno"].ToString(),
                                           InvoiceDate = dr["InvoiceDate"].ToString().Split(" ")[0],
                                           DocName = dr["DocName"].ToString(),
                                           CompGateNo = dr["CompGateNo"].ToString(),
                                           POTypeServItem = dr["POTypeServItem"].ToString(),
                                           entryId = dr["entryId"].ToString(),
                                           yearcode = Convert.ToInt32(dr["yearcode"]),
                                           MrnNo = dr["MRNNO"].ToString(),
                                           MRNYEARCODE = Convert.ToInt32(dr["MRNYEARCODE"]),
                                           MRNDate = dr["MRNDate"].ToString().Split(" ")[0],
                                           EnteredBy = dr["EnteredBy"].ToString(),
                                           UpdatedBy = dr["UpdatedBy"].ToString(),
                                           ShowPoTillDate = string.IsNullOrEmpty(dr["ShowPOTillDate"].ToString()) ? "" : dr["ShowPOTillDate"].ToString(),
                                           CC = string.IsNullOrEmpty(dr["CC"].ToString()) ? "" : dr["CC"].ToString(),
                                           POtype = string.IsNullOrEmpty(dr["POType"].ToString()) ? "" : dr["POType"].ToString(),
                                           SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                           PoNo = string.IsNullOrEmpty(dr["PONo"].ToString()) ? "" : dr["PONo"].ToString(),
                                           Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                           Size = string.IsNullOrEmpty(dr["Size"].ToString()) ? "" : dr["Size"].ToString(),
                                           Color = string.IsNullOrEmpty(dr["Color"].ToString()) ? "" : dr["Color"].ToString(),
                                           PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                           ItemName = string.IsNullOrEmpty(dr["ItemNamePartCode"].ToString()) ? "" : dr["ItemNamePartCode"].ToString(),
                                           EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                           AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                           TareWeight = string.IsNullOrEmpty(dr["TareWeight"].ToString()) ? 0 : Convert.ToSingle(dr["TareWeight"].ToString()),
                                           GrossWeight = string.IsNullOrEmpty(dr["GrossWeight"].ToString()) ? 0 : Convert.ToSingle(dr["GrossWeight"].ToString()),
                                           NetWeight = string.IsNullOrEmpty(dr["NetWeight"].ToString()) ? 0 : Convert.ToSingle(dr["NetWeight"].ToString()),
                                           Qty = string.IsNullOrEmpty(dr["Qty"].ToString()) ? 0 : Convert.ToSingle(dr["Qty"].ToString()),
                                           PendPOQty = string.IsNullOrEmpty(dr["PendPOQty"].ToString()) ? 0 : Convert.ToSingle(dr["PendPOQty"].ToString()),
                                           AltPendQty = string.IsNullOrEmpty(dr["AltPendQty"].ToString()) ? 0 : Convert.ToSingle(dr["AltPendQty"].ToString()),
                                           AltQty = string.IsNullOrEmpty(dr["altqty"].ToString()) ? 0 : Convert.ToSingle(dr["altqty"].ToString()),
                                           Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToSingle(dr["Rate"].ToString()),
                                           PoYearCode = string.IsNullOrEmpty(dr["POYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["POYearCode"].ToString()),
                                           SchYearCode = string.IsNullOrEmpty(dr["SchYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SchYearCode"].ToString()),
                                           ChallanQty = string.IsNullOrEmpty(dr["ChallanQty"].ToString()) ? 0 : Convert.ToDecimal(dr["ChallanQty"].ToString()),
                                           AgainstChallanYearCode = string.IsNullOrEmpty(dr["AgainstChallanYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["AgainstChallanYearCode"].ToString()),
                                           AgainstChallanNo = dr["AgainstChallanNo"].ToString(),
                                           SaleBillNo = dr["SaleBillNo"].ToString(),
                                           SaleBillQty = string.IsNullOrEmpty(dr["SaleBillQty"].ToString()) ? 0 : Convert.ToDecimal(dr["SaleBillQty"].ToString()),
                                           SaleBillYearCode = string.IsNullOrEmpty(dr["SaleBillYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["SaleBillYearCode"].ToString()),
                                           NoOfBoxes = string.IsNullOrEmpty(dr["NoOfBoxes"].ToString()) ? 0 : Convert.ToInt32(dr["NoOfBoxes"].ToString()),
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
    public async Task<GateInwardModel> GetViewByID(int ID, int YearCode)
    {
        var model = new GateInwardModel();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);

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
    protected virtual string GetDBConnection => DBConnectionString;
    public async Task<IList<TextValue>> GetDropDownList(string Flag, string ServiceType, string SPName, string AccountCode = "", string InvoiceDate = "", string YearCode = "", string PoNo = "")
    {
        List<TextValue> _List = new List<TextValue>();
        dynamic Listval = null;

        try
        {
            using (SqlConnection myConnection = new SqlConnection(GetDBConnection))
            {
                SqlCommand oCmd = new SqlCommand(SPName, myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                oCmd.Parameters.AddWithValue("@Flag", Flag);
                oCmd.Parameters.AddWithValue("@ItemService", ServiceType);

                if (Flag == "PENDINGPOLIST" || Flag == "PODATELIST" || Flag == "POYEARLIST")
                {
                    oCmd.Parameters.AddWithValue("@AccountCode", AccountCode);
                    oCmd.Parameters.AddWithValue("@InvoiceDate", InvoiceDate);
                    oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                    oCmd.Parameters.AddWithValue("@PONO", PoNo);
                }

                await myConnection.OpenAsync();

                Reader = await oCmd.ExecuteReaderAsync();

                if (Reader != null)
                {
                    while (Reader.Read())
                    {

                        if (Flag == "PENDINGPOLIST")
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader[Constants.PONO].ToString(),
                                Value = Reader[Constants.PONO].ToString()
                            };
                        }
                        else if (Flag == "PODATELIST")
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader[Constants.PODate].ToString(),
                                Value = Reader[Constants.PODate].ToString()
                            };
                        }
                        else if (Flag == "POYEARLIST")
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader[Constants.YearCode].ToString(),
                                Value = Reader[Constants.YearCode].ToString()
                            };
                        }

                        _List.Add(Listval);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Listval.Add(new TextValue { Text = ex.Message, Value = ex.Source });
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
    public async Task<ResponseResult> FillSaleBillChallan(int AccountCode, int doctype, int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            if (doctype == 3)
            {
                SqlParams.Add(new SqlParameter("@Flag", "SALEBILL"));
            }
            else if (doctype == 4)
            {
                SqlParams.Add(new SqlParameter("@Flag", "CHALLAN"));
            }
            else if (doctype == 5)
            {
                SqlParams.Add(new SqlParameter("@Flag", "CHALLAN"));
            }
            else if(doctype == 11)
            {
                SqlParams.Add(new SqlParameter("@Flag", "CHALLAN"));
            }

            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@docTypeId", doctype));


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillChallanQty(int AccountCode, int ItemCode,string ChallanNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            if (ChallanNo != "")
            {
                SqlParams.Add(new SqlParameter("@Flag", "CHALLANQty"));
            }           

            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@AgainstChallanNo", ChallanNo));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillSaleBillQty(int AccountCode, int ItemCode, string SaleBillNo,int SaleBillYearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            if (SaleBillNo != null)
            {
                SqlParams.Add(new SqlParameter("@Flag", "SALEBILLQty"));
            }

            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
            SqlParams.Add(new SqlParameter("@SaleBillYearcode", SaleBillYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    } 
    public async Task<ResponseResult> GetAccountCode(string AccountName)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
           
                SqlParams.Add(new SqlParameter("@Flag", "GetAccountCode"));
            SqlParams.Add(new SqlParameter("@AccountName", AccountName));
           
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetItemCode(string ItemName)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
           
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
            SqlParams.Add(new SqlParameter("@ItemName", ItemName));
           
            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetItems(string Flag, int doctype, string Check,int AccountCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GETITEMS"));
            SqlParams.Add(new SqlParameter("@docTypeId", doctype));
            SqlParams.Add(new SqlParameter("@ShowAll", Check));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> AltUnitConversion(int ItemCode, int AltQty, int UnitQty)
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
    public async Task<ResponseResult> FillPendQty(int ItemCode, int PartyCode, string PONO, int POYear, int Year, string SchNo, int SchYearCode, int ProcessId, int EntryId, int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@ITEMCODE", ItemCode));
            SqlParams.Add(new SqlParameter("@PARTY_CODE", PartyCode));
            SqlParams.Add(new SqlParameter("@PONO", PONO ?? ""));
            SqlParams.Add(new SqlParameter("@PO_YEAR", POYear));
            SqlParams.Add(new SqlParameter("@YEAR", Year));
            SqlParams.Add(new SqlParameter("@SCH_NO", SchNo ?? ""));
            SqlParams.Add(new SqlParameter("@SCH_YEAR", SchYearCode));
            SqlParams.Add(new SqlParameter("@TILLDATE", DateTime.Today));
            SqlParams.Add(new SqlParameter("@Process_id", ProcessId));
            SqlParams.Add(new SqlParameter("@EntryID", EntryId));
            SqlParams.Add(new SqlParameter("@YEARCODE", YearCode));


            _ResponseResult = await _IDataLogic.ExecuteDataTable("GatePendingQtyAgainstPOSch", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPopUpData(string Flag, int AccountCode, string PONO)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@PONO", PONO ?? ""));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckEditOrDelete(string GateNo, int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
            SqlParams.Add(new SqlParameter("@GateNo", GateNo));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetScheDuleByYearCodeandAccountCode(string Flag, string AccountCode, string YearCode, string poNo,int docTypeId, string InvoiceDate,string ItemService, string EntryDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));

            SqlParams.Add(new SqlParameter("@ItemService", ItemService));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            if (Flag == "PURCHSCHEDULE")
            {
                SqlParams.Add(new SqlParameter("@POYearCode", YearCode));
            }
            SqlParams.Add(new SqlParameter("@PONO", poNo));
            SqlParams.Add(new SqlParameter("@docTypeId", docTypeId));
            SqlParams.Add(new SqlParameter("@EntryDate",CommonFunc.ParseFormattedDate( EntryDate)));
            SqlParams.Add(new SqlParameter("@InvoiceDate", CommonFunc.ParseFormattedDate( InvoiceDate)));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    private static GateInwardModel PrepareView(DataSet DS, ref GateInwardModel? model)
    {
        var ItemList = new List<GateInwardItemDetail>();
        DS.Tables[0].TableName = "SSMain";
        DS.Tables[1].TableName = "SSDetail";
        int cnt = 1;
        model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
        model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
        model.EntryTime = Convert.ToDateTime(DS.Tables[0].Rows[0]["EntryTime"]).ToString("HH:mm:ss")?.Trim();
        //model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString().Split(" ")[0];
        model.GateNo = DS.Tables[0].Rows[0]["GateNo"].ToString();
        model.CompGateNo = DS.Tables[0].Rows[0]["EntryID"].ToString();
        //isnull(, '') Remark,Uid,POTypeServItem,isnull(TareWeight, 0)TareWeight ,
        //isnull(GrossWeight, 0)GrossWeight ,isnull(NetWeight, 0)NetWeight ,
        //address,isnull(ShowPOTillDate, getdate()) ShowPOTillDate ,isnull(ModeOfTransport, '')ModeOfTransport,
        //PreparedByEmpId,  

        model.Invoiceno = DS.Tables[0].Rows[0]["Invoiceno"].ToString();
        model.Address = DS.Tables[0].Rows[0]["address"].ToString();
        model.Transporter = DS.Tables[0].Rows[0]["Transporter"].ToString();
        model.Truck = DS.Tables[0].Rows[0]["Truck"].ToString();
        model.docTypeId = Convert.ToInt32(DS.Tables[0].Rows[0]["docTypeId"].ToString());
        model.RecUnit = Convert.ToInt32(DS.Tables[0].Rows[0]["RecUnit"].ToString());
        model.DriverName = DS.Tables[0].Rows[0]["DriverName"].ToString();
        model.BiltyNo = DS.Tables[0].Rows[0]["BiltyNo"].ToString();
        model.Truck = DS.Tables[0].Rows[0]["Truck"].ToString();

        model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();

        //model.EntryDate =  DS.Tables[0].Rows[0]["EntryDate"].ToString();
        model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
        model.ModeOfTransport = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString();
        model.PreparedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["PreparedByEmpId"].ToString());
        model.PreparedByEmp = DS.Tables[0].Rows[0]["PreparedBY"].ToString();
        model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
        model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryBY"].ToString();
        model.ActualEntryDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);

        model.TareWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["TareWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["TareWeight"].ToString());
        model.NetWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["NetWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["NetWeight"].ToString());
        model.GrossWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["GrossWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["GrossWeight"].ToString());
        model.RefNo = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["RefNo"].ToString()) ? "" : DS.Tables[0].Rows[0]["RefNo"].ToString();
        model.PaymentMode = DS.Tables[0].Rows[0]["PaymentMode"].ToString();


        if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()))
        {
            model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedBy"].ToString();

            model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
            model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"]);
        }
        //model.DocumentList = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString();
        model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
        if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
        {
            foreach (DataRow row in DS.Tables[1].Rows)
            {
                ItemList.Add(new GateInwardItemDetail
                {
                    SeqNo = cnt++,
                    PoNo = (row["PONo"].ToString()),
                    PoYear = Convert.ToInt32(row["POYearCode"].ToString()),
                    PoEntryId = Convert.ToInt32(row["PoEntryId"].ToString()),
                    POType = row["POTYpe"].ToString(),
                    SchNo = row["SchNo"].ToString(),
                    SchYearCode = Convert.ToInt32(row["SchYearCode"].ToString()),
                    ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                    Qty = Convert.ToDecimal(row["Qty"].ToString()),
                    AltQty = Convert.ToDecimal(row["AltQty"].ToString()),
                    PartCode = row["PartCode"].ToString(),
                    ItemName = row["ItemNamePartCode"].ToString(),
                    ProcessId = Convert.ToInt32(row["processid"].ToString()),
                    SupplierBatchNo = row["SupplierBatchNo"].ToString(),
                    ShelfLife = Convert.ToDecimal(row["ShelfLife"].ToString()),
                    Rate = Convert.ToDecimal(row["Rate"].ToString()),
                    Unit = row["Unit"].ToString(),
                    UnitRate = row["UnitRate"].ToString(),
                    AltUnit = row["altunit"].ToString(),
                    ItemSize = row["Size"].ToString(),
                    ItemColor = row["Color"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    SaleBillNo = (row["SaleBillNo"].ToString()),
                    SaleBillYearCode = Convert.ToInt32(row["SaleBillYearCode"].ToString()),
                    SaleBillQty = Convert.ToDecimal(row["SaleBillQty"].ToString()),
                    AgainstChallanNo = (row["AgainstChallanNo"].ToString()),
                    ChallanQty = Convert.ToDecimal(row["ChallanQty"].ToString()),
                    AgainstChallanYearcode =Convert.ToInt32(row["AgainstChallanYearcode"].ToString()),
                    PendQty = Convert.ToDecimal(row["PendPOQty"].ToString()),
                    NoOfBoxes = row["NoOfBoxes"].ToString() == "" ? 0 : Convert.ToInt32(row["NoOfBoxes"].ToString())
                });
            }
            model.ItemDetailGrid = ItemList;
        }

        if (DS.Tables[0].Rows[0]["BiltyDate"] != DBNull.Value)
        {
            model.BiltyDate = DS.Tables[0].Rows[0]["BiltyDate"].ToString();
        }
        else
        {
            model.BiltyDate = "";
        }
        if (DS.Tables[0].Rows[0]["ShowPOTillDate"] != DBNull.Value)
        {
            model.ShowPOTillDate = DS.Tables[0].Rows[0]["ShowPOTillDate"].ToString();
        }
        else
        {
            model.ShowPOTillDate = "";
        }

        //model.BiltyDate = DateTime.ParseExact(DS.Tables[0].Rows[0]["BiltyDate"].ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

        model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
        model.InvoiceDate = DS.Tables[0].Rows[0]["InvoiceDate"].ToString();

        return model;
    }
    public async Task<ResponseResult> FillItems(string Flag, string accountCode, string Year, string poNo, string Type, string scheduleNO = "", string scheduleYear = "", string Check = "")
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            if (Check == "T")
                SqlParams.Add(new SqlParameter("@Flag", "GETALLITEMS"));
            else
            {
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@ItemService", Type));
                SqlParams.Add(new SqlParameter("@AccountCode", accountCode));
                SqlParams.Add(new SqlParameter("@POYearCode", Year));
                SqlParams.Add(new SqlParameter("@PONO", poNo));
            }

            if (!string.IsNullOrEmpty(scheduleNO))
            {
                SqlParams.Add(new SqlParameter("@Schno", scheduleNO));
            }
            if (!string.IsNullOrEmpty(scheduleYear))
            {
                SqlParams.Add(new SqlParameter("@SchYearcode", scheduleYear));
            }
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckFeatureOption()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> CCEnableDisable()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ChangeBranch"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPoNumberDropDownList(string Flag, string ServiceType, string SPName, string AccountCode, int Year ,int DocTypeId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PENDINGPOLIST"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@InvoiceDate", DateTime.Today));
            SqlParams.Add(new SqlParameter("@YearCode", Year));
            SqlParams.Add(new SqlParameter("@ItemService", ServiceType));
            SqlParams.Add(new SqlParameter("@docTypeId", DocTypeId));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DuplicateInv"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@Invoiceno", InvNo));
            SqlParams.Add(new SqlParameter("@Doctypeid", DocType));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GateMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillSaleBillRate(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@flag", "SALEBILLRate"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
            SqlParams.Add(new SqlParameter("@SaleBillYearcode", SaleBillYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GateMainDetail", SqlParams);
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