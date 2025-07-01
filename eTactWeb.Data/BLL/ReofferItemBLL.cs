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
    public class ReofferItemBLL:IReofferItem
    {
        private ReOfferItemDAL? _IReOfferItemDAL { get; }
        public ReofferItemBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IReOfferItemDAL = new ReOfferItemDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> GETNEWENTRY(int ReofferYearcode)
        {
            return await _IReOfferItemDAL.GETNEWENTRY( ReofferYearcode);
        }

        public async Task<ResponseResult> FILLQCTYPE()
        {
            return await _IReOfferItemDAL.FILLQCTYPE();
        }
        public async Task<ResponseResult> FILLMIRNO(string ReofferMir, string MRNNO)
        {
            return await _IReOfferItemDAL.FILLMIRNO( ReofferMir,MRNNO);
        }
        public async Task<ResponseResult> FILLMRNNO(string ReofferMir)
        {
            return await _IReOfferItemDAL.FILLMRNNO(ReofferMir);
        }
        public async Task<ResponseResult> FILLMIRYearCode(string MIRNO)
        {
            return await _IReOfferItemDAL.FILLMIRYearCode(MIRNO);
        }
        public async Task<ResponseResult> FILLMIRData(string MIRNO, int MIRYearCode)
        {
            return await _IReOfferItemDAL.FILLMIRData(MIRNO,  MIRYearCode);
        }
        public async Task<ResponseResult> BINDSTORE()
        {
            return await _IReOfferItemDAL.BINDSTORE();
        }
        public async Task<ResponseResult> GetItemDeatil(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            return await _IReOfferItemDAL.GetItemDeatil( MIRNO,  MIRYearCode,  accountcode,  ReofferMir);
        }
        public async Task<ResponseResult> GetItemData(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            return await _IReOfferItemDAL.GetItemData(MIRNO, MIRYearCode, accountcode, ReofferMir);
        }

        public async Task<ResponseResult> GetItemQty(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir, int itemcode)
        {
            return await _IReOfferItemDAL.GetItemQty(MIRNO, MIRYearCode, accountcode, ReofferMir,itemcode);
        }
        public async Task<ResponseResult> FillOkRecStore(int itemcode, string ShowAllStore)
        {
            return await _IReOfferItemDAL.FillOkRecStore( itemcode,  ShowAllStore);
        }
        public async Task<ResponseResult> ALLOWSHOWALLSTORE()
        {
            return await _IReOfferItemDAL.ALLOWSHOWALLSTORE();
        }
        
        public async Task<ResponseResult> RejSTORE()
        {
            return await _IReOfferItemDAL.RejSTORE();
        }

        public async Task<ResponseResult> RewSTORE()
        {
            return await _IReOfferItemDAL.RewSTORE();
        }
        public async Task<ResponseResult> HoldSTORE()
        {
            return await _IReOfferItemDAL.HoldSTORE();
        }
        public async Task<ResponseResult> BINDEMP()
        {
            return await _IReOfferItemDAL.BINDEMP();
        }
        public async Task<ResponseResult> FillPODetail(string MIRNO, int MIRYearCode, int accountcode, int itemcode)
        {
            return await _IReOfferItemDAL.FillPODetail( MIRNO,  MIRYearCode,  accountcode,  itemcode);
        }
        public async Task<ResponseResult> SaveReoffer(ReOfferItemModel model, DataTable ISTGrid)
        {
            return await _IReOfferItemDAL.SaveReoffer(model, ISTGrid);
        }
        public async Task<ReOfferItemModel> GetDashBoardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _IReOfferItemDAL.GetDashBoardDetailData(FromDate, ToDate, ReportType);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _IReOfferItemDAL.GetDashboardData();
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName)
        {
            return await _IReOfferItemDAL.DeleteByID(ID, YC, EntryDate, ActualEntryBy, MachineName);
        }
        public async Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IReOfferItemDAL.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
        }

        public async Task<ReOfferItemModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _IReOfferItemDAL.GetViewByID(ID, Mode, YC);
        }

    }

}
