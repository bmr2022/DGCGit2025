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
    public class SSApprovalBLL : ISSApproval
    {
        private readonly SSApprovalDAL _SSAppDAL;
        private readonly IDataLogic _DataLogicDAL;
        public SSApprovalBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _SSAppDAL = new SSApprovalDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetSSData(string Flag, string FromDate, string ToDate, string ApprovalType, string SONO, string SchNo, string VendorName, int Eid, string uid)
        {
            return await _SSAppDAL.GetSSData(Flag, FromDate, ToDate, ApprovalType, SONO, SchNo, VendorName, Eid, uid);
        }
        public async Task<List<SSApprovalDetail>> ShowSSDetail(int ID, int YC, string SchNo)
        {
            return await _SSAppDAL.ShowSSDetail(ID, YC, SchNo);
        }
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string SchNo, string type, int EmpID)
        {
            return await _SSAppDAL.SaveApproval(EntryId, YC, SchNo, type, EmpID);
        }
    }
}
