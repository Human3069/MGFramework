using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public enum ItemType
    {
        Tree,
        Meat
    }

    public static class MGHelper
    {
        public static PoolerType ToPoolerItemType(this ItemType type)
        {
            if (type == ItemType.Tree)
            {
                return PoolerType.Item_Tree;
            }
            else if (type == ItemType.Meat)
            {
                return PoolerType.Item_Meat;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static PoolerType ToPoolerStackableType(this ItemType type)
        {
            if (type == ItemType.Tree)
            {
                return PoolerType.Stackable_Tree;
            }
            else if (type == ItemType.Meat)
            {
                return PoolerType.Stackable_Meat;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
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

        public virtual void Push(ItemType type)
        {
            int currentCount = 0;
            foreach (var pair in itemDic)
            {
                currentCount += pair.Value;
            }

            itemDic[type]++;

            GameObject instance = ObjectPoolManager.Instance.TakeOutObj(type.ToPoolerStackableType()).gameObject;
            instance.transform.parent = stackBagT;
            instance.transform.localPosition = new Vector3(0f, currentCount * stackDistance, 0f);
            instance.transform.forward = stackBagT.forward;
        }

        public virtual bool TryPop(ItemType type, out GameObject obj)
        {
            if (itemDic[type] > 0)
            {
                for (int i = stackBagT.childCount - 1; i >= 0; i--)
                {
                    GameObject targetObj = stackBagT.GetChild(i).gameObject;
                    if (targetObj.name.Contains("_Popping") == false)
                    {
                        itemDic[type]--;

                        obj = targetObj;
                        return true;
                    }
                }

                obj = null;
                return false;
            }
            else
            {
                obj = null;
                return false;
            }
        }
    }
}