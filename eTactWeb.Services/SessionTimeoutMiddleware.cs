using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services
{
    public class SessionCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            // Exclude login page from session check
            if (path == "/" || path.Contains("/login") || path.StartsWith("/getservername") || path.StartsWith("/home/getbranchname") || path.StartsWith("/home/getyearcode"))
            {
                await _next(context);
                return;
            }

            // Check if the session is active
            if (context.Session.GetString("UID") == null)
            {
                context.Response.Redirect("/");
                return;
            }

            await _next(context);
        }
    }
}
