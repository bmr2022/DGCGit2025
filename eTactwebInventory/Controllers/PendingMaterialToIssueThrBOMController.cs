using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class PendingMaterialToIssueThrBOMController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingMaterialToIssueThrBOM _IPendingMaterialToIssueThrBOM;      
        private readonly IIssueThrBOM _IssueThrBom;             
        private readonly ILogger<PendingMaterialToIssueThrBOMController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingMaterialToIssueThrBOMController(ILogger<PendingMaterialToIssueThrBOMController> logger, IDataLogic iDataLogic, 
            IPendingMaterialToIssueThrBOM IPendingMaterialToIssueThrBOM, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IIssueThrBOM IIssueThrBom)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendingMaterialToIssueThrBOM = IPendingMaterialToIssueThrBOM;            
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            _IssueThrBom = IIssueThrBom;
        }

        public async Task<IActionResult> PendingMaterialToIssueThrBOM()
        {
            ViewData["Title"] = "Pending Requisition to Issue Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyPendingToIssueThrBOM");
            var MainModel = new PendingMaterialToIssueThrBOMModel();
            var model = new IssueWithoutBomDetail();
            MainModel = await BindModel(MainModel);
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyPendingToIssueThrBOM", model, cacheEntryOptions);
            return View(MainModel);
        }

        private async Task<PendingMaterialToIssueThrBOMModel> BindModel(PendingMaterialToIssueThrBOMModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
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

        public async Task<JsonResult> FillRequisition(int ToDept, int ItemCode, int WorkCenter, int YearCode, string ToDate)
        {
            var JSON = await _IPendingMaterialToIssueThrBOM.FillRequisition(ToDept, ItemCode, WorkCenter, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId)
        {
            var JSON = await _IPendingMaterialToIssueThrBOM.ShowDetail(FromDate, ToDate, ReqNo, YearCode, ItemCode, WoNo, WorkCenter, DeptName, ReqYear, IssueDate, GlobalSearch, FromStore, StoreId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
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
        public IActionResult AddissueThrBom(List<IssueThrBomDetail> model)
        {
            //if (model != null)
            //{
            //    foreach (var listItem in model)
            //    {
            //        int YC = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            //        var FinStartDate = HttpContext.Session.GetString("FromDate");
            //        //first get batchno's
            //        //listItem.IssuedDate = ParseFormattedDate(listItem.IssuedDate);
            //        var Batchno = _IssueThrBom.FillBatchUnique(listItem.ItemCode, YC, listItem.StoreName, listItem.BatchNo, listItem.IssuedDate ?? DateTime.Today.ToString(), FinStartDate);
            //        if (Batchno.Result.Result != null)
            //        {
            //            var i = 0;
            //            foreach (var batchList in Batchno.Result.Result.Rows)
            //            {
            //                var checkTransDate = _IPendingMaterialToIssueThrBOM.CheckTransDate(listItem.ItemCode, listItem.IssuedDate, Batchno.Result.Result.Rows[i].ItemArray[1].ToString(), Batchno.Result.Result.Rows[i].ItemArray[2].ToString(), YC);
            //                if (checkTransDate.Result.Result.Tables[0].Rows[0].ItemArray[0] != "Successful")
            //                {
            //                    return Json(checkTransDate.Result.Result.Tables[0].Rows[0].ItemArray[0]);
            //                }
            //                i++;
            //            }
            //        }
            //    }
            //}
            try
            {
                _MemoryCache.Remove("KeyPendingToIssueThrBOM");
                _MemoryCache.TryGetValue("KeyPendingToIssueThrBOM", out IList<IssueThrBomDetail> IssueThrBomDetailGrid);
                TempData.Clear();

                var MainModel = new IssueThrBom();
                var IssueWithoutBomGrid = new List<IssueThrBomDetail>();
                var IssueGrid = new List<IssueThrBomDetail>();
                var SSGrid = new List<IssueThrBomDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (IssueThrBomDetailGrid == null)
                            {
                                item.seqno += seqNo + 1;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (IssueThrBomDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    item.seqno = IssueThrBomDetailGrid.Count + 1;
                                    IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }

                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyPendingToIssueThrBOM", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                _MemoryCache.TryGetValue("KeyPendingToIssueThrBOM", out IList<IssueThrBomDetail> grid);
                _MemoryCache.Set("KeyIssThrBom", MainModel.ItemDetailGrid, cacheEntryOptions);


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<JsonResult> EnableOrDisableIssueDate()
        //{
        //    var JSON = await _IPendingMaterialToIssueThrBOM.EnableOrDisableIssueDate();
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}


    }
}
