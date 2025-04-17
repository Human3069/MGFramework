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
    public class Poolable // ������ ��Ʈ�θ� �����ϱ� ����, Queue�� ����/�������� ����.
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