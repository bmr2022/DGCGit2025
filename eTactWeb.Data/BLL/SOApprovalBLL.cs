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
    public class SOApprovalBLL:ISOApproval
    {
        private readonly SOApprovalDAL _SOAppDAL;
        private readonly IDataLogic _DataLogicDAL;
        public SOApprovalBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _SOAppDAL = new SOApprovalDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<ResponseResult> GetProcData(string Flag, string Uid, int EmpId,string SONO, string AccountName)
        {
            return await _SOAppDAL.GetProcData(Flag, Uid, EmpId,SONO,AccountName);
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType,string Uid, int EmpId, string SONO, string AccountName, string CustOrderNo)
        {
            return await _SOAppDAL.GetSearchData(FromDate,ToDate,ApprovalType,Uid,EmpId,SONO,AccountName,CustOrderNo);
        }
        public async Task<List<SoApprovalDetail>> ShowSODetail(int ID, int YC, string SoNo)
        {
            return await _SOAppDAL.ShowSODetail(ID, YC, SoNo);
        }
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string SONO, string CustOrderNo, string type, int EmpID)
        {
            return await _SOAppDAL.SaveApproval(EntryId, YC, SONO,CustOrderNo, type, EmpID);
        }
    }
}
