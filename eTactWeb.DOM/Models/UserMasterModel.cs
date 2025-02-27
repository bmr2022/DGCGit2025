using System.ComponentModel.DataAnnotations;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    [Serializable()]
    public class UserMasterModel : TimeStamp
    {        
        public string? CnfPass { get; set; }

        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]
        public string? EmailID { get; set; }

        public string? EmpCode { get; set; }
        public string? EmpID { get; set; }

        public string? EmpName { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }

        public IList<TextValue>? EmpNameList { get; set; }

        public string? MobileNo { get; set; }                
        public string? Password { get; set; }

        public IList<UserMasterModel>? UserMasterList { get; set; }

        [Required]
        public string? UserName { get; set; }

        //[Range(101, 999)]
        public string? UserType { get; set; }

        public IList<TextValue>? UserTypeList { get; set; }
    }
}