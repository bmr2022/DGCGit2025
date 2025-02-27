using static eTactWeb.DOM.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace eTactWeb.DOM.Models
{
    public class ProcessMasterModel : TimeStamp
    {
        [Key]
        public int Process_Id { get; set; }

        public string StageCode { get; set; }
        public string StageDescription { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public string StageShortName { get; set; }
        public int? MaterialReqForRework { get; set; }
        public int? QCRequired { get; set; }
        public int MaxPendQtyReq { get; set; }
        public IList<ProcessMasterModel>? ProcessMasterList { get; set; }

    }
}
