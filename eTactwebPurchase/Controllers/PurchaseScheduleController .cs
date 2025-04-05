using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Barcode;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers;

[Authorize]
public class PurchaseScheduleController : Controller
{
    //private readonly ILogger _logger;
  //  public PurchaseScheduleController(IPurchaseSchedule iPurchaseSchedule, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<PurchaseOrderController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
public PurchaseScheduleController(IPurchaseSchedule iPurchaseSchedule, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<PurchaseOrderController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)


    //public PurchaseScheduleController(IPurchaseSchedule iPurchaseSchedule, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<PurchaseOrderController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
    {
        IPurchaseSchedule = iPurchaseSchedule;
        IDataLogic = iDataLogic;
        IMemoryCache = iMemoryCache;
        //_logger = logger;
        Logger = logger;
        EncryptDecrypt = encryptDecrypt;
        IWebHostEnvironment = iWebHostEnvironment;
    }

    public IDataLogic IDataLogic { get; }
    public IMemoryCache IMemoryCache { get; }
    public IPurchaseSchedule IPurchaseSchedule { get; }
    public IWebHostEnvironment IWebHostEnvironment { get; }
    public ILogger<PurchaseOrderController> Logger { get; }
 
    private EncryptDecrypt EncryptDecrypt { get; }
  

    public IActionResult AddScheduleDetail(PurchaseScheduleGrid model)
    {
        try
        {
            IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out IList<PurchaseScheduleGrid> PurchaseScheduleGrid);

            var MainModel = new PurchaseSubScheduleModel();
            var PurchSchGrid = new List<PurchaseScheduleGrid>();
            var PurchaseGrid = new List<PurchaseScheduleGrid>();
            var SSGrid = new List<PurchaseScheduleGrid>();

            if (model != null)
            {
                if (PurchaseScheduleGrid == null)
                {
                    model.SeqNo = 1;
                    PurchaseGrid.Add(model);
                }
                else
                {
                    if (PurchaseScheduleGrid.Where(x => x.ItemCode == model.ItemCode).Any() && PurchaseScheduleGrid.Where(x => ParseDate(x.DeliveryDate) == ParseDate(model.DeliveryDate)).Any())
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        model.SeqNo = PurchaseScheduleGrid.Count + 1;
                        PurchaseGrid = PurchaseScheduleGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(PurchaseGrid);
                        PurchaseGrid.Add(model);
                    }
                }

                MainModel.PurchaseScheduleList = PurchaseGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                IMemoryCache.Set("KeyPurchaseScheduleGrid", MainModel.PurchaseScheduleList, cacheEntryOptions);
            }
            else
            {
                ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
            }

