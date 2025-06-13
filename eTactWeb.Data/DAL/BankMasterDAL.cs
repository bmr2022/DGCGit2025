using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class BankMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public BankMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> DeleteByID(int ID)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Account_Code", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "DeleteByID");

                    await myConnection.OpenAsync();
                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = Convert.ToInt32(Reader["StatusCode"].ToString()) == 410
                                    ? HttpStatusCode.Gone
                                    : HttpStatusCode.BadRequest,
                                StatusText = "Success",
                                Result = Reader["Result"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Exception";
                _ResponseResult.Result = ex.Message.ToString();
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Account Master"));
                //SqlParams.Add(new SqlParameter("@SubMenu", ""));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<BankMasterModel> GetByID(int ID)
        {
            BankMasterModel? _BankMasterModel = new BankMasterModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Account_Code", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "GetByID");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }

                if (oDataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in oDataTable.Rows)
                    {
                        _BankMasterModel.Account_Code = Convert.ToInt32(dr["Account_Code"].ToString());
                        _BankMasterModel.DebCredCode = dr["DebCredCode"].ToString();
                        _BankMasterModel.Party_Code = dr["Party_Code"].ToString();
                        _BankMasterModel.Entry_Date = dr["Entry_Date"].ToString();
                        _BankMasterModel.Account_Name = dr["Account_Name"].ToString();
                        _BankMasterModel.DisplayName = dr["DisplayName"].ToString();
                        _BankMasterModel.ParentAccountCode = Convert.ToInt32(dr["ParentAccountCode"].ToString());
                        _BankMasterModel.MainGroup = dr["MainGroup"].ToString();
                        _BankMasterModel.AccountType = dr["AccountType"].ToString();
                        _BankMasterModel.SubGroup = dr["SubGroup"].ToString();
                        _BankMasterModel.SubSubGroup = dr["SubSubGroup"].ToString();
                        _BankMasterModel.UnderGroup = dr["UnderGroup"].ToString();
                        _BankMasterModel.ComAddress = dr["ComAddress"].ToString();
                        _BankMasterModel.ComAddress1 = dr["ComAddress1"].ToString();
                        _BankMasterModel.PinCode = dr["PinCode"].ToString();
                        _BankMasterModel.Country = dr["Country"].ToString();
                        _BankMasterModel.State = dr["State"].ToString();
                        _BankMasterModel.City = dr["City"].ToString();
                        _BankMasterModel.PhoneNo = dr["PhoneNo"].ToString();
                        _BankMasterModel.MobileNo = dr["MobileNo"].ToString();
                        _BankMasterModel.ContactPerson = dr["ContactPerson"].ToString();
                        _BankMasterModel.PartyType = dr["PartyType"].ToString();
                         _BankMasterModel.PANNO = dr["PANNO"].ToString();
                         _BankMasterModel.ResponsibleEmployee = dr["ResponsibleEmployee"].ToString();
                        _BankMasterModel.ResponsibleEmpContactNo = dr["ResponsibleEmpContactNo"].ToString();
                        _BankMasterModel.SalesPersonName = dr["SalesPersonName"].ToString();
                        _BankMasterModel.SalesPersonEmailId = dr["SalesPersonEmailId"].ToString();
                        _BankMasterModel.SalesPersonMobile = dr["SalesPersonMobile"].ToString();
                          _BankMasterModel.EMail = dr["EMail"].ToString();
                         _BankMasterModel.Division = dr["Division"].ToString();
                        _BankMasterModel.Commodity = dr["Commodity"].ToString();
                         _BankMasterModel.RateOfInt = Convert.ToInt32(dr["RateOfInt"]);
                        _BankMasterModel.CreditLimit = Convert.ToInt32(dr["CreditLimit"]);
                        _BankMasterModel.CreditDays = Convert.ToInt32(dr["CreditDays"]);
                         _BankMasterModel.BankAccount_No = dr["BankAccount_No"].ToString();
                        _BankMasterModel.BankAddress = dr["BankAddress"].ToString();
                        _BankMasterModel.BankIFSCCode = dr["BankIFSCCode"].ToString();
                        _BankMasterModel.BankSwiftCode = dr["BankSwiftCode"].ToString();
                        _BankMasterModel.salesperson_name = dr["SalesPersonName"].ToString();
                        _BankMasterModel.salesemailid = dr["SalesPersonEmailId"].ToString();
                        _BankMasterModel.salesmobileno = dr["SalesPersonMobile"].ToString();
                        _BankMasterModel.Approved_By = dr["Approved_By"].ToString();
                        _BankMasterModel.Approved = dr["Approved"].ToString();
                        _BankMasterModel.ApprovalDate = dr["ApprovalDate"].ToString();
                        _BankMasterModel.YearCode = Convert.ToInt32(dr["YearCode"].ToString());
                        _BankMasterModel.Uid = dr["Uid"].ToString();
                        _BankMasterModel.CC = dr["CC"].ToString();
                        _BankMasterModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        _BankMasterModel.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
                        _BankMasterModel.CreatedByName = dr["CreatedByName"].ToString();
                        _BankMasterModel.UpdatedBy = dr["UpdatedBy"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UpdatedBy"]);
                        _BankMasterModel.UpdatedByName = dr["UpdatedByName"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataTable.Dispose();
            }

            return _BankMasterModel;
        }

        public async Task<BankMasterModel> GetDashboardData(BankMasterModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Account_Name", string.IsNullOrEmpty(model.Account_Name) ? null : model.Account_Name.Trim());
                    oCmd.Parameters.AddWithValue("@ParentAccountCode", model.ParentAccountCode);
                    oCmd.Parameters.AddWithValue("@SalesPersonName", string.IsNullOrEmpty(model.SalesPersonName) ? null : model.SalesPersonName.Trim());
                    oCmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(model.State) ? null : model.State.Trim());
                    oCmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(model.City) ? null : model.City.Trim());
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.BankMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new BankMasterModel
                                               {
                                                   Account_Code = Convert.ToInt32(dr["Account_Code"]),
                                                   Account_Name = dr["Account_Name"].ToString(),
                                                   Entry_Date = dr["Entry_Date"].ToString(),
                                                   DisplayName = dr["DisplayName"].ToString(),
                                                   ParentAccountName = dr["ParentAccount"].ToString(),
                                                   ParentAccountCode = Convert.ToInt32(dr["ParentAccountCode"]),
                                                   SubGroup = dr["SubGroup"].ToString(),
                                                   AccountType = dr["AccountType"].ToString(),
                                                   MainGroup = dr["MainGroup"].ToString(),
                                                   SubSubGroup = dr["SubSubGroup"].ToString(),
                                                   UnderGroup = dr["UnderGroup"].ToString(),
                                                   BankAccount_No = dr["BankAccount_No"].ToString(),
                                                   BankIFSCCode = dr["BankIFSCCode"].ToString(),
                                                   BankAddress = dr["BankAddress"].ToString(),
                                                   PinCode = dr["PinCode"].ToString(),
                                                   City = dr["City"].ToString(),
                                                   State = dr["State"].ToString(),
                                                   Country = dr["Country"].ToString(),
                                               }).ToList();
                }

                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

                if (model.Mode != "Search")
                {
                    List<TextValue>? _list = new List<TextValue>();
                    model.ParentGroupList = _list;

                    if (model.BankMasterList != null)
                    {
                        foreach (BankMasterModel? item in model.BankMasterList)
                        {
                            TextValue? _lst = new TextValue
                            {
                                Text = item.ParentAccountName,
                                Value = item.ParentAccountCode.ToString()
                            };
                            _list.Add(_lst);
                        }

                        model.ParentGroupList = _list;
                    }
                   // if  model.AccountMasterList != null{
                     // ;
                    //}
                    
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }
        public async Task<BankMasterModel> GetDetailDashboardData(BankMasterModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Account_Name", string.IsNullOrEmpty(model.Account_Name) ? null : model.Account_Name.Trim());
                    oCmd.Parameters.AddWithValue("@ParentAccountCode", model.ParentAccountCode);
                    oCmd.Parameters.AddWithValue("@SalesPersonName", string.IsNullOrEmpty(model.SalesPersonName) ? null : model.SalesPersonName.Trim());
                    oCmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(model.State) ? null : model.State.Trim());
                    oCmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(model.City) ? null : model.City.Trim());
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.BankMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new BankMasterModel
                                               {
                                                   Account_Code = Convert.ToInt32(dr["Account_Code"]),
                                                   Account_Name = dr["Account_Name"].ToString(),
                                                   ParentAccountName = dr["ParentAccount"].ToString(),
                                                   ParentAccountCode = Convert.ToInt32(dr["ParentAccountCode"]),
                                                   SubGroup = dr["SubGroup"].ToString(),
                                                   MainGroup = dr["MainGroup"].ToString(),
                                                   UnderGroup = dr["UnderGroup"].ToString(),
                                                   ComAddress = dr["ComAddress"].ToString(),
                                                   ComAddress1 = dr["ComAddress1"].ToString(),
                                                   PinCode = dr["PinCode"].ToString(),
                                                   City = dr["City"].ToString(),
                                                   State = dr["State"].ToString(),
                                                   Country = dr["Country"].ToString(),
                                                   PhoneNo = dr["PhoneNo"].ToString(),
                                                   MobileNo = dr["MobileNo"].ToString(),
                                                   ContactPerson = dr["ContactPerson"].ToString(),
                                                   PartyType = dr["PartyType"].ToString(),
                                                    PANNO = dr["PANNO"].ToString(),
                                                    ResponsibleEmployee = dr["ResponsibleEmployee"].ToString(),
                                                   ResponsibleEmpContactNo = dr["ResponsibleEmpContactNo"].ToString(),
                                                   SalesPersonName = dr["SalesPersonName"].ToString(),
                                                   SalesPersonEmailId = dr["SalesPersonEmailId"].ToString(),
                                                   SalesPersonMobile = dr["SalesPersonMobile"].ToString(),
                                               }).ToList();
                }

                //var ilst = model.AccountMasterList.Select(m => new TextValue
                //{
                //    Text = m.ParentAccountName,
                //    Value = m.ParentAccountCode.ToString()
                //});

                if (model.Mode != "Search" || model.Mode != "DetailSearch")
                {
                    List<TextValue>? _list = new List<TextValue>();
                    model.ParentGroupList = _list;

                    if (model.BankMasterList != null)
                    {
                        foreach (BankMasterModel? item in model.BankMasterList)
                        {
                            TextValue? _lst = new TextValue
                            {
                                Text = item.ParentAccountName,
                                Value = item.ParentAccountCode.ToString()
                            };
                            _list.Add(_lst);
                        }

                        model.ParentGroupList = _list;
                    }
                   // if  model.AccountMasterList != null{
                     // ;
                    //}
                    
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }

        public async Task<IList<TextValue>> GetDropDownList(string Flag)
        {
            List<TextValue>? List = new List<TextValue>();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_GetDropDownList", myConnection);
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }

                    if (oDataTable.Rows.Count > 0)
                    {
                        if (Flag == "StateMaster")
                        {
                            List = (from DataRow dr in oDataTable.Rows
                                    select new TextValue
                                    {
                                        Text = dr["StateName"].ToString(),
                                        Value = dr["Entry_Id"].ToString()
                                    }).ToList();
                        }

                        if (Flag == "VPrimaryAccountHeadMasterForBank")
                        {
                            List = (from DataRow dr in oDataTable.Rows
                                    select new TextValue
                                    {
                                        Text = dr["Account_Name"].ToString(),
                                        Value = dr["Account_Code"].ToString()
                                    }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                List.Add(new TextValue
                {
                    Text = ex.Message,
                    Value = ex.Source
                });
            }
            finally
            {
                oDataTable.Dispose();
            }
            return List;
        }

        public async Task<ResponseResult> GetParentGroupDetail(string ID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ParentGroupDetail"));
                SqlParams.Add(new SqlParameter("@Account_Code", ID));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_AccountMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<ResponseResult> SaveAccountMaster(BankMasterModel model)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);

                    oCmd.Parameters.AddWithValue("@Account_Code", model.Account_Code);
                    oCmd.Parameters.AddWithValue("@DebCredCode", model.DebCredCode);
                    oCmd.Parameters.AddWithValue("@Party_Code", model.Party_Code);
                    oCmd.Parameters.AddWithValue("@Entry_Date", ParseDate(model.Entry_Date));
                    oCmd.Parameters.AddWithValue("@Account_Name", model.Account_Name);
                    oCmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                    oCmd.Parameters.AddWithValue("@ParentAccountCode", model.ParentAccountCode);
                    oCmd.Parameters.AddWithValue("@MainGroup", model.MainGroup);
                    oCmd.Parameters.AddWithValue("@AccountType", model.AccountType);
                    oCmd.Parameters.AddWithValue("@SubGroup", model.SubGroup);
                    oCmd.Parameters.AddWithValue("@SubSubGroup", model.SubSubGroup);
                    oCmd.Parameters.AddWithValue("@UnderGroup", model.UnderGroup);
                    oCmd.Parameters.AddWithValue("@ComAddress", model.ComAddress);
                    oCmd.Parameters.AddWithValue("@ComAddress1", model.ComAddress1);
                    oCmd.Parameters.AddWithValue("@PinCode", model.PinCode);
                    oCmd.Parameters.AddWithValue("@Country", model.Country);
                    oCmd.Parameters.AddWithValue("@State", model.State);
                    oCmd.Parameters.AddWithValue("@City", model.City);
                    oCmd.Parameters.AddWithValue("@PhoneNo", model.PhoneNo);
                    oCmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    oCmd.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
                    oCmd.Parameters.AddWithValue("@PartyType", model.PartyType);
                    oCmd.Parameters.AddWithValue("@PANNO", model.PANNO);
                    oCmd.Parameters.AddWithValue("@ResponsibleEmployee", model.ResponsibleEmployee);
                    oCmd.Parameters.AddWithValue("@ResponsibleEmpContactNo", model.ResponsibleEmpContactNo);
                    oCmd.Parameters.AddWithValue("@SalesPersonName", model.SalesPersonName);
                    oCmd.Parameters.AddWithValue("@SalesPersonEmailId", model.SalesPersonEmailId);
                    oCmd.Parameters.AddWithValue("@SalesPersonMobile", model.SalesPersonMobile);
                    oCmd.Parameters.AddWithValue("@EMail", model.EMail);
                    oCmd.Parameters.AddWithValue("@Division", model.Division);
                    oCmd.Parameters.AddWithValue("@Commodity", model.Commodity);
                    oCmd.Parameters.AddWithValue("@RateOfInt", model.RateOfInt);
                    oCmd.Parameters.AddWithValue("@CreditLimit", model.CreditLimit);
                    oCmd.Parameters.AddWithValue("@CreditDays", model.CreditDays);
                    oCmd.Parameters.AddWithValue("@BankAccount_No", model.BankAccount_No);
                    oCmd.Parameters.AddWithValue("@BankAddress", model.BankAddress);
                    oCmd.Parameters.AddWithValue("@BankIFSCCode", model.BankIFSCCode);
                    oCmd.Parameters.AddWithValue("@BankSwiftCode", model.BankSwiftCode);
                    oCmd.Parameters.AddWithValue("@salesperson_name", model.salesperson_name);
                    oCmd.Parameters.AddWithValue("@salesemailid", model.salesemailid);
                    oCmd.Parameters.AddWithValue("@salesmobileno", model.salesmobileno);
                    oCmd.Parameters.AddWithValue("@Approved_By", model.Approved_By);
                    oCmd.Parameters.AddWithValue("@Approved", model.Approved);
                    oCmd.Parameters.AddWithValue("@ApprovalDate", model.ApprovalDate);
                    oCmd.Parameters.AddWithValue("@YearCode", model.YearCode);
                    oCmd.Parameters.AddWithValue("@Uid", model.Uid);
                    oCmd.Parameters.AddWithValue("@CC", model.CC);

                    oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    if (model.Mode == "Update")
                    {
                        oCmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);

                    }
                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusText = Reader["StatusText"].ToString(),
                                Result = Reader["Result"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> GetTDSPartyList()
        {
            ResponseResult? _ResponseResult = new ResponseResult();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    DataTable? oDataTable = new DataTable();
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "TDSPartyList");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                    if (oDataTable.Rows.Count != 0)
                    {
                        _ResponseResult.StatusCode = HttpStatusCode.OK;
                        _ResponseResult.StatusText = "Success";
                        _ResponseResult.Result = oDataTable;
                    }
                    else
                    {
                        _ResponseResult.StatusCode = HttpStatusCode.NotFound;
                        _ResponseResult.StatusText = "Error";
                        _ResponseResult.Result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = (HttpStatusCode)ex.HResult;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = ex.StackTrace;
            }
            return _ResponseResult;
        }
    }
}