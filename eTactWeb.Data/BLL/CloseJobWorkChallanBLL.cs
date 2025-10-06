using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
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
    public class CloseJobWorkChallanBLL: ICloseJobWorkChallan
    {
        private CloseJobWorkChallanDAL _CloseJobWorkChallanDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CloseJobWorkChallanBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _CloseJobWorkChallanDAL = new CloseJobWorkChallanDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll, string ChallanYear)
        {
            return await _CloseJobWorkChallanDAL.GetSearchData( FromDate,  ToDate,  AccountCode,  ChallanNO,  ShowClsoedPendingAll, ChallanYear);
        }
        public async Task<List<CloseJobWorkChallanModel>> ShowDetail(int ID, int YC,string ShowClsoedPendingAll, string ChallanYear)
        {
            return await _CloseJobWorkChallanDAL.ShowDetail(ID, YC, ShowClsoedPendingAll, ChallanYear);
        }
        public async Task<ResponseResult> SaveActivation(int JWCloseEntryId, int JWCloseYearCode, string JWCloseEntryDate, int JWCloseEntryByEmpid, string VendJwCustomerJW, int AccountCode, int VendJWIssEntryId, int VendJWIssYearCode, string VendJWIssChallanNo,
           string VendJWIssChallanDate, int CustJwIssEntryid, int CustJwIssYearCode, string CustJwIssChallanNo, string CustJwIssChallanDate, float TotalChallanAmount, float NetAmount, string ClosingReason,
           string CC, string ActualEntryDate, int ActualEnteredBy, string EntryByMachineName, string ShowClsoedPendingAll,string ChallanYear,string CloseAllItem, string itemcode)
        {
            return await _CloseJobWorkChallanDAL.SaveActivation( JWCloseEntryId,  JWCloseYearCode,  JWCloseEntryDate,  JWCloseEntryByEmpid,  VendJwCustomerJW,  AccountCode,  VendJWIssEntryId,   VendJWIssYearCode,  VendJWIssChallanNo,
            VendJWIssChallanDate,  CustJwIssEntryid,  CustJwIssYearCode,  CustJwIssChallanNo,  CustJwIssChallanDate,  TotalChallanAmount,  NetAmount,  ClosingReason,
            CC,  ActualEntryDate,  ActualEnteredBy,  EntryByMachineName, ShowClsoedPendingAll, ChallanYear, CloseAllItem, itemcode);
        }

        public async Task<ResponseResult> FillVendorList(string fromDate, string toDate, string ShowClsoedPendingAll,string ChallanYear)
        {
            return await _CloseJobWorkChallanDAL.FillVendorList( fromDate,  toDate,  ShowClsoedPendingAll, ChallanYear);
        }
         public async Task<ResponseResult> FillJWChallanList(string fromDate, string toDate, string ShowClsoedPendingAll,string ChallanYear)
        {
            return await _CloseJobWorkChallanDAL.FillJWChallanList( fromDate,  toDate,  ShowClsoedPendingAll, ChallanYear);
        }

    }
}
