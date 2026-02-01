using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TaxMasterBLL : ITaxMaster
    {
        public TaxMasterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _TaxMasterDAL = new TaxMasterDAL(configuration, iDataLogic, connectionStringService);
        }

        private TaxMasterDAL? _TaxMasterDAL { get; }

        public async Task<DataSet> BindAllDropDown(string TaxName, decimal TaxPercent)
        {
            return await _TaxMasterDAL.BindAllDropDown( TaxName,  TaxPercent);
        }
        public async Task<ResponseResult> FillHSN(string taxCategory)
        {
            return await _TaxMasterDAL.FillHSN(taxCategory);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int CreatedBy)
        {
            return await _TaxMasterDAL.DeleteByID(ID, CreatedBy);
        }
        public async Task<ResponseResult> GetFormRights(int uId)
        {
            return await _TaxMasterDAL.GetFormRights(uId);
        }
        public async Task<ResponseResult> GetSGSTByTaxPercent(decimal taxPercent)
        {
            return await _TaxMasterDAL.GetSGSTByTaxPercent(taxPercent);
        }
        public async Task<IList<TaxMasterDashboard>> GetSearchData(string TaxName, string TaxType, string HSNno, int userID)
        {
            return await _TaxMasterDAL.GetSearchData(TaxName, TaxType, HSNno, userID);
        }

        public async Task<IList<TaxMasterDashboard>> GetDashBoardData(int userID)
        {
            return await _TaxMasterDAL.GetDashBoardData(userID);
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