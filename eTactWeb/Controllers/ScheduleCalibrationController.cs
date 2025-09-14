using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class ScheduleCalibrationController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IScheduleCalibration _IScheduleCalibration { get; }
        private readonly ILogger<ScheduleCalibrationController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ScheduleCalibrationController(ILogger<ScheduleCalibrationController> logger, IDataLogic iDataLogic, IScheduleCalibration iScheduleCalibration, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IScheduleCalibration = iScheduleCalibration;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> ScheduleCalibration(int ID, int YC, string Mode)
        {
            var MainModel = new ScheduleCalibrationModel();

            MainModel.CalibSchYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
          
            HttpContext.Session.Remove("KeyScheduleCalibration");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                //MainModel = await _IMaterialConversion.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
               // MainModel.Mode = Mode; 
                //MainModel.EntryId = ID;
              
                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                    MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.CC = HttpContext.Session.GetString("Branch");
                   
                }

                string serializedGrid = JsonConvert.SerializeObject(MainModel.CalibrationScheduleGrid);
                HttpContext.Session.SetString("KeyScheduleCalibration", serializedGrid);
            }

            return View(MainModel);
        }
		[HttpPost]
		public IActionResult StoreCheckedRowsToSession(List<PendingScheduleCalibrationModel> model)
		{
			try
			{
				HttpContext.Session.Remove("KeyScheduleCalibration");
				var sessionData = HttpContext.Session.GetString("KeyScheduleCalibration");
				var PendingDetails = string.IsNullOrEmpty(sessionData)
	? new List<PendingScheduleCalibrationModel>()
	: JsonConvert.DeserializeObject<List<PendingScheduleCalibrationModel>>(sessionData);

				TempData.Clear();

				var MainModel = new ScheduleCalibrationModel();
				var IssueGrid = new List<PendingScheduleCalibrationModel>();
				var SSGrid = new List<PendingScheduleCalibrationModel>();

				var seqNo = 0;
				if (model != null)
				{
					foreach (var item in model)
					{
						if (item != null)
						{
							if (PendingDetails == null)
							{
								item.seqno += seqNo + 1;
								IssueGrid.Add(item);
								seqNo++;
							}
							else
							{

								item.seqno = PendingDetails.Count + 1;
								SSGrid.AddRange(IssueGrid);
								IssueGrid.Add(item);

							}


							HttpContext.Session.SetString("KeyScheduleCalibration", JsonConvert.SerializeObject(IssueGrid));

						}

					}
					
				}
				var sessionGridData = HttpContext.Session.GetString("KeyScheduleCalibration");
				var grid = string.IsNullOrEmpty(sessionGridData)
	? new List<PendingScheduleCalibrationModel>()
	: JsonConvert.DeserializeObject<List<PendingScheduleCalibrationModel>>(sessionGridData);
				var issueDataJson = JsonConvert.SerializeObject(IssueGrid);
				HttpContext.Session.SetString("KeyScheduleCalibration", issueDataJson);


				return Json("done");
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		[Route("{controller}/PendingScheduleCalibration")]
		public async Task<IActionResult> PendingScheduleCalibration()
		{
			var model = new PendingScheduleCalibrationModel();
			model.PendingScheduleCalibrationGrid = new List<PendingScheduleCalibrationModel>();
			//model = await PendingBindModel(model);
			model.FromDate = HttpContext.Session.GetString("FromDate");
			return View(model);
		}
		public async Task<IActionResult> GetScheduleCalibrationSearchData(string PartCode, string ItemName, string ToolCode, string ToolName, int pageNumber = 1,int pageSize = 10,string SearchBox = "")

		{
			var model = new PendingScheduleCalibrationModel();
			model = await _IScheduleCalibration.GetScheduleCalibrationSearchData( PartCode,  ItemName,  ToolCode,  ToolName);

			var modelList = model?.PendingScheduleCalibrationGrid ?? new List<PendingScheduleCalibrationModel>();


			if (string.IsNullOrWhiteSpace(SearchBox))
			{
				model.TotalRecords = modelList.Count();
				model.PageNumber = pageNumber;
				model.PageSize = pageSize;
				model.PendingScheduleCalibrationGrid = modelList
				.Skip((pageNumber - 1) * pageSize)
				   .Take(pageSize)
				   .ToList();
			}
			else
			{
				List<PendingScheduleCalibrationModel> filteredResults;
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
				model.PageNumber = pageNumber;
				model.PageSize = pageSize;
			}
			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpiration = DateTime.Now.AddMinutes(60),
				SlidingExpiration = TimeSpan.FromMinutes(55),
				Size = 1024,
			};

			string serializedGrid = JsonConvert.SerializeObject(modelList);
			HttpContext.Session.SetString("KeyScheduleCalibration", serializedGrid);
			return PartialView("_ScheduleCalibrationDisplayDataDetail", model);
		}

		[HttpGet]
		public IActionResult PendingScheduleCalibrationGlobalSearch(string searchString, int pageNumber = 1, int pageSize = 10)
		{
			PendingScheduleCalibrationModel model = new PendingScheduleCalibrationModel();
			if (string.IsNullOrWhiteSpace(searchString))
			{
				return PartialView("_ScheduleCalibrationDisplayDataDetail", new List<PendingScheduleCalibrationModel>());
			}

			string modelJson = HttpContext.Session.GetString("KeyScheduleCalibration");
			List<PendingScheduleCalibrationModel> gateInwardDashboard = new List<PendingScheduleCalibrationModel>();
			if (!string.IsNullOrEmpty(modelJson))
			{
				gateInwardDashboard = JsonConvert.DeserializeObject<List<PendingScheduleCalibrationModel>>(modelJson);
			}
			if (gateInwardDashboard == null)
			{
				return PartialView("_ScheduleCalibrationDisplayDataDetail", new List<PendingScheduleCalibrationModel>());
			}

			List<PendingScheduleCalibrationModel> filteredResults;

			if (string.IsNullOrWhiteSpace(searchString))
			{
				filteredResults = gateInwardDashboard.ToList();
			}
			else
			{
				filteredResults = gateInwardDashboard
					.Where(i => i.GetType().GetProperties()
						.Where(p => p.PropertyType == typeof(string))
						.Select(p => p.GetValue(i)?.ToString())
						.Any(value => !string.IsNullOrEmpty(value) &&
									  value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
					.ToList();


				if (filteredResults.Count == 0)
				{
					filteredResults = gateInwardDashboard.ToList();
				}
			}
			model.TotalRecords = filteredResults.Count;
			model.PageNumber = pageNumber;
			model.PendingScheduleCalibrationGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
			model.PageSize = pageSize;


			return PartialView("_ScheduleCalibrationDisplayDataDetail", model);
		}

	}
}
