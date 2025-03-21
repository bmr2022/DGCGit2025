using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRShiftMasterModel: TimeStamp
    {
        public int ShiftId {  get; set; }
        public string? EntryDate {  get; set; }
        public int YearCode { get; set; }
        public string? ShiftCode { get; set; }

        public string? ShiftName { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public float? GraceTimeIn { get; set; }
        public float? GraceTimeOut { get; set; }
        public string? Lunchfrom { get; set; }
        public int? EntryByEmpId { get; set; }
        public int? UpdatedById { get; set; }
        public string? EntryByEmpName { get; set; }
        public string? UpdatedByEmpName { get; set; }



        public string? Lunchto { get; set; }
        public string? FirstBreakFrom { get; set; }
        public string? FirstBreakTo { get; set; }//to should be more then from 
        public string? SecondBreakFrom { get; set; }
        public string? SecondBreakTo { get; set; }// it should be more then from 
        public string? CC { get; set; }
        public int UID { get; set; }
        public string? ThirdBreakFrom { get; set; }
        public string? ThirdBreakTo { get; set; }
        public float? duration { get; set; }
        public string? ShiftForProdOnly { get; set; } //drop down -- yes No
        public string? OutDay { get; set; }//Same NextDay

        public string? ShiftTypeFixRotFlex { get; set; }
        public string? ApplicableToEmployeeCategory { get; set; }
       // public IList<string>? ApplicableToEmployeeCategory { get; set; }
        public IList<TextValue>? EmpCategList { get; set; }
        public string? AttandanceMode { get; set; }
        public int? MinimumHourRequiredForFullDay { get; set; }

        //DashBoard
        public string? Searchbox { get; set; }
        public IList<HRShiftMasterModel> HRShiftMasterDashBoardGrid { get; set; }

    }
}
