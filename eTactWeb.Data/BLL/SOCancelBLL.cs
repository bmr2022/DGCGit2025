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
    public class SOCancelBLL:ISOCancel
    {
        private readonly SOCancelDAL _SOCancelDAL;
        private readonly IDataLogic _DataLogicDAL;
        public SOCancelBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _SOCancelDAL = new SOCancelDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string CancelType, string Uid, int EmpId, string SONO, string AccountName, string CustOrderNo)
        {
            return await _SOCancelDAL.GetSearchData(FromDate, ToDate, CancelType, Uid, EmpId, SONO, AccountName, CustOrderNo);
        }

        public async Task<List<SoCancelDetail>> ShowSODetail(int ID, int YC, string SoNo)
        {
            return await _SOCancelDAL.ShowSODetail(ID, YC, SoNo);
        }

        public async Task<ResponseResult> SaveActivation(int EntryId, int YC, string SONO, string CustOrderNo, string type, int EmpID)
        {
            return await _SOCancelDAL.SaveActivation(EntryId, YC, SONO, CustOrderNo, type, EmpID);
        }
    }
}