            return PartialView("_PurchaseScheduleGrid", MainModel);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public static DateTime ParseDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }

        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        else
        {
            return DateTime.Parse(dateString);
        }
    }
    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IPurchaseSchedule.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GetFormRightsAmm()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IPurchaseSchedule.GetFormRightsAmm(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public IActionResult ClearGrid()
    {
        IMemoryCache.Remove("KeyPurchaseScheduleGrid");
        var MainModel = new PurchaseSubScheduleModel();
        return PartialView("_PurchaseScheduleGrid", MainModel);
    }

    public async Task<IActionResult> Dashboard()
    { 
        try
        { 
        IMemoryCache.Remove("KeyPurchaseScheduleGrid");
        var model = new PSDashboard();
            model.ToDate = HttpContext.Session.GetString("ToDate");

            var Result = await IPurchaseSchedule.GetDashboardData(model.ToDate).ConfigureAwait(true);

        if (Result != null)
        {
            var _List = new List<TextValue>();
            DataSet DS = Result.Result;


                if (DS != null)
                {
                    //var DT = DS.Tables[0].DefaultView.ToTable(true,"EntryId", "SchNo", "SchDate","PONO" , "PODate" ,"VendorName", "DeliveryAddress", "SchEffFromDate", "SchEffTillDate",
                    //"SchApproved", "SchYear", "POYearCode",   "CreatedBy", "CreatedOn", "ApprovedBy","UserName");
//                    var DT = Result.Result.DefaultView.ToTable(false, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
//"SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedByEmpName",
//"CreatedOn", "ApprovedByEmp", "Canceled", "EntryByMachineName", "SchAmendApprove",
//"SchAmendApprovedByEmp", "SchCompleted", "schAmendNo", "schAmendDate", "MRPNO", "Branch", "Active",
//"PartCode", "ItemName", "Rate", "SchQty", "PendQty", "Unit", "DeliveryDate", "ItemAmendNo", "AltSchQty",
//"AltPendQty", "AltUnit");

//                    model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT, "POSCHEDULEDETAILDASHBOARD");
//                    model.DashboardType = "Detail";

                    var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
  "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedByEmpName", 
  "CreatedOn", "ApprovedByEmp", "Canceled", "EntryByMachineName", "CreatedBy",
  "SchAmendApprovedByEmp", "SchAmendApprove", "SchCompleted", "schAmendNo", "schAmendDate", "MRPNO", "Branch", "Active");

                    model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT, "POSCHEDULEDASHBOARD");

                    foreach (var row in DS.Tables[0].AsEnumerable())
                    {
                        _List.Add(new TextValue()
                        {
                            Text = row["PONO"].ToString(),
                            Value = row["PONO"].ToString()
                        });
                    }
                    //var dd = _List.Select(x => x.Value).Distinct();
                    var _PONOList = _List.DistinctBy(x => x.Value).ToList();
                    //model.PONOList = _PONOList;
                    _List = new List<TextValue>();
                }
                // model.FromDate = new DateTime(DateTime.Today.Year, 4, 1); // 1st Feb this year

                //  model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/"); // 1st Feb this year
                //  model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");//.AddDays(-1); // Last day in January next year
                // model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31);//.AddDays(-1); // Last day in January next year

                model.ToDate = HttpContext.Session.GetString("ToDate");
            }

            return View(model);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IActionResult> DeleteByID(int ID, int YC,int createdBy,string entryByMachineName)
    {
        var Result = await IPurchaseSchedule.DeleteByID(ID, YC,createdBy,entryByMachineName).ConfigureAwait(false);

        if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult DeleteItemRow(int SeqNo)
    {
        var MainModel = new PurchaseSubScheduleModel();
        IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out List<PurchaseScheduleGrid> PurchaseScheduleGrid);
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (PurchaseScheduleGrid != null && PurchaseScheduleGrid.Count > 0)
        {
            PurchaseScheduleGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (var item in PurchaseScheduleGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.PurchaseScheduleList = PurchaseScheduleGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeyPurchaseScheduleGrid", MainModel.PurchaseScheduleList, cacheEntryOptions);
        }
        return PartialView("_PurchaseScheduleGrid", MainModel);
    }

    public async Task<IActionResult> GetSearchData(PSDashboard model)
    {
        var Result = await IPurchaseSchedule.GetSearchData(model);

        //var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
        //"SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedByEmpName", "CreatedOn", "ApprovedBy",
        //"SchAmendApprovedByEmp", "SchCompleted", "schAmendNo", "schAmendDate", "MRPNO", "Branch", "Active");
        var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
              "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedByEmpName",
              "CreatedOn", "ApprovedByEmp", "EntryByMachineName", "CreatedBy",
              "SchAmendApprovedByEmp", "SchCompleted", "schAmendNo", "schAmendDate", "MRPNO", "Branch", "Active");

        model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT,"POSCHEDULEDASHBOARD");
        model.DashboardType = "Summary";
        return PartialView("_PSDashboardGrid", model);
    }
    public async Task<IActionResult> GetDetailData(PSDashboard model)
    {
        var Result = await IPurchaseSchedule.GetDetailData(model);

        var DT = Result.Result.DefaultView.ToTable(false, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
  "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedByEmpName",
  "CreatedOn", "ApprovedByEmp", "Canceled", "EntryByMachineName", "SchAmendApprove", "CreatedBy",
  "SchAmendApprovedByEmp", "SchCompleted", "schAmendNo", "schAmendDate", "MRPNO", "Branch", "Active", 
  "PartCode", "ItemName", "Rate", "SchQty", "PendQty", "Unit", "DeliveryDate", "ItemAmendNo", "AltSchQty",
  "AltPendQty", "AltUnit");

        model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT, "POSCHEDULEDETAILDASHBOARD");
        model.DashboardType = "Detail";

        return PartialView("_PSDashboardGrid", model);
    }

    public async Task<IActionResult> GetPODATA(int AccountCode, string PONO, int Year)
    {
         
            var JSONString = await IPurchaseSchedule.GetPODATA(AccountCode, PONO, Year);
            return Json(JSONString);
        
    }

    public async Task<IActionResult> GetPOItem(int AccountCode, string PONO, int Year, int ItemCode)
    {
        var JSONString = await IPurchaseSchedule.GetPOItem(AccountCode, PONO, Year, ItemCode);

        //var Dlist = JsonConvert.DeserializeObject<Dictionary<object, object>>(JSONString);
        //JObject json = JObject.Parse(JSONString);
        //object obj = JsonConvert.DeserializeObject(JSONString, typeof(object));
        //JToken jToken = (JToken)json;

        //object value = "";
        //Dlist.TryGetValue(key: "Result", out value);

        return Json(JSONString);
    }

    public async Task<IActionResult> GetPONO(int AccountCode)
    {
        var JSONString = await IPurchaseSchedule.GetPONO(AccountCode);
        return Json(JSONString);
    }
    public async Task<JsonResult> CheckLockYear(int YearCode)
    {
        var JSON = await IPurchaseSchedule.CheckLockYear(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<IActionResult> GetPOYear(int AccountCode, string PONO)
    {
        var JSONString = await IPurchaseSchedule.GetPOYear(AccountCode, PONO);
        return Json(JSONString);
    }

    //public ILogger GetLogger()
    //{
    //    return Logger;
    //}

    // GET: PurchaseScheduleController/PurchaseScheduleForm
    [Route("{controller}/Index")]
    public async Task<ActionResult> PurchaseSchedule(int ID, string Mode, int YC)//, ILogger logger)
    {
         Logger.LogInformation("\n \n ********** Page Purchase Schedule ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

        TempData.Clear();
        var MainModel = new PurchaseSubScheduleModel();
        IMemoryCache.Remove("KeyPurchaseScheduleGrid");
        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        //MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        //MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
        MainModel.CC = HttpContext.Session.GetString("Branch");
        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U" || Mode == "C" || Mode == "PSA" || Mode == "PSU"))
        {
            MainModel = await IPurchaseSchedule.GetViewByID(ID, YC,Mode).ConfigureAwait(true);
            MainModel.Mode = Mode;
            MainModel.ID = ID;
            MainModel = await BindModel(MainModel);

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeyPurchaseScheduleGrid", MainModel.PurchaseScheduleList, cacheEntryOptions);
        }
        else
        {
            MainModel = await BindModel(MainModel);
        }
        if (Mode != "PSA" && Mode != "PSU" && Mode != "PSC")
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        else
        {

            MainModel.YearCode = YC;
            MainModel.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }

        if (Mode != "U" && Mode != "PSA" && Mode != "PSU" && Mode != "PSC")
        {
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedOn = DateTime.Now;
        }
        else
        {
            //MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            // MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
            // MainModel.UpdatedOn = DateTime.Now;
        }
        return View(MainModel);
    }

    // POST: PurchaseScheduleController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{controller}/Index")]
    public async Task<IActionResult> PurchaseSchedule(PurchaseSubScheduleModel model)
    {
        try
        {
            //ViewBag.isSuccess = false;
            var DTSSGrid = new DataTable();
            IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out IList<PurchaseScheduleGrid> PurchaseScheduleGrid);

            if (PurchaseScheduleGrid == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("PurchaseScheduleGrid", "Purchase Schedule Grid Should Have Atlease 1 Item...!");
                model = await BindModel(model);
                return View("PurchaseSchedule", model);
            }
            else
            {
                if(model.Mode == "U")
                {
                    model.Mode = "UPDATE";
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                }
                else if (model.Mode == "C")
                {
                    model.Mode = "COPY";
                }
                else if(model.Mode == "PSA")
                {
                    model.Mode = "PSA";
                }
                else
                {
                    model.Mode = "INSERT";
                }
                DTSSGrid = GetDetailTable(PurchaseScheduleGrid);

                 var Result = await IPurchaseSchedule.SavePurchSchedule (model, DTSSGrid);

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                    }
                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    if(Result.Result == null)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        return View("Error", Result);
                    }
                }
            }
            if (model.Mode == "PSA")
            {
                return RedirectToAction(nameof(PSAmendmentList));
            }
            return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
            LogException<PurchaseOrderController >.WriteException(Logger, ex);
             

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }
    }

    [HttpPost]
    public IActionResult UploadExcel()
    {
        IFormFile ExcelFile = Request.Form.Files[0];
        System.String ErrMsg = System.String.Empty; 
        var ExcelDup = new List<string>();
        var PurchaseGrid = new PurchaseScheduleGrid();
        var MainModel = new PurchaseSubScheduleModel();
        var PurchaseGridList = new List<PurchaseScheduleGrid>();
        var Error = new Dictionary<string, string>();

        if (ExcelFile != null)
        {
            //Create a Folder.
            string path = Path.Combine(this.IWebHostEnvironment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Save the uploaded Excel file.
            string fileName = Path.GetFileName(ExcelFile.FileName);
            string filePath = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                ExcelFile.CopyTo(stream);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    //return or alert message here
                }
                else
                {
                    var AC = Convert.ToInt32(Request.Form.Where(x => x.Key == "AC").FirstOrDefault().Value, System.Globalization.CultureInfo.InvariantCulture);
                    var PONO = Convert.ToString( Request.Form.Where(x => x.Key == "PONO").FirstOrDefault().Value, System.Globalization.CultureInfo.InvariantCulture);
                    var Year = Convert.ToInt32(Request.Form.Where(x => x.Key == "Year").FirstOrDefault().Value, new CultureInfo("en-IN"));
                    var FromDate = Convert.ToDateTime(Request.Form.Where(x => x.Key == "FromDate").FirstOrDefault().Value).ToString("dd/MM/yyyy");
                    var TillDate = Convert.ToDateTime(Request.Form.Where(x => x.Key == "TillDate").FirstOrDefault().Value).ToString("dd/MM/yyyy");

                    var JSONString = IPurchaseSchedule.GetPOItem(AC, PONO, Year, 0).GetAwaiter().GetResult();

                    var ItemList = JsonConvert.DeserializeObject<Root>(JSONString);

                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var ExlPC = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();
                        var ExlDD = (worksheet.Cells[row, 3].Value ?? string.Empty).ToString().Trim();

                        DateTime DelDate;
                        var isDate = DateTime.TryParse(ExlDD, out DelDate);

                        var isValidPartCode = ItemList.Result.Where(x => x.PartCode == ExlPC).Any();

                        //var isValidDate = (Convert.ToDateTime(ExlDD).CompareTo(Convert.ToDateTime(FromDate)) > 0) && (Convert.ToDateTime(ExlDD).CompareTo(Convert.ToDateTime(TillDate)) < 1);

                        var isValidDate = (DelDate.CompareTo(Convert.ToDateTime(FromDate)) > 0) && (DelDate.CompareTo(Convert.ToDateTime(TillDate)) < 1);

                        if (!isValidPartCode && !isValidDate)
                        {
                            ErrMsg = "Row : " + row + " has invalid Partcode & DeliveryDate.";
                        }
                        else if (!isValidPartCode && isValidDate)
                        {
                            ErrMsg = "Row : " + row + " has invalid Partcode.";
                        }
                        else if (isValidPartCode && !isValidDate)
                        {
                            ErrMsg = "Row : " + row + " has invalid DeliveryDate.";
                        }
                        else if (ExcelDup.Contains(ExlPC))
                        {
                            ErrMsg = "Row : " + row + " has Duplicate PartCode.";
                        }
                        else
                        {
                            ExcelDup.Add(ExlPC);
                        }

                        if (!isValidPartCode || !isValidDate || !string.IsNullOrEmpty(ErrMsg))
                        {
                            Error.Add(row.ToString(), ErrMsg);
                        }
                        else
                        {
                            PurchaseGridList.Add(new PurchaseScheduleGrid
                            {
                                SeqNo = PurchaseGridList.Count + 1,
                                ItemCode = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.ItemCode).FirstOrDefault(),
                                ItemName = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.ItemName).FirstOrDefault(),
                                PartCode = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim(),
                                SchQty = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                                PendQty = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                                Unit = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.Unit).FirstOrDefault(),
                                AltUnit = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.AltUnit).FirstOrDefault(),
                                Rate = Convert.ToDecimal(ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.Rate).FirstOrDefault()),
                                //DeliveryDate = Convert.ToDateTime((worksheet.Cells[row, 3].Value ?? string.Empty).ToString().Trim()).ToString("dd/MM/yyyy").Replace("-", "/"),
                            });
                        }
                    }
                }
            }

            ////Read the connection string for the Excel file.
            //string conString = this.Configuration.GetConnectionString("ExcelConString");
            //DataTable dt = new DataTable();
            //conString = string.Format(conString, filePath);

            //using (OleDbConnection connExcel = new OleDbConnection(conString))
            //{
            //    using (OleDbCommand cmdExcel = new OleDbCommand())
            //    {
            //        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
            //        {
            //            cmdExcel.Connection = connExcel;

            // //Get the name of First Sheet. connExcel.Open(); DataTable dtExcelSchema;
            // dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null); string
            // sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString(); connExcel.Close();

            //            //Read Data from First Sheet.
            //            connExcel.Open();
            //            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            //            odaExcel.SelectCommand = cmdExcel;
            //            odaExcel.Fill(dt);
            //            connExcel.Close();
            //        }
            //    }
            //}

            ////Insert the Data read from the Excel file to Database Table.
            //conString = this.Configuration.GetConnectionString("DBConnection");
            //using (SqlConnection con = new SqlConnection(conString))
            //{
            //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
            //    {
            //        //Set the database table name.
            //        sqlBulkCopy.DestinationTableName = "dbo.customersDB";

            // //[OPTIONAL]: Map the Excel columns with that of the database table.
            // sqlBulkCopy.ColumnMappings.Add("CustomerId", "customer_id");
            // sqlBulkCopy.ColumnMappings.Add("FirstName", "first_name");
            // sqlBulkCopy.ColumnMappings.Add("LastName", "last_name");
            // sqlBulkCopy.ColumnMappings.Add("Phone", "phone");
            // sqlBulkCopy.ColumnMappings.Add("Email", "email");
            // sqlBulkCopy.ColumnMappings.Add("Street", "street");
            // sqlBulkCopy.ColumnMappings.Add("City", "city");
            // sqlBulkCopy.ColumnMappings.Add("State", "state");
            // sqlBulkCopy.ColumnMappings.Add("Zip", "zip_code");

            //        con.Open();
            //        sqlBulkCopy.WriteToServer(dt);
            //        con.Close();
            //    }
            //}
        }

        if (Error.Count > 0)
        {
            return StatusCode(207, Error);
        }
        else
        {
            MainModel.PurchaseScheduleList = PurchaseGridList;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeyPurchaseScheduleGrid", MainModel.PurchaseScheduleList, cacheEntryOptions);

            return PartialView("_PurchaseScheduleGrid", MainModel);
        }

        return View();
    }

    private static DataTable GetDetailTable(IList<PurchaseScheduleGrid> DetailList)
    {
        var DTSSGrid = new DataTable();

        DTSSGrid.Columns.Add("ItemCode", typeof(int));
        DTSSGrid.Columns.Add("Unit", typeof(string));
        DTSSGrid.Columns.Add("SchQty", typeof(decimal));
        DTSSGrid.Columns.Add("PendQty", typeof(decimal));
        DTSSGrid.Columns.Add("AltUnit", typeof(string));
        DTSSGrid.Columns.Add("AltSchQty", typeof(decimal));
        DTSSGrid.Columns.Add("AltPendQty", typeof(decimal));
        DTSSGrid.Columns.Add("Rate", typeof(decimal));
        DTSSGrid.Columns.Add("RateInOthCurr", typeof(decimal));
        DTSSGrid.Columns.Add("DeliveryDate", typeof(string)); // datetime
        DTSSGrid.Columns.Add("ItemSize", typeof(string));
        DTSSGrid.Columns.Add("ItemColor", typeof(string));
        DTSSGrid.Columns.Add("OtherDetail", typeof(string));
        DTSSGrid.Columns.Add("Remarks", typeof(string));
        DTSSGrid.Columns.Add("DeliveryWeek", typeof(string));
        DTSSGrid.Columns.Add("schAmendNo", typeof(int));
        DTSSGrid.Columns.Add("schAmendDate", typeof(string)); // datetime    
        DTSSGrid.Columns.Add("SchAmendYear", typeof(int));
        DTSSGrid.Columns.Add("TentQtyFor1stMonth", typeof(decimal));
        DTSSGrid.Columns.Add("TentQtyFor2stMonth", typeof(decimal));

        foreach (var Item in DetailList)
        {
            DTSSGrid.Rows.Add(
                new object[]
                {
                    Item.ItemCode,
                    Item.Unit ?? string.Empty,
                    Item.SchQty,
                    Item.PendQty,
                    Item.AltUnit ?? string.Empty,
                    Item.AltSchQty,
                    Item.AltPendQty,
                    Item.Rate,
                    Item.RateInOthCurr,
                    Item.DeliveryDate == null ? string.Empty : ParseFormattedDate(Item.DeliveryDate),
                    Item.ItemSize ?? string.Empty,
                    Item.ItemColor ?? string.Empty,
                    Item.OtherDetail ?? string.Empty,
                    Item.Remarks ?? string.Empty,
                    Item.DeliveryWeek ?? string.Empty,
                    Item.schAmendNo,
                    Item.schAmendDate == null ? string.Empty : ParseFormattedDate(Item.schAmendDate),
                    Item.SchAmendYear,
                    Item.TentQtyFor1stMonth,
                    Item.TentQtyFor2stMonth
                });
        }
        DTSSGrid.Dispose();
        return DTSSGrid;
    }

    public async Task<JsonResult> FillMRPNo()
    {
        var JSON = await IPurchaseSchedule.FillMRPNo();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> FillMRPDetail(string MRPNo)
    {
        var JSON = await IPurchaseSchedule.FillMRPDetail( MRPNo);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    private async Task<PurchaseSubScheduleModel> BindModel(PurchaseSubScheduleModel model)
    {
        var oDataSet = new DataSet();
        var _List = new List<TextValue>();
        oDataSet = await IPurchaseSchedule.BindAllDropDown();

        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in oDataSet.Tables[0].Rows)
            {
                _List.Add(new TextValue
                {
                    Value = row["EntryID"].ToString(),
                    Text = row["Com_Name"].ToString()
                });
            }
            model.BranchList = _List;
            _List = new List<TextValue>();

            foreach (DataRow row in oDataSet.Tables[1].Rows)
            {
                _List.Add(new TextValue
                {
                    Value = row["AccountCode"].ToString(),
                    Text = row["AccountName"].ToString()
                });
            }
            model.AccountList = _List;
            _List = new List<TextValue>();
        }

        if (model.Mode == null && model.ID == 0)
        {
            //model.YearCode = Constants.FinincialYear;
            model.EntryID = IDataLogic.GetEntryID("PurchaseScheduleMain", Constants.FinincialYear, "SchEntryID", "SchYearcode");
          //  model.ScheduleNo = IDataLogic.GetEntryID("PurchaseScheduleMain", Constants.FinincialYear, "Entry_ID");
           // model.EntryDate = DateTime.Now.ToString("yy-mm-dd", CultureInfo.InvariantCulture);//.Replace("-", "/");
            //model.SchAmendmentDate = DateTime.Today;// DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            //model.ScheduleDate = DateTime.Today;//DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            //model.SchEffFromDate = DateTime.Today;// DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            //model.SchEffTillDate = DateTime.Today;// DateTime.Today.AddMonths(1).ToString("dd/MM/yyyy").Replace("-", "/");
        }
        return model;
    }

    public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
    {
        var JSON = await IPurchaseSchedule.AltUnitConversion(ItemCode, AltQty, UnitQty);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillEntry(int YearCode)
    {
        var JSON = await IPurchaseSchedule.FillEntry(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAddress(int AccountCode)
    {
        var JSON = await IPurchaseSchedule.GetAddress(AccountCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillAmendEntry(int YearCode)
    {
        var JSON = await IPurchaseSchedule.FillAmendEntry(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public IActionResult EditItemRow(PurchaseSubScheduleModel model)
    {
        var MainModel = new PurchaseSubScheduleModel();
        IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out List<PurchaseScheduleGrid> PurchaseScheduleGrid);
        var SSGrid = PurchaseScheduleGrid.Where(x => x.SeqNo == model.SeqNo);
        string JsonString = JsonConvert.SerializeObject(SSGrid);
        return Json(JsonString);
    }

    [HttpGet]
    public async Task<IActionResult> PSAmendmentList()
    {
        IMemoryCache.Remove("KeyPurchaseScheduleGrid");
        var model = new PSDashboard();

        //model.Mode = "Pending";
        //model.CC = HttpContext.Session.GetString("Branch");

        return View(model);
    }
    public async Task<IActionResult> GetPSAmmData(PSDashboard model)
    {
        var Result = await IPurchaseSchedule.GetPSAmmData(model);

        var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "Unit", "SchQty", "AltUnit", "AltSchQty", "AltPendQty", "PendQty", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
        "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");

        model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT, "POSCHEDULEDASHBOARD");
        model.Mode = "Pending";
        return PartialView("_AmmListGrid", model);
    }
    public async Task<IActionResult> GetUpdPSAmmData(PSDashboard model)
    {
        var Result = await IPurchaseSchedule.GetUpdPSAmmData(model);

        var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
        "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");

        model.PSDashboard = CommonFunc.DataTableToList<PurchaseScheduleDashboard>(DT, "POSCHEDULEDASHBOARD");
        model.Mode = "U";
        return PartialView("_AmmListGrid", model);
    }

    public async Task<JsonResult> GetCurrency(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag)
    {
        var JSON = await IPurchaseSchedule.GetCurrency(PONo, POYearCode, ItemCode, AccountCode, SchNo, SchYearCode, Flag);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GetExchangeRate(string Currency)
    {
        var JSON = await IPurchaseSchedule.GetExchangeRate(Currency);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpPost]
    public IActionResult UploadPurchaseSchedule()
    {
        var excelFile = Request.Form.Files[0];
        string pono = Request.Form.Where(x => x.Key == "PoNo").FirstOrDefault().Value;
        int poYearcode = Convert.ToInt32(Request.Form.Where(x => x.Key == "POYearcode").FirstOrDefault().Value);
        int AccountCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "AccountCode").FirstOrDefault().Value);
        string SchNo = Request.Form.Where(x => x.Key == "SchNo").FirstOrDefault().Value;
        var SchYearCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "SchYearCode").FirstOrDefault().Value);        
        var Flag = Request.Form.Where(x => x.Key == "Flag").FirstOrDefault().Value;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<PurchaseScheduleGrid> data = new List<PurchaseScheduleGrid>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var itemCode = IPurchaseSchedule.GetItemCode(worksheet.Cells[row, 3].Value.ToString());
                var partcode = 0;
                var itemCodeValue = 0;
                if (itemCode.Result.Result != null)
                {
                    partcode = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                    itemCodeValue = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                }
                if (partcode == 0)
                {
                    return Json("Partcode not available");
                }

                //for altunit conversion
                var altUnitConversion = AltUnitConversion(partcode, Convert.ToDecimal(worksheet.Cells[row, 9].Value.ToString()), Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()));
                JObject AltUnitCon = JObject.Parse(altUnitConversion.Result.Value.ToString());
                decimal altUnitValue = (decimal)AltUnitCon["Result"][0]["ActualUnitValue"];

                var GetCurrencyval = GetCurrency(pono, poYearcode, itemCodeValue, AccountCode, SchNo, SchYearCode, Flag);


                JObject AltPendQTy = JObject.Parse(GetCurrencyval.Result.Value.ToString());
                JToken recqtyToken = AltPendQTy["Result"]["Table"][0]["RECQTY"];                
                decimal recqtyValue = recqtyToken.Value<decimal>();
                decimal AltPendQTyValue = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()) - recqtyValue;

                if (AltPendQTyValue < 0)
                {
                    return Json("Not Done");
                }


                JObject AltCurrency = JObject.Parse(GetCurrencyval.Result.Value.ToString());
                JToken CurrencyToken = AltCurrency["Result"]["Table"][0]["Currency"];
                string Currency = CurrencyToken.Value<string>();

                var GetExhange = GetExchangeRate(Currency);

                JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 11].Value) * AltRateToken;

                data.Add(new PurchaseScheduleGrid()
                {
                    SeqNo = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                    ItemCode = itemCodeValue,
                    ItemName = worksheet.Cells[row, 4].Value.ToString(),
                    Unit = worksheet.Cells[row, 5].Value.ToString(),
                    SchQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
                    PendQty = Convert.ToDecimal(AltPendQTyValue.ToString("F2")),
                    AltUnit = worksheet.Cells[row, 8].Value.ToString(),
                    AltSchQty = altUnitValue,
                    AltPendQty = Convert.ToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                    Rate = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                    RateInOthCurr = RateInOther,
                    DeliveryDate = worksheet.Cells[row, 13].Value.ToString(),
                    ItemSize = worksheet.Cells[row, 14].Value == null ? "" : worksheet.Cells[row, 14].Value.ToString(),
                    ItemColor = worksheet.Cells[row, 15].Value == null ? "" : worksheet.Cells[row, 15].Value.ToString(),
                    OtherDetail = worksheet.Cells[row, 16].Value == null ? "" : worksheet.Cells[row, 16].Value.ToString(),
                    Remarks = worksheet.Cells[row, 17].Value == null ? "" : worksheet.Cells[row, 17].Value.ToString(),                   
                });
            }
        }


        var MainModel = new PurchaseSubScheduleModel();
        var purchaseSchedules = new List<PurchaseScheduleGrid>();
        var SSGrid = new List<PurchaseScheduleGrid>();
        var purchaseScheduleList = new List<PurchaseScheduleGrid>();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        var seqNo = 0;
        IMemoryCache.Remove("KeyPurchaseScheduleGrid");

        foreach (var item in data)
        {
            if (item != null)
            {
                IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out IList<PurchaseScheduleGrid> PurchaseScheduleGrid);

                if (PurchaseScheduleGrid == null)
                {
                    item.SeqNo += seqNo + 1;
                    purchaseSchedules.Add(item);
                    seqNo++;
                }
                else
                {
                    if (PurchaseScheduleGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        item.SeqNo = PurchaseScheduleGrid.Count + 1;
                        purchaseSchedules = PurchaseScheduleGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(purchaseSchedules);
                        purchaseSchedules.Add(item);
                    }
                    
                }
                MainModel.PurchaseScheduleList = purchaseSchedules;

                IMemoryCache.Set("KeyPurchaseScheduleGrid", MainModel.PurchaseScheduleList, cacheEntryOptions);

            }
        }
        IMemoryCache.TryGetValue("KeyPurchaseScheduleGrid", out IList<PurchaseScheduleGrid> PurchaseScheduleGrid1);

        return PartialView("_PurchaseScheduleGrid", MainModel);
    }
}