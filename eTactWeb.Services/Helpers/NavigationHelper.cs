using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class NavigationHelper
{
    private const string SessionKey = "NavigationStack";

    public static Stack<NavigationState> GetStack(HttpContext context)
    {
        var json = context.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return new Stack<NavigationState>();

        var list = JsonConvert.DeserializeObject<List<NavigationState>>(json);
        return new Stack<NavigationState>(list.Reverse<NavigationState>()); // preserve order
    }

    public static void SaveStack(HttpContext context, Stack<NavigationState> stack)
    {
        var list = stack.ToList();
        var json = JsonConvert.SerializeObject(list);
        context.Session.SetString(SessionKey, json);
    }

    public static void Push(HttpContext context, NavigationState state)
    {
        var stack = GetStack(context);

        // Avoid duplicate push
        if (stack.Count == 0 || !stack.Peek().Equals(state))
        {
            stack.Push(state);
            SaveStack(context, stack);
        }
    }

    public static NavigationState Pop(HttpContext context)
    {
        var stack = GetStack(context);
        if (stack.Count == 0) return null;

        var popped = stack.Pop();
        SaveStack(context, stack);
        return popped;
    }

    public static void Clear(HttpContext context)
    {
        context.Session.Remove(SessionKey);
    }
}
