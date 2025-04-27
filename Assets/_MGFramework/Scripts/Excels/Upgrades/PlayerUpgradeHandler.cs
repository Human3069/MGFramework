using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerUpgradeHandler : BaseUpgradeHandler<PlayerExcelRow>
    {
        protected override void EvaluateValue(float normal, UpgradeType type)
        {
            float value;
            switch (type)
            {
                case UpgradeType.Health:
                    value = Mathf.Lerp(_minRow.Health, _maxRow.Health, normal);
                    break;

                case UpgradeType.MoveSpeed:
                    value = Mathf.Lerp(_minRow.MovementSpeed, _maxRow.MovementSpeed, normal);
                    break;

                case UpgradeType.AttackSpeed:
                    value = Mathf.Lerp(_minRow.AttackSpeed, _maxRow.AttackSpeed, normal);
                    break;

                case UpgradeType.AttackDamage:
                    value = Mathf.Lerp(_minRow.AttackDamage, _maxRow.AttackDamage, normal);
                    break;

                default:
                    throw new System.NotImplementedException("type : " + type);
            }

            Player.Instance.Upgrade(value, type);
        }
    }
}
