using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactwebHR.Controllers
{
    public class EmployeeAdvancePayementController : Controller
    {
        [Route("{controller}/Index")]
        public async Task<IActionResult> EmployeeAdvancePayment(int ID, int YearCode, string Mode)
        {
            HttpContext.Session.Remove("EmployeeAdvancePayement");
            var MainModel = new HRAdvanceModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel.Mode = Mode;
                MainModel.ID = ID;
               
                string serializedGrid = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("EmployeeAdvancePayement", serializedGrid);
            }
            

            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedOn = DateTime.Now;
            }

            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("EmployeeAdvancePayement", serializedGateAttendance);

            return View(MainModel);
        }
    }
}
