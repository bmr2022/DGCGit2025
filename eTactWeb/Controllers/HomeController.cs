using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Newtonsoft.Json;
using ClosedXML.Excel;
using eTactWeb.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace eTactWeb.Controllers;

public class HomeController : Controller
{
    private readonly IDataLogic _IDataLogic;
    public IDashboard _IDashboard { get; set; }
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly EncryptDecrypt _EncryptDecrypt;
    private readonly IConnectionStringHelper _connectionStringHelper;
    private readonly UserContextService _userContextService;
    private readonly ConnectionStringService _connectionStringService;
    private string CC;
    private bool IsDrOpen = false;
    private string sql = string.Empty;
    private string year_code;
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    public HomeController(IWebHostEnvironment iWebHostEnvironment, IConfiguration config, ILogger<HomeController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IConnectionStringHelper connectionStringHelper, UserContextService userContextService, ConnectionStringService connectionStringService,IDashboard IDashboard)
    {
        _logger = logger;
        this._IDataLogic = iDataLogic;
        _configuration = config;
        _EncryptDecrypt = encryptDecrypt;
        _connectionStringHelper = connectionStringHelper;
        _userContextService = userContextService;
        _connectionStringService = connectionStringService;
        _IDashboard = IDashboard;
        _IWebHostEnvironment = iWebHostEnvironment;
    }
    [HttpPost]
    public JsonResult AutoComplete(string Schema, string ColName, string prefix, string FromDate = "", string ToDate = "", int ItemCode = 0, int Storeid = 0)
    {
        IList<Common.TextValue> iList = _IDataLogic.AutoComplete(Schema, ColName, FromDate, ToDate, ItemCode, Storeid);
        var Result = (from item in iList where item.Text.ToLower().Contains(prefix.ToLower()) select new { item.Text })
            .Distinct()
            .ToList();

        return Json(Result);
    }
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
       
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public void ChangeConnectionString(string companyName)
    {
        // Step 1: Base connection string with {ServerName} placeholder
        string baseConnectionString = _configuration.GetConnectionString("eTactDB1");

        // Step 2: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 3: Replace placeholder with actual server
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        // Step 4: Parameterized query to avoid SQL injection
        string sql = "SELECT DISTINCT DataBase_Name FROM Company_Detail WHERE Company_Name = @CompanyName";
        using SqlCommand cmdAccountCode = new(sql, conn);
        cmdAccountCode.Parameters.AddWithValue("@CompanyName", companyName);

        List<string> detail = new List<string>();
        using SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            while (rdrAccount.Read())
            {
                detail.Add(rdrAccount[0].ToString());
            }
        }

        // Step 5: Store selected database name in session
        if (detail.Count > 0)
        {
            HttpContext.Session.SetString("databaseName", detail[0]);
        }

