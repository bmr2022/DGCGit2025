using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ItemMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _DataLogicDAL;
        //private readonly IConfiguration configuration;
        private dynamic? _ResponseResult;
        private readonly ConnectionStringService _connectionStringService;

        private IDataReader? Reader;

        public ItemMasterDAL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = config.GetConnectionString("eTactDB");
            _DataLogicDAL = dataLogicDAL;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Item Master"));                

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
        public async Task<ResponseResult> GetPartCode(int ParentCode, int ItemType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GeneratePartCode"));
                SqlParams.Add(new SqlParameter("@ParentCode", ParentCode));
                SqlParams.Add(new SqlParameter("@ItemType", ItemType));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemCategory(string ItemServAssets)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetCategoryList"));
                SqlParams.Add(new SqlParameter("@ItemServAssets", ItemServAssets));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetItemGroup(string ItemServAssets)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetGroupList"));
                SqlParams.Add(new SqlParameter("@ItemServAssets", ItemServAssets));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

		public async Task<ResponseResult> GetUnitList()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "GetUnitList"));
				

				_ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemMasterData", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}
			return _ResponseResult;
		}
		public async Task<ResponseResult> GetProdInWorkcenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "LoadProdInWorkcenter"));                
                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemMasterData", SqlParams);
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
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
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

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetWorkCenterId(string WorkCenterDescription)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetWorkCenterId"));
                SqlParams.Add(new SqlParameter("@WorkCenterDescription", WorkCenterDescription));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> DeleteItemByID(int ID)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                SqlParams.Add(new SqlParameter("@Item_Code", ID));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;            
        }

        public async Task<IList<ItemMasterModel>> GetAllItemMaster(string Flag)
        {
            List<ItemMasterModel> ItemMasterList = new List<ItemMasterModel>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    DataTable oDataTable = new DataTable("New");
                    SqlCommand oCmd = new SqlCommand("SP_ItemMasterData", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        oDataTable.Load(Reader);

                        foreach (DataRow dr in oDataTable.Rows)
                        {
                            ItemMasterList.Add(new ItemMasterModel
                            {
                                Item_Code = Convert.ToInt32(dr["Item_Code"]),
                                PartCode = dr["PartCode"].ToString(),
                                Item_Name = dr["Item_Name"].ToString(),
                                ParentCode = Convert.ToInt32(dr["ParentCode"]),
                                EntryDate = dr["EntryDate"].ToString(),
                                LastUpdatedDate = dr["LastUpdatedDate"].ToString(),
                                LeadTime = Convert.ToInt32(dr["LeadTime"]),
                                CC = dr["CC"].ToString(),
                                Unit = dr["Unit"].ToString(),
                                SalePrice = Convert.ToInt32(dr["SalePrice"]),
                                PurchasePrice = Convert.ToInt32(dr["PurchasePrice"]),
                                CostPrice = Convert.ToInt32(dr["CostPrice"]),
                                WastagePercent = Convert.ToInt32(dr["WastagePercent"]),
                                WtSingleItem = Convert.ToInt32(dr["WtSingleItem"]),
                                NoOfPcs = Convert.ToInt32(dr["NoOfPcs"]),
                                QcReq = dr["QcReq"].ToString(),
                                ItemType = Convert.ToInt32(dr["ItemType"]),
                                ImageURL = dr["UploadImage"].ToString(),
                                UID = dr["UID"].ToString(),
                                DrawingNo = dr["DrawingNo"].ToString(),
                                MinimumLevel = Convert.ToInt32(dr["MinimumLevel"]),
                                MaximumLevel = Convert.ToInt32(dr["MaximumLevel"]),
                                ReorderLevel = Convert.ToInt32(dr["ReorderLevel"]),
                                YearCode = Convert.ToInt32(dr["YearCode"]),
                                AlternateUnit = dr["AlternateUnit"].ToString(),
                                RackID = dr["RackID"].ToString(),
                                BinNo = dr["BinNo"].ToString(),
                                ItemSize = dr["ItemSize"].ToString(),
                                Colour = dr["Colour"].ToString(),
                                NeedPO = dr["NeedPO"].ToString(),
                                StdPacking = Convert.ToInt32(dr["StdPacking"]),
                                PackingType = dr["PackingType"].ToString(),
                                ModelNo = dr["ModelNo"].ToString(),
                                YearlyConsumedQty = Convert.ToInt32(dr["YearlyConsumedQty"]),
                                DispItemName = dr["DispItemName"].ToString(),
                                PurchaseAccountcode = dr["PurchaseAccountcode"].ToString(),
                                SaleAccountcode = dr["SaleAccountcode"].ToString(),
                                MinLevelDays = Convert.ToInt32(dr["MinLevelDays"]),
                                MaxLevelDays = Convert.ToInt32(dr["MaxLevelDays"]),
                                EmpName = dr["EmpName"].ToString(),
                                DailyRequirment = Convert.ToInt32(dr["DailyRequirment"]),
                                Stockable = dr["Stockable"].ToString(),
                                WipStockable = dr["WipStockable"].ToString(),
                                Store = dr["Store"].ToString(),
                                ProductLifeInus = Convert.ToInt32(dr["ProductLifeInus"]),
                                ItemDesc = dr["ItemDesc"].ToString(),
                                MaxWipStock = Convert.ToInt32(dr["MaxWipStock"]),
                                NeedSo = dr["NeedSo"].ToString(),
                                BomRequired = dr["BomRequired"].ToString(),
                                JobWorkItem = dr["JobWorkItem"].ToString(),
                                HSNNO = Convert.ToInt32(dr["HsnNo"]),
                                CreatedBy = Convert.ToInt32(dr["CreatedBy"]),
                                CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                                Active = dr["Active"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                Error.Procedure = ((SqlException)ex).Procedure;
                Error.Server = ((SqlException)ex).Server;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return ItemMasterList;
        }

        public async Task<IList<ItemMasterModel>> GetDashBoardData(string ItemName, string PartCode, string ItemGroup, string ItemCategory, string HsnNo, string UniversalPartCode, string Flag)
        {
            List<ItemMasterModel> ItemMasterList = new List<ItemMasterModel>();

            DataSet oDataSet = new DataSet();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_FiletrItemMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemGroup", ItemGroup);
                    oCmd.Parameters.AddWithValue("@ItemCategory", ItemCategory);
                    oCmd.Parameters.AddWithValue("@HsnNo", HsnNo);
                    oCmd.Parameters.AddWithValue("@UniversalPartCode", UniversalPartCode);
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                for(int i = 0; i < oDataSet.Tables[0].Rows.Count;i++)
                {
                    ItemMasterList.Add(new ItemMasterModel
                    {
                        Item_Code = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[0]),
                        PartCode = oDataSet.Tables[0].Rows[i].ItemArray[1].ToString(),
                        Item_Name = oDataSet.Tables[0].Rows[i].ItemArray[2].ToString(),
                        ParentCode = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[3]),
                        ItemGroup = oDataSet.Tables[0].Rows[i].ItemArray[4].ToString(),
                        EntryDate = oDataSet.Tables[0].Rows[i].ItemArray[5].ToString(),
                        LastUpdatedDate = oDataSet.Tables[0].Rows[i].ItemArray[6].ToString(),
                        LeadTime = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[7]),
                        CC = oDataSet.Tables[0].Rows[i].ItemArray[8].ToString(),
                        Unit = oDataSet.Tables[0].Rows[i].ItemArray[9].ToString(),
                        SalePrice = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[10]),
                        PurchasePrice = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[11]),
                        CostPrice = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[12]),
                        WastagePercent = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[13]),
                        WtSingleItem = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[14]),
                        NoOfPcs = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[15]),
                        QcReq = oDataSet.Tables[0].Rows[i].ItemArray[16].ToString(),
                        ItemType = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[17]),
                        TypeName = oDataSet.Tables[0].Rows[i].ItemArray[18].ToString(),
                        ImageURL = oDataSet.Tables[0].Rows[i].ItemArray[19].ToString(),
                        UID = oDataSet.Tables[0].Rows[i].ItemArray[20].ToString(),
                        DrawingNo = oDataSet.Tables[0].Rows[i].ItemArray[21].ToString(),
                        MinimumLevel = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[22]),
                        MaximumLevel = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[23]),
                        ReorderLevel = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[24]),
                        YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[25]),
                        AlternateUnit = oDataSet.Tables[0].Rows[i].ItemArray[26].ToString(),
                        RackID = oDataSet.Tables[0].Rows[i].ItemArray[27].ToString(),
                        BinNo = oDataSet.Tables[0].Rows[i].ItemArray[28].ToString(),
                        ItemSize = oDataSet.Tables[0].Rows[i].ItemArray[29].ToString(),
                        Colour = oDataSet.Tables[0].Rows[i].ItemArray[30].ToString(),
                        NeedPO = oDataSet.Tables[0].Rows[i].ItemArray[31].ToString(),
                        StdPacking = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[32]),
                        PackingType = oDataSet.Tables[0].Rows[i].ItemArray[33].ToString(),
                        ModelNo = oDataSet.Tables[0].Rows[i].ItemArray[34].ToString(),
                        YearlyConsumedQty = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[35]),
                        DispItemName = oDataSet.Tables[0].Rows[i].ItemArray[36].ToString(),
                        PurchaseAccountcode = oDataSet.Tables[0].Rows[i].ItemArray[37].ToString(),
                        SaleAccountcode = oDataSet.Tables[0].Rows[i].ItemArray[38].ToString(),
                       
                        MinLevelDays = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[39]),
                        MaxLevelDays = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[40]),
                        EmpName = oDataSet.Tables[0].Rows[i].ItemArray[41].ToString(),
                        DailyRequirment = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[42]),
                        Stockable = oDataSet.Tables[0].Rows[i].ItemArray[43].ToString(),
                        WipStockable = oDataSet.Tables[0].Rows[i].ItemArray[44].ToString(),
                        Store = oDataSet.Tables[0].Rows[i].ItemArray[45].ToString(),
                        ProductLifeInus = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[46]),
                        ItemDesc = oDataSet.Tables[0].Rows[i].ItemArray[47].ToString(),
                        MaxWipStock = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[48]),
                        NeedSo = oDataSet.Tables[0].Rows[i].ItemArray[49].ToString(),
                        BomRequired = oDataSet.Tables[0].Rows[i].ItemArray[50].ToString(),
                        HSNNO = Convert.ToInt32(oDataSet.Tables[0].Rows[i].ItemArray[52]),
                        CreatedByName = oDataSet.Tables[0].Rows[i].ItemArray[53].ToString(),
                        CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[i].ItemArray[54].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[i].ItemArray[54].ToString()),
                        UpdatedByName = oDataSet.Tables[0].Rows[i].ItemArray[56].ToString(),
                        UpdatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[i].ItemArray[55].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[i].ItemArray[55].ToString()),
                        Active = oDataSet.Tables[0].Rows[i].ItemArray[57].ToString(),
                        UniversalPartCode = oDataSet.Tables[0].Rows[i].ItemArray[58].ToString(),
                        UniversalDescription = oDataSet.Tables[0].Rows[i].ItemArray[59].ToString(),
                        ProdWorkCenterDescription = oDataSet.Tables[0].Rows[i].ItemArray[60].ToString(),
                        ProdInhouseJW = oDataSet.Tables[0].Rows[i].ItemArray[61].ToString(),
                        BatchNO = oDataSet.Tables[0].Rows[i].ItemArray[62].ToString(),
                        VoltageVlue = oDataSet.Tables[0].Rows[i].ItemArray[63].ToString(),
                        OldPartCode = oDataSet.Tables[0].Rows[i].ItemArray[64].ToString(),
                        SerialNo = oDataSet.Tables[0].Rows[i].ItemArray[65].ToString(),
                        Package = oDataSet.Tables[0].Rows[i].ItemArray[66].ToString(),
                    });
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

            return ItemMasterList;
        }

        public FeatureOption GetFeatureOption()
        {
            FeatureOption? FO = new FeatureOption();
            DataTable? oDataTable = new DataTable();
            try
            {
                using SqlConnection myConnection = new SqlConnection(DBConnectionString);
                SqlCommand oCmd = new SqlCommand("SP_ItemMasterData", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "FeatureOption");
                myConnection.Open();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataTable);
                }

                if (oDataTable.Rows.Count != 0)
                {
                    FO.AllowPartCode = oDataTable.Rows[0]["AutoGen_PartCode"].ToString() == "Y" ? false : true;
                    FO.DuplicateItemName = oDataTable.Rows[0]["DuplicateItemName"].ToString() == "Y" ? true : false;
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.WriteLine(e);
            }

            return FO;
        }

        public int GetisDelete()
        {
            DataTable? oDataTable = new DataTable();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemMasterData", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "GetByID");
                    myConnection.Open();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.WriteLine(e);
            }

            return 0;
        }

        public async Task<ItemMasterModel> GetItemMasterByID(int ID)
        {
            DataTable? oDataTable = new DataTable();
            ItemMasterModel? model = new ItemMasterModel();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemMasterData", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "GetByID");
                    oCmd.Parameters.AddWithValue("@Item_Code", ID);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }

                if (oDataTable.Rows.Count != 0)
                {
                    foreach (DataRow dr in oDataTable.Rows)
                    {
                        model.IsDelete = Convert.ToInt32(dr["IsDelete"]);
                        model.Item_Code = Convert.ToInt32(dr["Item_Code"]);
                        model.PartCode = dr["PartCode"].ToString();
                        model.Item_Name = dr["Item_Name"].ToString();
                        model.ParentCode = Convert.ToInt32(dr["ParentCode"]);
                        model.EntryDate = dr["EntryDate"].ToString();
                        model.LastUpdatedDate = dr["LastUpdatedDate"].ToString();
                        model.LeadTime = Convert.ToInt32(dr["LeadTime"]);
                        model.CC = dr["CC"].ToString();
                        model.Unit = dr["Unit"].ToString();
                        model.SalePrice = Convert.ToDecimal(dr["SalePrice"]);
                        model.PurchasePrice = Convert.ToDecimal(dr["PurchasePrice"]);
                        model.CostPrice = Convert.ToDecimal(dr["CostPrice"]);
                        model.WastagePercent = Convert.ToDecimal(dr["WastagePercent"]);
                        model.WtSingleItem = Convert.ToDecimal(dr["WtSingleItem"]);
                        model.NoOfPcs = Convert.ToDecimal(dr["NoOfPcs"]);
                        model.QcReq = dr["QcReq"].ToString();
                        model.ItemType = Convert.ToInt32(dr["ItemType"]);
                        model.ImageURL = dr["UploadImage"].ToString();
                        model.ItemImageURL = dr["UploadItemImage"].ToString();
                        model.UID = dr["UID"].ToString();
                        model.DrawingNo = dr["DrawingNo"].ToString();
                        model.MinimumLevel = Convert.ToDecimal(dr["MinimumLevel"]);
                        model.MaximumLevel = Convert.ToDecimal(dr["MaximumLevel"]);
                        model.ReorderLevel = Convert.ToDecimal(dr["ReorderLevel"]);
                        model.YearCode = Convert.ToInt32(dr["YearCode"]);
                        model.AlternateUnit = dr["AlternateUnit"].ToString();
                        model.RackID = dr["RackID"].ToString();
                        model.BinNo = dr["BinNo"].ToString();
                        model.ItemSize = dr["ItemSize"].ToString();
                        model.Colour = dr["Colour"].ToString();
                        model.NeedPO = dr["NeedPO"].ToString();
                        model.StdPacking = Convert.ToDecimal(dr["StdPacking"]);
                        model.PackingType = dr["PackingType"].ToString();
                        model.ModelNo = dr["ModelNo"].ToString();
                        model.YearlyConsumedQty = Convert.ToInt32(dr["YearlyConsumedQty"]);
                        model.DispItemName = dr["DispItemName"].ToString();
                        model.PurchaseAccountcode = dr["PurchaseAccountcode"].ToString();
                        model.SaleAccountcode = dr["SaleAccountcode"].ToString();
                        model.MinLevelDays = Convert.ToDecimal(dr["MinLevelDays"]);
                        model.MaxLevelDays = Convert.ToDecimal(dr["MaxLevelDays"]);
                        model.EmpName = dr["EmpName"].ToString();
                        model.DailyRequirment = Convert.ToDecimal(dr["DailyRequirment"]);
                        model.Stockable = dr["Stockable"].ToString();
                        model.WipStockable = dr["WipStockable"].ToString();
                        model.Store = dr["Store"].ToString();
                        model.ProductLifeInus = Convert.ToDecimal(dr["ProductLifeInus"]);
                        model.ItemDesc = dr["ItemDesc"].ToString();
                        model.MaxWipStock = Convert.ToDecimal(dr["MaxWipStock"]);
                        model.NeedSo = dr["NeedSo"].ToString();
                        model.BomRequired = dr["BomRequired"].ToString();
                        model.JobWorkItem = dr["JobWorkItem"].ToString();
                        model.HSNNO = Convert.ToInt32(dr["HsnNo"]);
                        model.ItemServAssets = dr["ItemServAssets"].ToString();
                        model.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        model.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["CreatedOn"]);

                        model.CreatedByName = dr["CreatedByName"].ToString();
                        model.Active = dr["Active"].ToString();
                        model.UniversalPartCode = dr["UniversalPartCode"].ToString();
                        model.UniversalDescription = dr["UniversalDescription"].ToString();
                        model.ProdInWorkcenter = dr["ProdInWorkcenter"].ToString() == "" ? 0 : Convert.ToInt32(dr["ProdInWorkcenter"].ToString());
                        model.ProdInhouseJW = dr["ProdInhouseJW"].ToString();
                        model.BatchNO = dr["BatchNO"].ToString();
                        model.VoltageVlue = dr["VoltageValue"].ToString();
                        model.OldPartCode = dr["OldPartCode"].ToString();
                        model.SerialNo = dr["SerialNo"].ToString();
                        model.Package = dr["Package"].ToString();


                        if (!string.IsNullOrEmpty(dr["UpdatedByName"].ToString()))
                        {
                            model.UpdatedByName =dr["UpdatedByName"].ToString();

                            model.UpdatedBy = string.IsNullOrEmpty(dr["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["UpdatedBy"]);
                            model.UpdatedOn = string.IsNullOrEmpty(dr["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["UpdatedOn"]);
                        }
                        if (!string.IsNullOrEmpty(dr["ItemServAssets"].ToString()))
                        {
                            if(dr["ItemServAssets"].ToString() == "Service")
                            {
                                model.ItemServAssets = "Service";
                            }
                            else if(dr["ItemServAssets"].ToString() == "Assets")
                            {
                                model.ItemServAssets = "Asset";
                            }
                            else
                            {
                                model.ItemServAssets = "Item";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //dynamic Error = new ExpandoObject();
                //Error.Message = ex.Message;
                //Error.Source = ex.Source;
                //Error.Procedure = ((SqlException)ex).Procedure;
                //Error.Server = ((SqlException)ex).Server;
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                oDataTable.Dispose();
            }
            return model;
        }

        public async Task<ResponseResult> SaveData(ItemMasterModel model)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_ItemMasterData", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Item_Code", model.Item_Code);
                    oCmd.Parameters.AddWithValue("@PartCode", model.PartCode);
                    oCmd.Parameters.AddWithValue("@Item_Name", model.Item_Name);
                    oCmd.Parameters.AddWithValue("@ParentCode", model.ParentCode);
                    oCmd.Parameters.AddWithValue("@EntryDate", model.EntryDate);
                    oCmd.Parameters.AddWithValue("@LastUpdatedDate", model.LastUpdatedDate);
                    oCmd.Parameters.AddWithValue("@LeadTime", model.LeadTime);
                    oCmd.Parameters.AddWithValue("@CC", model.CC);
                    oCmd.Parameters.AddWithValue("@Unit", model.Unit);
                    oCmd.Parameters.AddWithValue("@SalePrice", model.SalePrice);
                    oCmd.Parameters.AddWithValue("@PurchasePrice", model.PurchasePrice);
                    oCmd.Parameters.AddWithValue("@CostPrice", model.CostPrice);
                    oCmd.Parameters.AddWithValue("@WastagePercent", model.WastagePercent);
                    oCmd.Parameters.AddWithValue("@WtSingleItem", model.WtSingleItem);
                    oCmd.Parameters.AddWithValue("@NoOfPcs", model.NoOfPcs);
                    oCmd.Parameters.AddWithValue("@QcReq", model.QcReq);
                    oCmd.Parameters.AddWithValue("@ItemType", model.ItemType);
                    oCmd.Parameters.AddWithValue("@UploadImage", model.ImageURL);
                    oCmd.Parameters.AddWithValue("@UploadItemImage", model.ItemImageURL);
                    oCmd.Parameters.AddWithValue("@UID", model.UID);
                    oCmd.Parameters.AddWithValue("@DrawingNo", model.DrawingNo);
                    oCmd.Parameters.AddWithValue("@MinimumLevel", model.MinimumLevel);
                    oCmd.Parameters.AddWithValue("@MaximumLevel", model.MaximumLevel);
                    oCmd.Parameters.AddWithValue("@ReorderLevel", model.ReorderLevel);
                    oCmd.Parameters.AddWithValue("@YearCode", model.YearCode);
                    oCmd.Parameters.AddWithValue("@AlternateUnit", model.AlternateUnit);
                    oCmd.Parameters.AddWithValue("@RackID", model.RackID);
                    oCmd.Parameters.AddWithValue("@BinNo", model.BinNo);
                    oCmd.Parameters.AddWithValue("@ItemSize", model.ItemSize);
                    oCmd.Parameters.AddWithValue("@Colour", model.Colour);
                    oCmd.Parameters.AddWithValue("@NeedPO", model.NeedPO);
                    oCmd.Parameters.AddWithValue("@StdPacking", model.StdPacking);
                    oCmd.Parameters.AddWithValue("@PackingType", model.PackingType);
                    oCmd.Parameters.AddWithValue("@ModelNo", model.ModelNo);
                    oCmd.Parameters.AddWithValue("@YearlyConsumedQty", model.YearlyConsumedQty);
                    oCmd.Parameters.AddWithValue("@DispItemName", model.DispItemName);
                    oCmd.Parameters.AddWithValue("@PurchaseAccountcode", model.PurchaseAccountcode);
                    oCmd.Parameters.AddWithValue("@SaleAccountcode", model.SaleAccountcode);
                    oCmd.Parameters.AddWithValue("@MinLevelDays", model.MinLevelDays);
                    oCmd.Parameters.AddWithValue("@MaxLevelDays", model.MaxLevelDays);
                    oCmd.Parameters.AddWithValue("@EmpName", model.EmpName);
                    oCmd.Parameters.AddWithValue("@DailyRequirment", model.DailyRequirment);
                    oCmd.Parameters.AddWithValue("@Stockable", model.Stockable);
                    oCmd.Parameters.AddWithValue("@WipStockable", model.WipStockable);
                    oCmd.Parameters.AddWithValue("@Store", model.Store);
                    oCmd.Parameters.AddWithValue("@ProductLifeInus", model.ProductLifeInus);
                    oCmd.Parameters.AddWithValue("@ItemDesc", model.ItemDesc);
                    oCmd.Parameters.AddWithValue("@MaxWipStock", model.MaxWipStock);
                    oCmd.Parameters.AddWithValue("@NeedSo", model.NeedSo);
                    oCmd.Parameters.AddWithValue("@BomRequired", model.BomRequired);
                    oCmd.Parameters.AddWithValue("@JobWorkItem", model.JobWorkItem);
                    oCmd.Parameters.AddWithValue("@HsnNo", model.HSNNO);
                    oCmd.Parameters.AddWithValue("@Active", model.Active);
                    oCmd.Parameters.AddWithValue("@ItemServAssets", model.ItemServAssets);
                    oCmd.Parameters.AddWithValue("@EntryByMachineName", model.EntryByMachineName);
                    oCmd.Parameters.AddWithValue("@UniversalPartCode", model.UniversalPartCode);
                    oCmd.Parameters.AddWithValue("@UniversalDescription", model.UniversalDescription);
                    oCmd.Parameters.AddWithValue("@ProdInWorkcenter", model.ProdInWorkcenter);
                    oCmd.Parameters.AddWithValue("@ProdInhouseJW", model.ProdInhouseJW);
                    oCmd.Parameters.AddWithValue("@BatchNO", model.BatchNO);
                    oCmd.Parameters.AddWithValue("@VoltageValue", model.VoltageVlue);
                    oCmd.Parameters.AddWithValue("@SerialNo", model.SerialNo);
                    oCmd.Parameters.AddWithValue("@OldPartCode", model.OldPartCode);
                    oCmd.Parameters.AddWithValue("@package", model.Package);

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
                                StatusText = "Success",
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
        public async Task<ResponseResult> SaveMultipleItemData(DataTable ItemDetailGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "INSERTMULTIPLE"));   
                SqlParams.Add(new SqlParameter("@DTSSGrid", ItemDetailGrid));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        } 
        public async Task<ResponseResult>UpdateMultipleItemData(DataTable ItemDetailGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "UPDATEMULTIPLE"));   
                SqlParams.Add(new SqlParameter("@DTSIMGrid", ItemDetailGrid));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("SP_ItemMasterData", SqlParams);
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