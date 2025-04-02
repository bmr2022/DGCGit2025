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
    public class RoutingBLL : IRouting
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly RoutingDAL _RoutingDAL;
        public RoutingBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _RoutingDAL = new RoutingDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _RoutingDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> FillStoreName()
        {
            return await _RoutingDAL.FillStoreName();
        }
        public async Task<ResponseResult> FillItems(string Flag)
        {
            return await _RoutingDAL.FillItems(Flag);
        }
        public async Task<ResponseResult> AlreadyExistItems(string Flag)
        {
            return await _RoutingDAL.AlreadyExistItems(Flag);
        }
        public async Task<ResponseResult> FillSubItems(string Flag)
        {
            return await _RoutingDAL.FillSubItems(Flag);
        }
        public Task<RoutingModel> GetAllDataItemWise(string Flag, int ItemCode)
        {
            return _RoutingDAL.GetAllDataItemWise(Flag, ItemCode);
        }
        public async Task<ResponseResult> SaveRouting(RoutingModel model, DataTable JWGrid)
        {
            return await _RoutingDAL.SaveRouting(model, JWGrid);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate)
        {
            return await _RoutingDAL.GetDashboardData(FromDate, ToDate);

            //throw new NotImplementedException();
        }

        public async Task<RoutingGridDashBoard> GetDashboardData(string SummaryDetail, string PartCode, string ItemName, string Stage, string WorkCenter, string FromDate, string ToDate)
        {
            return await _RoutingDAL.GetDashboardData(SummaryDetail, PartCode, ItemName, Stage, WorkCenter, FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _RoutingDAL.DeleteByID(ID);
        }
        public async Task<ResponseResult> GetNewEntryId()
        {
            return await _RoutingDAL.GetNewEntryId();
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _RoutingDAL.GetFormRights(ID);
        }

        public async Task<RoutingModel> GetViewByID(int ID, string Mode)
        {
            return await _RoutingDAL.GetViewByID(ID, Mode);

        }
    }
}