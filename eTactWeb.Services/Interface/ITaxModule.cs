using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITaxModule
    {
        Task<ResponseResult> GetHSNTaxInfo(HSNTAX HSNTaxParam);

        string GetTaxPercentage(string Flag, string TxCode);

        DataTable SgstCgst(int TxCode);
    }
}