using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using eTactWeb.Services.Interface;
using eTactWeb.DOM.Models;

namespace eTactWeb.Data.DAL
{
    public class DeassembleItemDAL
    {
        private readonly ConnectionStringService _connectionStringService;
        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }
        public DeassembleItemDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> NewEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
              

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> BomQty(int RMItemCode, int FinishItemCode, int bomNo, float FGQty)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLBOMQTY"));
                SqlParams.Add(new SqlParameter("@RMItemCode", RMItemCode));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishItemCode));
                SqlParams.Add(new SqlParameter("@bomNo", bomNo));
                SqlParams.Add(new SqlParameter("@FGQty", FGQty));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillMRNNO(int FGItemCode, int yearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMRNNO"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FGItemCode));
                SqlParams.Add(new SqlParameter("@DeassYearcode", yearcode));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillMRNYearCode(int FGItemCode, int yearcode,string MRNNO)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMRNYEARCODE"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FGItemCode));
                SqlParams.Add(new SqlParameter("@DeassYearcode", yearcode));
                SqlParams.Add(new SqlParameter("@MRNO", MRNNO));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> FillMRNDetail(int yearcode, string MRNNO, int mrnyearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMRNDETAIL"));
                SqlParams.Add(new SqlParameter("@MRNO", MRNNO));
                SqlParams.Add(new SqlParameter("@DeassYearcode", yearcode));
                SqlParams.Add(new SqlParameter("@MRNYearCode", mrnyearcode));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLSTORENAME"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGITEMNAME"));
               
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGPARTCODE"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillBomNo(int FinishItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BOMNO"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillRMItemName(int FinishItemCode, int BomNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMITEMNAME"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishItemCode));
                SqlParams.Add(new SqlParameter("@bomno", BomNo));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }


        public async Task<ResponseResult> FillRMPartCode(int FinishItemCode, int BomNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMPARTCODE"));
                SqlParams.Add(new SqlParameter("@FinishItemCode", FinishItemCode));
                SqlParams.Add(new SqlParameter("@bomno", BomNo));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                var Date = DateTime.Now;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                SqlParams.Add(new SqlParameter("@transDate", Date));
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


        public async Task<ResponseResult> SaveDeassemble(DeassembleItemModel model, DataTable ISTGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                //DateTime entDt = new DateTime();
                //DateTime SADt = new DateTime();

                var entDt = CommonFunc.ParseFormattedDate(model.DeassEntryDate);
                var SADt = CommonFunc.ParseFormattedDate(model.MRNDate);

                var ActEntDt = CommonFunc.ParseFormattedDate(model.CreatedOn);
                //var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                    //SqlParams.Add(new SqlParameter("@LastUpdatetionDate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@DeassEntryID", model.DeassEntryID));
                SqlParams.Add(new SqlParameter("@DeassEntryDate", entDt));
                SqlParams.Add(new SqlParameter("@DeassYearcode", model.DeassYearcode));
                SqlParams.Add(new SqlParameter("@DeassSlipNo", model.DeassSlipNo));
                SqlParams.Add(new SqlParameter("@FGStoreId",model.FGStoreId));
                SqlParams.Add(new SqlParameter("@FinishItemCode", model.FinishItemCode));
                SqlParams.Add(new SqlParameter("@FGBatchNo", model.FGBatchNo));
                SqlParams.Add(new SqlParameter("@FGUniqueBatchNo", model.FGUniqueBatchNo));
                SqlParams.Add(new SqlParameter("@TotalStock", model.TotalStock));
                SqlParams.Add(new SqlParameter("@FGQty", model.FGQty));
                SqlParams.Add(new SqlParameter("@Unit", model.Unit ));
                SqlParams.Add(new SqlParameter("@FGConvQty", model.FGConvQty));
                SqlParams.Add(new SqlParameter("@CreatedByEmp", model.CreatedByEmp));
                SqlParams.Add(new SqlParameter("@CreatedOn", ActEntDt));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@MRNO", model.MRNO ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MRNYearCode", model.MRNYearCode ?? 0));
                SqlParams.Add(new SqlParameter("@MRNDate", SADt ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MRNEntryID", model.MRNEntryID ?? 0));
                SqlParams.Add(new SqlParameter("@ProdSlipNO", model.ProdSlipNO ?? 0));
                SqlParams.Add(new SqlParameter("@ProdDate", model.ProdDate ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ProdYearCode", model.ProdYearCode ?? 0));
                SqlParams.Add(new SqlParameter("@ProdEntryId", model.ProdEntryId ?? 0));
                SqlParams.Add(new SqlParameter("@MaterialRecFrom", model.MaterialRecFrom));
                SqlParams.Add(new SqlParameter("@BomNo", model.BomNo));




                SqlParams.Add(new SqlParameter("@DTItemGrid", ISTGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
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
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _IDataLogic.ExecuteDataSet("SP_DeassembleItemMainDetail", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }


        public async Task<DeassembleItemDashBoard> GetDashBoardDetailData(string FromDate, string ToDate, string ReportType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new DeassembleItemDashBoard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_DeassembleItemMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@Fromdate", CommonFunc.ParseFormattedDate(FromDate));
                    oCmd.Parameters.AddWithValue("@todate", CommonFunc.ParseFormattedDate(ToDate));
                    oCmd.Parameters.AddWithValue("@reporttype", ReportType);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "SUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.DeassembleItemDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                    select new DeassembleItemDashBoard
                                                    {

                                                        DeassEntryID = Convert.ToInt32(dr["DeassEntryID"]),
                                                        
                                                        DeassEntryDate =CommonFunc.ParseFormattedDate(dr["DeassEntryDate"].ToString()),
                                                        DeassYearcode = Convert.ToInt32(dr["DeassYearcode"]),
                                                        DeassSlipNo = dr["DeassSlipNo"].ToString(),
                                                        FGStoreId = Convert.ToInt32(dr["FGStoreId"]),
                                                        FGStoreName = dr["FGStoreName"].ToString(),

                                                        FinishItemCode = Convert.ToInt32(dr["FinishItemCode"]),
                                                        FinishItemName = dr["FinishItemName"].ToString(),
                                                        FinishPartCode = dr["FinishPartCode"].ToString(),
                                                        FGBatchNo = dr["FGBatchNo"].ToString(),
                                                        FGUniqueBatchNo = dr["FGUniqueBatchNo"].ToString(),

                                                        TotalStock = Convert.ToDecimal(dr["TotalStock"]),
                                                        FGQty = Convert.ToDecimal(dr["FGQty"]),
                                                        FGUnit = dr["FGUnit"].ToString(),
                                                        FGConvQty = Convert.ToDecimal(dr["FGConvQty"]),

                                                        CreatedByEmp = Convert.ToInt32(dr["CreatedByEmp"]),
                                                        CreatedByEmpName = dr["CreatedByEmpName"].ToString(),
                                                        CreatedOn =CommonFunc.ParseFormattedDate(dr["CreatedOn"].ToString()),

                                                        UpdatedBy = Convert.ToInt32(dr["UpdatedBy"]),
                                                        UpdatedByEmpName = dr["UpdatedByEmpName"].ToString(),
                                                        UpdatedOn =CommonFunc.ParseFormattedDate(dr["UpdatedOn"].ToString()),
                                                        EntryByMachine = dr["EntryByMachine"].ToString(),
                                                    }).ToList();
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

        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName)
        {
            var _ResponseResult = new ResponseResult();
            var etrDt = CommonFunc.ParseFormattedDate(EntryDate);
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@DeassEntryID", ID));
                SqlParams.Add(new SqlParameter("@DeassYearcode", YC));
                SqlParams.Add(new SqlParameter("@DeassEntryDate", etrDt));
                SqlParams.Add(new SqlParameter("@CreatedByEmp", ActualEntryBy));
                SqlParams.Add(new SqlParameter("@EntryByMachine", MachineName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DeassembleItemMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<DeassembleItemModel> GetViewByID(int ID, string Mode, int YC)
        {
            var model = new DeassembleItemModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@DeassEntryID", ID));
                SqlParams.Add(new SqlParameter("@DeassYearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DeassembleItemMainDetail", SqlParams);

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

        private static DeassembleItemModel PrepareView(DataSet DS, ref DeassembleItemModel? model, string Mode)
        {
            var ItemList = new List<DeassembleItemDetail>();
            DS.Tables[0].TableName = "VDeassembleItemMainDetail";

            int cnt = 1;

            model.DeassEntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["DeassEntryID"].ToString());
            model.DeassEntryDate = DS.Tables[0].Rows[0]["DeassEntryDate"].ToString();
            model.DeassYearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["DeassYearcode"].ToString());
            model.DeassSlipNo = DS.Tables[0].Rows[0]["DeassSlipNo"].ToString();
            model.MaterialRecFrom = DS.Tables[0].Rows[0]["MaterialRecFrom"].ToString();
            model.FGStoreId = Convert.ToInt32(DS.Tables[0].Rows[0]["FGStoreId"].ToString());
            model.FGStoreName = DS.Tables[0].Rows[0]["FGStoreName"].ToString();
            model.FinishItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["FinishItemCode"].ToString());
            model.FinishItemName = DS.Tables[0].Rows[0]["FinishItemName"].ToString();
            model.FinishPartCode = DS.Tables[0].Rows[0]["FinishPartCode"].ToString();
            model.BomNo = Convert.ToInt32(DS.Tables[0].Rows[0]["BomNo"].ToString());
            model.FGBatchNo = DS.Tables[0].Rows[0]["FGBatchNo"].ToString();
            model.FGUniqueBatchNo = DS.Tables[0].Rows[0]["FGUniqueBatchNo"].ToString();
            model.TotalStock = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalStock"].ToString());
            model.FGQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["FGQty"].ToString());
            model.Unit = DS.Tables[0].Rows[0]["FGUnit"].ToString();
            model.FGConvQty = Convert.ToDecimal(DS.Tables[0].Rows[0]["FGConvQty"].ToString());
            model.CreatedByEmp = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedByEmp"].ToString());
            model.CreatedByEmpName = DS.Tables[0].Rows[0]["CreatedByEmpName"].ToString();
            model.CreatedOn = DS.Tables[0].Rows[0]["CreatedOn"].ToString();
            model.MRNEntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNEntryID"].ToString());
            model.MRNYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNYearCode"].ToString());
            model.MRNO = DS.Tables[0].Rows[0]["MRNO"].ToString();
            model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"].ToString();
            model.ProdSlipNO = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdSlipNO"].ToString());
            model.ProdDate = DS.Tables[0].Rows[0]["ProdDate"].ToString();
            model.ProdYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdYearCode"].ToString());
            model.ProdEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["ProdEntryId"].ToString());
            model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
            
            model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();


            if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString()))
            {
                model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"].ToString());
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
                model.UpdatedOn = DS.Tables[0].Rows[0]["UpdatedOn"].ToString();
            }

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    ItemList.Add(new DeassembleItemDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"]),
                        RMItemCode = Convert.ToInt32(row["RMItemCode"]),
                        RmPartCode = row["RMPartCode"].ToString(),
                        RMItemName = row["RMItemName"].ToString(),
                        BomQty = Convert.ToDecimal(row["BomQty"]),
                        DeassQty = Convert.ToDecimal(row["DeassQty"]),
                        RMStoreId = Convert.ToInt32(row["RMStoreId"]),
                        RMStoreName = row["RMStoreName"].ToString(),


                        RMUnit = row["RMUnit"].ToString(),
                        
                        Remark = row["Remark"].ToString(),
                        
                        IdealDeassQty = Convert.ToDecimal(row["IdealDeassQty"]),
                        RMBatchNo = row["RMBatchNo"].ToString(),
                        RmUniqueBatchNo = row["RmUniqueBatchNo"].ToString(),
                    });
                   
                }
                ItemList = ItemList.OrderBy(item => item.SeqNo).ToList();
                model.DeassembleItemDetail = ItemList;
            }

            return model;
        }

    }
}
