using _KMH_Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        [ReadOnly]
        [SerializeField]
        private List<CostExcelRow> costRowList;

        [Space(10)]
        public PlayerUpgradeHandler PlayerHandler;
        public EmployeeUpgradeHandler EmployeeHandler;
        public HunterUpgradeHandler HunterHandler;

        private void Awake()
        {
            costRowList = ExcelReadHandler.Instance.GetSheet<CostExcelRow>();
            CostExcelRow playerCostRow = costRowList.Find(x => x.GetName() == "player");
            CostExcelRow employeeCostRow = costRowList.Find(x => x.GetName() == "employee");
            CostExcelRow hunterCostRow = costRowList.Find(x => x.GetName() == "hunter");

            PlayerHandler.Initialize(playerCostRow);
            EmployeeHandler.Initialize(employeeCostRow);
            HunterHandler.Initialize(hunterCostRow);
        }

        public T GetUpgrader<T, U>() where T : BaseUpgradeHandler<U>
                                     where U : IExcelRow, new()
        {
            if (typeof(T) == typeof(PlayerUpgradeHandler))
            {
                return PlayerHandler as T;
            }
            else if (typeof(T) == typeof(EmployeeUpgradeHandler))
            {
                return EmployeeHandler as T;
            }
            else if (typeof(T) == typeof(HunterUpgradeHandler))
            {
                return HunterHandler as T;
            }
            else
            {
                throw new System.NotImplementedException("typeof(T) : " + typeof(T));
            }
        }
    }
}
