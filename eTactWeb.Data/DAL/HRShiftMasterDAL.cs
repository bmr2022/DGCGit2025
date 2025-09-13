using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class HRShiftMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public HRShiftMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> GetShiftId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPShiftMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveHrShiftMaster(HRShiftMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "update"));
                    //SqlParams.Add(new SqlParameter("@CostcenterId", model.CostcenterId > 0 ? model.CostcenterId : (object)DBNull.Value));
                    //SqlParams.Add(new SqlParameter("@EntryDate", string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : model.EntryDate));
                    //SqlParams.Add(new SqlParameter("@CostCenterName", string.IsNullOrEmpty(model.CostCenterName) ? DBNull.Value : model.CostCenterName));
                    //SqlParams.Add(new SqlParameter("@ShortName", string.IsNullOrEmpty(model.ShortName) ? DBNull.Value : model.ShortName));
                    //SqlParams.Add(new SqlParameter("@DeptId", model.DepartmentID > 0 ? model.DepartmentID : (object)DBNull.Value));
                    //SqlParams.Add(new SqlParameter("@UnderGroupId", model.UnderGroupId > 0 ? model.UnderGroupId : (object)DBNull.Value));
                    //SqlParams.Add(new SqlParameter("@Remarks", string.IsNullOrEmpty(model.Remarks) ? DBNull.Value : model.Remarks));
                    //SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    //SqlParams.Add(new SqlParameter("@CostCenterCode", string.IsNullOrEmpty(model.CostCenterCode) ? DBNull.Value : model.CostCenterCode));
                    //SqlParams.Add(new SqlParameter("@CostcentergroupID", model.CostcentergroupID > 0 ? model.CostcentergroupID : (object)DBNull.Value));
                    //SqlParams.Add(new SqlParameter("@CostcenetrGroupName", string.IsNullOrEmpty(model.CostcenetrGroupName) ? DBNull.Value : model.CostcenetrGroupName));


                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                }
                SqlParams.Add(new SqlParameter("@ShiftId", model.ShiftId > 0 ? model.ShiftId : (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@EntryDate", model.EntryDate ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode > 0 ? model.YearCode : (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@ShiftCode", string.IsNullOrEmpty(model.ShiftCode) ? DBNull.Value : model.ShiftCode));
                SqlParams.Add(new SqlParameter("@ShiftName", string.IsNullOrEmpty(model.ShiftName) ? DBNull.Value : model.ShiftName));
                //SqlParams.Add(new SqlParameter("@FromTime", string.IsNullOrEmpty(model.FromTime) ? DBNull.Value : model.FromTime));
                //SqlParams.Add(new SqlParameter("@ToTime", string.IsNullOrEmpty(model.ToTime) ? DBNull.Value : model.ToTime));
                SqlParams.Add(new SqlParameter("@FromTime",
string.IsNullOrEmpty(model.FromTime) ? DBNull.Value : Convert.ToDateTime($"1900-04-01 {model.FromTime}")));

                SqlParams.Add(new SqlParameter("@ToTime",
                    string.IsNullOrEmpty(model.ToTime) ? DBNull.Value : Convert.ToDateTime($"1900-04-01 {model.ToTime}")));
                SqlParams.Add(new SqlParameter("@GraceTimeIn", model.GraceTimeIn ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@GraceTimeOut", model.GraceTimeOut ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@Lunchfrom", string.IsNullOrEmpty(model.Lunchfrom) ? DBNull.Value : model.Lunchfrom));
                SqlParams.Add(new SqlParameter("@Lunchto", string.IsNullOrEmpty(model.Lunchto) ? DBNull.Value : model.Lunchto));
                SqlParams.Add(new SqlParameter("@FirstBreakFrom", string.IsNullOrEmpty(model.FirstBreakFrom) ? DBNull.Value : model.FirstBreakFrom));
                SqlParams.Add(new SqlParameter("@FirstBreakTo", string.IsNullOrEmpty(model.FirstBreakTo) ? DBNull.Value : model.FirstBreakTo));
                SqlParams.Add(new SqlParameter("@SecondBreakFrom", string.IsNullOrEmpty(model.SecondBreakFrom) ? DBNull.Value : model.SecondBreakFrom));
                SqlParams.Add(new SqlParameter("@SecondBreakTo", string.IsNullOrEmpty(model.SecondBreakTo) ? DBNull.Value : model.SecondBreakTo));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@UID", model.UID > 0 ? model.UID : (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@ThirdBreakFrom", string.IsNullOrEmpty(model.ThirdBreakFrom) ? DBNull.Value : model.ThirdBreakFrom));
                SqlParams.Add(new SqlParameter("@ThirdBreakTo", string.IsNullOrEmpty(model.ThirdBreakTo) ? DBNull.Value : model.ThirdBreakTo));
                SqlParams.Add(new SqlParameter("@duration", model.duration ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@ShiftForProdOnly", model.ShiftForProdOnly ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@OutDay", model.OutDay ?? (object)DBNull.Value));
                SqlParams.Add(new SqlParameter("@ShiftTypeFixRotFlex", string.IsNullOrEmpty(model.ShiftTypeFixRotFlex) ? DBNull.Value : model.ShiftTypeFixRotFlex));
                SqlParams.Add(new SqlParameter("@ApplicableToEmployeeCategory", string.IsNullOrEmpty(model.ApplicableToEmployeeCategory) ? DBNull.Value : model.ApplicableToEmployeeCategory));
                SqlParams.Add(new SqlParameter("@AttandanceMode", string.IsNullOrEmpty(model.AttandanceMode) ? DBNull.Value : model.AttandanceMode));
                SqlParams.Add(new SqlParameter("@MinimumHourRequiredForFullDay", model.MinimumHourRequiredForFullDay ?? (object)DBNull.Value));



                // Call the stored procedure with the provided parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPShiftMaster", SqlParams);
            }
            catch (Exception ex)
            {
                // Handle exceptions and prepare the error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("ShiftMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<HRShiftMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRShiftMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPShiftMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.HRShiftMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new HRShiftMasterModel
                                                        {
                                                            ShiftCode = dr["ShiftCode"] != DBNull.Value ? dr["ShiftCode"].ToString() : string.Empty,
                                                            ShiftName = dr["ShiftName"] != DBNull.Value ? dr["ShiftName"].ToString() : string.Empty,
                                                            FromTime = dr["FromTime"] != DBNull.Value ? Convert.ToDateTime(dr["FromTime"]).ToString("dd/MMM/yyyy") : string.Empty,
                                                            ToTime = dr["ToTime"] != DBNull.Value ? Convert.ToDateTime(dr["ToTime"]).ToString("dd/MMM/yyyy") : string.Empty,
                                                            GraceTimeIn = dr["GraceTimeIn"] != DBNull.Value ? Convert.ToSingle(dr["GraceTimeIn"]) : 0,
                                                            GraceTimeOut = dr["GraceTimeOut"] != DBNull.Value ? Convert.ToSingle(dr["GraceTimeOut"]) : 0,

                                                            Lunchfrom = dr["Lunchfrom"] != DBNull.Value ? dr["Lunchfrom"].ToString() : string.Empty,
                                                            Lunchto = dr["Lunchto"] != DBNull.Value ? dr["Lunchto"].ToString() : string.Empty,
                                                            FirstBreakFrom = dr["FirstBreakFrom"] != DBNull.Value ? dr["FirstBreakFrom"].ToString() : string.Empty,
                                                            FirstBreakTo = dr["FirstBreakTo"] != DBNull.Value ? dr["FirstBreakTo"].ToString() : string.Empty,
                                                            SecondBreakFrom = dr["SecondBreakFrom"] != DBNull.Value ? dr["SecondBreakFrom"].ToString() : string.Empty,
                                                            SecondBreakTo = dr["SecondBreakTo"] != DBNull.Value ? dr["SecondBreakTo"].ToString() : string.Empty,
                                                            ThirdBreakFrom = dr["ThirdBreakFrom"] != DBNull.Value ? dr["ThirdBreakFrom"].ToString() : string.Empty,
                                                            ThirdBreakTo = dr["ThirdBreakTo"] != DBNull.Value ? dr["ThirdBreakTo"].ToString() : string.Empty,

                                                            duration = dr["duration"] != DBNull.Value ? Convert.ToInt32(dr["duration"]) : 0,
                                                            ShiftForProdOnly = dr["ShiftForProdOnly"] != DBNull.Value ? dr["ShiftForProdOnly"].ToString() : string.Empty,
                                                            OutDay = dr["OutDay"] != DBNull.Value ? dr["OutDay"].ToString() : string.Empty,
                                                            ShiftTypeFixRotFlex = dr["ShiftTypeFixRotFlex"] != DBNull.Value ? dr["ShiftTypeFixRotFlex"].ToString() : string.Empty,
                                                            ApplicableToEmployeeCategory = dr["ApplicableToEmployeeCategory"] != DBNull.Value ? dr["ApplicableToEmployeeCategory"].ToString() : string.Empty,
                                                            AttandanceMode = dr["AttandanceMode"] != DBNull.Value ? dr["AttandanceMode"].ToString() : string.Empty,
                                                            MinimumHourRequiredForFullDay = dr["MinimumHourRequiredForFullDay"] != DBNull.Value ? Convert.ToInt32(dr["MinimumHourRequiredForFullDay"]) : 0,

                                                            ShiftId = dr["ShiftId"] != DBNull.Value ? Convert.ToInt32(dr["ShiftId"]) : 0,
                                                            EntryDate = dr["EntryDate"] != DBNull.Value ? Convert.ToDateTime(dr["EntryDate"]).ToString("dd/MMM/yyyy") : string.Empty,
                                                            YearCode = dr["YearCode"] != DBNull.Value ? Convert.ToInt32(dr["YearCode"]) : 0,
                                                            CC = dr["CC"] != DBNull.Value ? dr["CC"].ToString() : string.Empty,
                                                            UID = dr["UID"] != DBNull.Value ? Convert.ToInt32(dr["UID"]) : 0



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
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@ShiftId", ID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPShiftMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                // Optional: log the error using a logger
                Console.WriteLine($"Error: {Error.Message}");
                Console.WriteLine($"Source: {Error.Source}");
            }

            return _ResponseResult;
        }
        public async Task<HRShiftMasterModel> GetViewByID(int ID)
        {
            var model = new HRShiftMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@ShiftId", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPShiftMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    //PrepareView(_ResponseResult.Result, ref model);
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
    }
}
