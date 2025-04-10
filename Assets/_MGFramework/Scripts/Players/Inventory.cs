using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class Inventory 
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

        public bool IsInventoryNull
        {
            get
            {
                return itemList.Count == 0;
            }
        }

        public int ItemCount
        {
            get
            {
                return itemList.Count;
            }
        }

        public bool TryPush(PoolType type)
        {
            // �κ��丮�� ����
            if (itemList.Count < maxItemCount)
            {
                type.EnablePool(OnBeforeEnable);
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

                ValidateItemList();
                return true;
            }
            else
            {
                ValidateItemList();
                return false;
            }
        }

        public bool TryPush(Item item)
        {
            // �κ��丮�� ����
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

                ValidateItemList();
                return true;
            }
            else
            {
                ValidateItemList();
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

            ValidateItemList();
            return item != null;
        }

        private void ValidateItemList()
        {
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i].IsOnInventory == false ||
                    itemList[i].gameObject.activeSelf == false)
                {
                    // �κ��丮�� ������ ����� �� ���� ���¸� ����
                    itemList.RemoveAt(i);
                }
                else
                {
                    // ���� �缳�� (����Ʈ ���� ���� ���������� �̵��ϰ� ��.)
                    itemList[i].transform.SetSiblingIndex(i);
                }
            }

            // ��ġ �缳��
            for (int i = 0; i < itemList.Count; i++)
            {
                Vector3 offset = inventoryT.up * heightOffset * i;
                itemList[i].transform.position = inventoryT.position + offset;
            }
        }
    }
}