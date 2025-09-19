using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPOApproval
    {
        Task<ResponseResult> GetInitialData(string Flag, string UId, int EmpId);
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string PONO, string VendorName,int EmpId, string UID);
        Task<ResponseResult> GetAllowedAction(string Flag, int EmpId);
        Task<ResponseResult> SaveApproval(int EntryId, int YC, string PONO, string type,int EmpID);
        Task<List<POApprovalDetail>> ShowPODetail(int ID, int YearCode, string PoNo, string TypeOfApproval,string showonlyamenditem);
        Task<ResponseResult> GetReportName();
        Task<ResponseResult> GetFeaturesOptions();
        Task<ResponseResult> GetMobileNo(int ID, int YearCode, string PoNo);
    }
}
