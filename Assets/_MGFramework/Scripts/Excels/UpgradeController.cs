using _KMH_Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class UpgradeController : MonoBehaviour
    {
        private const float CURVE_STEEPNESS = 0.25f; // Sigmoid curve steepness

        [ReadOnly]
        [SerializeField]
        private List<CostExcelRow> costRowList;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private List<PlayerExcelRow> playerRowList;
        [ReadOnly]
        [SerializeField]
        private List<EmployeeExcelRow> employeeRowList;
        [ReadOnly]
        [SerializeField]
        private List<HunterExcelRow> hunterRowList;

        private void Awake()
        {
            costRowList = ExcelReadHandler.Instance.GetSheet<CostExcelRow>();

            playerRowList = ExcelReadHandler.Instance.GetSheet<PlayerExcelRow>();
            employeeRowList = ExcelReadHandler.Instance.GetSheet<EmployeeExcelRow>();
            hunterRowList = ExcelReadHandler.Instance.GetSheet<HunterExcelRow>();
        }

        private float index = 0f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                float sigmoidValue = Sigmoid(index);
                Debug.Log("Sigmoid Value: " + sigmoidValue);

                index++;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("√ ±‚»≠");
                index = 0f;
            }
        }

        private float Sigmoid(float x)
        {
            return (2f / (1f + Mathf.Exp(-CURVE_STEEPNESS * x))) - 1f;
        }
    }
}
