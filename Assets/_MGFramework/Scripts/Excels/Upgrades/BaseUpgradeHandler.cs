using _KMH_Framework;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public enum UpgradeType
    {
        None = -1,

        Health,
        MoveSpeed,
        AttackSpeed,
        AttackDamage,
    }

    [System.Serializable]
    public abstract class BaseUpgradeHandler<T> where T : IExcelRow, new()
    {
        private const string LOG_FORMAT = "<color=white><b>[BaseUpgradeHandler]</b></color> {0}";

        [SerializeField]
        [SerializedDictionary("UpgradeType", "Controller")]
        private SerializedDictionary<UpgradeType, UpgradeController> controllerDic = new SerializedDictionary<UpgradeType, UpgradeController>();

        [ReadOnly]
        [SerializeField]
        private List<T> rowList;
        protected T _minRow;
        protected T _maxRow;

        private CostExcelRow _costRow;

        public delegate void UpgradeCostDelegate(UpgradeType type, int costValue);
        public event UpgradeCostDelegate OnUpgradeCostChanged;

        public void Initialize(CostExcelRow costRow)
        {
            _costRow = costRow; 

            rowList = ExcelReadHandler.Instance.GetSheet<T>();
            _minRow = rowList.Find(x => x.GetName() == "min");
            _maxRow = rowList.Find(x => x.GetName() == "max");

            Debug.Assert(_minRow != null);
            Debug.Assert(_maxRow != null);

            foreach (UpgradeType type in controllerDic.Keys)
            {
                controllerDic[type].Initialize();
                EvaluateValue(0f, type);
            }
        }

        public int GetOriginCost(UpgradeType type)
        {
            int originCost = _costRow.GetOriginCost(type);
            return originCost;
        }

        public void Upgrade(UpgradeType type)
        {
            if (controllerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "딕셔너리에 키가 존재하지 않습니다 : " + type);
                return;
            }

            UpgradeController controller = controllerDic[type];
            float normal = controller.Upgrade(out int count);
            EvaluateValue(normal, type);

            int costValue = _costRow.GetUpgradedCost(type, count);
            OnUpgradeCostChanged?.Invoke(type, costValue);
        }

        protected abstract void EvaluateValue(float normal, UpgradeType type);
    }

    [System.Serializable]
    public class UpgradeController
    {
        [ReadOnly]
        [SerializeField]
        private int currentRate = 0;
        [SerializeField]
        private float curveStepness = 0.075f;

        public void Initialize()
        {
            currentRate = 0;
        }

        /// <summary>
        /// Upgrade (add currentRate one) and calculate normal value
        /// </summary>
        /// <returns>normal from sigmoid function</returns>
        public float Upgrade(out int count)
        {
            currentRate++;
            count = currentRate;

            float normal = Sigmoid(currentRate);
            return normal;
        }

        private float Sigmoid(float x)
        {
            return (2f / (1f + Mathf.Exp(-curveStepness * x))) - 1f;
        }
    }
}
