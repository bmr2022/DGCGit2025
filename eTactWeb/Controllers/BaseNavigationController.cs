using Microsoft.AspNetCore.Mvc;

public class BaseNavigationController : Controller
{
    [HttpGet]
    public IActionResult DrillDown(NavigationState state)
    {
        if (state == null)
            return RedirectToAction("Index", "TransactionLedger");

        // Define your root page (controller + action)
        const string rootController = "TransactionLedger";
        const string rootAction = "Index";

        // Push the navigation state into session stack
        NavigationHelper.Push(HttpContext, state);

        // Check if the target is NOT the root page
        bool isRootPage = string.Equals(state.ControllerName, rootController, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(state.ActionName, rootAction, StringComparison.OrdinalIgnoreCase);

        if (isRootPage)
        {
            // Stay on the same page — no redirect
            return View(rootAction);
        }
        else
        {
            // Drill down into target page
            return RedirectToAction(state.ActionName, state.ControllerName, new
            {
                ID = state.ID,
                YearCode = state.YearCode,
                AccountCode = state.AccountCode,
                Mode = state.Mode
            });
        }
    }
    [HttpGet]
    public IActionResult GoBack()
    {
        var previous = NavigationHelper.Pop(HttpContext);
        if (previous == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Always redirect to root page (TransactionLedger Index)
        string rootController = "TransactionLedger";
        string rootAction = "Index";

        // Keep only filters
        var filterValues = new
        {
            FromDate = previous.FromDate,
            ToDate = previous.ToDate,
            ReportType = previous.ReportType,
            GroupOrLedger = previous.GroupOrLedger,
            ParentLedger = previous.ParentLedger,
            AccountCode = previous.AccountCode,
            VoucherType = previous.VoucherType,
            VoucherNo = previous.VoucherNo,
            INVNo = previous.INVNo,
            Narration = previous.Narration,
            Amount = previous.Amount,
            Dr = previous.Dr,
            Cr = previous.Cr,
            GlobalSearch = previous.GlobalSearch
        };

        return RedirectToAction(rootAction, rootController, filterValues);
    }
}
