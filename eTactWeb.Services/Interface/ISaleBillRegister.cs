using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISaleBillRegister
    {
        //3--//1
        Task<ResponseResult> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, int itemCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO, int AccountCode, int yearcode,int ItemParentGroup);

        Task<ResponseResult> FillCustomerList(string FromDate, string ToDate, string SearchText);//4
        Task<ResponseResult> FillItemGroupList(string FromDate, string ToDate, string SearchText);//4
        Task<ResponseResult> FillDocumentList(string FromDate, string ToDate);//5
        Task<ResponseResult> FillSaleBillList(string FromDate, string ToDate, string SearchText);//8
        Task<ResponseResult> FillItemNamePartcodeList(string FromDate, string ToDate, string PartCode, string ItemName);//1
        Task<ResponseResult> FillSONO(string FromDate, string ToDate, string SearchText);//9
        Task<ResponseResult> FillSchNo(string FromDate, string ToDate, string SearchText);//2
        Task<ResponseResult> FillHSNNo(string FromDate, string ToDate, string SearchText);//7
        Task<ResponseResult> FillGSTNo(string FromDate, string ToDate, string SearchText);//6
    }
}
