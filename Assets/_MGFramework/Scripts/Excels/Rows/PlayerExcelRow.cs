using _KMH_Framework;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerExcelRow : IExcelRow 
    {
        public string Name;
        public float Health;
        public float MovementSpeed;
        public float AttackSpeed;
        public float AttackDamage;

        public string GetName()
        {
            return Name;
        }

        public void Validate()
        {
            this.Name = Name.Trim().Replace(" ", "").ToLower();
        }

        public override string ToString()
        {
            return "Name : " + Name + ", Health : " + Health + ", MovementSpeed : " + MovementSpeed + ", AttackSpeed : " + AttackSpeed + ", AttackDamage : " + AttackDamage;
        }
    }
}