        // Step 6: Optionally, get new connection string for the selected company
        string connectionStringForCompany = _connectionStringHelper.GetConnectionStringForCompany();
    }

    public IActionResult GetBranchName(string companyName)
    {
        // Step 1: Change session database if needed
        ChangeConnectionString(companyName);

        // Step 2: Base connection string with {ServerName} placeholder
        string baseConnectionString = _configuration.GetConnectionString("eTactDB1");

        // Step 3: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 4: Replace placeholder with actual server name
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);

        List<string> detail = new List<string>();

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        // Step 5: Parameterized query
        string sql = "SELECT DISTINCT CC FROM Company_Detail WHERE Company_Name = @CompanyName";
        using SqlCommand cmdAccountCode = new(sql, conn);
        cmdAccountCode.Parameters.AddWithValue("@CompanyName", companyName);

        using SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            while (rdrAccount.Read())
            {
                detail.Add(rdrAccount[0].ToString());
            }
        }

        return Json(detail);
    }

    public IActionResult GetYearCode(string branchName, string companyName)
    {
        List<string> detail = new();

        // Step 1: Base connection string with {ServerName} placeholder
        string baseConnectionString = _configuration.GetConnectionString("eTactDB1");

        // Step 2: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 3: Replace placeholder
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);

        // Step 4: Open connection using 'using' block
        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        // Step 5: Parameterized query
        string sql = @"SELECT DISTINCT Financial_Year 
                   FROM Company_Detail 
                   WHERE CC = @BranchName AND Company_Name = @CompanyName 
                   ORDER BY Financial_Year DESC";

        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.AddWithValue("@BranchName", branchName);
        cmd.Parameters.AddWithValue("@CompanyName", companyName);

        using SqlDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            detail.Add(rdr[0].ToString());
        }

        return Json(detail);
    }

    [HttpGet]
    public JsonResult GetCC(string compname)
    {
        string CC = string.Empty;
        string year_code = string.Empty;

        // Step 1: Base connection string with {ServerName} placeholder
        string baseConnectionString = _configuration.GetConnectionString("eTactDB1");

        // Step 2: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 3: Replace placeholder
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        // Step 4: First stored procedure call to get CC
        using (SqlCommand cmd = new("GetId", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TableName", "Company_Detail");
            cmd.Parameters.AddWithValue("@FieldName", "CC");
            cmd.Parameters.AddWithValue("@WhereField", "Company_Name");
            cmd.Parameters.AddWithValue("@WhereValue", compname);

            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows && rdr.Read())
            {
                CC = rdr["CC"].ToString();
            }
        }

        // Step 5: Second stored procedure call to get Financial_Year
        using (SqlCommand cmd = new("GetId", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TableName", "Company_Detail");
            cmd.Parameters.AddWithValue("@FieldName", "Financial_Year");
            cmd.Parameters.AddWithValue("@WhereField", "Company_Name");
            cmd.Parameters.AddWithValue("@WhereValue", compname);

            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows && rdr.Read())
            {
                year_code = rdr["Financial_Year"].ToString();
            }
        }

        // Step 6: Return result as JSON
        List<string> detail = new() { CC, year_code };
        return Json(detail);
    }

    public List<LoginModel> GetCombodata(string Tablename, string fieldname)
    {
        List<LoginModel> catlist = new();

        // Determine base connection string
        string baseConnectionString =
            Tablename == "Store_Master"
                ? _configuration.GetConnectionString("eTactDB")
                : _configuration.GetConnectionString("eTactDB1");

        // Only replace server name if it's eTactDB1
        string finalConnStr = baseConnectionString;
        if (Tablename != "Store_Master")
        {
            // Read server name from file
            string serverName = System.IO.File.ReadAllText(
                Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
            ).Trim();

            // Replace placeholder {ServerName} with actual server
            finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);
        }

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        string sql = "SELECT DISTINCT " + fieldname + " FROM " + Tablename;
        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            var lstAdd = new LoginModel();

            if (fieldname == "Company_Name")
                lstAdd.CompanyName = rdr[fieldname].ToString();
            else if (fieldname == "CC")
                lstAdd.CC = rdr[fieldname].ToString();
            else if (fieldname == "Item_name")
                lstAdd.ItemName = rdr[fieldname].ToString();

            catlist.Add(lstAdd);
        }

        return catlist;
    }

    [HttpGet]
    [Route("GetServerNames")]
    //public IActionResult GetServerName()
    //{
    //    var __configuration = new ConfigurationBuilder()
    //        .AddJsonFile("appsettings.json")
    //        .Build();

    //    // Retrieve connection string
    //    string connectionString = __configuration.GetConnectionString("eTactDB");

    //    // Extract server name
    //    string serverName = GetServerNameFromConnectionString(connectionString);
    //    try
    //    {

    //        string lines = serverName; // Split by line breaks
    //        return Ok(new { Lines = lines });
    //    }
    //    catch (FileNotFoundException ex)
    //    {
    //        return StatusCode(500, $"An error occurred: {ex.Message}");
    //    }
    //}
    public IActionResult GetServerName()
    {
        try
        {
            // Get the path to wwwroot
            string webRootPath = _IWebHostEnvironment.WebRootPath;

            // Construct the path to your text file
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            //string webRootPath1 = _IWebHostEnvironment.WebRootPath;
            string filePath = Path.Combine(webRootPath,  "servername.txt");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return StatusCode(500, "Server name file not found");
            }

            // Read all text from the file
            string serverName = System.IO.File.ReadAllText(filePath);

            // Trim any whitespace and return
            serverName = serverName.Trim();

            string lines = serverName;
            return Ok(new { Lines = lines });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    static string GetServerNameFromConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.DataSource; // DataSource property contains the server name
    }
    public LoginModel GetLastLoginDetail()
    {
        var model = new LoginModel();

        // Base connection string with {ServerName} placeholder
        string connectionstring = _configuration.GetConnectionString("eTactDB1");

        // Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Replace placeholder with actual server
        string finalConnStr = connectionstring.Replace("{ServerName}", serverName);

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        string EntryByMachineName = Environment.MachineName;
        string sql = "exec [SpLastLoggedInDetail] @flag = 'LastLoginDetail', @EntryByMachine = @MachineName";

        using SqlCommand cmdParty = new(sql, conn);
        cmdParty.Parameters.AddWithValue("@MachineName", EntryByMachineName);

        using SqlDataReader rdrParty = cmdParty.ExecuteReader();
        if (rdrParty.HasRows)
        {
            if (rdrParty.Read())
            {
                model.CompanyName = Convert.ToString(rdrParty["CompanyName"]);
                model.YearCode = Convert.ToInt32(rdrParty["FinYear"]);
                model.UserName = Convert.ToString(rdrParty["UserName"]);
                model.Unit = Convert.ToString(rdrParty["BranchName"]);
            }
        }

        return model;
    }
    private string GetClientIpAddress(HttpContext context)
    {
        string ip = context.Connection.RemoteIpAddress?.ToString();

        // If behind proxy or load balancer
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0];
        }

        // Handle local requests (::1 means localhost in IPv6)
        if (ip == "::1" || ip == "0:0:0:0:0:0:0:1")
        {
            // Get local IPv4 address of this machine
            ip = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                    .ToString();
        }

        return ip ?? "Unknown";
    }


    public LoginModel GeteDTRModel()
    {
        var model = new LoginModel();

        // Base connection string with {ServerName} placeholder
        string connectionstring = _configuration.GetConnectionString("eTactDB1");

        // Read server name only
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Replace placeholder
        string finalConnStr = connectionstring.Replace("{ServerName}", serverName);

        using SqlConnection conn = new(finalConnStr);
        conn.Open();

        string sql = "select company_name from company_detail";
        using SqlCommand cmdParty = new(sql, conn);
        using SqlDataReader rdrParty = cmdParty.ExecuteReader();

        if (rdrParty.HasRows)
        {
            IsDrOpen = true;
            if (rdrParty.Read())
            {
                ViewBag.Party = Convert.ToString(rdrParty["company_name"]);
            }
        }

        if (IsDrOpen)
        {
            IsDrOpen = false;
            rdrParty.Close();
        }

        model.AccList = GetCombodata("Company_detail", "Company_Name");
        model.ItemList = GetCombodata("Company_detail", "CC");

        var lastLogin = GetLastLoginDetail();
        if (lastLogin != null)
        {
            model.CompanyName = lastLogin?.CompanyName ?? model.AccList.FirstOrDefault()?.CompanyName;
            model.Unit = lastLogin?.Unit ?? model.ItemList.FirstOrDefault()?.Unit;
            model.UserName = lastLogin?.UserName;
            model.YearCode = lastLogin.YearCode;
        }
        return model;
    }

    //app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}"); enable this code if the line used in startup.cs
    //https://www.infoworld.com/article/3545304/how-to-handle-404-errors-in-aspnet-core-mvc.html#:~:text=A%20simple%20solution%20is%20to,a%20404%20error%20has%20occurred.
    //[Route("/Home/HandleError/{code:int}")]
    //public IActionResult HandleError(int code)
    //{
    //    ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
    //    return View("~/Views/Shared/HandleError.cshtml");
    //}
    [HttpPost]
    public IActionResult Index(InputModel model)
    {
        // HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return View();
    }
    [HttpGet]
    public IActionResult Index()
    {
        var model = new SaleBillModel(); // Create a new model instance for the view
        return View(model);
    }
    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        LoginModel model = GeteDTRModel();
        
        if (!System.IO.File.Exists("license.lic"))
            return Content("No license found. Contact admin.");
        var encrypted = System.IO.File.ReadAllText("license.lic");
        var plain = LicenseCrypto.Decrypt(encrypted);
        var lic = System.Text.Json.JsonSerializer.Deserialize<LicenseInfo>(plain);
        if (DateTime.UtcNow > lic.ExpiryDate)
            return Content("License expired!");
        // Proceed with login...
        // return Content("Login successful, license valid until: " + lic.ExpiryDate.ToShortDateString());

        var daysLeft = (lic.ExpiryDate - DateTime.UtcNow).TotalDays;
        if (daysLeft <= 15)
        {
            // Pass a message to the view about upcoming expiry
            ViewBag.LicenseExpiryWarning = $"Warning: License will expire in {Math.Ceiling(daysLeft)} day(s). Please renew soon.";
        }
        return View(model);
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var servName = model.ServerName;
        var compName = model.CompanyName;
        var branchName = model.Unit;
        var yearCode = model.YearCode;
        var uid = "";
        var EmpId = "";
        var EMPID = 0;
        var EMPNAME = "";
        var DepId = 0;
        var DepName = "";
        string RetailerOrManufacturar = string.Empty;
        string empName = "";
        var userRole = "";
        Constants.FinincialYear = model.YearCode;
        ClaimsIdentity identity = null;
        bool isAuthenticate = false;
       
        #region dynamicConnection

        //string dbNameSql = "SELECT DataBase_Name FROM Company_Detail WHERE Company_Name = @CompanyName";
        //string databaseName = "";

        //using (SqlConnection connDb = new SqlConnection(__configuration.GetConnectionString("eTactDB1")))
        //{
        //    connDb.Open();
        //    using (SqlCommand cmd = new SqlCommand(dbNameSql, connDb))
        //    {
        //        cmd.Parameters.AddWithValue("@CompanyName", compName);

        //        object result = cmd.ExecuteScalar();
        //         databaseName = result?.ToString() ?? "eTactDB"; // Handle null values safely

        //    }
        //    connDb.Close();
        //}
        //string connectionstring = GetConnectionString(databaseName);
        #endregion
       string baseConnectionString = _configuration.GetConnectionString("eTactDB1");
        // Step 2: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 3: Replace placeholder
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);
        SqlConnection conn = new(finalConnStr);
        conn.Open();
        CultureInfo culture = new CultureInfo("en-US");
        DateTime FromDate = new DateTime();
        DateTime ToDate = new DateTime();
        var frmDt = "";
        var toDt = "";
        sql = "select From_Date,To_Date from Company_Detail where Company_Name = '" + compName + "' and Financial_Year='" + yearCode + "'";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();

        List<string> detail = new List<string>();
        while (rdrAccount.Read())
        {
            // Access fields by column name
            FromDate = Convert.ToDateTime(rdrAccount["From_Date"]);
            ToDate = Convert.ToDateTime(rdrAccount["To_Date"]);
            frmDt = FromDate.ToString("dd/MMM/yyyy HH:mm:ss", culture);
            toDt = ToDate.ToString("dd/MMM/yyyy HH:mm:ss", culture);

        }
        //FromDate = detail[0];
        //ToDate = detail[1];
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();

        conn.Open();
        sql = "Select Database_Name from Company_Detail where Company_Name='" + model.CompanyName + "'";
        cmdAccountCode = new(sql, conn);
        rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {

            IsDrOpen = true;
            while (rdrAccount.Read())
            {
                for (int i = 0; i < rdrAccount.FieldCount; i++)
                {
                    detail.Add(rdrAccount[i].ToString());
                }
            }
        }
        var dbName = detail[0].ToString();
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();
        using (SqlConnection logConn = new SqlConnection(finalConnStr))
        {
            await logConn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("SpLastLoggedInDetail", logConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", "INSERT");
                cmd.Parameters.AddWithValue("@EntryByMachine", Environment.MachineName); // or HttpContext.Connection.RemoteIpAddress?.ToString()
                cmd.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                cmd.Parameters.AddWithValue("@FinYear", yearCode);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@LastLogindate", DateTime.Now);
                cmd.Parameters.AddWithValue("@BranchName", model.Unit);
                Console.WriteLine("Connected DB: " + logConn.Database);

                await cmd.ExecuteReaderAsync();
            }
        }
        finalConnStr = $"Data Source={model.ServerName};Initial Catalog={dbName};;User Id=web;Password=bmr2401;Integrated Security=False";
       // finalConnStr = GetConnectionString(dbName);
        _connectionStringService.SetConnectionString(finalConnStr);
        conn = new(finalConnStr);
        try
        {
            conn.Open();
        }
        catch (Exception ex)
        {
            var model1 = GeteDTRModel();
            model.AccList = model1.AccList;
            model.StoreLst = model1.StoreLst;
            model.ItemList = model1.ItemList;

            model.ServerName = servName;
            model.CompanyName = compName;
            model.CC = branchName;
            model.YearCode = yearCode;
            model.Mode = "Y";
            Constants.FinincialYear = yearCode;

            ModelState.Clear();
            ModelState.AddModelError(string.Empty, "Server doesn't exist");
            return View(model);
        }
        //sql = "Select Financial_Year from Company_Detail where CC='" + branchName + "' and Company_Name='" + companyName + "'";
        sql = "exec [GetUserDetail]   @Flage = 'CHKUSERNAME',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
        cmdAccountCode = new(sql, conn);
        rdrAccount = cmdAccountCode.ExecuteReader();
        detail = new List<string>();
        if (rdrAccount.HasRows)
        {

            IsDrOpen = true;
            while (rdrAccount.Read())
            {


                for (int i = 0; i < rdrAccount.FieldCount; i++)
                {
                    detail.Add(rdrAccount[i].ToString());
                }
            }
        }
        var successfulUser = detail[0];
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();

        var successfulPass = "";
        if (successfulUser == "SUCCESSFULL user")
        {
            conn.Open();
            //model.Password = _EncryptDecrypt.Encrypt(model.Password);
            sql = "exec [GetUserDetail]   @Flage = 'CHKPASS',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
            cmdAccountCode = new(sql, conn);
            rdrAccount = cmdAccountCode.ExecuteReader();
            detail = new List<string>();
            if (rdrAccount.HasRows)
            {
                IsDrOpen = true;
                while (rdrAccount.Read())
                {
                    for (int i = 0; i < rdrAccount.FieldCount; i++)
                    {
                        detail.Add(rdrAccount[i].ToString());
                    }
                }
            }
            successfulPass = detail[0];
            if (IsDrOpen == true)
            {
                IsDrOpen = false;
                rdrAccount.Close();
            }
            conn.Close();

            if (successfulPass != "UNSUCCESSFULL")
            {
                EmpId = detail[0];
                var active = detail[1];
                conn.Open();
                sql = "exec [GetUserDetail]   @Flage = 'CHKACTIVE',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
                //sql = "select Active from UserMaster where UserName='" + model.UserName + "' and Password = '" + model.Password + "'";

                cmdAccountCode = new(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();
                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        for (int i = 0; i < rdrAccount.FieldCount; i++)
                        {
                            active = rdrAccount[i].ToString();
                        }
                    }
                }
                if (active == "N")
                {
                    var model1 = GeteDTRModel();
                    model.AccList = model1.AccList;
                    model.StoreLst = model1.StoreLst;
                    model.ItemList = model1.ItemList;

                    model.ServerName = servName;
                    model.CompanyName = compName;
                    model.CC = branchName;
                    model.YearCode = yearCode;
                    model.Mode = "Y";

                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "User is Inactive");
                    return View(model);
                }
                userRole = detail[0];
                if (IsDrOpen == true)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }
                conn.Close();
                conn.Open();
                

                sql = "SELECT isnull(RetailerOrManufacturar,'')  RetailerOrManufacturar  FROM Company_Detail";

                cmdAccountCode = new SqlCommand(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();

                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        RetailerOrManufacturar = rdrAccount.GetString(0);
                    }
                }

                if (IsDrOpen)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }

                conn.Close();
                conn.Open();
                sql = "exec [GetUserDetail]   @Flage = 'GETUSERTYPE',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
                // sql = "select UserType from UserMaster where UserName='" + model.UserName + "' and Password = '" + model.Password + "'";
                cmdAccountCode = new(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();
                detail = new List<string>();
                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        for (int i = 0; i < rdrAccount.FieldCount; i++)
                        {
                            detail.Add(rdrAccount[i].ToString());
                        }
                    }
                }
                userRole = detail[0];
                if (IsDrOpen == true)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }
                conn.Close();

                conn.Open();
                sql = "exec [GetUserDetail]   @Flage = 'GETUID',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
                //sql = "select UID from UserMaster where UserName='" + model.UserName + "' and Password = '" + model.Password + "'";

                cmdAccountCode = new(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();
                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        for (int i = 0; i < rdrAccount.FieldCount; i++)
                        {
                            uid = rdrAccount[i].ToString();
                        }
                    }
                }
                userRole = detail[0];
                if (IsDrOpen == true)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }
                conn.Close();
                conn.Open();
                //sql = "exec [GetUserDetail]   @Flage = 'GetEmpName',@EmpiD ='" + EmpId + "'";

                sql = "select Emp_Name+'--->'+Emp_Code 'EmpName',dm.deptid Department_Id,dm.DeptName from Employee_Master em join UserMaster um on em.Emp_Id = um.EmpID join DepartmentMaster dm on em.DeptId = dm.DeptId where EmpID='" + EmpId + "'";

                //sql = "select Emp_Name+'--->'+Emp_Code 'EmpName',em.deptId ,dm.DeptName from Employee_Master em join UserMaster um on em.Emp_Id = um.EmpID join DepartmentMaster dm on em.deptId  = dm.DeptId where EmpID='" + EmpId+"'";


                cmdAccountCode = new(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();
                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        //for (int i = 0; i < rdrAccount.FieldCount; i++)
                        //{
                        //    EMPNAME = rdrAccount[i].ToString();
                        //}
                        EMPNAME = rdrAccount.GetString(0);
                        DepId = Convert.ToInt32(rdrAccount.GetInt64(1));
                        DepName = rdrAccount.GetString(2);
                    }
                }
                userRole = detail[0];
                if (IsDrOpen == true)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }
                conn.Close();
                conn.Open();
                //sql = "exec [GetUserDetail]   @Flage = 'CHKPASS',@uname ='" + model.UserName + "', @Pass = '" + model.Password + "'";
                sql = "select UID,EmpID,EmpName,Module, MainMenu, OptAll, OptSave, OptUpdate,OptDelete, OptView from UserRights where EmpID='" + EmpId + "'";
                cmdAccountCode = new(sql, conn);
                rdrAccount = cmdAccountCode.ExecuteReader();
                List<NavigationData> data = new List<NavigationData>();
                if (rdrAccount.HasRows)
                {
                    IsDrOpen = true;
                    while (rdrAccount.Read())
                    {
                        int entryid = Convert.ToInt32(rdrAccount.GetInt64(0));
                        int empId = Convert.ToInt32(rdrAccount.GetInt64(1));
                        empName = rdrAccount.GetString(2);
                        string module = rdrAccount.GetString(3);
                        string mainMenu = rdrAccount.GetString(4);
                        //string subMenu = rdrAccount.GetString(5);
                        bool optAll = rdrAccount.GetBoolean(6);
                        bool optSave = rdrAccount.GetBoolean(7);
                        bool optUpdate = rdrAccount.GetBoolean(8);
                        bool optDelete = rdrAccount.GetBoolean(9);
                        //bool optView = rdrAccount.GetBoolean(10);
                        NavigationData row = new NavigationData
                        {
                            Entry_id = entryid,
                            EmpID = empId,
                            EmpName = empName,
                            Module = module,
                            MainMenu = mainMenu,
                            OptAll = optAll,
                            OptSave = optSave,
                            OptUpdate = optUpdate,
                            OptDelete = optDelete
                        };
                        // Add the MyData object to the list
                        data.Add(row);
                    }
                }
                userRole = detail[0];
                if (IsDrOpen == true)
                {
                    IsDrOpen = false;
                    rdrAccount.Close();
                }
                conn.Close();

                Console.WriteLine(data);
                string[] mainModule = new string[data.Count];
                string[] mainMenus = new string[data.Count];
                //string[] subMenus=new string[data.Count];
                for (int i = 0; i < data.Count; i++)
                {
                    mainModule[i] = data[i].Module;
                    mainMenus[i] = data[i].MainMenu;
                    //subMenus[i]=data[i].SubMenu;
                }
                Console.WriteLine(mainModule);
                string joinModules = "";
                string joinMainMenu = "";
                //string joinSubMenu = "";
                joinModules = String.Join(", ", mainModule);
                joinMainMenu = String.Join(", ", mainMenus);
                //joinSubMenu = String.Join(", ", subMenus);

                identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, userRole),
                        new Claim("NavigationMain", joinMainMenu)
                        //new Claim("NavigationSub",joinSubMenu)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                isAuthenticate = true;
            }
        }
        if (isAuthenticate)
        {
            ClaimsPrincipal principal = new(identity);
            HttpContext.Session.SetString("Branch", model.Unit); //done CompanyName
            HttpContext.Session.SetString("CompanyName", model.CompanyName); //done CompanyName
            HttpContext.Session.SetString("YearCode", yearCode.ToString());//done
            HttpContext.Session.SetString("EmpID", EmpId.ToString());//use this everywhere
            HttpContext.Session.SetString("UID", EmpId.ToString());//done there is no UID it will be same as EMpid
            HttpContext.Session.SetString("EmpName", EMPNAME); // done
            HttpContext.Session.SetString("FromDate", frmDt);//done
            HttpContext.Session.SetString("ToDate", toDt);//done
            HttpContext.Session.SetString("UserType", userRole);//done
            HttpContext.Session.SetString("DeptName", DepName);
            HttpContext.Session.SetString("DeptId", DepId.ToString());
            HttpContext.Session.SetString("RetailerOrManufacturar", RetailerOrManufacturar);
            string ipAddress = GetClientIpAddress(HttpContext);

            // Store in session
            HttpContext.Session.SetString("ClientIP", ipAddress);
            //Task login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        else
        {
            var model1 = GeteDTRModel();
            model.AccList = model1.AccList;
            model.StoreLst = model1.StoreLst;
            model.ItemList = model1.ItemList;

            model.ServerName = servName;
            model.CompanyName = compName;
            model.CC = branchName;
            model.YearCode = yearCode;
            model.Mode = "Y";

            ModelState.Clear();
            ModelState.AddModelError(string.Empty, "Invalid Username or Password");
            return View(model);
        }

        

        return RedirectToAction("Dashboard", "Home");
        

    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Home");
    }
    public IActionResult Privacy()
    {
        return View();
    }
    //public JsonResult GetRights()
    //{
    //    var JSON = await _IReqWithoutBOM.FillWorkCenter();
    //    string JsonString = JsonConvert.SerializeObject(JSON);
    //    return Json(JsonString);
    //}
    public string GetConnectionString(string databaseName)
    {
        string baseConnectionString = _configuration.GetConnectionString("eTactDB");

        var builder = new SqlConnectionStringBuilder(baseConnectionString)
        {
            InitialCatalog = databaseName // Dynamically set the database name
        };

        return builder.ConnectionString;
    }
    public JsonResult GetUnitandBranch(string Company, string servername)
    {
        // Step 1: Base connection string with {ServerName} placeholder
        string baseConnectionString = _configuration.GetConnectionString("eTactDB1");

        // Step 2: Read server name from file
        string serverName = System.IO.File.ReadAllText(
            Path.Combine(_IWebHostEnvironment.WebRootPath, "servername.txt")
        ).Trim();

        // Step 3: Replace placeholder
        string finalConnStr = baseConnectionString.Replace("{ServerName}", serverName);
        SqlConnection conn = new(baseConnectionString);
        conn.Open();
        sql = "select CC from Company_Detail where Company_Name = '" + Company + "'";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            IsDrOpen = true;
            if (rdrAccount.Read())
            {
                CC = rdrAccount.GetString(0);
                rdrAccount.Close();
            }
        }
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();
        conn.Open();
        sql = "select Financial_Year from Company_Detail where Company_Name = '" + Company + "'";
        cmdAccountCode = new SqlCommand(sql, conn);
        rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            IsDrOpen = true;
            if (rdrAccount.Read())
            {
                year_code = rdrAccount.GetString(0);
                rdrAccount.Close();
            }
        }
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();
        List<string> detail = new() { CC, year_code };

        //return Json(CC, JsonRequestBehavior.AllowGet);
        return Json(detail);
    }
    [HttpPost]
    public JsonResult GetRights()
    {
        var EmpID = HttpContext.Session.GetString("EmpID");

        // Render profile page with username
        return Json(EmpID);
    }
    public async Task<JsonResult> FillInventoryDashboardData()
    {
        var JSON = await _IDashboard.FillInventoryDashboardData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }  
  
    public async Task<JsonResult> FillInventoryDashboardForPendingData()
    {
        var JSON = await _IDashboard.FillInventoryDashboardForPendingData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillInventoryByCategory()
    {
        var JSON = await _IDashboard.FillInventoryByCategory();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetTopItemByStockValue()
    {
        var JSON = await _IDashboard.GetTopItemByStockValue();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetTopItemBelowMinLevel()
    {
        var JSON = await _IDashboard.GetTopItemBelowMinLevel();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetTopFastMovingItem()
    {
        var JSON = await _IDashboard.GetTopFastMovingItem();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardData()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardDataByCategoryValue()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardDataByCategoryValue();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardDataMonthlyTrend()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardDataMonthlyTrend();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardTop10ItemData()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardTop10ItemData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardTop10VendorData()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardTop10VendorData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseDashboardNewVendorInThisMonthData()
    {
        var JSON = await _IDashboard.FillPurchaseDashboardNewVendorInThisMonthData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPurchaseVsConsumptionDashboardData()
    {
        var JSON = await _IDashboard.FillPurchaseVsConsumptionDashboardData();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillBestSupplier()
    {
        var JSON = await _IDashboard.FillBestSupplier();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillWorstSupplier()
    {
        var JSON = await _IDashboard.FillWorstSupplier();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillDisplaySalesHeading()
    {
        var JSON = await _IDashboard.FillDisplaySalesHeading();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillSALEMonthlyTrend()
    {
        var JSON = await _IDashboard.FillSALEMonthlyTrend();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillTop10SALECUSTOMER()
    {
        var JSON = await _IDashboard.FillTop10SALECUSTOMER();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillTop10SOLDItem()
    {
        var JSON = await _IDashboard.FillTop10SOLDItem();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillTop10SALESPERSON()
    {
        var JSON = await _IDashboard.FillTop10SALESPERSON();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillNEWCustomerOFTHEMONTH()
    {
        var JSON = await _IDashboard.FillNEWCustomerOFTHEMONTH();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillMonthlyRejectionTrend()
    {
        var JSON = await _IDashboard.FillMonthlyRejectionTrend();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillSaleOrderVsDispatch()
    {
        var JSON = await _IDashboard.FillSaleOrderVsDispatch();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
}