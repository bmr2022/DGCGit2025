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

namespace eTactWeb.Data.DAL
{
    public class UserRightReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public UserRightReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> FillUserName(string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLUSERNAME"));
                SqlParams.Add(new SqlParameter("@ReportType", ReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpreportUserRights", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEmployeeName(string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLEMPLOYEENAME"));
                SqlParams.Add(new SqlParameter("@ReportType", ReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpreportUserRights", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFormName(string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFormNAME"));
                SqlParams.Add(new SqlParameter("@ReportType", ReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpreportUserRights", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillModuleName(string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLModuleNAME"));
                SqlParams.Add(new SqlParameter("@ReportType", ReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpreportUserRights", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMachineName(string ReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMachineName"));
                SqlParams.Add(new SqlParameter("@ReportType", ReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpreportUserRights", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<UserRightReportModel> GetUserRightsReportDetailData(string fromDate, string toDate, string ReportType, string UserName, string EmployeeName, string FormName, string ModuleName, string MachineName)
        {
            var resultList = new UserRightReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SpreportUserRights", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Add parameters
                    if(ReportType== "UserRights")
                    {
                        command.Parameters.AddWithValue("@Flag", "UserRightsDetail");
                        command.Parameters.AddWithValue("@UserName", UserName ?? string.Empty);
                        command.Parameters.AddWithValue("@EmployeeName", EmployeeName ?? string.Empty);
                        command.Parameters.AddWithValue("@FormName", FormName ?? string.Empty);
                        command.Parameters.AddWithValue("@ModuleName", ModuleName ?? string.Empty);
                    }
                    else if(ReportType== "LogBook")
                    {
                        command.Parameters.AddWithValue("@Flag", "LogBook");
                        command.Parameters.AddWithValue("@UserName", UserName ?? string.Empty);
                        command.Parameters.AddWithValue("@EmployeeName", EmployeeName ?? string.Empty);
                        command.Parameters.AddWithValue("@FormName", FormName ?? string.Empty);
                        command.Parameters.AddWithValue("@ModuleName", ModuleName ?? string.Empty);
                        command.Parameters.AddWithValue("@MachineName", MachineName ?? string.Empty);
                    }
                    

                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (ReportType == "UserRights")
                {
                    // Map data to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.UserRightReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                          select new UserRightReportModel
                                                          {
                                                              EmpCode = row["EmpCode"] == DBNull.Value ? string.Empty : row["EmpCode"].ToString(),
                                                              EmployeeName = row["EmployeeName"] == DBNull.Value ? string.Empty : row["EmployeeName"].ToString(),
                                                              UserName = row["UserName"] == DBNull.Value ? string.Empty : row["UserName"].ToString(),
                                                              Active = row["Active"] == DBNull.Value ? string.Empty : row["Active"].ToString(),
                                                              ModuleName = row["Module"] == DBNull.Value ? string.Empty : row["Module"].ToString(),
                                                              FormName = row["FormName"] == DBNull.Value ? string.Empty : row["FormName"].ToString(),
                                                              AllRights = row["AllRights"] == DBNull.Value ? "N" : row["AllRights"].ToString(),
                                                              Save = row["Save"] == DBNull.Value ? "N" : row["Save"].ToString(),
                                                              Update = row["Update"] == DBNull.Value ? "N" : row["Update"].ToString(),
                                                              Delete = row["Delete"] == DBNull.Value ? "N" : row["Delete"].ToString(),
                                                              View = row["View"] == DBNull.Value ? "N" : row["View"].ToString(),
                                                              //CreatedOn = row["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CreatedOn"]),
                                                              UpdatedByEmp = row["UpdatedByEmp"] == DBNull.Value ? string.Empty : row["UpdatedByEmp"].ToString(),
                                                              // UpdatedOn might be nullable
                                                              UpdatedOn = row["UpdatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["UpdatedOn"]),
                                                              UID = row["uid"] == DBNull.Value ? 0 : Convert.ToInt32(row["uid"]),
                                                              EmpID = row["EmpID"] == DBNull.Value ? 0 : Convert.ToInt32(row["EmpID"])
                                                          }).ToList();
                    }
                }
                else if (ReportType == "LogBook")
                {
                    // Map data to the LogBook model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.UserRightReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                        select new UserRightReportModel
                                                        {
                                                            FormName = row["FormName"].ToString(),
                                                            ActionTaken = row["ActionTaken"].ToString(),
                                                            ActionDate = Convert.ToDateTime(row["ActionDate"]),
                                                            ActionTime = row["ActionTime"].ToString(),
                                                            SlipNo = row["SlipNo"].ToString(),
                                                            EntryIdOfTrans = Convert.ToInt32(row["EntryIdOfTrans"]),
                                                            ActionTakenBy = row["ActionTakenBy"].ToString(),
                                                            CustVendName = row["Cust/vend Name"].ToString(),
                                                            OtherDetail = row["OtherDetail"].ToString(),
                                                            Remarks = row["Remarks"].ToString(),
                                                            MachineName = row["MachineName"].ToString()
                                                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user rights detail data.", ex);
            }

            return resultList;
        }
    }
}
