using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;


namespace eTactWeb.Data.DAL
{
    public class IndentRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public IndentRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {


                //DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //oCmd.Parameters.AddWithValue("@Flag", ReportType);
                //oCmd.Parameters.AddWithValue("@Yearcode", YearCode);
                //oCmd.Parameters.AddWithValue("@fromDate", fromDt.ToString("yyyy/MM/dd"));
                //oCmd.Parameters.AddWithValue("@todate", toDt.ToString("yyyy/MM/dd"));

                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
                //SqlParams.Add(new SqlParameter("@Fromdate", FromDate));
                //SqlParams.Add(new SqlParameter("@todate", ToDate));
                SqlParams.Add(new SqlParameter("@Fromdate", fromDt));
                SqlParams.Add(new SqlParameter("@todate", toDt));
                
                
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportIndet", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<ResponseResult> GetPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
                SqlParams.Add(new SqlParameter("@Fromdate", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@todate",ParseFormattedDate(ToDate)));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportIndet", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<ResponseResult> GetIndentNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillIndentNo"));
                SqlParams.Add(new SqlParameter("@Fromdate",ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@todate", ParseFormattedDate(ToDate)));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportIndet", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<IndentRegisterModel> GetDetailsData(string FromDate, string ToDate, string ItemName, string PartCode, string IndentNo,string ReportType)
        {
            var resultList = new IndentRegisterModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPreportIndet", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@FLAG", ReportType);
                    command.Parameters.AddWithValue("@fromDate",  ParseFormattedDate(FromDate));
                    command.Parameters.AddWithValue("@ToDate", ParseFormattedDate(ToDate));
                    command.Parameters.AddWithValue("@ItemName", ItemName);
                    command.Parameters.AddWithValue("@PartCode", PartCode);
                    command.Parameters.AddWithValue("@IndentNo", IndentNo);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IndentRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new IndentRegisterModel
                                                         {
                                                             IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),
                                                             DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             CreatedByName = row["CreatedByName"] == DBNull.Value ? string.Empty : row["CreatedByName"].ToString(),
                                                             MachineNo = row["MachineNo"] == DBNull.Value ? string.Empty : row["MachineNo"].ToString(),
                                                             LastUpdationDate = row["LastUpdationDate"] == DBNull.Value ? string.Empty : row["LastUpdationDate"].ToString(),
                                                             LastUpdatedName = row["LastUpdatedName"] == DBNull.Value ? string.Empty : row["LastUpdatedName"].ToString(),
                                                             BOMtem = row["BOMtem"] == DBNull.Value ? string.Empty : row["BOMtem"].ToString(),
                                                             BOMPartCode = row["BOMPartCode"] == DBNull.Value ? string.Empty : row["BOMPartCode"].ToString(),
                                                             BOMIND = row["BOMIND"] == DBNull.Value ? string.Empty : row["BOMIND"].ToString(),
                                                             BOMQty = row["BOMQty"] == DBNull.Value ? string.Empty : row["BOMQty"].ToString(),
                                                             BOMRevNo = row["BOMRevNo"] == DBNull.Value ? string.Empty : row["BOMRevNo"].ToString(),
                                                             Approved = row["Approved"] == DBNull.Value ? string.Empty : row["Approved"].ToString(),
                                                             IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             canceled = row["canceled"] == DBNull.Value ? string.Empty : row["canceled"].ToString(),
                                                             closed = row["closed"] == DBNull.Value ? string.Empty : row["closed"].ToString(),
                                                             CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                             ApprovedDate = row["ApprovedDate"] == DBNull.Value ? string.Empty : row["ApprovedDate"].ToString(),
                                                             MRPNO = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),
                                                             MRPEntryId = row["MRPEntryId"] == DBNull.Value ? string.Empty : row["MRPEntryId"].ToString(),
                                                             MRPyearcode = row["MRPyearcode"] == DBNull.Value ? string.Empty : row["MRPyearcode"].ToString(),

                                                         }).ToList();
                    }
                }
                else if(ReportType=="Detail")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IndentRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new IndentRegisterModel
                                                         {
                                                             IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),

