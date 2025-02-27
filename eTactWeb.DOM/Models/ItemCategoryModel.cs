using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ItemCategoryModel : TimeStamp
    {
        [Key]
        public int Entry_id { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Entry_Date { get; set; } = DateTime.UtcNow;
        //public DateTime Entry_Date { get; set; } = DateTime.Today;
        public IList<ItemCategoryModel>? ItemCatList { get; set; }
        public long Year_code { get; set; }
        [Required]
        public string Type_Item { get; set; } = string.Empty;
        [Required]
        public string Main_Category_Type { get; set; } = string.Empty;
        [Required]
        public string CC { get; set; } = string.Empty;
        public long Uid { get; set; }
        public string Category_Code { get; set; } = string.Empty;
        public string? Category_Name { get; set; }

        public IList<SelectListItem> MainItemCateory { get; set; } =
            new List<SelectListItem>()
            {
                new() { Value = "FINISHED", Text = "FINISHED" },
                new() { Value = "RAW MATERIAL", Text = "RAW MATERIAL" },
                  new() { Value = "SCRAP", Text = "SCRAP" },
                 new() { Value = "PACKING", Text = "PACKING" },
                  new() { Value = "FINISHED", Text = "FINISHED" },
                 new() { Value = "GENERAL ITEMS", Text = "GENERAL ITEMS" },
                 new() { Value = "FIXED ASSETS & TOOLS PLANTS", Text = "FIXED ASSETS & TOOLS PLANTS" },
                  new() { Value = "MACHINARY", Text = "MACHINARY" },
                  new() { Value = "SPARES", Text = "SPARES" },
                 new() { Value = "OTHERS", Text = "OTHERS" },
            };

    }
}
