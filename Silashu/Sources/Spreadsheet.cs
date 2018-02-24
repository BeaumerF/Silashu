using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Xml;
using System.Drawing;
using OfficeOpenXml.Style;

namespace EmailScraper
    {
    class Spreadsheet
    {
        string file = @"./List.xlsx";

        FileStream _myFile;
        public Spreadsheet()
        {

            if (!File.Exists(file))
            {
               _myFile = File.Create(file);
                return;
            }
            File.Delete(file);
            _myFile = File.Create(file);
        }

        //make a spreadshit
        public void SpreadsheetConvert(List<Target> targets)
        {
            using (var package = new ExcelPackage(_myFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("list");

                int count = 1;
                worksheet.Cells["A" + count].Value = "Nom";
                worksheet.Cells["B" + count].Value = "Email";
                worksheet.Cells["C" + count].Value = "Url";
                ExcelRange modelTable = worksheet.Cells["A" + count + ":C" + count];
                modelTable.Style.Fill.PatternType = ExcelFillStyle.Solid;
                modelTable.Style.Fill.BackgroundColor.SetColor(Color.Black);
                modelTable.Style.Font.Color.SetColor(Color.White);
                modelTable.Style.Font.Bold = true;

                bool lineIsPair = true;
                foreach (var target in targets)
                {
                    ++count;
                    worksheet.Cells["A" + count].Value = target.Name;
                    worksheet.Cells["B" + count].Value = target.Email;
                    worksheet.Cells["C" + count].Value = target.Url;

                    modelTable = worksheet.Cells["A" + count + ":C" + count];
                    if (lineIsPair)
                    {
                        modelTable.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        modelTable.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        lineIsPair = false;
                    }
                    else
                        lineIsPair = true;
                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
        }
    }
}
