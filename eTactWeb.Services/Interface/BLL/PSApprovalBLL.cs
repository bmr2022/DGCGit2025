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
    public class PSApprovalBLL : IPSApproval
    {
        private readonly PSApprovalDAL _PSAppDAL;
        private readonly IDataLogic _DataLogicDAL;
        public PSApprovalBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _PSAppDAL = new PSApprovalDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetPSData(string Flag,string FromDate, string ToDate, string ApprovalType, string PONO,string SchNo, string VendorName, int Eid, string uid)
        {
            return await _PSAppDAL.GetPSData(Flag,FromDate, ToDate, ApprovalType, PONO,SchNo, VendorName, Eid, uid);
        }
        public async Task<List<PSApprovalDetail>> ShowPSDetail(int ID, int YC, string SchNo)
        {
            return await _PSAppDAL.ShowPSDetail(ID, YC, SchNo);
        }
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string SchNo, string type, int EmpID)
        {
            return await _PSAppDAL.SaveApproval(EntryId, YC, SchNo, type, EmpID);
        }
    }
}
