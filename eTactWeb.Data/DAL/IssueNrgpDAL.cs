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
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class IssueNrgpDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public IssueNrgpDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Issue Challan"));
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

        public async Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo, int accountcode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@TransDate", ParseFormattedDate(TillDate)));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@BatchNo", BatchNo));
                SqlParams.Add(new SqlParameter("@uniquebatchno", UniqueBatchNo));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));

                Result = await _IDataLogic.ExecuteDataTable("GetItemRate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }

        public async Task<ResponseResult> FillEntryandChallanNo(int YearCode, string RGPNRGP)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@RGPNRGP", RGPNRGP));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();    
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }

        public async Task<ResponseResult> IssueChaallanTaxIsMandatory()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "IssueChaallanTaxIsMandatory"));
              

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetBatchInventory()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetBatchInventory"));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetVendorList()
        {
            var Result = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetVendorList"));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }  
        public async Task<ResponseResult> GetProcessList()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetProcessList"));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetStoreList()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetStoreList"));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetPrevQty"));
                SqlParams.Add(new SqlParameter("@entryid", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@uniquebatchno", uniqueBatchNo));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetAllowBackDate()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAllowBackDate"));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetAddressDetails(int AccountCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAddress"));
                SqlParams.Add(new SqlParameter("@ID", AccountCode));

                Result = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }

        public async Task<ResponseResult> GetEmails(int AccountCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetEmails"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueNRGP", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "BranchList";
                    _ResponseResult.Result.Tables[1].TableName = "VendorList";
                    _ResponseResult.Result.Tables[2].TableName = "ProcessList";
                    _ResponseResult.Result.Tables[3].TableName = "EmployeeList";
                    _ResponseResult.Result.Tables[4].TableName = "StoreList";
                    _ResponseResult.Result.Tables[5].TableName = "ScrapList";

                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }
        public async Task<ResponseResult> GetAllItems(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", Flag));
                SqlParams.Add(new SqlParameter("@SHowAllItem", 'Y'));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", Flag));
                SqlParams.Add(new SqlParameter("@SHowAllItem", 'N'));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckItems(string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", Flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillChallanType(string RGPNRGP)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", "FillChallanType"));
                SqlParams.Add(new SqlParameter("@RGPNRGP", RGPNRGP));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> StockableItems(string Flag, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", Flag));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData(INDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                string Flag = "";
                if (model.SummaryDetail == "Detail")
                {
                    Flag = "DETAILDASHBOARD";
                }
                else
                {
                    Flag = "SEARCH";
                }
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("Flag", Flag));
                SqlParams.Add(new SqlParameter("@VendorName", model.VendorName));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@ChallanType", model.ChallanType));
                SqlParams.Add(new SqlParameter("@RGPNRGP", model.RGPNRGP));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
                SqlParams.Add(new SqlParameter("@FromDate", ParseFormattedDate((model.FromDate).Split(" ")[0])));
                SqlParams.Add(new SqlParameter("@ToDate", ParseFormattedDate((model.ToDate).Split(" ")[0])));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string machineName, int actuaEntryBy)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@MachinName", machineName));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", actuaEntryBy));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        
        internal async Task<ResponseResult> CheckGateEntry(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CheckGateEntryExists"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<IssueNRGPModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new IssueNRGPModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueNRGP", SqlParams);

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

        private static IssueNRGPModel PrepareView(DataSet DS, ref IssueNRGPModel? model, string Mode)
        {
            var ItemList = new List<IssueNRGPDetail>();
            var TaxList = new List<IssueNRGPTaxDetail>();
            DS.Tables[0].TableName = "VIssueNRGP";
            DS.Tables[1].TableName = "TaxGrid";

            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.ChallanNo = DS.Tables[0].Rows[0]["ChallanNo"].ToString();
            model.ChallanType = DS.Tables[0].Rows[0]["ChallanType"].ToString();
            model.ChallanDate = DS.Tables[0].Rows[0]["ChallanDate"].ToString();
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
            model.DeliveryAddress = DS.Tables[0].Rows[0]["DeliveryAddress"].ToString();
            model.VendorStateCode = DS.Tables[0].Rows[0]["VendorStateCode"].ToString();
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
            model.Closed = DS.Tables[0].Rows[0]["Closed"].ToString();
            model.GSTType = DS.Tables[0].Rows[0]["gsttype"].ToString();
            model.RGPNRGP = DS.Tables[0].Rows[0]["RGPNRGP"].ToString();
            model.ActualEnteredEMpBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredEMpBy"].ToString());
            model.ActualEnteredEmpByName = DS.Tables[0].Rows[0]["ActualEnteredEmp"].ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["UId"].ToString());
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.VehicleNo = DS.Tables[0].Rows[0]["vehicleno"].ToString();
            model.Transporter = DS.Tables[0].Rows[0]["Transporter"].ToString();
            model.PurchaseBillNo = DS.Tables[0].Rows[0]["PurchaseBillNo"].ToString();
            model.PurchaseBillDate = DS.Tables[0].Rows[0]["PurchaseBilDate"].ToString();
            model.PurchaseBillYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["PurchaseBillYearCode"]);
            model.Distance = Convert.ToInt32(DS.Tables[0].Rows[0]["Distance"]);
            model.IssByEmpCode = DS.Tables[0].Rows[0]["IssByEmpId"] != DBNull.Value
    ? Convert.ToInt32(DS.Tables[0].Rows[0]["IssByEmpId"])
    : 0;
            model.IssByEmpCodeName = DS.Tables[0].Rows[0]["IssByEmpName"].ToString();

            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString() != "")
                {
                    model.UpdatedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedByEmpId"].ToString());
                    model.UpdatedByEmpName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
                    model.UpdatedDate = DS.Tables[0].Rows[0]["UpdatedDate"].ToString();
                }
            }


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new IssueNRGPDetail
                    {
                        SEQNo = Convert.ToInt32(row["SeqNo"]),
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        PartCode = row["partcode"].ToString(),
                        RemarkDetail = row["RemarkDetail"].ToString(),
                        ItemName = row["item_Name"].ToString(),
                        HSNNo = Convert.ToInt32(row["HSNNO"].ToString()),
                        Storeid = Convert.ToInt32(row["StoreId"]),
                        StoreName = row["Store"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        uniquebatchno = row["uniquebatchno"].ToString(),
                        TotalStock = Convert.ToInt32(row["TotalStock"]),
                        BatchStock = Convert.ToInt32(row["BatchStock"]),
                        ProcessId = Convert.ToInt32(row["ProcessId"]),
                        ProcessName = row["StageDescription"].ToString(),
                        Qty = (float)Math.Round(Convert.ToSingle(row["Qty"]), 2),
                        unit = row["unit"].ToString(),
                        Rate = (float)Math.Round(Convert.ToSingle(row["Rate"]), 2),
                        Amount = (decimal)Math.Round(Convert.ToDecimal(row["Amount"]), 4),
                        PurchasePrice = Convert.ToInt32(row["PurchasePrice"]),
                        AltQty = (float)Math.Round(Convert.ToSingle(row["AltQty"]), 2),
                        AltUnit = row["altUnit"].ToString(),
                        PONO = row["PONo"].ToString(),
                        PODate = row["PODate"].ToString(),
                        POAmendementNo = Convert.ToInt32(row["POAmmendNo"]),
                        POYearCode = Convert.ToInt32(row["PoYear"]),
                        discper = Convert.ToSingle(row["discper"]),
                        disamt = Convert.ToSingle(row["disamt"]),
                        AgainstChallanEntryId = Convert.ToInt32(row["AgainstChallanNoEntryId"]),
                        AgainstChallanNo= row["AgainstChallanNo"].ToString(),
                        AgainstChallanType = row["AgainstChallanType"].ToString(),
                        AgainstChallanYearCode = Convert.ToInt32(row["AgainstChallanYearCode"]),
                        Closed = row["Closed"].ToString(),
                        ItemColor = row["ItemColor"].ToString(),
                        ItemSize = row["ItemSize"].ToString(),
                        ItemModel = row["ItemModel"].ToString(),
                        PendQty = Convert.ToSingle(row["PendQty"]),
                        PendAltQty = Convert.ToSingle(row["PendAltQty"])
                    });
                }
                ItemList = ItemList.OrderBy(item => item.SEQNo).ToList();
                model.IssueNRGPDetailGrid = ItemList;
            }
            int cnt1 = 1;
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    TaxList.Add(new IssueNRGPTaxDetail
                    {
                        SeqNo = cnt1++,
                        TxItemCode = Convert.ToInt32(row["ItemCode"]),
                        TxType = row["Type"].ToString(),
                        TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                        TxTaxTypeName = row["TaxType"].ToString(),
                        TxAccountCode = Convert.ToInt32(row["TaxAccountCode"]),
                        TxAccountName = row["Tax_Name"].ToString(),
                        TxPercentg = Convert.ToInt32(row["TaxPer"]),
                        TxRoundOff = row["RoundOff"].ToString(),
                        TxAmount = Convert.ToInt32(row["Amount"]),
                        TxOnExp = Convert.ToInt32(row["TaxonExp"]),
                        TxRefundable = row["TaxRefundable"].ToString(),
                        TxAdInTxable = row["AddInTaxable"].ToString(),
                        TxRemark = row["Remarks"].ToString(),
                        TxPartCode = row["PartCode"].ToString(),
                        TxItemName = row["Item_Name"].ToString(),
                    });
                }
                model.IssueNRGPTaxGrid = TaxList;
            }

            return model;
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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<ResponseResult> SaveIssueNRGP(IssueNRGPModel model, DataTable INGrid, DataTable TaxDetailDT)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                //DateTime entDt = new DateTime();
                //DateTime ChallanDt = new DateTime();
                //DateTime ActualEntryDt = new DateTime();

               var entDt = ParseFormattedDate(model.EntryDate);
               var ChallanDt = ParseFormattedDate(model.ChallanDate);
               var ActualEntryDt =ParseFormattedDate(model.ActualEntryDate);

                if (model.Mode == "U")
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                else
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@EntryTime", model.EntryTime));
                SqlParams.Add(new SqlParameter("@RGPNRGP", model.RGPNRGP == null ? "" : model.RGPNRGP));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo));
                SqlParams.Add(new SqlParameter("@ChallanDate", ChallanDt == default ? string.Empty : ChallanDt));
                SqlParams.Add(new SqlParameter("@ChallanType", model.ChallanType == "-Select-" ? "" : model.ChallanType));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@DeliveryAddress", model.DeliveryAddress));
                SqlParams.Add(new SqlParameter("@VendorStateCode", model.VendorStateCode ?? ""));
                SqlParams.Add(new SqlParameter("@GSTType", model.GSTType == null ? string.Empty : model.GSTType));
                SqlParams.Add(new SqlParameter("@TotalAmount", model.TotalAmount));
                SqlParams.Add(new SqlParameter("@NetAmount", model.NetTotal));
                SqlParams.Add(new SqlParameter("@Completed", model.Completed));
                SqlParams.Add(new SqlParameter("@FullyReturned", model.FullyReturned));
                SqlParams.Add(new SqlParameter("@TotalGSTAmt", model.TotalAmount));
                 SqlParams.Add(new SqlParameter("@FromDepartId", model.FromDepartId));
                SqlParams.Add(new SqlParameter("@VehicleNo", model.VehicleNo));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark));
                SqlParams.Add(new SqlParameter("@TransportMode", model.TransportMode));
                SqlParams.Add(new SqlParameter("@RemovalTime", model.RemovalTime));
                SqlParams.Add(new SqlParameter("@EmpId", model.EmpId));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ActualEntryDt == default ? string.Empty : ActualEntryDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredEMpBy));
                SqlParams.Add(new SqlParameter("@MachinName", model.MachineName));
                SqlParams.Add(new SqlParameter("@Transporter", model.Transporter));
                SqlParams.Add(new SqlParameter("@Distance", model.Distance));
                SqlParams.Add(new SqlParameter("@IssByEmpId", model.IssByEmpCode));

                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
                }

                SqlParams.Add(new SqlParameter("@DTItemGrid", INGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var finStDt = new DateTime();
                finStDt = Convert.ToDateTime(FinStartDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                //finStDt.ToString("yyyy/MM/dd").Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@transDate", Date));
                    //.Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@batchno", batchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("FillCurrentBatchINStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string FinStartDate, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                //var finStDt = new DateTime();
                //finStDt = Convert.ToDateTime(FinStartDate);
                //DateTime TransDt = DateTime.ParseExact(TransDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                //SqlParams.Add(new SqlParameter("@FinStartDate", finStDt.ToString("yyyy/MM/dd").Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                //SqlParams.Add(new SqlParameter("@transDate", TransDt.ToString("yyyy/MM/dd").Replace("-", "/")));
                SqlParams.Add(new SqlParameter("@transDate", TransDate));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));

                Result = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetStoreTotalStock(string Flag, int ItemCode, int StoreId, string TillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime ChallanDt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var ChallanDt =ParseFormattedDate(TillDate.ToString());

                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                //  SqlParams.Add(new SqlParameter("@TILL_DATE", ChallanDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@TILL_DATE", ChallanDt));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(Flag, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchStockQty(string Flag, int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime tilldt = DateTime.ParseExact(TillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", StoreId));
                //SqlParams.Add(new SqlParameter("@TILL_DATE", tilldt.ToString("yyyy/MM/dd")));
 
                SqlParams.Add(new SqlParameter("@TILL_DATE", ParseFormattedDate ( TillDate)));
 
                SqlParams.Add(new SqlParameter("@TILL_DATE", ParseFormattedDate(TillDate)));
 
                SqlParams.Add(new SqlParameter("@BATCHNO", BatchNo));
                SqlParams.Add(new SqlParameter("@Uniquebatchno", UniqueBatchNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable(Flag, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int POYear, int ItemCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@PONO", PONO));
                SqlParams.Add(new SqlParameter("@POYearCode", POYear));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillAgainstChallanNo(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@ChallanDate", ChallanDate));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillAgainstChallanYC(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate,string AgainstChallanNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@ChallanDate", ChallanDate));
                SqlParams.Add(new SqlParameter("@AgainstChallanNo", AgainstChallanNo));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> FillAgainstChallanEntryId(string Flag, int AccountCode, int ItemCode, string ChallanDate,string AgainstChallanNo,string AgainstChallanYC)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@ChallanDate", ChallanDate));
                SqlParams.Add(new SqlParameter("@AgainstChallanNo", AgainstChallanNo));
                SqlParams.Add(new SqlParameter("@AgainstChallanyearCode", AgainstChallanYC));

                Result = await _IDataLogic.ExecuteDataTable("SP_IssueNRGP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
    }
}
