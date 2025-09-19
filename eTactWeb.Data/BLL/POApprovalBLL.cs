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
    public class POApprovalBLL : IPOApproval
    {
        private readonly POApprovalDAL _POAppDAL;
        private readonly IDataLogic _DataLogicDAL;
        public POApprovalBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _POAppDAL = new POApprovalDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetInitialData(string Flag, string Uid, int EmpId)
        {
            return await _POAppDAL.GetInitialData(Flag, Uid,EmpId);
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string PONO, string VendorName,int Eid,string uid)
        {
            return await _POAppDAL.GetSearchData(FromDate, ToDate, ApprovalType, PONO, VendorName,Eid,uid);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _POAppDAL.GetReportName();
        }
        public async Task<ResponseResult> GetFeaturesOptions()
        {
            return await _POAppDAL.GetFeaturesOptions();
        }
        public async Task<ResponseResult> GetMobileNo(int ID, int YearCode, string PoNo)
        {
            return await _POAppDAL.GetMobileNo(ID, YearCode,PoNo);
        }
        public async Task<ResponseResult> GetAllowedAction(string Flag, int EmpId)
        {
            return await _POAppDAL.GetAllowedAction(Flag,EmpId);
        }
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string PONO, string type, int EmpID)
        {
            return await _POAppDAL.SaveApproval(EntryId, YC, PONO, type, EmpID);
        }
        public async Task<List<POApprovalDetail>> ShowPODetail(int ID, int YC, string PoNo, string TypeOfApproval,string showonlyamenditem)
        {
            return await _POAppDAL.ShowPODetail(ID, YC,PoNo,TypeOfApproval, showonlyamenditem);
        }
    }
}
