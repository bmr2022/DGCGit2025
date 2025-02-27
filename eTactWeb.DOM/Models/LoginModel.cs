using System.ComponentModel.DataAnnotations;

namespace eTactWeb.DOM.Models
{
    public class LoginModel
    {
        public List<LoginModel>? AccList { get; set; }
        public string? CC { get; set; }
        public string? CompanyName { get; set; }
        public string? ItemCode { get; set; }

        public List<LoginModel>? ItemList { get; set; }

        public string? ItemName { get; set; }
        public string? ServerName { get; set; }

        public string? Password { get; set; }

        public List<LoginModel>? StoreLst { get; set; }

        [Display(Name = "Store")]
        public string? StoreName { get; set; }

        public string? Unit { get; set; }

        public string? UserName { get; set; }

        [Display(Name = "Year")]
        public int YearCode { get; set; }
        public string? Mode { get; set; } = "N";
    }
    public class NavigationData
    {
        public long Entry_id { get; set; }
        public long EmpID { get; set; }
        public string? EmpName { get; set; }
        public string? Module { get; set; }
        public string MainMenu { get; set; }
        //public string SubMenu { get; set; }
        public bool OptAll { get; set; }
        public bool OptSave { get; set; }
        public bool OptUpdate { get; set; }
        public bool OptDelete { get; set; }
        public bool OptView { get; set; }
    }

}