using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Globalization;
using System.Data;
namespace eTactWeb.Controllers
{
    public class RoutingController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IRouting _IRouting { get; }
        private readonly ILogger<RoutingController> _logger;
        public IWebHostEnvironment IWebHostEnvironment { get; }

        public RoutingController(ILogger<RoutingController> logger, IDataLogic iDataLogic, IRouting iRouting, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IRouting = iRouting;
            IWebHostEnvironment = iWebHostEnvironment;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> Routing()
        {
            //RoutingModel model = new RoutingModel();
            ViewData["Title"] = "Inventory Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyRoutingGrid");
            var MainModel = new RoutingModel();
            if (MainModel.Mode != "U")
            {
                MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            }
            var model = await BindModel(MainModel);
            model.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.SetString("KeyRoutingGrid", JsonConvert.SerializeObject(model));
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> Routing(RoutingModel model)
        {
            try
            {
                var RoutingGrid = new DataTable();
                string jsonString = HttpContext.Session.GetString("KeyRoutingGrid");
                IList<RoutingDetail> RoutingDetailGrid = new List<RoutingDetail>();
                if (jsonString != null)
                {
                    RoutingDetailGrid = JsonConvert.DeserializeObject<List<RoutingDetail>>(jsonString);
                }
                if (RoutingDetailGrid == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("RoutingGridDetail", "Routing Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("Routing", model);
                }
                else
                {
                    if (model.CC == null)
                    {
                        model.CC = HttpContext.Session.GetString("Branch");
                    }
                    //model.EntryId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
                    if (model.Mode == "U")
                    {
                        model.LastUpdateByEmpName = HttpContext.Session.GetString("EmpName");
                        model.LastUpdateBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    }
                    RoutingGrid = GetDetailTable(RoutingDetailGrid);
                    var Result = await _IRouting.SaveRouting(model, RoutingGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********* LogError ********* \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                    var model1 = new RoutingModel();
                    return RedirectToAction(nameof(Routing));
                }
            }
            catch (Exception ex)
            {
                LogException<RoutingController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        public async Task<JsonResult> GetAllDataItemWise(int ItemCode)
        {
            var model = new RoutingModel();
            model = await _IRouting.GetAllDataItemWise("GetAllDataItemWise", ItemCode);
            return Json(model.EntryId);
        }


        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new RoutingModel();
            string jsonString = HttpContext.Session.GetString("KeyRoutingGrid");
            IList<RoutingDetail> GridDetail = new List<RoutingDetail>();
            if (jsonString != null)
            {
                GridDetail = JsonConvert.DeserializeObject<List<RoutingDetail>>(jsonString);
            }
            var SAGrid = GridDetail.Where(x => x.SequenceNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetNewEntryId()
        {
            var JSON = await _IRouting.GetNewEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private async Task<RoutingModel> BindModel(RoutingModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IRouting.BindAllDropDowns("BINDALLDROPDOWN");
            model.StageList = new List<TextValue>();
            model.MachineList = new List<TextValue>();
            model.BranchList = new List<TextValue>();
            model.EmpList = new List<TextValue>();
            model.WorkCenterList = new List<TextValue>();
            model.TransfertoWCList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Com_Name"].ToString(),
                        Text = row["Com_Name"].ToString()
                    });
                }
                model.BranchList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[1].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["StageDescription"].ToString()
                    });
                }
                model.StageList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()
                    });
                }
                model.EmpList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["WorkCenterDescription"].ToString()
                    });
                }
                model.WorkCenterList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["WorkCenterDescription"].ToString()
                    });
                }
                model.TransfertoWCList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryId"].ToString(),
                        Text = row["MachGroup"].ToString()
                    });
                }
                model.MachineList = _List;
            }
            return model;
        }

        public async Task<JsonResult> FillItems()
        {
            var JSON = await _IRouting.FillItems("FillItems");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IRouting.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AlreadyExistItems()
        {
            var JSON = await _IRouting.AlreadyExistItems("UniqueItems");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubItems()
        {
            var JSON = await _IRouting.FillSubItems("FillSubItems");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //public async Task<JsonResult> GetAllDataItemWise(int ItemCode)
        //{
        //    var JSON = await _IRouting.GetAllDataItemWise("GetAllDataItemWise",ItemCode);
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}

        public IActionResult AddRoutingDetail(RoutingDetail model)
        {
            try
            {
                string jsonString = HttpContext.Session.GetString("KeyRoutingGrid");
                IList<RoutingDetail> GridDetail = new List<RoutingDetail>();
                if (jsonString != null)
                {
                    GridDetail = JsonConvert.DeserializeObject<List<RoutingDetail>>(jsonString);
                }

                var MainModel = new RoutingModel();
                var RoutingDetailGrid = new List<RoutingDetail>();
                var RoutingGrid = new List<RoutingDetail>();
                var SSGrid = new List<RoutingDetail>();

                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        //model.SequenceNo = 1;
                        RoutingGrid.Add(model);
                    }
                    else
                    {
                        if (GridDetail.Where(x => x.StageID == model.StageID && x.SubItemCode == model.SubItemCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            // model.SequenceNo = GridDetail.Count + 1;
                            RoutingGrid = GridDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(RoutingGrid);
                            RoutingGrid.Add(model);
                        }
                    }
                    RoutingGrid = RoutingGrid.OrderBy(item => item.SequenceNo).ToList();
                    MainModel.RoutingDetailGrid = RoutingGrid;

                    HttpContext.Session.SetString("KeyRoutingGrid", JsonConvert.SerializeObject(MainModel.RoutingDetailGrid));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_RoutingGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new RoutingModel();
            string jsonString = HttpContext.Session.GetString("KeyRoutingGrid");
            IList<RoutingDetail> RoutingDetail = new List<RoutingDetail>();
            if (jsonString != null)
            {
                RoutingDetail = JsonConvert.DeserializeObject<List<RoutingDetail>>(jsonString);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (RoutingDetail != null && RoutingDetail.Count > 0)
            {
                RoutingDetail.RemoveAt(Convert.ToInt32(Indx));
                Indx = 0;

                foreach (var item in RoutingDetail)
                {
                    Indx++;
                    // item.SequenceNo = Indx;
                }
                MainModel.RoutingDetailGrid = RoutingDetail;

               HttpContext.Session.SetString("KeyRoutingGrid", JsonConvert.SerializeObject(MainModel.RoutingDetailGrid));
            }
            return PartialView("_RoutingGrid", MainModel);
        }


        public async Task<IActionResult> RoutingDashboard(string ToDate, string SummaryDetail, string Flag = "True", string PartCode = "", string ItemName = "", string Stage = "", string WorkCenter = "", string FromDate = "04/01/2020")
        {
            try
            {
                HttpContext.Session.Remove("KeyRoutingGrid");
                var model = new RoutingMainDashboard();
                //model.FromDate = HttpContext.Session.GetString("FromDate");
                model.FromDate = FromDate;
                model.ToDate = DateTime.Today.ToString();
                // model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IRouting.GetDashboardData(model.FromDate, model.ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "EntryDate", "ItemCode", "PartCode",
                            "ItemName", "StageID", "StageDescription", "MachineGroupID", "MachGroup", "WorkCenterID",
                            "WorkCenterDescription", "TransferToWCID", "TransferWCName",  "InitialSetupTime", "LeadTime", "LeadTimeType",
                            "MandatoryOptionalProcess", "NoOfWorkers", "LaboursCost", "ProdCost", "TransferToStoreId", "TransferToStore",
                            "LeadTimeInMin", "Subitemcode", "SubItemName", "SubPartCode", "Remark", "ActualEntryByEmpName", "LAstUpdatedByEmpName"
                            , "RouteNo", "RevNo", "RevDate", "CC", "UID", "ActualEntryDate", "ActualEntryBy", "LastUpdateDate", "LatUpdatedBy");

                        model.RoutingGrid = CommonFunc.DataTableToList<RoutingGridDashBoard>(DT, "RoutingTable");
                        if (Flag != "True")
                        {
                            model.PartCode = PartCode;
                            model.ItemName = ItemName;
                            model.Stage = Stage;
                            model.WorkCenter = WorkCenter;
                            model.FromDate = FromDate;
                            model.ToDate = ToDate;
                            model.SummaryDetail = SummaryDetail;
                        }
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["EntryId"].ToString(),
                                Value = row["EntryId"].ToString()
                            });
                        }

                    }
                    //model.FromDate = new DateTime(DateTime.Today.Year, 4, 1);
                    //model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31);
                }
                model.SummaryDetail = "Summary";
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IActionResult> GetSearchData(string SummaryDetail, string PartCode, string ItemName, string Stage, string WorkCenter, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RoutingGridDashBoard();
            model = await _IRouting.GetDashboardData(SummaryDetail, PartCode, ItemName, Stage, WorkCenter, FromDate, ToDate);
            return PartialView("_RoutingDashboardGrid", model);
        }
        private static DataTable GetDetailTable(IList<RoutingDetail> DetailList)
        {
            var JWGrid = new DataTable();

            JWGrid.Columns.Add("EntryId", typeof(int));
            JWGrid.Columns.Add("SequenceNo", typeof(int));
            JWGrid.Columns.Add("RevNO", typeof(int));
            JWGrid.Columns.Add("StageID", typeof(int));
            JWGrid.Columns.Add("MachineGroupID", typeof(int));
            JWGrid.Columns.Add("WorkCenterID", typeof(int));
            JWGrid.Columns.Add("TransferToWCID", typeof(int));
            JWGrid.Columns.Add("InitialSetupTime", typeof(float));
            JWGrid.Columns.Add("LeadTime", typeof(float));
            JWGrid.Columns.Add("LeadTimeType", typeof(string));
            JWGrid.Columns.Add("LeadTimeInMin", typeof(decimal));
            JWGrid.Columns.Add("Subitemcode", typeof(int));
            JWGrid.Columns.Add("Remark", typeof(string));
            JWGrid.Columns.Add("MandatoryOptionalProcess", typeof(string));
            JWGrid.Columns.Add("NoOfWorkers", typeof(int));
            JWGrid.Columns.Add("LaboursCost", typeof(int));
            JWGrid.Columns.Add("ProdCost", typeof(float));
            JWGrid.Columns.Add("NeedQc", typeof(string));
            JWGrid.Columns.Add("TransferToStoreId", typeof(int));

            foreach (var Item in DetailList)
            {
                JWGrid.Rows.Add(
                    new object[]
                    {
                        0,
                    Item.SequenceNo,
                    Item.RevNo,
                    Item.StageID,
                    Item.MachineGroupID,
                    Item.WorkCenterID,
                    Item.TransferToWCID,
                    Math.Round(Item.IntialSetupTime,2),
                    Item.LeadTime,
                    Item.LeadTimeType.Trim(),
                    Item.LeadTimeInMin,
                    Item.SubItemCode,
                    Item.Remark,
                    Item.MandatoryOptionalProcess,
                    Item.NoOfWorkers,
                    Item.LaboursCost,
                    Item.ProdCost,
                    Item.QCRequired,
                    Item.StoreID,
                    });
            }
            JWGrid.Dispose();
            return JWGrid;
        }

        public async Task<IActionResult> DeleteByID(int ID, string PartCode = "", string ItemName = "", string Stage = "", string WorkCenter = "", string FromDate = "", string ToDate = "", string SummaryDetail = "")
        {
            var Result = await _IRouting.DeleteByID(ID);

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

            return RedirectToAction("RoutingDashboard", new { Flag = "False", PartCode = PartCode, ItemName = ItemName, Stage = Stage, WorkCenter = WorkCenter, FromDate = FromDate, ToDate = ToDate, SummaryDetail = SummaryDetail });
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> Routing(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********* Page Gate Inward ********* \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new RoutingModel();
            HttpContext.Session.Remove("KeyRoutingGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U" || Mode == "C"))
            {
                MainModel = await _IRouting.GetViewByID(ID, Mode).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                HttpContext.Session.SetString("KeyRoutingGrid", JsonConvert.SerializeObject(MainModel.RoutingDetailGrid));  
            }
            else
            {
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel = await BindModel(MainModel);
                MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            }
            return View(MainModel);
        }

        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var dt = time.ToString(format);
                return Json(formattedDate);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log any other unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IRouting.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}