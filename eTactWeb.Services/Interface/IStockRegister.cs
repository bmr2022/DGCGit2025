using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IStockRegister
    {
        Task<StockRegisterModel> GetStockRegisterData(string FromDate, string ToDate,string PartCode,string ItemName, string ItemGroup, string ItemType,int StoreId,string ReportType,string BatchNo,string UniquebatchNo);
        Task<ResponseResult> GetAllItems();
        Task<ResponseResult> GetAllItemTypes();
        Task<ResponseResult> GetAllItemGroups();
        Task<ResponseResult> GetAllStores();

    }
}
