using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICustomerJWR
    {
        Task<ResponseResult> GetNewEntry(int YearCode);
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> GetGateNo(string FromDate,string ToDate);
        Task<ResponseResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId);
        Task<ResponseResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId);
        Task<ResponseResult> SaveCustJWR(CustomerJobWorkReceiveModel model, DataTable CustJWRGrid);
        Task<ResponseResult> GetDashboardData(string FromDate,string ToDate,string VendorName,string ChallanNO,string PartCode,string ItemName,string MrnNO);
        Task<CustomerJWRQDashboard> GetSearchData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode);
        Task<CustomerJWRQDashboard> GetSearchDetailData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode);
        Task<CustomerJobWorkReceiveModel> GetViewByID(int ID, int YearCode);
        Task<IList<TextValue>> GetEmployeeList();
        Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode);
        Task<ResponseResult> DeleteByID(int ID, int YC, string EntryByMachineName);
    }
}
