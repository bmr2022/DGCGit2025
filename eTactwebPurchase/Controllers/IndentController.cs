using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.Runtime.InteropServices.JavaScript.JSType.String;

namespace eTactWeb.Controllers
{
    public class IndentController : Controller
    {
        public IDataLogic IDataLogic { get; }
        public IIndent _Indent { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<IndentController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration _iconfiguration;
        private readonly ConnectionStringService _connectionStringService;
        public IndentController(IIndent _IIndent, IDataLogic iDataLogic, ILogger<IndentController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _Indent = _IIndent;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<ActionResult> Indent(int ID, string Mode, int YC, string FromDate = "", string ToDate = "",  string IndentNo = "", string DeptName = "", string ItemName = "", string PartCode="",string Searchbox = "", string SummaryDetail = "")
        {
            var MainModel = new IndentModel();
            HttpContext.Session.Remove("KeyIndentList");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.IndentorEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.IndentorEmpName = HttpContext.Session.GetString("EmpName");
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                //MainModel.CreatedOn = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _Indent.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.IndentorEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.IndentorEmpName = HttpContext.Session.GetString("EmpName");
                MainModel = await BindModel(MainModel);

                HttpContext.Session.SetString("KeyIndentList", JsonConvert.SerializeObject(MainModel.IndentDetails));
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.IndentNoBack = IndentNo;
            MainModel.DeptNameBack = DeptName;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack= SummaryDetail;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            return View(MainModel);
        }

        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = IWebHostEnvironment.ContentRootPath;
            string webRootPath = IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _Indent.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0]); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\PO.frx"); // default report
            }
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            my_connection_string = _connectionStringService.GetConnectionString();
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Prepare();
            foreach (var dataSource in webReport.Report.Dictionary.DataSources)
            {
                if (dataSource is TableDataSource tableDataSource)
                {
                    tableDataSource.Enabled = true;
                    tableDataSource.Init(); // Refresh the data source
                }
            }
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();

