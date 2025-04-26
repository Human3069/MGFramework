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
        /// generic을 IExcelRow로 업캐스팅하여 리턴함.
        /// </summary>
        /// <param name="directory">읽을 엑셀의 주소</param>
        /// <param name="fileName">읽을 엑셀의 이름</param>
        /// <returns>T[] 업캐스팅 -> IExcelRow[]</returns>
        public List<IExcelRow> Read(string directory, string fileName)
        {
            List<T> rowInstanceList = NPOIExcelReadUtility.ReadExcel<T>(directory, fileName, _sheetName);
            List<IExcelRow> castedRowInstanceList = rowInstanceList.ConvertAll(row => (IExcelRow)row);
     
            return castedRowInstanceList;
        }
    }
}