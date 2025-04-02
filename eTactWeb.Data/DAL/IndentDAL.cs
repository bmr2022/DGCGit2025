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
    public class IndentDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }

        public IndentDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetSizeModelColour(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetSizeModelColour"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetBomRevNo(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomRevNo"));
                SqlParams.Add(new SqlParameter("@BomItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetBomQty(int ItemCode,int BomRevNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomQty"));
                SqlParams.Add(new SqlParameter("@BomItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@BOMRevNo", BomRevNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetBomChildDetail(int ItemCode, int BomRevNo, int BomQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBomChildDetail"));
                SqlParams.Add(new SqlParameter("@BomItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@BOMRevNo", BomRevNo));
                SqlParams.Add(new SqlParameter("@Bomqty", BomQty));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }  
        public async Task<ResponseResult> FillFGItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetFGItemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }   
        public async Task<ResponseResult> FillFGPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetFGPartcode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }  
        public async Task<ResponseResult> FillStoreList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetStoreDetail"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }  
        public async Task<ResponseResult> FillVendorList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAccountDetail"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }  
        public async Task<ResponseResult> FillDeptList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetDepartmentDetail"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetServiceItems()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetServicesItem"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", ItemCode));
                SqlParams.Add(new SqlParameter("@STORE_ID", Storeid));
                SqlParams.Add(new SqlParameter("@TILL_DATE", DateTime.Now));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("GETSTORETotalSTOCK", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Indent"));

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
        internal async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
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
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Indent", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "BranchList";
                    _ResponseResult.Result.Tables[1].TableName = "ProjectList";
                    _ResponseResult.Result.Tables[2].TableName = "DeptList";
                    _ResponseResult.Result.Tables[3].TableName = "CostCenterList";
                    _ResponseResult.Result.Tables[4].TableName = "EmpList";
                    _ResponseResult.Result.Tables[5].TableName = "MachineList";
                    _ResponseResult.Result.Tables[6].TableName = "StoreList";
                    _ResponseResult.Result.Tables[7].TableName = "PartCodeList";
                    _ResponseResult.Result.Tables[8].TableName = "ItemNameList";
                    _ResponseResult.Result.Tables[9].TableName = "VendorList";

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

        public async Task<ResponseResult> SaveIndentDetail(IndentModel model, DataTable IndentGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                DateTime entDt = new DateTime();
                DateTime InDt = new DateTime();
                DateTime AppDt = new DateTime();

                entDt = ParseDate(model.EntryDate);
                InDt = ParseDate(model.IndentDate);
                AppDt = ParseDate(model.ApprovedDate);


                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", DateTime.Now));
                    SqlParams.Add(new SqlParameter("@Approved", model.Approved == null ? string.Empty : model.Approved));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    SqlParams.Add(new SqlParameter("@FullyApproved", model.Approved));
                    SqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));

                }

                SqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@IndentNo", model.IndentNo));
                SqlParams.Add(new SqlParameter("@IndentDate", InDt == default ? string.Empty : InDt));
                SqlParams.Add(new SqlParameter("@ItemService", model.ItemService));
                SqlParams.Add(new SqlParameter("@BOMIND", model.BOMIND));
                SqlParams.Add(new SqlParameter("@BOMRevNo", model.BomRevNo));
                SqlParams.Add(new SqlParameter("@BomItemCode", model.BomItemCode));
                SqlParams.Add(new SqlParameter("@Bomqty", model.Bomqty));
                SqlParams.Add(new SqlParameter("@IndentorEmpId", model.IndentorEmpId));
                SqlParams.Add(new SqlParameter("@DepartmentId", model.DepartmentId));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@ApprovedbyEmpId", model.ApprovedbyEmpId));
                SqlParams.Add(new SqlParameter("@ApprovedDate", AppDt == default ? string.Empty : AppDt));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@IndentRemark", model.IndentRemark));
                SqlParams.Add(new SqlParameter("@IndentCompleted", model.IndentCompleted));
                SqlParams.Add(new SqlParameter("@MRPNO", model.MRPNO));
                SqlParams.Add(new SqlParameter("@MRPEntryId", model.MRPEntryId));
                SqlParams.Add(new SqlParameter("@MRPyearcode", model.MRPyearcode));
                SqlParams.Add(new SqlParameter("@MachineNo", model.MachineNo));
                SqlParams.Add(new SqlParameter("@canceled", model.canceled));
                SqlParams.Add(new SqlParameter("@closed", model.closed));

                SqlParams.Add(new SqlParameter("@DTItemGrid", IndentGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<IndentModel> GetViewByID(int ID, string Mode, int YC)
        {
            var model = new IndentModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Indent", SqlParams);

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

        private static IndentModel PrepareView(DataSet DS, ref IndentModel? model, string Mode)
        {
            var ItemList = new List<IndentDetail>();
            DS.Tables[0].TableName = "VIndentMainDetail";
            var BOMIND = "";
            if (DS.Tables[0].Rows[0]["BOMIND"].ToString() == "Ind")
            {
                BOMIND = "Ind";
            }
            else
            {
                BOMIND = "BOM";
            }
            int cnt = 1;

            model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryId"]);
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"]);
            model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            model.IndentNo = DS.Tables[0].Rows[0]["IndentNo"].ToString();
            model.IndentDate = DS.Tables[0].Rows[0]["IndentDate"].ToString();
            model.ItemService = DS.Tables[0].Rows[0]["itemservice"].ToString();
            model.BOMIND = BOMIND;
            model.BomItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["BomItemCode"]);
            model.BomRevNo = Convert.ToInt32(DS.Tables[0].Rows[0]["BomRevNo"]);
            model.BomPartCode = DS.Tables[0].Rows[0]["BOMPartCode"].ToString();
            model.BomItemName = DS.Tables[0].Rows[0]["BOMtem"].ToString();
            model.Bomqty = Convert.ToInt32(DS.Tables[0].Rows[0]["Bomqty"]);
            model.DepartmentId = Convert.ToInt32(DS.Tables[0].Rows[0]["DepartmentId"]);
            model.IndentorEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["IndentorEmpId"]);
            model.DeptName = DS.Tables[0].Rows[0]["DeptName"].ToString();
            model.Approved = DS.Tables[0].Rows[0]["Approved"].ToString();
            model.ApprovedbyEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ApprovedbyEmpId"]);
            model.ApprovedDate = DS.Tables[0].Rows[0]["ApprovedDate"].ToString();
            model.UID = Convert.ToInt32(DS.Tables[0].Rows[0]["UID"].ToString());
            model.IndentRemark = DS.Tables[0].Rows[0]["ItemRemark"].ToString();
            model.IndentCompleted = DS.Tables[0].Rows[0]["IndentCompleted"].ToString();
            model.canceled = DS.Tables[0].Rows[0]["canceled"].ToString();
            model.closed = DS.Tables[0].Rows[0]["closed"].ToString();
            model.firstapproved = DS.Tables[0].Rows[0]["firstapproved"].ToString();
            model.firstapprovedby = Convert.ToInt32(DS.Tables[0].Rows[0]["firstapprovedby"]);
            model.firstapproveddate = DS.Tables[0].Rows[0]["firstapproveddate"].ToString();
            model.MRPNO = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPNO"].ToString());
            model.MRPEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPEntryId"]);
            model.MRPyearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPyearcode"]);
            model.MachineNo = DS.Tables[0].Rows[0]["MachineNo"].ToString();
            model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedBy"]);
            model.CreatedByName = DS.Tables[0].Rows[0]["CreatedByName"].ToString();

            if (Mode == "U")
            {
                if (DS.Tables[0].Rows[0]["LastUpdatedName"].ToString() != "")
                {
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedName"].ToString();
                    model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"].ToString();
                }
            }


            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new IndentDetail
                    {
                        SeqNo = cnt++,
                        ItemCode = Convert.ToInt32(row["ItemCode"]),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        PendReqNo = row["PendReqNo"].ToString(),
                        ReqYearCode = Convert.ToInt32(row["ReqYearCode"]),
                        Specification = row["Specification"].ToString(),
                        Qty = Convert.ToInt32(row["Qty"]),
                        PendQtyForPO = Convert.ToInt32(row["PendQtyForPO"]),
                        Unit = row["Unit"].ToString(),
                        AltQty = Convert.ToInt32(row["AltQty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        StoreID = Convert.ToInt32(row["StoreID"]),
                        StoreName = row["StoreName"].ToString(),
                        TotalStock = Convert.ToInt32(row["TotalStock"]),
                        PendAltQtyForPO = Convert.ToInt32(row["PendAltQtyForPO"]),
                        ReqDate = row["ReqDate"].ToString(),
                        AccountCode1 = Convert.ToInt32(row["AccountCode1"]),
                        AccountCode2 = Convert.ToInt32(row["AccountCode2"]),
                        AccountName1 = row["Account_Name"].ToString(),
                        AccountName2 = row["Account_Name2"].ToString(),
                        Model = row["Model"].ToString(),
                        Size = row["Size"].ToString(),
                        Color = row["Color"].ToString(),
                        ItemRemark = row["ItemRemark"].ToString(),
                        ReqQty = Convert.ToInt32(row["ReqQty"]),
                        Approvalue = Convert.ToInt32(row["Approvalue"]),
                        ItemDescription = row["ItemDescription"].ToString(),
                    });
                }
                model.IndentDetails = ItemList;
            }

            return model;
        }

        internal async Task<ResponseResult> GetDashboardData(IndentDashboard model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime StDt = new DateTime();
                StDt = ParseDate(model.FromDate);
                DateTime EndDt = new DateTime();
                EndDt = ParseDate(model.ToDate);
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var SqlParams = new List<dynamic>();
                var Flag = "";

                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDGRID"));
                SqlParams.Add(new SqlParameter("@FromDate", StDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", EndDt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@IndentNo", model.IndentNo));
                SqlParams.Add(new SqlParameter("@DeptName", model.DeptName));
                SqlParams.Add(new SqlParameter("@PartCode", model.PartCode));
                SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_Indent", SqlParams);
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_Indent", SqlParams);

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
