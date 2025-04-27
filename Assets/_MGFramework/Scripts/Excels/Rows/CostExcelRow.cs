using _KMH_Framework;
using System;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class CostExcelRow : IExcelRow
    {
        public string Name;
        public float Health;
        public float MovementSpeed;
        public float AttackSpeed;
        public float AttackDamage;
        public float GrowthRate;

        public string GetName()
        {
            return Name;
        }

        public void Validate()
        {
            this.Name = Name.Trim().Replace(" ", "").ToLower();
        }

        public int GetOriginCost(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Health:
                    return (int)Health;

                case UpgradeType.MoveSpeed:
                    return (int)MovementSpeed;

                case UpgradeType.AttackSpeed:
                    return (int)AttackSpeed;

                case UpgradeType.AttackDamage:
                    return (int)AttackDamage;

                default:
                    throw new Exception("Invalid UpgradeType: " + type);
            }
        }

        public int GetUpgradedCost(UpgradeType type, int upgradedCount)
        {
            int originCost = GetOriginCost(type);
            float powered = Mathf.Pow(GrowthRate, upgradedCount);
            float result = originCost * powered;

            return (int)result;
        }

        public override string ToString()
        {
            return "Name : " + Name + ", Health : " + Health + ", MovementSpeed : " + MovementSpeed + ", AttackSpeed : " + AttackSpeed + ", AttackDamage : " + AttackDamage + ", GrowthRate : " + GrowthRate;
        }
    }
}
