#region Copyright
// Copyright © 1996-2010 EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EPiServer.Templates.Advanced.PageProviders
{
    /// <summary>
    /// Parses data from excel file and returns property values
    /// </summary>
    public class ExcelParser
    {
        private string _excelFilePath;
        private string _idColumnName;
        private string _pageNameColumnName;
        private string _sheetName;

        /// <summary>
        /// Creates Excel parser
        /// </summary>
        /// <param name="excelFilePath">Path to the file</param>
        /// <param name="idColumnName">Id column name</param>
        /// <param name="sheetName">Sheet name</param>
        public ExcelParser(string excelFilePath, string idColumnName, string pageNameColumnName, string sheetName)
        {
            _excelFilePath = excelFilePath;
            _idColumnName = idColumnName;
            _sheetName = sheetName;
            _pageNameColumnName = pageNameColumnName;
        }

        /// <summary>
        /// Gets properties and values for specified pageDataID
        /// </summary>
        /// <param name="pageDataID">PageDataID</param>
        /// <returns>Dictionary of properties and their values</returns>
        public Dictionary<string, string> GetPageProperties(int pageDataID)
        {
            Workbook workbook;
            WorksheetPart workSheetPart;
            SharedStringTable sharedStrings;
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (Stream stream = new FileStream(_excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
                {
                    workbook = document.WorkbookPart.Workbook;
                    sharedStrings = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    Sheet sheet = workbook.Descendants<Sheet>().FirstOrDefault(item => item.Name == _sheetName);
                    if (sheet != null && (workSheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id)) != null)
                    {
                        result = GetRowValues(workSheetPart.Worksheet, sharedStrings, pageDataID);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets IDs of available in excel file pages
        /// </summary>
        /// <returns></returns>
        public List<int> GetPageIDs()
        {
            List<int> result = new List<int>();

            Workbook workbook;
            WorksheetPart workSheetPart;
            SharedStringTable sharedStrings;

            using (Stream stream = new FileStream(_excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
                {
                    workbook = document.WorkbookPart.Workbook;
                    sharedStrings = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    Sheet sheet = workbook.Descendants<Sheet>().FirstOrDefault(item => item.Name == _sheetName);
                    if (sheet != null && (workSheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id)) != null)
                    {
                        Dictionary<string, string> headings = GetColumnHeadings(workSheetPart.Worksheet, sharedStrings);

                        string idColumnIndex = headings.FirstOrDefault(item => item.Value == _idColumnName).Key;

                        List<string> columnValues = new List<string>();

                        foreach (Row row in workSheetPart.Worksheet.Descendants<Row>().Where(item => item.RowIndex > 1))
                        {
                            Cell cell = row.Descendants<Cell>().FirstOrDefault(item => GetColumnIndex(item.CellReference.Value) == idColumnIndex);
                            string cellValue = cell == null ? string.Empty : SelectCellValue(cell, sharedStrings);
                            columnValues.Add(cellValue);
                        }

                        foreach (String idInString in columnValues)
                        {
                            int id = 0;
                            if (!int.TryParse(idInString, out id))
                            {
                                string message = string.IsNullOrEmpty(idInString) ? "Value in id column is empty." :
                                    string.Format("{0} is not an integer value.", id);
                                throw new ExcelParserException(_excelFilePath, string.Format("{0} ID column should not be empty and has to contain only integer values.", message));
                            }
                            result.Add(id);
                        }
                    }
                }
            }

            foreach (IGrouping<int, int> item in result.GroupBy(item => item))
            {
                if (item.Count() > 1)
                {
                    throw new ExcelParserException(_excelFilePath, string.Format("Value {0} is duplicated in ID column.", item.First()));
                }
            }

            return result;
        }

        #region Help Methods

        private Dictionary<string, string> GetColumnHeadings(Worksheet workSheet, SharedStringTable sharedStrings)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Row row = workSheet.Descendants<Row>().FirstOrDefault(item => item.RowIndex == 1);

            if (row != null)
            {
                foreach (Cell cell in row.Descendants<Cell>())
                {
                    string cellValue = SelectCellValue(cell, sharedStrings);

                    if (string.IsNullOrEmpty(cellValue))
                    {
                        throw new ExcelParserException(_excelFilePath, "Column heading has not to be empty.");
                    }

                    result.Add(GetColumnIndex(cell.CellReference.Value), cellValue);
                }
                
                if (!result.ContainsValue(_idColumnName))
                {
                    throw new ExcelParserException(_excelFilePath, string.Format("ID column with title {0} should be in specified excel worksheet.", _idColumnName));
                }

                if (!result.ContainsValue(_pageNameColumnName))
                {
                    throw new ExcelParserException(_excelFilePath, string.Format("Page name column with title {0} should be in specified excel worksheet.", _pageNameColumnName));
                }
            }

            return result;
        }

        private Dictionary<string, string> GetRowValues(Worksheet workSheet, SharedStringTable sharedStrings, int pageDataID)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Dictionary<string, string> columnHeadings = GetColumnHeadings(workSheet, sharedStrings);
            string idColumnIndex = columnHeadings.FirstOrDefault(item => item.Value == _idColumnName).Key;

            Row row = null;

            foreach (Row currentRow in workSheet.Descendants<Row>().Where(item => item.RowIndex > 1))
            {
                Cell cell = currentRow.Descendants<Cell>().FirstOrDefault(item => GetColumnIndex(item.CellReference.Value) == idColumnIndex);
                if (cell != null && cell.InnerText == pageDataID.ToString())
                {
                    row = currentRow;
                }
            }            

            if (row != null)
            {
                IEnumerable<Cell> rowCells = row.Descendants<Cell>();

                foreach (KeyValuePair<string, string> keyValuePair in columnHeadings)
                {
                    Cell currentCell = rowCells.FirstOrDefault(item => GetColumnIndex(item.CellReference.Value) == keyValuePair.Key);
                    string cellValue = currentCell == null ? string.Empty : SelectCellValue(currentCell, sharedStrings);

                    if (keyValuePair.Value == _pageNameColumnName && string.IsNullOrEmpty(cellValue))
                    {
                        throw new ExcelParserException(_excelFilePath, string.Format("Cell in page name column {0} can not be empty.", _pageNameColumnName));
                    }

                    result.Add(keyValuePair.Value, cellValue);
                }
            }

            return result;
        }

        private string SelectCellValue(Cell cell, SharedStringTable sharedStrings)
        {
            string result = string.Empty;
            if (cell.CellValue != null)
            {
                result = cell.DataType != null &&
                       cell.DataType.HasValue &&
                       cell.DataType == CellValues.SharedString ?
                       sharedStrings.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText :
                       cell.CellValue.InnerText;
            }
            return result;
        }

        private string GetColumnIndex(string value)
        {
            StringBuilder result = new StringBuilder();
            char[] array = value.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                if (char.IsLetter(array[i]))
                {
                    result.Append(array[i]);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
