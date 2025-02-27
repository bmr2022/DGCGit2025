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
        public DateTime? EntryDate {  get; set; }
        public int YearCode { get; set; }
        public string? ShiftCode { get; set; }

        public string? ShiftName { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? GraceTimeIn { get; set; }
        public string? GraceTimeOut { get; set; }
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
        public string? duration { get; set; }
        public string? ShiftForProdOnly { get; set; } //drop down -- yes No
        public string? OutDay { get; set; }//Same NextDay

        public string? ShiftTypeFixRotFlex { get; set; }
        public string? ApplicableToEmployeeCategory { get; set; }
        public string? AttandanceMode { get; set; }
        public string? MinimumHourRequiredForFullDay { get; set; }
    }
}
