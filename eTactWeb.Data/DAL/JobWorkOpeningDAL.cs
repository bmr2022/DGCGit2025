using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
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
    public class JobWorkOpeningDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public JobWorkOpeningDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Jobwork Opening"));
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

        public async Task<ResponseResult> FillEntryId(string Flag, int YearCode, string FormTypeCustJWNRGP, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                if (FormTypeCustJWNRGP == "CustomerJobwork")
                {
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "CustomerJobworkOpening"));
                }
                else if (FormTypeCustJWNRGP == "RGPChallaan")
                {
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLANOpening"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "JobworkOpening"));
                }
                _ResponseResult = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                SqlParams.Add(new SqlParameter("@PartCode", PartCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<JobWorkOpeningModel> GetViewByID(int ID, string Mode, int YC, string OpeningType)
        {
            var model = new JobWorkOpeningModel();
            try
            {
                var SqlParams = new List<dynamic>();
                if (OpeningType == "CustomerJobworkOpening")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "CustomerJobworkOpening"));
                    SqlParams.Add(new SqlParameter("@EntryID", ID));
                    SqlParams.Add(new SqlParameter("@YearCode", YC));

                    var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkOpeningDetail", SqlParams);

                    if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                    {
                        PrepareViewForCustomer(ResponseResult.Result, ref model, Mode);
                    }
                }
                else if (OpeningType == "RGPCHALLAN")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                    SqlParams.Add(new SqlParameter("@EntryID", ID));
                    SqlParams.Add(new SqlParameter("@YearCode", YC));

                    var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkOpeningDetail", SqlParams);

                    if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                    {
                        PrepareViewForRGPChallan(ResponseResult.Result, ref model, Mode);
                    }
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                    SqlParams.Add(new SqlParameter("@EntryID", ID));
                    SqlParams.Add(new SqlParameter("@YearCode", YC));

                    var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkOpeningDetail", SqlParams);

                    if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                    {
                        PrepareView(ResponseResult.Result, ref model, Mode);
                    }
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

        private static JobWorkOpeningModel PrepareViewForCustomer(DataSet DS, ref JobWorkOpeningModel? model, string Mode)
        {
            var ItemList = new List<JobWorkOpeningModel>();
            DS.Tables[0].TableName = "CustomerJobWork";
            //DS.Tables[1].TableName = "SADetail";

            int cnt = 1;

            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["CJWOPENEntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["CJWOPENYearCode"].ToString());
            model.cc = DS.Tables[0].Rows[0]["cc"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            //model.EnteredByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EnteredByEmpId"].ToString());
            model.MachineName = DS.Tables[0].Rows[0]["EnteredByMachine"].ToString();
            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryById"].ToString());
            model.CreatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString());
            model.UpdatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdateDate"].ToString());
            model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdateBy"].ToString());
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
            model.Accountcode = Convert.ToInt32(DS.Tables[0].Rows[0]["accountcode"].ToString());
            model.AccountName = DS.Tables[0].Rows[0]["CustomerName"].ToString();
            model.OpeningType = "CustomerJobwork";


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new JobWorkOpeningModel
                    {
                        SeqNo = cnt++,
                        EntryID = Convert.ToInt32(row["CJWOPENEntryID"]),
                        YearCode = Convert.ToInt32(row["CJWOPENYearCode"].ToString()),
                        EntryDate = row["EntryDate"].ToString(),
                        ItemCode = Convert.ToInt32(row["recitemcode"].ToString()),
                        PartCode = row["partcode"].ToString(),
                        ItemName = row["RecItemName"].ToString(),
                        RecQty = Convert.ToInt32(row["RecQty"]),
                        unit = row["unit"].ToString(),
                        Rate = Convert.ToInt32(row["Rate"].ToString()),
                        Amount = Convert.ToInt32(row["Amount"].ToString()),
                        Closed = row["Closed"].ToString(),
                        RecItemCode = Convert.ToInt32(row["IssueItemcode"].ToString()),
                        RecPartCode = row["IssuePartcode"].ToString(),
                        RecItemName = row["IssueItemName"].ToString(),                        
                        BomNo = Convert.ToInt32(row["BomNo"].ToString()),
                        BomDate = row["bomdate"].ToString(),
                        Accountcode = Convert.ToInt32(row["accountcode"].ToString()),
                        AccountName = row["CustomerName"].ToString(),
                        IssJWChallanNo = row["Recchallanno"].ToString(),
                        IssChallanYearcode = Convert.ToInt32(row["RecChallanYearCode"].ToString()),
                        Isschallandate = row["Recchallandate"].ToString(),
                        cc = row["cc"].ToString(),
                        ProcessId = Convert.ToInt32(row["processid"].ToString()),
                        pendqty = Convert.ToInt32(row["PendOpnQty"]),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"].ToString(),
                        ScrapItemCode = Convert.ToInt32(row["ScrapItemCode"].ToString()),
                        ScrapPartCode = row["ScrapPartcode"].ToString(),
                        ScrapItemName = row["ScrapItemName"].ToString(),
                        UID = Convert.ToInt32(row["UID"].ToString()),
                        CreatedBy = Convert.ToInt32(row["ActualEntryById"].ToString()),
                        CreatedOn = Convert.ToDateTime(row["ActualEntryDate"].ToString()),
                        UpdatedOn = Convert.ToDateTime(row["LastUpdateDate"].ToString()),
                        UpdatedBy = Convert.ToInt32(row["LastUpdateBy"].ToString()),
                        ActualEnteredByName = row["ActualEntryByEmpName"].ToString(),
                        UpdatedByName = row["UpdatedByEmpName"].ToString(),
                        OpeningType = "CustomerJobwork",
                        BomStatus = row["BomInd"].ToString(),
                        BomType = row["BomInd"].ToString(),
                        ScrapQty = Convert.ToInt32(row["recscrapQty"].ToString()),
                        ChallanQty = Convert.ToInt32(row["ChallanQty"].ToString()),
                    });
                }
                model.ItemDetailGrid = ItemList;
            }

            return model;
        }
        private static JobWorkOpeningModel PrepareViewForRGPChallan(DataSet DS, ref JobWorkOpeningModel? model, string Mode)
        {
            var ItemList = new List<JobWorkOpeningModel>();
            DS.Tables[0].TableName = "RGPChallan";
            //DS.Tables[1].TableName = "SADetail";

            int cnt = 1;

            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["RGPOPNEntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RGPOPNYearCode"].ToString());
            model.cc = DS.Tables[0].Rows[0]["cc"].ToString();
            model.OpeningType = "RGPChallaan";
            model.Accountcode = Convert.ToInt32(DS.Tables[0].Rows[0]["accountcode"].ToString());
            model.AccountName = DS.Tables[0].Rows[0]["CustomerName"].ToString();
            //model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            ////model.EnteredByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EnteredByEmpId"].ToString());
            //model.MachineName = DS.Tables[0].Rows[0]["EnteredByMachine"].ToString();
            //model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryById"].ToString());
            //model.CreatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString());
            //model.UpdatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdateDate"].ToString());
            //model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdateBy"].ToString());
            //model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            //model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();          


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new JobWorkOpeningModel
                    {
                        SeqNo = cnt++,
                        EntryID = Convert.ToInt32(row["RGPOPNEntryID"]),
                        YearCode = Convert.ToInt32(row["RGPOPNYearCode"].ToString()),
                        EntryDate = row["EntryDate"].ToString(),
                        PartCode = row["Partcode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        ItemCode = Convert.ToInt32(row["itemcode"].ToString()),
                        RecQty = Convert.ToInt32(row["Qty"].ToString()),
                        unit = row["unit"].ToString(),
                        Rate = Convert.ToInt32(row["Rate"].ToString()),
                        Amount = Convert.ToInt32(row["Amount"].ToString()),
                        Closed = row["Closed"].ToString(),
                        ChallanQty = Convert.ToInt32(row["ChallanQty"].ToString()),
                        pendqty = Convert.ToInt32(row["OpnrecQty"].ToString()),
                        Accountcode = Convert.ToInt32(row["accountcode"].ToString()),
                        AccountName = row["CustomerName"].ToString(),
                        IssJWChallanNo = row["ChallanNo"].ToString(),
                        Isschallandate = row["challandate"].ToString(),
                        cc = row["cc"].ToString(),
                        ProcessId = Convert.ToInt32(row["processid"].ToString()),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"].ToString(),
                        UID = Convert.ToInt32(row["UID"].ToString()),
                        ClosedChallan = row["ClosedChallan"].ToString(),
                        CreatedBy = Convert.ToInt32(row["EnteredByEmpId"].ToString()),
                        ActualEnteredByName = row["ActualEntryByEmpName"].ToString(),
                        OpeningType = "RGPChallaan",
                        IssChallanYearcode = Convert.ToInt32(row["ChallanYearcode"].ToString()),
                    });
                }
                model.ItemDetailGrid = ItemList;
            }

            return model;
        }
        private static JobWorkOpeningModel PrepareView(DataSet DS, ref JobWorkOpeningModel? model, string Mode)
        {
            var ItemList = new List<JobWorkOpeningModel>();
            DS.Tables[0].TableName = "VendorJobwork";
            //DS.Tables[1].TableName = "SADetail";

            int cnt = 1;

            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["VJOBOPENEntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["VJOBOPENYearCode"].ToString());
            model.cc = DS.Tables[0].Rows[0]["CC"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.EnteredByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EnteredByEmpId"].ToString());
            model.MachineName = DS.Tables[0].Rows[0]["EnteredByMachine"].ToString();

            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryById"].ToString());
            model.CreatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString());
            model.UpdatedOn = Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString());
            model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"].ToString();
            model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
            model.Accountcode = Convert.ToInt32(DS.Tables[0].Rows[0]["Accountcode"].ToString());
            model.AccountName = DS.Tables[0].Rows[0]["VendorName"].ToString();
            model.OpeningType = "VendorJobwork";


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new JobWorkOpeningModel
                    {
                        SeqNo = cnt++,
                        EntryID = Convert.ToInt32(row["VJOBOPENEntryID"]),
                        YearCode = Convert.ToInt32(row["VJOBOPENYearCode"].ToString()),
                        TransactionType = row["TransactionType"].ToString(),
                        IssJWChallanNo = row["IssJWChallanNo"].ToString(),
                        IssChalanEntryId = Convert.ToInt32(row["IssChalanEntryId"]),
                        IssChallanYearcode = Convert.ToInt32(row["IssChallanYearcode"]),
                        Isschallandate = row["Isschallandate"].ToString(),
                        Accountcode = Convert.ToInt32(row["Accountcode"]),
                        AccountName = row["VendorName"].ToString(),
                        EntryDate = row["EntryDate"].ToString(),
                        ItemCode = Convert.ToInt32(row["ItemCode"].ToString()),
                        PartCode = row["partcode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        OpnIssQty = Convert.ToInt32(row["ItemCode"].ToString()),
                        Rate = Convert.ToInt32(row["Rate"].ToString()),
                        unit = row["unit"].ToString(),
                        Amount = Convert.ToInt32(row["Amount"]),
                        RecQty = Convert.ToInt32(row["RecQty"]),
                        pendqty = Convert.ToInt32(row["pendqty"]),
                        ScrapItemCode = Convert.ToInt32(row["ScrapItemCode"]),
                        ScrapPartCode = row["ScrapPartcode"].ToString(),
                        ScrapItemName = row["ScrapItemName"].ToString(),
                        ScrapQty = Convert.ToInt32(row["ScrapQty"]),
                        PendScrapToRec = Convert.ToInt32(row["PendScrapToRec"]),
                        BomStatus = row["BomStatus"].ToString(),
                        BomType = row["BomStatus"].ToString(),
                        RecItemCode = Convert.ToInt32(row["RecItemCode"]),
                        RecPartCode = row["RecPartcode"].ToString(),
                        RecItemName = row["RecItemName"].ToString(),
                        ProcessId = Convert.ToInt32(row["ProcessId"]),
                        ChallanQty = Convert.ToInt32(row["ChallanQty"]),
                        BatchWise = row["BatchWise"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"].ToString(),
                        cc = row["CC"].ToString(),
                        UID = Convert.ToInt32(row["UID"]),
                        EnteredByEmpId = Convert.ToInt32(row["EnteredByEmpId"]),
                        BomNo = Convert.ToInt32(row["BomNo"].ToString()),
                        BomDate = row["Bomdate"].ToString(),
                        MachineName = row["EnteredByMachine"].ToString(),
                        CreatedBy = Convert.ToInt32(row["ActualEntryById"].ToString()),
                        CreatedOn = Convert.ToDateTime(row["ActualEntryDate"].ToString()),
                        UpdatedOn = Convert.ToDateTime(row["LastUpdatedDate"].ToString()),
                        UpdatedBy = Convert.ToInt32(row["LastUpdatedBy"].ToString()),
                        ActualEnteredByName = row["ActualEntryByEmpName"].ToString(),
                        UpdatedByName = row["UpdatedByEmpName"].ToString(),
                        Closed = row["Closed"].ToString(),
                        OpeningType = "VendorJobwork"
                    });
                }
                model.ItemDetailGrid = ItemList;
            }

            return model;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string SumDetail, int Itemcode, string ChallanNo, string OpeningType,
            int ActualEntryById, string MachineName, int AccountCode, string Partcode, string ItemName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (OpeningType == "vendorjobwork")
                {
                    if (SumDetail == "SUMM")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                        SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@ActualEntryById", ActualEntryById));
                        SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                        SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                        SqlParams.Add(new SqlParameter("@Partcode", Partcode));
                        SqlParams.Add(new SqlParameter("@ItemName", ItemName));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                        SqlParams.Add(new SqlParameter("@IssJWChallanNo", ChallanNo));
                    }
                }
                else if (OpeningType == "CustomerJobworkOpening")
                {

                    if (SumDetail == "SUMM")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "customerjobwork"));
                        SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@ActualEntryById", ActualEntryById));
                        SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                        SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                        SqlParams.Add(new SqlParameter("@Partcode", Partcode));
                        SqlParams.Add(new SqlParameter("@ItemName", ItemName));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "customerjobwork"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                        SqlParams.Add(new SqlParameter("@IssJWChallanNo", ChallanNo));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                        SqlParams.Add(new SqlParameter("@ActualEntryById", ActualEntryById));
                        SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                        SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                    }
                }
                else
                {

                    if (SumDetail == "SUMM")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                        SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@ActualEntryById", ActualEntryById));
                        SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                        SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                        SqlParams.Add(new SqlParameter("@Partcode", Partcode));
                        SqlParams.Add(new SqlParameter("@ItemName", ItemName));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                        SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                        SqlParams.Add(new SqlParameter("@EntryID", ID));
                        SqlParams.Add(new SqlParameter("@YearCode", YC));
                        SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                        SqlParams.Add(new SqlParameter("@IssJWChallanNo", ChallanNo));
                        SqlParams.Add(new SqlParameter("@ItemCode", Itemcode));
                        SqlParams.Add(new SqlParameter("@ActualEntryById", ActualEntryById));
                        SqlParams.Add(new SqlParameter("@MAchineName", MachineName));
                        SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                    }
                }

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
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
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }
        }

        internal async Task<ResponseResult> GetDashboardData(JobWorkOpeningDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.OpeningType == "CustomerJobwork")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "CustomerJobworkOpening"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "SUMM"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetDetailData(JobWorkOpeningDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var SqlParams = new List<dynamic>();
                if (model.OpeningType == "CustomerJobwork")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "CustomerJobworkOpening"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                    SqlParams.Add(new SqlParameter("@SummDetail", "DETAIL"));
                    SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                    SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));
                    SqlParams.Add(new SqlParameter("@Accountname", model.VendoreName));
                    SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                    SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                    SqlParams.Add(new SqlParameter("@IssJWChallanNo", model.IssJWChallanNo));
                }


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveJobWorkOpening(JobWorkOpeningModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var entDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                if (model.OpeningType == "CustomerJobwork")
                {
                    if (model.Mode == "U" || model.Mode == "V")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));                        
                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    }
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "CustomerJobworkOpening"));
                    SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                    SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.Accountcode));
                    //DateTime parsedEntryDate = ParseDate(model.EntryDate);
                    //string formattedEntryDate = parsedEntryDate.ToString("yyyy-MM-dd");
                    SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                    SqlParams.Add(new SqlParameter("@MAchineName", model.MachineName));

                    SqlParams.Add(new SqlParameter("@DTCustJWItemGrid", GIGrid));
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    if (model.Mode == "U" || model.Mode == "V")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));                        

                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    }
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "RGPCHALLAN"));
                    SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                    SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.Accountcode));
                    //DateTime parsedEntryDate = ParseDate(model.EntryDate);
                    //string formattedEntryDate = parsedEntryDate.ToString("yyyy-MM-dd");
                    SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                    //SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.UpdatedBy));
                    //SqlParams.Add(new SqlParameter("@LastUpdatedDate", model.UpdatedOn));
                    //SqlParams.Add(new SqlParameter("@MAchineName", model.MachineName));
                    //SqlParams.Add(new SqlParameter("@ActualEntryById", model.CreatedBy));
                    //SqlParams.Add(new SqlParameter("@ActualEntryDate", model.CreatedOn));

                    SqlParams.Add(new SqlParameter("@DTRGPChalItemGrid", GIGrid));
                }
                else
                {
                    if (model.Mode == "U" || model.Mode == "V")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));                        
                    }
                    else
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    }
                    SqlParams.Add(new SqlParameter("@FormTypeCustJWNRGP", "vendorjobwork"));
                    SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                    SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                   
                    var formattedEntryDate = CommonFunc.ParseFormattedDate(model.EntryDate);
                    var createdDate = CommonFunc.ParseFormattedDate(model.CreatedOn.ToString());
                    var upDate = CommonFunc.ParseFormattedDate(model.UpdatedOn.ToString());
                    SqlParams.Add(new SqlParameter("@EntryDate", formattedEntryDate));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdatedDate", upDate));
                    SqlParams.Add(new SqlParameter("@MAchineName", model.MachineName));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.Accountcode));
                    SqlParams.Add(new SqlParameter("@ActualEntryById", model.CreatedBy));
                    SqlParams.Add(new SqlParameter("@ActualEntryDate", createdDate));

                    SqlParams.Add(new SqlParameter("@DTVendJWItemGrid", GIGrid));
                }
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }


        public async Task<ResponseResult> FillItemPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillBomNo(int ItemCode, int RecItemCode, string BomType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillBOMNO"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@RecItemCode", RecItemCode));
                SqlParams.Add(new SqlParameter("@BOMIND", BomType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartyName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPARTYNAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillUnitAltUnit(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetUnitAltUnit"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProcessName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillProcess"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRecItemPartCode(string BOMIND, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRECITEM"));
                SqlParams.Add(new SqlParameter("@BOMIND", BOMIND));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillScrapItemPartCode(string BOMIND, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSCRAPITEM"));
                SqlParams.Add(new SqlParameter("@BOMIND", BOMIND));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_JobworkOpeningDetail", SqlParams);
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
