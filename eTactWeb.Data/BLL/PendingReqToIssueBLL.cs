using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    public class PendingReqToIssueBLL:IPendingReqToIssue
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly PendingReqToIssueDAL _PendReqToissueDAL;
        public PendingReqToIssueBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _PendReqToissueDAL = new PendingReqToIssueDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<DataSet> BindAllDropDowns(string Flag,int YearCode)
        {
            return await _PendReqToissueDAL.BindAllDropDowns(Flag,YearCode);    
        }

        public async Task<ResponseResult> FillItemCode(string reqNo, int workCenter, int deptName,int yearCode, string toDate)
        {
            return await _PendReqToissueDAL.FillItemCode(reqNo,workCenter, deptName,yearCode, toDate);
        }
        public async Task<ResponseResult> FillWorkCenter(string reqNo, int itemCode, int deptName,int yearCode, string toDate)
        {
            return await _PendReqToissueDAL.FillWorkCenter(reqNo, itemCode, deptName,yearCode, toDate);
        }
        public async Task<ResponseResult> FillDeptName(string reqNo, int itemCode, int workCenter,int yearCode, string toDate)
        {
            return await _PendReqToissueDAL.FillDeptName(reqNo, itemCode, workCenter, yearCode, toDate);
        }
        public async Task<ResponseResult> FillRequisition(int toDept, int itemCode, int workCenter,int yearCode, string toDate)
        {
            return await _PendReqToissueDAL.FillRequisition(toDept, itemCode, workCenter, yearCode, toDate);
        }
        public async Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId)
        {
            return await _PendReqToissueDAL.ShowDetail(FromDate, ToDate, ReqNo, YearCode, ItemCode, WoNo, WorkCenter, DeptName,ReqYear, IssueDate, GlobalSearch, FromStore, StoreId);
        }
        public async Task<ResponseResult> BindReqYear(int yearCode, string toDate)
        {
            return await _PendReqToissueDAL.BindReqYear(yearCode, toDate);
        }
        public async Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate, string BatchNo, string UniqBatchNo,int YearCode, int StoreId)
        {
            return await _PendReqToissueDAL.CheckTransDate(ItemCode,IssueDate,BatchNo,UniqBatchNo,YearCode, StoreId);
        }
        public async Task<ResponseResult> EnableOrDisableIssueDate()
        {
            return await _PendReqToissueDAL.EnableOrDisableIssueDate();
        }
        public async Task<ResponseResult> GetAlternateItemCode(int MainIC)
        {
            return await _PendReqToissueDAL.GetAlternateItemCode(MainIC);
        }
    }
}
