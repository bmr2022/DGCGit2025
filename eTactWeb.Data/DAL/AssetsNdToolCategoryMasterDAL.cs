using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
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
    public class AssetsNdToolCategoryMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public AssetsNdToolCategoryMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }


        public async Task<AssetsNdToolCategoryMasterModel> GetNewEntryId()
        {
            
            var model = new AssetsNdToolCategoryMasterModel();
            var response = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "NewEntryId"),
            new SqlParameter("@ActualEntryDate", DateTime.Now) // required for SP
        };

                 response = await _IDataLogic.ExecuteDataTable("[ACCSPAssetsNdToolcategoryMaster]", SqlParams);
                if (response.Result is DataTable dt && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    model.AssetsCateogryId = row["EntryId"] != DBNull.Value ? Convert.ToInt32(row["EntryId"]) : 0;
                    model.AssetsCategoryCode = row["ToolCode"] != DBNull.Value ? row["ToolCode"].ToString() : string.Empty;
                    model.ActualEntryDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.StatusText = "Error";
                response.Result = new { ex.Message, ex.StackTrace };
            }

            return model;
        }


        // Insert / Update
        public async Task<ResponseResult> SaveAsync(AssetsNdToolCategoryMasterModel model)
        {
            var response = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@Flag", model.Mode == "U" ? "Update" : "INSERT"), // match SP
                    new SqlParameter("@AssetsCateogryId", model.AssetsCateogryId > 0 ? model.AssetsCateogryId : (object)DBNull.Value),
                    new SqlParameter("@AsstesToolInstrument", string.IsNullOrEmpty(model.AsstesToolInstrument) ? DBNull.Value : model.AsstesToolInstrument),
                    new SqlParameter("@AssetsCategoryName", string.IsNullOrEmpty(model.AssetsCategoryName) ? DBNull.Value : model.AssetsCategoryName),
                    new SqlParameter("@AssetsCategoryCode", string.IsNullOrEmpty(model.AssetsCategoryCode) ? DBNull.Value : model.AssetsCategoryCode),
                    new SqlParameter("@AssetsMainCateogryId", model.AssetsMainCateogryId ?? (object)DBNull.Value),
                    new SqlParameter("@ToolMaincategoryId", model.ToolMaincategoryId ?? (object)DBNull.Value),
                    new SqlParameter("@EntryByMachineName", string.IsNullOrEmpty(model.EntryByMachineName) ? DBNull.Value : model.EntryByMachineName),
                    new SqlParameter("@ActualEntryBy", model.ActualEntryBy > 0 ? model.ActualEntryBy : (object)DBNull.Value),
                    new SqlParameter("@ActualEntryDate", model.ActualEntryDate != DateTime.MinValue ? model.ActualEntryDate : DateTime.Now),
                    new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy > 0 ? model.LastUpdatedBy : (object)DBNull.Value),
                    new SqlParameter("@LastUpdationDate", DateTime.Now)
                };

                response = await _IDataLogic.ExecuteDataTable("[ACCSPAssetsNdToolcategoryMaster]", sqlParams);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.StatusText = "Error";
                response.Result = new { ex.Message, ex.StackTrace };
            }
            return response;
        }

        // Delete
        public async Task<ResponseResult> DeleteAsync(long id, string categoryName)
        {
            var sqlParams = new List<dynamic>
    {
        new SqlParameter("@Flag", "Delete"),
        new SqlParameter("@AssetsCateogryId", id),
        new SqlParameter("@AssetsCategoryName", categoryName ?? string.Empty),
        new SqlParameter("@ActualEntryDate", DateTime.Now),
        new SqlParameter("@ActualEntryBy", 1), // replace with logged-in user id
        new SqlParameter("@EntryByMachineName", Environment.MachineName),
        new SqlParameter("@AssetsCategoryCode", string.Empty)
    };

            // Call SP → already returns ResponseResult
            var result = await _IDataLogic.ExecuteDataTable("[ACCSPAssetsNdToolcategoryMaster]", sqlParams);

            // just return it back, no need for Columns/Rows
            return result;
        }


        // Dashboard
        public async Task<List<AssetsNdToolCategoryMasterModel>> GetDashboardAsync()
        {
            var sqlParams = new List<dynamic>
    {
        new SqlParameter("@Flag", "DASHBOARD")
    };

            var response = await _IDataLogic.ExecuteDataTable("[ACCSPAssetsNdToolcategoryMaster]", sqlParams);

            var result = new List<AssetsNdToolCategoryMasterModel>();
            if (response.Result is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new AssetsNdToolCategoryMasterModel
                    {
                        AssetsCateogryId = row["AssetsCateogryId"] != DBNull.Value ? Convert.ToInt64(row["AssetsCateogryId"]) : 0,
                        AsstesToolInstrument = row["AsstesToolInstrument"] != DBNull.Value ? row["AsstesToolInstrument"].ToString() : string.Empty,
                        AssetsCategoryName = row["AssetsCategoryName"] != DBNull.Value ? row["AssetsCategoryName"].ToString() : string.Empty,
                        AssetsCategoryCode = row["AssetsCategoryCode"] != DBNull.Value ? row["AssetsCategoryCode"].ToString() : string.Empty,
                        Otherdetail = row["Otherdetail"] != DBNull.Value ? row["Otherdetail"].ToString() : string.Empty,
                        AssetsMainCateogryId = row["AssetsMainCateogryId"] != DBNull.Value ? Convert.ToInt64(row["AssetsMainCateogryId"]) : (long?)null,
                        ToolMaincategoryId = row["ToolMaincategoryId"] != DBNull.Value ? Convert.ToInt64(row["ToolMaincategoryId"]) : (long?)null,
                        EntryByMachineName = row["EntryByMachineName"] != DBNull.Value ? row["EntryByMachineName"].ToString() : string.Empty,
                        ActualEntryBy = row["ActualEntryBy"] != DBNull.Value? Convert.ToInt64(row["ActualEntryBy"]): 0,
                        ActualEntryEmp = row["ActualEntryEmp"] != DBNull.Value ? row["ActualEntryEmp"].ToString() : string.Empty,
                        ActualEntryDate = row["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["ActualEntryDate"]) : (DateTime?)null,
                        LastUpdationDate = row["LastUpdationDate"] != DBNull.Value ? Convert.ToDateTime(row["LastUpdationDate"]) : (DateTime?)null,
                        LastUpdatedBy = row["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt64(row["LastUpdatedBy"]) : (long?)null,
                        UpdatedByEmp = row["UpdatedByEmp"] != DBNull.Value ? row["UpdatedByEmp"].ToString() : string.Empty,
                        AssetsName = row["AssetsName"] != DBNull.Value ? row["AssetsName"].ToString() : string.Empty,
                        AssetsEntryId = row["AssetsEntryId"] != DBNull.Value ? Convert.ToInt64(row["AssetsEntryId"]) : (long?)null
                    });
                }
            }
            return result;
        }
        // View by Id
        public async Task<AssetsNdToolCategoryMasterModel?> ViewByIdAsync(long id, string categoryName)
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@Flag", "ViewByID"),
                new SqlParameter("@AssetsCateogryId", id),
                new SqlParameter("@AssetsCategoryName", categoryName) // required as per SP
            };

            var response = await _IDataLogic.ExecuteDataTable("[ACCSPAssetsNdToolcategoryMaster]", sqlParams);

            if (response.Result is DataTable dt && dt.Rows.Count > 0)
            {
                var row = dt.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt64(r["AssetsCateogryId"]) == id);
                if (row != null)
                {
                    return new AssetsNdToolCategoryMasterModel
                    {
                        AssetsCateogryId = row["AssetsCateogryId"] != DBNull.Value ? Convert.ToInt64(row["AssetsCateogryId"]) : 0,
                        AsstesToolInstrument = row["AsstesToolInstrument"]?.ToString(),
                        AssetsCategoryName = row["AssetsCategoryName"]?.ToString(),
                        AssetsCategoryCode = row["AssetsCategoryCode"]?.ToString(),
                        Otherdetail = row["Otherdetail"]?.ToString(),
                        AssetsMainCateogryId = row["AssetsMainCateogryId"] != DBNull.Value ? Convert.ToInt32(row["AssetsMainCateogryId"]) : 0,
                        ToolMaincategoryId = row["ToolMaincategoryId"] != DBNull.Value ? Convert.ToInt32(row["ToolMaincategoryId"]) : 0,
                        EntryByMachineName = row["EntryByMachineName"]?.ToString(),
                        ActualEntryBy = row["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(row["ActualEntryBy"]) : 0,
                        ActualEntryEmp = row["ActualEntryEmp"]?.ToString(),
                        ActualEntryDate = row["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["ActualEntryDate"]) : (DateTime?)null,
                        LastUpdationDate = row["LastUpdationdate"] != DBNull.Value ? Convert.ToDateTime(row["LastUpdationdate"]) : (DateTime?)null,
                        LastUpdatedBy = row["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(row["LastUpdatedBy"]) : (int?)null,
                        UpdatedByEmp = row["UpdatedByEmp"]?.ToString()
                    };

                }
            }

            return null;
        }
    }
}


