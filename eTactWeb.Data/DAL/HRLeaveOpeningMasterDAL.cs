using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class HRLeaveOpeningMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private dynamic? _ResponseResult;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }

        public HRLeaveOpeningMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetEmpCat()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillEmployeeName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDepartment(int empid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillDepartment"));
                SqlParams.Add(new SqlParameter("@EmpId", empid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDesignation(int empid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillDesignation"));
                SqlParams.Add(new SqlParameter("@EmpId", empid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetLeaveName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillLeave"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetShiftName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillShift"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetEmpCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillEmployeeCode"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveMainData(HRLeaveOpeningMasterModel model,DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var sqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    sqlParams.Add(new SqlParameter("@flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@LeaveOpnEntryId", model.LeaveOpnEntryId));


                    sqlParams.Add(new SqlParameter("@LeaveOpnYearCode", model.LeaveOpnYearCode));

                    sqlParams.Add(new SqlParameter("@EmpId", model.EmpId));

                    sqlParams.Add(new SqlParameter("@DesignationId", model.DesignationId));
                    sqlParams.Add(new SqlParameter("@DeptId", model.DeptId));
                    sqlParams.Add(new SqlParameter("@ShiftId", model.ShiftId));
                    sqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
                    sqlParams.Add(new SqlParameter("@EntryDate",
               string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : DateTime.Parse(model.EntryDate).ToString("dd/MMM/yyyy")));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate",
                     string.IsNullOrEmpty(model.ActualEntryDate) ? DBNull.Value : DateTime.Parse(model.ActualEntryDate).ToString("dd/MMM/yyyy")));

                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
                    sqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                   
                    sqlParams.Add(new SqlParameter("@UpdatedOn",
                    string.IsNullOrEmpty(model.UpdatedOn) ? DBNull.Value : DateTime.Parse(model.UpdatedOn).ToString("dd/MMM/yyyy")));

                }
                else
                {
                    sqlParams.Add(new SqlParameter("@flag", "INSERT"));
                    sqlParams.Add(new SqlParameter("@LeaveOpnEntryId", model.LeaveOpnEntryId));


                    sqlParams.Add(new SqlParameter("@LeaveOpnYearCode", model.LeaveOpnYearCode));

                    sqlParams.Add(new SqlParameter("@EmpId", model.EmpId));

                    sqlParams.Add(new SqlParameter("@DesignationId", model.DesignationId));
                    sqlParams.Add(new SqlParameter("@DeptId", model.DeptId));
                    sqlParams.Add(new SqlParameter("@ShiftId", model.ShiftId));
                    sqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
                    sqlParams.Add(new SqlParameter("@EntryDate",
               string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : DateTime.Parse(model.EntryDate).ToString("dd/MMM/yyyy")));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate",
                     string.IsNullOrEmpty(model.ActualEntryDate) ? DBNull.Value : DateTime.Parse(model.ActualEntryDate).ToString("dd/MMM/yyyy")));

                    //sqlParams.Add(new SqlParameter("@dt", GIGrid);

                }


                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", sqlParams);

            }
            catch (Exception ex)
            {
                // Set error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _IDataLogic.ExecuteDataSet("HRSPHRLeaveOpeningMainDetail", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRLeaveOpeningDashBoardModel> GetDashboardDetailData(string ReportType,string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRLeaveOpeningDashBoardModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPHRLeaveOpeningMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@todate", ToDate);
                    oCmd.Parameters.AddWithValue("@ReportType", ReportType);



                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "SUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.HRLeaveOpeningDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                               select new HRLeaveOpeningDashBoardModel
                                                               {


                                                                   LeaveOpnEntryId = Convert.ToInt32(dr["LeaveOpnEntryId"]),
                                                                   LeaveOpnYearCode = Convert.ToInt32(dr["LeaveOpnYearCode"]),
                                                                   ActualEntryDate = dr["EntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["EntryDate"]).ToString("dd-MM-yyyy"),

                                                                   EmpName = dr["Empname"].ToString(),

                                                                   EntryByMachine = dr["EntryByMachine"].ToString(),
                                                                   Department = dr["DeptName"].ToString(),
                                                                   Shift = dr["ShiftName"].ToString(),
                                                                   Designation = dr["Designation"].ToString(),
                                                                   EntryByEmp = dr["ActualEntryByEmp"].ToString(),
                                                                   UpdatedByEmp = dr["LastUpdatedEmpname"].ToString(),
                                                                   UpdatedOn = dr["UpdatedOn"].ToString(),
                                                                  

                                                               }).ToList();
                    }
                }
                else if (ReportType == "DETAIL")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.HRLeaveOpeningDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                               select new HRLeaveOpeningDashBoardModel
                                                               {


                                                                   LeaveOpnEntryId = Convert.ToInt32(dr["LeaveOpnEntryId"]),
                                                                   LeaveOpnYearCode = Convert.ToInt32(dr["LeaveOpnYearCode"]),
                                                                   ActualEntryDate = dr["EntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["EntryDate"]).ToString("dd-MM-yyyy"),


                                                                   EmpName = dr["Empname"].ToString(),


                                                                   EntryByMachine = dr["EntryByMachine"].ToString(),
                                                                   Department = dr["DeptName"].ToString(),
                                                                   Shift = dr["ShiftName"].ToString(),
                                                                   Designation = dr["Designation"].ToString(),
                                                                   EntryByEmp = dr["ActualEntryByEmp"].ToString(),
                                                                   UpdatedByEmp = dr["LastUpdatedEmpname"].ToString(),
                                                                   UpdatedOn = dr["UpdatedOn"].ToString(),
                                                                   LeaveName = dr["LeaveName"].ToString(),
                                                                   LeaveAccrualType = dr["LeaveAccrualType"].ToString(),
                                                                   OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"]),
                                                                   CarriedForward = Convert.ToDecimal(dr["CarriedForward"]),
                                                                   TotalLeaves = Convert.ToDecimal(dr["TotalLeaves"]),
                                                                   MaxCarryForward = Convert.ToDecimal(dr["MaxCarryForward"]),
                                                                   LeaveEncashmentAllowed = dr["LeaveEncashmentAllowed"].ToString(),
                                                                   LeaveValidityPeriod = dr["LeaveValidityPeriod"].ToString(),
                                                                   MandatoryLeaveAfter = Convert.ToInt32(dr["MandatoryLeaveAfterdays"]),
                                                                   MaxAllowedLeaves = Convert.ToDecimal(dr["MaxAllowedLeaves"]),

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


        private static HRLeaveOpeningMasterModel PrepareView(DataSet DS, ref HRLeaveOpeningMasterModel? model)
        {
            var HRLeaveOpeningDetail = new List<HRLeaveOpeningDetail>();
            DS.Tables[0].TableName = "HRLeaveOpeningDetail";
            int cnt = 0;
            model.LeaveOpnEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["LeaveOpnEntryId"].ToString());
            model.LeaveOpnYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["LeaveOpnYearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.EmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpId"].ToString());
            model.EmpCode = DS.Tables[0].Rows[0]["Emp_Code"].ToString();
            model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
            model.DesignationId = Convert.ToInt32(DS.Tables[0].Rows[0]["DesignationId"].ToString());
            model.DeptId = Convert.ToInt32(DS.Tables[0].Rows[0]["DeptId"].ToString());
            model.ShiftId = Convert.ToInt32(DS.Tables[0].Rows[0]["ShiftId"].ToString());
            model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
              model.Designation = DS.Tables[0].Rows[0]["Designation"].ToString();
            model.Department = DS.Tables[0].Rows[0]["DeptName"].ToString();
            model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"].ToString());
            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    HRLeaveOpeningDetail.Add(new HRLeaveOpeningDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        LeaveId = Convert.ToInt32(row["LeaveEntryId"].ToString()),
                        LeaveName = row["LeaveName"].ToString(),
                        LeaveAccrualType = row["LeaveAccrualType"].ToString(),
                        OpeningBalance = Convert.ToDecimal(row["OpeningBalance"].ToString()),
                        CarriedForward = Convert.ToDecimal(row["CarriedForward"].ToString()),
                        TotalLeaves = Convert.ToDecimal(row["TotalLeaves"].ToString()),
                        MaxCarryForward = Convert.ToDecimal(row["MaxCarryForward"].ToString()),
                        LeaveEncashmentAllowed = row["LeaveEncashmentAllowed"].ToString(),
                        LeaveValidityPeriod = row["LeaveValidityPeriod"].ToString(),
                        MandatoryLeaveAfter = Convert.ToInt32(row["MandatoryLeaveAfterdays"].ToString()),
                        MaxAllowedLeaves = Convert.ToDecimal(row["MaxAllowedLeaves"].ToString()),

                    });
                }
                model.HRLeaveOpeningDetailGrid = HRLeaveOpeningDetail.OrderBy(x => x.SeqNo).ToList();
            }
            return model;
        }
        public async Task<HRLeaveOpeningMasterModel> GetViewByID(int id,int year)
        {
            var model = new HRLeaveOpeningMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewByid"));
                SqlParams.Add(new SqlParameter("@LeaveOpnEntryId", id));
                SqlParams.Add(new SqlParameter("@LeaveOpnYearCode", year));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHRLeaveOpeningMainDetail", SqlParams);

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

        public async Task<ResponseResult> DeleteByID(int id,int year, string EntryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@LeaveOpnEntryId", id));
                SqlParams.Add(new SqlParameter("@LeaveOpnYearCode", year));
                SqlParams.Add(new SqlParameter("@EntryByMachine", EntryByMachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHRLeaveOpeningMainDetail", SqlParams);
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
