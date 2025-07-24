
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
    public HomeController(IConfiguration config, ILogger<HomeController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IConnectionStringHelper connectionStringHelper, UserContextService userContextService, ConnectionStringService connectionStringService,IDashboard IDashboard)
    {
        _logger = logger;
        this._IDataLogic = iDataLogic;
        _configuration = config;
        _EncryptDecrypt = encryptDecrypt;
        _connectionStringHelper = connectionStringHelper;
        _userContextService = userContextService;
        _connectionStringService = connectionStringService;
        _IDashboard = IDashboard;
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
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
        conn.Open();
        sql = "Select DISTINCT DataBase_Name from Company_Detail where Company_Name='" + companyName + "'";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        List<string> detail = new List<string>();
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
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        HttpContext.Session.SetString("databaseName", detail[0]);
        string connectionString = _connectionStringHelper.GetConnectionStringForCompany();

    }
    public IActionResult GetBranchName(string companyName)
    {
        ChangeConnectionString(companyName);
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
        conn.Open();
        sql = "Select DISTINCT CC from Company_Detail where Company_Name='" + companyName + "'";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        List<string> detail = new List<string>();
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
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();
        return Json(detail);
    }
    public IActionResult GetYearCode(string branchName, string companyName)
    {
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
        conn.Open();
        sql = "Select DISTINCT Financial_Year from Company_Detail where CC='" + branchName + "' and Company_Name='" + companyName + "' ORDER BY Financial_Year DESC";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        List<string> detail = new List<string>();
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
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        conn.Close();
        return Json(detail);
    }
    [HttpGet]
    public JsonResult GetCC(string compname)
    {
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
        conn.Open();
        sql = " exec GetId 'Company_Detail','CC', 'Company_Name','" + compname + "'";
        SqlCommand cmdAccountCode = new(sql, conn);
        SqlDataReader rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            IsDrOpen = true;
            if (rdrAccount.Read())
            {
                CC = Convert.ToString(rdrAccount["CC"]);
                rdrAccount.Close();
            }
        }
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrAccount.Close();
        }
        sql = " exec GetId 'Company_Detail','Financial_Year', 'Company_Name','" + compname + "'";
        cmdAccountCode = new SqlCommand(sql, conn);
        rdrAccount = cmdAccountCode.ExecuteReader();
        if (rdrAccount.HasRows)
        {
            IsDrOpen = true;
            if (rdrAccount.Read())
            {
                year_code = Convert.ToString(rdrAccount["Financial_Year"]);
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
    public List<LoginModel> GetCombodata(string Tablename, string fieldname)
    {
        List<LoginModel> catlist = new();
        string connectionstring =
            Tablename == "Store_Master"
                ? _configuration.GetConnectionString("eTactDB")
                : _configuration.GetConnectionString("eTactDB1");

        SqlConnection conn = new(connectionstring);
        conn.Open();
        string sql = " select distinct " + fieldname + " from " + Tablename;

        SqlCommand cmd = new(sql, conn);
        SqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            var lstAdd = new LoginModel();
            if (fieldname == "Company_Name")
            {
                lstAdd.CompanyName = rdr[fieldname].ToString();
            }
            else if (fieldname == "CC")
            {
                lstAdd.CC = rdr[fieldname].ToString();
            }
            else if (fieldname == "Item_name")
            {
                lstAdd.ItemName = rdr[fieldname].ToString();
            }
            catlist.Add(lstAdd);
        }
        rdr.Close();
        conn.Close();
        return catlist;
    }
    [HttpGet]
    [Route("GetServerNames")]
    public IActionResult GetServerName()
    {
        var __configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Retrieve connection string
        string connectionString = __configuration.GetConnectionString("eTactDB");

        // Extract server name
        string serverName = GetServerNameFromConnectionString(connectionString);
        try
        {

            string lines = serverName; // Split by line breaks
            return Ok(new { Lines = lines });
        }
        catch (FileNotFoundException ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    static string GetServerNameFromConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.DataSource; // DataSource property contains the server name
    }
    public LoginModel GeteDTRModel()
    {
        var model = new LoginModel();
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
        conn.Open();
        sql = "select company_name from company_detail";
        SqlCommand cmdParty = new(sql, conn);
        SqlDataReader rdrParty = cmdParty.ExecuteReader();
        if (rdrParty.HasRows)
        {
            IsDrOpen = true;
            if (rdrParty.Read())
            {
                ViewBag.Party = Convert.ToString(rdrParty["company_name"]);
            }
        }
        if (IsDrOpen == true)
        {
            IsDrOpen = false;
            rdrParty.Close();
        }

        model.AccList = GetCombodata("Company_detail", "Company_Name");
        model.ItemList = GetCombodata("Company_detail", "CC");
        model.StoreLst = GetCombodata("Store_Master", "Store_Name");
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
       string  connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
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
        //connectionstring = $"Data Source={model.ServerName};Initial Catalog={dbName};;User Id=web;Password=bmr2401;Integrated Security=False";
        connectionstring = GetConnectionString(dbName);
        _connectionStringService.SetConnectionString(connectionstring);
        conn = new(connectionstring);
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
        /*var model1 = GeteDTRModel();
        model.AccList = model1.AccList;
        model.StoreLst = model1.StoreLst;
        model.ItemList = model1.ItemList;

            //    ModelState.Clear();
            //    ModelState.AddModelError(string.Empty, "All Fields are Mandatory & Cannot be Blank.");
            //    return View(model);
            //}
            //else
            //{
            ClaimsIdentity identity = null;
            bool isAuthenticate = false;

            if (model.UserName == "admin" && model.Password == "a")
            {
                identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                isAuthenticate = true;
            }

            if (model.UserName == "demo" && model.Password == "c")
            {
                identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                isAuthenticate = true;
            }

            if (isAuthenticate)
            {
                ClaimsPrincipal principal = new(identity);
                //Task login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            else
            {
                return View(model);
            }
        */

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
        string connectionstring = _configuration.GetConnectionString("eTactDB1");
        SqlConnection conn = new(connectionstring);
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
}