using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
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
            ItemType itemType = item._ItemType;
            if (itemList.Count < maxItemCount)
            {
                item.DisableAsync().Forget();
                Item enabledItem = itemType.EnablePool<Item>(OnBeforeEnable);
                void OnBeforeEnable(Item pooledItem)
                {
                    pooledItem.IsOnInventory = true;

                    Vector3 offset = inventoryT.up * heightOffset * itemList.Count;
                    pooledItem.transform.parent = inventoryT;
                    pooledItem.transform.localScale = Vector3.one;
                    pooledItem.transform.position = inventoryT.position + offset;
                    pooledItem.transform.localEulerAngles = Vector3.zero;
                }
                itemList.Add(enabledItem);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryPop(ItemType type, Vector3? suctionPoint)
        {
            Item item = itemList.FindLast(x => x._ItemType == type && x.IsOnInventory == true);
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