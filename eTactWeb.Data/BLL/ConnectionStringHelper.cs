using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Runtime.Caching;

namespace eTactWeb.Data.BLL
{
    public class ConnectionStringHelper : IConnectionStringHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        public ConnectionStringHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
        }
        public string GetConnectionStringForCompany()
        {
            var databaseName = _httpContextAccessor.HttpContext.Session.GetString("databaseName") == null ? string.Empty : _httpContextAccessor.HttpContext.Session.GetString("databaseName");
            //var _baseConnectionString = _configuration.GetConnectionString("eTactDB");
            var _baseConnectionString = _connectionStringService.GetConnectionString();
            var builder = new SqlConnectionStringBuilder(_baseConnectionString)
            {
                InitialCatalog = databaseName 
            };
            return builder.ConnectionString;

        }
    }

}
