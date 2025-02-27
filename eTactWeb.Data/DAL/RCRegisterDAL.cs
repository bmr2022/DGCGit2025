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

        private IDataReader? Reader;

        public RCRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RCRegisterModel> GetRCRegisterData(string FromDate, string ToDate, string Partyname, string IssueChallanNo, string RecChallanNo, string PartCode, string ItemName, string IssueChallanType,string RGPNRGP, string ReportMode)
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
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@flag", ReportMode);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@Todate", toDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@accountname", Partyname == null ? string.Empty : Partyname);
                    oCmd.Parameters.AddWithValue("@IssueChallanNo", IssueChallanNo == null ? "" : RecChallanNo);
                    oCmd.Parameters.AddWithValue("@recChallanno", RecChallanNo == null ? "" : RecChallanNo);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode == null ? "" : PartCode);
                    oCmd.Parameters.AddWithValue("@Itemname", ItemName == null ? "" : ItemName);
                    oCmd.Parameters.AddWithValue("@IssueChallanTYpe", IssueChallanType == null ? "" : IssueChallanType);
                    oCmd.Parameters.AddWithValue("@RGPNRGP", RGPNRGP == null ? "" : RGPNRGP);
                   
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
