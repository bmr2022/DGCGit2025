using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class MIRRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMIRRegister _IMIRRegister { get; }

        private readonly ILogger<MIRRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MIRRegisterController(ILogger<MIRRegisterController> logger, IDataLogic iDataLogic, IMIRRegister iMIRRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMIRRegister = iMIRRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult MIRRegister()
        {
            var model = new MIRRegisterModel();
            model.MIRRegisterDetail = new List<MIRRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName,string MRNStatus, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new MIRRegisterModel();
            if (string.IsNullOrEmpty(gateno)||gateno == "0" )
                {                gateno = "";            }
            if (string.IsNullOrEmpty(MRNno) || MRNno == "0")
            { MRNno = ""; }
            if (string.IsNullOrEmpty(docname) || docname == "0")
            { docname = ""; }
            if (string.IsNullOrEmpty(PONo) || PONo == "0")
            { PONo = ""; }
            if (string.IsNullOrEmpty(Schno) || Schno == "0")
            { Schno = ""; }
            if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
            { PartCode = ""; }
            if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
            { ItemName = ""; }
            if (string.IsNullOrEmpty(invoiceNo) || invoiceNo == "0")
            { invoiceNo = ""; }
            if (string.IsNullOrEmpty(VendorName) || VendorName == "0")
            { VendorName = ""; }
       
            model = await _IMIRRegister.GetRegisterData(MRNType,ReportType,  FromDate,  ToDate,  gateno,  MRNno,docname,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName,MRNStatus);
            model.ReportMode= ReportType;
            var modelList = model?.MIRRegisterDetail ?? new List<MIRRegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.MIRRegisterDetail = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<MIRRegisterDetail> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.MIRRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyMIRRegisterList", modelList, cacheEntryOptions);
            return PartialView("_MIRRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            MIRRegisterModel model = new MIRRegisterModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_MIRRegisterGrid", new List<MIRRegisterDetail>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyMIRRegisterList", out IList<MIRRegisterDetail> mIRRegisterDetail) || mIRRegisterDetail == null)
            {
                return PartialView("_MIRRegisterGrid", new List<MIRRegisterDetail>());
            }

            List<MIRRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = mIRRegisterDetail.ToList();
            }
            else
            {
                filteredResults = mIRRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = mIRRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.MIRRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportMode == "vendorItemWiseSummary")
            {
                return PartialView("_MIRRegisterVendorItemWiseSummary", model);
            }
            else if (model.ReportMode == "POBATCHWISEDETAIL")
            {
                return PartialView("_MRNRegisterVendorItemConsolidated", model);
            }
            else if (model.ReportMode == "PPMRating")
            {
                return PartialView("_MIRRegisterPPM", model);
            }
            else if (model.ReportMode == "vendorWiseConsolidated")
            {
                return PartialView("_MRNRegisterVendorWiseConsolidated", model);
            }
            else if (model.ReportMode == "DAYWISEMIRENTRYLIST")
            {
                return PartialView("_MRNRegisterDayWiseList", model);
            }
            else if (model.ReportMode == "PENDMRNFORQC(SUMMARY)")
            {
                return PartialView("_MIRRegisterPendMRNForQCSummary", model);
            }
            else if (model.ReportMode == "PENDMRNFORQC(Detail)")
            {
                return PartialView("_MIRRegisterPendMRNForQCDetail", model);
            }
            else if (model.ReportMode == "vendorItemRejectionSummary")
            {
                return PartialView("_MIRRegisterVendorItemRejectionSummary", model);
            }
            else if (model.ReportMode == "ItemWiseSummary")
            {
                return PartialView("_MIRRegisterItemWiseSummary", model);
            }
            else if (model.ReportMode == "MRNWiseSummary")
            {
                return PartialView("_MRNWiseSummary", model);
            }
            else
            {
                return PartialView("_MIRRegisterGrid", model);
            }
        }
        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMIRNo(string FromDate, string ToDate)
        {

            var JSON = await _IMIRRegister.FillMIRNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemPartcode(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemPartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillSchNo(  FromDate,  ToDate);
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
    }
}
