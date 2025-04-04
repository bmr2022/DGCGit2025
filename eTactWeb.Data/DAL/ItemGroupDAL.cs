using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ItemGroupDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration _configuration;

        public ItemGroupDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //_configuration = configuration;
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
                    SqlCommand oCmd = new SqlCommand("SP_ItemGroup", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Group_Code", ID);
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
                SqlParams.Add(new SqlParameter("@MainMenu", "Item Group"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Item Group"));

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
        public async Task<ResponseResult> GetUnderCategory(string Mode, string Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                // if(Mode=="U")
                SqlParams.Add(new SqlParameter("@Flag", "ItemMasterCategory"));
                //else
                //SqlParams.Add(new SqlParameter("@Flag", "ItemGroupCategory"));


                SqlParams.Add(new SqlParameter("@Type", Type));

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
        public async Task<ResponseResult> CheckBeforeUpdate(int GroupCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                // if(Mode=="U")
                SqlParams.Add(new SqlParameter("@Flag", "CheckBeforeUpdate"));
                //else
                //SqlParams.Add(new SqlParameter("@Flag", "ItemGroupCategory"));


                SqlParams.Add(new SqlParameter("@Group_Code", GroupCode));

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
        public async Task<ResponseResult> GetAllItemGroup()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAllItemGroup"));


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
        public async Task<ItemGroupModel> GetByID(int ID)
        {
            ItemGroupModel? _ItemGroupModel = new ItemGroupModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemGroup", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Group_code", ID);
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
                        //Group_Code,Group_name,Entry_date,GroupCatCode,ItemCategory,Main_Category_Type ,Under_GroupCode,UnderCategoryId
                        _ItemGroupModel.Group_Code = Convert.ToInt32(dr["Group_Code"].ToString());
                        _ItemGroupModel.Group_name = dr["Group_name"].ToString();
                        _ItemGroupModel.Under_GroupCode = Convert.ToInt32(dr["Under_GroupCode"].ToString());
                        _ItemGroupModel.Entry_date = dr["Entry_date"].ToString();
                        _ItemGroupModel.GroupPrefix = dr["GroupPrefix"].ToString();
                        _ItemGroupModel.UnderCategoryId = Convert.ToInt32(dr["UnderCategoryId"].ToString());
                        _ItemGroupModel.ItemCategory = dr["ItemCategory"].ToString();
                        _ItemGroupModel.ItemServAssets = dr["Main_Category_Type"].ToString();

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

            return _ItemGroupModel;
        }
        public async Task<ItemGroupModel> GetDashboardData(ItemGroupModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemGroup", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Group_name", model.Group_name);
                    oCmd.Parameters.AddWithValue("@Itemcategory", model.ItemCategory);
                    oCmd.Parameters.AddWithValue("@MainCategory", model.Main_Category_Type);

                    //oCmd.Parameters.AddWithValue("@ItemCategory", model.ItemCategory);
                    // oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.ItemGroupList = (from DataRow dr in oDataSet.Tables[0].Rows
                                           select new ItemGroupModel
                                           {
                                               //,,,,,  
                                               Group_Code = Convert.ToInt32(dr["Group_Code"]),
                                               Group_name = dr["Group_name"].ToString(),
                                               Entry_date = dr["Entry_date"].ToString(),
                                               GroupPrefix = Convert.ToString(dr["GroupPrefix"]),
                                               ItemCategory = Convert.ToString(dr["ItemCategory"]),
                                               Main_Category_Type = dr["Main_Category_Type"].ToString()
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
                    foreach (ItemGroupModel? item in model.ItemGroupList)
                    {
                        TextValue? _lst = new TextValue
                        {
                            Text = item.Group_name,
                            Value = item.Group_Code.ToString()
                        };
                        _list.Add(_lst);
                    }

                    model.ItemGroupList = (IList<ItemGroupModel>)_list;
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
                        if (Flag == "Itemgroup_master")
                        {
                            List = (from DataRow dr in oDataTable.Rows
                                    select new TextValue
                                    {
                                        Text = dr["group_name"].ToString(),
                                        Value = dr["group_code"].ToString()
                                    }).ToList();
                        }

                        if (Flag == "Item_Master_Type")
                        {
                            List = (from DataRow dr in oDataTable.Rows
                                    select new TextValue
                                    {
                                        Text = dr["Type_Item"].ToString(),
                                        Value = dr["Entry_id"].ToString()
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
        public async Task<ResponseResult> SaveItemGroup(ItemGroupModel model)
        {
            dynamic _ResponseResult = null;
            try
            {
                DateTime EntryDt = new DateTime();
                EntryDt = ParseDate(model.Entry_date);

                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemGroup", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Group_Code", model.Group_Code);
                    oCmd.Parameters.AddWithValue("@Group_name", model.Group_name);
                    oCmd.Parameters.AddWithValue("@Entry_date", EntryDt == default ? string.Empty : EntryDt);
                    oCmd.Parameters.AddWithValue("@Under_GroupCode", model.Under_GroupCode);
                    oCmd.Parameters.AddWithValue("@GroupPrefix", model.GroupPrefix);
                    oCmd.Parameters.AddWithValue("@UnderCategoryId", model.UnderCategoryId);
                    oCmd.Parameters.AddWithValue("@seqNo", model.seqNo);
                    oCmd.Parameters.AddWithValue("@ItemServAssets", model.ItemServAssets);
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

    }
}
