using DocumentFormat.OpenXml.Office2010.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PdfSharp.Drawing.BarCodes;
using static eTactWeb.DOM.Models.Common;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace eTactWeb.Controllers
{
    public class CustSaleNewRateController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICustSaleNewRate _custSaleNewRate { get; }
        private readonly ILogger<CustSaleNewRateController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CustSaleNewRateController(ILogger<CustSaleNewRateController> logger, IDataLogic iDataLogic, ICustSaleNewRate iCustSaleNewRate, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _custSaleNewRate = iCustSaleNewRate;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<IActionResult> CustSaleNewRate(int ID, string Mode, int YC)
        {
            var model = new CustomerSaleRateMasterModel();
           
            model.CNRMYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

           
           

            model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            
            HttpContext.Session.Remove("KeyIssueNRGPGrid");
            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
            if (Mode == "U")
            {
                model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
               
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                //model = await _IIssueNRGP.GetViewByID(ID, YC, Mode).ConfigureAwait(true);

                //model.Mode = Mode;
                //model.YearCode = YC;
                //model.EntryTime = model.EntryTime;
                //model = await BindModel(model);

                //model.ID = ID;

                //if (model.IssueNRGPDetailGrid?.Count != 0 && model.IssueNRGPDetailGrid != null)
                //{
                //    HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(model.IssueNRGPDetailGrid));
                //}

                //if (model.IssueNRGPTaxGrid != null)
                //{
                //    //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //    //{
                //    //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                //    //    SlidingExpiration = TimeSpan.FromMinutes(55),
                //    //    Size = 1024,
                //    //};
                //    HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(model.IssueNRGPDetailGrid));
                //    HttpContext.Session.SetString("KeyIssueNRGPTaxGrid", JsonConvert.SerializeObject(model.IssueNRGPTaxGrid));
                //    string modelNRGPJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                //    List<IssueNRGPDetail> TaxGrid11 = new List<IssueNRGPDetail>();
                //    if (string.IsNullOrEmpty(modelNRGPJson))
                //    {
                //        TaxGrid11 = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelNRGPJson);
                //    }
                //}
            }
            else
            {
                
            }
            //HttpContext.Session.SetString("IssueNRGP", JsonConvert.SerializeObject(model));
            //string modelJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
            //List<IssueNRGPDetail> TaxGrid = new List<IssueNRGPDetail>();
            //if (!string.IsNullOrEmpty(modelJson))
            //{
            //    TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
            //}
           
            return View(model);



           
        }

        public async Task<JsonResult> AutoFillPartCode( string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _custSaleNewRate.AutoFillitem("AutoFillPartCode", SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillItemName( string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _custSaleNewRate.AutoFillitem("AutoFillItemName", SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}
