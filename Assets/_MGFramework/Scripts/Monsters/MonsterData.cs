using _KMH_Framework.Pool;
using System;
using UnityEngine;

namespace MGFramework
{
    [Serializable]
    public class MonsterData
    {
        [Header("State Checker")]
        public float MaxAlertRange = 15f;
        public float MaxMoveToAttackRange = 7.5f;
        public float MaxAttackRange = 2f;

        [Header("In Multiple State")]
        public float LookAtSpeed = 1f;

        [Header("In Attack State")]
        public float AttackSpeed = 1f;
        public float AttackDamage = 10f;

        [Header("OnDead")]
        public PoolType DropItemType = PoolType.Item_RawMeat;
        public Vector2Int DropItemCountRange = new Vector2Int(3, 5);
        public Vector3 DropItemOffset;
        public float DropItemRadius = 1f;

#if UNITY_EDITOR
        [Space(10)]
        public bool IsShowLog = true;
#endif
    }
}