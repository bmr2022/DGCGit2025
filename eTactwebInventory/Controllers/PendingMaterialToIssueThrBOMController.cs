using ClosedXML.Excel;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Data;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class PendingMaterialToIssueThrBOMController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingMaterialToIssueThrBOM _IPendingMaterialToIssueThrBOM;
        private readonly IIssueThrBOM _IssueThrBom;
        private readonly ILogger<PendingMaterialToIssueThrBOMController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        public PendingMaterialToIssueThrBOMController(ILogger<PendingMaterialToIssueThrBOMController> logger, IDataLogic iDataLogic,
           IConfiguration iconfiguration, IPendingMaterialToIssueThrBOM IPendingMaterialToIssueThrBOM, IWebHostEnvironment iWebHostEnvironment, IIssueThrBOM IIssueThrBom)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendingMaterialToIssueThrBOM = IPendingMaterialToIssueThrBOM;
            _IWebHostEnvironment = iWebHostEnvironment;
            _IssueThrBom = IIssueThrBom;
            _iconfiguration = iconfiguration;
        }

        public async Task<IActionResult> PendingMaterialToIssueThrBOM(string formKey)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            ViewData["Title"] = "Pending Requisition to Issue Details";
            //TempData.Clear();
            HttpContext.Session.Remove($"KeyPendingToIssueThrBOM_{uniqueKey}");
            var MainModel = new PendingMaterialToIssueThrBOMModel();
            var model = new IssueWithoutBomDetail();
            MainModel = await BindModel(MainModel, formKey);
            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            //MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
            HttpContext.Session.SetString($"KeyPendingToIssueThrBOM_{uniqueKey}", JsonConvert.SerializeObject(model));
            return View(MainModel);
        }

        private async Task<PendingMaterialToIssueThrBOMModel> BindModel(PendingMaterialToIssueThrBOMModel model, string formKey)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            oDataSet = await _IPendingMaterialToIssueThrBOM.BindAllDropDowns("BINDDATA", YearCode).ConfigureAwait(true);
            model.ReqList = _List;
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["REQNO"].ToString()
                    });
                }
                model.ReqList = _List;

            }
            var oDataSet1 = new DataSet();
            oDataSet1 = await _IPendingMaterialToIssueThrBOM.BindAllDropDowns("BINDPARTCODE", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            model.PartCodeList = _List;

            if (oDataSet1.Tables.Count > 0 && oDataSet1.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet1.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.PartCodeList = _List;

            }
            var oDataSet2 = new DataSet();

            oDataSet2 = await _IPendingMaterialToIssueThrBOM.BindAllDropDowns("BindItemName", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            model.ItemList = _List;

            if (oDataSet2.Tables.Count > 0 && oDataSet2.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet2.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.ItemList = _List;

            }
            var oDataSet3 = new DataSet();

            oDataSet3 = await _IPendingMaterialToIssueThrBOM.BindAllDropDowns("BindIssueToDepartment", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            model.DeptList = _List;

            if (oDataSet3.Tables.Count > 0 && oDataSet3.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet3.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptList = _List;

            }
            var oDataSet4 = new DataSet();

            oDataSet4 = await _IPendingMaterialToIssueThrBOM.BindAllDropDowns("BinDworkcenetr", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            model.WorkCenterList = _List;

            if (oDataSet4.Tables.Count > 0 && oDataSet4.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet4.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["WorkCenterDescription"].ToString()
                    });
                }
                model.WorkCenterList = _List;

            }
            return model;
        }

        public async Task<JsonResult> FillRequisition(int ToDept, int ItemCode, int WorkCenter, int YearCode, string ToDate, string formKey)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            var JSON = await _IPendingMaterialToIssueThrBOM.FillRequisition(ToDept, ItemCode, WorkCenter, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> ShowDetail(string formKey, string Flag, string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            var JSON = await _IPendingMaterialToIssueThrBOM.ShowDetail(Flag, FromDate, ToDate, ReqNo, YearCode, ItemCode, WoNo, WorkCenter, DeptName, ReqYear, IssueDate, GlobalSearch, FromStore, StoreId);
            string JsonString = JsonConvert.SerializeObject(JSON);

            return Json(JsonString);
            //return Json(JSON);
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
        //public IActionResult AddissueThrBom(List<IssueThrBomDetail> model)
        //{

        //    try
        //    {
        //        HttpContext.Session.Remove($"KeyPendingToIssueThrBOM_{uniqueKey}");
        //        string serializedData = HttpContext.Session.GetString($"KeyPendingToIssueThrBOM_{uniqueKey}");
        //        List<IssueThrBomDetail> IssueThrBomDetailGrid = new List<IssueThrBomDetail>();
        //        if (!string.IsNullOrEmpty(serializedData))
        //        {
        //            IssueThrBomDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(serializedData);
        //        }
        //        TempData.Clear();

        //        var MainModel = new IssueThrBom();
        //        var IssueWithoutBomGrid = new List<IssueThrBomDetail>();
        //        var IssueGrid = new List<IssueThrBomDetail>();
        //        var SSGrid = new List<IssueThrBomDetail>();

        //        var seqNo = 0;
        //        if (model != null)
        //        {
        //            foreach (var item in model)
        //            {
        //                if (item != null)
        //                {
        //                    if (IssueThrBomDetailGrid == null)
        //                    {
        //                        item.seqno += seqNo + 1;
        //                        IssueGrid.Add(item);
        //                        seqNo++;
        //                    }
        //                    else
        //                    {
        //                        if (IssueThrBomDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
        //                        {
        //                            return StatusCode(207, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            item.seqno = IssueThrBomDetailGrid.Count + 1;
        //                            //IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
        //                            SSGrid.AddRange(IssueGrid);
        //                            IssueGrid.Add(item);
        //                        }
        //                    }

        //                    MainModel.ItemDetailGrid = IssueGrid;

        //                    HttpContext.Session.SetString($"KeyPendingToIssueThrBOM_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
        //                }
        //            }
        //        }
        //        string modelJson = HttpContext.Session.GetString($"KeyPendingToIssueThrBOM_{uniqueKey}");
        //        List<IssueThrBomDetail> grid = new List<IssueThrBomDetail>();
        //        if (!string.IsNullOrEmpty(modelJson))
        //        {
        //            grid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
        //        }

        //        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
        //        HttpContext.Session.SetString("KeyIssThrBom", serializedGrid);


        //        return Json("done");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IActionResult AddissueThrBom(List<IssueThrBomDetail> model, string formKey, string uniqueKey)
        {
            try
            {
                //string serializedData = HttpContext.Session.GetString($"KeyPendingToIssueThrBOM_{uniqueKey}");

                //List<IssueThrBomDetail> IssueThrBomDetailGrid = new List<IssueThrBomDetail>();

                //if (!string.IsNullOrEmpty(serializedData))
                //{
                //    IssueThrBomDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(serializedData);
                //}

                TempData.Clear();

                var MainModel = new IssueThrBom();
                var IssueGrid = new List<IssueThrBomDetail>();
                var seqNo = 0;

                //if (model != null)
                //{
                //    foreach (var item in model)
                //    {
                //        if (item != null)
                //        {

                //                item.seqno = ++seqNo;
                //                IssueGrid.Add(item);
                //            //}
                //            //else
                //            //{
                //            //    if (IssueThrBomDetailGrid.Any(x => x.ItemCode == item.ItemCode))
                //            //    {
                //            //        return StatusCode(207, "Duplicate");
                //            //    }
                //            //    else
                //            //    {
                //            //        item.seqno = IssueThrBomDetailGrid.Count + 1;
                //            //        IssueGrid = IssueThrBomDetailGrid;
                //            //        IssueGrid.Add(item);
                //            //    }
                //            //}
                //        }
                //    }
                //}

                MainModel.ItemDetailGrid = model;

                // ✅ Session Save Only Once (Moved Outside Loop)
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString($"KeyPendingToIssueThrBOM_{uniqueKey}", serializedGrid);
                HttpContext.Session.SetString($"KeyIssThrBom_{uniqueKey}", serializedGrid);

                return Json("done");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> ExportToExcel(
            string formKey,
            string Flag,
            string FromDate,
            string ToDate,
            string ReqNo,
            int YearCode,
            int ItemCode,
            string WoNo,
            int WorkCenter,
            int DeptName,
            int ReqYear,
            string IssueDate,
            string GlobalSearch,
            string FromStore,
            int StoreId)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));

            var result = await _IPendingMaterialToIssueThrBOM.ShowDetail(
                Flag,
                FromDate,
                ToDate,
                ReqNo,
                YearCode,
                ItemCode,
                WoNo,
                WorkCenter,
                DeptName,
                ReqYear,
                IssueDate,
                GlobalSearch,
                FromStore,
                StoreId);

            DataTable dt = result.Result.Tables[0];

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Pending Material");

                // Header
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cells[1, i + 1].Value = dt.Columns[i].ColumnName;
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Data
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        ws.Cells[r + 2, c + 1].Value = dt.Rows[r][c];
                    }
                }

                ws.Cells.AutoFitColumns();

                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "PendingMaterial.xlsx");
            }
        }
    }
}
