using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    public enum PoolType
    {
        None = -1,

        Wood = 0,
        RawMeat = 1,
        CookedMeat = 2,

        PlayerMarker = 100,
    }

    [System.Serializable]
    public class Poolable // 데이터 시트로만 관리하기 위해, Queue를 생성/관리하지 않음.
    {
        [SerializeField]
        internal PoolType type;
        [SerializeField]
        internal GameObject prefab;

        [Space(10)]
        [SerializeField]
        internal int initCount = 10;
        [SerializeField]
        internal int maxCount = 100;
    }
}