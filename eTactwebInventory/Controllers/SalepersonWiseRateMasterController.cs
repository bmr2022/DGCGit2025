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
        public async Task<ActionResult> SalepersonWiseRateMaster(int ID, string Mode, string CC, int YC)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new SalepersonWiseRateMasterModel();
           
            MainModel.CC = HttpContext.Session.GetString("Branch");
            
            MainModel.MachineName = Environment.MachineName;

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _ISalepersonWiseRateMaster.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                //MainModel.ActualEntryBy= Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
               

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }
            else
            {
                MainModel.EntryDate = HttpContext.Session.GetString("EntryDate");
                MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");

            }

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
                            model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
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
                    _logger.LogInformation("Save salepersonrate Data done", DateTime.UtcNow);
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


        public async Task<JsonResult> NewEntryId(int YearCode, string entrydate)
        {
            var JSON = await _ISalepersonWiseRateMaster.NewEntryId(YearCode, entrydate);
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

        public async Task<IActionResult> SalePersonRateMasterDashBoard()
        {
            var model = new SalepersonWiseRateMasterModel();
            //model = await _ISalepersonWiseRateMaster.DashBoard();


            //return View("SalePersonRateMasterDashBoard", model);
            return View(model);

        }
        public async Task<IActionResult> DashBoardData()
        {
            var model = new SalepersonWiseRateMasterModel();
            model = await _ISalepersonWiseRateMaster.DashBoard();


            return PartialView("_SalePersonDashBoardGrid", model);

        }

        public async Task<IActionResult> DeleteByID(int entryid, int yearcode, string machinename, string CC, int actualentryby, int salespersonid)
        {
            machinename=Environment.MachineName;
            CC = HttpContext.Session.GetString("Branch");
            actualentryby = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _ISalepersonWiseRateMaster.DeleteByID( entryid,  yearcode,  machinename,  CC,  actualentryby,  salespersonid);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Locked)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction(nameof(SalePersonRateMasterDashBoard));
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel()
        {
            try
            {
                IFormFile ExcelFile = Request.Form.Files.FirstOrDefault();
                if (ExcelFile == null || ExcelFile.Length == 0)
                {
                    return BadRequest("Invalid file. Please upload a valid Excel file.");
                }

                string path = Path.Combine(this._IWebHostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(ExcelFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await ExcelFile.CopyToAsync(stream);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var SaleGridList = new List<SalepersonWiseRateDetail>();
                var MainModel = new SalepersonWiseRateMasterModel();
                var errors = new List<string>(); // List to collect validation errors

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return BadRequest("Uploaded file does not contain any worksheet.");
                    }

                    var rowCount = worksheet.Dimension.Rows;
                    var itemList = new List<SalepersonWiseRateDetail>();
                    for (int row = 2; row <= rowCount; row++)
                    {
                        bool isRowEmpty = true;
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            if (!string.IsNullOrEmpty((worksheet.Cells[row, col].Value ?? string.Empty).ToString().Trim()))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }
                        if (isRowEmpty) continue;

                        var partCode = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();
                        

                       

                        // **Fetch Item Details from Database**
                        var response = await _ISalepersonWiseRateMaster.GetExcelData(partCode);

                        if (response?.Result is not DataTable itemData || itemData.Rows.Count == 0)
                        {
                            errors.Add($"No data found for PartCode '{partCode}' at row {row}.");
                            continue;
                        }

                        
                        string itemName = itemData.Rows[0]["Item_Name"].ToString();
                        int itemCode = Convert.ToInt32(itemData.Rows[0]["Item_Code"]);
                        var SalePrice= Convert.ToDecimal(itemData.Rows[0]["SalePrice"]);


                       

                        

                        

                        

                      
                        // **Delivery Date Validation**
                       
                        
                        // **Add to SaleGridList**
                        SaleGridList.Add(new SalepersonWiseRateDetail
                        {
                           
                            PartCode = partCode,                          
                            ItemCode = itemCode,
                            ItemName = itemName,
                            OriginalRate= SalePrice,
                            NewRate = decimal.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out decimal tempStockqty) ? tempStockqty : 0,
                            

                        });
                    }

                    var duplicateItems = SaleGridList
         .GroupBy(x => new { x.ItemCode })
         .Where(g => g.Count() > 1)
         .Select(g => $"[PartCode: {g.Key.ItemCode}]")
         .ToList();

                    if (duplicateItems.Any())
                    {
                        var duplicateErrorMsg = "Duplicate PartCode found:\n" + string.Join("\n", duplicateItems);
                        return BadRequest(string.Join("\n", duplicateErrorMsg));
                    }

                    if (errors.Count > 0)
                    {
                        return BadRequest(string.Join("\n", errors));
                    }
                }

                MainModel.ItemDetailGrid = SaleGridList;
               
                return PartialView("_SalePersonWiseRateGrid", MainModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the Excel file.");
                return StatusCode(500, "An internal server error occurred. Please check the file format.");
            }
        }


    }
}
