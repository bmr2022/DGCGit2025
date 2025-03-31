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
    public class CustomerJWRBLL : ICustomerJWR
    {
        private readonly CustomerJWRDAL _CustomerJWRDAL;
        private readonly IDataLogic _DataLogicDAL;
        public CustomerJWRBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CustomerJWRDAL = new CustomerJWRDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _CustomerJWRDAL.GetFormRights(userID);
        }

        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            return await _CustomerJWRDAL.GetNewEntry(YearCode);
        }
        public async Task<ResponseResult> GetGateNo(string FromDate,string ToDate)
        {
            return await _CustomerJWRDAL.GetGateNo(FromDate,ToDate);
        }
        public async Task<ResponseResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            return await _CustomerJWRDAL.GetGateMainData(GateNo,GateYearCode,GateEntryId);
        }
        public async Task<ResponseResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            return await _CustomerJWRDAL.GetGateItemData(GateNo,GateYearCode,GateEntryId);
        }
        public async Task<ResponseResult> SaveCustJWR(CustomerJobWorkReceiveModel model, DataTable CustJWRGrid)
        {
            return await _CustomerJWRDAL.SaveCustJWR(model, CustJWRGrid);
        }
        public async Task<CustomerJWRQDashboard> GetSearchData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            return await _CustomerJWRDAL.GetSearchData(FromDate,ToDate,VendorName,MrnNo,ChallanNo,ItemName,PartCode);
        }
        public async Task<CustomerJWRQDashboard> GetSearchDetailData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            return await _CustomerJWRDAL.GetSearchDetailData(FromDate,ToDate,VendorName,MrnNo,ChallanNo,ItemName,PartCode);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string VendorName, string ChallanNO, string PartCode, string ItemName, string MrnNO)
        {
            return await _CustomerJWRDAL.GetDashboardData(FromDate, ToDate, VendorName,ChallanNO,PartCode,ItemName,MrnNO);
        }
        public async Task<CustomerJobWorkReceiveModel> GetViewByID(int ID, int YearCode)
        {
            return await _CustomerJWRDAL.GetViewByID(ID, YearCode);
        }
        public async Task<IList<TextValue>> GetEmployeeList()
        {
            return await _CustomerJWRDAL.GetEmployeeList();
        }
        public async Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            return await _CustomerJWRDAL.CheckEditOrDelete(MRNNo, YearCode);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC,string EntryByMachineName)
        {
            return await _CustomerJWRDAL.DeleteByID(ID, YC, EntryByMachineName);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _CustomerJWRDAL.BindAllDropDowns(Flag);
        }
    }
}
