using _KMH_Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class UpgradeController : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private List<EmployeeExcelRow> employeeRowList;
        [ReadOnly]
        [SerializeField]
        private List<HunterExcelRow> hunterRowList;

        private void Awake()
        {
            employeeRowList = ExcelReadHandler.Instance.GetSheet<EmployeeExcelRow>();
            hunterRowList = ExcelReadHandler.Instance.GetSheet<HunterExcelRow>();
        }

        [SerializeField]
        private float curveSteepness = 0.25f; // Sigmoid curve steepness
        [ReadOnly]
        [SerializeField]
        private float index = 0f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                float sigmoidValue = Sigmoid(index, curveSteepness);
                Debug.Log("Sigmoid Value: " + sigmoidValue);

                index++;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("√ ±‚»≠");
                index = 0f;
            }
        }

        private float Sigmoid(float x, float k = 1f)
        {
            return (2f / (1f + Mathf.Exp(-k * x))) - 1f;
        }
    }
}
