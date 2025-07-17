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

				var SqlParams = new List<dynamic>();

				if (model.Mode == "U" || model.Mode == "V")
				{

					SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));

				}
				else
				{
					SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

				}
				var entDate = CommonFunc.ParseFormattedDate(model.Entry_Date);
				var ActentDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
				var upDate = CommonFunc.ParseFormattedDate(model.LastUpdationDate);
                SqlParams.Add(new SqlParameter("@InspEntryId", model.EntryId));//not
                SqlParams.Add(new SqlParameter("@InspYearCode", model.YearCode));//not
                SqlParams.Add(new SqlParameter("@Entrydate", model.Entry_Date));//not
                SqlParams.Add(new SqlParameter("@Testingdate", model.TestingDate));//not
                SqlParams.Add(new SqlParameter("@IncomInprocessOutgoing", model.ForInOutInprocess ?? ""));//not
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
                SqlParams.Add(new SqlParameter("@MRNDate", model.MRNDate??""));//not
                SqlParams.Add(new SqlParameter("@ProdSlipNo", model.ProdSlipNo ?? ""));//not
                SqlParams.Add(new SqlParameter("@ProdYearCode", model.ProdYearCode));//not
                SqlParams.Add(new SqlParameter("@ProdDate", model.ProdDate ??""));//not
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
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedDate", model.LastUpdationDate));
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
	}
}
