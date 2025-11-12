using eTactWeb.DOM.Models;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;


namespace eTactWeb.Services.Interface
{
    public interface IDeleteTransaction
    {
        Task<ResponseResult> GetFormName(string Flag);
        Task<ResponseResult> GetSlipNoData(string Flag, string MainTableName);
        Task<ResponseResult> InsertAndDeleteTransaction(DeleteTransactionModel model);
    }
}
