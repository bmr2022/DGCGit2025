using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CompanyDetailDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CompanyDetailDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> SaveCompanyDetail(CompanyDetailModel model)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                DateTime? startDt = new DateTime();
                DateTime? endDt = new DateTime();
                //DateTime? softwarestartDt = new DateTime();
                //DateTime? lutdt = new DateTime();

                startDt = ParseDate(model.Start_Date);
                endDt = ParseDate(model.End_Date);
                //softwarestartDt = ParseDate(model.SoftwareStartDate);
                //lutdt = ParseDate(model.LUTDATE);
                var sqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                    sqlParams.Add(new SqlParameter("@DispName", model.DispName));
                    sqlParams.Add(new SqlParameter("@Com_Name", model.Com_Name));
                    sqlParams.Add(new SqlParameter("@WebSite", model.WebSite));
                    sqlParams.Add(new SqlParameter("@OfficeAdd1", model.OfficeAdd1));
                    sqlParams.Add(new SqlParameter("@OfficeAdd2", model.OfficeAdd2));
                    sqlParams.Add(new SqlParameter("@PinCode", model.PinCode));
                    sqlParams.Add(new SqlParameter("@Email", model.Email));
                    sqlParams.Add(new SqlParameter("@Phone", model.Phone));
                    sqlParams.Add(new SqlParameter("@Mobile", model.Mobile));
                    sqlParams.Add(new SqlParameter("@StateCode", model.StateCode));
                    sqlParams.Add(new SqlParameter("@StateName", model.StateName));
                    sqlParams.Add(new SqlParameter("@Commodity", model.Commodity));
                    sqlParams.Add(new SqlParameter("@Start_Date", startDt == null ? DBNull.Value : startDt));
                    sqlParams.Add(new SqlParameter("@End_Date", endDt == null ? DBNull.Value : endDt));
                    sqlParams.Add(new SqlParameter("@PhoneF", model.PhoneF));
                    sqlParams.Add(new SqlParameter("@Division", model.Division));
                    sqlParams.Add(new SqlParameter("@OrgType", model.OrgType));
                    sqlParams.Add(new SqlParameter("@PANNo", model.PANNo));
                    sqlParams.Add(new SqlParameter("@TDSAccount", model.TDSAccount));
                    sqlParams.Add(new SqlParameter("@Range", model.Range));
                    sqlParams.Add(new SqlParameter("@TDSRange", model.TDSRange));
                    sqlParams.Add(new SqlParameter("@GSTNO", model.GSTNO));
                    sqlParams.Add(new SqlParameter("@PFApplicable", model.PFApplicable));
                    sqlParams.Add(new SqlParameter("@PFNO", model.PFNO));
                    sqlParams.Add(new SqlParameter("@Registration_No", model.Registration_No));
                    sqlParams.Add(new SqlParameter("@VENDOR_CODE", model.VENDOR_CODE));
                    sqlParams.Add(new SqlParameter("@SoftwareStartDate", model.SoftwareStartDate == null ? DBNull.Value : model.SoftwareStartDate));
                    sqlParams.Add(new SqlParameter("@Prefix", model.Prefix));
                    sqlParams.Add(new SqlParameter("@ContactPersonSales", model.ContactPersonSales));
                    sqlParams.Add(new SqlParameter("@ContacPersonPurchase", model.ContacPersonPurchase));
                    sqlParams.Add(new SqlParameter("@ContactPersonQC", model.ContactPersonQC));
                    sqlParams.Add(new SqlParameter("@ContactPersonAccounts", model.ContactPersonAccounts));
                    sqlParams.Add(new SqlParameter("@Country", model.Country));
                    sqlParams.Add(new SqlParameter("@LUTNO", model.LUTNO));
                    sqlParams.Add(new SqlParameter("@LUTDATE", model.LUTDATE == null ? DBNull.Value : model.LUTDATE));
                    sqlParams.Add(new SqlParameter("@UDYMANO", model.UDYMANO));

                }

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpCompanyDetail", sqlParams);

                // Set success response if data is inserted successfully
                //_ResponseResult.StatusCode = HttpStatusCode.OK;
                //_ResponseResult.StatusText = "Success";
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
        public async Task<CompanyDetailModel > GetViewByID(int ID)
        {
            var model = new CompanyDetailModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));;
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SpCompanyDetail", SqlParams);

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
        private static CompanyDetailModel PrepareView(DataSet DS, ref CompanyDetailModel? model)
        {
            try
            {
                var ItemList = new List<CompanyDetailModel>();
               
                DS.Tables[0].TableName = "CompanyDetail";
                int cnt = 0;

                model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
               
                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new CompanyDetailModel
                        {
                            EntryID = row["EntryID"] != DBNull.Value ? Convert.ToInt32(row["EntryID"]) : 0,
                            DispName = row["DispName"] != DBNull.Value ? row["DispName"].ToString() : string.Empty,
                            Com_Name = row["Com_Name"] != DBNull.Value ? row["Com_Name"].ToString() : string.Empty,
                            WebSite = row["WebSite"] != DBNull.Value ? row["WebSite"].ToString() : string.Empty,
                            OfficeAdd1 = row["OfficeAdd1"] != DBNull.Value ? row["OfficeAdd1"].ToString() : string.Empty,
                            OfficeAdd2 = row["OfficeAdd2"] != DBNull.Value ? row["OfficeAdd2"].ToString() : string.Empty,
                            PinCode = row["PinCode"] != DBNull.Value ? row["PinCode"].ToString() : string.Empty,
                            Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : string.Empty,
                            Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : string.Empty,
                            Mobile = row["Mobile"] != DBNull.Value ? row["Mobile"].ToString() : string.Empty,
                            StateCode = row["StateCode"] != DBNull.Value ? row["StateCode"].ToString() : string.Empty,
                            StateName = row["StateName"] != DBNull.Value ? row["StateName"].ToString() : string.Empty,
                            Commodity = row["Commodity"] != DBNull.Value ? row["Commodity"].ToString() : string.Empty,
                            Start_Date = row["Start_Date"] != DBNull.Value ? Convert.ToDateTime(row["Start_Date"]).ToString("dd/MMM/yyyy") : string.Empty,
                            End_Date = row["End_Date"] != DBNull.Value ? Convert.ToDateTime(row["End_Date"]).ToString("dd/MMM/yyyy") : string.Empty,
                            PhoneF = row["PhoneF"] != DBNull.Value ? row["PhoneF"].ToString() : string.Empty,
                            Division = row["Division"] != DBNull.Value ? row["Division"].ToString() : string.Empty,
                            OrgType = row["OrgType"] != DBNull.Value ? row["OrgType"].ToString() : string.Empty,
                            PANNo = row["PANNo"] != DBNull.Value ? row["PANNo"].ToString() : string.Empty,
                            TDSAccount = row["TDSAccount"] != DBNull.Value ? row["TDSAccount"].ToString() : string.Empty,
                            Range = row["Range"] != DBNull.Value ? row["Range"].ToString() : string.Empty,
                            TDSRange = row["TDSRange"] != DBNull.Value ? row["TDSRange"].ToString() : string.Empty,
                            GSTNO = row["GSTNO"] != DBNull.Value ? row["GSTNO"].ToString() : string.Empty,
                            PFApplicable = row["PFApplicable"] != DBNull.Value ? row["PFApplicable"].ToString() : string.Empty,
                            PFNO = row["PFNO"] != DBNull.Value ? row["PFNO"].ToString() : string.Empty,
                            Registration_No = row["Registration_No"] != DBNull.Value ? row["Registration_No"].ToString() : string.Empty,
                            VENDOR_CODE = row["VENDOR_CODE"] != DBNull.Value ? row["VENDOR_CODE"].ToString() : string.Empty,
                            SoftwareStartDate = row["SoftwareStartDate"] != DBNull.Value ? Convert.ToDateTime(row["SoftwareStartDate"]).ToString("dd/MMM/yyyy") : string.Empty,
                            Prefix = row["Prefix"] != DBNull.Value ? row["Prefix"].ToString() : string.Empty,
                            ContactPersonSales = row["ContactPersonSales"] != DBNull.Value ? row["ContactPersonSales"].ToString() : string.Empty,
                            ContacPersonPurchase = row["ContacPersonPurchase"] != DBNull.Value ? row["ContacPersonPurchase"].ToString() : string.Empty,
                            ContactPersonQC = row["ContactPersonQC"] != DBNull.Value ? row["ContactPersonQC"].ToString() : string.Empty,
                            ContactPersonAccounts = row["ContactPersonAccounts"] != DBNull.Value ? row["ContactPersonAccounts"].ToString() : string.Empty,
                            Country = row["Country"] != DBNull.Value ? row["Country"].ToString() : string.Empty,
                            LUTNO = row["LUTNO"] != DBNull.Value ? row["LUTNO"].ToString() : string.Empty,
                            LUTDATE = row["LUTDATE"] != DBNull.Value ? Convert.ToDateTime(row["LUTDATE"]).ToString("dd/MMM/yyyy") : string.Empty,

                            UDYMANO = row["UDYMANO"] != DBNull.Value ? row["UDYMANO"].ToString() : string.Empty,
                            ActualEntryBy = row["ActualEntryBy"] != DBNull.Value ? row["ActualEntryBy"].ToString() : string.Empty,
                            ActualEntryDate =row["Start_Date"] != DBNull.Value ? Convert.ToDateTime(row["Start_Date"]).ToString("dd/MMM/yyyy") : string.Empty,
                        });
                    }
                   
                    model.CompanyDetailGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@Flag", "DASHBAORD")
                };

                responseResult = await _IDataLogic.ExecuteDataSet("SpCompanyDetail", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }

        public async Task<CompanyDetailModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new CompanyDetailModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpCompanyDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBAORD");

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.CompanyDetailGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                              select new CompanyDetailModel
                                                              {
                                                                  EntryID = dr["EntryID"] != DBNull.Value ? Convert.ToInt64(dr["EntryID"]) : 0,
                                                                  DispName = dr["DispName"] != DBNull.Value ? dr["DispName"].ToString() : string.Empty,
                                                                  Com_Name = dr["Com_Name"] != DBNull.Value ? dr["Com_Name"].ToString() : string.Empty,
                                                                  WebSite = dr["WebSite"] != DBNull.Value ? dr["WebSite"].ToString() : string.Empty,
                                                                  OfficeAdd1 = dr["OfficeAdd1"] != DBNull.Value ? dr["OfficeAdd1"].ToString() : string.Empty,
                                                                  OfficeAdd2 = dr["OfficeAdd2"] != DBNull.Value ? dr["OfficeAdd2"].ToString() : string.Empty,
                                                                  PinCode = dr["PinCode"] != DBNull.Value ? dr["PinCode"].ToString() : string.Empty,
                                                                  Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : string.Empty,
                                                                  Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString() : string.Empty,
                                                                  Mobile = dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : string.Empty,
                                                                  StateCode = dr["StateCode"] != DBNull.Value ? dr["StateCode"].ToString() : string.Empty,
                                                                  StateName = dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : string.Empty,
                                                                  Commodity = dr["Commodity"] != DBNull.Value ? dr["Commodity"].ToString() : string.Empty,
                                                                  Start_Date = dr["Start_Date"] != DBNull.Value ? Convert.ToDateTime(dr["Start_Date"]).ToString("dd-MM-yyyy") : null,
                                                                  End_Date = dr["End_Date"] != DBNull.Value ? Convert.ToDateTime(dr["End_Date"]).ToString("dd-MM-yyyy") : null,
                                                                  
                                                                  PhoneF = dr["PhoneF"] != DBNull.Value ? dr["PhoneF"].ToString() : string.Empty,
                                                                  Division = dr["Division"] != DBNull.Value ? dr["Division"].ToString() : string.Empty,
                                                                  OrgType = dr["OrgType"] != DBNull.Value ? dr["OrgType"].ToString() : string.Empty,
                                                                  PANNo = dr["PANNo"] != DBNull.Value ? dr["PANNo"].ToString() : string.Empty,
                                                                  TDSAccount = dr["TDSAccount"] != DBNull.Value ? dr["TDSAccount"].ToString() : string.Empty,
                                                                  Range = dr["Range"] != DBNull.Value ? dr["Range"].ToString() : string.Empty,
                                                                  TDSRange = dr["TDSRange"] != DBNull.Value ? dr["TDSRange"].ToString() : string.Empty,
                                                                  GSTNO = dr["GSTNO"] != DBNull.Value ? dr["GSTNO"].ToString() : string.Empty,
                                                                  PFApplicable = dr["PFApplicable"] != DBNull.Value ? dr["PFApplicable"].ToString() : string.Empty,
                                                                  PFNO = dr["PFNO"] != DBNull.Value ? dr["PFNO"].ToString() : string.Empty,
                                                                  Registration_No = dr["Registration_No"] != DBNull.Value ? dr["Registration_No"].ToString() : string.Empty,
                                                                  VENDOR_CODE = dr["VENDOR_CODE"] != DBNull.Value ? dr["VENDOR_CODE"].ToString() : string.Empty,
                                                                  SoftwareStartDate = dr["SoftwareStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["SoftwareStartDate"]).ToString("dd-MM-yyyy") : null,

                                                                  Prefix = dr["Prefix"] != DBNull.Value ? dr["Prefix"].ToString() : string.Empty,
                                                                  ContactPersonSales = dr["ContactPersonSales"] != DBNull.Value ? dr["ContactPersonSales"].ToString() : string.Empty,
                                                                  ContacPersonPurchase = dr["ContacPersonPurchase"] != DBNull.Value ? dr["ContacPersonPurchase"].ToString() : string.Empty,
                                                                  ContactPersonQC = dr["ContactPersonQC"] != DBNull.Value ? dr["ContactPersonQC"].ToString() : string.Empty,
                                                                  ContactPersonAccounts = dr["ContactPersonAccounts"] != DBNull.Value ? dr["ContactPersonAccounts"].ToString() : string.Empty,
                                                                  Country = dr["Country"] != DBNull.Value ? dr["Country"].ToString() : string.Empty,
                                                                  LUTNO = dr["LUTNO"] != DBNull.Value ? dr["LUTNO"].ToString() : string.Empty,
                                                                  LUTDATE = dr["LUTDATE"] != DBNull.Value ? Convert.ToDateTime(dr["LUTDATE"]).ToString("dd-MM-yyyy") : null,
                                                                
                                                                  UDYMANO = dr["UDYMANO"] != DBNull.Value ? dr["UDYMANO"].ToString() : string.Empty
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
        public static DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }
            dateString = dateString.Replace("-", "/");
            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            throw new FormatException($"Invalid date format: {dateString}. Expected format: dd/MM/yyyy");
        }
    }
}
