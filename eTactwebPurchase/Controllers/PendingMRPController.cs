using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Caching;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class PendingMRPController : Controller
    {
        public IDataLogic IDataLogic { get; }
        public IMRP IMRP { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<PendingMRPController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        public PendingMRPController(IMRP _IMRP, IDataLogic iDataLogic, ILogger<PendingMRPController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            IMRP = _IMRP;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> PendingMRP()
        {
            HttpContext.Session.Remove("KeyPendingMRPDetail");
            HttpContext.Session.Remove("KeyMRPSaleOrderDetail");
            var model = new PendingMRP();
            //  var MRPSOList = new List<MRPSaleOrderDetail>();
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.Month = now.Month;
            var Result = await IMRP.PendingMRPData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "sono", "SODate",
                                "SOENtryId", "SOyearCode", "SOTYPE", "ScheduleNo", "schYearCode", "accountcode", "schEntryID", "sch_date",
                                "account_name", "orderQty", "FGItem", "FGstock", "SaleRate", "IIndMonthQty",
                                "IIIrdMonthQty", "BOM");

                model.PendingMRPGrid = CommonFunc.DataTableToList<PendingMRP>(DT, "PendingMRP");

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                //var SODetailData = GetMRPSOList(model.PendingMRPGrid);

                //MRPSOList.AddRange(SODetailData);
                //IMemoryCache.Set("KeyMRPSaleOrderDetail", MRPSOList, cacheEntryOptions);
            }

            return View(model);
        }

        public async Task<IActionResult> FillMRPSOList(int ForMonth, int YearCode)
        {
            var model = new PendingMRP();
            var MRPSOList = new List<MRPSaleOrderDetail>();
            model.Month = ForMonth;
            model.YearCode = YearCode;
            var Result = await IMRP.PendingMRPData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "sono", "SODate",
                                "SOENtryId", "SOyearCode", "SOTYPE", "ScheduleNo", "schYearCode", "accountcode", "schEntryID", "sch_date",
                                "account_name", "orderQty", "FGItem", "FGstock", "SaleRate", "IIndMonthQty",
                                "IIIrdMonthQty", "BOM");

                model.PendingMRPGrid = CommonFunc.DataTableToList<PendingMRP>(DT, "PendingMRP");


                var SODetailData = GetMRPSOList(model.PendingMRPGrid);

                MRPSOList.AddRange(SODetailData);
                HttpContext.Session.SetString("KeyMRPSaleOrderDetail", JsonConvert.SerializeObject(MRPSOList));
            }

            return View(model);
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

        public List<MRPSaleOrderDetail> GetMRPSOList(List<PendingMRP> model)
        {
            List<MRPSaleOrderDetail> MRPSO = new List<MRPSaleOrderDetail>();
            foreach (var item in model)
            {
                MRPSO.Add(new MRPSaleOrderDetail
                {
                    SONo = item.sono,
                    SOYearCode = item.SOYearCode,
                    SODAte = item.SODate,
                    ScheduleNo = item.ScheduleNo,
                    SchYearCode = item.schYearCode,
                    AccountCode = item.accountcode,
                    AccountName = item.account_name,
                    BOMExist = item.BOM

                });
            }

            return MRPSO;
        }

        public async Task<IActionResult> GetFilteredData(int Month, int YearCode, string IncludeProjection, int AccountCode = 0, string SONO = "", int SOYearCode = 0, string schNo = "")
        {
            var mainmodel = new PendingMRP();
            mainmodel.Month = Month;
            mainmodel.YearCode = YearCode;
            mainmodel.accountcode = AccountCode;
            mainmodel.sono = SONO;
            mainmodel.SOYearCode = SOYearCode;
            mainmodel.ScheduleNo = schNo;
            mainmodel.IncludeProjection = IncludeProjection;

            var Result = await IMRP.PendingMRPData(mainmodel);
            DataSet DS = Result.Result;
            var DT = DS.Tables[0].DefaultView.ToTable(true, "sono", "SODate",
                                 "SOENtryId", "SOyearCode", "SOTYPE", "ScheduleNo", "schYearCode", "accountcode", "schEntryID", "sch_date",
                                 "account_name", "orderQty", "FGItem", "FGstock", "SaleRate", "IIndMonthQty",
                                 "IIIrdMonthQty", "BOM");

            mainmodel.PendingMRPGrid = CommonFunc.DataTableToList<PendingMRP>(DT, "PendingMRP");

            return PartialView("_PendingMRPGrid", mainmodel);
        }

        public async Task<JsonResult> GetStore()
        {
            var JSON = await IMRP.GetStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenter()
        {
            var JSON = await IMRP.GetWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> IsCheckMonthWiseData(int Month, int Year)
        {
            var JSON = await IMRP.IsCheckMonthWiseData(Month, Year);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearPendingMRPGrid()
        {
            HttpContext.Session.Remove("KeyPendingMRPDetail");
            var JSON = await IMRP.GetWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GenerateMRP(List<PendingMRP> model)
        {
            try
            {
                var MainModel = new PendingMRP();
                var PendingMRP = new List<PendingMRP>();
                var PMRPGrid = new List<PendingMRP>();
                var Mainmodel = new MRPMain();

                var SeqNo = 0;
                var ListOfMRPGrid = new List<MRPDetail>();
                var ListOfMRPFGRMGrid = new List<MRPFDRMDetail>();
                // List<PendingMRP> distinctModel = model.Distinct().ToList();
                foreach (var item in model)
                {
                    var DetailData = await IMRP.GetMRPDetailData(item);
                    var FGRMDetailData = await IMRP.GetMRPFGRMDetailData(item);
                    if (DetailData.MRPGrid != null)
                    {
                        ListOfMRPGrid.AddRange(DetailData.MRPGrid);
                    }

                    if (FGRMDetailData.MRPFGRMGrid != null)
                    {
                        ListOfMRPFGRMGrid.AddRange(FGRMDetailData.MRPFGRMGrid);
                    }

                    HttpContext.Session.SetString("KeyPendingMRPToMRPDetail", JsonConvert.SerializeObject(ListOfMRPGrid));
                    HttpContext.Session.SetString("KeyPendingFGRMMRP", JsonConvert.SerializeObject(ListOfMRPFGRMGrid));
                }


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}

