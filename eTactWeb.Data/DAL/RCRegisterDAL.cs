using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class RCRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public RCRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<RCRegisterModel> GetRCRegisterData(string FromDate, string ToDate, string Partyname, string IssueChallanNo, string RecChallanNo, string PartCode, string ItemName, string IssueChallanType,string RGPNRGP, string ReportMode,int ProcessId)
        {
            DataSet? oDataSet = new DataSet();
            var model = new RCRegisterModel();
            var _RCDetail = new List<RCRegisterDetail>();
            var _ResponseResult = new ResponseResult();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("spReportIssueRecChallanOngrid", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@flag", ReportMode);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@Todate", toDt);
                    oCmd.Parameters.AddWithValue("@accountname", Partyname == null ? string.Empty : Partyname);
                    oCmd.Parameters.AddWithValue("@IssueChallanNo", IssueChallanNo == null ? "" : IssueChallanNo);
                    oCmd.Parameters.AddWithValue("@recChallanno", RecChallanNo == null ? "" : RecChallanNo);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode == null ? "" : PartCode);
                    oCmd.Parameters.AddWithValue("@Itemname", ItemName == null ? "" : ItemName);
                    oCmd.Parameters.AddWithValue("@IssueChallanTYpe", IssueChallanType == null ? "" : IssueChallanType);
                    oCmd.Parameters.AddWithValue("@RGPNRGP", RGPNRGP == null ? "" : RGPNRGP);
                    oCmd.Parameters.AddWithValue("@ProcessId", ProcessId == null ? 0 : ProcessId);
                   
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }

                    if (ReportMode == "ISSUEDETAIL" || ReportMode == "ISSUESUMMARY"|| ReportMode == "ISSUEPARTYSUMMARY" || ReportMode == "ISSUEITEMSUMMARY" || ReportMode == "RECSUMMARY" || ReportMode == "RECDETAIL"|| ReportMode == "RECPARTYSUMMARY" || ReportMode == "RECITEMSUMMARY" || ReportMode == "RECONCILATIONSUMMARY" || ReportMode == "RECONCILATIONDETAIL")
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[0].Rows)  
                            {
                                var RCDetail = CommonFunc.DataRowToClass<RCRegisterDetail>(row);
                                _RCDetail.Add(RCDetail);
                            }
                            model.RCRegisterDetails = _RCDetail;
                        }
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
    }
}
