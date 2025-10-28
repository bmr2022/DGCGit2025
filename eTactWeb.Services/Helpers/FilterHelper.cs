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
        public static void SaveFilters(HttpContext context, string key, FilterState filters)
        {
            var json = JsonConvert.SerializeObject(filters);
            context.Session.SetString(key, json);
        }

        public static FilterState GetFilters(HttpContext context, string key)
        {
            var json = context.Session.GetString(key);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<FilterState>(json);
        }
    }
}
