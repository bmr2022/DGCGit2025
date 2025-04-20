using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class RetFromDepartmentMainDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public RetFromDepartmentMainDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> GetDashboardData(string Fromdate, string Todate, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (Flag == "True")
                {
                    //DateTime fromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    var fromDt = CommonFunc.ParseFormattedDate(Fromdate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                    SqlParams.Add(new SqlParameter("@DashTypeSummDetail", "SUMMARY"));
                }
                else
                {
                    //DateTime fromDt = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime toDt = DateTime.ParseExact(Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var fromDt = CommonFunc.ParseFormattedDate(Fromdate);
                    var toDt = CommonFunc.ParseFormattedDate(Todate);

                    SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                    SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                    SqlParams.Add(new SqlParameter("@ToDate", toDt));
                    SqlParams.Add(new SqlParameter("@DashTypeSummDetail", "DETAIL"));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillItems(string returnByEmpName, string showAllItem)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItem"));
                SqlParams.Add(new SqlParameter("@returnByEmpName", returnByEmpName));
                SqlParams.Add(new SqlParameter("@ShowAllItem", showAllItem));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(string returnByEmpName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
                SqlParams.Add(new SqlParameter("@returnByEmpName", returnByEmpName));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillBatchNo(int itemCode, int receivedByEmpId, DateTime? retFromDepEntrydate, int retFromDepYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillBatch"));
                SqlParams.Add(new SqlParameter("@ItemCode", itemCode));
                SqlParams.Add(new SqlParameter("@ReceivedByEmpId", receivedByEmpId));
                SqlParams.Add(new SqlParameter("@RetFromDepEntrydate", retFromDepEntrydate));
                SqlParams.Add(new SqlParameter("@RetFromDepYearCode", retFromDepYearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                _ResponseResult.StatusText = "Error: " + ex.Message;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetNewEntry(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@RetFromDepYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReturnFromDepartmentMainDetail", SqlParams);
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
        public async Task<ResponseResult> SaveRetFromDeptMain(ReturnFromDepartmentMainModel model, DataTable ReqGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                //DateTime entryDt = new DateTime();
                //DateTime reqDt = new DateTime();
                //DateTime retDt = new DateTime();
                //DateTime rettDt = new DateTime();
                //DateTime woDt = new DateTime();

                var entryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                var reqDt = CommonFunc.ParseFormattedDate(model.ReqDate);
                var retDt = CommonFunc.ParseFormattedDate(model.ReturnDate);
                var rettDt = CommonFunc.ParseFormattedDate(model.ReturnnDate);
                var Update = DateTime.Now.ToString("dd/MM/yyyy");
                var upDt = CommonFunc.ParseFormattedDate(Update);
                var AppDate = DateTime.Now.ToString("dd/MM/yyyy");
                var appDt = CommonFunc.ParseFormattedDate(AppDate);
                var toDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                SqlParams.Add(new SqlParameter("@RetFromDepEntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@RetFromDepYearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@RetFromDepEntrydate", entryDt));
                SqlParams.Add(new SqlParameter("@RetFromDepActualReturDate", retDt));
                SqlParams.Add(new SqlParameter("@ReturnByEmpId", model.ReturnByEmployee));
                SqlParams.Add(new SqlParameter("@DeptId", model.ReturnByDepartment));
                SqlParams.Add(new SqlParameter("@Remarks", model.IDMark));
                SqlParams.Add(new SqlParameter("@CC", "Test CC"));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", rettDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", 1));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", 102));
                SqlParams.Add(new SqlParameter("@LastUpdatedDate", upDt));
                SqlParams.Add(new SqlParameter("@Approved", "Y"));
                SqlParams.Add(new SqlParameter("@ReceivedByEmpId", 1003));
                SqlParams.Add(new SqlParameter("@ApprovalDate", appDt));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMC));

                SqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
                SqlParams.Add(new SqlParameter("@BatchNo", model.BatchNo));
                SqlParams.Add(new SqlParameter("@UniqueBatchno", model.UniqueBatchNo));
                SqlParams.Add(new SqlParameter("@ReturnDepName", model.ReturnByDepartment));
                SqlParams.Add(new SqlParameter("@returnByEmpName", model.ReturnByEmployee));
                SqlParams.Add(new SqlParameter("@partcode", model.PartCode));
                SqlParams.Add(new SqlParameter("@itemName", model.ItemName));
                SqlParams.Add(new SqlParameter("@ShowAllItem","Y"));
                SqlParams.Add(new SqlParameter("@Fromdate", DateTime.Now.AddDays(-30)));
                SqlParams.Add(new SqlParameter("@Todate", toDt));
                SqlParams.Add(new SqlParameter("@DashTypeSummDetail", "Summary"));
                SqlParams.Add(new SqlParameter("@DTSSGrid", ReqGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        private static ReturnFromDepartmentMainModel PrepareView(DataSet DS, ref ReturnFromDepartmentMainModel? model)
        {
            var ItemGrid = new List<ReturnFromDepartmentDetail>();
            model = new ReturnFromDepartmentMainModel();

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["RetFromDepEntryId"]);
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["RetFromDepYearCode"]);
            model.EntryDate = DS.Tables[0].Rows[0]["RetFromDepEntrydate"].ToString();
            model.SlipNo = DS.Tables[0].Rows[0]["RetFromDepSlipNo"].ToString();
            model.ReturnDate = DS.Tables[0].Rows[0]["RetFromDepActualReturnDate"].ToString();
            model.EntryByMC = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();
            model.ReturnByEmployee = Convert.ToInt32(DS.Tables[0].Rows[0]["ReturnByEmpId"]);
            model.ReturnByDepartment = Convert.ToInt32(DS.Tables[0].Rows[0]["DeptId"]);

            if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemGrid.Add(new ReturnFromDepartmentDetail
                    {
                        SeqNo = ItemGrid.Count + 1,  // Incremental SeqNo based on ItemGrid count
                        EntryId = Convert.ToInt32(row["RetFromDepEntryId"]),  // Bind from DataRow
                        YearCode = Convert.ToInt32(row["RetFromDepYearCode"]),
                        ItemCode = Convert.ToInt32(row["Itemcode"]),
                        Qty = Convert.ToInt32(row["Qty"]),
                        ReturnnDate = row["RetFromDepActualReturnDate"].ToString(),
                        BatchNo = row["BatchNo"].ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"].ToString(),
                        ProdValue = Convert.ToInt32(row["ProductValue"]),
                        IDMark = row["ItemIdentMark"].ToString(),  // Assuming ItemIdentMark is the correct field for IDMark
                        CC = row["CC"].ToString(),
                        ReasonOfReturn = row["ReasonOfDamage"].ToString(),
                        Damaged = row["IsItemDamaged"].ToString(),  // Assuming IsItemDamaged corresponds to the Damaged field
                        DamageDetail = row["DamageDetail"].ToString(),
                        Pic1 = row["ImageOfProduct1"].ToString(),
                        Pic2 = row["ImageOfProduct2"].ToString(),
                        Pic3 = row["ImageOfProduct3"].ToString(),
                        Pic4 = row["ImageOfProduct4"].ToString(),
                    });
                }
            }

            // Assign the populated grid
            model.ReturnDetailGrid = ItemGrid;

            return model;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@RetFromDepEntryId", ID));
                SqlParams.Add(new SqlParameter("@RetFromDepYearCode", YC));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ReturnFromDepartmentMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<ReturnFromDepartmentMainModel> GetViewByID(int ID, int YearCode)
        {
            var model = new ReturnFromDepartmentMainModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@RetFromDepEntryId", ID));
                SqlParams.Add(new SqlParameter("@RetFromDepYearCode", YearCode));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ReturnFromDepartmentMainDetail", SqlParams);

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
    }
}
