using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IInProcessInspection
    {
		Task<ResponseResult> FillPartCode(string InspectionType);
		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillShift();
		Task<ResponseResult> FillMachineName();
		Task<ResponseResult> FillCustomer();
		Task<ResponseResult> FillColor(string PartNo);
		Task<ResponseResult> FillEntryID(int YearCode,string TestingDate);
		Task<InProcessInspectionModel> GetInprocessInspectionGridData(int ItemCode, int SampleSize);
		Task<ResponseResult> SaveInprocessInspection(InProcessInspectionModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData(InProcessInspectionModel model);
        Task<InProcessInspectionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType,string ItemName,string PartCode,string SlipNo,string MachinNo);
        Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId);
        Task<InProcessInspectionModel> GetViewByID(int ID, int YC, string FromDate, string TODate);
    }
}
