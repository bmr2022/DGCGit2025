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
    public class VendJWRegisterDal
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public VendJWRegisterDal(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<VendJWRegisterModel> GetJWRegisterData(string FromDate, string ToDate, string RecChallanNo,string IssChallanNo,string PartyName, string PartCode, string ItemName, string IssueChallanType, string ReportMode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new VendJWRegisterModel();
            var _VenJWDet = new List<VendJWRegisterDetail>();
            var _ResponseResult = new ResponseResult();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportVendJobWorkIssueRecRecoSummDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var issfromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var isstoDt = CommonFunc.ParseFormattedDate(ToDate);
                    //DateTime recfromDt = DateTime.ParseExact(RecFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime rectoDt = DateTime.ParseExact(RecToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@flag", ReportMode);
                    oCmd.Parameters.AddWithValue("@flagIssueRecRECO", IssueChallanType==null ? "" : IssueChallanType);
                    if (IssueChallanType=="REC")
                    {
                        oCmd.Parameters.AddWithValue("@RecFromDate", issfromDt);
                        oCmd.Parameters.AddWithValue("@RecTodate", isstoDt);
                        oCmd.Parameters.AddWithValue("@RecChallanNo", RecChallanNo);
                    }
                    else if (IssueChallanType=="ISSUE")
                    {
                        oCmd.Parameters.AddWithValue("@IssFromDate", issfromDt);
                        oCmd.Parameters.AddWithValue("@Isstodate", isstoDt);
                        oCmd.Parameters.AddWithValue("@IssueChallanno", IssChallanNo);
                    }
                    else if (IssueChallanType=="RECO")
                    {
                        oCmd.Parameters.AddWithValue("@RecFromDate", issfromDt);
                        oCmd.Parameters.AddWithValue("@RecTodate", isstoDt);
                        oCmd.Parameters.AddWithValue("@IssFromDate", issfromDt);
                        oCmd.Parameters.AddWithValue("@Isstodate", isstoDt);
                        oCmd.Parameters.AddWithValue("@RecChallanNo", RecChallanNo);
                        oCmd.Parameters.AddWithValue("@IssueChallanno", IssChallanNo);
                    }
                    oCmd.Parameters.AddWithValue("@VendorName", PartyName == null ? "" : PartyName);
                    //oCmd.Parameters.AddWithValue("@issItemcode", ItemCode == null ? "" : ItemCode);
                    oCmd.Parameters.AddWithValue("@IssPartCode", PartCode == null ? "" : PartCode);
                    oCmd.Parameters.AddWithValue("@Issitemname", ItemName == null ? "" : ItemName);
                    //oCmd.Parameters.AddWithValue("@IssueChallanTYpe", IssueChallanType == null ? "" : IssueChallanType);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                    if (IssueChallanType == "REC")
                    {
                        if (ReportMode=="JOBWORKReceiveDETAIL" || ReportMode=="JOBWORKReceiveSUMMARY" || ReportMode=="JOBWORKReceiveWithAdjustemntDETAIL" || ReportMode == "JOBWORKReceiveItemWiseSUMM"|| ReportMode == "JOBWORK RECEIVE CONSOLIDATED")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var JWDetail = CommonFunc.DataRowToClass<VendJWRegisterDetail>(row);
                                    _VenJWDet.Add(JWDetail);
                                }
                                //model.RCRegisterDetails = _VenJWDet;
                                model.VendJWRegisterDetails = _VenJWDet;

                            }
                        }
                    }
                    else if(IssueChallanType == "ISSUE")
                    {
                        if (ReportMode == "JOBWORKISSUECHALLANSUMMARY" || ReportMode == "JOBWORKISSUECHALLANITEMDETAIL"|| ReportMode == "JOBWORKISSUEITEMDETAIL" || ReportMode == "JOBWORKISSUEPatyITEMDETAIL" || ReportMode == "JOBWORK ISSUECHALLAN CONSOLIDATED")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var JWDetail = CommonFunc.DataRowToClass<VendJWRegisterDetail>(row);
                                    _VenJWDet.Add(JWDetail);
                                }
                                //model.RCRegisterDetails = _VenJWDet;
                                model.VendJWRegisterDetails = _VenJWDet;

                            }
                        }
                    }
                    else if(IssueChallanType == "RECO")
                    {
                        if (ReportMode == "JOBWORKRecoDETAIL" || ReportMode == "JOBWORKRecoSummary"|| ReportMode == "ONLY PENDING CHALLAN")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var JWDetail = CommonFunc.DataRowToClass<VendJWRegisterDetail>(row);
                                    _VenJWDet.Add(JWDetail);
                                }
                                //model.RCRegisterDetails = _VenJWDet;
                                model.VendJWRegisterDetails = _VenJWDet;

                            }
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
