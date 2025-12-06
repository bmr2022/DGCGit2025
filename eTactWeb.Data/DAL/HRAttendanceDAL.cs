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
    internal async Task<HRAListDataModel> GetHRAttendanceListData(int? YearCode, string? AttendanceDate, string? EmpCateg, HRAListDataModel model)
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
                SqlCommand oCmd = new SqlCommand("HRSPHRAttendanceMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //MRNType = !string.IsNullOrEmpty(MRNType) ? MRNType : "MRN";
                //dashboardtype = !string.IsNullOrEmpty(dashboardtype) ? dashboardtype : "SUMMARY";
                //firstdate = CommonFunc.ParseFormattedDate(firstdate);
                //todate = CommonFunc.ParseFormattedDate(todate);
                
                oCmd.Parameters.AddWithValue("@flag", "PendingGateAttendance");
                //oCmd.Parameters.AddWithValue("@Fromdate", firstdate);
                //oCmd.Parameters.AddWithValue("@Todate", todate);
                oCmd.Parameters.AddWithValue("@HRAttYearCode", !string.IsNullOrEmpty(model.EmpAttYear.ToString()) && model.EmpAttYear > 0 ? model.EmpAttYear : string.Empty);
                oCmd.Parameters.AddWithValue("@AttendanceDate", !string.IsNullOrEmpty(model.AttandanceDate) && model.AttandanceDate != "0" ? model.AttandanceDate : string.Empty);
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
                        //MRNNo = dr["MRNNo"].ToString(),
                        //MRNYearCode = string.IsNullOrEmpty(dr["MRNYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["MRNYearCode"]),
                        //MRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["MRNEntryDate"]),
                        //PartyName = dr["VendorName"].ToString(),
                        //InvoiceNo = dr["InvoiceNo"].ToString(),
                        //InvoiceDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["InvDate"]),
                        //GateNo = dr["GateNo"].ToString(),
                        //GateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["GateDate"]),
                        //DocumentName = string.Empty, // Default as empty
                        //CheckQc = dr["CheckQc"].ToString(),
                        //QCCompleted = dr["QCCompleted"].ToString(),
                        TotalItemCount = string.IsNullOrEmpty(dr["TotalItemCount"].ToString()) ? 0 : Convert.ToInt32(dr["TotalItemCount"])
                        //QCtotalQty = string.IsNullOrEmpty(dr["QCtotalQty"].ToString()) ? 0 : Convert.ToInt32(dr["QCtotalQty"]),
                        //ItemQCCompledCount = string.IsNullOrEmpty(dr["ItemQCCompledCount"].ToString()) ? 0 : Convert.ToInt32(dr["ItemQCCompledCount"]),
                        //AccountCode = string.IsNullOrEmpty(dr["AccountCode"].ToString()) ? 0 : Convert.ToInt32(dr["AccountCode"])
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
}
