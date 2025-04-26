using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    public enum ExcelType
    {
        None = -1,

        UpgradeInfo
    }

    public class ExcelReadHandler : MonoSingleton<ExcelReadHandler>
    {
        [SerializeField]
        private ExcelController excelController;

        public List<T> GetSheet<T>() where T : IExcelRow, new()
        {
            return excelController.GetSheet<T>();
        }
    }
}