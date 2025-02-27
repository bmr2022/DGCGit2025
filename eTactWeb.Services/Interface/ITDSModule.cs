using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITDSModule
    {
        Task<ResponseResult> GetHSNTaxInfo(HSNTAX HSNTaxParam);

        string GetTDSPercentage(string Flag, string TxCode);

        DataTable SgstCgst(int TxCode);
    }
}