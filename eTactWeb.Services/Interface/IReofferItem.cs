using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IReofferItem
    {
        Task<ResponseResult> GETNEWENTRY(int ReofferYearcode);
        Task<ResponseResult> FILLQCTYPE();
        Task<ResponseResult> FILLMIRNO(string ReofferMir, string MRNNO);
        Task<ResponseResult> FILLMRNNO(string ReofferMir);
        Task<ResponseResult> BINDSTORE();
        Task<ResponseResult> FILLMIRYearCode(string MIRNO);
       
        Task<ResponseResult> FILLMIRData(string MIRNO,int MIRYearCode);
        Task<ResponseResult> GetItemDeatil(string MIRNO,int MIRYearCode,int accountcode,string ReofferMir);
        Task<ResponseResult> GetItemData(string MIRNO,int MIRYearCode,int accountcode,string ReofferMir);
        Task<ResponseResult> GetItemQty(string MIRNO,int MIRYearCode,int accountcode,string ReofferMir,int itemcode);
        Task<ResponseResult> FillOkRecStore(int itemcode,string ShowAllStore);
        Task<ResponseResult> ALLOWSHOWALLSTORE();
        Task<ResponseResult> RejSTORE();
        Task<ResponseResult> RewSTORE();
        Task<ResponseResult> HoldSTORE();
        Task<ResponseResult> BINDEMP();
        Task<ResponseResult> FillPODetail(string MIRNO, int MIRYearCode, int accountcode, int itemcode);
        Task<ResponseResult> SaveReoffer(ReOfferItemModel model, DataTable ISTGrid);
        Task<ReOfferItemModel> GetDashBoardDetailData(string FromDate, string ToDate, string ReportType);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName);
        Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);
        Task<ReOfferItemModel> GetViewByID(int ID, string Mode, int YC);
    }
}
