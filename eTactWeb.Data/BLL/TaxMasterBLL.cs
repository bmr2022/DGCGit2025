using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TaxMasterBLL : ITaxMaster
    {
        public TaxMasterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _TaxMasterDAL = new TaxMasterDAL(configuration, iDataLogic);
        }

        private TaxMasterDAL? _TaxMasterDAL { get; }

        public async Task<DataSet> BindAllDropDown()
        {
            return await _TaxMasterDAL.BindAllDropDown();
        }

        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _TaxMasterDAL.DeleteByID(ID);
        } 
        public async Task<ResponseResult> GetFormRights(int uId)
        {
            return await _TaxMasterDAL.GetFormRights(uId);
        }
        public async Task<IList<TaxMasterDashboard>> GetSearchData(string TaxName, string TaxType,string HSNno)
        {
            return await _TaxMasterDAL.GetSearchData(TaxName,TaxType,HSNno);
        }

        public async Task<IList<TaxMasterDashboard>> GetDashBoardData()
        {
            return await _TaxMasterDAL.GetDashBoardData();
        }

        public async Task<ResponseResult> SaveTaxMaster(TaxMasterModel model, DataTable TaxMasterDT)
        {
            return await _TaxMasterDAL.SaveTaxMaster(model, TaxMasterDT);
        }

        public async Task<TaxMasterModel> ViewByID(int ID)
        {
            return await _TaxMasterDAL.ViewByID(ID);
        }
    }
}