using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class DataLogicDAL
    {
        //private readonly IConfiguration configuration;
        //private readonly DataSet? oDataSet = new();
        //private readonly DataTable oDataTable = new();
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        private readonly IConnectionStringHelper _connectionStringHelper;
        public DataLogicDAL(IConfiguration configuration, IConnectionStringHelper connectionStringHelper, ConnectionStringService connectionStringService)
        {
            _connectionStringHelper = connectionStringHelper;
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();

            //DBConnectionString = _connectionStringHelper.GetConnectionStringForCompany();
        }

        public string? GetDBConnection => DBConnectionString;

        //private ResponseResult? _ResponseResult { get; set; }
        private string? DBConnectionString { get; }

        public IList<TextValue> AutoComplete(string Flag, string Column, string FromDate, string ToDate, int ItemCode, int StoreId)
        {
            List<TextValue>? List = new List<TextValue>();
            var oDataTable = new DataTable();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_AutoComplete", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime fromdt= DateTime.Now;
                    DateTime todt= DateTime.Now;
                    if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                    {
                         fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                         todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@ColumnName", Column);
                    oCmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                    oCmd.Parameters.AddWithValue("@storeid", StoreId);
                    oCmd.Parameters.AddWithValue("@FromDate",fromdt == DateTime.Now?DateTime.Now : fromdt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@Todate", todt == DateTime.Now ? DateTime.Now : todt.ToString("yyyy/MM/dd"));
                    myConnection.Open();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }

                    if (oDataTable.Rows.Count > 0)
                    {
                        List = (
                            from DataRow dr in oDataTable.Rows
                            select new TextValue
                            {
                                Text = dr[Column].ToString(),
                                Value = dr[Column].ToString()
                            }
                               ).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                List.Add(new TextValue { Text = ex.Message, Value = ex.Source });
            }
            finally
            {
                oDataTable.Dispose();
            }
            return List;
        }

        public async Task<DataSet> GetDropDownList(string Flag)
        {
            var oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_GetDropDownList", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    await myConnection.OpenAsync();

                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
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
                oDataSet.Dispose();
            }
            return oDataSet;
        }
        public async Task<DataSet> GetDropDownList(string Flag, int AccountCode, string TaxType, string vendorStateCode)
        {
            var oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_GetDropDownList", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@Accountcode", AccountCode);
                    oCmd.Parameters.AddWithValue("@taxtype", TaxType);
                    //oCmd.Parameters.AddWithValue("@state", vendorStateCode);
                    await myConnection.OpenAsync();

                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
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
                oDataSet.Dispose();
            }
            return oDataSet;
        }

        //public async 


        public async Task<IList<TextValue>> GetDropDownList(string Flag, string SPName)
        {
            List<TextValue> _List = new List<TextValue>();
            dynamic Listval = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetDBConnection))
                {
                    SqlCommand oCmd = new SqlCommand(SPName, myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            if (Flag == "Itemgroup_master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Group_name"].ToString(),
                                    Value = Reader["Group_Code"].ToString()
                                };
                            }
                            else if (Flag == "Item_Master_Type")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Type_Item"].ToString(),
                                    Value = Reader["Entry_id"].ToString()
                                };
                            }
                            else if (Flag == "Account_Head_Master_SA")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_name"].ToString(),
                                    Value = Reader["Account_code"].ToString()
                                };
                            }
                            else if (Flag == "Account_Head_Master_PA")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_name"].ToString(),
                                    Value = Reader["Account_code"].ToString()
                                };
                            }
                            else if (Flag == "Store_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Store_Name"].ToString(),
                                    Value = Reader["EntryID"].ToString()
                                };
                            }
                            else if (Flag == "Employee_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Emp_Name"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "EmpNameWithCode")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["EmpNameCode"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "EmpNameNCode")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["EmpNameCode"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "UserMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["UserName"].ToString(),
                                    Value = Reader["UID"].ToString()
                                };
                            }
                            else if (Flag == "Unit_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Unit_Name"].ToString(),
                                    Value = Reader["Unit_Name"].ToString()
                                };
                            }
                            else if (Flag == "UserTypeTB")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["UserType"].ToString(),
                                    Value = Reader["UserType"].ToString()
                                };
                            }
                            else if (Flag == "StateMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["StateName"].ToString(),
                                    Value = Reader["statecode"].ToString()
                                };
                            }
                            else if (Flag == "PendingPOAccountList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_Name"].ToString(),
                                    Value = Reader["Account_Code"].ToString()
                                };
                            }
                            else if (Flag == "GetDiscountCategory")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["DiscountCategory"].ToString(),
                                    Value = Reader["DiscountCategory"].ToString()
                                };
                            } 
                            else if (Flag == "GetGroupDiscountCategory")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["DiscCategoryName"].ToString(),
                                    Value = Reader["DiscCategoryEntryId"].ToString()
                                };
                            }
                            else if (Flag == "GetRegion")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Region"].ToString(),
                                    Value = Reader["Region"].ToString()
                                };
                            }
                            else if (Flag == "VPrimaryAccountHeadMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_Name"].ToString(),
                                    Value = Reader["Account_Code"].ToString()
                                };
                            }
                            else if (Flag == "VPrimaryAccountHeadMasterForBank")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_Name"].ToString(),
                                    Value = Reader["Account_Code"].ToString()
                                };
                            }
                            else if (Flag == "FINISHEDGOODS")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Item_Code"].ToString(),
                                    Value = Reader["PartCode"].ToString()
                                };
                            }
                            else if (
                                Flag == "FINISHEDGOODS"
                                || Flag == "UNFINISHEDGOODS"
                                || Flag == "ALLGOODS"
                                    )
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["ItemName"].ToString(),
                                    Value = Reader["PartCode"].ToString()
                                };
                            }
                            else if (Flag == "SOFOR")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.SOFor].ToString(),
                                    Value = Reader[Constants.SOFor].ToString()
                                };
                            }
                            else if (Flag == "SOTYPE")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.SOType].ToString(),
                                    Value = Reader[Constants.SOType].ToString()
                                };
                            }
                            else if (Flag == "CURRENCY")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.Currency].ToString(),
                                    Value = Reader["Entry_ID"].ToString()
                                };
                            }
                            else if (Flag == "BRANCH")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.CompanyName].ToString(),
                                    Value = Reader[Constants.CompanyName].ToString()
                                };
                            }
                            else if (Flag == "QUOTNO")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.QuotNo].ToString(),
                                    Value = Reader[Constants.EntryID].ToString()
                                };
                            }
                            else if (Flag == "QUOTYEAR")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.QuotNo].ToString(),
                                    Value = Reader[Constants.EntryID].ToString()
                                };
                            }
                            else if (Flag == "TXTYPE")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.TaxType].ToString(),
                                    Value = Reader[Constants.TaxID].ToString()
                                };
                            }
                            else if (Flag == "TXNAME")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.TaxName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "SALEDOC")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.AccountName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "POFOR")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.POFOR].ToString(),
                                    Value = Reader[Constants.POFOR].ToString()
                                };
                            }
                            else if (Flag == "POFOR")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.POFOR].ToString(),
                                    Value = Reader[Constants.POFOR].ToString()
                                };
                            }
                            else if (Flag == "UsedStageList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.StageDesc].ToString(),
                                    Value = Reader[Constants.EntryID].ToString()
                                };
                            }
                            else if (Flag == "DocumentList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.DocName].ToString(),
                                    Value = Reader[Constants.EntryID].ToString()
                                };
                            }
                            else if (Flag == "COSTCENTER")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.CostCenterName].ToString(),
                                    Value = Reader[Constants.EntryID].ToString()
                                };
                            }
                            else if (Flag == "ProcessList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.ProcessName].ToString(),
                                    Value = Reader[Constants.ProcessId].ToString()
                                };
                            }
                            else if (Flag == "FILLVendorList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.VendorName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "FILLDocumentList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.DocumentName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "FillDesignation")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.Designation].ToString(),
                                    Value = Reader[Constants.desigEntryId].ToString()
                                };
                            }
                            else if (Flag == "FillDepartment")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.DeptName].ToString(),
                                    Value = Reader[Constants.DeptId].ToString()
                                };
                            }
                            else if (Flag == "FillCategory")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.EmpCateg].ToString(),
                                    Value = Reader[Constants.CategoryId].ToString()
                                };
                            }
                            else if (Flag == "Shift")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.ShiftName].ToString(),
                                    Value = Reader[Constants.ShiftId].ToString()
                                };
                            }
                            else if (Flag == "FillEmployeeName")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.EmpName].ToString(),
                                    Value = Reader[Constants.EmpyID].ToString()
                                };
                            }
                            else if (Flag == "FillEmployee")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.EmpName].ToString(),
                                    Value = Reader[Constants.EmpyID].ToString()
                                };
                            }
                            else if (Flag == "FillPartCode")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.ItemName].ToString(),
                                    Value = Reader[Constants.Item_Code].ToString()
                                };
                            }
                            else if (Flag == "FillItem")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.ItemName].ToString(),
                                    Value = Reader[Constants.Item_Code].ToString()
                                };
                            }
                            else if (Flag == "FillBatch")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.BatchNo].ToString(),
                                    Value = Reader[Constants.UniqueBatchNo].ToString()
                                };
                            }

                            else if (Flag == "FillUniqueBatchNo")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.BatchNo].ToString(),
                                    Value = Reader[Constants.UniqueBatchNo].ToString()
                                };
                            }
                            else if(Flag == "FillCustomerList")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.AccountCode].ToString(),
                                    Value = Reader[Constants.CustomerName].ToString()
                                };
                            }
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
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
        //ArrayWithOffset 3 parameter
        public async Task<IList<TextValue>> GetDropDownList(string Flag, string CTRL, string SPName)
        {
            List<TextValue> _List = new List<TextValue>();
            dynamic Listval = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetDBConnection))
                {
                    SqlCommand oCmd = new SqlCommand(SPName, myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@CTRL", CTRL);

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            if (Flag == "Itemgroup_master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Group_name"].ToString(),
                                    Value = Reader["Group_Code"].ToString()
                                };
                            }
                            else if (Flag == "Item_Master_Type")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Type_Item"].ToString(),
                                    Value = Reader["Entry_id"].ToString()
                                };
                            }
                            else if (Flag == "Account_Head_Master_SA")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_name"].ToString(),
                                    Value = Reader["Account_code"].ToString()
                                };
                            }
                            else if (Flag == "Account_Head_Master_PA")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_name"].ToString(),
                                    Value = Reader["Account_code"].ToString()
                                };
                            }
                            else if (Flag == "Store_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Store_Name"].ToString(),
                                    Value = Reader["Entry_Id"].ToString()
                                };
                            }
                          
                            else if (Flag == "Employee_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Emp_Name"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "EmpNameWithCode")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["EmpNameCode"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "EmpNameNCode")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["EmpNameCode"].ToString(),
                                    Value = Reader["Emp_Id"].ToString()
                                };
                            }
                            else if (Flag == "UserMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["UserName"].ToString(),
                                    Value = Reader["EmpId"].ToString()
                                };
                            }
                            else if (Flag == "Unit_Master")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Unit_Name"].ToString(),
                                    Value = Reader["Unit_Name"].ToString()
                                };
                            }
                            else if (Flag == "UserTypeTB")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["UserType"].ToString(),
                                    Value = Reader["UserID"].ToString()
                                };
                            }
                            else if (Flag == "StateMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["StateName"].ToString(),
                                    Value = Reader["Entry_Id"].ToString()
                                };
                            }
                            else if (Flag == "VPrimaryAccountHeadMaster")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_Name"].ToString(),
                                    Value = Reader["Account_Code"].ToString()
                                };
                            }
                            else if (Flag == "VPrimaryAccountHeadMasterForBank")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Account_Name"].ToString(),
                                    Value = Reader["Account_Code"].ToString()
                                };
                            }
                            else if (Flag == "FINISHEDGOODS" || Flag == "UNFINISHEDGOODS" || Flag == "ALLGOODS")
                            {
                                if (CTRL == "CODELIST")
                                {
                                    Listval = new TextValue()
                                    {
                                        Text = Reader["Item_Code"].ToString(),
                                        Value = Reader["PartCode"].ToString()
                                    };
                                }
                                if (CTRL == "NAMELIST")
                                {
                                    Listval = new TextValue()
                                    {
                                        Text = Reader["Item_Code"].ToString(),
                                        Value = Reader["ItemName"].ToString()
                                    };
                                }
                            }
                            else if (Flag == "PARTYNAMELIST")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.AccountName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "CREDITORDEBTORLIST")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader[Constants.AccountName].ToString(),
                                    Value = Reader[Constants.AccountCode].ToString()
                                };
                            }
                            else if (Flag == "BRANCHLIST")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["Com_Name"].ToString(),
                                    Value = Reader["Com_Name"].ToString()
                                };
                            }
                            //else if (Flag == "TAXBYTYPE")
                            //{
                            //    Listval = new TextValue()
                            //    {
                            //        Text = Reader["Tax_Name"].ToString(),
                            //        Value = Reader["Account_Code"].ToString()
                            //    };
                            //}

                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Listval.Add(new TextValue { Text = ex.Message, Value = ex.Source });
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
        public async Task<DataSet> GetTDSAccountList(string Flag, int AccountCode, string TaxType)
        {
            var oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_Filtertaxmaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    //oCmd.Parameters.AddWithValue("@Accountcode", AccountCode);
                    oCmd.Parameters.AddWithValue("@TaxType", !string.IsNullOrEmpty(TaxType) ? TaxType : "TDS");
                    await myConnection.OpenAsync();

                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
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
                oDataSet.Dispose();
            }
            return oDataSet;
        }

        public ResponseResult isDuplicate(string ColumnValue, string ColumnName, string TableName)
        {
            ResponseResult? Result = new ResponseResult();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetDBConnection))
                {
                    SqlCommand oCmd = new SqlCommand("SP_Duplicate", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@ColumnValue", ColumnValue);
                    oCmd.Parameters.AddWithValue("@ColumnName", ColumnName);
                    oCmd.Parameters.AddWithValue("@TableName", TableName);
                    oCmd.Parameters.AddWithValue("@Flag", "Duplicate");
                    myConnection.Open();
                    Reader = oCmd.ExecuteReader();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            if (Reader["Duplicate"].ToString() == "0")
                            {
                                Result.Result = Reader["Duplicate"].ToString();
                                Result.StatusText = "Success";
                                Result.StatusCode = HttpStatusCode.OK;
                            }
                            else
                            {
                                Result.Result = Reader["Duplicate"].ToString();
                                Result.StatusText = "Error";
                                Result.StatusCode = HttpStatusCode.Ambiguous;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.StatusCode = HttpStatusCode.InternalServerError;
                Result.StatusText = "Exception";
                Result.Result = ex.Message.ToString();
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return Result;
        }

        internal async Task<ResponseResult> ExecuteDataSet(string SPName, IList<dynamic> SQLParams)
        {
            var _ResponseResult = new ResponseResult();
            var oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand(SPName, myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        foreach (dynamic param in SQLParams)
                        {
                            oCmd.Parameters.Add(param);
                        }
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            //WRITE TABLENAME AFTER DATASET
                            oDataAdapter.Fill(oDataSet);
                        }

                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataSet
                            };
                        }
                        else
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.InternalServerError,
                                StatusText = "Error",
                                Result = oDataSet
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
                oDataSet.Dispose();
            }
            return _ResponseResult;
        }

        internal async Task<ResponseResult> ExecuteDataTable(string SPName, IList<dynamic> SQLParams)
        {
            int retryCount = 3;
            int delayMilliseconds = 1000;
            var _ResponseResult = new ResponseResult();
                var oDataTable = new DataTable();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand(SPName, myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddRange(SQLParams.ToArray());
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            //  oDataAdapter.Fill(oDataTable);
                            for (int i = 0; i < retryCount; i++)
                            {
                                try
                                {
                                    oDataAdapter.Fill(oDataTable);
                                    break; // Success - exit loop
                                }
                                catch (SqlException ex) when (i < retryCount - 1 &&
                                                             (ex.Number == -2 || ex.Message.Contains("timeout")))
                                {
                                    await Task.Delay(delayMilliseconds);
                                    delayMilliseconds *= 2; // Exponential backoff
                                }
                            }
                            //oCmd.Parameters.Clear();
                            //await oCmd.DisposeAsync();
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = !oDataTable.Columns.Contains("StatusCode") ? HttpStatusCode.OK : (HttpStatusCode)oDataTable.Rows[0]["StatusCode"],
                                StatusText = !oDataTable.Columns.Contains("StatusText") ? "Success" : oDataTable.Rows[0]["StatusText"].ToString(),
                                Result = oDataTable
                            };
                        }
                        else
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.InternalServerError,
                                StatusText = "Error",
                                Result = oDataTable
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
            }
            finally
            {
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }

        internal async Task<object?> ExecuteScalar(string SPName, IList<dynamic> SQLParams)
        {
            object Result;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand(SPName, myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddRange(SQLParams.ToArray());
                        await myConnection.OpenAsync();
                        Result = (dynamic)oCmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
            }
            return Result;
        }

        internal async Task<DataSet> GetDataSet(string FormName, string SPName, List<KeyValuePair<string, string>> SQLParams)
        {
            var oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand(SPName, myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        foreach (var param in SQLParams)
                        {
                            oCmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataSet);
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
                oDataSet.Dispose();
            }
            return oDataSet;
        }

        internal int GetEntryID(string TableName, int YearCode, string ColName, string Yearcodecolumn)
        {
            int Result = 0;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("GetMaxEntryID", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@tblName", TableName);
                        oCmd.Parameters.AddWithValue("@yearCode", YearCode);
                        oCmd.Parameters.AddWithValue("@IDColumnName", ColName);
                        oCmd.Parameters.AddWithValue("@YearCodeColumnName", Yearcodecolumn);

                        myConnection.Open();
                        Result = Convert.ToInt32(oCmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return Result;
        }

        internal int IsDelete(int ID, string Type)
        {
            int Result = 0;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SP_IsDelete", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@ItemCode", ID);
                        oCmd.Parameters.AddWithValue("@Type", Type);
                        myConnection.Open();
                        Result = Convert.ToInt32(oCmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return Result;
        }
        public async Task<ResponseResult> GetDbCrDataGrid(DataTable DbCrGridd, DataTable TaxDetailDT, DataTable TDSDetailDT, string FormName,int? docAccountCode, int? AccountCode, decimal? ItemNetAmount, decimal? NetTotal)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@VendCustAccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@documentAccountCode", docAccountCode ?? 0));
                SqlParams.Add(new SqlParameter("@NetAmt", NetTotal ?? 0)); //ItemTotal + TaxAmount
                SqlParams.Add(new SqlParameter("@BillAmt", ItemNetAmount ?? 0)); //TotalItemNetAmount
                SqlParams.Add(new SqlParameter("@Formname", FormName));

                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));
                SqlParams.Add(new SqlParameter("@DTGrid", DbCrGridd));
                if(TDSDetailDT.Rows.Count != 0)
                {
                   SqlParams.Add(new SqlParameter("@DtTDS", TDSDetailDT));
                }

                _ResponseResult = await ExecuteDataTable("Sp_SetDRCrAmountForBill", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public static DateTime ParseDate(string dateString, string dateType)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, dateType.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<AdjustmentModel> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate, string Flag)
        {
            var oDataSet = new DataSet();
            var _List = new List<AdjustmentModel>();
            var ModelList = new AdjustmentModel();
            var SqlParams = new List<dynamic>();
            //var listObject = new List<DeliverySchedule>();

            try
            {
                DateTime TransVouchDt = new DateTime();
                TransVouchDt = ParseDate(TransVouchDate, "dd/MMM/yyyy") == default ? DateTime.Now : ParseDate(TransVouchDate, "dd/MMM/yyyy");

                SqlParams.Add(new SqlParameter("@flag", ""));
                SqlParams.Add(new SqlParameter("@TransYearcode", YC ?? 0));
                SqlParams.Add(new SqlParameter("@ACCOUNTCODE", AC));
                SqlParams.Add(new SqlParameter("@AMOUNTTYPEDRCR", DRCR));
                SqlParams.Add(new SqlParameter("@TransVoucherTYpe", TransVouchType));
                SqlParams.Add(new SqlParameter("@TransvoucherDAte", TransVouchDt));
                SqlParams.Add(new SqlParameter("@PayRecENtryId", PayRecEntryId ?? 0));
                SqlParams.Add(new SqlParameter("@PayRecYearcode", PayRecYearcode ?? 0));

                var ResponseResult = await ExecuteDataSet("Sp_GetPendingVouchBillForAgainstRefAdjustmentInBILL", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0 && ResponseResult != null)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "GetPendingVouchBill";

                    int cnt = 1;

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var vouchdate = new DateTime();
                            vouchdate = !string.IsNullOrEmpty(row["VOUCHERDATE"].ToString()) ? DateTime.Parse(row["VOUCHERDATE"].ToString()) : new DateTime();
                            _List.Add(new AdjustmentModel
                            {
                                AdjSeqNo = cnt,
                                AdjAgnstAccEntryID = !string.IsNullOrEmpty(row["ENTRYID"].ToString()) ? Convert.ToInt32(row["ENTRYID"].ToString()) : 0,
                                AdjAgnstAccYearCode = !string.IsNullOrEmpty(row["YEAR_CODE"].ToString()) ? Convert.ToInt32(row["YEAR_CODE"].ToString()) : 0,
                                AdjAgnstOpenEntryID = !string.IsNullOrEmpty(row["OpnEntryId"].ToString()) ? Convert.ToInt32(row["OpnEntryId"].ToString()) : 0,
                                AdjAgnstOpeningYearCode = !string.IsNullOrEmpty(row["OpnYearCode"].ToString()) ? Convert.ToInt32(row["OpnYearCode"].ToString()) : 0,
                                AdjAgnstModeOfAdjstment = row["MODOFADJUST"].ToString(),
                                AdjAgnstVouchNo = row["VOUCHERNO"].ToString(),
                                AdjAgnstVouchDate = vouchdate,
                                AdjAgnstVouchType = row["VOUCHERTYPE"].ToString(),
                                AdjAgnstPendAmt = !string.IsNullOrEmpty(row["BillNetAmt"].ToString()) ? Convert.ToSingle(row["BillNetAmt"].ToString()) : 0,
                                AdjAgnstRemainingAmt = !string.IsNullOrEmpty(row["RemainingBalanceAmt"].ToString()) ? Convert.ToSingle(row["RemainingBalanceAmt"].ToString()) : 0,
                                AdjAgnstAdjstedAmt = !string.IsNullOrEmpty(row["ADJUSTEDAMT"].ToString()) ? Convert.ToSingle(row["ADJUSTEDAMT"].ToString()) : 0,
                                AdjAgnstDrCrName = !string.IsNullOrEmpty(row["DR/CR"].ToString()) ? row["DR/CR"].ToString() : string.Empty,
                                AdjAgnstDrCr = !string.IsNullOrEmpty(row["DR/CR"].ToString()) ? row["DR/CR"].ToString() : string.Empty,
                                AdjAgnstTransType = !string.IsNullOrEmpty(row["TransType"].ToString()) ? row["TransType"].ToString() : string.Empty,
                            });
                            cnt++;
                        }
                        ModelList.AdjAdjustmentDetailGrid = _List;
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
                oDataSet.Dispose();
            }
            return ModelList;
        }
        public async Task<IList<TextValue>> GetDropDownListWithCustomeVar(string SPName, Dictionary<string, object> parameters, bool? IsTextandValueSame = false, bool? IsValueInFirstcolumn = true)
        {
            List<TextValue> _List = new List<TextValue>();
            SqlDataReader reader = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetDBConnection))
                {
                    SqlCommand oCmd = new SqlCommand(SPName, myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    foreach (var param in parameters)
                    {
                        oCmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    await myConnection.OpenAsync();
                    reader = await oCmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var Listval = new TextValue();
                            if (Convert.ToBoolean(IsTextandValueSame))
                            {
                                Listval = new TextValue()
                                {
                                    Text = reader[0].ToString(),
                                    Value = reader[0].ToString()
                                };
                            }
                            else
                            {
                                if (Convert.ToBoolean(IsValueInFirstcolumn))
                                {
                                    Listval = new TextValue()
                                    {
                                        Text = reader[0].ToString(),
                                        Value = reader[1].ToString()
                                    };
                                }
                                else
                                {
                                    Listval = new TextValue()
                                    {
                                        Text = reader[1].ToString(),
                                        Value = reader[0].ToString()
                                    };
                                }
                            }
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _List.Add(new TextValue { Text = ex.Message, Value = ex.Source });
            }
            finally
            {
                if (reader != null)
                {
                    await reader.CloseAsync();
                    await reader.DisposeAsync();
                }
            }

            return _List;
        }
    }
}