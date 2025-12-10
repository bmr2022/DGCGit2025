using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection.Metadata;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ItemCategoryDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        private readonly IDataLogic _DataLogicDAL;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public ItemCategoryDAL(IConfiguration configuration, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            // DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Item Category"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Item Category"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetFeatureOption()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FeatureOption"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SPItemCategory", SqlParams);
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
                    SqlCommand oCmd = new SqlCommand("SPItemCategory", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Entry_id", ID);
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

        public async Task<ItemCategoryModel> GetByID(int ID)
        {
            ItemCategoryModel? _ItemCategoryModel = new ItemCategoryModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPItemCategory", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Entry_id", ID);
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

                        _ItemCategoryModel.Entry_id = Convert.ToInt32(dr["Entry_id"].ToString());
                        _ItemCategoryModel.Entry_Date = Convert.ToDateTime(dr["Entry_Date"]);
                        _ItemCategoryModel.Year_code = Convert.ToInt32(dr["Year_code"].ToString());
                        _ItemCategoryModel.Type_Item = Convert.ToString(dr["Type_Item"].ToString());
                        _ItemCategoryModel.Main_Category_Type = Convert.ToString(dr["Main_Category_Type"].ToString());
                        _ItemCategoryModel.Uid = Convert.ToInt32(dr["Uid"].ToString());
                        _ItemCategoryModel.CC = Convert.ToString(dr["CC"].ToString());
                        _ItemCategoryModel.Category_Code = dr["Category_Code"].ToString();
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

            return _ItemCategoryModel;
        }

        public async Task<ItemCategoryModel> GetDashboardData(ItemCategoryModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPItemCategory", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.ItemCatList = (from DataRow dr in oDataSet.Tables[0].Rows
                                         select new ItemCategoryModel
                                         {
                                             Entry_id = Convert.ToInt32(dr["Entry_id"]),
                                             Entry_Date = Convert.ToDateTime(dr["Entry_Date"]),
                                             Year_code = Convert.ToInt32(dr["Year_code"].ToString()),
                                             Type_Item = dr["Type_Item"].ToString(),
                                             Main_Category_Type = dr["Main_Category_Type"].ToString(),
                                             CC = dr["CC"].ToString(),
                                             Uid = Convert.ToInt32(dr["Uid"].ToString()),
                                             Category_Code = dr["Category_Code"].ToString(),
                                         }).ToList();
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
        public async Task<ItemCategoryModel> GetSearchData(ItemCategoryModel model, string CategoryName, string TypeItem)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPItemCategory", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Type_Item", TypeItem);
                    oCmd.Parameters.AddWithValue("@Main_Category_Type", CategoryName);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.ItemCatList = (from DataRow dr in oDataSet.Tables[0].Rows
                                         select new ItemCategoryModel
                                         {
                                             Entry_id = Convert.ToInt32(dr["Entry_id"]),
                                             Entry_Date = Convert.ToDateTime(dr["Entry_Date"]),
                                             Year_code = Convert.ToInt32(dr["Year_code"].ToString()),
                                             Type_Item = dr["Type_Item"].ToString(),
                                             Main_Category_Type = dr["Main_Category_Type"].ToString(),
                                             CC = dr["CC"].ToString(),
                                             Uid = Convert.ToInt32(dr["Uid"].ToString()),
                                             Category_Code = dr["Category_Code"].ToString(),
                                         }).ToList();
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

        public async Task<ResponseResult> CheckBeforeUpdate(int Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                // if(Mode=="U")
                SqlParams.Add(new SqlParameter("@Flag", "CheckBeforeUpdate"));
                SqlParams.Add(new SqlParameter("@Type", Type));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SPItemCategory", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllItemCategory()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                // if(Mode=="U")
                SqlParams.Add(new SqlParameter("@Flag", "GetAllItemCategory"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SPItemCategory", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveItemCategory(ItemCategoryModel model)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPItemCategory", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (model.Year_code == 0)
                    {
                        model.Year_code = Constants.FinincialYear;
                        model.Entry_Date = DateTime.Today;
                        model.CC = Constants.CC;
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Entry_id", model.Entry_id);
                    oCmd.Parameters.AddWithValue("@Entry_Date", model.Entry_Date);
                    //                    oCmd.Parameters.AddWithValue("@Year_code", model.Year_code);
                    oCmd.Parameters.AddWithValue("@Year_code", model.Year_code);
                    oCmd.Parameters.AddWithValue("@Type_Item", model.Type_Item);
                    oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
                    oCmd.Parameters.AddWithValue("@cc", model.CC);
                    //oCmd.Parameters.AddWithValue("@cc", "CC");
                    oCmd.Parameters.AddWithValue("@Uid", model.Uid);
                    //oCmd.Parameters.AddWithValue("@Uid", 1);
                    //oCmd.Parameters.AddWithValue("@Category_Code", "111");
                    oCmd.Parameters.AddWithValue("@Category_Code", model.Category_Code);
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

        internal Task<ItemCategoryModel> GetDropDownList(ItemCategoryModel model)
        {
            throw new NotImplementedException();
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

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SPItemCategory", SqlParams);
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
