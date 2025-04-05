using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class RetFromDepartmentMainBLL : IRetFromDepartmentMain
    {
        private RetFromDepartmentMainDAL _RetFromDepartmentMainDAL;
        private readonly IDataLogic _DataLogicDAL;

        public RetFromDepartmentMainBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _RetFromDepartmentMainDAL = new RetFromDepartmentMainDAL(config, dataLogicDAL,connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _RetFromDepartmentMainDAL.GetDashboardData(FromDate, Todate, Flag);
        }
        public async Task<ResponseResult> FillItems(string returnByEmpName, string showAllItem)
        {
            return await _RetFromDepartmentMainDAL.FillItems(returnByEmpName, showAllItem);
        }
        public async Task<ResponseResult> FillPartCode(string returnByEmpName)
        {
            return await _RetFromDepartmentMainDAL.FillPartCode(returnByEmpName);
        }

        public async Task<ResponseResult> FillBatchNo(int itemCode, int receiveByEmp, DateTime? retFromDepEntrydate, int retFromDepYearCode)
        {
            return await _RetFromDepartmentMainDAL.FillBatchNo(itemCode, receiveByEmp, retFromDepEntrydate, retFromDepYearCode);
        }
        public async Task<ResponseResult> GetNewEntry(int yearCode)
        {
            return await _RetFromDepartmentMainDAL.GetNewEntry(yearCode);
        }

        public async Task<ResponseResult> SaveRetFromDeptMain(ReturnFromDepartmentMainModel model, DataTable ReqGrid)
        {
            return await _RetFromDepartmentMainDAL.SaveRetFromDeptMain(model, ReqGrid);
        }

        public async Task<ReturnFromDepartmentMainModel> GetViewByID(int ID, int YearCode)
        {
            return await _RetFromDepartmentMainDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _RetFromDepartmentMainDAL.DeleteByID(ID, YC);
        }
    }
}
