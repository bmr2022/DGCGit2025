using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIndent
    {
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> GetSizeModelColour(int ItemCode);
        Task<ResponseResult> GetBomRevNo(int ItemCode);
        Task<ResponseResult> GetBomChildDetail(int ItemCode, int BomRevNo, int BomQty);
        Task<ResponseResult> GetItemCode();
        Task<ResponseResult> FillFGPartCode();
        Task<ResponseResult> FillFGItemName();
        Task<ResponseResult> FillStoreList();
        Task<ResponseResult> FillVendorList();
        Task<ResponseResult> FillDeptList();
        Task<ResponseResult> GetServiceItems();
        Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid);
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty);
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> SaveIndentDetail(IndentModel model, DataTable IndentGrid);
        Task<ResponseResult> GetDashboardData(IndentDashboard model);
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<IndentModel> GetViewByID(int ID, string Mode, int YC);
        Task<ResponseResult> GetBomQty(int ItemCode, int BomRevNo);
        Task<ResponseResult> GetReportName();
    }
}