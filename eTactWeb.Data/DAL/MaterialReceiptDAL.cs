using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class MaterialReceiptDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public MaterialReceiptDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GenerateMultiMRNPrint(string MRNNo, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@mrn_No", MRNNo));
                SqlParams.Add(new SqlParameter("@Year_Code", YearCode));
                
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMRNMIRDetailsReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

      
        public async Task<ResponseResult> GetGateNo(string Flag, string SPName, string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@Todate", toDt));
                //SqlParams.Add(new SqlParameter("@YearCode", DBNull.Value));
                SqlParams.Add(new SqlParameter("@YearCode", SqlDbType.Int) { Value = 0 });

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Material Receipt Note"));
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
        public async Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
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

        public async Task<ResponseResult> GetGateItemData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));
                SqlParams.Add(new SqlParameter("@GateEntryId", GateEntryId));
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
        public async Task<ResponseResult> GetDeptAndEmp(string Flag, string SPName, int deptid, int ResEmp)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@DepartmentId", deptid));
                SqlParams.Add(new SqlParameter("@EmpId", ResEmp));
                _ResponseResult = await _IDataLogic.ExecuteDataSet(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> BindDept(string Flag, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
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
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@ALtQty", AltQty));
                SqlParams.Add(new SqlParameter("@UnitQty", UnitQty));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AltUnitConversion", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;

                var FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");

                var ToDate = DateTime.Now.ToString("dd/MM/yyyy");
                var currDT= CommonFunc.ParseFormattedDate(currentDate.ToString("dd/MM/yyyy"));
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
               var firstdt = CommonFunc.ParseFormattedDate(firstDateOfMonth.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@Todate", ParseFormattedDate(ToDate)));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<MaterialReceiptModel> GetViewByID(int ID, int YearCode)
        {
            var model = new MaterialReceiptModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRN", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model);
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
        private static MaterialReceiptModel PrepareView(DataSet DS, ref MaterialReceiptModel? model)
        {


            var ItemList = new List<MaterialReceiptDetail>();
            var BatchDetail = new List<BatchDetailModel>();
            DS.Tables[0].TableName = "SSMain";
            DS.Tables[1].TableName = "SSDetail";
            DS.Tables[2].TableName = "BatchDetail";
            int cnt = 1;
            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
            
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.EntryTime = Convert.ToDateTime(DS.Tables[0].Rows[0]["EntryTime"]).ToString("HH:mm:ss")?.Trim();
            //            EntryId,YearCode,,gm.MRNNo ,EntryDate, 
            model.GateNo = DS.Tables[0].Rows[0]["GateNo"].ToString().Trim();
            model.GateEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["GateEntryId"].ToString());
            model.MRNNo = DS.Tables[0].Rows[0]["mrnno"].ToString().Trim();

            //model.GateNolst.Add(new MaterialReceiptModel { DS.Tables[0].Rows[0]["GateNo"].ToString() });

            model.GateYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"].ToString());
            model.GateDate = DS.Tables[0].Rows[0]["GateDate"].ToString();

            model.InvoiceNo = DS.Tables[0].Rows[0]["InvNo"].ToString().Trim();
            model.InvoiceDate = DS.Tables[0].Rows[0]["InvDate"].ToString();
            model.DocType = DS.Tables[0].Rows[0]["docname"].ToString();
            model.DOCID = Convert.ToInt32(DS.Tables[0].Rows[0]["DocID"].ToString());
            model.AccountID = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
            model.AccountCode = DS.Tables[0].Rows[0]["VendorName"].ToString();
            model.Address = DS.Tables[0].Rows[0]["VendAddress"].ToString();

            //model. = DS.Tables[0].Rows[0]["Account_Name"].ToString();
            model.TotalAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalAmt"].ToString());
            model.NetAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["NetAmt"].ToString());


            model.CheckQC = DS.Tables[0].Rows[0]["CheckQc"].ToString().Trim();
            model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString().Trim();
            model.RecStoreId = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["RecStoreid"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["RecStoreid"].ToString());
            model.RecInStore = DS.Tables[0].Rows[0]["RecStore"].ToString().Trim();
            model.QCCompleted = DS.Tables[0].Rows[0]["QCCompleted"].ToString().Trim();
            //model.ItemName = DS.Tables[0].Rows[0]["Item"].ToString().Trim();
            model.ItemServType = DS.Tables[0].Rows[0]["ItemSerType"].ToString().Trim();
            model.TareWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["TareWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["TareWeight"].ToString());
            model.GrossWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["GrossWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["GrossWeight"].ToString());
            model.NetWeight = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["NetWeight"].ToString()) ? 0 : Convert.ToDecimal(DS.Tables[0].Rows[0]["NetWeight"].ToString());
            model.CurrencyID = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CurrencyID"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["CurrencyID"].ToString());
            model.CurrencyName = DS.Tables[0].Rows[0]["Currency"].ToString().Trim();

            //model = DS.Tables[0].Rows[0]["Currency"].ToString();
            model.DepartId = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["DepartmentId"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["DepartmentId"].ToString());
            //model.de = DS.Tables[0].Rows[0]["DeptName"].ToString();
            model.ModeOfTransport = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString();
            model.FOC = DS.Tables[0].Rows[0]["FOC"].ToString().Trim().Trim();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString().Trim();
            model.UID = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UID"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
            model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEntryBY"].ToString();
            model.ActualEntryDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]);
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.CC1 = DS.Tables[0].Rows[0]["CC1"].ToString();
            model.CC2 = DS.Tables[0].Rows[0]["CC2"].ToString();
            model.CC3 = DS.Tables[0].Rows[0]["CC3"].ToString();
            model.EmailId = DS.Tables[0].Rows[0]["EmailId"].ToString();

            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()))
            {
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedBy"].ToString();

                model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                model.UpdatedDate = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"]);
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemList.Add(new MaterialReceiptDetail
                    {
                        SeqNo = cnt++,
                        PartCode = row["PartCode"].ToString().Trim(),
                        ItemName = row["ItemNamePartCode"].ToString().Trim(),
                        PONO = row["pono"].ToString().Trim(),
                        PoYearCode = string.IsNullOrEmpty(row["poyearcode"].ToString()) ? 0 : Convert.ToInt32(row["poyearcode"].ToString()),
                        SchNo = row["schno"].ToString().Trim(),
                        SchYearCode = string.IsNullOrEmpty(row["schyearcode"].ToString()) ? 0 : Convert.ToInt32(row["schyearcode"].ToString()),
                        PoType = row["PoType"].ToString().Trim(),
                        POAmendNo = string.IsNullOrEmpty(row["PoAmendNo"].ToString()) ? 0 : Convert.ToInt32(row["PoAmendNo"].ToString()),
                        PODate = row["PODate"].ToString(),
                        // = row["ItemCode"].ToString().Trim(),
                        Unit = row["Unit"].ToString().Trim(),
                        RateUnit = row["RateUnit"].ToString().Trim(),
                        AltUnit = row["AltUnit"].ToString().Trim(),
                        NoOfCase = string.IsNullOrEmpty(row["NoOfCase"].ToString()) ? 0 : Convert.ToInt32(row["NoOfCase"].ToString()),
                        BillQty = string.IsNullOrEmpty(row["BillQty"].ToString()) ? 0 : Convert.ToDecimal(row["BillQty"].ToString()),
                        Qty = string.IsNullOrEmpty(row["BillQty"].ToString()) ? 0 : Convert.ToDecimal(row["BillQty"].ToString()),
                        ItemCode = string.IsNullOrEmpty(row["ItemCode"].ToString()) ? 0 : Convert.ToInt32(row["ItemCode"].ToString()),
                        RecQty = string.IsNullOrEmpty(row["RecQty"].ToString()) ? 0 : Convert.ToDecimal(row["RecQty"].ToString()),
                        AltRecQty = string.IsNullOrEmpty(row["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(row["AltRecQty"].ToString()),
                        ShortExcessQty = string.IsNullOrEmpty(row["ShortExcessQty"].ToString()) ? 0 : Convert.ToDecimal(row["ShortExcessQty"].ToString()),
                        Rate = string.IsNullOrEmpty(row["Rate"].ToString()) ? 0 : Convert.ToDecimal(row["Rate"].ToString()),
                        RateinOther = string.IsNullOrEmpty(row["rateinother"].ToString()) ? 0 : Convert.ToDecimal(row["rateinother"].ToString()),
                        DisPer = string.IsNullOrEmpty(row["DiscPer"].ToString()) ? 0 : Convert.ToDecimal(row["DiscPer"].ToString()),
                        DiscRs = string.IsNullOrEmpty(row["DiscAmt"].ToString()) ? 0 : Convert.ToDecimal(row["DiscAmt"].ToString()),
                        Amount = string.IsNullOrEmpty(row["Amount"].ToString()) ? 0 : Convert.ToDecimal(row["Amount"].ToString()),
                        PendPOQty = string.IsNullOrEmpty(row["PendPOQty"].ToString()) ? 0 : Convert.ToDecimal(row["PendPOQty"].ToString()),
                        QCCompleted = row["QCCompleted"].ToString().Trim(),
                        RetChallanPendQty = string.IsNullOrEmpty(row["RetChallanPendQty"].ToString()) ? 0 : Convert.ToDecimal(row["RetChallanPendQty"].ToString()),
                        BatchWise = row["batchWise"].ToString().Trim(),
                        SaleBillNo= row["Salebillno"].ToString().Trim(),
                        SaleBillYearCode = string.IsNullOrEmpty(row["salebillYearCode"].ToString()) ? 0 : Convert.ToInt32(row["salebillYearCode"].ToString()),
                        AgainstChallanNo = row["AgainstChallanNo"].ToString().Trim(),
                        BatchNo = row["Batchno"].ToString().Trim(),
                        UniqueBatchNo = row["Uniquebatchno"].ToString().Trim(),
                        //= row["SupplierBatchNo"].ToString().Trim(),
                        ItemSize = row["ItemSize"].ToString().Trim(),
                        ItemColor = row["ItemColor"].ToString().Trim(),
                        //ItemName = row["PartCode"].ToString().Trim(),


                        SupplierBatchNo = row["SupplierBatchNo"].ToString().Trim(),
                        ShelfLife = string.IsNullOrEmpty(row["ShelfLife"].ToString()) ? 0 : Convert.ToDecimal(row["ShelfLife"].ToString()),

                    });
                }
                model.ItemDetailGrid = ItemList;
            }
            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                var cntr = 1;
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    BatchDetail.Add(new BatchDetailModel
                    {
                        SeqNo = cntr++,
                        PartCode = row["PartCode"].ToString().Trim(),
                        ItemName = row["Item_Name"].ToString().Trim(),
                        ItemCode = string.IsNullOrEmpty(row["ItemCode"].ToString()) ? 0 : Convert.ToInt32(row["ItemCode"].ToString()),
                        PONO = row["PONo"].ToString().Trim(),
                        POYearCode = string.IsNullOrEmpty(row["POYearcode"].ToString()) ? 0 : Convert.ToInt32(row["POYearcode"].ToString()),
                        SchNO = row["SchNo"].ToString().Trim(),
                        SchYearCode = string.IsNullOrEmpty(row["SchYearcode"].ToString()) ? 0 : Convert.ToInt32(row["SchYearcode"].ToString()),
                        Qty = string.IsNullOrEmpty(row["TotalQty"].ToString()) ? 0 : Convert.ToDecimal(row["TotalQty"].ToString()),
                        RecQty = string.IsNullOrEmpty(row["TotalRecQty"].ToString()) ? 0 : Convert.ToDecimal(row["TotalRecQty"].ToString()),
                        VendorBatchQty = string.IsNullOrEmpty(row["VendorBatchQty"].ToString()) ? 0 : Convert.ToDecimal(row["VendorBatchQty"].ToString()),
                        VendorBatchNo = row["VendorBatchNo"].ToString().Trim(),
                        UniqueBatchNO = row["Uniquebatchno"].ToString(),
                        ManufactureDate = string.IsNullOrEmpty(row["ManufactureDate"].ToString()) ? "" : row["ManufactureDate"].ToString().Split(" ")[0],
                        ExpiryDate = string.IsNullOrEmpty(row["ExpiryDate"].ToString()) ? "" : row["ExpiryDate"].ToString().Split(" ")[0],
                    });
                }
                model.BatchDetailGrid = BatchDetail;
            }

            return model;
        }

        internal async Task<ResponseResult> GetSearchData(MRNQDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(model.FromDate);
                var toDt = CommonFunc.ParseFormattedDate(model.ToDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
                SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
                SqlParams.Add(new SqlParameter("@PONo", model.MrnNo));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@FromMRNNo", model.FromMRNNo));
                SqlParams.Add(new SqlParameter("@ToMRNNo", model.ToMRNNo));
                SqlParams.Add(new SqlParameter("@StartDate", fromDt));
                SqlParams.Add(new SqlParameter("@EndDate", toDt));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<ResponseResult> SaveMaterialReceipt(MaterialReceiptModel model, DataTable MRGrid, DataTable BatchGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                var entDt = ParseFormattedDate(model.EntryDate);
                var gatedate = ParseFormattedDate(model.GateDate);
                var invoiceDt = ParseFormattedDate(model.InvoiceDate);
                var upDt = ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                if (model.Mode == "U")
                {

                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@updatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@updatedon", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryID));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@MRNNo", model.MRNNo ?? ""));
                SqlParams.Add(new SqlParameter("@GateNo", model.GateNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
                SqlParams.Add(new SqlParameter("@GateDate", gatedate == default ? string.Empty : gatedate));
                SqlParams.Add(new SqlParameter("@InvNo", model.InvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvDate", invoiceDt == default ? string.Empty : invoiceDt));
                SqlParams.Add(new SqlParameter("@DocID", model.DOCID));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountID));
                SqlParams.Add(new SqlParameter("@TotalAmt", model.TotalAmt));
                SqlParams.Add(new SqlParameter("@NetAmt", model.NetAmt));
                SqlParams.Add(new SqlParameter("@CheckQc", model.CheckQC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RecStoreid", model.RecStoreId));
                SqlParams.Add(new SqlParameter("@QCCompleted", "N"));
                SqlParams.Add(new SqlParameter("@ItemSerType", model.ItemServType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TareWeight", model.TareWeight));
                SqlParams.Add(new SqlParameter("@GrossWeight", model.GrossWeight));
                SqlParams.Add(new SqlParameter("@NetWeight", model.NetWeight));
                SqlParams.Add(new SqlParameter("@CurrencyID", model.CurrencyID));
                SqlParams.Add(new SqlParameter("@PurchBillVoucherno", model.PurchBillVoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PurchBillnoyearCode", model.PurchBillNoYearCode));
                SqlParams.Add(new SqlParameter("@PurchaseBillPosted", model.PurchaseBillPosted ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DepartmentId", model.DepartId));
                SqlParams.Add(new SqlParameter("@ModeOfTransport", model.ModeOfTransport ?? string.Empty));
                SqlParams.Add(new SqlParameter("@FOC", model.FOC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@UID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@EnteredEMPID", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? string.Empty));


                SqlParams.Add(new SqlParameter("@DTSSGrid", MRGrid));
                if (BatchGrid.Rows.Count != 0)
                {
                    SqlParams.Add(new SqlParameter("@DTBSGrid", BatchGrid));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> CheckFeatureOption()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<MRNQDashboard> GetDashboardData(string VendorName, string MRNNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate,int FromMRNNo,int ToMRNNo)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MRNQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_MRN", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "Search");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MRNNo);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@pono", PONo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@FromMRNNo", FromMRNNo);
                    oCmd.Parameters.AddWithValue("@ToMRNNo", ToMRNNo);


                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.MRNQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new MRNDashboard
                                           {
                                               MrnNo = dr["MrnNo"].ToString(),
                                               MrnDate = dr["MrnDate"].ToString(),
                                               GateNo = dr["GateNo"].ToString(),
                                               GateDate = dr["GateDate"].ToString(),
                                               VendorName = dr["VendorName"].ToString(),
                                               InvNo = dr["InvNo"].ToString(),
                                               InvDate = dr["InvDate"].ToString(),
                                               Docname = dr["Docname"].ToString(),
                                               MRNQCCompleted = dr["MRNQCCompleted"].ToString(),
                                               TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                               NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                               EntryId = dr["EntryId"].ToString(),
                                               YearCode = Convert.ToInt32(dr["YearCode"]),
                                               EntryBy = dr["EntryBy"].ToString(),
                                               UpdatedBy = dr["UpdatedBy"].ToString()
                                           }).ToList();
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
        public async Task<MRNQDashboard> GetDetailDashboardData(string VendorName, string MRNNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MRNQDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_MRN", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "DetailDashboard");
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@MRNNo", MRNNo);
                    oCmd.Parameters.AddWithValue("@GateNo", GateNo);
                    oCmd.Parameters.AddWithValue("@pono", PONo);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);


                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.MRNQDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new MRNDashboard
                                           {
                                               MrnNo = dr["MrnNo"].ToString(),
                                               MrnDate = dr["MrnDate"].ToString(),
                                               GateNo = dr["GateNo"].ToString(),
                                               GateDate = dr["GateDate"].ToString(),
                                               VendorName = dr["VendorName"].ToString(),
                                               InvNo = dr["InvNo"].ToString(),
                                               InvDate = dr["InvDate"].ToString(),
                                               Docname = dr["Docname"].ToString(),
                                               MRNQCCompleted = dr["MRNQCCompleted"].ToString(),
                                               TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                               NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                               EntryId = dr["EntryId"].ToString(),
                                               YearCode = Convert.ToInt32(dr["YearCode"]),
                                               EntryBy = dr["EntryBy"].ToString(),
                                               UpdatedBy = dr["UpdatedBy"].ToString(),
                                               ItemName = dr["Item_Name"].ToString(),
                                               PartCode = dr["PartCode"].ToString(),
                                               PONO = dr["pono"].ToString(),
                                               PoYearCode = Convert.ToInt32(dr["poyearcode"].ToString()),
                                               SchNo = dr["SchNo"].ToString(),
                                               SchYearCode = Convert.ToInt32(dr["schyearcode"].ToString()),
                                               PoType = dr["PoType"].ToString(),
                                               PODate = dr["PoDate"].ToString(),
                                               Unit = dr["unit"].ToString(),
                                               BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                               RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                               RateUnit = dr["RateUnit"].ToString(),
                                               AltUnit = dr["AltUnit"].ToString(),
                                               NoOfCase = Convert.ToInt32(dr["NoOfCase"].ToString()),
                                               AltRecQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                               ShortExcessQty = Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                               Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                               RateinOther = Convert.ToDecimal(dr["rateinother"].ToString()),
                                               Amount = Convert.ToDecimal(dr["Amount"].ToString()),
                                               PendPOQty = Convert.ToDecimal(dr["PendPOQty"].ToString()),
                                               QCCompleted = dr["QCCompleted"].ToString(),
                                               BatchWise = dr["batchWise"].ToString(),
                                               SaleBillNo = dr["Salebillno"].ToString(),
                                               SaleBillYearCode = Convert.ToInt32(dr["salebillYearCode"].ToString()),
                                               AgainstChallanNo = dr["AgainstChallanNo"].ToString(),
                                               BatchNo = dr["Batchno"].ToString(),
                                               UniqueBatchNo = dr["Uniquebatchno"].ToString(),
                                               SupplierBatchNo = dr["SupplierBatchNo"].ToString(),
                                               ShelfLife = Convert.ToDecimal(dr["ShelfLife"].ToString()),
                                               ItemSize = dr["ItemSize"].ToString(),
                                               ItemColor = dr["ItemColor"].ToString()
                                           }).ToList();
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
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEntryandMRN(string Flag, int YearCode, string SPName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
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
        public async Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ALLOWTOEDITDELETE"));
                SqlParams.Add(new SqlParameter("@MRNNo", MRNNo));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckBeforeInsert(string GateNo, int GateYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "CHECKBEFOREINSERT"));
                SqlParams.Add(new SqlParameter("@GateNo", GateNo));
                SqlParams.Add(new SqlParameter("@GateYearCode", GateYearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_MRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<IList<TextValue>> GetEmployeeList()
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_MRN", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "BindEmpData");

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["EmpNameCode"].ToString(),
                                Value = Reader["Emp_Id"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
    }

}
