using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace _KMH_Framework
{ 
    public static class NPOIExcelReadUtility
    {
        private const string LOG_FORMAT = "<color=white><b>[NPOIExcelReader]</b></color> {0}";

        public static List<T> ReadExcel<T>(string streamingAssetExcelUri, string streamingAssetExcelName, string sheetName) where T : IExcelRow, new()
        {
            Debug.Assert(string.IsNullOrEmpty(streamingAssetExcelUri) == false);
            Debug.Assert(string.IsNullOrEmpty(streamingAssetExcelName) == false);
            Debug.Assert(string.IsNullOrEmpty(sheetName) == false);

            if (streamingAssetExcelName.Contains(".xls") == false)
            {
                streamingAssetExcelName += ".xls";
            }

            string excelFullPath = Application.streamingAssetsPath + "/" + streamingAssetExcelUri + "/" + streamingAssetExcelName;

            Debug.LogFormat(LOG_FORMAT, "ReadExcel(), excelFullPath : <color=white><b>" + excelFullPath + "</b></color>");

            HSSFWorkbook _workbook;
            using (FileStream _fileStream = new FileStream(excelFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                _workbook = new HSSFWorkbook(_fileStream);
            }

            ISheet readSheet = _workbook.GetSheet(sheetName);
            List<T> rowList = readSheet.AsRowList<T>();

            return rowList;
        }

        private static List<T> AsRowList<T>(this ISheet sheet) where T : IExcelRow, new()
        {
            int rowCount = sheet.LastRowNum + 1;
            List<T> rowInstanceList = new List<T>();

            string[] fieldTitles = GetTitles<T>();
            for (int i = 1; i < rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                int cellCount = row.LastCellNum;

                T rowInstance = new T();
                Debug.Assert(fieldTitles.Length == cellCount);

                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = row.GetCell(j);
                    object cellObject = cell.ToObject();

                    rowInstance.SetFieldValue(fieldTitles[j], cellObject);
                }

                rowInstance.Validate();
                rowInstanceList.Add(rowInstance);
            }

            return rowInstanceList;
        }

        private static string[] GetTitles<T>() where T : IExcelRow, new()
        {
            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Instance |
                                                         BindingFlags.Public);

            string[] titles = new string[fieldInfos.Length];
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                titles[i] = fieldInfos[i].Name;
            }

            return titles;
        }

        public static object ToObject(this ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            object parsedCell;
            switch (cell.CellType)
            {
                case CellType.String:
                    parsedCell = cell.StringCellValue;
                    break;

                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell) == true)
                    {
                        parsedCell = cell.DateCellValue;
                    }
                    else
                    {
                        parsedCell = cell.NumericCellValue; // double 타입
                    }
                    break;

                case CellType.Boolean:
                    parsedCell = cell.BooleanCellValue;
                    break;

                case CellType.Blank:
                    parsedCell = null;
                    break;

                default:
                    parsedCell = cell.ToString();
                    break;
            }

            return parsedCell;
        }

        public static void SetFieldValue(this object obj, string fieldName, object boxedValue)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.Public |
                                                       BindingFlags.Instance);

            if (field.FieldType == typeof(int))
            {
                int value = (int)boxedValue;
                field.SetValue(obj, value);
            }
            else if (field.FieldType == typeof(float))
            {
                float value = Convert.ToSingle(boxedValue);
                field.SetValue(obj, value);
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(obj, boxedValue);
            }
            else
            {
                Debug.AssertFormat(false, "type is " + field.FieldType);
            }
        }

        /// <summary>
        /// GenericType에 해당하는 SheetController를 생성합니다.
        /// </summary>
        /// <param name="genericType">엑셀의 Row로 사용될 제너릭 타입</param>
        /// <param name="sheetName">필드로 사용되는 sheetName</param>
        /// <returns>Generic이 붙은 SheetController => interface 타입</returns>
        public static ISheetController CreateGenericSheetController(this Type genericType, string sheetName)
        {
            Debug.Assert(genericType != null);

            Type genericControllerType = typeof(SheetController<>).MakeGenericType(genericType);
            object instance = Activator.CreateInstance(genericControllerType, new object[] { sheetName });

            return instance as ISheetController;
        }
    }
}