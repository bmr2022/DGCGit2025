using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICustSaleNewRate
    {
        Task<DataSet> GetCustomerListAsync();
        public Task<ResponseResult> AutoFillitem(string Flag, string SearchItemCode, string SearchPartCode);

        Task<DataSet> GetPreviousRateDetailsAsync(long accountCode, long itemCode);
        Task<DataSet> GetNewEntryIdAsync(long yearCode, DateTime entryDate);
        Task<DataSet> InsertOrUpdateRateAsync(DataTable rateData, long yearCode, long entryId, string slipNo, DateTime entryDate);
    }
}
