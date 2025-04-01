using _KMH_Framework.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public struct MonsterData
    {
        public Monster _Monster;
        public NavMeshAgent _Agent;
        public Animator _Animator;
        public Transform _MonsterT;

        [Space(10)]
        public float _MinSleepRange; // = 15f;
        public float _MinAlertRange; // = 7.5f;
        [Space(10)]
        public float _AttackDelay; // = 1f;
        public int _AttackAnimeCount; // = 4;

        [Space(10)]
        public float _AttackDamage; // = 10f;
        public float _AttackRange; // = 2.5f;
        public float _AttackOverlapRange; // = 3.5f;

        [Space(10)]
        public float _TowardSpeed; // = 4f;

        [Space(10)]
        public PoolType _OutputType;
        public Vector2Int _OutputCountRange;
        public Vector3 _SpawnOffset;
        public float _SpawnRadius;
        public float _SpawnLinearForce;
        public float _SpawnAngularForce;
    }
}