using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
        public async Task<ResponseResult> FILLMIRNO()
        {
            return await _IReOfferItemDAL.FILLMIRNO();
        }
    }

}
