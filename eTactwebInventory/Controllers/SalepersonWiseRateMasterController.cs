using System.Data;
using System.Diagnostics;
using System.Xml;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Data;
using System.Globalization;
using System.Reflection;


namespace eTactwebInventory.Controllers
{
    public class SalepersonWiseRateMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISalepersonWiseRateMaster _ISalepersonWiseRateMaster { get; }
        private readonly ILogger<SalepersonWiseRateMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SalepersonWiseRateMasterController(ILogger<SalepersonWiseRateMasterController> logger, IDataLogic iDataLogic, ISalepersonWiseRateMaster iSalepersonWiseRateMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISalepersonWiseRateMaster = iSalepersonWiseRateMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> SalepersonWiseRateMaster(int ID, string Mode, string CC)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new SalepersonWiseRateMasterModel();
           
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EntryDate = HttpContext.Session.GetString("EntryDate");
            MainModel.YearCode =Convert.ToInt32( HttpContext.Session.GetString("YearCode"));
            //if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            //{
            //    MainModel = await _ISalepersonWiseRateMaster.GetViewByID(ID).ConfigureAwait(false);
            //    MainModel.Mode = Mode; // Set Mode to Update
            //    MainModel.StoreId = ID;
            //    MainModel.Store_Name = Store_Name;
            //    MainModel.StoreType = StoreType;
            //    MainModel.CC = CC;
            //    MainModel.EntryDate = EntryDate;

            //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            //        SlidingExpiration = TimeSpan.FromMinutes(55),
            //        Size = 1024
            //    };
            //}

            return View(MainModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<ActionResult> SalepersonWiseRateMaster(SalepersonWiseRateMasterModel model)
        {
            try
            {
                bool isError = true;
                DataSet DS = new();
                DataTable ItemDetailDT = null;
                DataTable TaxDetailDT = null;
                DataTable MultiBuyersDT = null;
                ResponseResult Result = new();
                DataTable DelieveryScheduleDT = null;
                Dictionary<string, string> ErrList = new();


                var ItemDetailList = new List<SalepersonWiseRateMasterModel>();

                _logger.LogInformation("ItemDetailList session Data done", DateTime.UtcNow);


                var MainModel = new SalepersonWiseRateMasterModel();
                //_MemoryCache.TryGetValue("ItemList", out List<ItemDetail> ItemDetailList);

                ModelState.Clear();

                //var ItemDetailList = MainModel.ItemDetailGrid;
                _logger.LogInformation("TaxGrid session Data done", DateTime.UtcNow);
                DataTable Table = new DataTable();


                Table.Columns.Add("entryid", typeof(int));
                Table.Columns.Add("yearcode", typeof(int));
                Table.Columns.Add("entrydate", typeof(string));
                Table.Columns.Add("salespersonid", typeof(int));
                Table.Columns.Add("itemgroupId", typeof(int));
                Table.Columns.Add("itemcode", typeof(int));
                Table.Columns.Add("Originalrate", typeof(float));
                Table.Columns.Add("Newrate", typeof(float));
                Table.Columns.Add("actualentryby", typeof(int));
                Table.Columns.Add("updatedby", typeof(int));
                Table.Columns.Add("updationdate", typeof(string));
                Table.Columns.Add("machinename", typeof(string));
                Table.Columns.Add("CC", typeof(string));

                if (model.ItemDetailGrid != null)
                {
                    foreach (var Item in model.ItemDetailGrid.Where(x => x.NewRate.HasValue && x.NewRate > 0))
                    {
                        Table.Rows.Add(
                            0,
                   0,
                    "",
                    0,
                   0,
                    Item.ItemCode,
                    Item.OriginalRate,
                    Item.NewRate,
                     0,
                    0,
                   "",
                    "",
                   ""

                        );
                    }
                }


                isError = false;

                _logger.LogInformation("GetItemDetailTable Data done", DateTime.UtcNow);




                if (!isError)
                {
                    if (Table.Rows.Count > 0 || Table.Rows.Count > 0)
                    {
                        if (model.Mode != "U" )
                        {
                            model.Mode = "INSERT";
                            model.UpdatedBy = 0;
                            model.UpdationDate = "";
                            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                            model.MachineName=Environment.MachineName;
                            model.CC= HttpContext.Session.GetString("Branch");
                        }
                        else if (model.Mode == "U")
                        {
                            model.Mode = "UPDATE";
                            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                            model.UpdationDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            model.MachineName = Environment.MachineName;
                            model.CC = HttpContext.Session.GetString("Branch");
                        }
                       
                        //model.Mode = model.Mode == "U" ? "Update" : "Insert";
                        model.CreatedBy = Constants.UserID;
                        Result = await _ISalepersonWiseRateMaster.SaveSalePersonWiseRate(Table, model);
                    }
                    _logger.LogInformation("Save SaleOrder Data done", DateTime.UtcNow);
                    if (Result != null)
                    {
                        var stringResponse = JsonConvert.SerializeObject(Result);


                        if (Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            input = Result.StatusText;
                            int index = input.IndexOf("#ERROR_MESSAGE");

                            if (index != -1)
                            {
                                int messageStartIndex = index + "#ERROR_MESSAGE".Length; // Remove the extra space and colon
                                string errorMessage = input.Substring(messageStartIndex).Trim();

                                int maxLength = 100;
                                int wrapLength = Math.Min(maxLength, errorMessage.Length);

                                string formattedMessage = errorMessage.Substring(0, wrapLength).Replace("\n", "<br>");

                                TempData["ErrorMessage"] = formattedMessage;
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }

                        }
                        else if (model.Mode == "Update")
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        else
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }



                        return RedirectToAction("SalepersonWiseRateMaster", new { ID = 0, YC = 0, Mode = "" });
                    }

                }

            
          
               
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                _logger.LogError("\n \n" + ex, ex.Message, model);
            }
            return View(model);
        }
        private static DataSet GetItemDetailTable(List<SalepersonWiseRateMasterModel> itemDetailList)
        {
            DataSet DS = new();
            DataTable Table = new();

            Table.Columns.Add("entryid", typeof(int));
            Table.Columns.Add("yearcode", typeof(int));
            Table.Columns.Add("entrydate", typeof(string));
            Table.Columns.Add("salespersonid", typeof(int));
            Table.Columns.Add("itemgroupId", typeof(int));
            Table.Columns.Add("itemcode", typeof(int));
            Table.Columns.Add("Originalrate", typeof(float));
            Table.Columns.Add("Newrate", typeof(float));
            Table.Columns.Add("actualentryby", typeof(int));
            Table.Columns.Add("updatedby", typeof(int));
            Table.Columns.Add("updationdate", typeof(string));
            Table.Columns.Add("machinename", typeof(string));
            Table.Columns.Add("CC", typeof(string));
          

           

            foreach (SalepersonWiseRateMasterModel Item in itemDetailList)
            {
                Table.Rows.Add(
                    new object[]
                    {
                    Item.EntryId,
                    Item.YearCode,
                    Item.EntryDate,
                    Item.SalesPersonId,
                    Item.ItemGroupId,
                    Item.ItemCode,
                    Item.OriginalRate,
                    Item.NewRate,
                    Item.ActualEntryBy ?? 0,
                    Item.UpdatedBy ?? 0,
                    Item.UpdationDate == null ? "" : ParseFormattedDate(Item.UpdationDate),
                    Item.MachineName ?? string.Empty,
                    Item.CC ?? string.Empty

                    
                    });

               

            }

            DS.Tables.Add(Table);
           
            return DS;
        }


        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _ISalepersonWiseRateMaster.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSalePerson()
        {
            var JSON = await _ISalepersonWiseRateMaster.FillSalePerson();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetItemData(int ItemGroupId)
        {
            var model = new SalepersonWiseRateMasterModel();
            model = await _ISalepersonWiseRateMaster.GetItemData(ItemGroupId);


            return PartialView("_SalePersonWiseRateGrid", model);

        }
    }
}
