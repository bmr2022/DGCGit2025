using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Helpers
{
    public static class EncodeHelper
    {
        public static string ToBase64(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }
}