                                                             ItemNameOnly = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             Specification = row["Specification"] == DBNull.Value ? string.Empty : row["Specification"].ToString(),
                                                             ItemDescription = row["ItemDescription"] == DBNull.Value ? string.Empty : row["ItemDescription"].ToString(),
                                                             IndentQty = row["IndentQty"] == DBNull.Value ? string.Empty : row["IndentQty"].ToString(),
                                                             Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                             PendQtyForPO = row["PendQtyForPO"] == DBNull.Value ? string.Empty : row["PendQtyForPO"].ToString(),
                                                             ReqDate = row["ReqDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ReqDate"]).ToString("dd-MM-yyyy"),
                                                             Model = row["Model"] == DBNull.Value ? string.Empty : row["Model"].ToString(),
                                                             Size = row["Size"] == DBNull.Value ? string.Empty : row["Size"].ToString(),
                                                             Color = row["Color"] == DBNull.Value ? string.Empty : row["Color"].ToString(),
                                                             ItemRemark = row["ItemRemark"] == DBNull.Value ? string.Empty : row["ItemRemark"].ToString(),
                                                             Approvalue = row["Approvalue"] == DBNull.Value ? string.Empty : row["Approvalue"].ToString(),
                                                             AltQty = row["AltQty"] == DBNull.Value ? string.Empty : row["AltQty"].ToString(),
                                                             AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             TotalStock = row["TotalStock"] == DBNull.Value ? string.Empty : row["TotalStock"].ToString(),
                                                             Account_Name = row["Vendor1"] == DBNull.Value ? string.Empty : row["Vendor1"].ToString(),
                                                             Account_Name2 = row["vendor2"] == DBNull.Value ? string.Empty : row["vendor2"].ToString(),
                                                             DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             CreatedByName = row["CreatedByName"] == DBNull.Value ? string.Empty : row["CreatedByName"].ToString(),
                                                             MachineNo = row["MachineNo"] == DBNull.Value ? string.Empty : row["MachineNo"].ToString(),
                                                             LastUpdationDate = row["LastUpdationDate"] == DBNull.Value ? string.Empty : row["LastUpdationDate"].ToString(),
                                                             LastUpdatedName = row["LastUpdatedName"] == DBNull.Value ? string.Empty : row["LastUpdatedName"].ToString(),
                                                             BOMtem = row["BOMtem"] == DBNull.Value ? string.Empty : row["BOMtem"].ToString(),
                                                             BOMPartCode = row["BOMPartCode"] == DBNull.Value ? string.Empty : row["BOMPartCode"].ToString(),
                                                             BOMIND = row["BOMIND"] == DBNull.Value ? string.Empty : row["BOMIND"].ToString(),
                                                             BOMQty = row["BOMQty"] == DBNull.Value ? string.Empty : row["BOMQty"].ToString(),
                                                             BOMRevNo = row["BOMRevNo"] == DBNull.Value ? string.Empty : row["BOMRevNo"].ToString(),
                                                             Approved = row["Approved"] == DBNull.Value ? string.Empty : row["Approved"].ToString(),
                                                             IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             canceled = row["canceled"] == DBNull.Value ? string.Empty : row["canceled"].ToString(),
                                                             closed = row["closed"] == DBNull.Value ? string.Empty : row["closed"].ToString(),
                                                             CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                             ApprovedDate = row["ApprovedDate"] == DBNull.Value ? string.Empty : row["ApprovedDate"].ToString(),
                                                             MRPNO = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),
                                                             MRPEntryId = row["MRPEntryId"] == DBNull.Value ? string.Empty : row["MRPEntryId"].ToString(),
                                                             MRPyearcode = row["MRPyearcode"] == DBNull.Value ? string.Empty : row["MRPyearcode"].ToString(),


                                                         }).ToList();
                    }

                }
                else if(ReportType=="Detail")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IndentRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new IndentRegisterModel
                                                         {
                                                             IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),

                                                             ItemNameOnly = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             Specification = row["Specification"] == DBNull.Value ? string.Empty : row["Specification"].ToString(),
                                                             ItemDescription = row["ItemDescription"] == DBNull.Value ? string.Empty : row["ItemDescription"].ToString(),
                                                             IndentQty = row["IndentQty"] == DBNull.Value ? string.Empty : row["IndentQty"].ToString(),
                                                             Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                             PendQtyForPO = row["PendQtyForPO"] == DBNull.Value ? string.Empty : row["PendQtyForPO"].ToString(),
                                                             ReqDate = row["ReqDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ReqDate"]).ToString("dd-MM-yyyy"),
                                                             Model = row["Model"] == DBNull.Value ? string.Empty : row["Model"].ToString(),
                                                             Size = row["Size"] == DBNull.Value ? string.Empty : row["Size"].ToString(),
                                                             Color = row["Color"] == DBNull.Value ? string.Empty : row["Color"].ToString(),
                                                             ItemRemark = row["ItemRemark"] == DBNull.Value ? string.Empty : row["ItemRemark"].ToString(),
                                                             Approvalue = row["Approvalue"] == DBNull.Value ? string.Empty : row["Approvalue"].ToString(),
                                                             AltQty = row["AltQty"] == DBNull.Value ? string.Empty : row["AltQty"].ToString(),
                                                             AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             TotalStock = row["TotalStock"] == DBNull.Value ? string.Empty : row["TotalStock"].ToString(),
                                                             Account_Name = row["Vendor1"] == DBNull.Value ? string.Empty : row["Vendor1"].ToString(),
                                                             Account_Name2 = row["vendor2"] == DBNull.Value ? string.Empty : row["vendor2"].ToString(),
                                                             DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             CreatedByName = row["CreatedByName"] == DBNull.Value ? string.Empty : row["CreatedByName"].ToString(),
                                                             MachineNo = row["MachineNo"] == DBNull.Value ? string.Empty : row["MachineNo"].ToString(),
                                                             LastUpdationDate = row["LastUpdationDate"] == DBNull.Value ? string.Empty : row["LastUpdationDate"].ToString(),
                                                             LastUpdatedName = row["LastUpdatedName"] == DBNull.Value ? string.Empty : row["LastUpdatedName"].ToString(),
                                                             BOMtem = row["BOMtem"] == DBNull.Value ? string.Empty : row["BOMtem"].ToString(),
                                                             BOMPartCode = row["BOMPartCode"] == DBNull.Value ? string.Empty : row["BOMPartCode"].ToString(),
                                                             BOMIND = row["BOMIND"] == DBNull.Value ? string.Empty : row["BOMIND"].ToString(),
                                                             BOMQty = row["BOMQty"] == DBNull.Value ? string.Empty : row["BOMQty"].ToString(),
                                                             BOMRevNo = row["BOMRevNo"] == DBNull.Value ? string.Empty : row["BOMRevNo"].ToString(),
                                                             Approved = row["Approved"] == DBNull.Value ? string.Empty : row["Approved"].ToString(),
                                                             IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             canceled = row["canceled"] == DBNull.Value ? string.Empty : row["canceled"].ToString(),
                                                             closed = row["closed"] == DBNull.Value ? string.Empty : row["closed"].ToString(),
                                                             CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                             ApprovedDate = row["ApprovedDate"] == DBNull.Value ? string.Empty : row["ApprovedDate"].ToString(),
                                                             MRPNO = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),
                                                             MRPEntryId = row["MRPEntryId"] == DBNull.Value ? string.Empty : row["MRPEntryId"].ToString(),
                                                             MRPyearcode = row["MRPyearcode"] == DBNull.Value ? string.Empty : row["MRPyearcode"].ToString(),


                                                         }).ToList();
                    }

                }
                else if(ReportType== "Indent With PO Detail")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IndentRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new IndentRegisterModel
                                                         {
                                                             IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),
                                                             ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             Specification = row["Specification"] == DBNull.Value ? string.Empty : row["Specification"].ToString(),
                                                             ItemDescription = row["ItemDescription"] == DBNull.Value ? string.Empty : row["ItemDescription"].ToString(),
                                                             IndentQty = row["IndentQty"] == DBNull.Value ? string.Empty : row["IndentQty"].ToString(),
                                                             Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                             TotalPOQty = row["TotalPOQty"] == DBNull.Value ? string.Empty : row["TotalPOQty"].ToString(),
                                                             PendPOQty = row["PendPOQty"] == DBNull.Value ? string.Empty : row["PendPOQty"].ToString(),
                                                             PendQtyForPO = row["PendQtyForPO"] == DBNull.Value ? string.Empty : row["PendQtyForPO"].ToString(),
                                                             ReqDate = row["ReqDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ReqDate"]).ToString("dd-MM-yyyy"),
                                                             POVendorName = row["POVendorName"] == DBNull.Value ? string.Empty : row["POVendorName"].ToString(),
                                                             PONO = row["PONO"] == DBNull.Value ? string.Empty : row["PONO"].ToString(),
                                                             PODate = row["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PODate"]).ToString("dd-MM-yyyy"),
                                                             Model = row["Model"] == DBNull.Value ? string.Empty : row["Model"].ToString(),
                                                             Size = row["Size"] == DBNull.Value ? string.Empty : row["Size"].ToString(),
                                                             Color = row["Color"] == DBNull.Value ? string.Empty : row["Color"].ToString(),
                                                             ItemRemark = row["ItemRemark"] == DBNull.Value ? string.Empty : row["ItemRemark"].ToString(),
                                                             Approvalue = row["Approvalue"] == DBNull.Value ? string.Empty : row["Approvalue"].ToString(),
                                                             AltQty = row["AltQty"] == DBNull.Value ? string.Empty : row["AltQty"].ToString(),
                                                             AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             TotalStock = row["TotalStock"] == DBNull.Value ? string.Empty : row["TotalStock"].ToString(),
                                                             Account_Name = row["Vendor1"] == DBNull.Value ? string.Empty : row["Vendor1"].ToString(),
                                                             Account_Name2 = row["Vendor2"] == DBNull.Value ? string.Empty : row["Vendor2"].ToString(),
                                                             DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             CreatedByName = row["CreatedByName"] == DBNull.Value ? string.Empty : row["CreatedByName"].ToString(),
                                                             MachineNo = row["MachineNo"] == DBNull.Value ? string.Empty : row["MachineNo"].ToString(),
                                                             LastUpdationDate = row["LastUpdationDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["LastUpdationDate"]).ToString("dd-MM-yyyy"),
                                                             LastUpdatedName = row["LastUpdatedName"] == DBNull.Value ? string.Empty : row["LastUpdatedName"].ToString(),
                                                             BOMtem = row["BOMtem"] == DBNull.Value ? string.Empty : row["BOMtem"].ToString(),
                                                             BOMPartCode = row["BOMPartCode"] == DBNull.Value ? string.Empty : row["BOMPartCode"].ToString(),
                                                             BOMIND = row["BOMIND"] == DBNull.Value ? string.Empty : row["BOMIND"].ToString(),
                                                             BOMQty = row["BOMQty"] == DBNull.Value ? string.Empty : row["BOMQty"].ToString(),
                                                             BOMRevNo = row["BOMRevNo"] == DBNull.Value ? string.Empty : row["BOMRevNo"].ToString(),
                                                             Approved = row["Approved"] == DBNull.Value ? string.Empty : row["Approved"].ToString(),
                                                             IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             canceled = row["Canceled"] == DBNull.Value ? string.Empty : row["Canceled"].ToString(),
                                                             closed = row["Closed"] == DBNull.Value ? string.Empty : row["Closed"].ToString(),
                                                             CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                             ApprovedDate = row["ApprovedDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ApprovedDate"]).ToString("dd-MM-yyyy"),
                                                             MRPNO = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),
                                                             MRPEntryId = row["MRPEntryId"] == DBNull.Value ? string.Empty : row["MRPEntryId"].ToString(),
                                                             MRPyearcode = row["MRPyearcode"] == DBNull.Value ? string.Empty : row["MRPyearcode"].ToString()

                                                         }).ToList();
                    }

                }

                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IndentRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new IndentRegisterModel
                                                         {
                                                             //IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             //IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             //itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),

                                                             //ItemNameOnly = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             //PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             //Specification = row["Specification"] == DBNull.Value ? string.Empty : row["Specification"].ToString(),
                                                             //ItemDescription = row["ItemDescription"] == DBNull.Value ? string.Empty : row["ItemDescription"].ToString(),
                                                             //IndentQty = row["IndentQty"] == DBNull.Value ? string.Empty : row["IndentQty"].ToString(),
                                                             //Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                             //TotalPOQty = row["TotalPOQty"] == DBNull.Value ? string.Empty : row["TotalPOQty"].ToString(),
                                                             ////PendPOQty = row["PendPOQty"] == DBNull.Value ? string.Empty : row["PendPOQty"].ToString(),                                                             
                                                             //PendQtyForPO = row["PendQtyForPO"] == DBNull.Value ? string.Empty : row["PendQtyForPO"].ToString(),
                                                             //ReqDate = row["ReqDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ReqDate"]).ToString("dd-MM-yyyy"),
                                                             //POVendorName = row["POVendorName"] == DBNull.Value ? string.Empty : row["POVendorName"].ToString(),
                                                             //PONO = row["PONO"] == DBNull.Value ? string.Empty : row["PONO"].ToString(),
                                                             //PODate = row["PODate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["PODate"]).ToString("dd-MM-yyyy"),
                                                             //Model = row["Model"] == DBNull.Value ? string.Empty : row["Model"].ToString(),
                                                             //Size = row["Size"] == DBNull.Value ? string.Empty : row["Size"].ToString(),
                                                             //Color = row["Color"] == DBNull.Value ? string.Empty : row["Color"].ToString(),
                                                             //ItemRemark = row["ItemRemark"] == DBNull.Value ? string.Empty : row["ItemRemark"].ToString(),
                                                             //Approvalue = row["Approvalue"] == DBNull.Value ? string.Empty : row["Approvalue"].ToString(),
                                                             //AltQty = row["AltQty"] == DBNull.Value ? string.Empty : row["AltQty"].ToString(),
                                                             //AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                             //StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             //TotalStock = row["TotalStock"] == DBNull.Value ? string.Empty : row["TotalStock"].ToString(),
                                                             //Account_Name = row["Vendor1"] == DBNull.Value ? string.Empty : row["Vendor1"].ToString(),
                                                             //Account_Name2 = row["vendor2"] == DBNull.Value ? string.Empty : row["vendor2"].ToString(),
                                                             //DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             //IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             //IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             //CreatedByName = row["CreatedByName"] == DBNull.Value ? string.Empty : row["CreatedByName"].ToString(),
                                                             //MachineNo = row["MachineNo"] == DBNull.Value ? string.Empty : row["MachineNo"].ToString(),
                                                             //LastUpdationDate = row["LastUpdationDate"] == DBNull.Value ? string.Empty : row["LastUpdationDate"].ToString(),
                                                             //LastUpdatedName = row["LastUpdatedName"] == DBNull.Value ? string.Empty : row["LastUpdatedName"].ToString(),
                                                             //BOMtem = row["BOMtem"] == DBNull.Value ? string.Empty : row["BOMtem"].ToString(),
                                                             //BOMPartCode = row["BOMPartCode"] == DBNull.Value ? string.Empty : row["BOMPartCode"].ToString(),
                                                             //BOMIND = row["BOMIND"] == DBNull.Value ? string.Empty : row["BOMIND"].ToString(),
                                                             //BOMQty = row["BOMQty"] == DBNull.Value ? string.Empty : row["BOMQty"].ToString(),
                                                             //BOMRevNo = row["BOMRevNo"] == DBNull.Value ? string.Empty : row["BOMRevNo"].ToString(),
                                                             //Approved = row["Approved"] == DBNull.Value ? string.Empty : row["Approved"].ToString(),
                                                             //IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             //canceled = row["canceled"] == DBNull.Value ? string.Empty : row["canceled"].ToString(),
                                                             //closed = row["closed"] == DBNull.Value ? string.Empty : row["closed"].ToString(),
                                                             //CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                             //ApprovedDate = row["ApprovedDate"] == DBNull.Value ? string.Empty : row["ApprovedDate"].ToString(),
                                                             //MRPNO = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),
                                                             //MRPEntryId = row["MRPEntryId"] == DBNull.Value ? string.Empty : row["MRPEntryId"].ToString(),
                                                             //MRPyearcode = row["MRPyearcode"] == DBNull.Value ? string.Empty : row["MRPyearcode"].ToString(),
                                                             IndentNo = row["IndentNo"] == DBNull.Value ? string.Empty : row["IndentNo"].ToString(),
                                                             IndentDate = row["IndentDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["IndentDate"]).ToString("dd-MM-yyyy"),
                                                             itemservice = row["Item/Service"] == DBNull.Value ? string.Empty : row["Item/Service"].ToString(),
                                                             ItemNameOnly = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             Specification = row["Specification"] == DBNull.Value ? string.Empty : row["Specification"].ToString(),
                                                             ItemDescription = row["ItemDescription"] == DBNull.Value ? string.Empty : row["ItemDescription"].ToString(),
                                                             IndentQty = row["IndentQty"] == DBNull.Value ? string.Empty : row["IndentQty"].ToString(),
                                                             Unit = row["Unit"] == DBNull.Value ? string.Empty : row["Unit"].ToString(),
                                                             TotalPOQty = row["TotalPOQty"] == DBNull.Value ? string.Empty : row["TotalPOQty"].ToString(),
                                                             PendQtyForPO = row["PendQtyForPO"] == DBNull.Value ? string.Empty : row["PendQtyForPO"].ToString(),
                                                             ReqDate = row["ReqDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["ReqDate"]).ToString("dd-MM-yyyy"),
                                                             Model = row["Model"] == DBNull.Value ? string.Empty : row["Model"].ToString(),
                                                             Size = row["Size"] == DBNull.Value ? string.Empty : row["Size"].ToString(),
                                                             Color = row["Color"] == DBNull.Value ? string.Empty : row["Color"].ToString(),
                                                             ItemRemark = row["ItemRemark"] == DBNull.Value ? string.Empty : row["ItemRemark"].ToString(),
                                                             Approvalue = row["Approvalue"] == DBNull.Value ? string.Empty : row["Approvalue"].ToString(),
                                                             AltQty = row["AltQty"] == DBNull.Value ? string.Empty : row["AltQty"].ToString(),
                                                             AltUnit = row["AltUnit"] == DBNull.Value ? string.Empty : row["AltUnit"].ToString(),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             Account_Name = row["Vendor1"] == DBNull.Value ? string.Empty : row["Vendor1"].ToString(),
                                                             Account_Name2 = row["vendor2"] == DBNull.Value ? string.Empty : row["vendor2"].ToString(),
                                                             DeptName = row["ForDepartment"] == DBNull.Value ? string.Empty : row["ForDepartment"].ToString(),
                                                             IndentorName = row["IndentorName"] == DBNull.Value ? string.Empty : row["IndentorName"].ToString(),
                                                             IndentRemark = row["IndentRemark"] == DBNull.Value ? string.Empty : row["IndentRemark"].ToString(),
                                                             IndentCompleted = row["IndentCompleted"] == DBNull.Value ? string.Empty : row["IndentCompleted"].ToString(),
                                                             canceled = row["Canceled"] == DBNull.Value ? string.Empty : row["Canceled"].ToString(),
                                                             closed = row["Closed"] == DBNull.Value ? string.Empty : row["Closed"].ToString()
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
