using eTactWeb.DOM;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public static class NavigationHelper
{
    private const string DrillDownSessionKey = "DrillDownStack";

    // Push current page state onto the stack
    public static void Push(HttpContext httpContext, NavigationState state)
    {
        var stack = GetStack(httpContext);
        stack.Add(state);
        SaveStack(httpContext, stack);
    }

    // Pop the last page state
    public static NavigationState Pop(HttpContext httpContext)
    {
        var stack = GetStack(httpContext);
        if (stack.Count == 0) return null;

        var state = stack.Last();
        stack.RemoveAt(stack.Count - 1);
        SaveStack(httpContext, stack);
        return state;
    }

    // Peek the current page state (without removing)
    public static NavigationState Peek(HttpContext httpContext)
    {
        var stack = GetStack(httpContext);
        return stack.LastOrDefault();
    }

    // Clear the stack completely
    public static void Clear(HttpContext httpContext)
    {
        httpContext.Session.Remove(DrillDownSessionKey);
    }

    // ===== Private Helpers =====
    private static List<NavigationState> GetStack(HttpContext httpContext)
    {
        var json = httpContext.Session.GetString(DrillDownSessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<NavigationState>()
            : JsonConvert.DeserializeObject<List<NavigationState>>(json);
    }

    private static void SaveStack(HttpContext httpContext, List<NavigationState> stack)
    {
        httpContext.Session.SetString(DrillDownSessionKey, JsonConvert.SerializeObject(stack));
    }
}
