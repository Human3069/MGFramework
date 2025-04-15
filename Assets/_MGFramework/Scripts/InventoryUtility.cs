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
                Debug.LogError("StackPoolType enum 필드에 Stackable 단어가 포함되어 있지 않습니다.");
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
                Debug.LogError("Stackable 컴포넌트가 null입니다.");
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
                Debug.LogError("Item 컴포넌트 enum 필드 ItemPoolType에 Item 단어가 포함되어 있지 않습니다.");
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
                Debug.LogError("Stackable 컴포넌트가 null입니다.");
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
                Debug.LogError("알 수 없는 카테고리 : " + poolType);
                return PoolCategory.None;
            }
        }
    }
}
