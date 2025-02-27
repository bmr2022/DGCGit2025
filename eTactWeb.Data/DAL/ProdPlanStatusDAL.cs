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
    public class ProdPlanStatusDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public ProdPlanStatusDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        
        public async Task<ProdPlanStatusModel> GetProdPlanStatus()
        {
            var resultList = new ProdPlanStatusModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportProdProdPlanStatus", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@Flag", "ProdPlanStatus");
                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.ProdPlanStatusGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new ProdPlanStatusModel
                                                    {
                                                        PartCode = row["PartCode"].ToString(),
                                                        ItemName = row["ItemName"]?.ToString(),
                                                        PlanNo = row["PlanNo"]?.ToString(),
                                                        ProdPlanDate = row.Field<DateTime>("ProdPlanDate"),
                                                        PlanNoYearCode = row.IsNull("PlanNoYearCode") ? 0 : row.Field<long>("PlanNoYearCode"),
                                                        ProdSchNo = row["ProdScno"]?.ToString(),
                                                        ProdSchDate = row.IsNull("ProdSchDate") ? (DateTime?)null : row.Field<DateTime?>("ProdSchDate"),
                                                        SaleOrderQty = row.IsNull("SaleOrderQty") ? 0 : row.Field<double>("SaleOrderQty"),
                                                        ProdPlanQty = row.IsNull("ProdPlanQty") ? 0 : row.Field<double>("ProdPlanQty"),
                                                        ProdSchQty = row.IsNull("ProdSchQty") ? 0 : row.Field<double>("ProdSchQty"),
                                                        ProdQty = row.IsNull("ProdQty") ? 0 : row.Field<double>("ProdQty"),
                                                        OKProdQty = row.IsNull("OKProdQty") ? 0 : row.Field<double>("OKProdQty"),
                                                        PendQtyForProd = row.IsNull("PendQtyForProd") ? 0 : row.Field<double>("PendQtyForProd"),
                                                        QCOkQty = row.IsNull("QCOkQty") ? 0 : row.Field<double>("QCOkQty"),
                                                        RewQty = row.IsNull("RewQty") ? 0 : row.Field<double>("RewQty"),
                                                        RejQty = row.IsNull("RejQty") ? 0 : row.Field<double>("RejQty"),
                                                        PendForQc = row.IsNull("PendForQc") ? 0 : row.Field<double>("PendForQc"),
                                                        ItemCode = row.IsNull("itemcode") ? 0 : row.Field<long>("itemcode"),
                                                        ProdPlanDeactive = row["ProdPlanDeactive"]?.ToString(),
                                                        DODeactivate = row.IsNull("DODeactive") ? (DateTime?)null : row.Field<DateTime?>("DODeactive"),
                                                        ProdPlanClosed = row["ProdPlanClosed"]?.ToString(),
                                                        CloseDate = row.IsNull("CloseDate") ? (DateTime?)null : row.Field<DateTime?>("CloseDate"),
                                                        PlanStatus = row["PlanStatus"]?.ToString(),
                                                        RemarkForProduction = row["RemarkForProduction"]?.ToString(),
                                                        OtherInstruction = row["OtherInstruction"]?.ToString()
                                                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching BOM tree data.", ex);
            }

            return resultList;
        }


    }
}

