using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.Common
{
    public class CustomActionFilter : ActionFilterAttribute
    {
        public ILogger _logger;

        public CustomActionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            LogException<CustomActionFilter>.WriteLog(_logger, null);
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LogException<CustomActionFilter>.WriteLog(_logger, context);
            base.OnActionExecuting(context);
        }
    }

    public class LoggerInfo
    {
        public LoggerInfo(IHttpContextAccessor httpContextAccessor)
        {
            LocalPort = httpContextAccessor.HttpContext.Connection.LocalPort.ToString();
            LocalIPAddress = httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString();
            RemotePort = httpContextAccessor.HttpContext.Connection.RemotePort.ToString();
            RemoteIPAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public string? IPEndPoint { get; set; }
        public bool IsDebug { get; set; }
        public string? LocalIPAddress { get; set; }
        public string? LocalPort { get; set; }
        public string? RemoteIPAddress { get; set; }
        public string? RemotePort { get; set; }
    }
}