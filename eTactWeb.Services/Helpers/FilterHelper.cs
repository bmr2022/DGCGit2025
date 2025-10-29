using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Helpers
{
    public static class FilterHelper
    {
        public static void SaveFilters(HttpContext context, string key, ReportFilter filters)
        {
            context.Session.SetString(key, JsonConvert.SerializeObject(filters));
        }

        public static ReportFilter? GetFilters(HttpContext context, string key)
        {
            var json = context.Session.GetString(key);
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<ReportFilter>(json);
        }

        public static void ClearFilters(HttpContext context, string key)
        {
            context.Session.Remove(key);
        }
    }
}
