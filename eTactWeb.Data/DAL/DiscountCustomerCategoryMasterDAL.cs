using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class DiscountCustomerCategoryMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public DiscountCustomerCategoryMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
		public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				SqlParams.Add(new SqlParameter("@DiscountCustCatYearCode", YearCode));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesDiscountCustomerCategoryMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillDiscountCategory()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillDiscountCategory"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesDiscountCustomerCategoryMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}


		public async Task<ResponseResult> SaveDiscountCustomerCategoryMaster(DiscountCustomerCategoryMasterModel model)
		{
			var _ResponseResult = new ResponseResult();

			try
			{
				var sqlParams = new List<dynamic>();

				if (model.Mode == "U" || model.Mode == "V")
				{
					sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					sqlParams.Add(new SqlParameter("@DiscountCustCatEntryId", model.DiscountCustCatEntryId));
					sqlParams.Add(new SqlParameter("@LastUpdatedbyEmpId", model.LastUpdatedbyEmpId));
					sqlParams.Add(new SqlParameter("@LastupDationDate", model.LastupDationDate));
				}
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@DiscountCustCatEntryId", model.DiscountCustCatEntryId));
				}
					
					sqlParams.Add(new SqlParameter("@DiscountCustCatYearCode", model.DiscountCustCatYearCode));
					sqlParams.Add(new SqlParameter("@EffectiveFromDate", model.EffectiveFromDate));
					sqlParams.Add(new SqlParameter("@DiscountCategory", model.DiscountCategory));
					sqlParams.Add(new SqlParameter("@DiscountCatSlipNo", model.DiscountCatSlipNo));
					sqlParams.Add(new SqlParameter("@MaxDiscountPer", model.MaxDiscountPer));
					sqlParams.Add(new SqlParameter("@MinDiscountPer", model.MinDiscountPer));
					sqlParams.Add(new SqlParameter("@ApplicableOnAdvancePayment", model.ApplicableOnAdvancePayment));
					sqlParams.Add(new SqlParameter("@MinmumAdvancePaymentPercent", model.MinmumAdvancePaymentPercent));
					sqlParams.Add(new SqlParameter("@CategoryActive", model.CategoryActive));
					sqlParams.Add(new SqlParameter("@ApplicableMonthlyYearlyAfterEachSale", model.ApplicableMonthlyYearlyAfterEachSale));
					
					sqlParams.Add(new SqlParameter("@ActualEntryByEmpId", model.ApprovedByEmpId));
					sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
					sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
					sqlParams.Add(new SqlParameter("@CC", model.CC));
					sqlParams.Add(new SqlParameter("@ApprovedByEmpId", model.ApprovedByEmpId));
				

				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesDiscountCustomerCategoryMaster", sqlParams);
			}
			catch (Exception ex)
			{
				_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
				_ResponseResult.StatusText = "Error";
				_ResponseResult.Result = new { ex.Message, ex.StackTrace };
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
				SqlParams.Add(new SqlParameter("@MainMenu", "Discount Customer Category Master"));
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
        public async Task<ResponseResult> GetDashboardData(DiscountCustomerCategoryMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPSalesDiscountCustomerCategoryMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<DiscountCustomerCategoryMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new DiscountCustomerCategoryMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPSalesDiscountCustomerCategoryMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@Todate", ToDate);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    
                        model.DiscountCustomerCategoryMasterGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new DiscountCustomerCategoryMasterModel
                                                        {
                                                            DiscountCustCatEntryId = dr["DiscountCustCatEntryId"] != DBNull.Value ? Convert.ToInt32(dr["DiscountCustCatEntryId"]) : 0,
                                                            DiscountCustCatYearCode = dr["DiscountCustCatYearCode"] != DBNull.Value ? Convert.ToInt32(dr["DiscountCustCatYearCode"]) : 0,
                                                            DiscountCategory = dr["DiscountCategory"] != DBNull.Value ? Convert.ToString(dr["DiscountCategory"]) : string.Empty,
                                                            DiscountCatSlipNo = dr["DiscountCatSlipNo"] != DBNull.Value ? Convert.ToString(dr["DiscountCatSlipNo"]) : string.Empty,
                                                            EffectiveFromDate = dr["EffectiveFromDate"] != DBNull.Value ? Convert.ToDateTime(dr["EffectiveFromDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            MinDiscountPer = dr["MinDiscountPer"] != DBNull.Value ? Convert.ToDecimal(dr["MinDiscountPer"]) : 0,
                                                            MaxDiscountPer = dr["MaxDiscountPer"] != DBNull.Value ? Convert.ToDecimal(dr["MaxDiscountPer"]) : 0,
                                                            ApplicableMonthlyYearlyAfterEachSale = dr["ApplicableMonthlyYearlyAfterEachSale"] != DBNull.Value ? Convert.ToString(dr["ApplicableMonthlyYearlyAfterEachSale"]) : string.Empty,
                                                            ApplicableOnAdvancePayment = dr["ApplicableOnAdvancePayment"] != DBNull.Value ? Convert.ToString(dr["ApplicableOnAdvancePayment"]) : string.Empty,
                                                            MinmumAdvancePaymentPercent = dr["MinmumAdvancePaymentPercent"] != DBNull.Value ? Convert.ToDecimal(dr["MinmumAdvancePaymentPercent"]) : 0,
                                                            
                                                            CategoryActive = dr["CategoryActive"] != DBNull.Value ? Convert.ToString(dr["CategoryActive"]) : string.Empty,
                                                            EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                                            ActualEntryByEmpId = dr["ActualEntryByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryByEmpId"]) : 0,
                                                            ActualEntryByEmpName = dr["ActualEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEmployee"]) : string.Empty,
                                                            ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            LastUpdatedbyEmpId = dr["LastUpdatedbyEmpId"] != DBNull.Value ? Convert.ToInt32(dr["LastUpdatedbyEmpId"]) : 0,
                                                            LastUpdatedbyEmpName = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToString(dr["UpdatedByEmployee"]) : string.Empty,
                                                            LastupDationDate = dr["LastupDationDate"] != DBNull.Value ? Convert.ToDateTime(dr["LastupDationDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,
                                                            ApprovedByEmpId = dr["ApprovedByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["ApprovedByEmpId"]) : 0,

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

        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@DiscountCustCatEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@DiscountCustCatYearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesDiscountCustomerCategoryMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<DiscountCustomerCategoryMasterModel> GetViewByID(int ID, int YC)
        {
            var model = new DiscountCustomerCategoryMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@DiscountCustCatEntryId", ID));
                SqlParams.Add(new SqlParameter("@DiscountCustCatYearCode", YC));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPSalesDiscountCustomerCategoryMaster", SqlParams);

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
        private static DiscountCustomerCategoryMasterModel PrepareView(DataSet DS, ref DiscountCustomerCategoryMasterModel? model)
        {
            try
            {
                var ItemList = new List<DiscountCustomerCategoryMasterModel>();
                var DetailList = new List<DiscountCustomerCategoryMasterModel>();
                DS.Tables[0].TableName = "DiscountCustomerCategoryMaster";
                int cnt = 0;
                model.DiscountCustCatEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["DiscountCustCatEntryId"].ToString());
                model.DiscountCustCatYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["DiscountCustCatYearCode"].ToString());
                model.DiscountCategory = DS.Tables[0].Rows[0]["DiscountCategory"].ToString();
                model.DiscountCatSlipNo = DS.Tables[0].Rows[0]["DiscountCatSlipNo"].ToString();
                model.EffectiveFromDate = DS.Tables[0].Rows[0]["EffectiveFromDate"].ToString();
                model.MinDiscountPer = Convert.ToDecimal(DS.Tables[0].Rows[0]["MinDiscountPer"].ToString());
                model.MaxDiscountPer = Convert.ToDecimal(DS.Tables[0].Rows[0]["MaxDiscountPer"].ToString());
                model.ApplicableMonthlyYearlyAfterEachSale = DS.Tables[0].Rows[0]["ApplicableMonthlyYearlyAfterEachSale"].ToString();
                model.ApplicableOnAdvancePayment = DS.Tables[0].Rows[0]["ApplicableOnAdvancePayment"].ToString();
                model.MinmumAdvancePaymentPercent = Convert.ToDecimal(DS.Tables[0].Rows[0]["MinmumAdvancePaymentPercent"].ToString());
                
                model.CategoryActive = DS.Tables[0].Rows[0]["CategoryActive"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                model.ActualEntryByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmpId"].ToString());
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                model.LastUpdatedbyEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedbyEmpId"].ToString());
                model.LastupDationDate = DS.Tables[0].Rows[0]["LastupDationDate"].ToString();
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"].ToString());
                model.ApprovedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ApprovedByEmpId"].ToString());


                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
