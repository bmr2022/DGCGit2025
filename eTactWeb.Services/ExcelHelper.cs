using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace eTactWeb.Services
{
    public static class ExcelHelper
    {
        public static MemoryStream GenerateExcel(
        DataTable dt,
        string sheetName,
        string companyName,
        string branchName,
        string fromDate = "",
        string toDate = "")
        {
            var stream = new MemoryStream();
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(sheetName);
                int totalColumns = dt.Columns.Count;

                // 🔹 Company + Branch
                ws.Cell(1, 1).Value = $"Company: {companyName}    Branch: {branchName}";
                ws.Range(1, 1, 1, totalColumns).Merge();
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                ws.Row(1).Style.Font.FontColor = XLColor.White;
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.FontSize = 14;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 🔹 From / To Date
                if (!string.IsNullOrEmpty(fromDate) || !string.IsNullOrEmpty(toDate))
                {
                    ws.Cell(2, 1).Value = $"From Date: {fromDate}   To Date: {toDate}";
                    ws.Range(2, 1, 2, totalColumns).Merge();
                    ws.Row(2).Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                    ws.Row(2).Style.Font.FontColor = XLColor.White;
                    ws.Row(2).Style.Font.Bold = true;
                    ws.Row(2).Style.Font.FontSize = 14;
                    ws.Row(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // 🔹 Insert table starting from row 4
                ws.Cell(4, 1).InsertTable(dt, $"{sheetName}Table", true);

                // Style header row
                var headerRow = ws.Row(4);
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                headerRow.Style.Font.FontColor = XLColor.Black;
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 🔹 Add Totals row for numeric columns
                int dataRowStart = 5;
                int dataRowEnd = dt.Rows.Count + 4; // data starts at row 5

                if (dataRowEnd >= dataRowStart)
                {
                    int totalRow = dataRowEnd + 1;
                    ws.Cell(totalRow, 1).Value = "Total";
                    ws.Range(totalRow, 1, totalRow, 2).Merge();
                    ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(totalRow, 1).Style.Font.Bold = true;

                    // Loop through columns
                    for (int col = 3; col <= totalColumns; col++)
                    {
                        var dataColumn = dt.Columns[col - 1]; // DataTable is 0-based
                                                              // Check if numeric type
                        if (
                            dataColumn.DataType == typeof(decimal) ||
                            dataColumn.DataType == typeof(double) ||
                            dataColumn.DataType == typeof(float))
                        {
                            string colLetter = ws.Cell(4, col).Address.ColumnLetter; // header row
                            ws.Cell(totalRow, col).FormulaA1 = $"SUM({colLetter}{dataRowStart}:{colLetter}{dataRowEnd})";
                        }
                    }

                    var totalRange = ws.Range(totalRow, 1, totalRow, totalColumns);
                    totalRange.Style.Font.Bold = true;
                    totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                    totalRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                // Auto-fit columns
                ws.Columns().AdjustToContents();
                wb.SaveAs(stream);
            }
            stream.Position = 0;
            return stream;
        }
    }
}

