using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerData 
    {
        [Header("Serialized Components")]
        public Transform Transform;
        public Camera Camera;

        [Header("Movement Value Datas")]
        public float MoveSpeed = 3f;
        public float MoveLerpPower = 7.5f;
        public float LookSpeed = 500f;

        [Header("Behaviour Value Datas")]
        public float SlowTickRate = 0.2f;
        [MinMaxSlider(0f, 2f)]
        public Vector2 AttackRange = new Vector2(1f, 1.25f); //min => start attack, max => attackable range
        public float AttackDamage = 30f;
        public float AttackSpeed = 1f;

        [Header("Animator Value Datas")]
        public float NormalLerpPower = 5f;
    }
}