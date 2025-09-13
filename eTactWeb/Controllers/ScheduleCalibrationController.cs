using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class ScheduleCalibrationController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IScheduleCalibration _IScheduleCalibration { get; }
        private readonly ILogger<ScheduleCalibrationController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ScheduleCalibrationController(ILogger<ScheduleCalibrationController> logger, IDataLogic iDataLogic, IScheduleCalibration iScheduleCalibration, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IScheduleCalibration = iScheduleCalibration;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> ScheduleCalibration(int ID, int YC, string Mode)
        {
            var MainModel = new ScheduleCalibrationModel();

            MainModel.CalibSchYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
          
            HttpContext.Session.Remove("KeyScheduleCalibration");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                //MainModel = await _IMaterialConversion.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
               // MainModel.Mode = Mode; 
                //MainModel.EntryId = ID;
              
                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                    MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.CC = HttpContext.Session.GetString("Branch");
                   
                }

                string serializedGrid = JsonConvert.SerializeObject(MainModel.CalibrationScheduleGrid);
                HttpContext.Session.SetString("KeyScheduleCalibration", serializedGrid);
            }

            return View(MainModel);
        }
    }
}
