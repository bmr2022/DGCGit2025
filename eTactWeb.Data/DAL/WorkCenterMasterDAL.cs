using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class WorkCenterMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private dynamic? _ResponseResult;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public WorkCenterMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<WorkCenterMasterModel> ViewByID(int id)
        {
            WorkCenterMasterModel?  _WorkCenterMasterModel = new WorkCenterMasterModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPWorkCenterMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@wcid", id);
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
                        _WorkCenterMasterModel.WCID = Convert.ToInt32(dr["wcid"].ToString());
                        _WorkCenterMasterModel.WorkCenterCode = dr["WorkCenterCode"].ToString();
                        _WorkCenterMasterModel.WorkCenterDescription = dr["WorkCenterDescription"].ToString();
                        _WorkCenterMasterModel.WorkCenterType = dr["WorkcenterType"].ToString();

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

            return _WorkCenterMasterModel;
        }

        public async Task<ResponseResult> SaveData(WorkCenterMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Insert"));
                    
                }


               SqlParams.Add(new SqlParameter("@wcid", model.WCID));
                SqlParams.Add(new SqlParameter("@WorkCenterCode", model.WorkCenterCode));
                SqlParams.Add(new SqlParameter("@WCName", model.WorkCenterDescription));
                SqlParams.Add(new SqlParameter("@WorkcenterType", model.WorkCenterType));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPWorkCenterMaster", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
           
            return _ResponseResult;
        }

        public async Task<ResponseResult> DeleteMachine(string WorkCenterDescription)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@WCName", WorkCenterDescription));
                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                //SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPWorkCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }



        public async Task<WorkCenterMasterModel> GetDashboardData(WorkCenterMasterModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPWorkCenterMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
                    oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
                    //oCmd.Parameters.AddWithValue("@Entryid", model.WCID);
                    oCmd.Parameters.AddWithValue("@WorkCenterCode", model.WorkCenterCode);
                    oCmd.Parameters.AddWithValue("@WCName", model.WorkCenterDescription);
                    oCmd.Parameters.AddWithValue("@WorkCenterType", model.WorkCenterType);

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
                    model.WorkCenterMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new WorkCenterMasterModel
                                               {
                                                   WCID = Convert.ToInt32(dr["WCID"]),
                                                   WorkCenterCode = dr["WorkCenterCode"].ToString(),
                                                   WorkCenterDescription = dr["WorkCenterDescription"].ToString(),
                                                   WorkCenterType = dr["WorkCenterType"]?.ToString() ?? string.Empty,
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


    }
}
