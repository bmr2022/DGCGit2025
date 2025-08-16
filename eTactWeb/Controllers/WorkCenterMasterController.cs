using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class WorkCenterMasterController : Controller
    {

        private readonly IDataLogic _IDataLogic;
        private readonly IWorkCenterMaster _IWorkCenterMaster;

        public WorkCenterMasterController(IDataLogic iDataLogic, IWorkCenterMaster iWorkCenterMaster)
        {
            _IDataLogic = iDataLogic;
            _IWorkCenterMaster = iWorkCenterMaster;
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IWorkCenterMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DashBoard()
        {
            var model = new WorkCenterMasterModel();
            model.Mode = "Dashboard";
            model = await _IWorkCenterMaster.GetDashboardData(model).ConfigureAwait(false);
            //model.MachineMasterList = model.MachineMasterList.DistinctBy(x => x.MachineId).ToList();

            return View(model);

        }

        public async Task<IActionResult> WorkCenterMaster(string mode, int id)
        {
            var model = new WorkCenterMasterModel();
            if (mode == "U" || mode == "V")
            {
                model.WCID = id;
                model = await _IWorkCenterMaster.ViewByID(id).ConfigureAwait(true);
                model.Mode = mode;
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> WorkCenterMaster(WorkCenterMasterModel model)
        {
            if (model.Mode == "U")
            {

                // Fetch the existing record from the database using EntryId or another unique identifier
                var existingRecord = await _IWorkCenterMaster.ViewByID(model.ID);
                if (existingRecord != null)
                {
                    //existingRecord.MachineId = model.MachineId;
                    existingRecord.WorkCenterCode = model.WorkCenterCode;
                    existingRecord.WorkCenterDescription = model.WorkCenterDescription;
                    existingRecord.WorkCenterType = model.WorkCenterType;
                    existingRecord.Mode = model.Mode;
                    //existingRecord.MachineId = model.MachineId;

                    // Save the updated record back to the database
                    var updateResult = await _IWorkCenterMaster.SaveData(existingRecord).ConfigureAwait(true);

                    if (updateResult.StatusText == "Success")
                    {
                        return RedirectToAction("Dashboard", "WorkCenterMaster");
                    }
                }
                else
                {

                    ModelState.AddModelError("", "Record not found for updating.");
                }
            }
            else
            {
                // Handle insert case if mode is not "U"
                var result = await _IWorkCenterMaster.SaveData(model).ConfigureAwait(true);
                if (result.StatusText == "Success")
                {
                    return RedirectToAction("Dashboard");
                }
            }

            // If something goes wrong, return the model back to the view
            return View(model);
        }

        public async Task<IActionResult> Delete(string WorkCenterDescription)
        {
            if (ModelState.IsValid)
            {
                var result = await _IWorkCenterMaster.DeleteMachine(WorkCenterDescription);
                if (result.StatusText == "Success")
                {
                    return RedirectToAction("Dashboard", "WorkCenterMaster");

                }

            }
            return RedirectToAction("Dashboard", "WorkCenterMaster");

        }

    }
}
