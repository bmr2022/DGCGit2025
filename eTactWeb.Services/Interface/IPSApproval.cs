using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPSApproval
    {
        Task<ResponseResult> GetPSData(string Flag, string FromDate, string ToDate, string ApprovalType, string PONO,string SchNo, string VendorName, int EmpId, string UID);
        Task<List<PSApprovalDetail>> ShowPSDetail(int ID, int YearCode, string SchNo,string ShowOnlyAmendItem);
        Task<ResponseResult> SaveApproval(int EntryId, int YC, string SchNo, string type, int EmpID);

    }
}
