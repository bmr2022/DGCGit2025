using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ItemGroupModel : TimeStamp
    {
        //        Group_Code bigint
        //Group_name nvarchar
        //Under_GroupCode bigint
        //Entry_date datetime
        //GroupCatCode nvarchar
        //UnderCategoryId bigint
        //seqNo bigint

        public int? Group_Code { get; set; }
        [Required, MaxLength(30)]
        public string? Group_name { get; set; }

        public string? Main_Category_Type { get; set; }
        public string? ItemServAssets { get; set; }
        public int Under_GroupCode { get; set; }
        public string? Entry_date { get; set; }

        public string? GroupCatCode { get; set; }
        public string? GroupPrefix { get; set; }
        public string? ItemCategory { get; set; }

        public string? OptDelete { get; set; }
        public string? OptUpdate { get; set; }
        public string? OptView { get; set; }
        public int? UnderCategoryId { get; set; }
        public int seqNo { get; set; }
        public IList<TextValue>? ItemCateList { get; set; }

        //public IList<AccountMasterModel>? AccountMasterList { get; set; }
        public IList<ItemGroupModel>? ItemGroupList { get; set; }
    }
}
