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
    public class ReOfferItemDAL
    {
        private readonly ConnectionStringService _connectionStringService;
        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }
        public ReOfferItemDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GETNEWENTRY(int ReofferYearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GETNEWENTRY"));
                SqlParams.Add(new SqlParameter("@ReofferYearcode", ReofferYearcode));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FILLQCTYPE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLQCTYPE"));
                
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FILLMIRNO()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMIRNO"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FILLMIRYearCode(string MIRNO)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMIRYearCode"));
                SqlParams.Add(new SqlParameter("@MIRNO", MIRNO));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FILLMIRData(string MIRNO, int MIRYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FILLMIRData"));
                SqlParams.Add(new SqlParameter("@MIRNO", MIRNO));
                SqlParams.Add(new SqlParameter("@MIRYearCode", MIRYearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> BINDSTORE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "BINDSTORE"));
                

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetItemDeatil(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetItemDeatil"));
                SqlParams.Add(new SqlParameter("@MIRNO", MIRNO));
                SqlParams.Add(new SqlParameter("@MIRYearCode", MIRYearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@ReofferMir", ReofferMir));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetItemQty(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir, int itemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetItemQty"));
                SqlParams.Add(new SqlParameter("@MIRNO", MIRNO));
                SqlParams.Add(new SqlParameter("@MIRYearCode", MIRYearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@ReofferMir", ReofferMir));
                SqlParams.Add(new SqlParameter("@itemcode", itemcode));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillOkRecStore(int itemcode, string ShowAllStore)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillOkRecStore"));
                SqlParams.Add(new SqlParameter("@itemcode", itemcode));
                SqlParams.Add(new SqlParameter("@ShowAllStore", ShowAllStore));
               


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> ALLOWSHOWALLSTORE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ALLOWSHOWALLSTORE"));
                


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> RejSTORE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "RejSTORE"));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> RewSTORE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "RewSTORE"));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> HoldSTORE()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "HoldSTORE"));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> BINDEMP()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "BINDEMP"));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillPODetail(string MIRNO, int MIRYearCode, int accountcode, int itemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillPODetail"));
                SqlParams.Add(new SqlParameter("@MIRNO", MIRNO));
                SqlParams.Add(new SqlParameter("@MIRYearCode", MIRYearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountcode));
                SqlParams.Add(new SqlParameter("@itemcode", itemcode));




                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveReoffer(ReOfferItemModel model, DataTable ISTGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                //DateTime entDt = new DateTime();
                //DateTime SADt = new DateTime();

                var entDt = CommonFunc.ParseFormattedDate(model.ReofferEntrydate);
                var MRNDate = CommonFunc.ParseFormattedDate(model.MRNDate);
                var MIRDate = CommonFunc.ParseFormattedDate(model.MIRDate);
                var BillDate = CommonFunc.ParseFormattedDate(model.BillDate);

                var ActualEntryDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                //var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@UpdatedOn", model.UpdatedOn));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                SqlParams.Add(new SqlParameter("@ReofferEntryId", model.ReofferEntryId));
                SqlParams.Add(new SqlParameter("@ReofferYearcode", model.ReofferYearcode));
                SqlParams.Add(new SqlParameter("@ReofferNo", model.ReofferNo));
                SqlParams.Add(new SqlParameter("@ReofferEntrydate", entDt));
                SqlParams.Add(new SqlParameter("@QcType", model.QcType));
                SqlParams.Add(new SqlParameter("@MirEntryid", model.MirEntryid));
                SqlParams.Add(new SqlParameter("@MIRNo", model.MIRNo));
                SqlParams.Add(new SqlParameter("@MIRDate", MIRDate));
                SqlParams.Add(new SqlParameter("@MIRYearCode", model.MIRYearCode));
                SqlParams.Add(new SqlParameter("@MRNNO", model.MRNNO));
                SqlParams.Add(new SqlParameter("@MRNDate", MRNDate));
                SqlParams.Add(new SqlParameter("@MrnYearCode", model.MrnYearCode));
                SqlParams.Add(new SqlParameter("@MRNJOBWORK", model.MRNJOBWORK));
                SqlParams.Add(new SqlParameter("@BillNo", model.BillNo));
                SqlParams.Add(new SqlParameter("@BillDate", BillDate));
                SqlParams.Add(new SqlParameter("@HoldRejrewStatus", model.HoldRejrewStatus));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@QCStore", model.QCStore));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@UID", model.UID));
                SqlParams.Add(new SqlParameter("@EnteredByEmpId", model.EnteredByEmpId));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ActualEntryDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));
                





                SqlParams.Add(new SqlParameter("@DTItemGrid", ISTGrid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReofferMIRMainDetail", SqlParams);
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
