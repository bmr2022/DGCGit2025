using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.Common
{
    public class ConnectionStringService
    {
        public string DBConnectionString { get; private set; }
        private readonly IConfiguration _configuration;

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
            return DBConnectionString;
        }
    }
}
