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
    public class InProcessInspectionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public InProcessInspectionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
				SqlParams.Add(new SqlParameter("@InspYearCode", YearCode));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillPartCode(string InspectionType)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
				SqlParams.Add(new SqlParameter("@InprocessOutIncoming", InspectionType));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillItemName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillMachineName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillMachineName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillShift()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillShift"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCustomer()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCustomer"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillColor(string PartNo)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillColor"));
				SqlParams.Add(new SqlParameter("@Itemcode", PartNo));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<InProcessInspectionModel> GetInprocessInspectionGridData(int ItemCode, int SampleSize)
		{
			DataSet? oDataSet = new DataSet();
			var model = new InProcessInspectionModel();
			try
			{
				using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
				{
					SqlCommand oCmd = new SqlCommand("SPInprocessInpection", myConnection)
					{
						CommandType = CommandType.StoredProcedure
					};
					oCmd.Parameters.AddWithValue("@Flag", "FillCharectristicDetail");
					oCmd.Parameters.AddWithValue("@Itemcode", ItemCode);

					await myConnection.OpenAsync();
					using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
					{
						oDataAdapter.Fill(oDataSet);
					}
				}
				if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
				{
					
						model.DTSSGrid = (from DataRow dr in oDataSet.Tables[0].Rows
														select new InProcessInspectionDetailModel
														{
															Characteristic = dr["Characteristic"] != DBNull.Value ? Convert.ToString(dr["Characteristic"]) : string.Empty,
															EvalutionMeasurmentTechnique = dr["EvalutionMeasurmentTechnique"] != DBNull.Value ? Convert.ToString(dr["EvalutionMeasurmentTechnique"]) : string.Empty,
															ControlMethod = dr["ControlMethod"] != DBNull.Value ? Convert.ToString(dr["ControlMethod"]) : string.Empty,
															SpecificationFrom = dr["SpecificationFrom"] != DBNull.Value ? Convert.ToString(dr["SpecificationFrom"]) : string.Empty,
															SpecificationTo = dr["SpecificationTo"] != DBNull.Value ? Convert.ToString(dr["SpecificationTo"]) : string.Empty,
															
															RejectionPlan = dr["RejectionPlan"] != DBNull.Value ? Convert.ToString(dr["RejectionPlan"]) : string.Empty,
															FrequencyofTesting = dr["FrequencyofTesting"] != DBNull.Value ? Convert.ToString(dr["FrequencyofTesting"]) : string.Empty,

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
		public async Task<ResponseResult> SaveInprocessInspection(InProcessInspectionModel model, DataTable GIGrid)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var entDate = CommonFunc.ParseFormattedDate(model.Entry_Date);
				var ActentDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
				var upDate = CommonFunc.ParseFormattedDate(model.LastUpdationDate);
				var mrnDate = CommonFunc.ParseFormattedDate(model.MRNDate);
				var prodDate = CommonFunc.ParseFormattedDate(model.ProdDate);
				var SqlParams = new List<dynamic>();

				if (model.Mode == "U" || model.Mode == "V")
				{

					SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
					SqlParams.Add(new SqlParameter("@LastUpdatedDate", upDate));
				}
				else
				{
					SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

				}
				
                SqlParams.Add(new SqlParameter("@InspEntryId", model.EntryId));//not
                SqlParams.Add(new SqlParameter("@InspYearCode", model.YearCode));//not
                SqlParams.Add(new SqlParameter("@Entrydate", entDate));//not
                SqlParams.Add(new SqlParameter("@Testingdate", model.TestingDate));//not
                SqlParams.Add(new SqlParameter("@IncomInprocessOutgoing", model.InspectionType ?? ""));//not
                SqlParams.Add(new SqlParameter("@Slipno", model.SlipNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@ProcessId", model.ProcessId));//not
                SqlParams.Add(new SqlParameter("@ShiftId", model.ShiftID));//not
                SqlParams.Add(new SqlParameter("@InspTimeFrom", model.InspTimeFrom));
                SqlParams.Add(new SqlParameter("@InspimeTo", model.InspTimeTo));
                SqlParams.Add(new SqlParameter("@Itemcode", model.ItemCode));//not
                SqlParams.Add(new SqlParameter("@SamlpeSize", model.SampleSize));//not
                SqlParams.Add(new SqlParameter("@ProjectNo", model.ProjectNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@ProjectDate", model.ProjectDate));
                SqlParams.Add(new SqlParameter("@ProjectYearCode", model.ProjectYearCode));//not
                SqlParams.Add(new SqlParameter("@Color", model.Color ?? ""));//not
                SqlParams.Add(new SqlParameter("@MachineId", model.MachineId));//not
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));//not
                SqlParams.Add(new SqlParameter("@NoOfCavity", model.NoOfCavity));//not
                SqlParams.Add(new SqlParameter("@MRNNo", model.MRNNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@MRNYearCode", model.MRNYearCode));//not
                SqlParams.Add(new SqlParameter("@MRNDate", mrnDate ?? ""));//not
                SqlParams.Add(new SqlParameter("@ProdSlipNo", model.ProdSlipNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@ProdYearCode", model.ProdYearCode));//not
                SqlParams.Add(new SqlParameter("@ProdDate", prodDate ??""));//not
                SqlParams.Add(new SqlParameter("@MRNQty", model.MRNQty));//not
                SqlParams.Add(new SqlParameter("@ProdQty", model.ProdQty));//not
                SqlParams.Add(new SqlParameter("@InspActqty", model.InspActqty));//not
                SqlParams.Add(new SqlParameter("@OkQty", model.OkQty));//not
                SqlParams.Add(new SqlParameter("@Rejqty", model.Rejqty));//not
                SqlParams.Add(new SqlParameter("@Lotno", model.LotNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@Weight", model.Weight));//not
                SqlParams.Add(new SqlParameter("@InpectionJudgement", model.InpectionJudgement ?? ""));//not
                SqlParams.Add(new SqlParameter("@InspectedBeginingOfProd", model.InspectedBeginingOfProd));//not
                SqlParams.Add(new SqlParameter("@InspectedAfterMoldCorrection", model.InspectedAfterMoldCorrection));//not
                SqlParams.Add(new SqlParameter("@InspectedAfterLotChange", model.InspectedAfterLotChange));//not
                SqlParams.Add(new SqlParameter("@InspectedAfterMachinIdel", model.InspectedAfterMachinIdel));//not
                SqlParams.Add(new SqlParameter("@InspectedEndOfProd", model.InspectedEndOfProd));//not
                SqlParams.Add(new SqlParameter("@InspectedOther1", model.InspectedOther1));//not
                SqlParams.Add(new SqlParameter("@InspectedOther2", model.InspectedOther2));//not
                SqlParams.Add(new SqlParameter("@InspectedOther3", model.InspectedOther3));//not
                SqlParams.Add(new SqlParameter("@wcid", model.WCID));//not
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? ""));//not
                SqlParams.Add(new SqlParameter("@TotalRows", model.TotalRows));//not
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));//not
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));//not
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEntryBy));//not
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachine ?? ""));//not
               
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));//not
                SqlParams.Add(new SqlParameter("@InspectedBy", model.InspectedBy));//not
                SqlParams.Add(new SqlParameter("@Attachment1", model.Attachment1 ?? ""));
                SqlParams.Add(new SqlParameter("@Attachment2", model.Attachment2 ?? ""));

                SqlParams.Add(new SqlParameter("@dt", GIGrid));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}
			return _ResponseResult;
		}
        internal async Task<ResponseResult> GetDashboardData(InProcessInspectionModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@ReportType", model.ReportType));
                SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPInprocessInpection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<InProcessInspectionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new InProcessInspectionModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPInprocessInpection", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@ReportType", "SUMMARY");
                    oCmd.Parameters.AddWithValue("@FromDate", FromDate);
                    oCmd.Parameters.AddWithValue("@ToDate", ToDate);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.DTSSGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                      select new InProcessInspectionDetailModel
                                      {
                                          InspEntryId = dr["InspEntryId"] != DBNull.Value ? Convert.ToInt32(dr["InspEntryId"]) : 0,
                                          InspYearCode = dr["InspYearCode"] != DBNull.Value ? Convert.ToInt32(dr["InspYearCode"]) : 0,
                                          Entry_Date = dr["Entrydate"] != DBNull.Value ? Convert.ToString(dr["Entrydate"]) : string.Empty,
                                          TestingDate = dr["TestingDate"] != DBNull.Value ? Convert.ToString(dr["TestingDate"]) : string.Empty,
                                          InspectionType = dr["IncomInprocessOutgoing"] != DBNull.Value ? Convert.ToString(dr["IncomInprocessOutgoing"]) : string.Empty,
                                          SlipNo = dr["SlipNo"] != DBNull.Value ? Convert.ToString(dr["SlipNo"]) : string.Empty,
                                          ShiftName = dr["ShiftName"] != DBNull.Value ? Convert.ToString(dr["ShiftName"]) : string.Empty,
                                          InspTimeFrom = dr["InspTimeFrom"] != DBNull.Value ? Convert.ToString(dr["InspTimeFrom"]) : string.Empty,
                                          InspTimeTo = dr["InspimeTo"] != DBNull.Value ? Convert.ToString(dr["InspimeTo"]) : string.Empty,
                                          ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                          SampleSize = dr["SamlpeSize"] != DBNull.Value ? Convert.ToInt32(dr["SamlpeSize"]) : 0,
                                          ShiftId = dr["ShiftId"] != DBNull.Value ? Convert.ToInt32(dr["ShiftId"]) : 0,
										  MachineId = dr["MachineId"] != DBNull.Value ? Convert.ToInt32(dr["MachineId"]) : 0,
                                          ProjectNo = dr["ProjectNo"] != DBNull.Value ? Convert.ToString(dr["ProjectNo"]) : string.Empty,
                                          ProjectDate = dr["ProjectDate"] != DBNull.Value ? Convert.ToString(dr["ProjectDate"]) : string.Empty,
                                          ProjectYearCode = dr["ProjectYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ProjectYearCode"]) : 0,
                                          Color = dr["Color"] != DBNull.Value ? Convert.ToString(dr["Color"]) : string.Empty,
                                          MachineName = dr["MachineName"] != DBNull.Value ? Convert.ToString(dr["MachineName"]) : string.Empty,
                                          AccountName = dr["AccountName"] != DBNull.Value ? Convert.ToString(dr["AccountName"]) : string.Empty,
                                          
                                          AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                          NoOfCavity = dr["NoOfCavity"] != DBNull.Value ? Convert.ToInt32(dr["NoOfCavity"]) : 0,
                                          MRNNo = dr["MRNNo"] != DBNull.Value ? Convert.ToString(dr["MRNNo"]) : string.Empty,
                                          MRNYearCode = dr["MRNYearCode"] != DBNull.Value ? Convert.ToInt32(dr["MRNYearCode"]) : 0,
                                          MRNDate = dr["MRNDate"] != DBNull.Value ? Convert.ToString(dr["MRNDate"]) : string.Empty,
                                          ProdSlipNo = dr["ProdSlipNo"] != DBNull.Value ? Convert.ToString(dr["ProdSlipNo"]) : string.Empty,
                                          ProdYearCode = dr["ProdYearCode"] != DBNull.Value ? Convert.ToInt32(dr["ProdYearCode"]) : 0,
                                          ProdDate = dr["ProdDate"] != DBNull.Value ? Convert.ToString(dr["ProdDate"]) : string.Empty,
                                          MRNQty = dr["MRNQty"] != DBNull.Value ? Convert.ToDecimal(dr["MRNQty"]) : 0,
                                          ProdQty = dr["ProdQty"] != DBNull.Value ? Convert.ToDecimal(dr["ProdQty"]) : 0,
                                          InspActqty = dr["InspActqty"] != DBNull.Value ? Convert.ToDecimal(dr["InspActqty"]) : 0,
                                          OkQty = dr["OkQty"] != DBNull.Value ? Convert.ToDecimal(dr["OkQty"]) : 0,
                                          Rejqty = dr["Rejqty"] != DBNull.Value ? Convert.ToDecimal(dr["Rejqty"]) : 0,
                                          LotNo = dr["LotNo"] != DBNull.Value ? Convert.ToString(dr["LotNo"]) : string.Empty,
                                          Weight = dr["Weight"] != DBNull.Value ? Convert.ToDecimal(dr["Weight"]) : 0,
                                          Remarks = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                          CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,
                                          ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToString(dr["ActualEntryDate"]) : string.Empty,
                                          ActualEntryByName = dr["ActualEnteredByName"] != DBNull.Value ? Convert.ToString(dr["ActualEnteredByName"]) : string.Empty,
                                          LastUpdatedByName = dr["LastUpdatedByName"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedByName"]) : string.Empty,
                                          LastUpdationDate = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedDate"]) : string.Empty,
                                          ApprovedByName = dr["ApprovedByName"] != DBNull.Value ? Convert.ToString(dr["ApprovedByName"]) : string.Empty,
                                          EntryByMachine = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                          PartCode= dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                          ItemCode= dr["Itemcode"] != DBNull.Value ? Convert.ToInt32(dr["Itemcode"]) : 0,
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
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var entrydate = CommonFunc.ParseFormattedDate(EntryDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@InspEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@InspYearCode", YearCode));
                
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entrydate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", EntryByempId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<InProcessInspectionModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            var model = new InProcessInspectionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@InspEntryId", ID));
                SqlParams.Add(new SqlParameter("@InspYearCode", YC));
                //SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                //SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPInprocessInpection", SqlParams);

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
        private static InProcessInspectionModel PrepareView(DataSet DS, ref InProcessInspectionModel? model)
        {
            try
            {
                var ItemList = new List<InProcessInspectionDetailModel>();
                var DetailList = new List<InProcessInspectionModel>();
                DS.Tables[0].TableName = "InProcessInspection";
                DS.Tables[1].TableName = "InProcessInspectionDetail";
                int cnt = 0;

                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["InspEntryId"].ToString());
                model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["InspYearCode"].ToString());
                model.EntryDate = DS.Tables[0].Rows[0]["Entrydate"].ToString();
                model.TestingDate = DS.Tables[0].Rows[0]["Testingdate"].ToString();
                model.InspectionType = DS.Tables[0].Rows[0]["IncomInprocessOutgoing"].ToString();
                model.SlipNo = DS.Tables[0].Rows[0]["Slipno"].ToString();
                model.ProcessId = Convert.ToInt32(DS.Tables[0].Rows[0]["ProcessId"].ToString());
                model.ShiftID = Convert.ToInt32(DS.Tables[0].Rows[0]["ShiftId"].ToString());
                model.Shift = DS.Tables[0].Rows[0]["ShiftName"].ToString();
                model.InspTimeFrom = DS.Tables[0].Rows[0]["InspTimeFrom"].ToString();
                model.InspTimeTo = DS.Tables[0].Rows[0]["InspimeTo"].ToString();
                model.ItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Itemcode"].ToString());
                model.PartCode = DS.Tables[0].Rows[0]["PartCode"].ToString();
                model.ItemName = DS.Tables[0].Rows[0]["Item_Name"].ToString();
                model.SampleSize = Convert.ToInt32(DS.Tables[0].Rows[0]["SamlpeSize"].ToString());
                model.ProjectNo = DS.Tables[0].Rows[0]["ProjectNo"].ToString();
                model.ProjectDate = DS.Tables[0].Rows[0]["ProjectDate"].ToString();
                model.ProjectYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ProjectYearCode"].ToString());
                model.Color = DS.Tables[0].Rows[0]["Color"].ToString();
                model.MachineId = Convert.ToInt32(DS.Tables[0].Rows[0]["MachineId"].ToString());
                model.MachineNo = DS.Tables[0].Rows[0]["MachineName"].ToString();
                model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
                model.CustomerName = DS.Tables[0].Rows[0]["Account_Name"].ToString();
                model.NoOfCavity = Convert.ToInt32(DS.Tables[0].Rows[0]["NoOfCavity"].ToString());
                model.MRNNo = DS.Tables[0].Rows[0]["MRNNo"].ToString();
                model.MRNYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNYearCode"].ToString());
                model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"].ToString();
                model.ProdSlipNo = DS.Tables[0].Rows[0]["ProdSlipNo"].ToString();
                model.ProdYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdYearCode"].ToString());
                model.ProdDate = DS.Tables[0].Rows[0]["ProdDate"].ToString();
                model.MRNQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["MRNQty"].ToString());
                model.ProdQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["ProdQty"].ToString());
                model.InspActqty = Convert.ToDecimal(DS.Tables[0].Rows[0]["InspActqty"].ToString());
                model.OkQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["OkQty"].ToString());
                model.Rejqty = Convert.ToDecimal(DS.Tables[0].Rows[0]["Rejqty"].ToString());
                model.LotNo = DS.Tables[0].Rows[0]["Lotno"].ToString();
                model.Weight = Convert.ToDecimal(DS.Tables[0].Rows[0]["Weight"].ToString());
                model.InpectionJudgement = DS.Tables[0].Rows[0]["InpectionJudgement"].ToString();
                model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
                model.TotalRows = Convert.ToInt32(DS.Tables[0].Rows[0]["TotalRows"].ToString());
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.ActualEntryDate =DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"].ToString());
                model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEmployee"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachineName"].ToString();
                model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
                model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmployee"].ToString();
                model.ApprovedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ApprovedBy"].ToString());
                model.InspectedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["InspectedBy"].ToString());
                model.Attachment1 = DS.Tables[0].Rows[0]["Attachment1"].ToString();
                model.Attachment2 = DS.Tables[0].Rows[0]["Attachment2"].ToString();
				model.InspectedBeginingOfProd = DS.Tables[0].Rows[0]["InspectedBeginingOfProd"].ToString().Equals("1");
				model.InspectedAfterMoldCorrection = DS.Tables[0].Rows[0]["InspectedAfterMoldCorrection"].ToString().Equals("1");
				model.InspectedAfterLotChange = DS.Tables[0].Rows[0]["InspectedAfterLotChange"].ToString().Equals("1");
				model.InspectedAfterMachinIdel = DS.Tables[0].Rows[0]["InspectedAfterMachinIdel"].ToString().Equals("1");
				model.InspectedEndOfProd = DS.Tables[0].Rows[0]["InspectedEndOfProd"].ToString().Equals("1");

				int sampleSize = Convert.ToInt32(DS.Tables[0].Rows[0]["SamlpeSize"].ToString());
				model.SampleSize = sampleSize;
				if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ItemList.Add(new InProcessInspectionDetailModel
                        {
                            InspEntryId = Convert.ToInt32(row["InspEntryId"].ToString()),
                            InspYearCode = Convert.ToInt32(row["InspYearCode"].ToString()),
                            SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                            Characteristic = row["Characteristic"].ToString(),
                            EvalutionMeasurmentTechnique = row["EvalutionMeasurmentTechnique"].ToString(),
                            SpecificationFrom = row["SpecificationFrom"].ToString(),
                            Operator = row["Operator"].ToString(),
                            SpecificationTo = row["SpecificationTo"].ToString(),
                            FrequencyofTesting = row["FrequencyofTesting"].ToString(),
                            InspectionBy = row["InspectionBy"].ToString(),
                            ControlMethod = row["ControlMethod"].ToString(),
                            RejectionPlan = row["RejectionPlan"].ToString(),
                            Remarks = row["Remarks"].ToString(),
                            InspValue1 = Convert.ToDecimal(row["InspValue1"].ToString()),
                            InspValue2 = Convert.ToDecimal(row["InspValue2"].ToString()),
                            InspValue3 = Convert.ToDecimal(row["InspValue3"].ToString()),
                            InspValue4 = Convert.ToDecimal(row["InspValue4"].ToString()),
                            InspValue5 = Convert.ToDecimal(row["InspValue5"].ToString()),
                            InspValue6 = Convert.ToDecimal(row["InspValue6"].ToString()),
                            InspValue7 = Convert.ToDecimal(row["InspValue7"].ToString()),
                            InspValue8 = Convert.ToDecimal(row["InspValue8"].ToString()),
                            InspValue10 = Convert.ToDecimal(row["InspValue10"].ToString()),
                            InspValue11 = Convert.ToDecimal(row["InspValue11"].ToString()),
                            InspValue12 = Convert.ToDecimal(row["InspValue12"].ToString()),
                            InspValue13 = Convert.ToDecimal(row["InspValue13"].ToString()),
                            InspValue14 = Convert.ToDecimal(row["InspValue14"].ToString()),
                            InspValue15 = Convert.ToDecimal(row["InspValue15"].ToString()),
                            InspValue16 = Convert.ToDecimal(row["InspValue16"].ToString()),
                            InspValue17 = Convert.ToDecimal(row["InspValue17"].ToString()),
                            InspValue18 = Convert.ToDecimal(row["InspValue18"].ToString()),
                            InspValue19 = Convert.ToDecimal(row["InspValue19"].ToString()),
                            InspValue20 = Convert.ToDecimal(row["InspValue20"].ToString()),
                            InspValue21 = Convert.ToDecimal(row["InspValue21"].ToString()),
                            InspValue22 = Convert.ToDecimal(row["InspValue22"].ToString()),
                            InspValue23 = Convert.ToDecimal(row["InspValue23"].ToString()),
                            InspValue24 = Convert.ToDecimal(row["InspValue24"].ToString()),
                            InspValue25 = Convert.ToDecimal(row["InspValue25"].ToString()),

							  SampleSize = sampleSize

						});
                    }
                    model.DTSSGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
