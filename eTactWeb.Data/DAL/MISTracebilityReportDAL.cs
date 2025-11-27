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
    public class MISTracebilityReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public MISTracebilityReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<MISTracebilityReportModel> GetMISTracebilityReportData(string FromDate, string ToDate, string SaleBillNo)
        {
            var result = new MISTracebilityReportModel();
            var ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SpItemTracibility", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                        var toDt = CommonFunc.ParseFormattedDate(ToDate);

                        command.Parameters.AddWithValue("@flag", "SaleBill+Production Entry Tracking");
                        command.Parameters.AddWithValue("@FromSaleBilldate", fromDt);
                        command.Parameters.AddWithValue("@ToSaleBilldate", toDt);
                        command.Parameters.AddWithValue("@SaleBillNo", SaleBillNo);

                        await connection.OpenAsync();

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                    }
                }

                // 🔵 Assign dynamic result tables
                if (ds.Tables.Count > 0)
                    result.HeaderTable = ds.Tables[0];

                if (ds.Tables.Count > 1)
                    result.DetailTable = ds.Tables[1];
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Traceability data.", ex);
            }

            return result;
        }

        public async Task<ResponseResult> FillSaleBillNoList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillList"));
                SqlParams.Add(new SqlParameter("@FromSaleBilldate", fromDt));
                SqlParams.Add(new SqlParameter("@ToSaleBilldate", toDt));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpItemTracibility", SqlParams);
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
