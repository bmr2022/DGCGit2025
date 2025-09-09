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
            var AttndanceDt = CommonFunc.ParseFormattedDate(Attdate.ToString("dd/MMM/yyyy"));
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

                //// filter only day-wise columns (like "1(Intime)", "1(OutTime)", ...)
                //var dayCols = allColumns.Where(c => c.Contains("(Intime)") || c.Contains("(OutTime)") || c.Contains("FromTime") || c.Contains("ToTime")).Select(x => x.ToLower())
                //                        .ToList();

                if (DayOrMonthType == "Daily")
                {
                    Data.DayHeaders = new List<string> { "FromTime", "ToTime" };
                    // Fill GateAttDetailsList with FromTime & ToTime from SP
                }
                else if (DayOrMonthType == "Monthly")
                {
                    daysInMonth = DateTime.DaysInMonth(YearCode, AttMonth);
                    Data.DayHeaders = new List<string>();

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
}
