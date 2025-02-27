using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISSApproval
    {
        Task<ResponseResult> GetSSData(string Flag, string FromDate, string ToDate, string ApprovalType, string SONO, string SchNo, string VendorName, int EmpId, string UID);
        Task<List<SSApprovalDetail>> ShowSSDetail(int ID, int YearCode, string SchNo);
        Task<ResponseResult> SaveApproval(int EntryId, int YC, string SchNo, string type, int EmpID);

    }
}
