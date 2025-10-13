using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPOApprovalPolicy
    {
		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillPartCode();
		Task<ResponseResult> FillEmpName();
		Task<ResponseResult> FillCatName();
		Task<ResponseResult> FillGroupName();
		Task<ResponseResult> FillEntryID();
	}
}
