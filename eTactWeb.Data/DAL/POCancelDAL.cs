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
    public class POCancelDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public POCancelDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string CancelationType, string PONO, string VendorName, int Eid, string uid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                string flag = "";
                if (CancelationType == "LISTOFUNCANCELEDPO")
                    flag = "LISTOFUNCANCELEDPO";
                if (CancelationType == "LISTOFCANCELEDPO")
                    flag = "LISTOFCANCELEDPO";
                if (CancelationType == "LISTOFACTIVEPO")
                    flag = "LISTOFACTIVEPO";
                if (CancelationType == "LISTOFDEACTIVEPO")
                    flag = "LISTOFDEACTIVEPO";
               
                SqlParams.Add(new SqlParameter("@Flag", flag));
                SqlParams.Add(new SqlParameter("@StartDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EndDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EmpId", Eid));
                SqlParams.Add(new SqlParameter("@UId", uid));
                //SqlParams.Add(new SqlParameter("@ApprovalType", ApprovalType));
                SqlParams.Add(new SqlParameter("@VendorName", VendorName));
                SqlParams.Add(new SqlParameter("@PONo", PONO));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<List<POCancleDetail>> ShowPODetail(int ID, int YC, string PONo, string TypeOfCancle)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<POCancleDetail>();


            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "SHOWPODETAIL"));
                SqlParams.Add(new SqlParameter("@POEntryId", ID));
                SqlParams.Add(new SqlParameter("@Poyearcode", YC));
                SqlParams.Add(new SqlParameter("@PoNo", PONo));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "POCancelMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var poCancelDetail = CommonFunc.DataRowToClass<POCancleDetail>(row);

                            MainModel.Add(poCancelDetail);
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
            return MainModel;
        }

        public async Task<ResponseResult> SaveCancelation(int EntryId, int YC, string PONO, string type, int EmpID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (type == "LISTOFUNCANCELEDPO")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "CANCELPO"));

                }
                if (type == "LISTOFCANCELEDPO")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UNCANCELPO"));

                }
                if (type == "LISTOFDEACTIVEPO")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ACTIVATEPO"));

                }
                if (type == "LISTOFACTIVEPO")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "DEACTIVATEPO"));

                }
                
                SqlParams.Add(new SqlParameter("@POEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@Poyearcode", YC));
                SqlParams.Add(new SqlParameter("@PoNo", PONO));
                SqlParams.Add(new SqlParameter("@ApprovalDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@EmpId", EmpID));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
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
