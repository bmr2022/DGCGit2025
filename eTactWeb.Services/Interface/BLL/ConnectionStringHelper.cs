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

        public ConnectionStringHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetConnectionStringForCompany()
        {
            var databaseName = _httpContextAccessor.HttpContext.Session.GetString("databaseName") == null ? string.Empty : _httpContextAccessor.HttpContext.Session.GetString("databaseName");
            var _baseConnectionString = _configuration.GetConnectionString("eTactDB");
            var builder = new SqlConnectionStringBuilder(_baseConnectionString)
            {
                InitialCatalog = databaseName 
            };
            return builder.ConnectionString;

        }
    }

}
