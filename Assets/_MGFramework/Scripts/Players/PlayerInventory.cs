using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerInventory 
    {
        [ReadOnly]
        [SerializeField]
        private List<Item> itemList = new List<Item>();

        [Space(10)]
        [SerializeField]
        private Transform inventoryT;
        [SerializeField]
        private float heightOffset = 1f;
        [SerializeField]
        private int maxItemCount = 20;

        public bool TryPush(Item item)
        {
            PoolType itemPoolType = item._Type;
            if (itemList.Count < maxItemCount)
            {
                item.DisableAsync().Forget();
                itemPoolType.EnablePool(OnBeforeEnable);
                void OnBeforeEnable(GameObject poolObj)
                {
                    Item poolItem = poolObj.GetComponent<Item>();
                    poolItem.IsOnInventory = true;

                    Vector3 offset = inventoryT.up * heightOffset * itemList.Count;
                    poolObj.transform.parent = inventoryT;
                    poolObj.transform.localScale = Vector3.one;
                    poolObj.transform.position = inventoryT.position + offset;
                    poolObj.transform.localEulerAngles = Vector3.zero;

                    itemList.Add(poolItem);
                }
              
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryPop(PoolType type, Vector3? suctionPoint)
        {
            Item item = itemList.FindLast(x => x._Type == type && x.IsOnInventory == true);
            if (item != null)
            {
                item.IsOnInventory = false;
                itemList.Remove(item);

                item.DisableAsync(suctionPoint).Forget();
            }

            return item != null;
        }
    }
}