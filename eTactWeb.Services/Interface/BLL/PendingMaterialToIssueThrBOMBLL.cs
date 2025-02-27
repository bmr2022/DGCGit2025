using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.BLL
{
    public class PendingMaterialToIssueThrBOMBLL : IPendingMaterialToIssueThrBOM
    {
        private readonly PendingMaterialToIssueThrBOMDAL _PendingMaterialToIssueThrBOMDAL;
        private readonly IDataLogic _IDataLogic;

        public PendingMaterialToIssueThrBOMBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            _PendingMaterialToIssueThrBOMDAL = new PendingMaterialToIssueThrBOMDAL(configuration, iDataLogic);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag, int YearCode)
        {
            return await _PendingMaterialToIssueThrBOMDAL.BindAllDropDowns(Flag, YearCode);
        }

        public async Task<ResponseResult> FillRequisition(int toDept, int itemCode, int workCenter, int yearCode, string toDate)
        {
            return await _PendingMaterialToIssueThrBOMDAL.FillRequisition(toDept,itemCode,workCenter,yearCode, toDate);
        }  
        public async Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId)
        {
            return await _PendingMaterialToIssueThrBOMDAL.ShowDetail(FromDate, ToDate, ReqNo, YearCode, ItemCode, WoNo, WorkCenter, DeptName, ReqYear, IssueDate, GlobalSearch, FromStore, StoreId);
        }
        public async Task<ResponseResult> EnableOrDisableIssueDate()
        {
            return await _PendingMaterialToIssueThrBOMDAL.EnableOrDisableIssueDate();
        }

        public async Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate, string BatchNo, string UniqBatchNo, int YearCode)
        {
            return await _PendingMaterialToIssueThrBOMDAL.CheckTransDate(ItemCode, IssueDate,BatchNo,UniqBatchNo,YearCode);
        }
    }
}
