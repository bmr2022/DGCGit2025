using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Globalization;
using System.Data;
using System.Configuration;
using static eTactWeb.Data.Common.CommonFunc;
using DocumentFormat.OpenXml.EMMA;

namespace eTactweb.Controllers
{
    public class PPCToolIssueController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReqWithoutBOM _IReqWithoutBOM;
        private readonly IPPCToolIssue _IPPCToolIssue;
        private readonly ILogger<PPCToolIssueController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;

        public PPCToolIssueController(ILogger<PPCToolIssueController> logger, IDataLogic iDataLogic, IPPCToolIssue iPPCToolIssue, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPPCToolIssue = iPPCToolIssue;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
        }
        [HttpGet]
        public IActionResult PPCToolIssue()
        {
            HttpContext.Session.Remove("KeyToolIssueGrid");

            var model = new PPCToolIssueMainModel
            {
                ToolIssueDetails = new List<PPCToolIssueDetailModel>()
            };

            
            model.ToolIssueYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            // ✅ Same style default values (matching your format)
            model.ToolIssueDate = DateTime.Now;
            model.ToolIssueEntryDate = DateTime.Now;
            model.ActualEntryDate = DateTime.Now;
            model.LastUpdatedDate = DateTime.Now;

            model.EntryByMachine = Environment.MachineName;
            model.ActualEntryBy = Convert.ToInt64(HttpContext.Session.GetString("UID"));
            model.LastUpdatedBy = Convert.ToInt64(HttpContext.Session.GetString("UID"));
            model.Mode = "INSERT";

            string serializedGrid = JsonConvert.SerializeObject(model.ToolIssueDetails);
            HttpContext.Session.SetString("KeyToolIssueGrid", serializedGrid);

            return View(model);
        }




