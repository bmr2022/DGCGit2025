using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISOCancel
    {
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string CancelType, string Uid, int Empid, string SONO, string AccountName, string CustOrderNo);
        Task<List<SoCancelDetail>> ShowSODetail(int ID, int YearCode, string SoNo);
        Task<ResponseResult> SaveActivation(int EntryId, int YC, string SONO, string CustOrderNo, string type, int EmpID);
    }
}
