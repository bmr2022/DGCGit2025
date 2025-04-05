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
    public class MaterialReceiptBLL : IMaterialReceipt
    {
        private readonly MaterialReceiptDAL _MaterialReceiptDAL;
        private readonly IDataLogic _DataLogicDAL;
        public MaterialReceiptBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _MaterialReceiptDAL = new MaterialReceiptDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _MaterialReceiptDAL.GetReportName();
        }
        public async Task<ResponseResult> GetGateNo(string Flag,string SPName,string FromDate,string ToDate)
        {
            return await _MaterialReceiptDAL.GetGateNo(Flag, SPName,FromDate, ToDate);
        }
        public async Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateYearCode,int GateEntryId)
        {
            return await _MaterialReceiptDAL.GetGateMainData(Flag, SPName,GateNo,GateYearCode,GateEntryId);
        }
        public async Task<ResponseResult> GetDeptAndEmp(string Flag, string SPName, int DeptId, int resEmp)
        {
            return await _MaterialReceiptDAL.GetDeptAndEmp(Flag, SPName,DeptId,resEmp);
        }
        public async Task<ResponseResult> SaveMaterialReceipt(MaterialReceiptModel model, DataTable MRGRid, DataTable BatchGrid)
        {
            return await _MaterialReceiptDAL.SaveMaterialReceipt(model, MRGRid,BatchGrid);
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _MaterialReceiptDAL.CheckFeatureOption();
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _MaterialReceiptDAL.GetDashboardData();
        }

        public async Task<ResponseResult> BindDept(string flag,string spname)
        {
            return await _MaterialReceiptDAL.BindDept(flag,spname);
        }

        public async Task<MRNQDashboard> GetDashboardData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            return await _MaterialReceiptDAL.GetDashboardData(VendorName, MrnNo,GateNo,PONo, ItemName, PartCode,  FromDate, ToDate);
        }
        public async Task<MRNQDashboard> GetDetailDashboardData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            return await _MaterialReceiptDAL.GetDetailDashboardData(VendorName, MrnNo,GateNo,PONo, ItemName, PartCode,  FromDate, ToDate);
        }
        public async Task<MaterialReceiptModel> GetViewByID(int ID, int YearCode)
        {
            return await _MaterialReceiptDAL.GetViewByID(ID, YearCode);

        }

        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _MaterialReceiptDAL.GetFormRights(ID);
        }
        
        public async Task<ResponseResult> AltUnitConversion(int ItemCode,decimal AltQty,decimal UnitQty)
        {
            return await _MaterialReceiptDAL.AltUnitConversion(ItemCode,AltQty,UnitQty);
        }


        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _MaterialReceiptDAL.DeleteByID(ID,  YC);
        }
        public async Task<ResponseResult> FillEntryandMRN(string Flag, int YearCode, string SPName)
        {
            return await _MaterialReceiptDAL.FillEntryandMRN(Flag, YearCode,SPName);
        }

        public Task<ResponseResult> GetSearchData(MRNQDashboard model)
        {
            throw new NotImplementedException();
        }
        public async Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            return await _MaterialReceiptDAL.CheckEditOrDelete(MRNNo, YearCode);
        }
        public async Task<ResponseResult> CheckBeforeInsert(string GateNo, int GateYearCode)
        {
            return await _MaterialReceiptDAL.CheckBeforeInsert(GateNo,GateYearCode);
        }
        public async Task<IList<TextValue>> GetEmployeeList()
        {
            return await _MaterialReceiptDAL.GetEmployeeList();
        }

    }
}
