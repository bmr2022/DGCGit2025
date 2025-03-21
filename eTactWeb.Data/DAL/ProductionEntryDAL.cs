using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks.Dataflow;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL;

public class ProductionEntryDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private IDataReader? Reader;

    public ProductionEntryDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
    {
        _IDataLogic = iDataLogic;
        DBConnectionString = configuration.GetConnectionString("eTactDB");
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate, int ActualEntryBy)
    {
        var _ResponseResult = new ResponseResult();
        var entrydt = ParseDate(EntryDate);
        string formattedEntryDate = entrydt.ToString("yyyy-MM-dd");
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));
            SqlParams.Add(new SqlParameter("@cc", CC));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
            SqlParams.Add(new SqlParameter("@ActualEntryBy", ActualEntryBy));
            SqlParams.Add(new SqlParameter("@EntryDate", formattedEntryDate));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
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
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillShift()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillShift"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillShiftTime(int ShiftId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillShiftTime"));
            SqlParams.Add(new SqlParameter("@ShiftId", ShiftId));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetWorkCenterTotalStock(string Flag, int ItemCode, int WcId, string TillDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime ChallanDt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
            SqlParams.Add(new SqlParameter("@WCID", WcId));
            SqlParams.Add(new SqlParameter("@TILL_DATE", ChallanDt.ToString("yyyy/MM/dd")));
            _ResponseResult = await _IDataLogic.ExecuteDataTable(Flag, SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetWorkCenterQty(string SPNAme, int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
    {
        var Result = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();
            DateTime tilldt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
            SqlParams.Add(new SqlParameter("@WCID", WcId));
            SqlParams.Add(new SqlParameter("@TILL_DATE", tilldt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo));
            SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo));


            Result = await _IDataLogic.ExecuteDataTable("GETWIPSTOCKBATCHWISE", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return Result;
    }
    public async Task<ResponseResult> FillStore()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Store"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetUnit(int RmItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Unit"));
            SqlParams.Add(new SqlParameter("@RmItemCode", RmItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetScrapUnit(int ScrapItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ScrapUnit"));
            SqlParams.Add(new SqlParameter("@FGItemCode", ScrapItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillWorkcenter()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Workcenter"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillIssWorkcenterForQcMandatory(string QcMandatory)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "IssWorkcenterForQcMandatory"));
            SqlParams.Add(new SqlParameter("@QCMandatory", QcMandatory));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillIssWorkcenter(string QcMandatory)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "IssWorkcenter"));
            SqlParams.Add(new SqlParameter("@QCMandatory", QcMandatory));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillIssStore()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "IssStore"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckAllowToAddNegativeStock()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "AllowNegatveValue"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillOperation(int ItemCode, int WcId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Operation"));
            SqlParams.Add(new SqlParameter("@FGitemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@WCID", WcId));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillMachineGroup()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "machinegroup"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillMachineName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "machine"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillSuperwiser()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Superwise"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ProductionEntryModel> GetChildData(string Flag, string SPName, int WcId, int YearCode, float ProdQty, int ItemCode, string ProdDate, int BomNo)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryModel();
        try
        {
            DateTime prodDt = DateTime.ParseExact(ProdDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SpGetBomitemWithWorkcenterStock", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", Flag);
                oCmd.Parameters.AddWithValue("@WCID", WcId);
                oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                oCmd.Parameters.AddWithValue("@TotalFGProdQty", ProdQty);
                oCmd.Parameters.AddWithValue("@FGItemCode", ItemCode);
                oCmd.Parameters.AddWithValue("@ProdDate", prodDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@BomNo", BomNo);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                int count = 1;
                model.ProductionChilDataDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new ProductionEntryItemDetail
                                                  {
                                                      RmItemCode = string.IsNullOrEmpty(dr["RMItemcode"].ToString()) ? 0 : Convert.ToInt32(dr["RMItemcode"].ToString()),
                                                      RmPartCode = dr["RMPartcode"].ToString() ?? "",
                                                      RmItemName = dr["RmItemName"].ToString() ?? "",
                                                      TotalReqRMQty = string.IsNullOrEmpty(dr["TotalReqRMQty"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalReqRMQty"].ToString()),
                                                      ConsumedRMQTY = string.IsNullOrEmpty(dr["ConsumedRMQTY"].ToString()) ? 0 : Convert.ToDecimal(dr["ConsumedRMQTY"].ToString()),
                                                      RmUnit = dr["RMUnit"].ToString() ?? "",
                                                      MainItemCode = string.IsNullOrEmpty(dr["MainItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["MainItemCode"].ToString()),
                                                      MainRMQTY = string.IsNullOrEmpty(dr["MainRMQty"].ToString()) ? 0 : Convert.ToDecimal(dr["MainRMQty"].ToString()),
                                                      ConsumedRMItemCode = string.IsNullOrEmpty(dr["ConsRmCode"].ToString()) ? 0 : Convert.ToInt32(dr["ConsRmCode"].ToString()),
                                                      FGProdQty = string.IsNullOrEmpty(dr["FGProdQty"].ToString()) ? 0 : Convert.ToDecimal(dr["FGProdQty"].ToString()),
                                                      TotalStock = string.IsNullOrEmpty(dr["ToatlStock"].ToString()) ? 0 : Convert.ToDecimal(dr["ToatlStock"].ToString()),
                                                      BatchStock = string.IsNullOrEmpty(dr["BatchStock"].ToString()) ? 0 : Convert.ToDecimal(dr["BatchStock"].ToString()),
                                                      WCId = string.IsNullOrEmpty(dr["WCId"].ToString()) ? 0 : Convert.ToInt32(dr["WCId"].ToString()),
                                                      AltRMItemCode = string.IsNullOrEmpty(dr["AltRMItemcode"].ToString()) ? 0 : Convert.ToInt32(dr["AltRMItemcode"].ToString()),
                                                      AltQty = string.IsNullOrEmpty(dr["AltQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltQty"].ToString()),
                                                      AltRMUnit = dr["AltUnit"].ToString() ?? "",
                                                      RMNetWt = string.IsNullOrEmpty(dr["RMNetWt"].ToString()) ? 0 : Convert.ToDecimal(dr["RMNetWt"].ToString()),
                                                      GrossWt = string.IsNullOrEmpty(dr["GrossWt"].ToString()) ? 0 : Convert.ToDecimal(dr["GrossWt"].ToString()),
                                                      Batchwise = dr["Batchwise"].ToString() ?? "",
                                                      BatchNo = dr["BatchNo"].ToString() ?? "",
                                                      UniqueBatchNo = dr["UniqueBatchNo"].ToString() ?? "",
                                                      PendQty = string.IsNullOrEmpty(dr["PendQty"].ToString()) ? 0 : Convert.ToDecimal(dr["PendQty"].ToString()),
                                                      StdPacking = string.IsNullOrEmpty(dr["StdPacking"].ToString()) ? 0 : Convert.ToDecimal(dr["StdPacking"].ToString()),
                                                      PendAfterIssue = string.IsNullOrEmpty(dr["PendAfterIssue"].ToString()) ? 0 : Convert.ToDecimal(dr["PendAfterIssue"].ToString()),
                                                      StoreName = dr["StoreName"].ToString() ?? "",
                                                      BatchDate = dr["Batchdate"].ToString() ?? "",
                                                      Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"].ToString()),
                                                      Remark = dr["Remark"].ToString() ?? "",
                                                      RemainingStock = string.IsNullOrEmpty(dr["RemainingStock"].ToString()) ? 0 : Convert.ToDecimal(dr["RemainingStock"].ToString()),
                                                      ID = string.IsNullOrEmpty(dr["id"].ToString()) ? 0 : Convert.ToInt32(dr["id"].ToString()),
                                                      SeqNo = string.IsNullOrEmpty(dr["SeqNo"].ToString()) ? 0 : Convert.ToInt32(dr["SeqNo"].ToString()),
                                                  }).OrderBy(x => x.SeqNo).ToList();
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
    public async Task<ResponseResult> FillOperator()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Operatory"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillTool()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillTool"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillOperatorName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "OperatorList"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillBreakdownreason()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "BindBreakDownReason"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillResponsibleEmp()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ResponsibleEmp"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillResFactor()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ResFactorDetail"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillScrapItems()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ScrapItemName"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillScrapPartCode()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ScrapPartCode"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillScrapType()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ScrapType"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillRMItemName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "FillChildItemName"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillRMPartCode()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "FillChildPartCode"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillAltItemName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "FillAlternateChildItemName"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillAltPartCode()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "FillAlternateChildPartCode"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetLastProddate(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "GetLastProddate"));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetBatchNumber(string SPName, int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
    {
        var Result = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();
            DateTime TransDt = DateTime.ParseExact(TransDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            SqlParams.Add(new SqlParameter("@transDate", TransDt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@WCID", WcId));
            SqlParams.Add(new SqlParameter("@batchno", BatchNo));

            Result = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return Result;
    }
    public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ProductionEntry"));
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
    public async Task<ResponseResult> DisplayRoutingDetail(int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Showrouting"));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetItems(string ProdAgainst, int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPartCode(string ProdAgainst, int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillBomNo(int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "BomNo"));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetProcessDetail(int ItemCode, int ProcessId, int WcId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "getnextProcessdetail"));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@ProcessId", ProcessId));
            SqlParams.Add(new SqlParameter("@WCID", WcId));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillReqNo(string FromDate, string ToDate, string ProdAgainst, int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ReqNO"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate, string ProdAgainst, int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ProdSchNo"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillProdPlanDetail(string FromDate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdYearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@FLAG", "PRODPlanDetail"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
            SqlParams.Add(new SqlParameter("@ProdpalnYearcode", ProdYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillReqYear(string FromDate, string ToDate, string ProdAgainst, string ReqNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ReqYearcode"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillReqDate(string FromDate, string ToDate, string ProdAgainst, string ReqNo, int ReqYearCode, int ItemCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ReqDate"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@ReqNoyearcode", ReqYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillProdDate(string FromDate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdPlanYearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PRODSCHDate"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
            SqlParams.Add(new SqlParameter("@ProdpalnYearcode", ProdPlanYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillProdSchYear(string FromDate, string ToDate, string ProdAgainst, string ProdSch)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PRODSCHYearcode"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@FromDate", FromDate));
            SqlParams.Add(new SqlParameter("@ToDate", ToDate));
            SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSch));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillReqQty(string CurrentDate, string ProdAgainst, string ReqNo, int YearCode, int ItemCode, int ReqYearCode)
    {
        var _ResponseResult = new ResponseResult();
        DateTime currentDt = new DateTime();
        currentDt = ParseDate(CurrentDate);
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ReqPendQty"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@CurrentDate", currentDt));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            SqlParams.Add(new SqlParameter("@ReqNo", ReqNo));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@ReqNoyearcode", ReqYearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> FillPendQty(string CurrentDate, string ProdAgainst, string ProdSchNo,
        int YearCode, int ItemCode, int ProdSchYear, int Entryid)
    {
        var _ResponseResult = new ResponseResult();
        DateTime currentDt = new DateTime();
        currentDt = ParseDate(CurrentDate);
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ProdTargetQty"));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", ProdAgainst));
            SqlParams.Add(new SqlParameter("@CurrentDate", currentDt));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            SqlParams.Add(new SqlParameter("@ProdSchNo", ProdSchNo));
            SqlParams.Add(new SqlParameter("@FGItemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@ProdSchYearcode", ProdSchYear));
            SqlParams.Add(new SqlParameter("@EntryId", Entryid));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
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
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
            SqlParams.Add(new SqlParameter("@FromDate", firstDateOfMonth.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@ToDate", currentDate.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetFeatureOption(string Flag, string SPName)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", Flag));
            _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
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
    public async Task<ResponseResult> CheckForEditandDelete(string ProdSlipNo, int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
            SqlParams.Add(new SqlParameter("@Entryid", ProdSlipNo));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> SaveProductionEntry(ProductionEntryModel model, DataTable GIGrid, DataTable BreakDownGrid, DataTable OperatorGrid, DataTable ScrapGrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {

            var SqlParams = new List<dynamic>();
            DateTime entDt = new DateTime();
            DateTime prodDt = new DateTime();
            DateTime prodplanDt = new DateTime();
            DateTime reqDt = new DateTime();
            DateTime bomDt = new DateTime();
            DateTime parentprodschDt = new DateTime();
            DateTime SoDt = new DateTime();
            DateTime qcofferDt = new DateTime();
            DateTime rewqcDt = new DateTime();
            DateTime actualDt = new DateTime();
            DateTime lastupdationDt = new DateTime();
            DateTime ProdStartTime = new DateTime();
            DateTime ProdEndTime = new DateTime();

            entDt = ParseDate(model.EntryDate);
            prodDt = ParseDate(model.ActualProdDate);
            prodplanDt = ParseDate(model.ProdSchDate);
            reqDt = ParseDate(model.ReqDate);
            actualDt = ParseDate(model.ActualEntryDate);
            ProdStartTime = ParseDate(model.ProdStartTime);
            ProdEndTime = ParseDate(model.ProdEndTime);
            bomDt = ParseDate(model.BomEffecDate);
            qcofferDt = ParseDate(model.QcOfferedDate);
            rewqcDt = ParseDate(model.ReWorkEntrydate);
            lastupdationDt = ParseDate(model.LastUpdatedDate);
            var Prodqty = model.ProdQty;
            var rejqty = model.QcRejQty;
            var startrejqty = model.StartupRej;
            var FGOkQty = Prodqty - rejqty - startrejqty;
            if (model.LastUpdationDate == null)
            {
                model.LastUpdationDate = "";
            }

            //DateTime Invoicedt = DateTime.ParseExact(model.InvoiceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (model.Mode == "U" || model.Mode == "V")
            {
                SqlParams.Add(new SqlParameter("@Flag", "Update"));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                if (model.LastUpdationDate != null)
                {
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", lastupdationDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", model.LastUpdationDate));
                }
            }
            else
            {
                SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
            }

            SqlParams.Add(new SqlParameter("@EntryId", model.EntryId == 0 ? 0 : model.EntryId));
            SqlParams.Add(new SqlParameter("@Entrydate", entDt == default ? string.Empty : entDt));
            SqlParams.Add(new SqlParameter("@Yearcode", model.YearCode == 0 ? 0 : model.YearCode));
            SqlParams.Add(new SqlParameter("@ProdAgainstReqPlanDirect", model.ProdAgainstPlanManual ?? ""));
            SqlParams.Add(new SqlParameter("@NewProdRework", model.ProdType ?? ""));
            SqlParams.Add(new SqlParameter("@ShiftId", model.ShiftId == 0 ? 0 : model.ShiftId));
            SqlParams.Add(new SqlParameter("@ProdSlipNo", model.ProdSlipNo ?? ""));
            SqlParams.Add(new SqlParameter("@ProdDate", prodDt == default ? string.Empty : prodDt));
            SqlParams.Add(new SqlParameter("@ProdPlanNo", model.ProdPlan ?? ""));
            SqlParams.Add(new SqlParameter("@ProdpalnYearcode", model.ProdPlanYear == 0 ? 0 : model.ProdPlanYear));
            SqlParams.Add(new SqlParameter("@ProdPlanDate", prodplanDt == default ? string.Empty : prodplanDt));
            SqlParams.Add(new SqlParameter("@ProdSchNo", model.ProdSch ?? ""));
            SqlParams.Add(new SqlParameter("@ProdSchYearcode", model.ProdSchYear == 0 ? 0 : model.ProdSchYear));
            SqlParams.Add(new SqlParameter("@ProdPlanSchDate", prodplanDt == default ? string.Empty : prodplanDt));
            SqlParams.Add(new SqlParameter("@Reqno", model.ReqNo));
            SqlParams.Add(new SqlParameter("@ReqNoyearcode", model.ReqYear == 0 ? 0 : model.ReqYear));
            SqlParams.Add(new SqlParameter("@ReqDate", reqDt == default ? string.Empty : reqDt));
            SqlParams.Add(new SqlParameter("@FGItemCode", model.FGItemCode == 0 ? 0.0 : model.FGItemCode));
            SqlParams.Add(new SqlParameter("@WOQTY", model.WOQTY == 0 ? 0.0 : model.WOQTY));
            if (model.ProdAgainstPlanManual == "UNPLANNED")
            {
                SqlParams.Add(new SqlParameter("@ProdSchQty", model.TargetQtyUnplanned == 0 ? 0.0 : model.TargetQtyUnplanned));
            }
            else
            {
                SqlParams.Add(new SqlParameter("@ProdSchQty", model.TargetQty == 0 ? 0.0 : model.TargetQty));
            }
            SqlParams.Add(new SqlParameter("@FGProdQty", model.ProdQty == 0 ? 0.0 : model.ProdQty));
            SqlParams.Add(new SqlParameter("@FGOkQty", FGOkQty == 0 ? 0.0 : FGOkQty));
            SqlParams.Add(new SqlParameter("@FGRejQty", model.QcRejQty == 0 ? 0.0 : model.QcRejQty));
            SqlParams.Add(new SqlParameter("@RejQtyDuetoTrail", model.RejQtyDuetoTrail == 0 ? 0.0 : model.RejQtyDuetoTrail));
            SqlParams.Add(new SqlParameter("@PendQtyForProd", model.PendQty == 0 ? 0.0 : model.PendQty));
            SqlParams.Add(new SqlParameter("@PendQtyForQC", model.PendQtyForQC == 0 ? 0.0 : model.PendQtyForQC));
            SqlParams.Add(new SqlParameter("@PendingQtyToIssue", model.PendingQtyToIssue == 0 ? 0.0 : model.PendingQtyToIssue));
            SqlParams.Add(new SqlParameter("@BOMNO", model.BOMNo == null ? string.Empty : model.BOMNo));
            SqlParams.Add(new SqlParameter("@BOMDate", bomDt == default ? string.Empty : bomDt));
            SqlParams.Add(new SqlParameter("@MachineId", model.MachineId == 0 ? 0 : model.MachineId));
            SqlParams.Add(new SqlParameter("@ProcessId", model.ProcessId == 0 ? 0 : model.ProcessId));
            SqlParams.Add(new SqlParameter("@ProdInWCID", model.ProdInWCID == 0 ? 0 : model.ProdInWCID));
            SqlParams.Add(new SqlParameter("@TransferRejtoStoreWC", model.StoreTransfer == null ? "" : model.StoreTransfer));
            SqlParams.Add(new SqlParameter("@RejWCId", model.RejWCId == 0 ? 0 : model.RejWCId));
            SqlParams.Add(new SqlParameter("@TransferToRejStoreId", model.TransferToRejStoreId == 0 ? 0 : model.TransferToRejStoreId));
            SqlParams.Add(new SqlParameter("@TransferFGToWCorSTORE", model.StoreTransfer == null ? "" : model.StoreTransfer));
            SqlParams.Add(new SqlParameter("@QCMandatory", model.QCMandatory == null ? "" : model.QCMandatory));
            SqlParams.Add(new SqlParameter("@TransferToQc", model.SenToQc == null ? "" : model.SenToQc));
            SqlParams.Add(new SqlParameter("@NextWCID", model.NextWCID == 0 ? 0 : model.NextWCID));
            SqlParams.Add(new SqlParameter("@NextStoreId", model.NextStoreId == 0 ? 0 : model.NextStoreId));
            SqlParams.Add(new SqlParameter("@StartTime", ProdStartTime));
            SqlParams.Add(new SqlParameter("@ToTime", ProdEndTime));
            SqlParams.Add(new SqlParameter("@setupTime", model.setupTime == 0 ? 0 : model.setupTime));
            SqlParams.Add(new SqlParameter("@PrevWC", model.PrevWC == 0 ? 0 : model.PrevWC));
            SqlParams.Add(new SqlParameter("@PrevProcessId", model.PrevProcessId == 0 ? 0 : model.PrevProcessId));
            SqlParams.Add(new SqlParameter("@ProducedINLineNo", model.ProducedINLineNo == null ? "" : model.ProducedINLineNo));
            SqlParams.Add(new SqlParameter("@QCChecked", model.QCChecked == null ? "" : model.QCChecked));
            SqlParams.Add(new SqlParameter("@InitialReading", model.InitalReading == 0 ? 0 : model.InitalReading));
            SqlParams.Add(new SqlParameter("@FinalReading", model.FinalReading == null ? "" : model.FinalReading));
            SqlParams.Add(new SqlParameter("@ToolItemCode", model.ToolItemCode == 0 ? 0 : model.ToolItemCode));
            SqlParams.Add(new SqlParameter("@Shots", model.Shots == 0 ? 0 : model.Shots));
            SqlParams.Add(new SqlParameter("@Completed", model.Completed == null ? "" : model.Completed));
            SqlParams.Add(new SqlParameter("@UtilisedHours", model.UtilisedHours == 0 ? 0 : model.UtilisedHours));
            SqlParams.Add(new SqlParameter("@ProdLineNo", model.ProdSeqNo == null ? "" : model.ProdSeqNo));
            SqlParams.Add(new SqlParameter("@stdShots", model.stdShots == 0 ? 0 : model.stdShots));
            SqlParams.Add(new SqlParameter("@stdCycletime", model.StdCycleTime == 0 ? 0 : model.StdCycleTime));
            SqlParams.Add(new SqlParameter("@Remark", model.Remark == null ? "" : model.Remark));
            SqlParams.Add(new SqlParameter("@CyclicTime", model.CyclicTime == 0 ? 0 : model.CyclicTime));
            SqlParams.Add(new SqlParameter("@ProductionHour", model.ProductionHour == 0 ? 0 : model.ProductionHour));
            SqlParams.Add(new SqlParameter("@ItemModel", model.ItemModel == null ? "" : model.ItemModel));
            SqlParams.Add(new SqlParameter("@cavity", model.Cavity == 0 ? 0 : model.Cavity));
            SqlParams.Add(new SqlParameter("@startupRejQty", model.StartupRej == 0 ? 0 : model.StartupRej));
            SqlParams.Add(new SqlParameter("@efficiency", model.efficiency == 0 ? 0 : model.efficiency));
            SqlParams.Add(new SqlParameter("@ActualTimeRequired", model.ActualTimeRequired == 0 ? 0 : model.ActualTimeRequired));
            SqlParams.Add(new SqlParameter("@BatchNo", model.BatchNo == null ? "" : model.BatchNo));
            SqlParams.Add(new SqlParameter("@UniqueBatchNo", model.UniqueBatchNo == null ? "" : model.UniqueBatchNo));
            SqlParams.Add(new SqlParameter("@parentProdSchNo", model.parentProdSchNo == null ? "" : model.parentProdSchNo));
            SqlParams.Add(new SqlParameter("@parentProdSchDate", parentprodschDt == default ? string.Empty : parentprodschDt));
            SqlParams.Add(new SqlParameter("@parentProdSchYearcode", model.ParentProdSchYear == 0 ? 0 : model.StdCycleTime));
            SqlParams.Add(new SqlParameter("@parentProdSchItemCode", model.parentProdSchItemCode == 0 ? 0 : model.parentProdSchItemCode));
            SqlParams.Add(new SqlParameter("@SONO", model.SONo == null ? "" : model.SONo));
            SqlParams.Add(new SqlParameter("@SOYearcode", model.SoYear == 0 ? 0 : model.SoYear));
            SqlParams.Add(new SqlParameter("@SODate", SoDt == default ? string.Empty : SoDt));
            SqlParams.Add(new SqlParameter("@sotype", model.sotype == null ? "" : model.sotype));
            SqlParams.Add(new SqlParameter("@QCOffered", model.QCOffered == null ? "" : model.QCOffered));
            SqlParams.Add(new SqlParameter("@QCOfferDate", qcofferDt == default ? string.Empty : qcofferDt));
            SqlParams.Add(new SqlParameter("@recbyempid", model.recbyempid == 0 ? 0 : model.recbyempid));
            SqlParams.Add(new SqlParameter("@QCQTy", model.QCQTy == 0 ? 0 : model.QCQTy));
            SqlParams.Add(new SqlParameter("@OKQty", model.OKQty == 0 ? 0 : model.OKQty));
            SqlParams.Add(new SqlParameter("@RejQTy", model.QcRejQty == 0 ? 0 : model.QcRejQty));
            SqlParams.Add(new SqlParameter("@StockQTy", model.Stock == 0 ? 0 : model.Stock));
            SqlParams.Add(new SqlParameter("@matTransferd", model.MaterialTransferd == null ? "" : model.MaterialTransferd));
            SqlParams.Add(new SqlParameter("@RewQcEntryId", model.RewQcEntryId == 0 ? 0 : model.RewQcEntryId));
            SqlParams.Add(new SqlParameter("@RewQcYearCode", model.RewQcYearCode == 0 ? 0 : model.RewQcYearCode));
            SqlParams.Add(new SqlParameter("@RewQcDate", rewqcDt == default ? string.Empty : rewqcDt));
            SqlParams.Add(new SqlParameter("@shiftclose", model.ShiftClosed == null ? "" : model.ShiftClosed));
            SqlParams.Add(new SqlParameter("@ComplProd", model.ComplProd == null ? "" : model.ComplProd));
            SqlParams.Add(new SqlParameter("@Uid", model.Uid == 0 ? 0 : model.Uid));
            SqlParams.Add(new SqlParameter("@CC", model.CC == null ? "" : model.CC));
            SqlParams.Add(new SqlParameter("@EntryByMachineNo", model.EntrybyMachineName == null ? "" : model.EntrybyMachineName));
            SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEnteredBy == 0 ? 0 : model.ActualEnteredBy));
            SqlParams.Add(new SqlParameter("@ActualEntryDate", actualDt == default ? string.Empty : actualDt));
            SqlParams.Add(new SqlParameter("@EntryByDesignation", model.EntryByDesignation == null ? "" : model.EntryByDesignation));
            SqlParams.Add(new SqlParameter("@operator", model.Operator == null ? "" : model.Operator));
            SqlParams.Add(new SqlParameter("@supervisior", model.Superwiser == null ? "" : model.Superwiser));

            SqlParams.Add(new SqlParameter("@DTItemGrid", GIGrid));
            SqlParams.Add(new SqlParameter("@DTBreakGrid", BreakDownGrid));
            SqlParams.Add(new SqlParameter("@DtOperatorGrid", OperatorGrid));
            SqlParams.Add(new SqlParameter("@DTScrapGrid", ScrapGrid));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ProductionEntryDashboard> GetDashboardData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ProdSlipNo", SlipNo);
                oCmd.Parameters.AddWithValue("@Itemname", ItemName);
                oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                oCmd.Parameters.AddWithValue("@Reqno", ReqNo);

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 PRODEntryId = Convert.ToInt32(dr["PRODEntryId"]),
                                                 PRODYearcode = Convert.ToInt32(dr["PRODYearcode"]),
                                                 Entrydate = dr["Entrydate"].ToString().Split(" ")[0],
                                                 ProdAgainstPlanManual = dr["ProdAgainstPlanManual"].ToString(),
                                                 NewProdRework = dr["NewProdRework"].ToString(),
                                                 ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                 ProdDate = dr["ProdDate"].ToString(),
                                                 ProdPlanNo = dr["ProdPlanNo"].ToString(),
                                                 ProdPlanYearCode = Convert.ToInt32(dr["ProdPlanYearCode"]),
                                                 ProdPlanDate = dr["ProdPlanDate"].ToString(),
                                                 ProdPlanSchNo = dr["ProdPlanSchNo"].ToString(),
                                                 ProdPlanSchYearCode = Convert.ToInt32(dr["ProdPlanSchYearCode"]),
                                                 ProdPlanSchDate = dr["ProdPlanSchDate"].ToString(),
                                                 Reqno = dr["Reqno"].ToString(),
                                                 ReqThrBOMYearCode = Convert.ToInt32(dr["ReqThrBOMYearCode"]),
                                                 ReqDate = dr["ReqDate"].ToString(),
                                                 FGPartCode = dr["FGPartCode"].ToString(),
                                                 FGItemName = dr["FGItemName"].ToString(),
                                                 WOQTY = Convert.ToDecimal(dr["WOQTY"]),
                                                 ProdSchQty = Convert.ToDecimal(dr["ProdSchQty"]),
                                                 FGProdQty = Convert.ToDecimal(dr["FGProdQty"]),
                                                 FGOKQty = Convert.ToDecimal(dr["FGOKQty"]),
                                                 FGRejQty = Convert.ToDecimal(dr["FGRejQty"]),
                                                 NextToStore = dr["NextToStore"].ToString(),
                                                 NextToWorkCenter = dr["NextToWorkCenter"].ToString(),
                                                 RejQtyDuetoTrail = Convert.ToDecimal(dr["RejQtyDuetoTrail"]),
                                                 PendQtyForProd = Convert.ToDecimal(dr["PendQtyForProd"]),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"]),
                                                 QCChecked = dr["QCChecked"].ToString(),
                                                 QCQTy = Convert.ToDecimal(dr["QCQty"]),
                                                 OKQty = Convert.ToDecimal(dr["OKQty"]),
                                                 RejQTy = Convert.ToDecimal(dr["RejQty"]),
                                                 StockQTy = Convert.ToDecimal(dr["StockQty"]),
                                                 matTransferd = dr["matTransferd"].ToString(),
                                                 PendingQtyToIssue = Convert.ToDecimal(dr["PendingQtyToIssue"]),
                                                 BOMNO = Convert.ToInt32(dr["BOMNO"]),
                                                 BOMDate = dr["BOMDate"].ToString(),
                                                 MachineName = dr["MachineName"].ToString(),
                                                 StageDescription = dr["StageDescription"].ToString(),
                                                 ProdInWC = dr["ProdInWC"].ToString(),
                                                 RejQtyInWC = dr["RejQtyInWC"].ToString(),
                                                 rejStore = dr["rejStore"].ToString(),
                                                 TransferFGToWCorSTORE = dr["TransferFGToWCorSTORE"].ToString(),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 TransferToQc = dr["TransferToQc"].ToString(),
                                                 StartTime = dr["StartTime"].ToString(),
                                                 ToTime = dr["StartTime"].ToString(),
                                                 setupTime = Convert.ToDecimal(dr["setupTime"]),
                                                 PrevWC = Convert.ToInt32(dr["PrevWC"]),
                                                 ProducedINLineNo = dr["ProducedINLineNo"].ToString(),
                                                 InitialReading = Convert.ToDecimal(dr["InitialReading"]),
                                                 FinalReading = Convert.ToDecimal(dr["FinalReading"]),
                                                 Shots = Convert.ToInt32(dr["Shots"]),
                                                 Completed = dr["Completed"].ToString(),
                                                 UtilisedHours = Convert.ToDecimal(dr["UtilisedHours"]),
                                                 SODate = dr["SODate"].ToString(),
                                                 ProdLineNo = dr["ProdLineNo"].ToString(),
                                                 stdShots = Convert.ToInt32(dr["stdShots"]),
                                                 stdCycletime = Convert.ToInt32(dr["stdCycletime"]),
                                                 Remark = dr["Remark"].ToString(),
                                                 CyclicTime = Convert.ToDecimal(dr["CyclicTime"]),
                                                 ProductionHour = Convert.ToInt32(dr["ProductionHour"]),
                                                 ItemModel = dr["ItemModel"].ToString(),
                                                 cavity = Convert.ToInt32(dr["cavity"]),
                                                 startupRejQty = Convert.ToDecimal(dr["startupRejQty"]),
                                                 efficiency = Convert.ToDecimal(dr["efficiency"]),
                                                 ActualTimeRequired = Convert.ToDecimal(dr["ActualTimeRequired"]),
                                                 BatchNo = dr["Batchno"].ToString(),
                                                 UniqueBatchNo = dr["Uniquebatchno"].ToString(),
                                                 parentProdSchNo = dr["parentProdSchNo"].ToString(),
                                                 parentProdSchDate = dr["parentProdSchDate"].ToString(),
                                                 parentProdSchYearcode = Convert.ToInt32(dr["parentProdSchYearcode"]),
                                                 SONO = dr["SONO"].ToString(),
                                                 SOYearcode = Convert.ToInt32(dr["SOYearcode"]),
                                                 sotype = dr["sotype"].ToString(),
                                                 QCOffered = dr["QCOffered"].ToString(),
                                                 QCOfferDate = dr["QCOfferDate"].ToString(),
                                                 RewQcYearCode = Convert.ToInt32(dr["RewQcYearCode"]),
                                                 RewQcDate = dr["ItemModel"].ToString(),
                                                 shiftclose = dr["shiftclose"].ToString(),
                                                 ComplProd = dr["ComplProd"].ToString(),
                                                 CC = dr["CC"].ToString(),
                                                 EntryByMachineNo = dr["EntryByMachineNo"].ToString(),
                                                 ActualEntryByEmp = Convert.ToInt32(dr["ActualEntryByEmp"]),
                                                 ActualEmpByName = dr["ActualEmpByName"].ToString(),
                                                 ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                 LastUpdationDate = dr["LastUpdationDate"].ToString(),
                                                 LastUpdatedByName = dr["LastUpdatedByName"].ToString(),
                                                 EntryByDesignation = dr["EntryByDesignation"].ToString(),
                                                 OperatorName = dr["operatorName"].ToString(),
                                                 supervisior = dr["supervisior"].ToString(),
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
    public async Task<ProductionEntryDashboard> GetDashboardDetailData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ProdSlipNo", SlipNo);
                oCmd.Parameters.AddWithValue("@FGItemNAme", ItemName);
                oCmd.Parameters.AddWithValue("@FGPartCode", PartCode);
                oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                oCmd.Parameters.AddWithValue("@Reqno", ReqNo);

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 NewProdRework = dr["NewProdRework"].ToString(),
                                                 ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                 ProdDate = dr["ProdDate"].ToString(),
                                                 FGPartCode = dr["FGPartCode"].ToString(),
                                                 FGItemName = dr["FGItemName"].ToString(),
                                                 WorkCenter = dr["Workcenter"].ToString(),
                                                 ProdQty = Convert.ToDecimal(dr["ProdQty"]),
                                                 Unit = dr["FGUnit"].ToString(),
                                                 RMPartCode = dr["RMPartCode"].ToString(),
                                                 RMItemName = dr["RMItemName"].ToString(),
                                                 ConsumedRMQTY = Convert.ToInt32(dr["ConsumedRMQTY"]),
                                                 ConsumedRMUnit = dr["RMunit"].ToString(),
                                                 MainRMQTY = Convert.ToDecimal(dr["MainRMQTY"]),
                                                 MainRMUnit = dr["MainRMUnit"].ToString(),
                                                 TotalReqRMQty = Convert.ToDecimal(dr["TotalReqRMQty"]),
                                                 TotalStock = Convert.ToDecimal(dr["TotalStock"]),
                                                 AltRMQty = Convert.ToDecimal(dr["AltRMQty"]),
                                                 AltRMUnit = dr["AltRMUnit"].ToString(),
                                                 RMNetWt = Convert.ToDecimal(dr["RMNetWt"]),
                                                 GrossWt = Convert.ToDecimal(dr["GrossWt"]),
                                                 BOMNO = Convert.ToInt32(dr["BOMRevNO"]),
                                                 BOMRevDate = dr["BOMRevDate"].ToString(),
                                                 ManualAutoEntry = dr["ManualAutoEntry"].ToString(),
                                                 EntryId = Convert.ToInt32(dr["Entryid"]),
                                                 Yearcode = Convert.ToInt32(dr["Yearcode"]),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"])
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
    public async Task<ProductionEntryDashboard> GetBatchwiseDetail(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "DETAILDASHBOARD(BatchWise)");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ProdSlipNo", SlipNo);
                oCmd.Parameters.AddWithValue("@FGItemNAme", ItemName);
                oCmd.Parameters.AddWithValue("@FGPartCode", PartCode);
                oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                oCmd.Parameters.AddWithValue("@Reqno", ReqNo);

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 NewProdRework = dr["NewProdRework"].ToString(),
                                                 ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                 ProdDate = dr["ProdDate"].ToString(),
                                                 FGPartCode = dr["FGPartCode"].ToString(),
                                                 FGItemName = dr["FGItemName"].ToString(),
                                                 WorkCenter = dr["Workcenter"].ToString(),
                                                 ProdQty = Convert.ToDecimal(dr["ProdQty"]),
                                                 RMPartCode = dr["RMPartCode"].ToString(),
                                                 RMItemName = dr["RMItemName"].ToString(),
                                                 Unit = dr["FGUnit"].ToString(),
                                                 ConsumedRMQTY = Convert.ToInt32(dr["ConsumedRMQTY"]),
                                                 ConsumedRMUnit = dr["ConsumedRMUnit"].ToString(),
                                                 MainRMQTY = Convert.ToDecimal(dr["MainRMQTY"]),
                                                 MainRMUnit = dr["MainRMUnit"].ToString(),
                                                 FGProdQty = Convert.ToDecimal(dr["FGProdQty"]),
                                                 TotalReqRMQty = Convert.ToDecimal(dr["TotalReqRMQty"]),
                                                 TotalStock = Convert.ToDecimal(dr["TotalStock"]),
                                                 BatchStock = Convert.ToDecimal(dr["BatchStock"]),
                                                 AltRMQty = Convert.ToDecimal(dr["AltRMQty"]),
                                                 AltRMUnit = dr["AltRMUnit"].ToString(),
                                                 RMNetWt = Convert.ToDecimal(dr["RMNetWt"]),
                                                 GrossWt = Convert.ToDecimal(dr["GrossWt"]),
                                                 Batchwise = dr["Batchwise"].ToString(),
                                                 BatchNo = dr["BatchNo"].ToString(),
                                                 UniqueBatchNo = dr["UniqueBatchNo"].ToString(),
                                                 BOMNO = Convert.ToInt32(dr["BOMRevNO"]),
                                                 BOMRevDate = dr["BOMRevDate"].ToString(),
                                                 ManualAutoEntry = dr["ManualAutoEntry"].ToString(),
                                                 EntryId = Convert.ToInt32(dr["Entryid"]),
                                                 Yearcode = Convert.ToInt32(dr["PRODYearcode"]),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"])
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
    public async Task<ProductionEntryDashboard> GetBreakdownData(string FromDate, string ToDate)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "BREAKDOWNDASHBOARD");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 Yearcode = Convert.ToInt32(dr["Yearcode"]),
                                                 Proddate = dr["Proddate"].ToString(),
                                                 BreakfromTime = dr["BreakfromTime"].ToString(),
                                                 BreaktoTime = dr["BreaktoTime"].ToString(),
                                                 ResonDetail = dr["ResonDetail"].ToString(),
                                                 BreakTimeMin = Convert.ToDecimal(dr["BreakTimeMin"]),
                                                 ResEmpName = dr["ResEmpName"].ToString(),
                                                 ResFactor = dr["ResFactor"].ToString(),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"])
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
    public async Task<ProductionEntryDashboard> GetOperationData(string FromDate, string ToDate)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "OPERATORDASHBOARD");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 EntryId = Convert.ToInt32(dr["EntryId"]),
                                                 Yearcode = Convert.ToInt32(dr["Yearcode"]),
                                                 NewProdRework = dr["Prod/Rework"].ToString(),
                                                 ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                 ProdDate = dr["ProdDate"].ToString(),
                                                 FGPartCode = dr["FGPartCode"].ToString(),
                                                 FGItemName = dr["FGItemName"].ToString(),
                                                 WorkCenter = dr["Workcenter"].ToString(),
                                                 MachineName = dr["MachineName"].ToString(),
                                                 OperatorName = dr["OperatorName"].ToString(),
                                                 Fromtime = dr["Fromtime"].ToString(),
                                                 totime = dr["totime"].ToString(),
                                                 TotalHrs = Convert.ToDecimal(dr["TotalHrs"]),
                                                 OverTimeHrs = Convert.ToDecimal(dr["OverTimeHrs"]),
                                                 MachineCharges = Convert.ToDecimal(dr["MachineCharges"]),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"]),

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
    public async Task<ProductionEntryDashboard> GetScrapData(string FromDate, string ToDate)
    {
        DataSet? oDataSet = new DataSet();
        var model = new ProductionEntryDashboard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_ProductionEntry", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oCmd.Parameters.AddWithValue("@Flag", "SCRAPDASHBOARD");
                oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.ProductionDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new ProductionEntryDashboard
                                             {
                                                 EntryId = Convert.ToInt32(dr["Entryid"]),
                                                 Yearcode = Convert.ToInt32(dr["Yearcode"]),
                                                 NewProdRework = dr["Prod/Rework"].ToString(),
                                                 ProdSlipNo = dr["ProdSlipNo"].ToString(),
                                                 ProdDate = dr["ProdDate"].ToString(),
                                                 FGPartCode = dr["FGPartCode"].ToString(),
                                                 FGItemName = dr["FGItemName"].ToString(),
                                                 WorkCenter = dr["Workcenter"].ToString(),
                                                 MachineName = dr["MachineName"].ToString(),
                                                 ScrapType = dr["ScrapType"].ToString(),
                                                 ScrapQty = Convert.ToDecimal(dr["ScrapQty"]),
                                                 Scrapunit = dr["Scrapunit"].ToString(),
                                                 QCMandatory = dr["QCMandatory"].ToString(),
                                                 PendQtyForQC = Convert.ToDecimal(dr["PendQtyForQC"])
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
    public async Task<ProductionEntryModel> GetViewByID(int ID, int YearCode)
    {
        var model = new ProductionEntryModel();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@flag", "ViewBYID"));
            SqlParams.Add(new SqlParameter("@Entryid", ID));
            SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
            var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ProductionEntry", SqlParams);

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
            else if (doctype == 11)
            {
                SqlParams.Add(new SqlParameter("@Flag", "CHALLAN"));
            }

            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));

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
    public async Task<ResponseResult> FillChallanQty(int AccountCode, int ItemCode, string ChallanNo)
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
    public async Task<ResponseResult> FillSaleBillQty(int AccountCode, int ItemCode, int SaleBillNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            if (SaleBillNo != 0)
            {
                SqlParams.Add(new SqlParameter("@Flag", "SALEBILLQty"));
            }

            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

            SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@SaleBillNo", SaleBillNo));
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
    public async Task<ResponseResult> CheckEditOrDelete(string ProdSlipNo, int ProdYearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
            SqlParams.Add(new SqlParameter("@Entryid", ProdSlipNo));
            SqlParams.Add(new SqlParameter("@YearCode", ProdYearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ProductionEntry", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetScheDuleByYearCodeandAccountCode(string Flag, string AccountCode, string YearCode, string poNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));

            SqlParams.Add(new SqlParameter("@ItemService", "Item"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            if (Flag == "PURCHSCHEDULE")
            {
                SqlParams.Add(new SqlParameter("@POYearCode", YearCode));
            }
            SqlParams.Add(new SqlParameter("@PONO", poNo));

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
    private static ProductionEntryModel PrepareView(DataSet DS, ref ProductionEntryModel? model)
    {
        try
        {
            var ItemList = new List<ProductionEntryItemDetail>();
            var BreakdownList = new List<ProductionEntryItemDetail>();
            var OperatorList = new List<ProductionEntryItemDetail>();
            var ScrapList = new List<ProductionEntryItemDetail>();
            DS.Tables[0].TableName = "SSMain";
            DS.Tables[1].TableName = "SSDetail";
            DS.Tables[2].TableName = "BreakDetail";
            DS.Tables[3].TableName = "OperatorDetail";
            DS.Tables[4].TableName = "ScrapDetail";
            int cnt = 0;
            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["PRODEntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["PRODYearcode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["Entrydate"].ToString();
            model.ProdAgainstPlanManual = DS.Tables[0].Rows[0]["ProdAgainstPlanManual"].ToString();
            model.ProdType = DS.Tables[0].Rows[0]["NewProdRework"].ToString();
            model.ShiftId = Convert.ToInt32(DS.Tables[0].Rows[0]["ShiftId"].ToString());
            model.ProdSlipNo = DS.Tables[0].Rows[0]["ProdSlipNo"].ToString();
            model.ActualProdDate = DS.Tables[0].Rows[0]["ProdDate"].ToString();
            model.ProdPlan = DS.Tables[0].Rows[0]["ProdPlanNo"].ToString();
            model.ProdPlanYear = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdPlanYearCode"].ToString());
            model.ProdPlanDate = DS.Tables[0].Rows[0]["ProdPlanDate"].ToString();
            model.ProdSch = DS.Tables[0].Rows[0]["ProdPlanSchNo"].ToString();
            model.ProdSchYear = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdPlanSchYearCode"].ToString());
            model.ProdSchDate = DS.Tables[0].Rows[0]["ProdPlanSchDate"].ToString();
            model.ReqNo = DS.Tables[0].Rows[0]["Reqno"].ToString();
            model.ReqYear = Convert.ToInt32(DS.Tables[0].Rows[0]["ReqYearcode"].ToString());
            model.ReqDate = DS.Tables[0].Rows[0]["ReqDate"].ToString();
            model.PartCode = DS.Tables[0].Rows[0]["FGPartCode"].ToString();
            model.ItemName = DS.Tables[0].Rows[0]["FGItemName"].ToString();
            model.FGItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["FGItemCode"].ToString());
            model.WOQTY = Convert.ToDecimal(DS.Tables[0].Rows[0]["WOQTY"].ToString());
            if (model.ProdAgainstPlanManual == "UNPLANNED")
            {
                model.TargetQtyUnplanned = Convert.ToDecimal(DS.Tables[0].Rows[0]["ProdSchQty"].ToString());
            }
            else
            {
                model.TargetQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["ProdSchQty"].ToString());
            }
            model.ProdQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["FGProdQty"].ToString());
            model.FGOkQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["FGOKQty"].ToString());
            model.FGRejQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["FGRejQty"].ToString());
            model.RejQtyDuetoTrail = Convert.ToDecimal(DS.Tables[0].Rows[0]["RejQtyDuetoTrail"].ToString());
            model.PendQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["PendQtyForProd"].ToString());
            model.PendQtyForQC = Convert.ToDecimal(DS.Tables[0].Rows[0]["PendQtyForQC"].ToString());
            model.PendingQtyToIssue = Convert.ToDecimal(DS.Tables[0].Rows[0]["PendingQtyToIssue"].ToString());
            model.BOMNo = Convert.ToInt32(DS.Tables[0].Rows[0]["BOMNO"].ToString());
            model.BOMRevDate = DS.Tables[0].Rows[0]["BOMDate"].ToString();
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"].ToString();
            model.MachineId = Convert.ToInt32(DS.Tables[0].Rows[0]["MachineId"].ToString());
            model.OperatorName = DS.Tables[0].Rows[0]["StageDescription"].ToString();
            model.ProcessId = Convert.ToInt32(DS.Tables[0].Rows[0]["ProcessId"].ToString());
            model.ProdInWCID = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdInWCID"].ToString());
            model.StoreTransfer = DS.Tables[0].Rows[0]["TransferRejtoStoreWC"].ToString();
            model.WorkCenter = DS.Tables[0].Rows[0]["ProdInWC"].ToString();
            model.RejWCId = Convert.ToInt32(DS.Tables[0].Rows[0]["RejWCId"].ToString());
            model.WorkCenter = DS.Tables[0].Rows[0]["RejQtyInWC"].ToString();
            model.TransferToRejStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["TransferToRejStoreId"].ToString());
            model.StoreName = DS.Tables[0].Rows[0]["rejStore"].ToString();
            model.Store = DS.Tables[0].Rows[0]["TransferFGToWCorSTORE"].ToString();
            model.QCMandatory = DS.Tables[0].Rows[0]["QCMandatory"].ToString();
            model.SenToQc = DS.Tables[0].Rows[0]["TransferToQc"].ToString();
            model.NextWCID = Convert.ToInt32(DS.Tables[0].Rows[0]["NextWCID"].ToString());
            model.NextStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["NextStoreId"].ToString());
            model.StartTime = DS.Tables[0].Rows[0]["StartTime"].ToString();
            model.ToTime = DS.Tables[0].Rows[0]["ToTime"].ToString();
            model.setupTime = Convert.ToInt32(DS.Tables[0].Rows[0]["setupTime"].ToString());
            model.PrevWC = Convert.ToInt32(DS.Tables[0].Rows[0]["PrevWC"].ToString());
            model.PrevProcessId = Convert.ToInt32(DS.Tables[0].Rows[0]["PrevProcessId"].ToString());
            model.ProducedINLineNo = DS.Tables[0].Rows[0]["ProducedINLineNo"].ToString();
            model.QCChecked = DS.Tables[0].Rows[0]["QCChecked"].ToString();
            model.InitalReading = Convert.ToDecimal(DS.Tables[0].Rows[0]["InitialReading"].ToString());
            model.FinalReading = Convert.ToDecimal(DS.Tables[0].Rows[0]["FinalReading"].ToString());
            model.Shots = Convert.ToInt32(DS.Tables[0].Rows[0]["Shots"].ToString());
            model.Completed = DS.Tables[0].Rows[0]["Completed"].ToString();
            model.UtilisedHours = Convert.ToInt32(DS.Tables[0].Rows[0]["UtilisedHours"].ToString());
            model.ProdSeqNo = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdLineNo"].ToString());
            model.stdShots = Convert.ToInt32(DS.Tables[0].Rows[0]["stdShots"].ToString());
            model.StdCycleTime = Convert.ToInt32(DS.Tables[0].Rows[0]["stdCycletime"].ToString());
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            model.CyclicTime = Convert.ToDecimal(DS.Tables[0].Rows[0]["CyclicTime"].ToString());
            model.ProductionHour = Convert.ToDecimal(DS.Tables[0].Rows[0]["ProductionHour"].ToString());
            model.ItemModel = DS.Tables[0].Rows[0]["ItemModel"].ToString();
            model.Cavity = Convert.ToInt32(DS.Tables[0].Rows[0]["cavity"].ToString());
            model.StartupRej = Convert.ToDecimal(DS.Tables[0].Rows[0]["startupRejQty"].ToString());
            model.efficiency = Convert.ToDecimal(DS.Tables[0].Rows[0]["efficiency"].ToString());
            model.ActualTimeRequired = Convert.ToDecimal(DS.Tables[0].Rows[0]["ActualTimeRequired"].ToString());
            model.BatchNo = DS.Tables[0].Rows[0]["BatchNo"].ToString();
            model.UniqueBatchNo = DS.Tables[0].Rows[0]["UniqueBatchNo"].ToString();
            model.parentProdSchNo = DS.Tables[0].Rows[0]["parentProdSchNo"].ToString();
            model.parentProdSchDate = DS.Tables[0].Rows[0]["parentProdSchDate"].ToString();
            model.ParentProdSchYear = Convert.ToInt32(DS.Tables[0].Rows[0]["parentProdSchYearcode"].ToString());
            model.parentProdSchItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["parentProdSchItemCode"].ToString());
            model.SONo = DS.Tables[0].Rows[0]["SONO"].ToString();
            model.SoYear = Convert.ToInt32(DS.Tables[0].Rows[0]["SOYearcode"].ToString());
            model.SoDate = DS.Tables[0].Rows[0]["SODate"].ToString();
            model.sotype = DS.Tables[0].Rows[0]["sotype"].ToString();
            model.QCOffered = DS.Tables[0].Rows[0]["QCOffered"].ToString();
            model.QcOfferedDate = DS.Tables[0].Rows[0]["QCOfferDate"].ToString();
            model.recbyempid = Convert.ToInt32(DS.Tables[0].Rows[0]["recbyempid"].ToString());
            model.QCQTy = Convert.ToDecimal(DS.Tables[0].Rows[0]["QCQTy"].ToString());
            model.OKQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["OKQty"].ToString());
            model.QcRejQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["RejQTy"].ToString());
            model.StockQTy = Convert.ToDecimal(DS.Tables[0].Rows[0]["StockQTy"].ToString());
            model.MaterialTransferd = DS.Tables[0].Rows[0]["matTransferd"].ToString();
            model.RewQcEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["RewQcEntryId"].ToString());
            model.RewQcYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RewQcYearCode"].ToString());
            model.RewQcDate = DS.Tables[0].Rows[0]["RewQcDate"].ToString();
            model.ShiftClosed = DS.Tables[0].Rows[0]["shiftclose"].ToString();
            model.ComplProd = DS.Tables[0].Rows[0]["ComplProd"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"].ToString());
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.EntryByMachineNo = DS.Tables[0].Rows[0]["EntryByMachineNo"].ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmpid"].ToString());
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpid"].ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.EntryByDesignation = DS.Tables[0].Rows[0]["EntryByDesignation"].ToString();
            model.OperatorName = DS.Tables[0].Rows[0]["operator"].ToString();
            model.supervisior = DS.Tables[0].Rows[0]["supervisior"].ToString();

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["LastUpdationDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]);
            }
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new ProductionEntryItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        EntryId = Convert.ToInt32(row["PRODEntryid"].ToString()),
                        YearCode = Convert.ToInt32(row["PRODYearcode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemcode"].ToString()),
                        FGPartCode = (row["FGPartCode"].ToString()),
                        FGItemName = row["FGItemName"].ToString(),
                        ConsumedRMItemCode = Convert.ToInt32(row["ConsumedRMitemCode"].ToString()),
                        PartCode = row["ConPartCode"].ToString(),
                        ItemName = row["ConsItemName"].ToString(),
                        IssueQty = Convert.ToDecimal(row["ConsumedRMQTY"].ToString()),
                        Unit = row["ConsumedRMUnit"].ToString(),
                        MainPartCode = row["MainPartCode"].ToString(),
                        MainItemName = row["MainItemName"].ToString(),
                        MainRMQTY = Convert.ToDecimal(row["MainRMQTY"].ToString()),
                        AltUnit = row["MainRMUnit"].ToString(),
                        ProdQty = Convert.ToDecimal(row["FGProdQty"].ToString()),
                        FGUnit = row["FGUnit"].ToString(),
                        ReqQty = Convert.ToDecimal(row["TotalReqRMQty"].ToString()),
                        TotalStock = Convert.ToDecimal(row["TotalStock"].ToString()),
                        BatchStock = Convert.ToDecimal(row["BatchStock"].ToString()),
                        WCId = Convert.ToInt32(row["WCId"].ToString()),
                        AltRMItemCode = Convert.ToInt32(row["AltRMItemcode"].ToString()),
                        AltRMQty = Convert.ToDecimal(row["AltRMQty"].ToString()),
                        RMNetWt = Convert.ToDecimal(row["RMNetWt"].ToString()),
                        GrossWt = Convert.ToDecimal(row["GrossWt"].ToString()),
                        AltRMUnit = row["AltRMUnit"].ToString(),
                        BatchWise = row["Batchwise"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo = (row["UniqueBatchNo"].ToString()),
                        BOMRevNO = Convert.ToInt32(row["BOMRevNO"].ToString()),
                        BatchDate = (row["BOMRevDate"].ToString()),
                        ManualAutoEntry = (row["ManualAutoEntry"].ToString()),
                        PendQty = Convert.ToDecimal(row["FGProdQty"].ToString()),
                        RemainingStock = Convert.ToDecimal(row["TotalStock"].ToString()) - Convert.ToDecimal(row["FGProdQty"].ToString()),
                    });
                }
                model.ItemDetailGrid = ItemList;
            }
            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    BreakdownList.Add(new ProductionEntryItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        EntryId = Convert.ToInt32(row["PRODEntryid"].ToString()),
                        YearCode = Convert.ToInt32(row["PRODYearcode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemcode"].ToString()),
                        FGPartCode = (row["FGPartCode"].ToString()),
                        FGItemName = row["FGItemName"].ToString(),
                        ProdDate = row["Proddate"].ToString(),
                        WCId = Convert.ToInt32(row["WCId"].ToString()),
                        BreakfromTime = row["BreakfromTime"].ToString(),
                        BreaktoTime = row["BreaktoTime"].ToString(),
                        ReasonId = Convert.ToInt32(row["ReasonId"].ToString()),
                        BreakTimeMin = Convert.ToDecimal(row["BreakTimeMin"].ToString()),
                        ReasonDetail = row["ResonDetail"].ToString(),
                        ResponcibleEmp = Convert.ToInt32(row["ResponcibleEmp"].ToString()),
                        ResEmpName = row["ResEmpName"].ToString(),
                        ResFactor = row["ResFactor"].ToString(),
                    });
                }
                model.BreakdownDetailGrid = BreakdownList;
            }
            if (DS.Tables.Count != 0 && DS.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[3].Rows)
                {
                    OperatorList.Add(new ProductionEntryItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        EntryId = Convert.ToInt32(row["PRODEntryid"].ToString()),
                        YearCode = Convert.ToInt32(row["PRODYearcode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemcode"].ToString()),
                        FGPartCode = (row["FGPartCode"].ToString()),
                        FGItemName = row["FGItemName"].ToString(),
                        EntryDate = row["EntryDate"].ToString(),
                        FGProdQty = Convert.ToDecimal(row["FGProdQty"].ToString()),
                        McId = Convert.ToInt32(row["McId"].ToString()),
                        ProcessId = Convert.ToInt32(row["ProcessId"].ToString()),
                        OperatorId = Convert.ToInt32(row["OperatorId"].ToString()),
                        OperatorName = row["OperatorName"].ToString(),
                        Fromtime = row["Fromtime"].ToString(),
                        Totime = row["totime"].ToString(),
                        TotalHours = Convert.ToDecimal(row["TotalHrs"].ToString()),
                        OverTimeHrs = Convert.ToDecimal(row["OverTimeHrs"].ToString()),
                        MachineCharges = Convert.ToDecimal(row["MachineCharges"].ToString()),
                    });
                }
                model.OperatorDetailGrid = OperatorList;
            }
            if (DS.Tables.Count != 0 && DS.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[4].Rows)
                {
                    ScrapList.Add(new ProductionEntryItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        EntryId = Convert.ToInt32(row["PRODEntryid"].ToString()),
                        YearCode = Convert.ToInt32(row["PRODYearcode"].ToString()),
                        FGItemCode = Convert.ToInt32(row["FGItemcode"].ToString()),
                        ScrapPartCode = (row["FGPartCode"].ToString()),
                        ScrapItemName = row["FGItemName"].ToString(),
                        EntryDate = row["EntryDate"].ToString(),
                        FGProdQty = Convert.ToDecimal(row["FGProdQty"].ToString()),
                        FGUnit = row["FGUnit"].ToString(),
                        ScrapItemCode = Convert.ToInt32(row["ScrapItemCode"].ToString()),
                        BOMRevNO = Convert.ToInt32(row["BOMRevNo"].ToString()),
                        ScrapQty = Convert.ToDecimal(row["ScrapQty"].ToString()),
                        BOMRevDate = row["BOMEFFDate"].ToString(),
                        ScrapType = row["ScrapType"].ToString(),
                        Scrapunit = row["Scrapunit"].ToString(),
                        TransferToWCStore = row["TransferToWCStore"].ToString(),
                        TransferToStoreId = Convert.ToInt32(row["TransferToStoreId"].ToString()),
                        TransferToWC = Convert.ToInt32(row["TransferToWCId"].ToString()),
                    });
                }
                model.ScrapDetailGrid = ScrapList;
            }
            return model;
        }
        catch (Exception ex)
        {
            throw;
        }
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
    public async Task<ResponseResult> GetTimeDiff(string Flag, string ToTime, string DiffType, string FromTime)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@flag", "getNoofMHD"));
            SqlParams.Add(new SqlParameter("@todatetime", ToTime));
            SqlParams.Add(new SqlParameter("@DiffType", DiffType));
            SqlParams.Add(new SqlParameter("@FromDatetime", FromTime));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("Sp_getTimediff", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetDateforBreakdown(string Flag, string DiffType, string QtyOfTime, string FromTime)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@flag", "GetToDate"));
            SqlParams.Add(new SqlParameter("@QtyOfTime", QtyOfTime));
            SqlParams.Add(new SqlParameter("@DiffType", DiffType));
            SqlParams.Add(new SqlParameter("@FromDatetime", FromTime));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("Sp_getTimediff", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPoNumberDropDownList(string Flag, string ServiceType, string SPName, string AccountCode, int Year, int DocTypeId)
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



    public async Task<PendingProductionEntryModel> GetPendingProductionEntry(int Yearcode)
    {
        var resultList = new PendingProductionEntryModel();
        DataSet oDataSet = new DataSet();

        try
        {
            using (SqlConnection connection = new SqlConnection(DBConnectionString))
            {
                SqlCommand command = new SqlCommand("SP_ProductionEntry", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ProdAgainstReqPlanDirect", "PendProductionSchDetailPopup");
                command.Parameters.AddWithValue("@Yearcode", Yearcode);

                await connection.OpenAsync();

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                resultList.PendingProductionEntryGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new PendingProductionEntryModel
                                                         {
                                                             PlanNo = row["PlanNo"] == DBNull.Value ? string.Empty : row["PlanNo"].ToString(),
                                                             PlanNoDate = row["PlanNoDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PlanNoDate"]).ToString("dd-MM-yyyy"),
                                                             ProdSchNo = row["ProdSchNo"] == DBNull.Value ? string.Empty : row["ProdSchNo"].ToString(),
                                                             ProdSchdate = row["ProdSchdate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ProdSchdate"]).ToString("dd-MM-yyyy"),
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             Item_Name = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),
                                                             Item_Code = Convert.ToInt32(row["Item_Code"]),
                                                             Qty = row["Qty"] == DBNull.Value ? string.Empty : row["Qty"].ToString(),
                                                             PlanNoEntryId = row["PlanNoEntryId"] == DBNull.Value ? string.Empty : row["PlanNoEntryId"].ToString(),
                                                             PlanNoYearCode = Convert.ToInt32(row["PlanNoYearCode"]),
                                                             ProdSchEntryid = row["ProdSchEntryid"] == DBNull.Value ? string.Empty : row["ProdSchEntryid"].ToString(),
                                                             ProdSchYearCode = Convert.ToInt32(row["ProdSchYearCode"]),
                                                             WorkCenter = row["WorkCenter"] == DBNull.Value ? string.Empty : row["WorkCenter"].ToString(),
                                                             WcId = Convert.ToInt32(row["WCID"]),
                                                             BomNo = Convert.ToInt32(row["BOMNO"])
                                                         }).ToList();
            }
        }
        catch (Exception ex)
        {
            // Handle exception (log it or rethrow)
            throw new Exception("Error fetching BOM tree data.", ex);
        }

        return resultList;
    }
}