using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class XONUserRightDashboardDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public XONUserRightDashboardDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<IList<TextValue>> GetUserList(string ShowAllUsers)
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "GetUserList");
                    oCmd.Parameters.AddWithValue("@ShowAllUser", ShowAllUsers);

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["UserName"].ToString(),
                                Value = Reader["UID"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        public async Task<IList<TextValue>> GetDashboardName()
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DashboardName");

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["DashboardName"].ToString(),
                                Value = Reader["DashboardName"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        public async Task<IList<TextValue>> GetDashboardSubScreen(string DashboardName)
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DashboardSubScreen");
                    oCmd.Parameters.AddWithValue("@DashboardName", DashboardName);

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["DashboardSubScreen"].ToString(),
                                Value = Reader["DashboardSubScreen"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
    }
}
