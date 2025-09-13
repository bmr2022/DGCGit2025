using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using etactwebBOT.Services;

namespace etactwebBOT.Controllers
{
    public class WrenController : Controller
    {
        private readonly WrenAiService _wrenAiService;

        //public WrenController(WrenAiService wrenAiService)
        //{
        //    _wrenAiService = wrenAiService;
        //}
        private readonly IConfiguration _configuration;

        public WrenController(WrenAiService wrenAiService, IConfiguration configuration)
        {
            _wrenAiService = wrenAiService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //public async Task<IActionResult> GenerateSql(string question)
        //{
        //    string metadataJson = System.IO.File.ReadAllText("Data/metadata.json");

        //    string sql = await _wrenAiService.GenerateSqlAsync(question, metadataJson);

        //    ViewBag.GeneratedSql = sql;
        //    return View("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> GenerateSql(string question)
        {
            string metadataJson = System.IO.File.ReadAllText("Data/metadata.json");

            // Get generated SQL from Wren AI
            string sql = await _wrenAiService.GenerateSqlAsync(question, metadataJson);
            string sqlAdjusted = sql.Replace("\"dbo_", "\"");
            string sqlAdjustedFinal = sqlAdjusted
                .Replace("STR_POSITION", "CHARINDEX")
                .Replace("LENGTH", "LEN");

            // Execute generated SQL
            DataTable results = ExecuteSqlQuery(sqlAdjustedFinal);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Convert DataTable to a serializable format
                var resultData = new
                {
                    GeneratedSql = sql,
                    Results = ConvertDataTableToDictionary(results)
                };

                return Json(resultData);
            }
            ViewBag.GeneratedSql = sql;
            ViewBag.Results = results;
            return View();

            // Regular request
            //ViewBag.GeneratedSql = sql;
            //ViewBag.Results = results;
            //return View("Index");
        }

        // Helper method to convert DataTable to a serializable format
        private List<Dictionary<string, object>> ConvertDataTableToDictionary(DataTable dt)
        {
            return dt.AsEnumerable().Select(row =>
                dt.Columns.Cast<DataColumn>().ToDictionary(
                    col => col.ColumnName,
                    col => row[col] == DBNull.Value ? null : row[col]
                )).ToList();
        }
        //    public async Task<IActionResult> GenerateSql(string question)
        //    {
        //        string metadataJson = System.IO.File.ReadAllText("Data/metadata.json");

        //        // Get generated SQL from Wren AI
        //        string sql = await _wrenAiService.GenerateSqlAsync(question, metadataJson);
        //        string sqlAdjusted = sql.Replace("\"dbo_", "\"");
        //        // And similarly for ending quote if needed
        //        string sqlAdjustedFinal = sqlAdjusted
        //.Replace("STR_POSITION", "CHARINDEX")
        //.Replace("LENGTH", "LEN");

        //        // Replace any other non-SQL Server compatible snippets here if needed
        //        // For example, fix the SUBSTRING and CASE statement syntax if needed manually

        //        // Optionally, replace quotes to match SQL Server if needed
        //        // string sqlAdjustedlast = sqlAdjustedFinal.Replace("\"", "[").Replace("\"", "]");
        //        // Add more replacements as needed
        //        ;

        //        // Execute generated SQL
        //        DataTable results = ExecuteSqlQuery(sqlAdjustedFinal);

        //        // Pass SQL and results to ViewBag
        //        ViewBag.GeneratedSql = sql;
        //        ViewBag.Results = results;

        //        return View("Index");
        //    }
        public DataTable ExecuteSqlQuery(string sqlQuery)
        {
            DataTable dt = new DataTable();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


    }
}
