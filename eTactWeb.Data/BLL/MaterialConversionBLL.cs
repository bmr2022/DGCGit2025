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
    public class MaterialConversionBLL: IMaterialConversion
    {
        private MaterialConversionDAL _MaterialConversionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public MaterialConversionBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _MaterialConversionDAL = new MaterialConversionDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
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

    }
}
