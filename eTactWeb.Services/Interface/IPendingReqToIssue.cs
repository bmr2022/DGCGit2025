using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingReqToIssue
    {
        Task<DataSet> BindAllDropDowns(string Flag,int YearCode);
        Task<ResponseResult> FillItemCode(string reqNo, int workCenter, int deptName,int yearCode, string toDate);
        Task<ResponseResult> FillWorkCenter(string reqNo, int itemCode, int deptName,int yearCode, string toDate);
        Task<ResponseResult> FillDeptName(string reqNo, int itemCode, int workCenter,int yearCode, string toDate);
        Task<ResponseResult> FillRequisition(int toDept, int itemCode, int workCenter,int yearCode, string toDate);
        Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId);
        Task<ResponseResult> BindReqYear(int yearCode, string toDate);
        Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate,string BatchNo, string UniqBatchNo,int YearCode, int StoreId );
        Task<ResponseResult> EnableOrDisableIssueDate();
        Task<ResponseResult> GetAlternateItemCode(int MainIC);

    }
}
