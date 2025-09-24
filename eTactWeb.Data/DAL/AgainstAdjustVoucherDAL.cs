using eTactWeb.Data.Common;
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
    public class AgainstAdjustVoucherDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public AgainstAdjustVoucherDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "JournalVoucher"));

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
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillLedger"));
                SqlParams.Add(new SqlParameter("@VoucherType", "JOURNAL-VOUCHER"));
                SqlParams.Add(new SqlParameter("@ShowAllLedger", Type));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpVoucherEntry", SqlParams);
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
