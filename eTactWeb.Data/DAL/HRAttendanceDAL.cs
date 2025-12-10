using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL;

public class HRAttendanceDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private readonly ConnectionStringService _connectionStringService;
    public HRAttendanceDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        _IDataLogic = iDataLogic;
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
    }
    //for list 
    internal async Task<HRAListDataModel> GetHRAttendanceListData(string? flag, string? firstdate, string? todate, HRAListDataModel model)
    {
        var HRAListData = new HRAListDataModel();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime today = DateTime.Now;
                DateTime attdate = DateTime.Now;
                SqlCommand oCmd = new SqlCommand("HRSPHRAttendanceMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                flag = !string.IsNullOrEmpty(flag) ? flag : "PendingGateAttendance";
                //model.DashboardType = !string.IsNullOrEmpty(model.DashboardType) ? model.DashboardType : "SUMMARY";
                firstdate = CommonFunc.ParseFormattedDate(firstdate);
                todate = CommonFunc.ParseFormattedDate(todate);
                attdate = CommonFunc.ParseSafeDate(model.DashAttendanceDate);
                //string fromDate = firstdate.HasValue ? CommonFunc.ParseFormattedDate(firstdate.Value.Date.ToString()) : CommonFunc.ParseFormattedDate(firstDayOfMonth.Date.ToString());
                //string toDate = todate.HasValue ? CommonFunc.ParseFormattedDate(todate.Value.Date.ToString()) : CommonFunc.ParseFormattedDate(today.Date.ToString());
                oCmd.Parameters.AddWithValue("@flag", flag);
                oCmd.Parameters.AddWithValue("@Fromdate", firstdate);
                oCmd.Parameters.AddWithValue("@Todate", todate);
                oCmd.Parameters.AddWithValue("@AttendanceDate", attdate);
                oCmd.Parameters.AddWithValue("@HRAttYearCode", model.HRAttYearCode > 0 ? model.HRAttYearCode : 0);
                //oCmd.Parameters.AddWithValue("@SummaryDetail", model.DashboardType.ToUpper());
                oCmd.Parameters.AddWithValue("@EmpCateid", !string.IsNullOrEmpty(model.DashCategory) && model.DashCategory != "0" ? Convert.ToInt32(model.DashCategory) : 0);
                oCmd.Parameters.AddWithValue("@Empid", !string.IsNullOrEmpty(model.DashEmployee) && model.DashEmployee != "0" ? Convert.ToInt32(model.DashEmployee) : 0);
                oCmd.Parameters.AddWithValue("@DepId", !string.IsNullOrEmpty(model.DashDepartment) && model.DashDepartment != "0" ? Convert.ToInt32(model.DashDepartment) : 0);
                oCmd.Parameters.AddWithValue("@DesgId", !string.IsNullOrEmpty(model.DashDesignation) && model.DashDesignation != "0" ? Convert.ToInt32(model.DashDesignation) : 0);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                HRAListData.HRAListData = new List<HRAListDataModel>();
                int seq = 1; // Start sequence number

                // Loop through each DataRow
                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    // Create and populate a new PBListDataModel object for each row
                    HRAListData.HRAListData.Add(new HRAListDataModel
                    {
                        HRAListDataSeqNo = seq++,
                        LeaveDate = string.IsNullOrEmpty(dr["LeaveDate"].ToString()) ? null : Convert.ToDateTime(dr["LeaveDate"]),
                        LeaveEntryId = string.IsNullOrEmpty(dr["LeaveEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["LeaveEntryId"]),
                        HalfDayFullDay = dr["HalfDayFullDay"].ToString(),
                        EmpName = dr["EMPName"].ToString(),
                        EmpCode = dr["Emp_Code"].ToString(),
                        EmpId = string.IsNullOrEmpty(dr["Emp_Id"].ToString()) ? 0 : Convert.ToInt32(dr["Emp_Id"]),
                        GateAttEntryId = string.IsNullOrEmpty(dr["GateAttEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["GateAttEntryId"]),
                        GateAttYearCode = string.IsNullOrEmpty(dr["GateAttYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["GateAttYearCode"]),
                        CardOrBiometricId = dr["CardOrBiometricId"].ToString(),
                        HRAttStatus = dr["HRAttStatus"].ToString(),
                        AttendStatus = dr["AttendStatus"].ToString(),
                        AttandanceDate = string.IsNullOrEmpty(dr["AttedanceDate"].ToString()) ? null : Convert.ToDateTime(dr["AttedanceDate"]),
                        AttInTime = string.IsNullOrEmpty(dr["AttInTime"].ToString()) ? null : Convert.ToDateTime(dr["AttInTime"]),
                        AttOutTime = string.IsNullOrEmpty(dr["AttOutTime"].ToString()) ? null : Convert.ToDateTime(dr["AttOutTime"]),
                        TotalNoOfHours = string.IsNullOrEmpty(dr["TotalNoOfHours"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalNoOfHours"]),
                        TotalNoOfShiftHours = string.IsNullOrEmpty(dr["TotalNoOfShiftHour"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalNoOfShiftHour"]),
                        LateEntry = dr["LateEntry"].ToString(),
                        EarlyExit = dr["EarlyExit"].ToString(),
                        LeaveTypeId = string.IsNullOrEmpty(dr["LeaveTypeId"].ToString()) ? 0 : Convert.ToInt32(dr["LeaveTypeId"]),
                        ShiftName = dr["ShiftName"].ToString(),
                        AttShiftId = string.IsNullOrEmpty(dr["AttShiftId"].ToString()) ? 0 : Convert.ToInt32(dr["AttShiftId"]),
                        EmpCateg = dr["EmpCateg"].ToString(),
                        CategoryCode = string.IsNullOrEmpty(dr["CategoryCode"].ToString()) ? 0 : Convert.ToInt32(dr["CategoryCode"]),
                        CC = dr["CC"].ToString(),
                        EmpAttDate = string.IsNullOrEmpty(dr["EmpAttDate"].ToString()) ? null : Convert.ToDateTime(dr["EmpAttDate"]),
                        EmpAttYear = string.IsNullOrEmpty(dr["EmpAttYear"].ToString()) ? 0 : Convert.ToInt32(dr["EmpAttYear"]),
                        EmpAttTime = string.IsNullOrEmpty(dr["EmpAttTime"].ToString()) ? null : Convert.ToDateTime(dr["EmpAttTime"]),
                        Designation = dr["Designation"].ToString(),
                        DeptName = dr["DeptName"].ToString(),
                        DeptId = string.IsNullOrEmpty(dr["DeptId"].ToString()) ? 0 : Convert.ToInt32(dr["DeptId"]),
                        DesigId = string.IsNullOrEmpty(dr["DesigId"].ToString()) ? 0 : Convert.ToInt32(dr["DesigId"])
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return HRAListData;
    }
    //for redirect to add screen to be implemented fully
    internal async Task<PurchaseBillModel> GetHRAttendanceItemData(string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode)
    {
        var PBItemData = new PurchaseBillModel();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("AccSP_PurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                flag = !string.IsNullOrEmpty(flag) ? flag : "DisplayPendmrn";
                FlagMRNJWCHALLAN = !string.IsNullOrEmpty(FlagMRNJWCHALLAN) ? FlagMRNJWCHALLAN : "MRN";
                Mrnno = !string.IsNullOrEmpty(Mrnno) ? Mrnno : string.Empty;
                mrnyearcode = mrnyearcode != null && mrnyearcode > 0 ? mrnyearcode : DateTime.Now.Year;
                accountcode = accountcode != null && accountcode > 0 ? accountcode : 0;
                oCmd.Parameters.AddWithValue("@flag", flag);
                oCmd.Parameters.AddWithValue("@FlagMRNJWCHALLAN", FlagMRNJWCHALLAN);
                oCmd.Parameters.AddWithValue("@Mrnno", Mrnno);
                oCmd.Parameters.AddWithValue("@mrnyearcode", mrnyearcode);
                oCmd.Parameters.AddWithValue("@accountcode", accountcode);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                var seq = 0;
                PurchaseBillModel pbItemData = null;

                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    pbItemData = new PurchaseBillModel
                    {
                        SeqNo = seq++,
                        //MRNNo = dr["MRNNo"].ToString(),
                        //MRNYearCode = string.IsNullOrEmpty(dr["MRNYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["MRNYearCode"]),
                        //MRNEntryId = string.IsNullOrEmpty(dr["MRNEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["MRNEntryId"]),
                        //MRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["MRNEntryDate"]),
                        //StrMRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? string.Empty : dr["MRNEntryDate"].ToString(),
                        //GateNo = dr["GateNo"].ToString(),
                        //GateYearCode = string.IsNullOrEmpty(dr["GateYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["GateYearCode"]),
                        //GateEntryId = string.IsNullOrEmpty(dr["GateEntryId"].ToString()) ? 0 : Convert.ToInt32(dr["GateEntryId"]),
                        //GateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["GateDate"]),
                        //StrGateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? string.Empty : dr["GateDate"].ToString(),
                        CC = dr["CC"].ToString(),
                        UID = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                        ActualEntryBy = string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                        CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                        CreatedBy = string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                    };
                    break;
                }
                PBItemData = pbItemData;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return PBItemData;
    }
    public async Task<ResponseResult> FillEntryId(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@HRAttYearCode", YearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHRAttendanceMainDetail", SqlParams);
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
            SqlParams.Add(new SqlParameter("@MainMenu", "HR Attendance"));

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
    public async Task<ResponseResult> CheckLockYear(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "CheckLockYear"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@Module", "purchase"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHRAttendanceMainDetail", SqlParams);
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
