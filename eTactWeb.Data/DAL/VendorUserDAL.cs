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
            //DBConnectionString = _connectionStringService.GetConnectionString();
            DBConnectionString = configuration.GetConnectionString("eTactDB");
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
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationdate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                var currentDate = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());
                //var ProdSchDate = CommonFunc.ParseFormattedDate(model.ProdSchDate);
                //var FromSchDt = CommonFunc.ParseFormattedDate(model.FinFromDate);
                //var ToSchDt = CommonFunc.ParseFormattedDate(model.FinToDate);
                //var EffFromDt = CommonFunc.ParseFormattedDate(model.EffectiveFrom);
                //var EffTillDt = CommonFunc.ParseFormattedDate(model.EffectiveTill);
                //var Revdt = CommonFunc.ParseFormattedDate(model.RevDate);
                //var Actudt = CommonFunc.ParseFormattedDate(model.ActualEntryDate);


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
                SqlParams.Add(new SqlParameter("@ourServerName", model.OurServerName));
                SqlParams.Add(new SqlParameter("@databaseName", model.DatabaseName));
                SqlParams.Add(new SqlParameter("@BranchName", model.BranchName));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", currentDate == default ? string.Empty : currentDate));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

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

    }
}
