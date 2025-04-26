using System;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    [System.Serializable]
    public class SheetController : ISheetController
    {
        public string SheetName;
        private List<IExcelRow> _rowList;

        public List<IExcelRow> Read(string directory, string fileName)
        {
            Debug.Assert(false);
            return null;
        }

        public List<T> GetCastedRows<T>() where T : IExcelRow, new()
        {
            List<T> castedRowList = _rowList.ConvertAll(row => (T)row);
            return castedRowList;
        }

        public void SetRowList(List<IExcelRow> rowList)
        {
            this._rowList = rowList;
        }
    }
}
