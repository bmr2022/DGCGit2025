using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDeassembleItem
    {
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> NewEntryId();
        Task<ResponseResult> BomQty(int RMItemCode, int FinishItemCode, int bomNo, float FGQty);

        Task<ResponseResult> FillMRNNO(int FGItemCode,int yearcode);
        Task<ResponseResult> FillMRNYearCode(int FGItemCode,int yearcode,string MRNNO);

        Task<ResponseResult> FillMRNDetail(int yearcode, string MRNNO,int mrnyearcode);
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillFGItemName();
        Task<ResponseResult> FillFGPartCode();
        Task<ResponseResult> FillBomNo(int FinishItemCode);
        Task<ResponseResult> FillRMItemName(int FinishItemCode,int BomNo);
        Task<ResponseResult> FillRMPartCode(int FinishItemCode,int BomNo);
        Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno, string FinStartDate);

        Task<ResponseResult> SaveDeassemble(DeassembleItemModel model, DataTable ISTGrid);

        Task<DeassembleItemDashBoard> GetDashBoardDetailData(string FromDate, string ToDate,string  ReportType);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName);
        Task<DeassembleItemModel> GetViewByID(int ID, string Mode, int YC);

        Task<ResponseResult> FILLRMAndBomDetail(int FinishItemCode, int bomNo, decimal FGQty);

    }
}
