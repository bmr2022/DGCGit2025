using eTactWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.DAL
{
    public class CustSaleNewRateDAL
    {
        private readonly IDataLogic _dataLogic;

        public CustSaleNewRateDAL(IDataLogic dataLogic)
        {
            _dataLogic = dataLogic;
        }


        public async Task<DataSet> GetPartCodeListAsync(bool showAll)
        {
            var sqlParams = new List<dynamic>
               {
                   new SqlParameter("@flag", "FillPartCode"),
                   new SqlParameter("@ShowAllItem", showAll ? "Y" : "N"),
               };
             
            var result = await _dataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }

        public async Task<DataSet> GetPreviousRateDetailsAsync(long accountCode, long itemCode)
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@flag", "FillPreviousRateDetail"),
                new SqlParameter("@accountCode", accountCode),
                new SqlParameter("@itemCode", itemCode),
            };

            var result = await _dataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
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

            var result = await _dataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
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

            var result = await _dataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }
        public async Task<DataSet> GetCustomerListAsync()
        {
            var sqlParams = new List<dynamic>
             {
                 new SqlParameter("@flag", "FillCustomer"),
                 new SqlParameter("@ShowAllCust", "Y") // or "Y" if needed
             };

            var result = await _dataLogic.ExecuteDataSet("SPCustSaleNewRateMaster", sqlParams);
            return result?.Result ?? new DataSet();
        }

    }
}
