using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TaxModuleBLL : ITaxModule
    {
        private TaxModuleDAL _TaxModuleDAL;

        public TaxModuleBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _TaxModuleDAL = new TaxModuleDAL(configuration, iDataLogic);
        }

        public async Task<ResponseResult> GetHSNTaxInfo(HSNTAX HSNTaxParam)
        {
            return await _TaxModuleDAL.GetHSNTaxInfo(HSNTaxParam);
        }

        public string GetTaxPercentage(string Flag, string TxCode)
        {
            return _TaxModuleDAL.GetTaxPercentage(Flag, TxCode);
        }

        public DataTable SgstCgst(int TxCode)
        {
            return _TaxModuleDAL.SgstCgst(TxCode);
        }
    }
}