using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TDSModuleBLL : ITDSModule
    {
        private TDSModuleDAL _TDSModuleDAL;

        public TDSModuleBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _TDSModuleDAL = new TDSModuleDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> GetHSNTaxInfo(HSNTAX HSNTDSParam)
        {
            return await _TDSModuleDAL.GetHSNTaxInfo(HSNTDSParam);
        }

        public string GetTDSPercentage(string Flag, string TxCode)
        {
            return _TDSModuleDAL.GetTDSPercentage(Flag, TxCode);
        }

        public DataTable SgstCgst(int TxCode)
        {
            return _TDSModuleDAL.SgstCgst(TxCode);
        }
    }
}