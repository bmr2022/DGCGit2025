using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.BLL
{
    public  class CustSaleNewRateBLL : ICustSaleNewRate
    {
        private readonly CustSaleNewRateDAL _newRateDAL;
        private readonly IDataLogic _dataLogic;

        public CustSaleNewRateBLL(IDataLogic dataLogic)
        {
            _dataLogic = dataLogic;
            _newRateDAL = new CustSaleNewRateDAL(_dataLogic);
        }

        public async Task<DataSet> GetCustomerListAsync()
        {
            return await _newRateDAL.GetCustomerListAsync();
        }

        public async Task<DataSet> GetPartCodeListAsync(bool showAll)
        {
            return await _newRateDAL.GetPartCodeListAsync(showAll);
        }

        public async Task<DataSet> GetPreviousRateDetailsAsync(long accountCode, long itemCode)
        {
            return await _newRateDAL.GetPreviousRateDetailsAsync(accountCode, itemCode);
        }

        public async Task<DataSet> GetNewEntryIdAsync(long yearCode, DateTime entryDate)
        {
            return await _newRateDAL.GetNewEntryIdAsync(yearCode, entryDate);
        }

        public async Task<DataSet> InsertOrUpdateRateAsync(DataTable rateData, long yearCode, long entryId, string slipNo, DateTime entryDate)
        {
            return await _newRateDAL.InsertOrUpdateRateAsync(rateData, yearCode, entryId, slipNo, entryDate);
        }

    }
}
