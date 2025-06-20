using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
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
        public async Task<ActionResult> ControlPlan(int ID, string Mode, int YC,string FromDate,string ToDate, string EntryDate, string ItemName, string PartCode,
            int SeqNo, string Characteristic, string EvalutionMeasurmentTechnique, string SpecificationFrom, string Operator,
            string SpecificationTo, string FrequencyofTesting, string InspectionBy, string ControlMethod, string RejectionPlan,
            string Remarks, string ItemimagePath, string DrawingNo, string DrawingNoImagePath,int ItemCode,string RevNo, bool isFromPartCode = false
)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            bool IsFromItemCode = false;
            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new ControlPlanModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.CntPlanEntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.CntPlanYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");

            HttpContext.Session.Remove("KeyControlPlanGrid");

            if (isFromPartCode && ItemCode > 0)
            {
                MainModel = await _IControlPlan.GetByItemOrPartCode(ItemCode);

                if (MainModel != null && MainModel.CntPlanEntryId > 0)
                {
                    if (ID != MainModel.CntPlanEntryId || (Mode == "U" || Mode == "V"))
                    {
                        return RedirectToAction("ControlPlan", new
                        {
                            ID = MainModel.CntPlanEntryId,
                            YC = MainModel.Yearcode,
                            Mode = "U",
                            EntryDate = MainModel.CntPlanEntryDate,
                            ItemCode = ItemCode,
                            ItemName = ItemName,
                            PartCode = PartCode,
                            RevNo = MainModel.RevNo,
                            isFromPartCode = false
                        });
                    }
                }
                else
                {
                    MainModel.ItemCode = ItemCode;
                    MainModel.PartCode = PartCode;
                    MainModel.ItemName = ItemName;
                    MainModel.Yearcode = YC;
                    MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                    MainModel.CntPlanEntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                    MainModel.CntPlanYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    //MainModel.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    MainModel.CC = HttpContext.Session.GetString("Branch");
                    MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.ActualEntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                    MainModel.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                    MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                   
                }
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                
                
                MainModel.Mode = Mode;
                MainModel = await _IControlPlan.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.CntPlanEntryId = ID;
                MainModel.CntPlanYearCode = YC;
                MainModel.CntPlanEntryDate = EntryDate;
                MainModel.PartCode = PartCode;
                MainModel.ItemName = ItemName;
                 RevNo=MainModel.RevNo;



                //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                //    SlidingExpiration = TimeSpan.FromMinutes(55),
                //    Size = 1024
                //};

                string serializedGrid = JsonConvert.SerializeObject(MainModel.DTSSGrid);
                HttpContext.Session.SetString("KeyControlPlanGrid", serializedGrid);


                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                }
            }
            
            return View(MainModel); 
        }
        [Route("{controller}/Index")]
        [HttpPost]

        public async Task<IActionResult> ControlPlan(ControlPlanModel model)
        {
            try
            {
            
                // Get session-stored detail grid
                string modelJson = HttpContext.Session.GetString("KeyControlPlanGrid");
                List<ControlPlanDetailModel> ControlPlanDetail = new List<ControlPlanDetailModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ControlPlanDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(modelJson);
                }
                if (model.Mode == "U" && ControlPlanDetail.Count > 0)
                {
                    model.ImageURL ??= ControlPlanDetail[0].ImageURL;
                    model.ItemImageURL ??= ControlPlanDetail[0].ItemImageURL;
                }
                // Handle Drawing Image Upload
                if (model.UploadImage != null && model.UploadImage.Length > 0)
                {
                    var fileName = Path.GetFileName(model.UploadImage.FileName);
                    var uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    var drawingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/DrawingImage");
                    Directory.CreateDirectory(drawingPath); // Ensure directory exists
                    var drawingFilePath = Path.Combine(drawingPath, uniqueName);

                    using (var stream = new FileStream(drawingFilePath, FileMode.Create))
                    {
                        await model.UploadImage.CopyToAsync(stream);
                    }

                    model.ImageURL = "/Uploads/DrawingImage/" + uniqueName;
                }

                // Handle Item Image Upload
                if (model.ItemImage != null && model.ItemImage.Length > 0)
                {
                    var fileName = Path.GetFileName(model.ItemImage.FileName);
                    var uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    var itemPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/ItemImage");
                    Directory.CreateDirectory(itemPath); // Ensure directory exists
                    var itemFilePath = Path.Combine(itemPath, uniqueName);

                    using (var stream = new FileStream(itemFilePath, FileMode.Create))
                    {
                        await model.ItemImage.CopyToAsync(stream);
                    }

                    model.ItemImageURL = "/Uploads/ItemImage/" + uniqueName;
                }
                // Update each detail row with new image URLs
                for (int i = 0; i < ControlPlanDetail.Count; i++)
                {
                    ControlPlanDetail[i].ImageURL = model.ImageURL;
                    ControlPlanDetail[i].ItemImageURL = model.ItemImageURL;
                }

                // Store updated detail back in session
                HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(ControlPlanDetail));

                var GIGrid = GetDetailTable(ControlPlanDetail);

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

                var Result = await _IControlPlan.SaveControlPlan(model, GIGrid);

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

                return RedirectToAction(nameof(ControlPlanDashBoard));
            }
            catch (Exception ex)
            {
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
                //GIGrid.Columns.Add("ItemimagePath", typeof(string));
                //GIGrid.Columns.Add("DrawingNo", typeof(string));
                //GIGrid.Columns.Add("DrawingNoImagePath", typeof(string));

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
Item.Characteristic ?? "",
evalTech ?? "",
Item.SpecificationFrom ?? "",
Item.Operator ?? "",
Item.SpecificationTo ?? "",
Item.FrequencyofTesting ?? "",
Item.InspectionBy ?? "",
Item.ControlMethod ?? "",
Item.RejectionPlan ?? "",
Item.Remarks ?? ""
//Item.ItemImageURL ?? "",
//Item.DrawingNo ?? "",
//Item.ImageURL ?? ""

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
        [HttpPost]
        public IActionResult ImportControlPlanDetails([FromBody] List<ControlPlanDetailModel> model)
        {
            try
            {
                string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
                IList<ControlPlanDetailModel> GridDetail = new List<ControlPlanDetailModel>();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
                }

                int currentMaxSeq = GridDetail.Count > 0 ? GridDetail.Max(x => x.SeqNo) : 0;

                foreach (var list in model)
                {
                    // Skip duplicates
                    bool isDuplicate = GridDetail.Any(x =>
                        x.Characteristic == list.Characteristic &&
                        x.EvalutionMeasurmentTechnique == list.EvalutionMeasurmentTechnique &&
                        x.ControlMethod == list.ControlMethod);

                    if (isDuplicate)
                        continue;

                    currentMaxSeq++;
                    list.SeqNo = currentMaxSeq;
                    GridDetail.Add(list);
                }

                var MainModel = new ControlPlanModel
                {
                    DTSSGrid = GridDetail.OrderBy(x => x.SeqNo).ToList()
                };

                HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(MainModel.DTSSGrid));
                return PartialView("_ControlPlanMainGrid", MainModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error importing data: " + ex.Message);
            }
        }

        public IActionResult AddControlPlanDetail(ControlPlanDetailModel model)
        {
            try
            {
                string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
                IList<ControlPlanDetailModel> GridDetail = new List<ControlPlanDetailModel>();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
                }

                var MainModel = new ControlPlanModel();

                if (model != null)
                {
                    // Check for duplicates except for the same SeqNo
                    bool isDuplicate = GridDetail.Any(x =>
                        x.SeqNo != model.SeqNo &&
                        x.Characteristic == model.Characteristic &&
                        x.EvalutionMeasurmentTechnique == model.EvalutionMeasurmentTechnique &&
                        x.ControlMethod == model.ControlMethod);

                    if (isDuplicate)
                        return StatusCode(207, "Duplicate");

                    if (model.Mode == "U")
                    {
                       
                        GridDetail.Add(model);
                    }
                    else
                    {
                      
                        int nextSeqNo = GridDetail.Count > 0 ? GridDetail.Max(x => x.SeqNo) + 1 : 1;
                        model.SeqNo = nextSeqNo;
                        if (model.SeqNo <= 0)
                        {
                            int nextSeqNo1 = GridDetail.Count > 0 ? GridDetail.Max(x => x.SeqNo) + 1 : 1;
                            model.SeqNo = nextSeqNo1;
                        }
                        GridDetail.Add(model);
                    }

                   
                    MainModel.DTSSGrid = GridDetail.OrderBy(x => x.SeqNo).ToList();
                    HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(MainModel.DTSSGrid));
                }

                return PartialView("_ControlPlanMainGrid", MainModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new ControlPlanModel();
            string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
            IList<ControlPlanDetailModel> GridDetail = new List<ControlPlanDetailModel>();

            if (!string.IsNullOrEmpty(jsonString))
            {
                GridDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
            }

            var result = GridDetail.Where(x => x.SeqNo == SeqNo).ToList();
            string JsonString = JsonConvert.SerializeObject(result);
            return Json(JsonString);
        }


        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new ControlPlanModel();
            string jsonString = HttpContext.Session.GetString("KeyControlPlanGrid");
            IList<ControlPlanDetailModel> ControlPlanDetail = new List<ControlPlanDetailModel>();

            if (!string.IsNullOrEmpty(jsonString))
            {
                ControlPlanDetail = JsonConvert.DeserializeObject<List<ControlPlanDetailModel>>(jsonString);
            }

            if (ControlPlanDetail != null && ControlPlanDetail.Count > 0)
            {
                var itemToRemove = ControlPlanDetail.FirstOrDefault(x => x.SeqNo == SeqNo);
                if (itemToRemove != null)
                    ControlPlanDetail.Remove(itemToRemove);

                MainModel.DTSSGrid = ControlPlanDetail.OrderBy(x => x.SeqNo).ToList();
                HttpContext.Session.SetString("KeyControlPlanGrid", JsonConvert.SerializeObject(MainModel.DTSSGrid));
            }

            return PartialView("_ControlPlanMainGrid", MainModel);
        }

        public async Task<IActionResult> ControlPlanDashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new ControlPlanModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            Dictionary<int, string> monthNames = new Dictionary<int, string>
            {
                {1, "Jan"}, {2, "Feb"}, {3, "Mar"}, {4, "Apr"}, {5, "May"}, {6, "Jun"},
                {7, "Jul"}, {8, "Aug"}, {9, "Sep"}, {10, "Oct"}, {11, "Nov"}, {12, "Dec"}
            };

            model.FromDate = $"{firstDayOfMonth.Day}/{monthNames[firstDayOfMonth.Month]}/{firstDayOfMonth.Year}";
            model.ToDate = $"{now.Day}/{monthNames[now.Month]}/{now.Year}";
            //DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            //model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            //model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ReportType = "SUMMARY";
            var Result = await _IControlPlan.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.DTSSGrid = CommonFunc.DataTableToList<ControlPlanDetailModel>(dt, "ControlPlanDashBoard");
                }
                
            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        {
            //model.Mode = "Search";
            var model = new ControlPlanModel();
            model = await _IControlPlan.GetDashboardDetailData(FromDate, ToDate, ReportType);
            
            return PartialView("_ControlPlanDashBoardGrid", model);
           
        }
        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var Result = await _IControlPlan.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("ControlPlanDashBoard");

        }
    }
}
