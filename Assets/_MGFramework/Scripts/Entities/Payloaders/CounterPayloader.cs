using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CounterPayloader : BasePayloader
    {
        [Header("=== CounterPayloader ===")]
        [SerializeField]
        private float storingHeight = 0.5f;

        private Dictionary<PoolType, List<Item>> itemInstanceDic = new Dictionary<PoolType, List<Item>>();

        protected override void OnInput(PoolType type)
        {
            AddStore(type);
        }

        private void AddStore(PoolType type)
        {
            int itemIndex = inputDic[type].Count - 1;
            if (itemInstanceDic.ContainsKey(type) == false)
            {
                itemInstanceDic.Add(type, new List<Item>());
            }

            ItemData data = inputDic[type];
            Transform storeT = data.StoreT;

            type.EnablePool(OnBeforeEnable);
            void OnBeforeEnable(GameObject poolObj)
            {
                Item poolItem = poolObj.GetComponent<Item>();
                poolItem.IsOnInventory = true;

                Vector3 pos = storeT.position + (storeT.up * storingHeight * itemIndex);
                poolObj.transform.parent = storeT;
                poolObj.transform.position = pos;
                poolObj.transform.localScale = Vector3.one;
                poolObj.transform.localEulerAngles = Vector3.zero;

                itemInstanceDic[type].Add(poolItem);
            }
        }
    }
}