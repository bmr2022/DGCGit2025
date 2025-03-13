using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
       
        public CompanyDetailDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> SaveCompanyDetail(CompanyDetailModel model)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
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
                    sqlParams.Add(new SqlParameter("@Start_Date", (object)model.Start_Date ?? DBNull.Value));
                    sqlParams.Add(new SqlParameter("@End_Date", (object)model.End_Date ?? DBNull.Value));
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
                    sqlParams.Add(new SqlParameter("@SoftwareStartDate", (object)model.SoftwareStartDate ?? DBNull.Value));
                    sqlParams.Add(new SqlParameter("@Prefix", model.Prefix));
                    sqlParams.Add(new SqlParameter("@ContactPersonSales", model.ContactPersonSales));
                    sqlParams.Add(new SqlParameter("@ContacPersonPurchase", model.ContacPersonPurchase));
                    sqlParams.Add(new SqlParameter("@ContactPersonQC", model.ContactPersonQC));
                    sqlParams.Add(new SqlParameter("@ContactPersonAccounts", model.ContactPersonAccounts));
                    sqlParams.Add(new SqlParameter("@Country", model.Country));
                    sqlParams.Add(new SqlParameter("@LUTNO", model.LUTNO));
                    sqlParams.Add(new SqlParameter("@LUTDATE", (object)model.LUTDATE ?? DBNull.Value));
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
    }
}
