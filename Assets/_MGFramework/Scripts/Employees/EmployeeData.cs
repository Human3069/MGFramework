using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class EmployeeData 
    {
        public float AttackDamage;
        public float AttackSpeed;
        public float AttackRange;

        [Space(10)]
        public bool IsShowLog;
    }
}
