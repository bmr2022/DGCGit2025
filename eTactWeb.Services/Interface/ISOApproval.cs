using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISOApproval
    {
        Task<ResponseResult> GetProcData(string Flag, string UId, int EmpId,string SONO, string AccountName);
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType,string Uid, int Empid, string SONO, string AccountName,string CustOrderNo);
        Task<List<SoApprovalDetail>> ShowSODetail(int ID, int YearCode, string SoNo);
        Task<ResponseResult> SaveApproval(int EntryId, int YC, string SONO,string CustOrderNo, string type, int EmpID);
    }
}
