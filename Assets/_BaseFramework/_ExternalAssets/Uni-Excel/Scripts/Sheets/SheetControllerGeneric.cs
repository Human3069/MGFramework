using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    public class SheetController<T> : ISheetController where T : IExcelRow, new()
    {
        private string _sheetName;
        
        public SheetController(string sheetName)
        {
            this._sheetName = sheetName;
        }

        /// <summary>
        /// generic�� IExcelRow�� ��ĳ�����Ͽ� ������.
        /// </summary>
        /// <param name="directory">���� ������ �ּ�</param>
        /// <param name="fileName">���� ������ �̸�</param>
        /// <returns>T[] ��ĳ���� -> IExcelRow[]</returns>
        public List<IExcelRow> Read(string directory, string fileName)
        {
            List<T> rowInstanceList = NPOIExcelReadUtility.ReadExcel<T>(directory, fileName, _sheetName);
            List<IExcelRow> castedRowInstanceList = rowInstanceList.ConvertAll(row => (IExcelRow)row);
     
            return castedRowInstanceList;
        }
    }
}