using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
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
    public class MachineMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private dynamic? _ResponseResult;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public MachineMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> FillMachineGroup()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMachineGroup"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        } 
        public async Task<ResponseResult> FillMachineWorkCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMachineWorkCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveMachineMaster(MachineMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@EntryId", model.MachineId > 0 ? model.MachineId : (object)DBNull.Value));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Insert"));
                }
                    SqlParams.Add(new SqlParameter("@EntryId", model.EntryId > 0 ? model.MachineId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@MachGroupId", model.MachGroupId > 0 ? model.MachGroupId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@MachineCode", string.IsNullOrEmpty(model.MachineCode) ? (object)DBNull.Value : model.MachineCode));
                    SqlParams.Add(new SqlParameter("@MachineName", string.IsNullOrEmpty(model.MachineName) ? (object)DBNull.Value : model.MachineName));
                    SqlParams.Add(new SqlParameter("@LabourCost", model.LabourCost > 0 ? model.LabourCost : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@NeedHelper", string.IsNullOrEmpty(model.NeedHelper) ? (object)DBNull.Value : model.NeedHelper));
                    SqlParams.Add(new SqlParameter("@TotalHelper", model.TotalHelper > 0 ? model.TotalHelper : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@HelperCost", model.HelperCost > 0 ? model.HelperCost : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@ElectricityCost", model.ElectricityCost > 0 ? model.ElectricityCost : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@OtherCost", model.OtherCost > 0 ? model.OtherCost : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@TotalCost", model.TotalCost > 0 ? model.TotalCost : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@EntryDate", model.EntryDate ?? (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Location", string.IsNullOrEmpty(model.Location) ? (object)DBNull.Value : model.Location));
                    SqlParams.Add(new SqlParameter("@TechSpecification", string.IsNullOrEmpty(model.TechSpecification) ? (object)DBNull.Value : model.TechSpecification));
                    SqlParams.Add(new SqlParameter("@LastCalibraDate", model.LastCalibraDate ?? (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CalibraDur", model.CalibraDur > 0 ? model.CalibraDur : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? (object)DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@UId", string.IsNullOrEmpty(model.UId) ? (object)DBNull.Value : model.UId));
               


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<MachineMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new MachineMasterModel();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMachineMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.MachineMasterGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new MachineMasterModel
                                               {
                                                   MachineId = dr["MachineId"] != DBNull.Value ? Convert.ToInt32(dr["MachineId"]) : 0,
                                                   MachGroupId = dr["MachGroupId"] != DBNull.Value ? Convert.ToInt32(dr["MachGroupId"]) : 0,
                                                   MachineGroup = dr["MachineGroup"] != DBNull.Value ? dr["MachineGroup"].ToString() : string.Empty,
                                                   MachineCode = dr["MachineCode"] != DBNull.Value ? dr["MachineCode"].ToString() : string.Empty,
                                                   MachineName = dr["MachineName"] != DBNull.Value ? dr["MachineName"].ToString() : string.Empty,
                                                   LabourCost = dr["LabourCost"] != DBNull.Value ? Convert.ToDouble(dr["LabourCost"]) : 0.0,
                                                   NeedHelper = dr["NeedHelper"] != DBNull.Value ? dr["NeedHelper"].ToString() : string.Empty,
                                                   TotalHelper = dr["TotalHelper"] != DBNull.Value ? Convert.ToInt32(dr["TotalHelper"]) : 0,
                                                   HelperCost = dr["HelperCost"] != DBNull.Value ? Convert.ToDouble(dr["HelperCost"]) : 0.0,
                                                   ElectricityCost = dr["ElectricityCost"] != DBNull.Value ? Convert.ToDouble(dr["ElectricityCost"]) : 0.0,
                                                   OtherCost = dr["OtherCost"] != DBNull.Value ? Convert.ToDouble(dr["OtherCost"]) : 0.0,
                                                   TotalCost = dr["TotalCost"] != DBNull.Value ? Convert.ToDouble(dr["TotalCost"]) : 0.0,
                                                   EntryDate = dr["EntryDate"] != DBNull.Value ? dr["EntryDate"].ToString() : string.Empty,
                                                   Make = dr["Make"] != DBNull.Value ? dr["Make"].ToString() : string.Empty,
                                                   Location = dr["Location"] != DBNull.Value ? dr["Location"].ToString() : string.Empty,
                                                   TechSpecification = dr["Techspecification"] != DBNull.Value ? dr["Techspecification"].ToString() : string.Empty,
                                                   LastCalibraDate = dr["LastCalibraDate"] != DBNull.Value ? dr["LastCalibraDate"].ToString() : string.Empty,
                                                   CalibraDur = dr["CalibraDur"] != DBNull.Value ? Convert.ToDouble(dr["CalibraDur"]) : 0.0,
                                                   CC = dr["CC"] != DBNull.Value ? dr["CC"].ToString() : string.Empty,
                                                   UId = dr["UId"] != DBNull.Value ? dr["UId"].ToString() : string.Empty

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
        public async Task<MachineMasterModel> GetViewByID()
        {
            var model = new MachineMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMachineMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    //PrepareView(_ResponseResult.Result, ref model);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                SqlParams.Add(new SqlParameter("@Entryid", ID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                // Optional: log the error using a logger
                Console.WriteLine($"Error: {Error.Message}");
                Console.WriteLine($"Source: {Error.Source}");
            }

            return _ResponseResult;
        }
        public async Task<MachineMasterModel> ViewByID(int id)
        {
            MachineMasterModel?  _MachineMasterModel = new MachineMasterModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMachineMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Entryid", id);
                    oCmd.Parameters.AddWithValue("@Flag", "ViewByID");
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
                        _MachineMasterModel.MachineId = Convert.ToInt32(dr["MachineId"].ToString());
                        _MachineMasterModel.MachGroupId = Convert.ToInt32(dr["MachGroupId"].ToString());
                        _MachineMasterModel.MachineCode = dr["MachineCode"].ToString();
                        _MachineMasterModel.MachineName = dr["MachineName"].ToString();
                        
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

            return _MachineMasterModel;
        }

        public async Task<ResponseResult> SaveData(MachineMasterModel model)
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


                SqlParams.Add(new SqlParameter("@Entryid", model.MachineId));
                SqlParams.Add(new SqlParameter("@MachGroupId", model.MachGroupId));
                SqlParams.Add(new SqlParameter("@MachineCode", model.MachineCode));
                SqlParams.Add(new SqlParameter("@MachineName", model.MachineName));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);


                //using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                //{
                //    SqlCommand oCmd = new SqlCommand("SPMachineMaster", myConnection)
                //    {
                //        CommandType = CommandType.StoredProcedure
                //    };

                //    oCmd.Parameters.AddWithValue("@Flag", "Insert");
                //    oCmd.Parameters.AddWithValue("@EntryId", model.EntryId);
                //    oCmd.Parameters.AddWithValue("@MachGroupId", model.MachGroupId);
                //    oCmd.Parameters.AddWithValue("@MachineCode", model.MachineCode);
                //    oCmd.Parameters.AddWithValue("@MachineName", model.MachineName);
                   

                //    myConnection.Open();
                //    Reader = await oCmd.ExecuteReaderAsync();
                //    if (Reader != null)
                //    {
                //        while (Reader.Read())
                //        {
                //            _ResponseResult = new ResponseResult()
                //            {
                //                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                //                StatusText = "Success",
                //                Result = Reader["Result"].ToString()
                //            };
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            //finally
            //{
            //    if (Reader != null)
            //    {
            //        Reader.Close();
            //        Reader.Dispose();
            //    }
            //}

            return _ResponseResult;
        }

        public async Task<ResponseResult> DeleteMachine(int id)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Entryid", id));
                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                //SqlParams.Add(new SqlParameter("@YearCode", YearCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }



        //public async Task<MachineMasterModel> GetDashboardData(MachineMasterModel model)
        //{
        //    DataSet? oDataSet = new DataSet();

        //    try
        //    {
        //        using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
        //        {
        //            SqlCommand oCmd = new SqlCommand("SPMachineMaster", myConnection)
        //            {
        //                CommandType = CommandType.StoredProcedure
        //            };
        //            //Group_Code,Group_name,Under_GroupCode,Entry_date,GroupCatCode,UnderCategoryId,seqNo
        //            oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
        //            //oCmd.Parameters.AddWithValue("@MachineId", model.MachineId);
        //            oCmd.Parameters.AddWithValue("@MachGroupId", model.MachGroupId);
        //            oCmd.Parameters.AddWithValue("@MachineCode", model.MachineCode);
        //            oCmd.Parameters.AddWithValue("@MachineName", model.MachineName);

        //            //oCmd.Parameters.AddWithValue("@ItemCategory", model.ItemCategory);
        //            // oCmd.Parameters.AddWithValue("@Main_Category_Type", model.Main_Category_Type);
        //            await myConnection.OpenAsync();
        //            using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
        //            {
        //                oDataAdapter.Fill(oDataSet);
        //            }
        //        }
        //        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
        //        {
        //            model.MachineMasterGrid = (from DataRow dr in oDataSet.Tables[0].Rows
        //                                       select new MachineMasterModel
        //                                       {
        //                                           MachineId = Convert.ToInt32(dr["MachineId"]),
        //                                           MachGroupId = Convert.ToInt32(dr["MachGroupId"]),
        //                                           MachineCode = dr["MachineCode"].ToString(),
        //                                           MachineName = dr["MachineName"]?.ToString() ?? string.Empty,
        //                                       }).ToList();

        //        }
        //        //var ilst = model.AccountMasterList.Select(m => new TextValue
        //        //{
        //        //    Text = m.ParentAccountName,
        //        //    Value = m.ParentAccountCode.ToString()
        //        //});
        //        //if (model.Mode != "Search")
        //        //{
        //        //    List<TextValue>? _list = new List<TextValue>();
        //        //    foreach (MachineMasterModel? item in model.MachineMasterList)
        //        //    {
        //        //        TextValue? _lst = new TextValue
        //        //        {
        //        //            Text = item.EntryId,
        //        //            Value = item.Group_Code.ToString()
        //        //        };
        //        //        _list.Add(_lst);
        //        //    }

        //        //    model.MachineMasterList = (IList<MachineMasterModel>)_list;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }
        //    finally
        //    {
        //        oDataSet.Dispose();
        //    }
        //    return model;
        //}


    }
}
