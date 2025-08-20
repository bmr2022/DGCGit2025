using eTactWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class CustSaleNewRateDAL
    {
        private readonly IDataLogic _IDataLogic;

        public CustSaleNewRateDAL(IDataLogic dataLogic)
        {
            _IDataLogic = dataLogic;
        }


        public async Task<ResponseResult> AutoFillitem(string Flag,  string SearchItemCode, string SearchPartCode)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                
                SqlParams.Add(new SqlParameter("@SearchItemCode", SearchItemCode ?? ""));
                SqlParams.Add(new SqlParameter("@SearchPartCode", SearchPartCode ?? ""));


                Result = await _IDataLogic.ExecuteDataTable("SPCustSaleNewRateMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }


        public async Task<DataSet> GetPreviousRateDetailsAsync(long accountCode, long itemCode)
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@flag", "FillPreviousRateDetail"),
                new SqlParameter("@accountCode", accountCode),
                new SqlParameter("@itemCode", itemCode),
            };

            var result = await _IDataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }
        public async Task<DataSet> GetNewEntryIdAsync(long yearCode, DateTime entryDate)
        {
            var sqlParams = new List<dynamic>
              {
                  new SqlParameter("@flag", "NewEntryId"),
                  new SqlParameter("@CNRMYearCode", yearCode),
                  new SqlParameter("@CNRMEntryDate", entryDate),
              };

            var result = await _IDataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }
        public async Task<DataSet> InsertOrUpdateRateAsync(DataTable rateData, long yearCode, long entryId, string slipNo, DateTime entryDate)
        {
            var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@flag", "INSERT"), // or "UPDATE"
                    new SqlParameter("@CNRMYearCode", yearCode),
                    new SqlParameter("@CNRMEntryId", entryId),
                    new SqlParameter("@CNRMSlipNo", slipNo),
                    new SqlParameter("@CNRMEntryDate", entryDate),
                    new SqlParameter("@dt", SqlDbType.Structured)
                    {
                        TypeName = "Type_CustSaleNewRateMaster",
                        Value = rateData ?? new DataTable()
                    }
                };

            var result = await _IDataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }
        public async Task<DataSet> GetCustomerListAsync()
        {
            var sqlParams = new List<dynamic>
             {
                 new SqlParameter("@flag", "FillCustomer"),
                 new SqlParameter("@ShowAllCust", "Y") // or "Y" if needed
             };

            var result = await _IDataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }

    }
}
