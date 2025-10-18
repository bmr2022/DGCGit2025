using eTactWeb.DOM;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class NavigationHelper
{
    private const string DrillDownSessionKey = "DrillDownStack";

    public static void Push(HttpContext httpContext, NavigationState state)
    {
        var stack = GetStack(httpContext);

        // Avoid consecutive duplicates
        if (stack.Count == 0 || !stack.Last().Equals(state))
            stack.Add(state);

        SaveStack(httpContext, stack);
    }

    public static NavigationState Pop(HttpContext httpContext)
    {
        var stack = GetStack(httpContext);
        if (stack.Count == 0) return null;

        var state = stack.Last();
        stack.RemoveAt(stack.Count - 1);
        SaveStack(httpContext, stack);
        return state;
    }

    public static NavigationState Peek(HttpContext httpContext)
    {
        return GetStack(httpContext).LastOrDefault();
    }

    public static List<NavigationState> GetStack(HttpContext httpContext)
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

    public static void Clear(HttpContext httpContext)
    {
        httpContext.Session.Remove(DrillDownSessionKey);
    }
}
