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
    public class ControlPlanDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ControlPlanDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> SaveControlPlan(ControlPlanModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();

                //var entDt = common.CommonFunc.ParseFormattedDate(model.EntryDate);
                //var bilDt = common.CommonFunc.ParseFormattedDate(model.BiltyDate);
                //var invDt = common.CommonFunc.ParseFormattedDate(model.InvoiceDate);
                //var updDt = common.CommonFunc.ParseFormattedDate(model.UpdatedDate);

                //DateTime Invoicedt = DateTime.ParseExact(model.InvoiceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                if (model.Mode == "U" || model.Mode == "V")
                {

                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    //model.GateNo = _IDataLogic.GetEntryID("GateMain", Constants.FinincialYear, "GateNo").ToString();
                    //model.GateNo = _IDataLogic.GetEntryID("GateMain", Constants.FinincialYear, "GateNo", "Gateyearcode").ToString();
                }

                SqlParams.Add(new SqlParameter("@CntPlanEntryId", model.CntPlanEntryId));
                SqlParams.Add(new SqlParameter("@CntPlanEntryDate", model.CntPlanEntryDate));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", model.CntPlanYearCode));
                SqlParams.Add(new SqlParameter("@ControlPlanNo", model.Control_PlanNo ?? ""));
                SqlParams.Add(new SqlParameter("@RevNo", model.RevNo ?? ""));
                SqlParams.Add(new SqlParameter("@ForInOutInprocess", model.ForInOutInprocess ?? ""));
                SqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@EngApprovedBy", model.EngApprovedBy));
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                SqlParams.Add(new SqlParameter("@Remarks", model.Remarks ?? ""));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@UId", model.UId));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationDate", model.LastUpdationDate));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? ""));

                SqlParams.Add(new SqlParameter("@DTSSGrid", GIGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetNewEntryId(int Yearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", Yearcode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetEvMeasureTech()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "EvalutionMeasurmentTechnique"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCharacteristic()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "Characteristic"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
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
