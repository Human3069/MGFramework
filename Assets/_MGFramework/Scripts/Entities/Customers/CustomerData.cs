using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public class CustomerData
    {
        public Customer _Customer;
        public NavMeshAgent _Agent;
        public Animator _Animator;
        public Transform _Transform;

        [Space(10)]
        public float _DirectionSpeed = 1f;
    }
}