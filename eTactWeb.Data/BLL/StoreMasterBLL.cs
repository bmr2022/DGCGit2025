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
    public class StoreMasterBLL:IStoreMaster
    {
        private StoreMasterDAL _StoreMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public StoreMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _StoreMasterDAL = new StoreMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillStoreType()
        {
            return await _StoreMasterDAL.FillStoreType();
        } 
        public async Task<ResponseResult> FillStoreID()
        {
            return await _StoreMasterDAL.FillStoreID();
        }
        public async Task<ResponseResult> ChkForDuplicate(string StoreName)
        {
            return await _StoreMasterDAL.ChkForDuplicate(StoreName);
        }
        public async Task<ResponseResult> ChkForDuplicateStoreType(string StoreType)
        {
            return await _StoreMasterDAL.ChkForDuplicateStoreType(StoreType);
        }
        public async Task<ResponseResult> SaveStoreMaster(StoreMasterModel model)
        {
            return await _StoreMasterDAL.SaveStoreMaster( model);
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _StoreMasterDAL.GetDashBoardData();
        }
        public async Task<StoreMasterModel> GetDashBoardDetailData()
        {
            return await _StoreMasterDAL.GetDashBoardDetailData();
        }
        public async Task<StoreMasterModel> GetViewByID(int ID)
        {
            return await _StoreMasterDAL.GetViewByID(ID);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _StoreMasterDAL.DeleteByID(ID);
        }
    }
}