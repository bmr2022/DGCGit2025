using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface ICustSaleNewRate
    {
        Task<DataSet> GetCustomerListAsync();
        Task<DataSet> GetPartCodeListAsync(bool showAll);
        Task<DataSet> GetPreviousRateDetailsAsync(long accountCode, long itemCode);
        Task<DataSet> GetNewEntryIdAsync(long yearCode, DateTime entryDate);
        Task<DataSet> InsertOrUpdateRateAsync(DataTable rateData, long yearCode, long entryId, string slipNo, DateTime entryDate);
    }
}
