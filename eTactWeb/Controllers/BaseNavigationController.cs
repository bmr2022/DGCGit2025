using Microsoft.AspNetCore.Mvc;

public class BaseNavigationController : Controller
{
    [HttpGet]
    public IActionResult DrillDown(NavigationState state)
    {
        if (state == null)
            return RedirectToAction("Index", "TransactionLedger");

        // Push state to session stack
        NavigationHelper.Push(HttpContext, state);

        // Redirect to target action (no query string needed)
        return RedirectToAction(state.ActionName, state.ControllerName, new {ID = state.ID, YearCode = state.YearCode, AccountCode = state.AccountCode, Mode = state.Mode});
    }
    [HttpGet]
    public IActionResult Back()
    {
        var stack = NavigationHelper.GetStack(HttpContext);

        if (stack.Count <= 1)
        {
            // Only one page left → create a state for TransactionLedger
            var defaultState = new NavigationState
            {
                ControllerName = "TransactionLedger",
                ActionName = "Index",
                FromDate = "2025-10-01", // default or previous value
                ToDate = "2025-10-19",
                ReportType = "Summary",
                GroupOrLedger = null,
                ParentLedger = null,
                VoucherType = null,
                VoucherNo = null,
                INVNo = null,
                Narration = null,
                Amount = null,
                Dr = null,
                Cr = null,
                GlobalSearch = null,
                AccountCode = 0
            };

            // Push default state to stack
            NavigationHelper.Push(HttpContext, defaultState);

            // Redirect to TransactionLedger/Index
            return RedirectToAction("Index", "TransactionLedger");
        }

        // Pop current page
        NavigationHelper.Pop(HttpContext);

        // Peek previous page
        var previous = stack.Peek();
        if (previous != null)
        {
            return RedirectToAction(previous.ActionName, previous.ControllerName);
        }

        // fallback
        return RedirectToAction("Index", "TransactionLedger");
    }

    private string BuildQueryString(NavigationState state)
    {
        var query = new Dictionary<string, string>
        {
            { "FromDate", state.FromDate },
            { "ToDate", state.ToDate },
            { "ReportType", state.ReportType },
            { "GroupOrLedger", state.GroupOrLedger },
            { "LedgerName", state.ParentLedger },
            { "VoucherType", state.VoucherType },
            { "VoucherNo", state.VoucherNo },
            { "INVNo", state.INVNo },
            { "Narration", state.Narration },
            { "Amount", state.Amount },
            { "Dr", state.Dr },
            { "Cr", state.Cr },
            { "GlobalSearch", state.GlobalSearch },
            { "AccountCode", state.AccountCode.ToString() },
            { "YearCode", state.YearCode.ToString() },
            { "ID", state.ID.ToString() },
            { "Mode", state.Mode }
        }.Where(kv => !string.IsNullOrEmpty(kv.Value));

        return string.Join("&", query.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
    }
}
