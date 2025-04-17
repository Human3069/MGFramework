using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    public enum PoolType
    {
        None = -1,

        Item_Wood = 0,
        Item_RawMeat = 1,
        Item_CookedMeat = 2,

        Stackable_Wood = 50,
        Stackable_RawMeat = 51,
        Stackable_CookedMeat = 52,

        Customer = 100,
        Employee = 101,

        PlayerMarker = 200,

        UI_Timer = 300,
        UI_Healthbar = 301,
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