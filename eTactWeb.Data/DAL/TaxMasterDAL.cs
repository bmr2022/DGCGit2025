using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    internal class TaxMasterDAL
    {
        private readonly ConnectionStringService _connectionStringService;
        public TaxMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        internal async Task<DataSet> BindAllDropDown()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BINDALLDROPDOWN"));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_TaxMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "TaxTypeList";
                    _ResponseResult.Result.Tables[1].TableName = "HSNList";
                    _ResponseResult.Result.Tables[2].TableName = "ParentGroupList";
                    _ResponseResult.Result.Tables[3].TableName = "SGSTHeadList";
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
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Tax"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Purchase order"));

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
        internal async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TaxMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<IList<TaxMasterDashboard>> GetSearchData(string TaxName, string TaxType, string HSNNo)
        {
            var DashBoard = new List<TaxMasterDashboard>();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
                SqlParams.Add(new SqlParameter("@TaxName", TaxName));
                SqlParams.Add(new SqlParameter("@TaxTypeName", TaxType));
                SqlParams.Add(new SqlParameter("@HSNNo", HSNNo));

                var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TaxMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    DataTable DT = _ResponseResult.Result.DefaultView.ToTable(true, "AccountCode", "AddInTaxable", "DisplayName", "Refundable", "TaxCategory", "TaxName", "TaxPercent", "TaxType", "HSNNo");
                    DT.TableName = "TAXMASTERDASHBOARD";

                    DashBoard = CommonFunc.DataTableToList<TaxMasterDashboard>(DT);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return DashBoard;
        }

        internal async Task<IList<TaxMasterDashboard>> GetDashBoardData()
        {
            var DashBoard = new List<TaxMasterDashboard>();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TaxMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    DataTable DT = _ResponseResult.Result.DefaultView.ToTable(true, "AccountCode", "AddInTaxable", "DisplayName", "Refundable", "TaxCategory", "TaxName", "TaxPercent", "TaxType", "HSNNo");
                    DT.TableName = "TAXMASTERDASHBOARD";

                    DashBoard = CommonFunc.DataTableToList<TaxMasterDashboard>(DT);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return DashBoard;
        }

        public async Task<ResponseResult> SaveTaxMaster(TaxMasterModel model, DataTable TaxMasterDT)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", model.Mode));
                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                SqlParams.Add(new SqlParameter("@TaxName", model.TaxName));
                SqlParams.Add(new SqlParameter("@ParentCode", model.ParentGroup));
                SqlParams.Add(new SqlParameter("@DisplayName", model.DisplayName));
                SqlParams.Add(new SqlParameter("@Refundable", model.Refundable));
                SqlParams.Add(new SqlParameter("@AddInTaxable", model.AddInTaxable));
                SqlParams.Add(new SqlParameter("@TaxPercent", model.TaxPercent));
                SqlParams.Add(new SqlParameter("@TaxType", model.TaxType));
                SqlParams.Add(new SqlParameter("@TaxTypeName", model.TaxTypeName));
                SqlParams.Add(new SqlParameter("@TaxCategory", model.TaxCategory));
                SqlParams.Add(new SqlParameter("@TaxApplicable", model.TaxApplicable));
                SqlParams.Add(new SqlParameter("@EffectiveDate", model.EffectiveDate));
                SqlParams.Add(new SqlParameter("@TaxAppliedOn", model.TaxAppliedOn));
                SqlParams.Add(new SqlParameter("@SGSTHead", model.SGSTHead));
                SqlParams.Add(new SqlParameter("@MainGroup", model.MainGroup));
                SqlParams.Add(new SqlParameter("@SubGroup", model.SubGroup));
                SqlParams.Add(new SqlParameter("@AccountType", model.AccountType));
                SqlParams.Add(new SqlParameter("@SubSubGroup", model.SubSubGroup));
                SqlParams.Add(new SqlParameter("@UnderGroup", model.UnderGroup));
                SqlParams.Add(new SqlParameter("@DTHSNDetail", TaxMasterDT));
                SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
                if (model.Mode == "UPDATE")
                {
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));

                }
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TaxMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<TaxMasterModel> ViewByID(int ID)
        {
            var model = new TaxMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_TaxMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTTaxMasterDetail = oDataSet.Tables[0];
                    var DTHSNDetail = oDataSet.Tables[1];

                    if (oDataSet.Tables.Count > 0 && DTTaxMasterDetail.Rows.Count > 0)
                    {
                        model.EntryID = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["Account_Code"]);
                        model.TaxName = DTTaxMasterDetail.Rows[0]["Tax_Name"].ToString();
                        model.TaxPercent = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["Tax_Percent"]);
                        model.DisplayName = DTTaxMasterDetail.Rows[0]["DisplayName"].ToString();
                        model.Refundable = DTTaxMasterDetail.Rows[0]["Refundable"].ToString();
                        model.ParentGroup = DTTaxMasterDetail.Rows[0]["Parent_code"].ToString();
                        model.AddInTaxable = DTTaxMasterDetail.Rows[0]["Add_In_Taxable"].ToString();
                        model.TaxType = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["TaxTypeId"]);
                        model.TaxCategory = DTTaxMasterDetail.Rows[0]["Local_Cen"].ToString();
                        model.TaxApplicable = DTTaxMasterDetail.Rows[0]["Tax_ApplicaleOn"].ToString();
                        model.EffectiveDate = Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["Effective_From_date"]).ToString("dd/MM/yyyy").Replace("-", "/");
                        model.TaxAppliedOn = DTTaxMasterDetail.Rows[0]["ApplicableOn"].ToString();
                        model.SGSTHead = DTTaxMasterDetail.Rows[0]["SGSTAccountCode"].ToString();
                        model.MainGroup = DTTaxMasterDetail.Rows[0]["MainGroup"].ToString();
                        model.AccountType = DTTaxMasterDetail.Rows[0]["AccountType"].ToString();
                        model.SubGroup = DTTaxMasterDetail.Rows[0]["SubGroup"].ToString();
                        model.SubSubGroup = DTTaxMasterDetail.Rows[0]["SubSubGroup"].ToString();
                        model.UnderGroup = DTTaxMasterDetail.Rows[0]["UnderGroup"].ToString();
                        model.CreatedByName = DTTaxMasterDetail.Rows[0]["UserName"].ToString();
                        model.CreatedBy = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["UID"]);
                        model.CreatedOn = string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["CreatedOn"]);

                        if (!string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString()))
                        {
                            model.UpdatedByName = DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString();

                            model.UpdatedBy = string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString()) ? 0 : Convert.ToInt32(DTTaxMasterDetail.Rows[0]["UpdatedBy"]);
                            model.UpdatedOn = string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["UpdatedOn"]);
                        }
                    }

                    if (oDataSet.Tables.Count > 0 && DTHSNDetail.Rows.Count > 0)
                    {
                        DTHSNDetail.TableName = "HSNDetail";
                        model.HSNDetailList = CommonFunc.DataTableToList<HSNDetail>(DTHSNDetail);
                        model.HSN = model.HSNDetailList.Select(x => x.HSNNO).ToList();
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
    }
}