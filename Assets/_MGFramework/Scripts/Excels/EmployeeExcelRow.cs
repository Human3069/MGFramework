using _KMH_Framework;

namespace MGFramework
{
    [System.Serializable]
    public class EmployeeExcelRow : IExcelRow
    {
        public string Name;
        public float MovementSpeed;
        public float AttackSpeed;
        public float AttackDamage;

        public void Validate()
        {
            this.Name = Name.Trim().Replace(" ", "").ToLower();
        }

        public override string ToString()
        {
            return "Name : " + Name + ", MovementSpeed : " + MovementSpeed + ", AttackSpeed : " + AttackSpeed + ", AttackDamage : " + AttackDamage;
        }
    }
}
