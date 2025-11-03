using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class VendorUserDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public VendorUserDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> FillEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NEWENTRY"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> ViewDataByVendor(int accountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYAccountCode"));
                SqlParams.Add(new SqlParameter("@AccountCode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillVendorList(string isShowAll)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillVendorList"));
                SqlParams.Add(new SqlParameter("@showAll", isShowAll));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckUserDuplication(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "IsDuplicateUser"));
                SqlParams.Add(new SqlParameter("@UserId", userId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> SaveVendorUser(VendorUserModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var upDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    //SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                    //SqlParams.Add(new SqlParameter("@LastUpdationdate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                var currentDate = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());
                
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationdate", currentDate == default ? string.Empty : currentDate));
                
                SqlParams.Add(new SqlParameter("@UserEntryId", model.UserEntryId));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@UserId", model.UserId));
                SqlParams.Add(new SqlParameter("@Password", model.Password));
                SqlParams.Add(new SqlParameter("@Active", model.Active));
                SqlParams.Add(new SqlParameter("@AllowTodelete", model.AllowToDelete));
                SqlParams.Add(new SqlParameter("@AllowtoUpdate", model.AllowToUpdate));
                SqlParams.Add(new SqlParameter("@rightsForReport", model.RightsForQCModule));
                SqlParams.Add(new SqlParameter("@RightsForPurchaseModule", model.RightsForPurchaseModule));
                SqlParams.Add(new SqlParameter("@RightsForQCmodule", model.RightsForAccountModule));
                SqlParams.Add(new SqlParameter("@RightsforAccountModule", model.RightsForAccountModule));
                SqlParams.Add(new SqlParameter("@AdminUser", model.AdminUser));
                //SqlParams.Add(new SqlParameter("@ourServerName", model.OurServerName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ourServerName", "NUSRAT\\BMR" ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@databaseName", model.DatabaseName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@databaseName", "AutoComp2may25" ?? string.Empty));
                SqlParams.Add(new SqlParameter("@BranchName", model.BranchName));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@SaleBillPrefix", model.SaleBillPrefix ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", currentDate == default ? string.Empty : currentDate));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));
                SqlParams.Add(new SqlParameter("@VendorEmpName", model.VendorEmpName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<VendorUserModel> GetViewByID(int ID,string mode)
        {
            var model = new VendorUserModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@UserEntryId", ID));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("VPSpUserMasterForVendorPortal", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareView(ResponseResult.Result, ref model,mode);
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

        private static VendorUserModel PrepareView(DataSet DS, ref VendorUserModel? model,string mode)
        {
            try
            {
                int cnt = 0;

                model.UserEntryId =  DS.Tables[0].Rows[0]["UserEntryId"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["UserEntryId"]) : 0;
                model.AccountCode = DS.Tables[0].Rows[0]["accountcode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["accountcode"]) : 0;
                model.SaleBillPrefix = DS.Tables[0].Rows[0]["SaleBillPrefix"]?.ToString();
                model.VendorEmpName = DS.Tables[0].Rows[0]["VendorEmpName"]?.ToString();
                model.UserId = DS.Tables[0].Rows[0]["UserId"]?.ToString();
                model.Password = DS.Tables[0].Rows[0]["Password"]?.ToString();
                model.Active = DS.Tables[0].Rows[0]["Active"]?.ToString();
                model.AllowToDelete = DS.Tables[0].Rows[0]["AllowTodelete"]?.ToString();
                model.AllowToUpdate = DS.Tables[0].Rows[0]["AllowtoUpdate"]?.ToString();
                model.RightsForReport = DS.Tables[0].Rows[0]["rightsForReport"]?.ToString();
                model.RightsForPurchaseModule = DS.Tables[0].Rows[0]["RightsForPurchaseModule"]?.ToString();
                model.RightsForQCModule = DS.Tables[0].Rows[0]["RightsForQCmodule"]?.ToString();
                model.RightsForAccountModule = DS.Tables[0].Rows[0]["RightsforAccountModule"]?.ToString();
                model.AdminUser = DS.Tables[0].Rows[0]["AdminUser"]?.ToString();
                model.OurServerName = DS.Tables[0].Rows[0]["ourServerName"]?.ToString();
                model.DatabaseName = DS.Tables[0].Rows[0]["databaseName"]?.ToString();
                model.BranchName = DS.Tables[0].Rows[0]["BranchName"]?.ToString();
                model.ActualEntryBy = DS.Tables[0].Rows[0]["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"]) : 0;
                model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEntryBYName"]?.ToString();
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
                model.EntryByMachineName = DS.Tables[0].Rows[0]["EntryByMachineName"]?.ToString();
                // model.SeqNo = cnt + 1;

                if (mode == "U" || mode == "V")
                {
                    if (DS.Tables[0].Rows[0]["LastUpdatedByName"].ToString() != "")
                    {
                        model.LastUpdatedBy = DS.Tables[0].Rows[0]["UpdatedBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]) : 0;
                        model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"]?.ToString();
                        model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationdate"]?.ToString();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal async Task<ResponseResult> DeleteByID(int userEntryId, int accountCode,int userId, string entryByMachineName,int actualEntryBy,string actualEntryDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                actualEntryDate = eTactWeb.Data.Common.CommonFunc.ParseFormattedDate(actualEntryDate);
                var now = DateTime.Now;
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@UserEntryId", userEntryId));
                SqlParams.Add(new SqlParameter("@AccountCode", accountCode));
                SqlParams.Add(new SqlParameter("@UserId", userId));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", entryByMachineName));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", actualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualEntryDate));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetDashboardData(string accountName = "", int? userId = null)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime FromDate1 = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                SqlParams.Add(new SqlParameter("@VendorName", accountName));
                SqlParams.Add(new SqlParameter("@UserId", userId == null || userId == 0 ? DBNull.Value : (object)userId));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("VPSpUserMasterForVendorPortal", SqlParams);
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
