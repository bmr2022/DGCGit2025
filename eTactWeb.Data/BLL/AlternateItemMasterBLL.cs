using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class AlternateItemMasterBLL:IAlternateItemMaster
    {
        private AlternateItemMasterDAL _AlternateItemMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AlternateItemMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _AlternateItemMasterDAL = new AlternateItemMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetMainItem()
        {
            return await _AlternateItemMasterDAL.GetMainItem();
        } 
        public async Task<ResponseResult> GetMainPartCode()
        {
            return await _AlternateItemMasterDAL.GetMainPartCode();
        }
        public async Task<ResponseResult> GetAltPartCode(int MainItemcode)
        {
            return await _AlternateItemMasterDAL.GetAltPartCode(MainItemcode);
        } 
        public async Task<ResponseResult> GetAltItemName(int MainItemcode)
        {
            return await _AlternateItemMasterDAL.GetAltItemName(MainItemcode);
        }
        public async Task<ResponseResult> SaveAlternetItemMaster(AlternateItemMasterModel model, DataTable GIGrid)
        {
            return await _AlternateItemMasterDAL.SaveAlternateItemMaster(model,  GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _AlternateItemMasterDAL.GetDashboardData();
        }
        public async Task<AlternateItemMasterModel> GetViewByID(int MainItemcode, int AlternateItemCode)
        {
            return await _AlternateItemMasterDAL.GetViewByID( MainItemcode,  AlternateItemCode);
        }
        public async Task<AlternateItemMasterDashBoardModel> GetDashboardDetailData()
        {
            return await _AlternateItemMasterDAL.GetDashboardDetailData();
        }
        public async Task<ResponseResult> DeleteByID(int MainItemCode, int AlternateItemCode, string MachineName, int EntryByempId)
        {
            return await _AlternateItemMasterDAL.DeleteByID(MainItemCode, AlternateItemCode, MachineName, EntryByempId);
        }
    }
}
