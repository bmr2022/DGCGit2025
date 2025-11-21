using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    internal class ReceiveChallanDAL
    {
        private readonly ConnectionStringService _connectionStringService;
        public ReceiveChallanDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> ListOfPendingChallaFromOtherBranch(int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ListOfPendingChallaFromOtherBranch"));
              
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
               

               
                    _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveChallan", SqlParams);
                

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }



        public async Task<ResponseResult> PendingChallaItemDetailFromOtherBranch(int AccountCode, int EntryId, int YearCode, string SourceDB)
        {
            var _ResponseResult = new ResponseResult();
            try
            {


                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PendingChallaItemDetailFromOtherBranch"));

                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@issueChallanEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@issueChallanYearcode", YearCode));
                SqlParams.Add(new SqlParameter("@DatabaseName", SourceDB));



                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveChallan", SqlParams);


            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Receive Challan"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Sale Order"));

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

        public async Task<ResponseResult> GetMRNNo(int yearcode, string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetMRNNo"));
                SqlParams.Add(new SqlParameter("@yearcode", yearcode));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        //public static string ParseFormattedDate(string dateString)
        //{
        //    if (string.IsNullOrEmpty(dateString))
        //    {
        //        return string.Empty;
        //    }
        //    DateTime date;
        //    string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "" };

        //    if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
        //    {
        //        return date.ToString("yyyy/MM/dd");
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}
        public async Task<ResponseResult> GetGateNo(int yearcode, string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetGateNo"));
                SqlParams.Add(new SqlParameter("@yearcode", yearcode));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@Todate", ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMRNDetail(int EntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "MRNITEM"));
                SqlParams.Add(new SqlParameter("@EntryId", EntryId));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillAlltDetail(string GateNo,int GateYC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetGateMAINDETRAIL"));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateDetail(string GateNo, int GateYc, int GateEntryId,string FromDate,string ToDate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var FromDt = ParseDate(FromDate);
                var ToDt = ParseDate(ToDate);   
                var SqlParams = new List<dynamic>();
                if (Flag == "MRN")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "MRNITEM"));      
                }
                else  
                {
                    SqlParams.Add(new SqlParameter("@Flag", "GATEMAINITEM"));
                }
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYc));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
                SqlParams.Add(new SqlParameter("@FromDate", FromDt));
                SqlParams.Add(new SqlParameter("@Todate", ToDt));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMRNYear(string MRNNO)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetMRNYear"));
                SqlParams.Add(new SqlParameter("@MRNNO", MRNNO));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetStoreName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMRNDate(string MRNNO, int MRNYear)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetMRNDate"));
                SqlParams.Add(new SqlParameter("@MRNNO", MRNNO));
                SqlParams.Add(new SqlParameter("@MRNYearcode", MRNYear));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateYear(string Gateno,string FromDate,string ToDate,int yearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime FromDt = new DateTime();
                DateTime ToDt = new DateTime();
                FromDt = ParseDate(FromDate);
                ToDt = ParseDate(ToDate);
                SqlParams.Add(new SqlParameter("@Flag", "GetGateYear"));
                SqlParams.Add(new SqlParameter("@gateNo", Gateno));
                SqlParams.Add(new SqlParameter("@FromDate", FromDt));
                SqlParams.Add(new SqlParameter("@Todate", ToDt));
                SqlParams.Add(new SqlParameter("@yearcode", yearcode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetGateDate(string Gateno, int GateYC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetGateDAte"));
                SqlParams.Add(new SqlParameter("@gateNo", Gateno));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public static DateTime ParseDate(string dateString)
        {
            string[] formats = { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" };
            if (string.IsNullOrEmpty(dateString))
            {
                return default; 
            }

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }
        }

        public async Task<ResponseResult> SaveReceiveChallan(ReceiveChallanModel model, DataTable RCGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime entDt = new DateTime();
                //DateTime ChallanDt = new DateTime();
                //DateTime CreatedDate = new DateTime();
                //DateTime MRNDt = new DateTime();
                //DateTime GateDt = new DateTime();

               var entDt = CommonFunc.ParseFormattedDate(model.Entrydate);
               var ChallanDt = CommonFunc.ParseFormattedDate(model.ChallanDate);
               var GateDt = CommonFunc.ParseFormattedDate(model.GateDate);
               var MRNDt = CommonFunc.ParseFormattedDate(model.MRNDate);
               var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
               var entryDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
               var createdDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@UpdatedOn", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.Yearcode));
                //SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@EntryDate", entryDt));
                SqlParams.Add(new SqlParameter("@RetNonRetChallan", model.RetNotRetChallan ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MRNNo", model.MRNNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AgainstMRNOrGAte", model.AgainstMRNNOrGate ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MRNDate", MRNDt == default ? string.Empty : MRNDt));
                SqlParams.Add(new SqlParameter("@AgainstMRNYearCode", model.AgainstMRNYearCode));
                SqlParams.Add(new SqlParameter("@BillChallan", model.BillOrChallan ?? string.Empty));
                SqlParams.Add(new SqlParameter("@gateno", model.gateno ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateDate", GateDt == default ? string.Empty : GateDt));
                SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Challandate", ChallanDt == default ? string.Empty : ChallanDt));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@TruckNo", model.TruckNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TransPort", model.TransPort ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DeptTo", model.DeptTo));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SendforQC", model.SendforQC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TotalAmount", model.TotalAmount));
                SqlParams.Add(new SqlParameter("@NetAmt", model.NetAmt));
                SqlParams.Add(new SqlParameter("@TotalDiscountPercent", model.TotalDiscountPercent));
                SqlParams.Add(new SqlParameter("@TotalDiscountAmount", model.TotalDiscountAmount));
                SqlParams.Add(new SqlParameter("@ChallanType", model.ChallanType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DocTypeCode", model.DocTypeCode ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvNO", model.InvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvoiceYearCode", model.InvoiceYearCode));
                SqlParams.Add(new SqlParameter("@pendcompleted", model.pendcompleted ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EnteredByEmpid", model.EnteredByEmpid));
                SqlParams.Add(new SqlParameter("@FocChallan", model.ForChallan ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.MachineName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CreatedByEmpid", model.CreatedByEmpid));
                SqlParams.Add(new SqlParameter("@CreatedOn", createdDt));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@UID", model.UID));

                SqlParams.Add(new SqlParameter("@DTSSGrid", RCGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal async Task<ResponseResult> GetDashboardData(RCDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var ToDate = DateTime.Now;
                var toDt= CommonFunc.ParseFormattedDate(ToDate.ToString("dd/MM/yyyy")); 
                var FromDate ="";
                if(model.FromDate == null)
                {
                    var fromdt = new DateTime(ToDate.Year, ToDate.Month, 1);
                    FromDate = CommonFunc.ParseFormattedDate(fromdt.ToString("dd/MM/yyyy"));
                }
                else
                {
                     FromDate = CommonFunc.ParseFormattedDate(model.FromDate);
                }   
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@VendorName", model.Account_Name));
                SqlParams.Add(new SqlParameter("@MRNNo", model.MRNNo));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
                SqlParams.Add(new SqlParameter("@GateNo", model.gateno));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", toDt));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveChallan", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ReceiveChallanModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new ReceiveChallanModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReceiveChallan", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareView(ResponseResult.Result, ref model, Mode);
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

        private static ReceiveChallanModel PrepareView(DataSet DS, ref ReceiveChallanModel? model, string Mode)
        {
            var ItemList = new List<ReceiveChallanDetail>();
            DS.Tables[0].TableName = "VReceiveChallanmainDetail";
            DS.Tables[1].TableName = "VReceiveChallanDetail";

            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
            model.Yearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["Yearcode"].ToString());
            model.Entrydate = DS.Tables[0].Rows[0]["Entrydate"].ToString().Split(" ")[0];
            model.RetNotRetChallan = DS.Tables[0].Rows[0]["RetNonRetChallan"].ToString();
            model.AgainstMRNNOrGate = DS.Tables[0].Rows[0]["AgainstMRNOrGate"].ToString();
            model.MRNNo = DS.Tables[0].Rows[0]["MRNNo"].ToString();
            model.AgainstMRNYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AgainstMRNYearCode"]);
            model.BillOrChallan = DS.Tables[0].Rows[0]["BillOrChallan"].ToString();
            model.gateno = DS.Tables[0].Rows[0]["gateno"].ToString();
            model.GateYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"]);
            model.ChallanNo = DS.Tables[0].Rows[0]["ChallanNo"].ToString();
            model.InvoiceNo = DS.Tables[0].Rows[0]["ChallanNo"].ToString();
            model.ChallanDate = DS.Tables[0].Rows[0]["ChallanDate"].ToString().Split(" ")[0];
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]);
            model.TruckNo = DS.Tables[0].Rows[0]["TruckNo"].ToString();
            //model.ActualEntryBy = DS.Tables[0].Rows[0]["StockAdjustmentDate"].ToString();
            model.TransPort = DS.Tables[0].Rows[0]["TransPort"].ToString();
            model.DeptTo = Convert.ToInt32(DS.Tables[0].Rows[0]["DeptTo"]);
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            model.SendforQC= DS.Tables[0].Rows[0]["SendforQC"].ToString();
            model.TotalAmount= Convert.ToInt32(DS.Tables[0].Rows[0]["TotalAmount"]);
            model.NetAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["NetAmt"]);
            model.TotalDiscountPercent = Convert.ToSingle(DS.Tables[0].Rows[0]["TotalDiscountPercent"]);
            model.TotalDiscountAmount = Convert.ToSingle(DS.Tables[0].Rows[0]["TotalDiscountAmount"]);
            model.ChallanType = DS.Tables[0].Rows[0]["ChallanType"].ToString();
            model.DocTypeCode = DS.Tables[0].Rows[0]["DocTypeCode"].ToString();
             model.InvoiceYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["InvoiceYearCode"]);
            model.pendcompleted = DS.Tables[0].Rows[0]["pendcompleted"].ToString();
            model.EnteredByEmpid = Convert.ToInt32(DS.Tables[0].Rows[0]["EnteredByEmpid"]);
            model.ForChallan= DS.Tables[0].Rows[0]["FocChallan"].ToString();
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"].ToString();
             model.CreatedByEmpName = DS.Tables[0].Rows[0]["CreatedByEmp"].ToString();
            model.CreatedByEmpid = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedByEmpid"]);
            model.CreatedOn= DS.Tables[0].Rows[0]["CreatedOn"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"]);
            model.GateDate = DS.Tables[0].Rows[0]["GateDate"].ToString();
            model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"].ToString();
            model.AccountName = DS.Tables[0].Rows[0]["Account_Name"].ToString();
            model.DocTypeName = DS.Tables[0].Rows[0]["DocName"].ToString();

            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString() != "")
                {
                    //model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedbyEmp"].ToString());
                    model.UpdatedByEmpName = DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString();
                    model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]);
                    model.UpdatedOn = DS.Tables[0].Rows[0]["UpdatedOn"].ToString();
                }
            }


            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new ReceiveChallanDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"]),
                        IssueChallanEntryId = Convert.ToInt32(row["IssueChallanEntryID"]),
                        IssueChallanYearCode = Convert.ToInt32(row["IssueChallanYearCode"]),
                        IssueChallanNo = row["IssueChallanNo"].ToString(),
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        PartCode= row["partcode"].ToString(),
                        ItemName= row["itemname"].ToString(),
                        Unit = row["Unit"].ToString(),
                        RecQty = Convert.ToSingle(row["RecQty"]),
                        Rate = Convert.ToSingle(row["Rate"]),
                        Amount = Convert.ToSingle(row["Amount"]),
                        IssuedQty= Convert.ToSingle(row["IssuedQty"]),
                        PendQty = Convert.ToSingle(row["PendQty"]),
                        Produced = row["Produced"].ToString(),
                        RemarkDetail= row["ItemRemark"].ToString(),
                        GateQty= Convert.ToSingle(row["GateQty"].ToString()),
                        Storeid= Convert.ToInt32(row["Storeid"]),
                        StoreName = row["Store_Name"].ToString(),
                        pendtoissue = row["pendtoissue"].ToString(),
                        BatchNo= row["Batchno"].ToString(),
                        UniqueBatchno= row["Uniquebatchno"].ToString(),
                        ItemSize = row["ItemSize"].ToString(),
                        AltUnit= row["AltUnit"].ToString(),
                        AltQty= Convert.ToSingle(row["AltQty"]),
                        PONO= row["PONO"].ToString(),
                        POYearCode= Convert.ToInt32(row["POYearCode"]),
                        PODate= row["PODate"].ToString(),
                        SchNo= row["SchNo"].ToString(),
                        SchDate= row["SchDate"].ToString(),
                        SchYearCode= Convert.ToInt32(row["SchYearcode"])
                    });
                }
                ItemList = ItemList.OrderBy(item => item.SeqNo).ToList();
                model.ReceiveChallanList = ItemList;
            }

            return model;
        }

        public async Task<ResponseResult> GetFeatureOption()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReceiveChallan", SqlParams);
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
