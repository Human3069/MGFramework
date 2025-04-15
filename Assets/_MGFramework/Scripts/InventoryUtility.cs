using _KMH_Framework.Pool;
using UnityEngine;

namespace MGFramework
{
    public enum PoolCategory
    {
        None = -1,

        Item = 0,
        Stackable = 1,
    }

    public static class InventoryUtility 
    {
        public static PoolType ToItemType(this PoolType stackablePoolType)
        {
            string stackablePoolTypeName = stackablePoolType.ToString();
            if (stackablePoolTypeName.Contains("Stackable") == false)
            {
                Debug.LogError("StackPoolType enum �ʵ忡 Stackable �ܾ ���ԵǾ� ���� �ʽ��ϴ�.");
                return PoolType.None;
            }

            string itemPoolTypeName = stackablePoolTypeName.Replace("Stackable", "Item");
            PoolType itemPoolType = (PoolType)System.Enum.Parse(typeof(PoolType), itemPoolTypeName);

            return itemPoolType;
        }

        public static PoolType ToItemType(this Stackable stackable)
        {
            if (stackable == null)
            {
                Debug.LogError("Stackable ������Ʈ�� null�Դϴ�.");
                return PoolType.None;
            }

            PoolType itemPoolType = stackable.StackablePoolType.ToItemType();
            return itemPoolType;
        }

        public static PoolType ToStackableType(this PoolType itemPoolType)
        {
            string itemPoolTypeName = itemPoolType.ToString();
            if (itemPoolTypeName.Contains("Item") == false)
            {
                Debug.LogError("Item ������Ʈ enum �ʵ� ItemPoolType�� Item �ܾ ���ԵǾ� ���� �ʽ��ϴ�.");
                return PoolType.None;
            }

            string stackablePoolTypeName = itemPoolTypeName.Replace("Item", "Stackable");
            PoolType stackablePoolType = (PoolType)System.Enum.Parse(typeof(PoolType), stackablePoolTypeName);

            return stackablePoolType;
        }

        public static PoolType ToStackableType(this Item item)
        {
            if (item == null)
            {
                Debug.LogError("Stackable ������Ʈ�� null�Դϴ�.");
                return PoolType.None;
            }

            PoolType stackablePoolType = item.ItemPoolType.ToStackableType();
            return stackablePoolType;
        }

        public static PoolCategory GetPoolCategory(this PoolType poolType)
        {
            string poolTypeName = poolType.ToString();
            if (poolTypeName.Contains("Item"))
            {
                return PoolCategory.Item;
            }
            else if (poolTypeName.Contains("Stackable"))
            {
                return PoolCategory.Stackable;
            }
            else
            {
                Debug.LogError("�� �� ���� ī�װ� : " + poolType);
                return PoolCategory.None;
            }
        }
    }
}
