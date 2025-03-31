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
    public class MaterialConversionBLL: IMaterialConversion
    {
        private MaterialConversionDAL _MaterialConversionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public MaterialConversionBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _MaterialConversionDAL = new MaterialConversionDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _MaterialConversionDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> FillEntryID(int YearCode)
        {
            return await _MaterialConversionDAL.FillEntryID(YearCode);
        }
        public async Task<ResponseResult> FillBranch()
        {
            return await _MaterialConversionDAL.FillBranch();
        }
        public async Task<ResponseResult> FillStoreName()
        {
            return await _MaterialConversionDAL.FillStoreName();
        }
        public async Task<ResponseResult> FillWorkCenterName()
        {
            return await _MaterialConversionDAL.FillWorkCenterName();
        }
         public async Task<ResponseResult> GetOriginalPartCode()
        {
            return await _MaterialConversionDAL.GetOriginalPartCode();
        }
         public async Task<ResponseResult> GetOriginalItemName()
        {
            return await _MaterialConversionDAL.GetOriginalItemName();
        }
        public async Task<ResponseResult> GetUnitAltUnit(int ItemCode)
        {
            return await _MaterialConversionDAL.GetUnitAltUnit(ItemCode);
        }
        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName,string WorkCenterName, int YearCode, string batchno, string FinStartDate)
        {
            return await _MaterialConversionDAL.FillStockBatchNo(ItemCode, StoreName, WorkCenterName, YearCode, batchno, FinStartDate);
        }
        public async Task<ResponseResult> GetAltPartCode(int MainItemcode)
        {
            return await _MaterialConversionDAL.GetAltPartCode(MainItemcode);
        }
        public async Task<ResponseResult> GetAltItemName(int MainItemcode)
        {
            return await _MaterialConversionDAL.GetAltItemName(MainItemcode);
        }
        public async Task<ResponseResult> SaveMaterialConversion(MaterialConversionModel model, DataTable GIGrid)
        {
            return await _MaterialConversionDAL.SaveMaterialConversion(model, GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData(MaterialConversionModel model)
        {
            return await _MaterialConversionDAL.GetDashboardData(model);
        }
        public async Task<MaterialConversionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _MaterialConversionDAL.GetDashboardDetailData( FromDate,  ToDate,  ReportType);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            return await _MaterialConversionDAL.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);
        }
        public async Task<MaterialConversionModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            return await _MaterialConversionDAL.GetViewByID(ID, YC,FromDate,ToDate);
        }
    }
}
