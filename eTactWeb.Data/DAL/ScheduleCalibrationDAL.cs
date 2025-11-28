using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using PdfSharp.Drawing.BarCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ScheduleCalibrationDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ScheduleCalibrationDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
		public async Task<PendingScheduleCalibrationModel> GetScheduleCalibrationSearchData(string PartCode,string ItemName, string ToolCode, string ToolName)
		{
			DataSet? oDataSet = new DataSet();
			var model = new PendingScheduleCalibrationModel();
			try
			{
				using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
				{
					SqlCommand oCmd = new SqlCommand("PPCSpScheduleCalibrationMainDetail", myConnection)
					{
						CommandType = CommandType.StoredProcedure
					};
					
					//var fromDt = CommonFunc.ParseFormattedDate(FromDate);
					//var toDt = CommonFunc.ParseFormattedDate(ToDate);
					oCmd.Parameters.AddWithValue("@Flag", "PendingcheduleCalibrationDashBoard");
					oCmd.Parameters.AddWithValue("@PartCode", PartCode);
					oCmd.Parameters.AddWithValue("@ItemName", ItemName);
					oCmd.Parameters.AddWithValue("@ToolCode", ToolCode);
					oCmd.Parameters.AddWithValue("@ToolName", ToolName);
				
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
					model.PendingScheduleCalibrationGrid = (from DataRow dr in oDataSet.Tables[0].Rows
													   select new PendingScheduleCalibrationModel
													   {
														   ToolName = dr["ToolName"].ToString(),
														   ToolCode = dr["ToolCode"].ToString(),
														   ItemCode = Convert.ToInt32(dr["Item_Code"].ToString()),     
														   ItemName = dr["Item_Name"].ToString(),
														   PartCode = dr["PartCode"].ToString(),
														   LastCalibrationDate = dr["LastCalibrationDate"].ToString(),
														   //NextCalibrationDate = dr["NextCalibrationDate"].ToString(),
														   DueCalibrationDate = dr["NextCalibrationDate"].ToString(),
														   CalibrationAgencyId =Convert.ToInt32(dr["CalibrationAgencyId"].ToString()),
														   LastCalibrationCertificateNo = dr["LastCalibrationCertificateNo"].ToString(),
														   CalibrationResultPassFail = dr["CalibrationResultPassFail"].ToString(),
														   TolrenceRange = dr["TolrenceRange"].ToString(),
														   CalibrationRemark = dr["CalibrationRemark"].ToString(),
														   TechEmployeeName = dr["TechEmployeeName"].ToString(),
														   Technician = dr["Technician"].ToString(),
														   TechnicialcontactNo = dr["TechnicialcontactNo"].ToString(),
														   CustoidianEmpId = Convert.ToInt32(dr["CustoidianEmpId"].ToString()),
														   CalibrationFrequencyInMonth = Convert.ToInt32(dr["CalibrationFrequencyInMonth"].ToString())

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
        public async Task<ResponseResult> GetCalibrationAgency()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CalibrationAgency"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("PPCSpScheduleCalibrationMainDetail", SqlParams);
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Qc"));
                SqlParams.Add(new SqlParameter("@SubMenu", "Calibration"));

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
}
