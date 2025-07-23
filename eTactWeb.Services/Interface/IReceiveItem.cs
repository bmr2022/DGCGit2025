using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IReceiveItem
    {
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName);
        Task<ResponseResult> BindDepartmentList(string FromDate,string ToDate);
        Task<ResponseResult> GetDashboardData();
        Task<ReceiveItemModel> GetViewByID(int ID, int YearCode);
        Task<ReceiveItemDashboard> GetDashboardData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType);
        Task<ReceiveItemDashboard> GetDashboardDetailData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType);
        Task<ResponseResult> SaveInprocessQc(ReceiveItemModel model, DataTable ReceiveItemDetail);
        Task<ResponseResult> DeleteByID(int ID, int YC, string RecMatSlipNo, string EntryByMachineName, int ActualEntryBy);
    }
}
