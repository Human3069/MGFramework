using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public enum ItemType
    {
        Tree
    }

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        protected Transform stackBagT;
        [SerializeField]
        protected float stackDistance = 0.5f;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        [SerializedDictionary("ItemType", "Count")]
        protected SerializedDictionary<ItemType, int> itemDic = new SerializedDictionary<ItemType, int>();

        protected virtual void Awake()
        {
            int enumLength = Enum.GetValues(typeof(ItemType)).Length;
            for (int i = 0; i < enumLength; i++)
            {
                ItemType type = (ItemType)i;
                itemDic.Add(type, 0);
            }
        }

        public virtual void Gained(ItemType type)
        {
            int currentCount = itemDic[type];
            itemDic[type]++;

            GameObject instance = ObjectPoolManager.Instance.TakeOutObj(PoolerType.Stackable_Tree);
            instance.transform.parent = stackBagT;
            instance.transform.localPosition = new Vector3(0f, currentCount * stackDistance, 0f);
            instance.transform.forward = stackBagT.forward;
        }
    }
}