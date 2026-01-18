using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace eTactWeb.Data.Common
{
    public class ConnectionStringService
    {
        public string DBConnectionString { get; private set; }
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        public ConnectionStringService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetConnectionString(string connectionString)
        {
            DBConnectionString = connectionString;
        }

        public string GetConnectionString()
        {
            // 1️⃣ Try from Session (per user)
            var connStr = _httpContextAccessor.HttpContext?
                                .Session
                                .GetString("DB_CONN");

            // 2️⃣ Fallback (optional safety)
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = _configuration.GetConnectionString("eTactDB");
            }

            return connStr;
        }
    }
}
