using eTactWeb.Data.Common;
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
    public class IndentBLL : IIndent
    {
        public IndentBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IndentDAL = new IndentDAL(configuration, iDataLogic, connectionStringService);
        }

        private IndentDAL? _IndentDAL { get; }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _IndentDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> GetSizeModelColour(int ItemCode)
        {
            return await _IndentDAL.GetSizeModelColour(ItemCode);
        }
        public async Task<ResponseResult> GetBomRevNo(int ItemCode)
        {
            return await _IndentDAL.GetBomRevNo(ItemCode);
        }
        public async Task<ResponseResult> GetBomQty(int ItemCode,int BomRevNo)
        {
            return await _IndentDAL.GetBomQty(ItemCode,BomRevNo);
        }
        public async Task<ResponseResult> GetBomChildDetail(int ItemCode, int BomRevNo, int BomQty)
        {
            return await _IndentDAL.GetBomChildDetail(ItemCode, BomRevNo, BomQty);
        }

        public async Task<ResponseResult> GetItemCode()
        {
            return await _IndentDAL.GetItemCode();
        }
        public async Task<ResponseResult> FillFGPartCode()
        {
            return await _IndentDAL.FillFGPartCode();
        }
        public async Task<ResponseResult> FillFGItemName()
        {
            return await _IndentDAL.FillFGItemName();
        }
        public async Task<ResponseResult> FillStoreList()
        {
            return await _IndentDAL.FillStoreList();
        }
        public async Task<ResponseResult> FillVendorList()
        {
            return await _IndentDAL.FillVendorList();
        } 
        public async Task<ResponseResult> FillDeptList()
        {
            return await _IndentDAL.FillDeptList();
        }
        public async Task<ResponseResult> GetServiceItems()
        {
            return await _IndentDAL.GetServiceItems();
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid)
        {
            return await _IndentDAL.FillTotalStock(ItemCode, Storeid);
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _IndentDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
        {
            return await _IndentDAL.GetAltUnitQty(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> SaveIndentDetail(IndentModel model, DataTable IndentGrid)
        {
            return await _IndentDAL.SaveIndentDetail(model, IndentGrid);
        }
        public async Task<IndentModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _IndentDAL.GetViewByID(ID, Mode, YC);
        }
        public async Task<ResponseResult> GetDashboardData(IndentDashboard model)
        {
            return await _IndentDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _IndentDAL.DeleteByID(ID, YC);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _IndentDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _IndentDAL.GetReportName();
        }
    }
}