        [HttpGet]
        public async Task<JsonResult> GetNewEntry()
        {
            int yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var result = await _IPPCToolIssue.GetNewEntry("NEWENTRY", yearCode);
            string JsonString = JsonConvert.SerializeObject(result);
            // Directly return the object, no need to serialize manually
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<JsonResult> FillDepartmentList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillDepartmentList("FillDepartmentList");
                string jsonString = JsonConvert.SerializeObject(result);
                return Json(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool list.");
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<JsonResult> FillToolList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolList("FillToolList");
                string jsonString = JsonConvert.SerializeObject(result);
                return Json(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool list.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillToolBarCode(long ToolEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolBarCode("FillToolBarCode", ToolEntryId);
                // Manually serialize the result
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool barcode.");
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> FillToolSerialNo(long ToolEntryId, string Barcode)
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolSerialNo("FillToolSerialNo", ToolEntryId, Barcode);
                // Manually serialize the result
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool serial no.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillToolBarCodeDetail(long ToolEntryId, string Barcode, string SerialNo)
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolBarCodeDetail("FillToolBarCodeDetail", ToolEntryId, Barcode, SerialNo);
                // Manually serialize the result
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool barcode details.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlan(long ToolEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlan("FillProdPlan", ToolEntryId);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanYearCode(long ToolEntryId, string ProdPlanNo)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanYearCode("FillProdPlanYearCode", ToolEntryId, ProdPlanNo);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan year code.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanDate(long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanDate("FillProdPlanDate", ToolEntryId, ProdPlanNo, ProdPlanYearCode);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan year code.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanSchedule(long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanSchedule("FillProdPlanSchedule", ToolEntryId, ProdPlanNo, ProdPlanYearCode, PlanNoEntryId);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan schedule.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanScheduleYearCode(long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanScheduleYearCode("FillProdPlanScheduleYearCode", ToolEntryId, ProdPlanNo, ProdPlanYearCode, ProdSchNo);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan schedule Yearcode.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanScheduleDetail(long ToolEntryId, string ProdPlanNo, long ProdPlanYearCode, string ProdSchNo, long ProdSchYearCode)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanScheduleDetail("FillProdPlanScheduleDetail", ToolEntryId, ProdPlanNo, ProdPlanYearCode, ProdSchNo,ProdSchYearCode);
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan schedule Detail.");
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<JsonResult> FillMachineList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillMachineList("FillMachineList");
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling machine list.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillIssuedByEmpList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillIssuedByEmpList("FillIssuedByEmpList");
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling EmpList.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertToolIssue(PPCToolIssueMainModel model)
        {
            try
            {
                // 1️⃣ Get JSON grid from session
                string gridJson = HttpContext.Session.GetString("KeyToolIssueGrid");
                List<PPCToolIssueDetailModel> toolDetails = new();

                if (!string.IsNullOrEmpty(gridJson))
                    toolDetails = JsonConvert.DeserializeObject<List<PPCToolIssueDetailModel>>(gridJson);

                if (toolDetails == null || toolDetails.Count == 0)
                {
                    ModelState.AddModelError("ToolIssueDetails", "Tool Issue Grid should have at least one record!");
                    return View("PPCToolIssue", model);
                }

                // 2️⃣ Prepare metadata
                model.CC = HttpContext.Session.GetString("Branch") ?? "";
                model.UID = Convert.ToInt64(HttpContext.Session.GetString("UID") ?? "0");
                model.EntryByMachine = Environment.MachineName;
                model.ActualEntryBy = Convert.ToInt64(HttpContext.Session.GetString("UID") ?? "0");
                model.LastUpdatedBy = Convert.ToInt64(HttpContext.Session.GetString("UID") ?? "0");
                model.ToolIssueYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode") ?? "0");

                // 3️⃣ Default nullable dates if empty
                model.ToolIssueDate ??= DateTime.Now;
                model.ToolIssueEntryDate ??= DateTime.Now;
                model.ActualEntryDate ??= DateTime.Now;
                model.LastUpdatedDate ??= DateTime.Now;

                // 4️⃣ Convert child list to DataTable
                DataTable dtTool = GetToolIssueDetailTable(toolDetails);

                // 5️⃣ Call DAL
                var result = await _IPPCToolIssue.InsertToolIssue(model, dtTool);

                // 6️⃣ Handle response
                if (result != null && string.IsNullOrWhiteSpace(result.Message))
                {
                    TempData["200"] = "Data Saved Successfully!";
                    HttpContext.Session.Remove("KeyToolIssueGrid");
                }
                else
                {
                    TempData["500"] = result?.Message ?? "Error while saving data!";
                }
                return RedirectToAction("PPCToolIssue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in InsertToolIssue");
                TempData["500"] = "Unexpected Error!";
                return View("Error");
            }
        }

        // Convert ToolIssueDetail list to DataTable for TVP
        private static DataTable GetToolIssueDetailTable(IList<PPCToolIssueDetailModel> list)
        {
            var dt = new DataTable();

            // --- Columns matching Type_PPCToolIssueDetail ---
            dt.Columns.Add("SeqNo", typeof(long));
            dt.Columns.Add("ToolIssueEntryId", typeof(long));
            dt.Columns.Add("ToolIssueYearCode", typeof(long));
            dt.Columns.Add("ToolEntryId", typeof(long));
            dt.Columns.Add("ToolYearCode", typeof(long));
            dt.Columns.Add("BarCode", typeof(string));
            dt.Columns.Add("SerialNo", typeof(string));
            dt.Columns.Add("ToolType", typeof(string));
            dt.Columns.Add("IssueQty", typeof(long));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("CurrentLocation", typeof(string));
            dt.Columns.Add("IssueToLocation", typeof(string));
            dt.Columns.Add("ToolCondition", typeof(string));
            dt.Columns.Add("LastCalibrationDate", typeof(DateTime));
            dt.Columns.Add("NextCalibrationDueDate", typeof(DateTime));
            dt.Columns.Add("RemainingToolLifeInMonths", typeof(long));
            dt.Columns.Add("ProdPlanEntryId", typeof(long));
            dt.Columns.Add("ProdPlanNo", typeof(string));
            dt.Columns.Add("ProdPlandate", typeof(DateTime));
            dt.Columns.Add("ProdPlanYearCode", typeof(long));
            dt.Columns.Add("ProdSchEntryId", typeof(long));
            dt.Columns.Add("ProdSchno", typeof(string));
            dt.Columns.Add("ProdSchDate", typeof(DateTime));
            dt.Columns.Add("ProdSchYearcode", typeof(long));
            dt.Columns.Add("ForMachineId", typeof(long));
            dt.Columns.Add("Specialinstruction", typeof(string));
            dt.Columns.Add("WillBeConsumedOrReturned", typeof(string));
            dt.Columns.Add("PendingStatus", typeof(string));
            dt.Columns.Add("PendingQty", typeof(long));
           

            foreach (var item in list)
            {
                dt.Rows.Add(
                    item.SeqNo,
                    item.ToolIssueEntryId,
                    item.ToolIssueYearCode,
                    item.ToolEntryId,
                    item.ToolYearCode,
                    string.IsNullOrWhiteSpace(item.BarCode) ? "" : item.BarCode,
                    string.IsNullOrWhiteSpace(item.SerialNo) ? "" : item.SerialNo,
                    string.IsNullOrWhiteSpace(item.ToolType) ? "" : item.ToolType,
                    item.IssueQty ?? 0,
                    string.IsNullOrWhiteSpace(item.Unit) ? "" : item.Unit,
                    string.IsNullOrWhiteSpace(item.CurrentLocation) ? "" : item.CurrentLocation,
                    string.IsNullOrWhiteSpace(item.IssueToLocation) ? "" : item.IssueToLocation,
                    string.IsNullOrWhiteSpace(item.ToolCondition) ? "" : item.ToolCondition, // NOT NULL string
                    item.LastCalibrationDate.HasValue ? (object)item.LastCalibrationDate.Value : DBNull.Value, // NULL allowed
                    item.NextCalibrationDueDate.HasValue ? (object)item.NextCalibrationDueDate.Value : DBNull.Value, // NULL allowed
                    item.RemainingToolLifeInMonths ?? 0,
                    item.ProdPlanEntryId ?? 0,
                    string.IsNullOrWhiteSpace(item.ProdPlanNo) ? "" : item.ProdPlanNo,
                    item.ProdPlandate ?? DateTime.Now,
                    item.ProdPlanYearCode ?? 0,
                    item.ProdSchEntryId ?? 0, // NOT NULL bigint, pass 0 if null
                    string.IsNullOrWhiteSpace(item.ProdSchNo) ? "" : item.ProdSchNo,
                    item.ProdSchDate ?? DateTime.Now,
                    item.ProdSchYearCode ?? 0,
                    item.ForMachineId ?? 0,
                    string.IsNullOrWhiteSpace(item.SpecialInstruction) ? "" : item.SpecialInstruction,
                    string.IsNullOrWhiteSpace(item.WillBeConsumedOrReturned) ? "" : item.WillBeConsumedOrReturned,
                    item.PendingStatus ?? "Pen",
                    item.PendingQty.HasValue ? item.PendingQty.Value : 0

                );
            }

            return dt;
        }
        [HttpPost]
        public IActionResult AddToolIssueDetail(PPCToolIssueDetailModel model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyToolIssueGrid");
                List<PPCToolIssueDetailModel> ToolGrid = string.IsNullOrEmpty(modelJson)
                    ? new List<PPCToolIssueDetailModel>()
                    : JsonConvert.DeserializeObject<List<PPCToolIssueDetailModel>>(modelJson);

                if (model.SeqNo > 0) // Update existing row
                {
                    var row = ToolGrid.FirstOrDefault(x => x.SeqNo == model.SeqNo);
                    if (row != null)
                    {
                        // Update all fields
                        row.ToolIssueEntryId = model.ToolIssueEntryId;
                        row.ToolEntryId = model.ToolEntryId;
                        row.ToolName = model.ToolName;
                        row.ToolYearCode = model.ToolYearCode;
                        row.BarCode = model.BarCode;
                        row.SerialNo = model.SerialNo;
                        row.ToolType = model.ToolType;
                        row.IssueQty = model.IssueQty;
                        row.Unit = model.Unit;
                        row.CurrentLocation = model.CurrentLocation;
                        row.IssueToLocation = model.IssueToLocation;
                        row.ToolCondition = model.ToolCondition;
                        row.LastCalibrationDate = model.LastCalibrationDate;
                        row.NextCalibrationDueDate = model.NextCalibrationDueDate;
                        row.RemainingToolLifeInMonths = model.RemainingToolLifeInMonths;
                        row.ProdPlanEntryId = model.ProdPlanEntryId;
                        row.ProdPlanNo = model.ProdPlanNo;
                        row.ProdPlandate = model.ProdPlandate;
                        row.ProdPlanYearCode = model.ProdPlanYearCode;
                        row.ProdSchEntryId = model.ProdSchEntryId;
                        row.ProdSchNo = model.ProdSchNo;
                        row.ProdSchDate = model.ProdSchDate;
                        row.ProdSchYearCode = model.ProdSchYearCode;
                        row.ForMachineId = model.ForMachineId;
                        row.SpecialInstruction = model.SpecialInstruction;
                        row.WillBeConsumedOrReturned = model.WillBeConsumedOrReturned;
                        row.PendingQty = model.PendingQty;
                        row.PendingStatus = model.PendingStatus;
                    }
                }
                else // Add new row
                {
                    // ✅ Assign a unique SeqNo
                    long maxSeq = ToolGrid.Any() ? (ToolGrid.Max(x => x.SeqNo) ?? 0) : 0;
                    model.SeqNo = maxSeq + 1;
                    ToolGrid.Add(model);
                }

                HttpContext.Session.SetString("KeyToolIssueGrid", JsonConvert.SerializeObject(ToolGrid));

                return PartialView("_PPCToolIssueGrid", ToolGrid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult DeleteToolIssueRow(int SeqNo)
        {
            var MainModel = new PPCToolIssueMainModel();

            // ✅ Load existing session grid
            string modelJson = HttpContext.Session.GetString("KeyToolIssueGrid");
            List<PPCToolIssueDetailModel> Grid = new List<PPCToolIssueDetailModel>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                Grid = JsonConvert.DeserializeObject<List<PPCToolIssueDetailModel>>(modelJson);
            }

            if (Grid != null && Grid.Count > 0)
            {
                int index = SeqNo - 1;

                // ✅ Remove row
                if (index >= 0 && index < Grid.Count)
                {
                    Grid.RemoveAt(index);
                }

                // ✅ Re-assign SeqNo again
                int newSeq = 1;
                foreach (var item in Grid)
                {
                    item.SeqNo = newSeq++;
                }

                // ✅ Assign to main model
                MainModel.ToolIssueDetails = Grid;

                // ✅ Save back to session
                string serializedGrid = JsonConvert.SerializeObject(Grid);
                HttpContext.Session.SetString("KeyToolIssueGrid", serializedGrid);
            }

            // ✅ Return updated grid
            return PartialView("_PPCToolIssueGrid", Grid);
        }

        public IActionResult EditToolIssueRow(int SeqNo)
        {
            // ✅ Load session grid
            string modelJson = HttpContext.Session.GetString("KeyToolIssueGrid");
            List<PPCToolIssueDetailModel> Grid = new List<PPCToolIssueDetailModel>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                Grid = JsonConvert.DeserializeObject<List<PPCToolIssueDetailModel>>(modelJson);
            }

            // ✅ Find row using SeqNo
            var row = Grid.FirstOrDefault(x => x.SeqNo == SeqNo);

            // ✅ Return JSON so frontend can fill controls
            string json = JsonConvert.SerializeObject(row);
            return Json(json);
        }

    }
}