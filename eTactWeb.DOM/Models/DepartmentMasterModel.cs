using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class DepartmentMasterModel: TimeStamp
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string CC { get; set; }
        public string Entry_Date { get; set; }
        public string DeptType { get; set; }
        public string departmentcode { get; set; }

        public string Searchbox { get; set; }
        public IList<DepartmentMasterModel> DepartmentMasterDashBoardGrid { get; set; }
    }
}
