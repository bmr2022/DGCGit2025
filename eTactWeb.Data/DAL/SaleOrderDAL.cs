using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;


namespace eTactWeb.Data.DAL
{
    public class SaleOrderDAL
    {

        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration? configuration;

        private dynamic? _ResponseResult;

        public SaleOrderDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

		internal async Task<string> GetSOItem(object AccountCode, object SONO, object Year, int ItemCode)
		{
			var JsonString = string.Empty;
			try
			{
				var SqlParams = new List<dynamic>();

				SqlParams.Add(new SqlParameter("@Flag", "SOITEM"));
				SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
				SqlParams.Add(new SqlParameter("@SONO", SONO));
				SqlParams.Add(new SqlParameter("@YearCode", Year));
				SqlParams.Add(new SqlParameter("@ID", ItemCode));
				var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
				JsonString = JsonConvert.SerializeObject(ResponseResult);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return JsonString;
		}
		public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Sales Order"));
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
        public async Task<ResponseResult> GetFormRightsAmm(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Sale Order Amendment"));
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
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@SODate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> NewAmmEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetNewAmmEntry"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTotalStockList(int store, int Itemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ITEM_CODE", Itemcode));
                SqlParams.Add(new SqlParameter("@STORE_ID", store));
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
        public async Task<ResponseResult> GetAllowMultiBuyerProp()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CheckAllowMultiBuyerSO"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> CheckOrderNo(int year, int accountcode, int entryid, string custorderno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@flag", "CheckOrderNo"));
				SqlParams.Add(new SqlParameter("@AccountCode", accountcode));
                SqlParams.Add(new SqlParameter("@YearCode", year));
				SqlParams.Add(new SqlParameter("@entryid", entryid));
				SqlParams.Add(new SqlParameter("@CustOrderNo", custorderno));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> GetCurrency(string Currency)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Date", DateTime.Now));
                SqlParams.Add(new SqlParameter("@Currency", Currency));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("getExchangeRate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetAltQty(int ItemCode, float UnitQty, float ALtQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@itemcode", ItemCode));
                SqlParams.Add(new SqlParameter("@UnitQty", UnitQty));
                SqlParams.Add(new SqlParameter("@ALtQty", ALtQty));
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
        public async Task<ResponseResult> GetLockedYear(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FinancialYearLocking"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@Module", "Sale"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAddress(string Code)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "GetAddress");
                        oCmd.Parameters.AddWithValue("@ID", Code);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable.Rows[0]["Address"]
                            };
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetFillCurrency(string CTRL)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_GetDropDownList", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "FILLCURRENCY");
                        oCmd.Parameters.AddWithValue("@CTRL", CTRL);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable
                            };
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetItemPartCode(string Code, string Flag)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", Code);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable
                            };
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
            }
            return _ResponseResult;
        }

		public async Task<ResponseResult> GetExcelData(string Code)
		{
			var oDataTable = new DataTable();

			try
			{
				using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
				{
					using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
					{
						oCmd.CommandType = CommandType.StoredProcedure;
						oCmd.Parameters.AddWithValue("@Flag", "GetExcelItemDate");
						oCmd.Parameters.AddWithValue("@PartCode", Code);
						await myConnection.OpenAsync();
						using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
						{
							oDataAdapter.Fill(oDataTable);
						}

						if (oDataTable.Rows.Count > 0)
						{
							_ResponseResult = new ResponseResult()
							{
								StatusCode = HttpStatusCode.OK,
								StatusText = "Success",
								Result = oDataTable
							};
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
				if (oDataTable != null)
				{
					oDataTable.Dispose();
				}
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPartyList(string Check)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_Bom", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Check);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)oDataTable.Rows[0]["StatusCode"],
                                StatusText = (string)oDataTable.Rows[0]["StatusText"],
                                Result = (string)oDataTable.Rows[0]["Result"]
                            };
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetQuotData(string Code, string Flag)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", Code);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable
                            };
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
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
        //public async Task<ResponseResult> SaveSaleOrder(DataTable DTItemGrid, DataTable DTSchedule, DataTable DTTaxGrid, DataTable MultiBuyersDT, SaleOrderModel model)
        //{
        //    var oDataTable = new DataTable();
        //    var SqlParams = new List<dynamic>();
        //    try
        //    {
        //        await using SqlConnection myConnection = new SqlConnection(DBConnectionString);
        //        SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        DateTime soDt = new DateTime();
        //        DateTime wef = new DateTime();
        //        DateTime soclDt = new DateTime();
        //        DateTime QDt = new DateTime();
        //        DateTime AmmEDt = new DateTime();
        //        DateTime soconDt = new DateTime();
        //        DateTime soDlDt = new DateTime();
        //        if (model.Mode == "SOA")
        //            oCmd.Parameters.AddWithValue("@SOAmendYC", model.AmmYearCode);

        //        soDt = ParseDate(model.SODate);
        //        wef = ParseDate(model.WEF);
        //        soclDt = ParseDate(model.SOCloseDate);
        //        QDt = ParseDate(model.QDate);
        //        AmmEDt = ParseDate(model.AmmEffDate);
        //        soconDt = ParseDate(model.SOConfirmDate);
        //        soDlDt = ParseDate(model.SODeliveryDate);



        //        oCmd.Parameters.AddWithValue("@Flag", model.Mode);
        //        oCmd.Parameters.AddWithValue("@EntryID", model.EntryID);
        //        oCmd.Parameters.AddWithValue("@YearCode", model.YearCode);
        //        oCmd.Parameters.AddWithValue("@Branch", model.Branch);
        //        oCmd.Parameters.AddWithValue("@SOFor", model.SOFor);
        //        oCmd.Parameters.AddWithValue("@SOType", model.SOType);
        //        oCmd.Parameters.AddWithValue("@CurrencyID", model.CurrencyID);
        //        oCmd.Parameters.AddWithValue("@AccountCode", model.AccountCode);
        //        oCmd.Parameters.AddWithValue("@Address", model.Address);
        //        oCmd.Parameters.AddWithValue("@DeliveryAddress", model.DeliveryAddress);
        //        oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
        //        oCmd.Parameters.AddWithValue("@CustOrderNo", model.CustOrderNo);
        //        oCmd.Parameters.AddWithValue("@SONo", model.SONo);
        //        oCmd.Parameters.AddWithValue("@ENTRYDATE", ParseDate(model.EntryDate));
        //        //SqlParams.Add(new SqlParameter("@EntryDate", DateTime.ParseExact(model.EntryDate.ToString(), "dd-mm-yyyy", CultureInfo.InvariantCulture)));

        //        oCmd.Parameters.AddWithValue("@SODate", soDt == default ? string.Empty : soDt);
        //        oCmd.Parameters.AddWithValue("@WEF", wef == default ? string.Empty : wef);
        //        oCmd.Parameters.AddWithValue("@SOCloseDate", soclDt == default ? string.Empty : soclDt);

        //        oCmd.Parameters.AddWithValue("@QuotNo", model.QuotNo);
        //        oCmd.Parameters.AddWithValue("@QDate", soclDt == default ? string.Empty : soclDt);

        //        oCmd.Parameters.AddWithValue("@QuotYear", model.QuotYear);
        //        oCmd.Parameters.AddWithValue("@AmmNo", model.AmmNo);
        //        oCmd.Parameters.AddWithValue("@AmmEffDate", AmmEDt == default ? string.Empty : AmmEDt);
        //        oCmd.Parameters.AddWithValue("@SOConfirmDate", soconDt == default ? string.Empty : soconDt);

        //        oCmd.Parameters.AddWithValue("@ConsigneeAccountCode", model.ConsigneeAccountCode);
        //        oCmd.Parameters.AddWithValue("@ConsigneeAddress", model.ConsigneeAddress);
        //        oCmd.Parameters.AddWithValue("@FreightPaidBy", model.FreightPaidBy);
        //        oCmd.Parameters.AddWithValue("@PackingChgApplicable", model.PackingChgApplicable);
        //        oCmd.Parameters.AddWithValue("@ModeTransport", model.TransportMode);
        //        oCmd.Parameters.AddWithValue("@Remark", model.SORemark);
        //        oCmd.Parameters.AddWithValue("@DeliverySch", model.DeliverySch);
        //        oCmd.Parameters.AddWithValue("@DeliveryTerms", model.DeliveryTerms);
        //        oCmd.Parameters.AddWithValue("@PreparedBy", model.PreparedBy);
        //        oCmd.Parameters.AddWithValue("@PortOfDischarge", model.PortOfDischarge);
        //        oCmd.Parameters.AddWithValue("@ResponsibleSalesPersonID", model.ResponsibleSalesPersonID);
        //        oCmd.Parameters.AddWithValue("@CustContactPerson", model.CustContactPerson);
        //        oCmd.Parameters.AddWithValue("@SaleDocType", model.SaleDocType);
        //        oCmd.Parameters.AddWithValue("@OtherDetail", model.OtherDetail);
        //        oCmd.Parameters.AddWithValue("@PortToLoading", model.Port2Loading);
        //        oCmd.Parameters.AddWithValue("@InsuApplicable", model.InsuApplicable);
        //        oCmd.Parameters.AddWithValue("@SODeliveryDate", soDlDt == default ? string.Empty : soDlDt);

        //        oCmd.Parameters.AddWithValue("@OrderAmt", model.ItemNetAmount);
        //        oCmd.Parameters.AddWithValue("@DTItemGrid", DTItemGrid);
        //        oCmd.Parameters.AddWithValue("@OrderNetAmt", model.NetTotal);
        //        oCmd.Parameters.AddWithValue("@DTSchedule", DTSchedule);
        //        oCmd.Parameters.AddWithValue("@DTTaxGrid", DTTaxGrid);
        //        oCmd.Parameters.AddWithValue("@DTBuyerGrid", MultiBuyersDT);
        //        oCmd.Parameters.AddWithValue("@RoundOff", model.TotalRoundOff);
        //        oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

        //        await myConnection.OpenAsync();

        //        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
        //        oDataAdapter.Fill(oDataTable);

        //        if (oDataTable.Rows.Count > 0)
        //        {
        //            _ResponseResult = new ResponseResult()
        //            {
        //                StatusCode = HttpStatusCode.OK,
        //                StatusText = "Success",
        //                Result = oDataTable
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //        Error.StackTrace = ex.StackTrace;
        //        Error.Exception = ex;
        //        _ResponseResult = new ResponseResult()
        //        {
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            StatusText = Error.Message,
        //            Result = oDataTable
        //        };
        //    }
        //    finally
        //    {
        //        oDataTable.Dispose();
        //    }
        //    return _ResponseResult;
        //}


        public async Task<ResponseResult> SaveSaleOrder(DataTable DTItemGrid, DataTable DTSchedule, DataTable DTTaxGrid, DataTable MultiBuyersDT, SaleOrderModel model)
        {
            var oDataTable = new DataTable();
            var SqlParams = new List<dynamic>();
            try
            {
                // await using SqlConnection myConnection = new SqlConnection(DBConnectionString);
                // SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection)
                // {
                //     CommandType = CommandType.StoredProcedure
                // };
                //DateTime soDt = new DateTime();
                //DateTime wef = new DateTime();
                //DateTime soclDt = new DateTime();
                //DateTime QDt = new DateTime();
                //DateTime AmmEDt = new DateTime();
                //DateTime soconDt = new DateTime();
                //DateTime soDlDt = new DateTime();
                if (model.Mode == "SOA")
                    SqlParams.Add(new SqlParameter("@SOAmendYC", model.AmmYearCode));

                var soDt = CommonFunc.ParseFormattedDate(model.SODate);
                var wef = CommonFunc.ParseFormattedDate(model.WEF);
                var soclDt = CommonFunc.ParseFormattedDate(model.SOCloseDate);
                var QDt = CommonFunc.ParseFormattedDate(model.QDate);
                var AmmEDt = CommonFunc.ParseFormattedDate(model.AmmEffDate);
                var soconDt = CommonFunc.ParseFormattedDate(model.SOConfirmDate);
                var soDlDt = CommonFunc.ParseFormattedDate(model.SODeliveryDate);
                var entDt = CommonFunc.ParseFormattedDate(model.EntryDate);



                SqlParams.Add(new SqlParameter("@Flag", model.Mode));
                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
                SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
                SqlParams.Add(new SqlParameter("@Branch", model.Branch));
                SqlParams.Add(new SqlParameter("@SOFor", model.SOFor));
                SqlParams.Add(new SqlParameter("@SOType", model.SOType));
                SqlParams.Add(new SqlParameter("@CurrencyID", model.CurrencyID));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@Address", model.Address));
                SqlParams.Add(new SqlParameter("@DeliveryAddress", model.DeliveryAddress));
                SqlParams.Add(new SqlParameter("@OrderType", model.OrderType));
                SqlParams.Add(new SqlParameter("@CustOrderNo", model.CustOrderNo));
                SqlParams.Add(new SqlParameter("@SONo", model.SONo));
                SqlParams.Add(new SqlParameter("@ENTRYDATE", entDt));
                //SqlParams.Add(new SqlParameter("@EntryDate", DateTime.ParseExact(model.EntryDate.ToString(), "dd-mm-yyyy", CultureInfo.InvariantCulture)));

                SqlParams.Add(new SqlParameter("@SODate", soDt == default ? string.Empty : soDt));
                SqlParams.Add(new SqlParameter("@WEF", wef == default ? string.Empty : wef));
                SqlParams.Add(new SqlParameter("@SOCloseDate", soclDt == default ? string.Empty : soclDt));

                SqlParams.Add(new SqlParameter("@QuotNo", model.QuotNo));
                SqlParams.Add(new SqlParameter("@QDate", soclDt == default ? string.Empty : soclDt));

                SqlParams.Add(new SqlParameter("@QuotYear", model.QuotYear));
                SqlParams.Add(new SqlParameter("@AmmNo", model.AmmNo));
                SqlParams.Add(new SqlParameter("@AmmEffDate", AmmEDt == default ? string.Empty : AmmEDt));
                SqlParams.Add(new SqlParameter("@SOConfirmDate", soconDt == default ? string.Empty : soconDt));

                SqlParams.Add(new SqlParameter("@ConsigneeAccountCode", model.ConsigneeAccountCode));
                SqlParams.Add(new SqlParameter("@ConsigneeAddress", model.ConsigneeAddress));
                SqlParams.Add(new SqlParameter("@FreightPaidBy", model.FreightPaidBy));
                SqlParams.Add(new SqlParameter("@PackingChgApplicable", model.PackingChgApplicable));
                SqlParams.Add(new SqlParameter("@ModeTransport", model.TransportMode));
                SqlParams.Add(new SqlParameter("@Remark", model.SORemark));
                SqlParams.Add(new SqlParameter("@DeliverySch", model.DeliverySch));
                SqlParams.Add(new SqlParameter("@DeliveryTerms", model.DeliveryTerms));
                SqlParams.Add(new SqlParameter("@PreparedBy", model.PreparedBy));
                SqlParams.Add(new SqlParameter("@PortOfDischarge", model.PortOfDischarge));
                SqlParams.Add(new SqlParameter("@ResponsibleSalesPersonID", model.ResponsibleSalesPersonID));
                SqlParams.Add(new SqlParameter("@CustContactPerson", model.CustContactPerson));
                SqlParams.Add(new SqlParameter("@SaleDocType", model.SaleDocType));
                SqlParams.Add(new SqlParameter("@OtherDetail", model.OtherDetail));
                SqlParams.Add(new SqlParameter("@PortToLoading", model.Port2Loading));
                SqlParams.Add(new SqlParameter("@InsuApplicable", model.InsuApplicable));
                SqlParams.Add(new SqlParameter("@SODeliveryDate", soDlDt == default ? string.Empty : soDlDt));

                SqlParams.Add(new SqlParameter("@OrderAmt", model.ItemNetAmount));
                SqlParams.Add(new SqlParameter("@DTItemGrid", DTItemGrid));
                SqlParams.Add(new SqlParameter("@OrderNetAmt", model.NetTotal));
                SqlParams.Add(new SqlParameter("@DTSchedule", DTSchedule));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", DTTaxGrid));
                SqlParams.Add(new SqlParameter("@DTBuyerGrid", MultiBuyersDT));
                SqlParams.Add(new SqlParameter("@RoundOff", model.TotalRoundOff));
                SqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));

                // await myConnection.OpenAsync();
                // 
                // using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                // oDataAdapter.Fill(oDataTable);
                // 
                // if (oDataTable.Rows.Count > 0)
                // {
                //     _ResponseResult = new ResponseResult()
                //     {
                //         StatusCode = HttpStatusCode.OK,
                //         StatusText = "Success",
                //         Result = oDataTable
                //     };
                // }
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                Error.StackTrace = ex.StackTrace;
                Error.Exception = ex;
                _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = Error.Message,
                    Result = oDataTable
                };
            }
            finally
            {
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@ID", ID);
                        oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                        await myConnection.OpenAsync();

                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = oDataTable.Rows[0]["StatusText"].ToString(),
                                Result = oDataTable.Rows[0]["Result"]
                            };
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
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }

        public async Task<SaleOrderDashboard> GetAmmDashboardData()
        {
            var Result = new SaleOrderDashboard();
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime now = DateTime.Now;
                        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "AMMDASHBOARD");
                        oCmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                        oCmd.Parameters.AddWithValue("@EndDate", DateTime.Today);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONo", "Year", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                                "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "CustomerAddress", "DeliveryAddress", "Consignee", "ConsigneeAddress",
                                "OrderAmt", "OrderNetAmt", "SOConfirmDate", "POActive", "SODate", "Approved", "SoComplete", "ApprovedDate", "Active", "UpdatedOn", "CreatedOn");
                        oDT.TableName = "AMMDASHBOARD";

                        Result.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return Result;
        }

        public async Task<SaleOrderDashboard> GetAmmSearchData(SaleOrderDashboard model)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime StartDate = new DateTime();
                        DateTime EndDate = new DateTime();
                        StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "AMMSEARCH");
                        oCmd.Parameters.AddWithValue("@Branch", model.CustomerName);
                        oCmd.Parameters.AddWithValue("@CustOrderNo", model.CustOrderNo);
                        oCmd.Parameters.AddWithValue("@SONo", model.SONo);
                        oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                        oCmd.Parameters.AddWithValue("@SOType", model.SOType);
                        oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                        oCmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy/MM/dd"));
                        oCmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy/MM/dd"));
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONo", "Year", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                        "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "CustomerAddress", "DeliveryAddress", "Consignee", "ConsigneeAddress",
                        "OrderAmt", "OrderNetAmt", "SOConfirmDate", "POActive", "SODate", "Approved", "SoComplete", "ApprovedDate", "Active", "UpdatedOn", "CreatedOn");


                        oDT.TableName = "AMMDASHBOARD";

                        model.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return model;
        }

        public async Task<SaleOrderDashboard> GetUpdAmmData(SaleOrderDashboard model)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime StartDate = new DateTime();
                        DateTime EndDate = new DateTime();
                        StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "UPDATEAMMSEARCH");
                        oCmd.Parameters.AddWithValue("@Branch", model.CustomerName);
                        oCmd.Parameters.AddWithValue("@CustOrderNo", model.CustOrderNo);
                        oCmd.Parameters.AddWithValue("@SONo", model.SONo);
                        oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                        oCmd.Parameters.AddWithValue("@SOType", model.SOType);
                        oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                        oCmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy/MM/dd"));
                        oCmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy/MM/dd"));
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONo", "Year", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                        "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "CustomerAddress", "DeliveryAddress", "Consignee", "ConsigneeAddress",
                        "OrderAmt", "OrderNetAmt", "SOConfirmDate", "POActive", "SODate", "Approved", "SoComplete", "ApprovedDate", "Active", "UpdatedOn", "CreatedOn");

                        oDT.TableName = "AMMDASHBOARD";

                        model.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return model;
        }

        internal async Task<object> GetAmmStatus(int EntryID, int YearCode)
        {
            object AmmStatus = 0;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;

                        oCmd.Parameters.AddWithValue("@Flag", "CHECKAMMSTATUS");
                        oCmd.Parameters.AddWithValue("@EntryID", EntryID);
                        oCmd.Parameters.AddWithValue("@YearCode", YearCode);

                        await myConnection.OpenAsync();
                        AmmStatus = await oCmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return AmmStatus;
        }

        internal async Task<ResponseResult> GetCurrencyDetail(string CurrentDate, string Currency)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("getExchangeRate", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Date", CurrentDate);
                        oCmd.Parameters.AddWithValue("@Currency", Currency);
                        await myConnection.OpenAsync();
                        //var val = await oCmd.ExecuteScalarAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);
                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable
                            };
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
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }

        public async Task<SaleOrderDashboard> GetDashboardData(string EndDate)
        {
            var Result = new SaleOrderDashboard();
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime now = DateTime.Now;
                        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                        oCmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                        oCmd.Parameters.AddWithValue("@EndDate", EndDate);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        //IEnumerable<DataRow> sequence = oDataTable.AsEnumerable();
                        //List<DataRow> list = oDataTable.AsEnumerable().ToList();

                        //List<DataRow> list2 = new List<DataRow>();
                        //foreach (DataRow dr in oDataTable.Rows)
                        //{
                        //    list2.Add(dr);
                        //}
                        
                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID","EntryDate","EntryTime", "SONo", "Year", "SODate","AccountCode", "CustomerName", "CustOrderNo", "CC",
                                "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency","QuotNo","QDate","QuotYear", "CustomerAddress", "ConsigneeAddress", "Consignee",
                                "AmmNo","AmmEffDate","Address","DeliveryAddress","ConsigneeAccountCode","OrderAmt","OrderNetAmt", "FreightPaidBy", "InsuApplicable", "ModeTransport","DeliverySch",
                                "PackingChgApplicable", "DeliveryTerms", "SOComplete", "PreparedBy", "TotalDiscount", "SODeliveryDate", "TotalDisPercent", "TotalDiscAmt", "DespatchAdviseComplete", "PortToLoading", "PortOfDischarge",
                                "ResponsibleSalesPersonID","CustContactPerson","SaleDocType","OtherDetail","SOConfirmDate","OrderDelayReason","Approved","ApprovedDate","ApprovedBy", "UID","UpdatedOn","UpdatedBy", "CreatedOn","RoundOff",
                                "EntryByMachineName");
                        oDT.TableName = "SODASHBOARD";

                        Result.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return Result;
        }
        public static string ConvertToDesiredFormat(string inputDate)
        {
            // string[] dateFormats = { "dd.MM.yy","dd.MM.yyyy", "dd/M/yy", "d.M.yy", "d/M/yy", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };
            string[] dateFormats = {
            "dd/MM/yyyy",
            "MM-dd-yyyy",
            "yyyy.MM.dd",
            "MMMM d, yyyy",
             "dd.MM.yy","dd.MM.yyyy", "dd/M/yy", "d.M.yy", "d/M/yy", "yyyy-MM-dd", "MM/dd/yyyy"
        };


            //DateTime parsedDate;


            //DateTime inputDateobject = DateTime.ParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture);


            //string outputDateString = inputDateobject.ToString("yyyy/MM/dd");


            //if (DateTime.TryParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            //{
            //    return parsedDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            //}
            //else
            //{
            //    return "Invalid Date";
            //}

            //foreach (var dateString in dateFormats)
            //{
            // Try to parse the date string to a DateTime object
            if (DateTime.TryParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    // Convert the DateTime object to yyyy/MM/dd format
                    string outputDateString = parsedDate.ToString("yyyy/MM/dd");
                    //  return outputDateString;
                    return outputDateString;
                }
                else
                {
                    return "Invalid Date";
                }
            //}


        }

        internal async Task<SaleOrderDashboard> GetSearchData(SaleOrderDashboard model)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    //DateTime StartDate = new DateTime();
                    //DateTime EndDate = new DateTime();
                    //StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string StartDate,EndDate;

                    StartDate = ConvertToDesiredFormat(model.FromDate);
                    EndDate = ConvertToDesiredFormat(model.ToDate);

                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "SEARCH");
                        oCmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                        oCmd.Parameters.AddWithValue("@CustOrderNo", model.CustOrderNo);
                        oCmd.Parameters.AddWithValue("@Branch", model.CC);
                        oCmd.Parameters.AddWithValue("@SONo", model.SONo);
                        oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                        oCmd.Parameters.AddWithValue("@SOType", model.SOType);
                        oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                        oCmd.Parameters.AddWithValue("@PartCode", model.PartCode);
                        //SqlParams.Add(new SqlParameter("@EntryDate", DateTime.ParseExact(model.EntryDate.ToString(), "dd-mm-yyyy", CultureInfo.InvariantCulture)));

                        oCmd.Parameters.AddWithValue("@StartDate", StartDate);
                        oCmd.Parameters.AddWithValue("@EndDate", EndDate);

                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        if (model.SummaryDetail == "Summary")
                        {
                            var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "EntryDate", "EntryTime", "SONo", "Year", "SODate", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                              "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "QuotNo", "QDate", "QuotYear", "CustomerAddress", "ConsigneeAddress", "Consignee",
                              "AmmNo", "AmmEffDate", "Address", "DeliveryAddress", "ConsigneeAccountCode", "OrderAmt", "OrderNetAmt", "FreightPaidBy", "InsuApplicable", "ModeTransport", "DeliverySch",
                              "PackingChgApplicable", "DeliveryTerms", "SOComplete", "PreparedBy", "TotalDiscount", "SODeliveryDate", "TotalDisPercent", "TotalDiscAmt", "DespatchAdviseComplete", "PortToLoading", "PortOfDischarge",
                              "ResponsibleSalesPersonID", "CustContactPerson", "SaleDocType", "OtherDetail", "SOConfirmDate", "OrderDelayReason", "Approved", "ApprovedDate", "ApprovedBy", "UID", "UpdatedOn", "UpdatedBy", "CreatedOn", "RoundOff",
                              "EntryByMachineName");

                            oDT.TableName = "SODASHBOARD";

                            model.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
                        }
                        else
                        {
                            var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "EntryDate", "EntryTime", "SONo", "Year","ItemName","PartCode", "SODate", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                                "HSNNO","Qty","Unit","AltQty","AltUnit","Rate","OtherRateCurr","UnitRate","DiscPer","Amount","TolLimit", "Remark", "Description","StoreName","StockQty","AmendmentDate","AmendmentReason","Color",
                                "RejPer","Excessper","ProjQty1","ProjQty2", "Consignee",
                               "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "QuotNo", "QDate", "QuotYear", "CustomerAddress", "ConsigneeAddress",
                               "AmmNo", "AmmEffDate", "Address", "DeliveryAddress", "ConsigneeAccountCode", "OrderAmt", "OrderNetAmt", "FreightPaidBy", "InsuApplicable", "ModeTransport", "DeliverySch",
                               "PackingChgApplicable", "DeliveryTerms", "SOComplete", "PreparedBy", "TotalDiscount", "SODeliveryDate", "TotalDisPercent", "TotalDiscAmt", "DespatchAdviseComplete", "PortToLoading", "PortOfDischarge",
                               "ResponsibleSalesPersonID", "CustContactPerson", "SaleDocType", "OtherDetail", "SOConfirmDate", "OrderDelayReason", "Approved", "ApprovedDate", "ApprovedBy", "UID", "UpdatedOn", "UpdatedBy", "CreatedOn", "RoundOff",
                               "EntryByMachineName");

                            oDT.TableName = "SODETAILDASHBOARD";

                            model.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return model;
        }

        public async Task<SaleOrderDashboard> GetSOAmmCompleted()
        {
            var Result = new SaleOrderDashboard();
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime now = DateTime.Now;
                        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "SOCOMPLETED");
                        oCmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                        oCmd.Parameters.AddWithValue("@EndDate", DateTime.Today);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONo", "Year", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                        "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "CustomerAddress", "DeliveryAddress", "Consignee", "ConsigneeAddress", "AmmNo", "AmmEffDate",
                        "OrderAmt", "OrderNetAmt", "SOConfirmDate", "POActive", "SODate", "Approved", "SoComplete", "ApprovedDate", "Active", "UpdatedOn", "CreatedOn");
                        oDT.TableName = "SOCOMPLETED";

                        Result.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                if (ex.Source != null) Error.Source = ex.Source;
            }
            finally
            {
                oDataTable.Dispose();
            }
            return Result;
        }

        internal async Task<SaleOrderDashboard> GetSOAmmCompletedSearchData(SaleOrderDashboard model)
        {
            var oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        DateTime now = DateTime.Now;
                        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "SOAMMCOMPLETEDSEARCH");
                        oCmd.Parameters.AddWithValue("@Branch", model.CustomerName);
                        oCmd.Parameters.AddWithValue("@CustOrderNo", model.CustOrderNo);
                        oCmd.Parameters.AddWithValue("@SONo", model.SONo);
                        oCmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                        oCmd.Parameters.AddWithValue("@SOType", model.SOType);
                        oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                        oCmd.Parameters.AddWithValue("@StartDate",firstDayOfMonth);
                        oCmd.Parameters.AddWithValue("@EndDate", DateTime.Today);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataTable);

                        var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONo", "Year", "AccountCode", "CustomerName", "CustOrderNo", "CC",
                        "OrderType", "SOType", "SOFor", "WEF", "SOCloseDate", "Currency", "CustomerAddress", "DeliveryAddress", "Consignee", "ConsigneeAddress", "AmmNo", "AmmEffDate",
                        "OrderAmt", "OrderNetAmt", "SOConfirmDate", "POActive", "SODate", "Approved", "SoComplete", "ApprovedDate", "Active", "UpdatedOn", "CreatedOn");
                        oDT.TableName = "SOCOMPLETED";

                        model.SODashboard = CommonFunc.DataTableToList<SaleOrderDashboard>(oDT);
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
                oDataTable.Dispose();
            }
            return model;
        }

        internal async Task<SaleOrderModel> GetViewByID(int ID, int YearCode, string Flag)
        {
            var oDataSet = new DataSet();
            var model = new SaleOrderModel();
            var _ItemList = new List<ItemDetail>();
            var _TaxList = new List<TaxModel>();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "VIEWBYID");
                        oCmd.Parameters.AddWithValue("@EntryID", ID);
                        oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataSet);
                        oDataSet.Tables[0].TableName = "SOMain";
                        oDataSet.Tables[1].TableName = "ItemDetailGrid";
                        oDataSet.Tables[2].TableName = "SOTaxDetail";
                        oDataSet.Tables[3].TableName = "SODeliverySchedule";
                        oDataSet.Tables[4].TableName = "SOMultiBuyer";

                        var listObject = new List<DeliverySchedule>();
                        var BillToShipToList = new List<SaleOrderBillToShipTo>();

                        if (oDataSet.Tables[3] != null && oDataSet.Tables[3].Rows.Count > 0)
                        {
                            //listObject = oDataSet.Tables[3].AsEnumerable()
                            //             .Select(x => new DeliverySchedule()
                            //             {
                            //                 AltQty = (int)x.Field<double>("AltQty"),
                            //                 Qty = (int)x.Field<double>("Qty"),
                            //                 DPartCode = (int)x.Field<Int32>("ItemCode"),
                            //                 Date = (string)x.Field<String>("DeliveryDate"),
                            //                 Remarks = (string)x.Field<String>("Remark"),
                            //                 Days = (int)x.Field<Int32>("Days"),
                            //             }).ToList();

                            foreach (DataRow row in oDataSet.Tables[3].Rows)
                            {
                                listObject.Add(new DeliverySchedule
                                {
                                    AltQty = Convert.ToInt32(row["AltQty"]),
                                    Qty = Convert.ToInt32(row["Qty"]),
                                    DPartCode = Convert.ToInt32(row["ItemCode"]),
                                    ItemName = row["ItemCode"].ToString(),
                                    Date = string.IsNullOrEmpty(row["DeliveryDate"].ToString()) ? "" : row["DeliveryDate"].ToString(),
                                    Remarks = row["Remark"].ToString(),
                                    Days = Convert.ToInt32(row["Days"]),
                                });
                            }
                            model.DeliveryScheduleList = listObject;
                        }

                        if (oDataSet.Tables[4] != null && oDataSet.Tables[4].Rows.Count > 0)
                        {

                            foreach (DataRow row in oDataSet.Tables[4].Rows)
                            {
                                BillToShipToList.Add(new SaleOrderBillToShipTo
                                {
                                    SeqNo = Convert.ToInt32(row["SeqNo"]),
                                    MainCustomerId = Convert.ToInt32(row["MainCustomerId"]),
                                    MainCustomer = string.IsNullOrEmpty(row["MainCustomer"].ToString()) ? "" : row["MainCustomer"].ToString(),
                                    BillToAccountCode = Convert.ToInt32(row["BillToAccountCode"]),
                                    BillToAccountName = string.IsNullOrEmpty(row["BilltoAccount"].ToString()) ? "" : row["BilltoAccount"].ToString(),
                                    BuyerAddress = string.IsNullOrEmpty(row["BuyerAddress"].ToString()) ? "" : row["BuyerAddress"].ToString(),
                                    ShiptoAccountCode = Convert.ToInt32(row["ShiptoAccountCode"]),
                                    ShiptoAccountName = string.IsNullOrEmpty(row["ShipToAccountName"].ToString()) ? "" : row["ShipToAccountName"].ToString(),
                                    ShipToAddress = string.IsNullOrEmpty(row["ShipToAddress"].ToString()) ? "" : row["ShipToAddress"].ToString()
                                });
                            }
                            model.SaleOrderBillToShipTo = BillToShipToList;
                        }

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.EntryID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["EntryID"]);
                            model.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["YearCode"]);
                            model.EntryDate = Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["EntryDate"]).ToString("dd/MM/yyyy");
                            model.EntryTime = Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["EntryTime"]).ToString("HH:mm:ss")?.Trim();
                            model.Branch = oDataSet.Tables[0].Rows[0]["CC"].ToString();
                            model.SOFor = oDataSet.Tables[0].Rows[0]["SOFor"].ToString();
                            model.SOType = oDataSet.Tables[0].Rows[0]["SOType"].ToString();
                            model.CurrencyID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CurrencyID"]);
                            model.AccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AccountCode"]);
                            model.Address = oDataSet.Tables[0].Rows[0]["Address"].ToString();
                            model.DeliveryAddress = oDataSet.Tables[0].Rows[0]["DeliveryAddress"].ToString();
                            model.OrderType = oDataSet.Tables[0].Rows[0]["OrderType"].ToString();
                            model.CustOrderNo = oDataSet.Tables[0].Rows[0]["CustOrderNo"].ToString();
                            model.SONo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SONo"]);
                            model.SODate = oDataSet.Tables[0].Rows[0]["SODate"].ToString();
                            model.WEF = oDataSet.Tables[0].Rows[0]["WEF"].ToString();
                            model.SOCloseDate = oDataSet.Tables[0].Rows[0]["SOCloseDate"].ToString();
                            model.QuotNo = oDataSet.Tables[0].Rows[0]["QuotNo"].ToString();
                            model.QuotYear = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["QuotYear"]);
                            model.QDate = oDataSet.Tables[0].Rows[0]["QDate"].ToString();

                            if (Flag == "SOA")
                            {
                                model.AmmNo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SOAmmNo"].ToString());
                            }
                            else
                            {
                                model.AmmNo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AmmNo"].ToString());
                            }

                            model.AmmEffDate = oDataSet.Tables[0].Rows[0]["AmmEffDate"].ToString();
                            model.SOConfirmDate = oDataSet.Tables[0].Rows[0]["SOConfirmDate"].ToString();
                            model.ConsigneeAccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ConsigneeAccountCode"]);
                            model.ConsigneeAddress = oDataSet.Tables[0].Rows[0]["ConsigneeAddress"].ToString();
                            model.FreightPaidBy = oDataSet.Tables[0].Rows[0]["FreightPaidBy"].ToString();
                            model.PackingChgApplicable = oDataSet.Tables[0].Rows[0]["PackingChgApplicable"].ToString();
                            model.TransportMode = oDataSet.Tables[0].Rows[0]["ModeTransport"].ToString();
                            model.DeliverySch = oDataSet.Tables[0].Rows[0]["DeliverySch"].ToString();
                            model.DeliveryTerms = oDataSet.Tables[0].Rows[0]["DeliveryTerms"].ToString();
                            model.PreparedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PreparedBy"]);
                            model.PortOfDischarge = oDataSet.Tables[0].Rows[0]["PortOfDischarge"].ToString();
                            model.ResponsibleSalesPersonID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ResponsibleSalesPersonID"]);
                            model.CustContactPerson = oDataSet.Tables[0].Rows[0]["CustContactPerson"].ToString();
                            model.SaleDocType = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SaleDocType"]);
                            model.OtherDetail = oDataSet.Tables[0].Rows[0]["OtherDetail"].ToString();
                            model.Port2Loading = oDataSet.Tables[0].Rows[0]["PortToLoading"].ToString();
                            model.InsuApplicable = oDataSet.Tables[0].Rows[0]["InsuApplicable"].ToString();
                            model.SODeliveryDate = oDataSet.Tables[0].Rows[0]["SODeliveryDate"].ToString();
                            model.SORemark = oDataSet.Tables[0].Rows[0]["Remark"].ToString();
                            model.ItemNetAmount = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["OrderAmt"]);
                            model.NetTotal = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["OrderNetAmt"]);
                            model.CreatedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CreatedBy"]);
                            model.CreatedOn = Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["CreatedOn"]);
                            if (!string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["UpdatedBy"].ToString()))
                            {
                                model.UpdatedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["UpdatedBy"]);
                                model.UpdatedOn = Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["UpdatedOn"]);
                            }
                        }

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[1].Rows)
                            {
                                _ItemList.Add(new ItemDetail
                                {
                                    SeqNo = Convert.ToInt32(row["SeqNo"]),
                                    AltQty = Convert.ToDecimal(row["AltQty"]),
                                    AltUnit = row["AltUnit"].ToString(),
                                    
									AmendmentDate = Convert.ToDateTime(row["AmendmentDate"]).ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
									AmendmentNo = row["AmendmentNo"].ToString(),
                                    AmendmentReason = row["AmendmentReason"].ToString(),
                                    Amount = Convert.ToDecimal(row["Amount"]),
                                    Color = row["Color"].ToString(),
                                    Description = row["Description"].ToString(),
                                    DiscPer = Convert.ToDecimal(row["DiscPer"]),
                                    DiscRs = Convert.ToDecimal(row["DiscRs"]),
                                    Excessper = Convert.ToInt32(row["Excessper"]),
                                    HSNNo = Convert.ToInt32(row["HSNNo"]),
                                    ItemCode = Convert.ToInt32(row["ItemCode"]),
                                    ItemText = row["ItemText"].ToString(),
                                    OtherRateCurr = Convert.ToInt32(row["OtherRateCurr"]),
                                    PartCode = Convert.ToInt32(row["ItemCode"]),
                                    PartText = row["PartText"].ToString(),
                                    ProjQty1 = Convert.ToDecimal(row["ProjQty1"]),
                                    ProjQty2 = Convert.ToDecimal(row["ProjQty2"]),
                                    Qty = Convert.ToDecimal(row["Qty"]),
                                    Rate = Convert.ToDecimal(row["Rate"]),
                                    Rejper = Convert.ToDecimal(row["Rejper"]),
                                    Remark = row["Remark"].ToString(),
                                    StockQty = Convert.ToDecimal(row["StockQty"]),
                                    StoreName = row["StoreName"].ToString(),
                                    TolLimit = Convert.ToDecimal(row["TolLimit"]),
                                    Unit = row["Unit"].ToString(),
                                    UnitRate = row["UnitRate"].ToString(),
                                    DeliveryDate = Convert.ToDateTime(row["DeliveryDate"]).ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),

                                    DeliveryScheduleList = listObject.Where(x => x.DPartCode == Convert.ToInt32(row["ItemCode"])).ToList(),
                                });

                                //var _ItemDetail = new ItemDetail()
                                //{
                                //    SeqNo = Convert.ToInt32(row["SeqNo"]),
                                //    AltQty = Convert.ToInt32(row["AltQty"]),
                                //    AltUnit = row["AltUnit"].ToString(),
                                //    AmendmentDate = row["AmendmentDate"].ToString(),
                                //    AmendmentNo = row["AmendmentNo"].ToString(),
                                //    AmendmentReason = row["AmendmentReason"].ToString(),
                                //    Amount = Convert.ToDecimal(row["AltQty"]),
                                //    Color = row["Color"].ToString(),
                                //    Description = row["Description"].ToString(),
                                //    DiscPer = Convert.ToInt32(row["DiscPer"]),
                                //    DiscRs = Convert.ToDecimal(row["DiscRs"]),
                                //    Excessper = Convert.ToInt32(row["Excessper"]),
                                //    //ExRate = Convert.ToInt32(row["ExRate"]),
                                //    HSNNo = Convert.ToInt32(row["HSNNo"]),
                                //    ItemCode = Convert.ToInt32(row["ItemCode"]),
                                //    //ItemNetAmount = Convert.ToDecimal(row["ItemNetAmount"]),
                                //    //ItemText = row["ItemText"].ToString(),
                                //    OtherRateCurr = Convert.ToInt32(row["OtherRateCurr"]),
                                //    PartCode = Convert.ToInt32(row["PartCode"]),
                                //    //PartText = row["PartText"].ToString(),
                                //    ProjQty1 = Convert.ToInt32(row["ProjQty1"]),
                                //    ProjQty2 = Convert.ToInt32(row["ProjQty2"]),
                                //    Qty = Convert.ToInt32(row["Qty"]),
                                //    Rate = Convert.ToInt32(row["Rate"]),
                                //    Rejper = Convert.ToInt32(row["Rejper"]),
                                //    Remark = row["Remark"].ToString(),
                                //    StockQty = Convert.ToInt32(row["StockQty"]),
                                //    StoreName = row["StoreName"].ToString(),
                                //    TolLimit = Convert.ToInt32(row["TolLimit"]),
                                //    Unit = row["Unit"].ToString(),
                                //    UnitRate = row["UnitRate"].ToString(),
                                //};
                                //_List.Add(_ItemDetail);
                            }

                            model.ItemDetailGrid = _ItemList;
                        }

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[2].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[2].Rows)
                            {
                                _TaxList.Add(new TaxModel
                                {
                                    TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                                    TxType = row["Type"].ToString(),
                                    TxPartCode = Convert.ToInt32(row["ItemCode"]),
                                    TxPartName = row["PartName"].ToString(),
                                    TxItemCode = Convert.ToInt32(row["ItemCode"]),
                                    TxItemName = row["ItemName"].ToString(),
                                    TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                                    TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                                    TxAdInTxable = row["AddInTaxable"].ToString(),
                                    TxRoundOff = row["RoundOff"].ToString(),
                                    TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                                    TxTaxTypeName = row["TaxType"].ToString(),
                                    TxAccountCode = Convert.ToInt32(row["TaxAccountCode"]),
                                    TxAccountName = row["TaxName"].ToString(),
                                    TxAmount = Convert.ToDecimal(row["Amount"]),
                                    TxRefundable = row["TaxRefundable"].ToString(),
                                    TxRemark = row["Remarks"].ToString(),
                                });
                            }
                            model.TaxDetailGridd = _TaxList;
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

        internal async Task<SaleOrderModel> GetViewSOCcompletedByID(int ID, int YearCode, string Flag)
        {
            var oDataSet = new DataSet();
            var model = new SaleOrderModel();
            var _ItemList = new List<ItemDetail>();
            var _TaxList = new List<TaxModel>();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_SaleOrder", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", Flag);
                        oCmd.Parameters.AddWithValue("@EntryID", ID);
                        oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataSet);
                        oDataSet.Tables[0].TableName = "SOMain";
                        oDataSet.Tables[1].TableName = "ItemDetailGrid";
                        oDataSet.Tables[2].TableName = "SOTaxDetail";

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.EntryID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["EntryID"]);
                            model.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["YearCode"]);
                            model.EntryDate = oDataSet.Tables[0].Rows[0]["EntryDate"].ToString();
                            model.EntryTime = oDataSet.Tables[0].Rows[0]["EntryTime"].ToString().Trim();
                            model.Branch = oDataSet.Tables[0].Rows[0]["CC"].ToString();
                            model.SOFor = oDataSet.Tables[0].Rows[0]["SOFor"].ToString();
                            model.SOType = oDataSet.Tables[0].Rows[0]["SOType"].ToString();
                            model.CurrencyID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CurrencyID"]);
                            model.AccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AccountCode"]);
                            model.Address = oDataSet.Tables[0].Rows[0]["Address"].ToString();
                            model.DeliveryAddress = oDataSet.Tables[0].Rows[0]["DeliveryAddress"].ToString();
                            model.OrderType = oDataSet.Tables[0].Rows[0]["OrderType"].ToString();
                            model.CustOrderNo = oDataSet.Tables[0].Rows[0]["CustOrderNo"].ToString();
                            model.SONo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SONo"]);
                            model.SODate = oDataSet.Tables[0].Rows[0]["SODate"].ToString();
                            model.WEF = oDataSet.Tables[0].Rows[0]["WEF"].ToString();
                            model.SOCloseDate = oDataSet.Tables[0].Rows[0]["SOCloseDate"].ToString();
                            model.QuotNo = oDataSet.Tables[0].Rows[0]["QuotNo"].ToString();
                            model.QuotYear = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["QuotYear"]);
                            model.QDate = oDataSet.Tables[0].Rows[0]["QDate"].ToString();
                            model.AmmNo = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AmmNo"].ToString());
                            model.AmmEffDate = oDataSet.Tables[0].Rows[0]["AmmEffDate"].ToString();
                            model.SOConfirmDate = oDataSet.Tables[0].Rows[0]["SOConfirmDate"].ToString();
                            model.ConsigneeAccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ConsigneeAccountCode"]);
                            model.ConsigneeAddress = oDataSet.Tables[0].Rows[0]["ConsigneeAddress"].ToString();
                            model.FreightPaidBy = oDataSet.Tables[0].Rows[0]["FreightPaidBy"].ToString();
                            model.PackingChgApplicable = oDataSet.Tables[0].Rows[0]["PackingChgApplicable"].ToString();
                            model.TransportMode = oDataSet.Tables[0].Rows[0]["ModeTransport"].ToString();
                            model.DeliverySch = oDataSet.Tables[0].Rows[0]["DeliverySch"].ToString();
                            model.DeliveryTerms = oDataSet.Tables[0].Rows[0]["DeliveryTerms"].ToString();
                            model.PreparedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PreparedBy"]);
                            model.PortOfDischarge = oDataSet.Tables[0].Rows[0]["PortOfDischarge"].ToString();
                            model.ResponsibleSalesPersonID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ResponsibleSalesPersonID"]);
                            model.CustContactPerson = oDataSet.Tables[0].Rows[0]["CustContactPerson"].ToString();
                            model.SaleDocType = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SaleDocType"]);
                            model.OtherDetail = oDataSet.Tables[0].Rows[0]["OtherDetail"].ToString();
                            model.Port2Loading = oDataSet.Tables[0].Rows[0]["PortToLoading"].ToString();
                            model.InsuApplicable = oDataSet.Tables[0].Rows[0]["InsuApplicable"].ToString();
                            model.SODeliveryDate = oDataSet.Tables[0].Rows[0]["SODeliveryDate"].ToString();
                            model.SORemark = oDataSet.Tables[0].Rows[0]["Remark"].ToString();
                            model.ItemNetAmount = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["OrderAmt"]);
                            model.NetTotal = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["OrderNetAmt"]);
                        }

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[1].Rows)
                            {
                                _ItemList.Add(new ItemDetail
                                {
                                    SeqNo = Convert.ToInt32(row["SeqNo"]),
                                    AltQty = Convert.ToInt32(row["AltQty"]),
                                    AltUnit = row["AltUnit"].ToString(),
                                    AmendmentDate = row["AmendmentDate"].ToString(),
                                    AmendmentNo = row["AmendmentNo"].ToString(),
                                    AmendmentReason = row["AmendmentReason"].ToString(),
                                    Amount = Convert.ToDecimal(row["Amount"]),
                                    Color = row["Color"].ToString(),
                                    Description = row["Description"].ToString(),
                                    DiscPer = Convert.ToInt32(row["DiscPer"]),
                                    DiscRs = Convert.ToDecimal(row["DiscRs"]),
                                    Excessper = Convert.ToInt32(row["Excessper"]),
                                    HSNNo = Convert.ToInt32(row["HSNNo"]),
                                    ItemCode = Convert.ToInt32(row["ItemCode"]),
                                    ItemText = row["ItemText"].ToString(),
                                    OtherRateCurr = Convert.ToInt32(row["OtherRateCurr"]),
                                    PartCode = Convert.ToInt32(row["ItemCode"]),
                                    PartText = row["PartText"].ToString(),
                                    ProjQty1 = Convert.ToInt32(row["ProjQty1"]),
                                    ProjQty2 = Convert.ToInt32(row["ProjQty2"]),
                                    Qty = Convert.ToInt32(row["Qty"]),
                                    Rate = Convert.ToInt32(row["Rate"]),
                                    Rejper = Convert.ToInt32(row["Rejper"]),
                                    Remark = row["Remark"].ToString(),
                                    StockQty = Convert.ToInt32(row["StockQty"]),
                                    StoreName = row["StoreName"].ToString(),
                                    TolLimit = Convert.ToInt32(row["TolLimit"]),
                                    Unit = row["Unit"].ToString(),
                                    UnitRate = row["UnitRate"].ToString(),
                                });
                            }
                            model.ItemDetailGrid = _ItemList;
                        }

                        if (oDataSet.Tables.Count != 0 && oDataSet.Tables[2].Rows.Count > 0)
                        {
                            foreach (DataRow row in oDataSet.Tables[2].Rows)
                            {
                                _TaxList.Add(new TaxModel
                                {
                                    TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                                    TxType = row["Type"].ToString(),
                                    TxPartCode = Convert.ToInt32(row["Item_Code"]),
                                    TxPartName = row["PartName"].ToString(),
                                    TxItemCode = Convert.ToInt32(row["Item_Code"]),
                                    TxItemName = row["ItemName"].ToString(),
                                    TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                                    TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                                    TxAdInTxable = row["AddInTaxable"].ToString(),
                                    TxRoundOff = row["RoundOff"].ToString(),
                                    TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                                    TxTaxTypeName = row["TaxType"].ToString(),
                                    TxAccountCode = Convert.ToInt32(row["TaxAccount_Code"]),
                                    TxAccountName = row["TaxName"].ToString(),
                                    TxAmount = Convert.ToDecimal(row["Amount"]),
                                    TxRefundable = row["TaxRefundable"].ToString(),
                                    TxRemark = row["Remarks"].ToString(),
                                    
                                });
                            }
                            model.TaxDetailGridd = _TaxList;
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
