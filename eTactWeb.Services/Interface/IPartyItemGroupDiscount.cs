using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPartyItemGroupDiscount
    {
		Task<ResponseResult> FillPartyName();
		Task<ResponseResult> FillCategoryName();
		Task<ResponseResult> FillCategoryCode();
		Task<ResponseResult> FillGroupName();
		Task<ResponseResult> FillGroupCode();
	}
}
