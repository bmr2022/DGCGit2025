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

        public async Task<ResponseResult> BomQty(int RMItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLBOMQTY"));
                SqlParams.Add(new SqlParameter("@RMItemCode", RMItemCode));


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


    }
}
