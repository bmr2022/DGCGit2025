using eTactWeb.DOM;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class NavigationHelper
{
    private const string DrillDownSessionKey = "DrillDownStack";

    // Push current page state onto the stack
    public static void Push(HttpContext httpContext, NavigationState state)
    {
        var stack = GetStack(httpContext);
        stack.Add(state);
        httpContext.Session.SetString(DrillDownSessionKey, JsonConvert.SerializeObject(stack));
    }

    // Pop the last page state from the stack
    public static NavigationState Pop(HttpContext httpContext)
    {
        var stack = GetStack(httpContext);
        if (stack.Count == 0) return null;

        var state = stack[stack.Count - 1];
        stack.RemoveAt(stack.Count - 1);

        httpContext.Session.SetString(DrillDownSessionKey, JsonConvert.SerializeObject(stack));
        return state;
    }
    private static List<NavigationState> GetStack(HttpContext httpContext)
    {
        var json = httpContext.Session.GetString(DrillDownSessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<NavigationState>()
            : JsonConvert.DeserializeObject<List<NavigationState>>(json);
    }

    // Optional: Clear the whole stack
    public static void Clear(HttpContext httpContext)
    {
        httpContext.Session.Remove(DrillDownSessionKey);
    }
}
