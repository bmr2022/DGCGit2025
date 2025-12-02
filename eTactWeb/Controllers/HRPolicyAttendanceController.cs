using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class HRPolicyAttendanceController : Controller
    {
        private readonly IHRPolicyAttendanceBLL _hrPolicyAttendanceBLL;

        public HRPolicyAttendanceController(IHRPolicyAttendanceBLL hrPolicyAttendanceBLL)
        {
            _hrPolicyAttendanceBLL = hrPolicyAttendanceBLL;
        }
        [HttpGet]
        public async Task<IActionResult> HRPolicyAttendance(int policyId = 1)
        {
            var result = await _hrPolicyAttendanceBLL.GetByIdHRPolicyAttendanc(policyId);
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> HRPolicyAttendance(HRPolicyAttendanceModel model)
        {
            var result = await _hrPolicyAttendanceBLL.SaveHRPolicyAttendance(model);

            if (result != null)
            {
                TempData["Success"] = "Policy saved successfully!";
                return RedirectToAction("HRPolicyAttendance");
            }

            return View();
        }
    }
}
