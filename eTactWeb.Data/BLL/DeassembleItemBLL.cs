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
    public class DeassembleItemBLL:IDeassembleItem
    {
        private DeassembleItemDAL? _IDeassembleItemDAL { get; }
        public DeassembleItemBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDeassembleItemDAL = new DeassembleItemDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> NewEntryId()
        {
            return await _IDeassembleItemDAL.NewEntryId();
        }
        public async Task<ResponseResult> BomQty(int RMItemCode)
        {
            return await _IDeassembleItemDAL.BomQty(RMItemCode);
        }
        public async Task<ResponseResult> FillMRNNO(int FGItemCode, int yearcode)
        {
            return await _IDeassembleItemDAL.FillMRNNO( FGItemCode,  yearcode);
        }

        public async Task<ResponseResult> FillMRNYearCode(int FGItemCode, int yearcode, string MRNNO)
        {
            return await _IDeassembleItemDAL.FillMRNYearCode(FGItemCode, yearcode,MRNNO);
        }

        public async Task<ResponseResult> FillMRNDetail( int yearcode, string MRNNO,int mrnyearcode)
        {
            return await _IDeassembleItemDAL.FillMRNDetail(yearcode, MRNNO, mrnyearcode);
        }


        public async Task<ResponseResult> FillStore()
        {
            return await _IDeassembleItemDAL.FillStore();
        }
        public async Task<ResponseResult> FillFGItemName()
        {
            return await _IDeassembleItemDAL.FillFGItemName();
        }
        public async Task<ResponseResult> FillFGPartCode()
        {
            return await _IDeassembleItemDAL.FillFGPartCode();
        }
        
        public async Task<ResponseResult> FillBomNo(int FinishItemCode)
        {
            return await _IDeassembleItemDAL.FillBomNo(FinishItemCode);
        }
        public async Task<ResponseResult> FillRMItemName(int FinishItemCode,int BomNo)
        {
            return await _IDeassembleItemDAL.FillRMItemName(FinishItemCode, BomNo);
        }
        public async Task<ResponseResult> FillRMPartCode(int FinishItemCode,int BomNo)
        {
            return await _IDeassembleItemDAL.FillRMPartCode(FinishItemCode, BomNo);
        }

        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno, string FinStartDate)
        {
            return await _IDeassembleItemDAL.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno, FinStartDate);
        }

        public async Task<ResponseResult> SaveDeassemble(DeassembleItemModel model, DataTable ISTGrid)
        {
            return await _IDeassembleItemDAL.SaveDeassemble(model, ISTGrid);
        }

        public async Task<DeassembleItemDashBoard> GetDashBoardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _IDeassembleItemDAL.GetDashBoardDetailData(FromDate, ToDate,   ReportType);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _IDeassembleItemDAL.GetDashboardData();
        }

    }
}
