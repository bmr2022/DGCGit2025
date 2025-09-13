using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class PartCodePartyWiseController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPartCodePartyWise _IPartCodePartyWise;
        private readonly ILogger<PartCodePartyWiseController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PartCodePartyWiseController(ILogger<PartCodePartyWiseController> logger, IDataLogic iDataLogic, IPartCodePartyWise iPartCodePartyWise, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPartCodePartyWise = iPartCodePartyWise;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> PartCodePartyWise()
        {
            ViewData["Title"] = "Part Code Party Wise";
            TempData.Clear();
            HttpContext.Session.Remove("KeyPartCodePartyWiseGrid");
            var model = new PartCodePartyWiseModel();
            model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };

            return View(model);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> PartCodePartyWise(string Mode,int ItemCode, string ItemName = "", string PartCode = "", string VendCustPartCode = "", string VendCustitemname = "", string AccountName = "", string Searchbox = "", string DashboardType = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new PartCodePartyWiseModel();
            HttpContext.Session.Remove("KeyPartCodePartyWiseGrid");
            if (!string.IsNullOrEmpty(Mode) && ItemCode > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IPartCodePartyWise.GetViewByID(ItemCode).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ItemCode = ItemCode;


                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }
            else
            {
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = HttpContext.Session.GetString("LastUpdatedDate");
            }
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.VendCustPartCodeBack = VendCustPartCode;
            MainModel.VendCustitemnameBack = VendCustitemname;
            MainModel.AccountNameBack = AccountName;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.SummaryDetailBack = DashboardType;
            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]

        public async Task<IActionResult> PartCodePartyWise(PartCodePartyWiseModel model)
        {
            try
            {
                var GIGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                List<PartCodePartyWiseItemDetail> PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                }
 
                var MainModel = new PartCodePartyWiseModel();
                var ProductionEntryGrid = new List<PartCodePartyWiseItemDetail>();
                var ProductionGrid = new List<PartCodePartyWiseItemDetail>();
                var SSGrid = new List<PartCodePartyWiseItemDetail>();
                if (PartCodePartyWiseItemDetail == null)
                {
                    ModelState.Clear();
                    model = new PartCodePartyWiseModel();
                    TempData["EmptyError"] = "EmptyError";
                    return View(model);
                }
                else
                {
                    if (model.Mode == "U")
                    {
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        GIGrid = GetDetailTable(PartCodePartyWiseItemDetail);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(PartCodePartyWiseItemDetail);
                    }

                    var Result = await _IPartCodePartyWise.SavePartCodePartWise(model, GIGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyPartCodePartyWiseGrid");
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = new PartCodePartyWiseModel();

                                model2.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model2.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View(model2);
                            }
                            else
                            {
                                TempData["500"] = "500";
                                model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model.ItemDetailGrid = PartCodePartyWiseItemDetail;
                                return View(model);
                            }
                        }
                    }
                    var model1 = new PartCodePartyWiseModel();

                    model1.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                    model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model1.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    return RedirectToAction(nameof(PartCodePartyWise));
                }
            }
            catch (Exception ex)
            {
                LogException<PartCodePartyWiseController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        private static DataTable GetDetailTable(IList<PartCodePartyWiseItemDetail> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("VendCustAccountCode", typeof(int));
                GIGrid.Columns.Add("VendCustPartCode", typeof(string));
                GIGrid.Columns.Add("VendCustitemname", typeof(string));
                GIGrid.Columns.Add("ItemRate", typeof(float));
                GIGrid.Columns.Add("BusinessPercentage", typeof(float));
                GIGrid.Columns.Add("moq", typeof(float));
                GIGrid.Columns.Add("LeadTimeInDays", typeof(int));
                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                    Item.SeqNo==0?0:Item.SeqNo,
                    Item.AccountCode == null ? 0 : Item.AccountCode,
                    Item.VendCustPartCode == null ? "" : Item.VendCustPartCode,
                    Item.VendCustitemname == null ? "" : Item.VendCustitemname,
                    Item.ItemRate == 0 ? 0:Item.ItemRate,
                    Item.BusinessPercentage == 0 ? 0 :Item.BusinessPercentage,
                    Item.MOQ== 0 ? 0 : Item.MOQ,
                    Item.LeadTimeInDays== 0 ? 0:Item.LeadTimeInDays
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
        public async Task<JsonResult> FillItems(string Type, string ShowAllItem)
        {
            var JSON = await _IPartCodePartyWise.FillItems(Type, ShowAllItem);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddPartCodePartWiseGrid(PartCodePartyWiseItemDetail model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                    List<PartCodePartyWiseItemDetail> PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                    }
                    var MainModel = new PartCodePartyWiseModel();
                    var ProductionEntryGrid = new List<PartCodePartyWiseItemDetail>();
                    var ProductionGrid = new List<PartCodePartyWiseItemDetail>();
                    var SSGrid = new List<PartCodePartyWiseItemDetail>();

                    if (model != null)
                    {
                        if (PartCodePartyWiseItemDetail == null)
                        {
                            model.SeqNo = 1;
                            ProductionGrid.Add(model);
                        }
                        else
                        {
                            if (PartCodePartyWiseItemDetail.Where(x => x.AccountName == model.AccountName).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = PartCodePartyWiseItemDetail.Count + 1;
                                ProductionGrid = PartCodePartyWiseItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ProductionGrid);
                                ProductionGrid.Add(model);
                            }
                        }

                        MainModel.ItemDetailGrid = ProductionGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_PartCodePartyWiseGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                    List<PartCodePartyWiseItemDetail> PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                    }

                    var MainModel = new PartCodePartyWiseModel();
                    var ProductionEntryGrid = new List<PartCodePartyWiseItemDetail>();
                    var ProductionGrid = new List<PartCodePartyWiseItemDetail>();
                    var SSGrid = new List<PartCodePartyWiseItemDetail>();

                    if (model != null)
                    {
                        if (PartCodePartyWiseItemDetail == null)
                        {
                            model.SeqNo = 1;
                            ProductionGrid.Add(model);
                        }
                        else
                        {
                            if (PartCodePartyWiseItemDetail.Where(x => x.AccountName == model.AccountName  &&  x.VendCustitemname == model.VendCustitemname && x.VendCustPartCode == model.VendCustPartCode).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = PartCodePartyWiseItemDetail.Count + 1;
                                ProductionGrid = PartCodePartyWiseItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ProductionGrid);
                                ProductionGrid.Add(model);
                            }

                        }

                        MainModel.ItemDetailGrid = ProductionGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_PartCodePartyWiseGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemFromGrid(int SeqNo, string Mode)
        {
            var MainModel = new PartCodePartyWiseModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                List<PartCodePartyWiseItemDetail> ItemDetailGrid = new List<PartCodePartyWiseItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ItemDetailGrid = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                }
                //_MemoryCache.TryGetValue("KeyPartCodePartyWiseGrid", out List<ItemDetailGrid> ItemDetailGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ItemDetailGrid != null && ItemDetailGrid.Count > 0)
                {
                    ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ItemDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = ItemDetailGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                List<PartCodePartyWiseItemDetail> PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                }
  
                //_MemoryCache.TryGetValue("KeyPartCodePartyWiseGrid", out List<ItemDetailGrid> ItemDetailGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (PartCodePartyWiseItemDetail != null && PartCodePartyWiseItemDetail.Count > 0)
                {
                    PartCodePartyWiseItemDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in PartCodePartyWiseItemDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = PartCodePartyWiseItemDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);
                }
            }

            return PartialView("_PartCodePartyWiseGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode, int AccountCode)
        {
            IList<PartCodePartyWiseItemDetail> PartCodePartyWiseItemDetail = new List<PartCodePartyWiseItemDetail>();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyPartCodePartyWiseGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    PartCodePartyWiseItemDetail = JsonConvert.DeserializeObject<List<PartCodePartyWiseItemDetail>>(modelJson);
                }
            }
            IEnumerable<PartCodePartyWiseItemDetail> SSGrid = PartCodePartyWiseItemDetail;
            if (PartCodePartyWiseItemDetail != null)
            {
                SSGrid = PartCodePartyWiseItemDetail
    .Where(x => x.SeqNo == SeqNo || x.AccountCode == AccountCode)
    .ToList();
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string Type, string ShowAllItem)
        {
            var JSON = await _IPartCodePartyWise.FillPartCode(Type, ShowAllItem);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetListForUpdate(int ItemCode)
        {
            HttpContext.Session.Remove("KeyPartCodePartyWiseGrid");
            var model = await _IPartCodePartyWise.GetListForUpdate(ItemCode);

            string serializedGrid = JsonConvert.SerializeObject(model.ItemDetailGrid);
            HttpContext.Session.SetString("KeyPartCodePartyWiseGrid", serializedGrid);

            if (model.ItemDetailGrid != null && model.ItemDetailGrid.Count > 0)
            {
                model.Mode="U";
                return Json(new { redirectUrl = Url.Action("PartCodePartyWise", new { model.Mode,ItemCode }) });
            }
            return PartialView("_PartCodePartyWiseGrid", model);
        }
        public async Task<JsonResult> FillAccountName(string Type)
        {
            var JSON = await _IPartCodePartyWise.FillAccountName(Type);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IPartCodePartyWise.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //public async Task<JsonResult> GetFeatureOption()
        //{
        //    var JSON = await _IPartCodePartyWise.GetFeatureOption("FeatureOption", "SP_ProductionEntry");
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}
        public async Task<JsonResult> GetDefaultBranch()
        {
            var username = HttpContext.Session.GetString("Branch");

            // Render profile page with username
            return Json(username);
        }
        public async Task<IActionResult> PartcodePartyWiseDashboard(string Flag = "True")
        {
            try
            {
                HttpContext.Session.Remove("KeyPartCodePartyWiseGrid");
                var model = new PartCodePartyWiseDashboard();
                var Result = await _IPartCodePartyWise.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "ItemName", "PartCode", "VendCustPartCode",
                        "VendCustitemname", "ItemRate", "BusinessPercentage", "MOQ", "LeadTimeInDays");
                        model.PartCodePartyDashboard = CommonFunc.DataTableToList<PartCodePartyWiseDashboard>(DT, "PartCodePartyWise");

                    }
                    if (Flag != "True")
                    {
                        //model.ItemName = ItemName;
                        //model.PartCode = PartCode;
                        //model.Searchbox = Searchbox;
                        //model.DashboardType = DashboardType;
                        return View(model);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                //string apiUrl = "https://worldtimeapi.org/api/ip";

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
        public async Task<JsonResult> GetAccountList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<IActionResult> GetSearchData(string ItemName , string PartCode, string VendCustPartCode , string VendCustitemname , string AccountName ,string DashboardType)
        {
            var model = new PartCodePartyWiseDashboard();
            model = await _IPartCodePartyWise.GetDashboardData(ItemName, PartCode, AccountName, VendCustPartCode, VendCustitemname, DashboardType);
            model.DashboardType = "Summary";
            return PartialView("_PartCodePartyWiseDashboardDetailGrid", model);
        }
        public async Task<JsonResult> GetBranchList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("BRANCHLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> GetUnit(int ItemCode, string Flag)
        {
            var JSON = _IPartCodePartyWise.GetUnit(ItemCode, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ItemCode, string EntryByMachineName ,string ItemName = "", string PartCode = "", string AccountName = "", string CustvendPartCode = "", string CustvendItemName = "", string DashboardType = "", string Searchbox = "")
        {
            var Result = await _IPartCodePartyWise.DeleteByID(ItemCode,EntryByMachineName);

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

            return RedirectToAction("PartcodePartyWiseDashboard", new { Flag = "False", ItemName = ItemName, PartCode = PartCode, AccountName = AccountName, CustvendPartCode = CustvendPartCode, CustvendItemName = CustvendItemName, DashboardType = DashboardType, Searchbox = Searchbox});
        }


    }
}
