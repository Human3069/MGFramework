using System;
using UnityEngine;

namespace MGFramework
{
    [Serializable]
    public class HunterData
    {
        public float LookAtSpeed = 5f;

        [Space(10)]
        public float AttackDamage = 30f;
        public float AttackSpeed = 1f;
        public float AttackRange = 1f;

#if UNITY_EDITOR
        [Space(10)]
        public bool IsShowLog = true;
        #endif
    }
}