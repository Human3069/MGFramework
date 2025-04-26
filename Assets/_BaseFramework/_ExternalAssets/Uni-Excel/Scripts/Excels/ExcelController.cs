using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    [Serializable]
    public class ExcelController
    {
        [SerializeField]
        private string directory;
        [SerializeField]
        private string fileName;

        [Space(10)]
        [SerializeField]
        [SerializedDictionary("Type", "SheetController")]
        private SerializedDictionary<DotNetPair, SheetController> sheetDic = new SerializedDictionary<DotNetPair, SheetController>();

        private bool hasRead = false;

        private void Read()
        {
            foreach (KeyValuePair<DotNetPair, SheetController> pair in sheetDic)
            {
                string sheetName = pair.Value.SheetName;
                Type genericType = pair.Key.GetValueType();
                ISheetController controller = genericType.CreateGenericSheetController(sheetName);

                List<IExcelRow> rowList = controller.Read(directory, fileName);
                pair.Value.SetRowList(rowList);
            }

            hasRead = true;
        }

        public List<T> GetSheet<T>() where T : IExcelRow, new()
        {
            if (hasRead == false)
            {
                Read();
            }

            foreach (KeyValuePair<DotNetPair, SheetController> pair in sheetDic)
            {
                if (pair.Key.GetValueType() == typeof(T))
                {
                    List<T> castedRowList = pair.Value.GetCastedRows<T>();
                    return castedRowList;
                }
            }

            Debug.Assert(false);
            return null;
        }
    }
}
