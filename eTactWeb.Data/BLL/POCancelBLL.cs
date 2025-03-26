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
    public class POCancelBLL:IPOCancel
    {
        private readonly POCancelDAL _POCancelDAL;
        private readonly IDataLogic _DataLogicDAL;
        public POCancelBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _POCancelDAL = new POCancelDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string CancelationType, string PONO, string VendorName, int Eid, string uid)
        {
            return await _POCancelDAL.GetSearchData(FromDate, ToDate, CancelationType, PONO, VendorName, Eid, uid);
        }

        public async Task<List<POCancleDetail>> ShowPODetail(int ID, int YC, string PoNo, string TypeOfCancle)
        {
            return await _POCancelDAL.ShowPODetail(ID, YC, PoNo, TypeOfCancle);
        }

        public async Task<ResponseResult> SaveCancelation(int EntryId, int YC, string PONO, string type, int EmpID)
        {
            return await _POCancelDAL.SaveCancelation(EntryId, YC, PONO, type, EmpID);
        }
    }
}
