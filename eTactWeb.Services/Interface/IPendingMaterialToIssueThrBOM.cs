using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Services.Interface
{
    public interface IPendingMaterialToIssueThrBOM
    {
        Task<DataSet> BindAllDropDowns(string Flag, int YearCode);
        Task<ResponseResult> FillRequisition(int toDept, int itemCode, int workCenter, int yearCode, string toDate);

        Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId);        
        Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate, string BatchNo, string UniqBatchNo, int YearCode);
    }
}
