using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public class EmployerData 
    {
        public Employer _Employer;
        public NavMeshAgent _Agent;
        public Animator _Animator;
        public Transform _Transform;
        [HideInInspector]
        public Inventory _Inventory;

        [Space(10)]
        public float _HandlingDistance = 1f;
        public float _HandlingSpeed = 1f;
        public float _HandlingDamage = 10;
    }
}