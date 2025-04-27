using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class EmployeeUpgradeHandler : BaseUpgradeHandler<EmployeeExcelRow>
    {
        protected override void EvaluateValue(float normal, UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.MoveSpeed:
                    float moveSpeed = Mathf.Lerp(_minRow.MovementSpeed, _maxRow.MovementSpeed, normal);
                    break;

                case UpgradeType.AttackSpeed:
                    float attackSpeed = Mathf.Lerp(_minRow.AttackSpeed, _maxRow.AttackSpeed, normal);
                    break;

                case UpgradeType.AttackDamage:
                    float attackDamage = Mathf.Lerp(_minRow.AttackDamage, _maxRow.AttackDamage, normal);
                    break;

                default:
                    throw new System.NotImplementedException("type : " + type);
            }
        }
    }
}