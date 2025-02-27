using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class UserRightModel : TimeStamp
    {
        public string? All { get; set; }
        public string? Delete { get; set; }
        public int EmpID { get; set; }
        public IList<TextValue>? UserList { get; set; }
        public string? EmpName { get; set; }
        public string? UserName { get; set; }
        public string? MainMenu { get; set; }
        public IList<TextValue>? MainMenuList { get; set; }
        public string? Module { get; set; }
        public IList<TextValue>? ModuleList { get; set; }
        public string? Save { get; set; }
        public string? SubMenu { get; set; }
        public string? SubMenu2 { get; set; }
        public int? Seqno { get; set; }
        //public IList<TextValue>? SubMenuList { get; set; }
        public string? Update { get; set; }
        public string? View { get; set; }
        public string? ModelMode { get; set; }
        public IList<UserRightModel>? UserRights { get; set; }

    }
}