using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    public class PendingMRNtoQcBLL : IPendingMRNToQC
    {
        private readonly PendingMRNtoQcDAL _PendDal;
        private readonly IDataLogic _DataLogicDAL;
        public PendingMRNtoQcBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _PendDal = new PendingMRNtoQcDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<DataSet> BindData(string Flag)
        {
            return await _PendDal.BindData(Flag);
        }

        public async Task<ResponseResult> GetDataForPendingMRN(string Flag,string MRNJW, int YearCode, string FromDate, string ToDate, int AccountCode, string MrnNo, int ItemCode, string InvoiceNo, int DeptId)
        {
            return await _PendDal.GetDataForPendingMRN(Flag,MRNJW, YearCode, FromDate, ToDate, AccountCode, MrnNo, ItemCode, InvoiceNo,DeptId);
        }
        public async Task<ResponseResult> GetDeptForUser(int Empid)
        {
            return await _PendDal.GetDeptForUser(Empid);
        }
    }
}