                webReport.Report.Load(webRootPath + "\\IndentPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the HTML export
                using (HTMLExport html = new HTMLExport())
                {
                    using (FileStream st = new FileStream(webRootPath + "\\test.html", FileMode.Create))
                    {
                        webReport.Report.Export(html, st);
                        return File("App_Data/test.html", "application/octet-stream", "Test.html");
                    }
                }
            }
        }

        public IActionResult GetImage(int EntryId = 0, int YearCode = 0)
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\IndentPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the Image export
                using (ImageExport image = new ImageExport())
                {
                    image.ImageFormat = ImageExportFormat.Jpeg;
                    image.JpegQuality = 100; // Set up the quality
                    image.Resolution = 100; // Set up a resolution 
                    image.SeparateFiles = false; // We need all pages in one big single file

                    using (MemoryStream st = new MemoryStream())// Using stream to save export
                    {
                        webReport.Report.Export(image, st);
                        return base.File(st.ToArray(), "image/jpeg");
                    }
                }
            }
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new IndentModel();
            string modelJson = HttpContext.Session.GetString("KeyIndentList");
            List<IndentDetail> IndentGrids = new List<IndentDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IndentGrids = JsonConvert.DeserializeObject<List<IndentDetail>>(modelJson);
            }
            var IndentGrid = IndentGrids.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(IndentGrid);
            return Json(JsonString);
        }

        public IActionResult ClearGrid()
        {
            HttpContext.Session.Remove("KeyIndentList");
            var MainModel = new IndentModel();
            return PartialView("_IndentGrid", MainModel);
        }
        private async Task<IndentModel> BindModel(IndentModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _Indent.BindAllDropDowns("BINDALLDROPDOWN");
            model.PartCodeList = new List<TextValue>();
            model.BranchList = new List<TextValue>();
            model.ProjectList = new List<TextValue>();
            model.DeptList = new List<TextValue>();
            model.CostCenterList = new List<TextValue>();
            model.EmpList = new List<TextValue>();
            model.MachineList = new List<TextValue>();
            model.StoreList = new List<TextValue>();
            model.VendorList = new List<TextValue>();
            model.ItemNameList = new List<TextValue>();
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
                        Value = row["projectname"].ToString(),
                        Text = row["projectname"].ToString()
                    });
                }
                model.ProjectList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["DeptId"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["CostCenterName"].ToString(),
                        Text = row["CostCenterName"].ToString()
                    });
                }
                model.CostCenterList = _List;
                _List = new List<TextValue>();


                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()
                    });
                }
                model.EmpList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["machinename"].ToString()
                    });
                }
                model.MachineList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[6].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["StoreId"].ToString(),
                        Text = row["Store_Name"].ToString()
                    });
                }
                model.StoreList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[7].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["FGItemCode"].ToString(),
                        Text = row["FGPartCode"].ToString()
                    });
                }
                model.PartCodeList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[8].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["FGItemCode"].ToString(),
                        Text = row["FGItem"].ToString()
                    });
                }
                model.ItemNameList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[9].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Account_Code"].ToString(),
                        Text = row["Account_Name"].ToString()
                    });
                }
                model.VendorList = _List;
                _List = new List<TextValue>();

            }
            return model;
        }

        public IActionResult AddIndent(IndentDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyIndentList");
                List<IndentDetail> IndentGrid = new List<IndentDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IndentGrid = JsonConvert.DeserializeObject<List<IndentDetail>>(modelJson);
                }

                var MainModel = new IndentModel();
                var IndentDetail = new List<IndentDetail>();
                var IndentDetailGrid = new List<IndentDetail>();
                model.ReqDate = ParseDate(model.ReqDate).ToString();

                if (model != null)
                {
                    if (IndentGrid == null)
                    {
                        model.SeqNo = 1;
                        IndentDetail.Add(model);
                    }
                    else
                    {
                        if (IndentGrid.Any(x => x.ItemCode == model.ItemCode && x.StoreID == model.StoreID))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SeqNo = IndentGrid.Count + 1;
                            IndentDetail = IndentGrid.Where(x => x != null).ToList();
                            IndentDetailGrid.AddRange(IndentDetail);
                            IndentDetail.Add(model);

                        }
                    }

                    MainModel.IndentDetails = IndentDetail;

                    HttpContext.Session.SetString("KeyIndentList", JsonConvert.SerializeObject(MainModel.IndentDetails));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Indent List Cannot Be Empty...!");
                }
                return PartialView("_IndentGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetSearchData(IndentDashboard model)
        {
            var Result = await _Indent.GetDashboardData(model);
            DataSet DS = Result.Result;

            if (model.SummaryDetail == "Detail")
            {
                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "EntryDate", "YearCode",
                       "IndentNo", "IndentDate", "itemservice", "BOMIND", "BomItemCode", "BOMPartCode", "BOMtem", "IndentorEmpId",
                                   "IndentCompleted", "DepartmentId", "DeptName", "Bomqty", "CC",
                                   "Approved", "ApprovedbyEmpId", "ApprovedDate", "UID", "IndentRemark",
                                   "MRPNO", "MRPEntryId", "canceled", "closed", "firstapproved", "firstapprovedby",
                                   "firstapproveddate", "MachineNo","PartCode","ItemName","PendReqNo","ReqYearCode","ReqDate","Qty",
                                   "Unit","StoreName","TotalStock","AltUnit","Model","Size","Account_Name","Account_Name2","ItemDescription","ItemRemark",
                                   "Specification","PendQtyForPO","PendAltQtyForPO","Color","ApproValue", "AltQty");

                model.IndentDashboardGrid = CommonFunc.DataTableToList<IndentDashboard>(DT, "IndentDetail");
            }
            else
            {
                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "EntryDate", "YearCode",
                       "IndentNo", "IndentDate", "itemservice", "BOMIND", "BomItemCode", "BOMPartCode", "BOMtem", "IndentorEmpId",
                                   "IndentCompleted", "DepartmentId", "DeptName", "Bomqty", "CC",
                                   "Approved", "ApprovedbyEmpId", "ApprovedDate", "UID", "IndentRemark",
                                   "MRPNO", "MRPEntryId", "canceled", "closed", "firstapproved", "firstapprovedby",
                                   "firstapproveddate", "MachineNo");

                model.IndentDashboardGrid = CommonFunc.DataTableToList<IndentDashboard>(DT, "Indent");
            }
            return PartialView("_IndentDashboardGrid", model);
        }

        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new IndentModel();

            string modelJson = HttpContext.Session.GetString("KeyIndentList");
            List<IndentDetail> IndentDetail = new List<IndentDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IndentDetail = JsonConvert.DeserializeObject<List<IndentDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IndentDetail != null && IndentDetail.Count > 0)
            {
                IndentDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IndentDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.IndentDetails = IndentDetail;

                HttpContext.Session.SetString("KeyIndentList", JsonConvert.SerializeObject(MainModel.IndentDetails));
            }

            return PartialView("_IndentGrid", MainModel);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC)
        {
            var Result = await _Indent.DeleteByID(ID, YC).ConfigureAwait(false);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> Indent(IndentModel Mainmodel)
        {
            try
            {
                var IndentGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyIndentList");
                IList<IndentDetail> IndentDetail = new List<IndentDetail>();
                if(!string.IsNullOrEmpty(modelJson))
                {
                    IndentDetail = JsonConvert.DeserializeObject<List<IndentDetail>>(modelJson);
                }
  
                if (IndentDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("IndentDetail", "Indent Detail Grid Should Have Atleast 1 Item...!");
                    return View("Indent", Mainmodel);
                }

                else
                {
                    Mainmodel.CC = HttpContext.Session.GetString("Branch");
                    Mainmodel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    Mainmodel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (Mainmodel.Mode == "U")
                    {
                        Mainmodel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        Mainmodel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                        IndentGrid = GetDetailTable(IndentDetail);
                    }
                    else
                    {
                        Mainmodel.CreatedByName = HttpContext.Session.GetString("UID");
                        Mainmodel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        //model.entryByEmpName = HttpContext.Session.GetString("EmpName");
                        IndentGrid = GetDetailTable(IndentDetail);
                    }

                    var Result = await _Indent.SaveIndentDetail(Mainmodel, IndentGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var model1 = new IndentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            model1 = await BindModel(model1);
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyIndentList");
                            return View(model1);
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new IndentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            model1 = await BindModel(model1);
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyIndentList");
                            return View(model1);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            if (Result != null)
                            {
                                input = Result.Result.ToString();
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                                else
                                {
                                    TempData["500"] = "500";
                                }
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            //  _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //model.IsError = "true";
                            //return View("Error", Result);
                        }

                        string modelDataJson = HttpContext.Session.GetString("KeyIndentList");
                        List<IndentDetail> IndentDetailGrid = new List<IndentDetail>();
                        if (!string.IsNullOrEmpty(modelDataJson))
                        {
                            IndentDetailGrid = JsonConvert.DeserializeObject<List<IndentDetail>>(modelDataJson);
                        }

                        Mainmodel.IndentDetails = IndentDetailGrid;
                        Mainmodel = await BindModel(Mainmodel);
                        //ModelState.Clear();
                        return View(Mainmodel);
                    }
                    return View(Mainmodel);
                }
            }
            catch (Exception ex)
            {
                LogException<IndentController>.WriteException(Logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
                //return View(model);
            }
        }

        public async Task<IActionResult> Dashboard(string Flag = "True",string FromDate = "", string ToDate = "", string IndentNo = "", string DeptName = "", string ItemName = "", string PartCode = "", string Searchbox = "", string SummaryDetail = "")
        {
            HttpContext.Session.Remove("KeyIndentList");
            var model = new IndentDashboard();
            DateTime now = DateTime.Now;

            model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

            var Result = await _Indent.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "EntryDate", "YearCode",
                    "IndentNo", "IndentDate", "itemservice", "BOMIND", "BomItemCode", "BOMPartCode", "BOMtem", "IndentorEmpId",
                                "IndentCompleted", "DepartmentId", "DeptName", "Bomqty", "CC",
                                "Approved", "ApprovedbyEmpId", "ApprovedDate", "UID", "IndentRemark",
                                "MRPNO", "MRPEntryId", "canceled", "closed", "firstapproved", "firstapprovedby",
                                "firstapproveddate", "MachineNo");
                model.IndentDashboardGrid = CommonFunc.DataTableToList<IndentDashboard>(DT, "Indent");

            }
            if (Flag != "True")
            {
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.ItemName=ItemName;
                model.PartCode=PartCode;
                model.IndentNo=IndentNo;
                model.DeptName=DeptName;
                model.Searchbox=Searchbox;
                model.SummaryDetail=SummaryDetail;
                return View(model);
            }
            return View(model);
        }

        private static DataTable GetDetailTable(IList<IndentDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("EntryId", typeof(int));
            DTSSGrid.Columns.Add("YearCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("PendReqNo", typeof(string));
            DTSSGrid.Columns.Add("ReqYearCode", typeof(int));
            DTSSGrid.Columns.Add("Specification", typeof(string));
            DTSSGrid.Columns.Add("Qty", typeof(float));
            DTSSGrid.Columns.Add("PendQtyForPO", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("StoreID", typeof(int));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("PenAltQtyForPO", typeof(float));
            DTSSGrid.Columns.Add("ReqDate", typeof(string));
            DTSSGrid.Columns.Add("AccountCode1", typeof(int));
            DTSSGrid.Columns.Add("AccountCode2", typeof(int));
            DTSSGrid.Columns.Add("Model", typeof(string));
            DTSSGrid.Columns.Add("Size", typeof(string));
            DTSSGrid.Columns.Add("Color", typeof(string));
            DTSSGrid.Columns.Add("ItemRemark", typeof(string));
            DTSSGrid.Columns.Add("ReqQty", typeof(float));
            DTSSGrid.Columns.Add("Approvalue", typeof(float));
            DTSSGrid.Columns.Add("ItemDescription", typeof(string));
            DTSSGrid.Columns.Add("Rate", typeof(float));
            //DateTime DeliveryDt = new DateTime();
            DateTime ReqDt = new DateTime();

            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                ReqDt = ParseDate(Item.ReqDate);
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.PendReqNo ?? string.Empty,
                    Item.ReqYearCode,
                    Item.Specification ?? string.Empty,
                    Item.Qty,
                    Item.PendQtyForPO,
                    Item.Unit ?? string.Empty,
                    Item.AltQty,
                    Item.AltUnit ?? string.Empty,
                    Item.StoreID,
                    Item.TotalStock,
                    Item.PendAltQtyForPO,
                    ReqDt == default ? string.Empty : ReqDt.ToString("yyyy/MM/dd"),
                    Item.AccountCode1,
                    Item.AccountCode2 ,
                    Item.Model ?? string.Empty,
                    Item.Size ?? string.Empty,
                    Item.Color ?? string.Empty,
                    Item.ItemRemark ?? string.Empty,
                    Item.ReqQty ,
                    Item.Approvalue,
                    Item.ItemDescription ?? string.Empty,
                    Item.Rate ,
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
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

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _Indent.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillFGPartCode()
        {
            var JSON = await _Indent.FillFGPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }  
        public async Task<JsonResult> FillFGItemName()
        {
            var JSON = await _Indent.FillFGItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }  
        public async Task<JsonResult> FillStoreList()
        {
            var JSON = await _Indent.FillStoreList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillVendorList()
        {
            var JSON = await _Indent.FillVendorList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillDeptList()
        {
            var JSON = await _Indent.FillDeptList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSizeModelColour(int ItemCode)
        {
            var JSON = await _Indent.GetSizeModelColour(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItemCode()
        {
            var JSON = await _Indent.GetItemCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetServiceItems()
        {
            var JSON = await _Indent.GetServiceItems();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
        {
            var JSON = await _Indent.GetAltUnitQty(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTotalStock(int ItemCode, int Storeid)
        {
            var JSON = await _Indent.FillTotalStock(ItemCode, Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomRevNo(int ItemCode)
        {
            var JSON = await _Indent.GetBomRevNo(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomQty(int ItemCode,int BomRevNo)
        {
            var JSON = await _Indent.GetBomQty(ItemCode,BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomChildDetail(int ItemCode, int BomRevNo, int BomQty)
        {
            var JSON = await _Indent.GetBomChildDetail(ItemCode, BomRevNo, BomQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _Indent.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


    }
}
