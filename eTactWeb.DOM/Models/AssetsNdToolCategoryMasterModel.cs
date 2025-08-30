using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class AssetsNdToolCategoryMasterModel
    {
        public long AssetsCateogryId { get; set; }
        public string AsstesToolInstrument { get; set; }
        public string AssetsCategoryName { get; set; }
        public string AssetsCategoryCode { get; set; }
        public long? AssetsMainCateogryId { get; set; }
        public long? ToolMaincategoryId { get; set; }
        public string EntryByMachineName { get; set; }
        public long ActualEntryBy { get; set; }
        public DateTime? ActualEntryDate { get; set; }
        public DateTime? LastUpdationDate { get; set; }
        public long? LastUpdatedBy { get; set; }
        public string Mode { get; set; }
        public string? Otherdetail { get; set; }

        // Dashboard extras (only if SP joins with employee tables)
        public string? UpdatedByEmp { get; set; }
        public string? ActualEntryEmp { get; set; }

        // New fields from SP (AssetsMaster join)
        public string? AssetsName { get; set; }
        public long? AssetsEntryId { get; set; }

    }
}
