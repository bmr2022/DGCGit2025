using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IWIPStockRegister
    {
        Task<ResponseResult> GetAllWorkCenter();
        Task<WIPStockRegisterModel> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int WCID, string ReportType, string BatchNo, string UniqueBatchNo,string WorkCenter);
        Task<ResponseResult> GetAllItemGroups();
        Task<ResponseResult> GetAllItemTypes();
    }
}
