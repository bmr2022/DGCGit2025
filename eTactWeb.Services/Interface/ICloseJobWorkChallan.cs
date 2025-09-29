using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICloseJobWorkChallan
    {
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll);

        Task<List<CloseJobWorkChallanModel>> ShowDetail(int ID, int YearCode);
        Task<ResponseResult> FillVendorList(string fromDate, string toDate,string ShowClsoedPendingAll);
        Task<ResponseResult> FillJWChallanList(string fromDate, string toDate,string ShowClsoedPendingAll);

        Task<ResponseResult> SaveActivation(int JWCloseEntryId, int JWCloseYearCode, string JWCloseEntryDate, int JWCloseEntryByEmpid, string VendJwCustomerJW, int AccountCode,int VendJWIssEntryId,int  VendJWIssYearCode, string VendJWIssChallanNo,
           string VendJWIssChallanDate,int CustJwIssEntryid,int CustJwIssYearCode,string CustJwIssChallanNo,string CustJwIssChallanDate,float TotalChallanAmount,float NetAmount,string ClosingReason,
           string CC,string ActualEntryDate,int ActualEnteredBy,string EntryByMachineName,string ShowClsoedPendingAll);




    }
}
