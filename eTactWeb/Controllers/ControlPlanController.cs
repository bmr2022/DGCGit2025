using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class ControlPlanController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IControlPlan _IControlPlan { get; }
        private readonly ILogger<ControlPlanController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ControlPlanController(ILogger<ControlPlanController> logger, IDataLogic iDataLogic, IControlPlan iControlPlan, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IControlPlan = iControlPlan;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ControlPlan(int AccountCode, int ID, string Mode, string Requisitionby, int CanSaleBillReqYearcode, string CanRequisitionNo, string CustomerName, int SaleBillEntryId, string SaleBillNo, int SaleBillYearCode, string SaleBillDate, int BillAmt, int INVNetAmt, string ReasonOfCancel, int Approvedby, string CC, int uid, string Canceled, string VoucherType, string ApprovalDate, string CancelDate, string MachineName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new ControlPlanModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            // MainModel.ActualEntryDate = HttpContext.Session.GetString("EntryDate");


            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                
                //MainModel = await _ICancelSaleBillrequisition.GetViewByID(CanRequisitionNo, CanSaleBillReqYearcode).ConfigureAwait(false);
                MainModel.Mode = Mode; 
               
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> ControlPlan(ControlPlanModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyControlPlanGrid");
                List<ControlPlanDetailModel> ControlPlanDetail = new List<ControlPlanDetailModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ControlPlanDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(modelJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(ControlPlanDetail);
                }
                else
                {
                    GIGrid = GetDetailTable(ControlPlanDetail);
                }
                var Result =  await _IControlPlan.SaveControlPlan(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyControlPlanGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(ControlPlan));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<ControlPlanController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<ControlPlanDetailModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("CntPlanEntryId", typeof(long));
                GIGrid.Columns.Add("CntPlanYearCode", typeof(long));
                GIGrid.Columns.Add("SeqNo", typeof(long));
                GIGrid.Columns.Add("Characteristic", typeof(string));
                GIGrid.Columns.Add("EvalutionMeasurementTechnique", typeof(string));
                GIGrid.Columns.Add("SpecificationFrom", typeof(string));
                GIGrid.Columns.Add("Operator", typeof(string));
                GIGrid.Columns.Add("SpecificationTo", typeof(string));
                GIGrid.Columns.Add("FrequencyOfTesting", typeof(string));
                GIGrid.Columns.Add("InspectionBy", typeof(string));
                GIGrid.Columns.Add("ControlMethod", typeof(string));
                GIGrid.Columns.Add("RejectionPlan", typeof(string));
                GIGrid.Columns.Add("Remarks", typeof(string));
                GIGrid.Columns.Add("ItemimagePath", typeof(string));
                GIGrid.Columns.Add("DrawingNo", typeof(string));
                GIGrid.Columns.Add("DrawingNoImagePath", typeof(string));

                foreach (var Item in DetailList)
                {
                    string evalTech = Item.EvalutionMeasurmentTechnique;
                    if (!string.IsNullOrWhiteSpace(evalTech) && evalTech.Length > 1)
                    {
                        evalTech = evalTech.Substring(0, 1); 
                    }
                    GIGrid.Rows.Add(
                        new object[]
                        {
                    Item.CntPlanEntryId,
                    Item.CntPlanYearCode,
                    Item.SeqNo,
                    Item.Characteristic,
                     evalTech, 
                    //Item.EvalutionMeasurmentTechnique,
                    Item.SpecificationFrom,
                    Item.Operator,
                    Item.SpecificationTo,
                    Item.FrequencyofTesting,
                    Item.InspectionBy,
                    Item.ControlMethod,
                    Item.RejectionPlan,
                    Item.Remarks,
                    Item.ItemimagePath,
                    Item.DrawingNo,
                    Item.DrawingNoImagePath,

                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> GetNewEntryId(int Yearcode)
        {
            var JSON = await _IControlPlan.GetNewEntryId( Yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetItemName()
        {
            var JSON = await _IControlPlan.GetItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetPartCode()
        {
            var JSON = await _IControlPlan.GetPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEvMeasureTech()
        {
            var JSON = await _IControlPlan.GetEvMeasureTech();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetCharacteristic()
        {
            var JSON = await _IControlPlan.GetCharacteristic();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddControlPlanDetail(ControlPlanDetailModel model)
        {
            try
            {
                string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
                IList<ControlPlanDetailModel> GridDetail = new List<ControlPlanDetailModel>();
                if (jsonString != null)
                {
                    GridDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
                }

                var MainModel = new ControlPlanModel();
                var RoutingDetailGrid = new List<ControlPlanDetailModel>();
                var RoutingGrid = new List<ControlPlanDetailModel>();
                var SSGrid = new List<ControlPlanDetailModel>();

                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        //model.SequenceNo = 1;
                        RoutingGrid.Add(model);
                    }
                    else
                    {
                        if (GridDetail.Any(x => x.SeqNo == model.SeqNo))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SeqNo = GridDetail.Count + 1;
                            RoutingGrid = GridDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(RoutingGrid);
                            RoutingGrid.Add(model);
                        }
                        //model.SeqNo = GridDetail.Count + 1;
                        //RoutingGrid = GridDetail.Where(x => x != null).ToList();
                        //SSGrid.AddRange(RoutingGrid);
                        //RoutingGrid.Add(model);
                    }
                    RoutingGrid = RoutingGrid.OrderBy(item => item.SeqNo).ToList();
                    MainModel.DTSSGrid = RoutingGrid;

                    HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(MainModel.DTSSGrid));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_ControlPlanMainGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new ControlPlanModel();
            string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
            IList<ControlPlanDetailModel> GridDetail = new List<ControlPlanDetailModel>();
            if (jsonString != null)
            {
                GridDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
            }
            var SAGrid = GridDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new ControlPlanModel();
            string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
            IList<ControlPlanDetailModel> ControlPlanDetail = new List<ControlPlanDetailModel>();
            if (jsonString != null)
            {
                ControlPlanDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (ControlPlanDetail != null && ControlPlanDetail.Count > 0)
            {
                ControlPlanDetail.RemoveAt(Convert.ToInt32(Indx));
                Indx = 0;

                foreach (var item in ControlPlanDetail)
                {
                    Indx++;
                    // item.SequenceNo = Indx;
                }
                MainModel.DTSSGrid = ControlPlanDetail;

                HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(MainModel.DTSSGrid));
            }
            return PartialView("_ControlPlanMainGrid", MainModel);
        }
    }
}
