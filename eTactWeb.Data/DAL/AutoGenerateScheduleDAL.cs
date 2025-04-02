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
    public class AutoGenerateScheduleDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public AutoGenerateScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> GetMrpNo(string SchEffectivedate, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMRPNO"));
                SqlParams.Add(new SqlParameter("@SchEffectivedate", SchEffectivedate));
                SqlParams.Add(new SqlParameter("@CurrentYear", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpGenerateAutoPurchScheduleAgainstMRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> GetMrpId(string SchEffectivedate, int YearCode, int MrpNo, int MrpYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMRPentryid"));
                SqlParams.Add(new SqlParameter("@SchEffectivedate", SchEffectivedate));
                SqlParams.Add(new SqlParameter("@CurrentYear", YearCode));
                SqlParams.Add(new SqlParameter("@MRPNO", MrpNo));
                SqlParams.Add(new SqlParameter("@MRPYearcode", MrpYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpGenerateAutoPurchScheduleAgainstMRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> GetMrpYearCode(string SchEffectivedate, int YearCode, int MrpNo)
         {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMRPyearcode"));
                SqlParams.Add(new SqlParameter("@SchEffectivedate", SchEffectivedate));
                SqlParams.Add(new SqlParameter("@CurrentYear", YearCode));
                SqlParams.Add(new SqlParameter("@MRPNO", MrpNo));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpGenerateAutoPurchScheduleAgainstMRP", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
         }
        //public async Task<ResponseResult> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        //{
        //    var _ResponseResult = new ResponseResult();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();

        //        SqlParams.Add(new SqlParameter("@flag", ReportType));
        //        SqlParams.Add(new SqlParameter("@MRPNO", MrpNo));
        //        SqlParams.Add(new SqlParameter("@MRPYearcode", MrpYearCode));
        //        SqlParams.Add(new SqlParameter("@MRPEntryid", MrpEntryId));
        //        SqlParams.Add(new SqlParameter("@SchEffectivedate", SchEffectivedate));
        //        SqlParams.Add(new SqlParameter("@CreatedBy", CreatedBy));
        //        SqlParams.Add(new SqlParameter("@CC", CC));
        //        SqlParams.Add(new SqlParameter("@UID", UID));
        //        SqlParams.Add(new SqlParameter("@EntryByMachineName", MachineName));
        //        _ResponseResult = await _IDataLogic.ExecuteDataTable("SpGenerateAutoPurchScheduleAgainstMRP", SqlParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return _ResponseResult;
        // }

        public async Task<AutoGenerateScheduleModel> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        {
            var resultList = new AutoGenerateScheduleModel();
            DataSet oDataSet = new DataSet();
            //oDataSet.Tables[0].TableName = "PurchaseScheduleMain";
            //oDataSet.Tables[1].TableName = "PurchaseScheduleDetail";
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SpGenerateAutoPurchScheduleAgainstMRP", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@flag", ReportType);
                    command.Parameters.Add(new SqlParameter("@MRPNO", MrpNo));
                    command.Parameters.Add(new SqlParameter("@MRPYearcode", MrpYearCode));
                    command.Parameters.AddWithValue("@MRPEntryid", MrpEntryId);
                    command.Parameters.AddWithValue("@SchEffectivedate", SchEffectivedate);
                    command.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                    command.Parameters.AddWithValue("@CC", CC);
                    command.Parameters.AddWithValue("@UID", UID);
                    command.Parameters.AddWithValue("@EntryByMachineName", MachineName);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (ReportType == "List Of Item For Schedule")
                    {
                        resultList.AutoGenerateScheduleGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new AutoGenerateScheduleModel
                                                               {
                                                                   PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                                   ItemName = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),
                                                                   VendorName = row["VendorName"] == DBNull.Value ? string.Empty : row["VendorName"].ToString(),
                                                                   VendSchOty = row["VendSchQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["VendSchQty"]),
                                                                   Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                                   RmItemCode = row["RMItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["RMItemCode"]),
                                                                   DeliveryDate = row["DeliveryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["DeliveryDate"]).ToString("dd/MM/yyyy"),
                                                                   BusinessPer = row["BusinessPercent"] == DBNull.Value ? 0 : Convert.ToInt32(row["BusinessPercent"]),
                                                                   VendCustAccCode = row["VendCustAccountCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["VendCustAccountCode"]),
                                                                   NoOfVend = row["NoofVendor"] == DBNull.Value ? 0 : Convert.ToInt32(row["NoofVendor"]),
                                                                   PONO = row["PONO"] == DBNull.Value ? string.Empty : row["PONO"].ToString(),
                                                                   PoDate = row["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy"),
                                                                   PoRate = row["PORate"] == DBNull.Value ? 0.0f : Convert.ToSingle(row["PORate"]),

                                                               }).ToList();

                    }
                    else if (ReportType == "Generate Purchase Schedule")
                    {
                        resultList.AutoGenerateScheduleGrid = (from DataRow row in oDataSet.Tables[1].Rows
                                                               select new AutoGenerateScheduleModel
                                                               {
                                                                   VendorName = row["VendorName"] == DBNull.Value ? string.Empty : row["VendorName"].ToString(),
                                                                   PONO = row["PONO"] == DBNull.Value ? string.Empty : row["PONO"].ToString(),
                                                                   PoDate = row["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy"),
                                                                   PoYearCode = row["POYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["POYearCode"]),
                                                                   SchNo = row["ScheduleNo"] == DBNull.Value ? string.Empty : row["ScheduleNo"].ToString(),
                                                                   ScheduleDate = row["ScheduleDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleDate"]).ToString("dd/MM/yyyy"),
                                                                   SchAmendNo = row["SchAmendNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SchAmendNo"]),
                                                                   ScheduleEffectiveFromDate = row["ScheduleEffectiveFromDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleEffectiveFromDate"]).ToString("dd/MM/yyyy"),
                                                                   ScheduleEffectiveTillDate = row["ScheduleEffectiveTillDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleEffectiveTillDate"]).ToString("dd/MM/yyyy"),

                                                                   MRPNO = row["MRPNO"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPNO"]),
                                                                   MRPNoYearCode = row["MRPNoYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPNoYearCode"]),
                                                                   MRPEntry_Id = row["MRPEntry_Id"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPEntry_Id"]),

                                                                   //VendorName = oDataSet.Tables[0].Rows[0]["VendorName"] == DBNull.Value ? string.Empty : oDataSet.Tables[0].Rows[0]["VendorName"].ToString(),
                                                                   //PONO = oDataSet.Tables[0].Rows[0]["PONO"] == DBNull.Value ? string.Empty : oDataSet.Tables[0].Rows[0]["PONO"].ToString(),
                                                                   //PoDate = oDataSet.Tables[0].Rows[0]["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["PODate"]).ToString("dd/MM/yyyy"),
                                                                   //PoYearCode = oDataSet.Tables[0].Rows[0]["POYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["POYearCode"]),
                                                                   //SchNo = oDataSet.Tables[0].Rows[0]["ScheduleNo"] == DBNull.Value ? string.Empty : oDataSet.Tables[0].Rows[0]["ScheduleNo"].ToString(),
                                                                   //ScheduleDate = oDataSet.Tables[0].Rows[0]["ScheduleDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ScheduleDate"]).ToString("dd/MM/yyyy"),
                                                                   //SchAmendNo = oDataSet.Tables[0].Rows[0]["SchAmendNo"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SchAmendNo"]),
                                                                   //ScheduleEffectiveFromDate = oDataSet.Tables[0].Rows[0]["ScheduleEffectiveFromDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ScheduleEffectiveFromDate"]).ToString("dd/MM/yyyy"),
                                                                   //ScheduleEffectiveTillDate = oDataSet.Tables[0].Rows[0]["ScheduleEffectiveTillDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ScheduleEffectiveTillDate"]).ToString("dd/MM/yyyy"),
                                                                   //MRPNO = oDataSet.Tables[0].Rows[0]["MRPNO"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRPNO"]),
                                                                   //MRPNoYearCode = oDataSet.Tables[0].Rows[0]["MRPNoYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRPNoYearCode"]),
                                                                   //MRPEntry_Id = oDataSet.Tables[0].Rows[0]["MRPEntry_Id"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRPEntry_Id"]),

                                                               }).ToList();
                    }
                }
                if (oDataSet.Tables.Count > 1 && oDataSet.Tables[1].Rows.Count > 0)
                {


                    if (ReportType == "Generate Purchase Schedule")
                    {
                        resultList.AutoGenerateScheduleGrid2 = (from DataRow row in oDataSet.Tables[1].Rows
                                                                select new AutoGenerateScheduleModel
                                                                {
                                                                    VendorName = row["VendorName"] == DBNull.Value ? string.Empty : row["VendorName"].ToString(),
                                                                    PONO = row["PONO"] == DBNull.Value ? string.Empty : row["PONO"].ToString(),
                                                                    PoDate = row["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy"),
                                                                    PoYearCode = row["POYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["POYearCode"]),
                                                                    SchNo = row["ScheduleNo"] == DBNull.Value ? string.Empty : row["ScheduleNo"].ToString(),
                                                                    ScheduleDate = row["ScheduleDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleDate"]).ToString("dd/MM/yyyy"),
                                                                    SchAmendNo = row["SchAmendNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SchAmendNo"]),
                                                                    ScheduleEffectiveFromDate = row["ScheduleEffectiveFromDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleEffectiveFromDate"]).ToString("dd/MM/yyyy"),
                                                                    ScheduleEffectiveTillDate = row["ScheduleEffectiveTillDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ScheduleEffectiveTillDate"]).ToString("dd/MM/yyyy"),
                                                                    PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                                    ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                                    SchQty = row["SchQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["SchQty"]),
                                                                    Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                                    DeliveryDate = row["DeliveryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["DeliveryDate"]).ToString("dd/MM/yyyy"),
                                                                    DeliveryWeek = row["DeliveryWeek"] == DBNull.Value ? string.Empty : row["DeliveryWeek"].ToString(),
                                                                    AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                                    PendQty = row["PendQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["PendQty"]),
                                                                    MRPNO = row["MRPNO"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPNO"]),
                                                                    MRPNoYearCode = row["MRPNoYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPNoYearCode"]),
                                                                    MRPEntry_Id = row["MRPEntry_Id"] == DBNull.Value ? 0 : Convert.ToInt32(row["MRPEntry_Id"]),

                                                                    //VendorName = oDataSet.Tables[1].Rows[1]["VendorName"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["VendorName"].ToString(),
                                                                    //PONO = oDataSet.Tables[1].Rows[1]["PONO"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["PONO"].ToString(),
                                                                    //PoDate = oDataSet.Tables[1].Rows[1]["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[1].Rows[1]["PODate"]).ToString("dd/MM/yyyy"),
                                                                    //PoYearCode = oDataSet.Tables[1].Rows[1]["POYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["POYearCode"]),
                                                                    //SchNo = oDataSet.Tables[1].Rows[1]["ScheduleNo"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["ScheduleNo"].ToString(),
                                                                    //ScheduleDate = oDataSet.Tables[1].Rows[1]["ScheduleDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[1].Rows[1]["ScheduleDate"]).ToString("dd/MM/yyyy"),
                                                                    //SchAmendNo = oDataSet.Tables[1].Rows[1]["SchAmendNo"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["SchAmendNo"]),
                                                                    //ScheduleEffectiveFromDate = oDataSet.Tables[1].Rows[1]["ScheduleEffectiveFromDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[1].Rows[1]["ScheduleEffectiveFromDate"]).ToString("dd/MM/yyyy"),
                                                                    //ScheduleEffectiveTillDate = oDataSet.Tables[1].Rows[1]["ScheduleEffectiveTillDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[1].Rows[1]["ScheduleEffectiveTillDate"]).ToString("dd/MM/yyyy"),
                                                                    //PartCode = oDataSet.Tables[1].Rows[1]["PartCode"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["PartCode"].ToString(),
                                                                    //ItemName = oDataSet.Tables[1].Rows[1]["ItemName"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["ItemName"].ToString(),
                                                                    //SchQty = oDataSet.Tables[1].Rows[1]["SchQty"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["SchQty"]),
                                                                    //Unit = oDataSet.Tables[1].Rows[1]["Unit"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["Unit"].ToString(),
                                                                    //DeliveryDate = oDataSet.Tables[1].Rows[1]["DeliveryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(oDataSet.Tables[1].Rows[1]["DeliveryDate"]).ToString("dd/MM/yyyy"),
                                                                    //DeliveryWeek = oDataSet.Tables[1].Rows[1]["DeliveryWeek"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["DeliveryWeek"].ToString(),
                                                                    //AltUnit = oDataSet.Tables[1].Rows[1]["AltUnit"] == DBNull.Value ? string.Empty : oDataSet.Tables[1].Rows[1]["AltUnit"].ToString(),
                                                                    //PendQty = oDataSet.Tables[1].Rows[1]["PendQty"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["PendQty"]),
                                                                    //MRPNO = oDataSet.Tables[1].Rows[1]["MRPNO"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["MRPNO"]),
                                                                    //MRPNoYearCode = oDataSet.Tables[1].Rows[1]["MRPNoYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["MRPNoYearCode"]),
                                                                    //MRPEntry_Id = oDataSet.Tables[1].Rows[1]["MRPEntry_Id"] == DBNull.Value ? 0 : Convert.ToInt32(oDataSet.Tables[1].Rows[1]["MRPEntry_Id"]),
                                                                }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exception (log it or rethrow)
                throw new Exception("Error fetching BOM tree data.", ex);
            }

            return resultList;
        }
    }
}
