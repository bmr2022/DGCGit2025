using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL;

public class GateAttendanceDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private IDataReader? Reader;
    private dynamic? _ResponseResult;
    private readonly ConnectionStringService _connectionStringService;
    public GateAttendanceDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        //configuration = config;
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        _IDataLogic = iDataLogic;
    }

    public async Task<GateAttendanceModel> GetManualAttendance(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode)
    {
        var Data = new GateAttendanceModel();
        Data.GateAttDetailsList = new List<GateAttendanceModel>();
        DataSet? oDataSet = new DataSet();
        var model1 = new GateAttendanceModel();
        try
        {
            //var AttndanceDt = CommonFunc.ParseFormattedDate(Attdate.ToString("dd/MMM/yyyy"));
            var AttndanceDt = CommonFunc.ParseFormattedDate(Attdate.ToString("yyyy-MM-dd"));
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                SqlCommand oCmd = new SqlCommand("HRSPGateAttendanceMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@flag", "FillEmployeesListForManualAttand");
                oCmd.Parameters.AddWithValue("@DailyMonthlyAttendance", DayOrMonthType);
                //oCmd.Parameters.AddWithValue("@AttndanceDt", AttndanceDt);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            int daysInMonth = 1;
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                var allColumns = oDataSet.Tables[0].Columns.Cast<DataColumn>()
                                 .Select(c => c.ColumnName)
                                 .ToList();
                Data.GateAttYearCode = YearCode;
                if (DayOrMonthType == "Daily")
                {
                    Data.DayHeaders = new List<string> { "FromTime", "ToTime" };
                    // Fill GateAttDetailsList with FromTime & ToTime from SP
                }
                else if (DayOrMonthType == "Monthly")
                {
                    daysInMonth = DateTime.DaysInMonth(YearCode, AttMonth);
                    Data.DayHeaders = new List<string>();
                    Data.strEmpAttMonth = new DateTime(YearCode, AttMonth, 1).ToString("MMM");
                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        Data.DayHeaders.Add($"{d}(InTime)");
                        Data.DayHeaders.Add($"{d}(OutTime)");
                    }
                }
                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    var GateAttList = new GateAttendanceModel
                    {
                        EmployeeName = dr["EmployeeName"].ToString(),
                        EmployeeCode = dr["Emp_Code"].ToString(),
                        ActualEmpShiftName = dr["ShiftName"].ToString(),
                        EmpCategory = dr["EmpCategory"].ToString(),
                        DeptName = dr["DeptName"].ToString(),
                        DesignationName = dr["Designation"].ToString(),
                        DeptId = dr["deptId"] != DBNull.Value ? Convert.ToInt32(dr["deptId"]) : 0,
                        EmpId = dr["emp_id"] != DBNull.Value ? Convert.ToInt32(dr["emp_id"]) : 0,
                        DesignationEntryId = dr["DesigId"] != DBNull.Value ? Convert.ToInt32(dr["DesigId"]) : 0,
                        ActualEmpShiftId = dr["ShiftId"] != DBNull.Value ? Convert.ToInt32(dr["ShiftId"]) : 0
                    };

                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        // fill Attendance dictionary dynamically
                        foreach (var col in allColumns.Where(c => c.Contains("(Intime)") || c.Contains("(OutTime)") || c.Contains("FromTime") || c.Contains("ToTime")).ToList())
                        {
                            GateAttList.Attendance[col.ToLower()] = dr[col]?.ToString();
                        }
                    }

                    Data.GateAttDetailsList.Add(GateAttList);
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
        return Data;
    }

    public GateAttendanceModel GetHolidayList(int EmpCatId, DateTime Attdate, int YearCode)
    {
        var Data = new GateAttendanceModel();
        Data.HolidayList = new List<GateAttendanceHolidayModel>();
        DataSet? oDataSet = new DataSet();
        var model1 = new GateAttendanceModel();
        try
        {
            var AttndanceDt = CommonFunc.ParseFormattedDate(Attdate.ToString("yyyy-MM-dd"));
            //var AttndanceDt = CommonFunc.ParseFormattedDate(Attdate.ToString("dd/MMM/yyyy"));
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                SqlCommand oCmd = new SqlCommand("HRSPGateAttendanceMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@flag", "GetHoliday");
                oCmd.Parameters.AddWithValue("@EmpCateid", EmpCatId);
                oCmd.Parameters.AddWithValue("@AttendanceDate", AttndanceDt);
                myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                Data.GateAttYearCode = YearCode;
                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    var HolidayList = new GateAttendanceHolidayModel
                    {
                        HolidayEntryId = dr["HolidayEntryId"] != DBNull.Value ? Convert.ToInt32(dr["HolidayEntryId"]) : 0,
                        HolidayYear = dr["HolidayYear"] != DBNull.Value ? Convert.ToInt32(dr["HolidayYear"]) : 0,
                        HolidayName = dr["HolidayName"].ToString(),
                        HalfDayFullDay = dr["HalfDayFullDay"].ToString(),
                        HolidayEffFrom = string.IsNullOrEmpty(dr["HolidayEffFrom"].ToString()) ? null : Convert.ToDateTime(dr["HolidayEffFrom"]),
                        HolidayEffTill = string.IsNullOrEmpty(dr["HolidayEffTill"].ToString()) ? null : Convert.ToDateTime(dr["HolidayEffTill"]),
                        DayName = dr["DayName"].ToString(),
                        CategoryId = dr["CategoryId"] != DBNull.Value ? Convert.ToInt32(dr["CategoryId"]) : 0,
                        DayType = dr["DayType"].ToString()
                    };

                    Data.HolidayList.Add(HolidayList);
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
        return Data;
    }

    public async Task<ResponseResult> GetFormRights(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Gate Attendance"));

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
    public async Task<ResponseResult> FillEntryId(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@GateAttYearCode", YearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPGateAttendanceMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> SaveGateAtt(GateAttendanceModel model, DataTable itemgrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            //DateTime EntryDt = new DateTime();
            //DateTime VouchDate = new DateTime();
            //DateTime InvDate = new DateTime();
            //DateTime AppDate = new DateTime();
            //DateTime CurrentDate = new DateTime();

            //var EntryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
            //var VouchDate = CommonFunc.ParseFormattedDate(model.VouchDate);
            //var InvDate = CommonFunc.ParseFormattedDate(model.InvDate);
            var CurrentDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
            var AppDate = "";
            SqlParams.Add(new SqlParameter("@Flag", model.Mode == "COPY" ? "INSERT" : model.Mode));
            //SqlParams.Add(new SqlParameter("@ID", model.ID));
            if (model.Mode == "INSERT")
                SqlParams.Add(new SqlParameter("@EntryID", 0));
            else
                SqlParams.Add(new SqlParameter("@EntryID", model.GateAttEntryId));
            if (model.Mode == "POA")
            {
                //AppDate = CommonFunc.ParseFormattedDate(model.ApprovedDate);
                SqlParams.Add(new SqlParameter("@ApprovedDate", AppDate == default ? string.Empty : AppDate));
                //SqlParams.Add(new SqlParameter("@Approved", model.Approved));
                //SqlParams.Add(new SqlParameter("@Approvedby", model.Approvedby));
            }

            //SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            //SqlParams.Add(new SqlParameter("@EntryDate", EntryDt == default ? string.Empty : EntryDt));
            //SqlParams.Add(new SqlParameter("@InvNo", model.InvoiceNo));
            //SqlParams.Add(new SqlParameter("@InvoiceTime", InvDate == default ? string.Empty : InvDate));
            //SqlParams.Add(new SqlParameter("@InvoiceDate", InvDate == default ? string.Empty : InvDate));
            //SqlParams.Add(new SqlParameter("@PurchVoucherNo", model.PurchVouchNo));
            //SqlParams.Add(new SqlParameter("@VoucherDate", VouchDate == default ? string.Empty : VouchDate));
            //SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            //SqlParams.Add(new SqlParameter("@StateName", model.VendorStateName));
            //SqlParams.Add(new SqlParameter("@GSTType", model.GstType));
            //SqlParams.Add(new SqlParameter("@TypeITEMSERVASSETS", model.DPBTypeServItem));

            //SqlParams.Add(new SqlParameter("@DomesticImport", model.DPBType));
            //SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerms));
            //SqlParams.Add(new SqlParameter("@Transporter", model.Transport));
            //SqlParams.Add(new SqlParameter("@Vehicleno", model.VehicleNo));
            //SqlParams.Add(new SqlParameter("@CurrencyId", Convert.ToInt32(model.Currency)));
            //SqlParams.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
            //SqlParams.Add(new SqlParameter("@ConversionFactor", model.ExchangeRate));
            //SqlParams.Add(new SqlParameter("@BillAmt", (float)Math.Round(model.ItemNetAmount, 2)));
            //SqlParams.Add(new SqlParameter("@RoundOffAmt", (float)Math.Round(model.TotalRoundOffAmt, 2)));
            //SqlParams.Add(new SqlParameter("@RoundoffType", model.TotalRoundOff));
            //SqlParams.Add(new SqlParameter("@GSTAmount", 0));
            //SqlParams.Add(new SqlParameter("@Taxableamt", (float)Math.Round(model.TxAmount, 2)));
            //SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", (float)Math.Round(model.TotalDiscountPercentage, 2)));
            //SqlParams.Add(new SqlParameter("@TotalDiscountAmount", (float)Math.Round(model.TotalAmtAftrDiscount, 2)));
            //SqlParams.Add(new SqlParameter("@NetAmt", (float)model.NetTotal));
            //SqlParams.Add(new SqlParameter("@Remark", model.Remark));
            //SqlParams.Add(new SqlParameter("@CC", model.Branch));
            //SqlParams.Add(new SqlParameter("@Uid", model.CreatedBy));
            //SqlParams.Add(new SqlParameter("@TaxVariationPOvsBill", string.Empty));
            //SqlParams.Add(new SqlParameter("@PONetAmt", (float)Math.Round(model.NetTotal, 2)));
            //SqlParams.Add(new SqlParameter("@BOEDate", EntryDt == default ? string.Empty : EntryDt));
            //SqlParams.Add(new SqlParameter("@ModeOfTrans", model.ModeOfTransport));
            //SqlParams.Add(new SqlParameter("@TotalAmtInOtherCurr", (float)Math.Round(model.NetTotal, 2)));
            //SqlParams.Add(new SqlParameter("@boeno", string.Empty));
            //SqlParams.Add(new SqlParameter("@Commodity", string.Empty));
            //SqlParams.Add(new SqlParameter("@PathOfFile1", model.PathOfFile1URL));
            //SqlParams.Add(new SqlParameter("@PathOfFile2", model.PathOfFile2URL));
            //SqlParams.Add(new SqlParameter("@PathOfFile3", model.PathOfFile3URL));
            //SqlParams.Add(new SqlParameter("@BalanceSheetClosed", 'N'));
            //SqlParams.Add(new SqlParameter("@GateRemark", string.Empty));
            //SqlParams.Add(new SqlParameter("@MrnRemark", string.Empty));
            //SqlParams.Add(new SqlParameter("@ActualEntryBy", model.PreparedBy));
            //SqlParams.Add(new SqlParameter("@ActualEntryDate", CurrentDate == default ? string.Empty : CurrentDate));
            //SqlParams.Add(new SqlParameter("@LastQcDate", EntryDt == default ? string.Empty : EntryDt));
            //SqlParams.Add(new SqlParameter("@PORemarks", string.Empty));
            //SqlParams.Add(new SqlParameter("@VendoreAddress", model.VendorAddress));
            //SqlParams.Add(new SqlParameter("@paymentDay", model.PaymentDays));

            //RoundFloatColumns(ItemDetailDT);
            //RoundFloatColumns(TaxDetailDT);

            //SqlParams.Add(new SqlParameter("@DTItemGrid", ItemDetailDT));
            //SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));
            //SqlParams.Add(new SqlParameter("@DTTDSGrid", TDSDetailDT));
            //SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
            //SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));
            //if (model.Mode == "UPDATE")
            //{
            //    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
            //    SqlParams.Add(new SqlParameter("@EneterdBy", model.UpdatedBy));
            //    SqlParams.Add(new SqlParameter("@LastUpdatedDate", CurrentDate == default ? string.Empty : CurrentDate));
            //}
            //else if (model.Mode == "INSERT")
            //{
            //    SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
            //}
            //SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachineName));
            //SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

            //_ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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
