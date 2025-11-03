using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class AccountMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public AccountMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetStateCode(string State)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetStateCode"));
                SqlParams.Add(new SqlParameter("@State", State));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_AccountMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetAccountGroupDetail(string AccountName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAccountGroupDetail"));
                SqlParams.Add(new SqlParameter("@Account_Name", AccountName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_AccountMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
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
        

        public async Task<AccountMasterModel> GetByID(int ID)
        {
            AccountMasterModel? _AccountMasterModel = new AccountMasterModel();
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
                        _AccountMasterModel.Account_Code = Convert.ToInt32(dr["Account_Code"].ToString());
                        _AccountMasterModel.DebCredCode = dr["DebCredCode"].ToString();
                        _AccountMasterModel.Party_Code = dr["Party_Code"].ToString();
                        _AccountMasterModel.Entry_Date = dr["Entry_Date"].ToString();
                        _AccountMasterModel.Account_Name = dr["Account_Name"].ToString();
                        _AccountMasterModel.DisplayName = dr["DisplayName"].ToString();
                        _AccountMasterModel.ParentAccountCode = Convert.ToInt32(dr["ParentAccountCode"].ToString());
                        _AccountMasterModel.MainGroup = dr["MainGroup"].ToString();
                        _AccountMasterModel.AccountType = dr["AccountType"].ToString();
                        _AccountMasterModel.SubGroup = dr["SubGroup"].ToString();
                        _AccountMasterModel.SubSubGroup = dr["SubSubGroup"].ToString();
                        _AccountMasterModel.UnderGroup = dr["UnderGroup"].ToString();
                        _AccountMasterModel.ComAddress = dr["ComAddress"].ToString();
                        _AccountMasterModel.ComAddress1 = dr["ComAddress1"].ToString();
                        _AccountMasterModel.PinCode = dr["PinCode"].ToString();
                        _AccountMasterModel.Country = dr["Country"].ToString();
                        _AccountMasterModel.State = dr["State"].ToString();
                        _AccountMasterModel.City = dr["City"].ToString();
                        _AccountMasterModel.PhoneNo = dr["PhoneNo"].ToString();
                        _AccountMasterModel.MobileNo = dr["MobileNo"].ToString();
                        _AccountMasterModel.ContactPerson = dr["ContactPerson"].ToString();
                        _AccountMasterModel.PartyType = dr["PartyType"].ToString();
                        _AccountMasterModel.GSTRegistered = dr["GSTRegistered"].ToString();
                        _AccountMasterModel.GSTNO = dr["GSTNO"].ToString();
                        _AccountMasterModel.GSTPartyTypes = dr["GSTPartyTypes"].ToString();
                        _AccountMasterModel.GSTTAXTYPE = dr["GSTTAXTYPE"].ToString();
                        _AccountMasterModel.Segment = dr["Segment"].ToString();
                        _AccountMasterModel.SSLNo = dr["SSLNo"].ToString();
                        _AccountMasterModel.PANNO = dr["PANNO"].ToString();
                        _AccountMasterModel.TDS = dr["TDS"].ToString();
                        _AccountMasterModel.TDSRate = dr["TDSRate"].ToString();
                        _AccountMasterModel.TDSPartyCategery = dr["TDSPartyCategery"].ToString();
                        _AccountMasterModel.ResponsibleEmployee = dr["ResponsibleEmployee"].ToString();
                        _AccountMasterModel.ResponsibleEmpContactNo = dr["ResponsibleEmpContactNo"].ToString();
                        _AccountMasterModel.SalesPersonName = dr["SalesPersonName"].ToString();
                        _AccountMasterModel.SalesPersonEmailId = dr["SalesPersonEmailId"].ToString();
                        _AccountMasterModel.SalesPersonMobile = dr["SalesPersonMobile"].ToString();
                        _AccountMasterModel.PurchPersonName = dr["PurchPersonName"].ToString();
                        _AccountMasterModel.PurchasePersonEmailId = dr["PurchasePersonEmailId"].ToString();
                        _AccountMasterModel.PurchMobileNo = dr["PurchMobileNo"].ToString();
                        _AccountMasterModel.QCPersonEmailId = dr["QCPersonEmailId"].ToString();
                        _AccountMasterModel.WebSite_Add = dr["WebSite_Add"].ToString();
                        _AccountMasterModel.EMail = dr["EMail"].ToString();
                        _AccountMasterModel.RANGE = dr["RANGE"].ToString();
                        _AccountMasterModel.Division = dr["Division"].ToString();
                        _AccountMasterModel.Commodity = dr["Commodity"].ToString();
                        _AccountMasterModel.WorkingAdd1 = dr["WorkingAdd1"].ToString();
                        _AccountMasterModel.WorkingAdd2 = dr["WorkingAdd2"].ToString();
                        _AccountMasterModel.RateOfInt = dr["RateOfInt"].ToString();
                        _AccountMasterModel.CreditLimit = dr["CreditLimit"].ToString();
                        _AccountMasterModel.CreditDays = dr["CreditDays"].ToString();
                        _AccountMasterModel.SSL = dr["SSL"].ToString();
                        _AccountMasterModel.BankAccount_No = dr["BankAccount_No"].ToString();
                        _AccountMasterModel.BankAddress = dr["BankAddress"].ToString();
                        _AccountMasterModel.BankIFSCCode = dr["BankIFSCCode"].ToString();
                        _AccountMasterModel.BankSwiftCode = dr["BankSwiftCode"].ToString();
                        _AccountMasterModel.InterbranchSaleBILL = dr["InterbranchSaleBILL"].ToString();
                        //_AccountMasterModel.salesperson_name = dr["salesperson_name"] == DBNull.Value ? "" : dr["salesperson_name"].ToString(); 
                        //_AccountMasterModel.salesemailid = dr["salesemailid"].ToString();
                        //_AccountMasterModel.salesmobileno = dr["salesmobileno"].ToString();
                        _AccountMasterModel.Approved_By = dr["Approved_By"].ToString();
                        _AccountMasterModel.Approved = dr["Approved"].ToString();
                        _AccountMasterModel.ApprovalDate = dr["ApprovalDate"].ToString();
                        _AccountMasterModel.BlackListed = dr["BlackListed"].ToString();
                        _AccountMasterModel.BlackListed_By = dr["BlackListed_By"].ToString();
                        _AccountMasterModel.YearCode = Convert.ToInt32(dr["YearCode"].ToString());
                        _AccountMasterModel.SalePersonEmpId = Convert.ToInt32(dr["SalePersonEmpId"].ToString());
                        _AccountMasterModel.Uid = dr["Uid"].ToString();
                        _AccountMasterModel.CC = dr["CC"].ToString();
                        _AccountMasterModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        _AccountMasterModel.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
                        _AccountMasterModel.CreatedByName = dr["UserName"].ToString();
                        _AccountMasterModel.MSMENo = dr["MSMENo"].ToString();
                        _AccountMasterModel.MSMEType = dr["MSMEType"].ToString();
                        _AccountMasterModel.DiscountCategory = dr["DiscountCategory"].ToString();
                        _AccountMasterModel.GroupDiscountCategory = Convert.ToInt32(dr["GroupDiscountCategory"].ToString());
                        _AccountMasterModel.Region = dr["Region"].ToString();
                     
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

            return _AccountMasterModel;
        }

        public async Task<AccountMasterModel> GetDashboardData(AccountMasterModel model)
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
                    model.AccountMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new AccountMasterModel
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
                                                   GSTNO = dr["GSTNO"].ToString(),
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

                    if (model.AccountMasterList != null)
                    {
                        foreach (AccountMasterModel? item in model.AccountMasterList)
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
        public async Task<AccountMasterModel> GetDetailDashboardData(AccountMasterModel model)
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
                    model.AccountMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new AccountMasterModel
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
                                                   GSTNO = dr["GSTNO"].ToString(),
                                                   PhoneNo = dr["PhoneNo"].ToString(),
                                                   MobileNo = dr["MobileNo"].ToString(),
                                                   ContactPerson = dr["ContactPerson"].ToString(),
                                                   PartyType = dr["PartyType"].ToString(),
                                                   GSTRegistered = dr["GSTRegistered"].ToString(),
                                                   GSTPartyTypes = dr["GSTPartyTypes"].ToString(),
                                                   GSTTAXTYPE = dr["GSTTAXTYPE"].ToString(),
                                                   Segment = dr["Segment"].ToString(),
                                                   SSLNo = dr["SSLNo"].ToString(),
                                                   PANNO = dr["PANNO"].ToString(),
                                                   TDS = dr["TDS"].ToString(),
                                                   TDSRate = dr["TDSRate"].ToString(),
                                                   TDSPartyCategery = dr["TDSPartyCategery"].ToString(),
                                                   ResponsibleEmployee = dr["ResponsibleEmployee"].ToString(),
                                                   ResponsibleEmpContactNo = dr["ResponsibleEmpContactNo"].ToString(),
                                                   SalesPersonName = dr["SalesPersonName"].ToString(),
                                                   SalesPersonEmailId = dr["SalesPersonEmailId"].ToString(),
                                                   SalesPersonMobile = dr["SalesPersonMobile"].ToString(),
                                                   PurchPersonName = dr["PurchPersonName"].ToString(),
                                                   PurchasePersonEmailId = dr["PurchasePersonEmailId"].ToString(),
                                                   PurchMobileNo = dr["PurchMobileNo"].ToString(),
                                                   QCPersonEmailId = dr["QCPersonEmailId"].ToString(),
                                                   WebSite_Add = dr["WebSite_Add"].ToString(),
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

                    if (model.AccountMasterList != null)
                    {
                        foreach (AccountMasterModel? item in model.AccountMasterList)
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
                    SqlCommand oCmd = new SqlCommand("SP_AccountMaster", myConnection);
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

                        if (Flag == "VPrimaryAccountHeadMaster")
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

        public async Task<ResponseResult> SaveAccountMaster(AccountMasterModel model)
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
                    oCmd.Parameters.AddWithValue("@GSTRegistered", model.GSTRegistered);
                    oCmd.Parameters.AddWithValue("@GSTNO", model.GSTNO);
                    oCmd.Parameters.AddWithValue("@GSTPartyTypes", model.GSTPartyTypes);
                    oCmd.Parameters.AddWithValue("@GSTTAXTYPE", model.GSTTAXTYPE);
                    oCmd.Parameters.AddWithValue("@Segment", model.Segment);
                    oCmd.Parameters.AddWithValue("@SSLNo", model.SSLNo);
                    oCmd.Parameters.AddWithValue("@PANNO", model.PANNO);
                    oCmd.Parameters.AddWithValue("@TDS", model.TDS);
                    oCmd.Parameters.AddWithValue("@TDSRate", model.TDSRate);
                    oCmd.Parameters.AddWithValue("@TDSPartyCategery", model.TDSPartyCategery);
                    oCmd.Parameters.AddWithValue("@ResponsibleEmployee", model.ResponsibleEmployee);
                    oCmd.Parameters.AddWithValue("@ResponsibleEmpContactNo", model.ResponsibleEmpContactNo);
                    oCmd.Parameters.AddWithValue("@SalesPersonName", model.SalesPersonName);
                    oCmd.Parameters.AddWithValue("@SalesPersonEmailId", model.SalesPersonEmailId);
                    oCmd.Parameters.AddWithValue("@SalesPersonMobile", model.SalesPersonMobile);
                    oCmd.Parameters.AddWithValue("@PurchPersonName", model.PurchPersonName);
                    oCmd.Parameters.AddWithValue("@PurchasePersonEmailId", model.PurchasePersonEmailId);
                    oCmd.Parameters.AddWithValue("@PurchMobileNo", model.PurchMobileNo);
                    oCmd.Parameters.AddWithValue("@QCPersonEmailId", model.QCPersonEmailId);
                    oCmd.Parameters.AddWithValue("@WebSite_Add", model.WebSite_Add);
                    oCmd.Parameters.AddWithValue("@EMail", model.EMail);
                    oCmd.Parameters.AddWithValue("@RANGE", model.RANGE);
                    oCmd.Parameters.AddWithValue("@Division", model.Division);
                    oCmd.Parameters.AddWithValue("@Commodity", model.Commodity);
                    oCmd.Parameters.AddWithValue("@WorkingAdd1", model.WorkingAdd1);
                    oCmd.Parameters.AddWithValue("@WorkingAdd2", model.WorkingAdd2);
                    oCmd.Parameters.AddWithValue("@RateOfInt", model.RateOfInt);
                    oCmd.Parameters.AddWithValue("@CreditLimit", model.CreditLimit);
                    oCmd.Parameters.AddWithValue("@CreditDays", model.CreditDays);
                    oCmd.Parameters.AddWithValue("@SSL", model.SSL);
                    oCmd.Parameters.AddWithValue("@BankAccount_No", model.BankAccount_No);
                    oCmd.Parameters.AddWithValue("@BankAddress", model.BankAddress);
                    oCmd.Parameters.AddWithValue("@BankIFSCCode", model.BankIFSCCode);
                    oCmd.Parameters.AddWithValue("@BankSwiftCode", model.BankSwiftCode);
                    oCmd.Parameters.AddWithValue("@InterbranchSaleBILL", model.InterbranchSaleBILL);
                    oCmd.Parameters.AddWithValue("@salesperson_name", model.salesperson_name);
                    oCmd.Parameters.AddWithValue("@salesemailid", model.salesemailid);
                    oCmd.Parameters.AddWithValue("@salesmobileno", model.salesmobileno);
                    oCmd.Parameters.AddWithValue("@Approved_By", model.Approved_By);
                    oCmd.Parameters.AddWithValue("@Approved", model.Approved);
                    oCmd.Parameters.AddWithValue("@ApprovalDate", model.ApprovalDate);
                    oCmd.Parameters.AddWithValue("@BlackListed", model.BlackListed);
                    oCmd.Parameters.AddWithValue("@BlackListed_By", model.BlackListed_By);
                    oCmd.Parameters.AddWithValue("@YearCode", model.YearCode);
                    oCmd.Parameters.AddWithValue("@Uid", model.Uid);
                    oCmd.Parameters.AddWithValue("@CC", model.CC);
                    oCmd.Parameters.AddWithValue("@MSMENo", model.MSMENo);
                    oCmd.Parameters.AddWithValue("@MSMEType", model.MSMEType);
                    oCmd.Parameters.AddWithValue("@Region", model.Region);
                    oCmd.Parameters.AddWithValue("@DiscountCategory", model.DiscountCategory);
                    oCmd.Parameters.AddWithValue("@GroupDiscountCategory", model.GroupDiscountCategory);
                    oCmd.Parameters.AddWithValue("@SalePersonEmpId", model.SalePersonEmpId);

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

        internal async Task<ResponseResult> GetSalePersonName()
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
                    oCmd.Parameters.AddWithValue("@Flag", "GetSalePersonName");
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
        public async Task<ResponseResult> GetItemCatCode(string CName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetCategoryCode"));
                SqlParams.Add(new SqlParameter("@TypeName", CName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_AccountMaster", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemGroupCode(string GName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetGroupCode"));
                SqlParams.Add(new SqlParameter("@GroupCodeName", GName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_AccountMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> UpdateMultipleItemDataFromExcel(DataTable ItemDetailGrid, string flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", flag),
            new SqlParameter("@ExcelData", ItemDetailGrid)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_AccountMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
    }
}