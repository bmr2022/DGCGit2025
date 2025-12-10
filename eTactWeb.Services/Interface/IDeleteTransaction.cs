using eTactWeb.DOM.Models;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;


namespace eTactWeb.Services.Interface
{
    public interface IDeleteTransaction
    {
        Task<ResponseResult> GetFormName(string Flag,string ModuleName);
        Task<ResponseResult> GetModuleName(string Flag);
        Task<ResponseResult> GetBomNo(string BomName);
        Task<ResponseResult> GetSlipNoData(string Flag, string MainTableName);
        Task<ResponseResult> InsertAndDeleteTransaction(DeleteTransactionModel model);
        Task<ResponseResult> UpdateExistingSlipNo(DeleteTransactionModel model);

    }
}
