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
    public class ReceiveItemBLL : IReceiveItem
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ReceiveItemDAL _ReceiveItemDAL;
        public ReceiveItemBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _ReceiveItemDAL = new ReceiveItemDAL(configuration, iDataLogic);
        }
        public async Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName)
        {
            return await _ReceiveItemDAL.FillEntryId(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> SaveInprocessQc(ReceiveItemModel model, DataTable ReceiveItemDetail)
        {
            return await _ReceiveItemDAL.SaveInprocessQc(model, ReceiveItemDetail);
        }
        public async Task<ResponseResult> BindDepartmentList(string FromDate,string ToDate)
        {
            return await _ReceiveItemDAL.BindDepartmentList(FromDate,ToDate);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _ReceiveItemDAL.GetDashboardData();
        }
        public async Task<ReceiveItemModel> GetViewByID(int ID, int YearCode)
        {
            return await _ReceiveItemDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ReceiveItemDashboard> GetDashboardData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            return await _ReceiveItemDAL.GetDashboardData(FromDate, ToDate, ItemName, PartCode, DashboardType);
        }
        public async Task<ReceiveItemDashboard> GetDashboardDetailData(string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            return await _ReceiveItemDAL.GetDashboardDetailData(FromDate, ToDate, ItemName, PartCode, DashboardType);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string RecMatSlipNo, string EntryByMachineName, int ActualEntryBy)
        {
            return await _ReceiveItemDAL.DeleteByID(ID, YC, RecMatSlipNo, EntryByMachineName, ActualEntryBy);
        }
    }
}
