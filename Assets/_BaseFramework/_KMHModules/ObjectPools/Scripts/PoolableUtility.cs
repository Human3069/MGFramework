using _KMH_Framework.Pool;
using System;
using UnityEngine;

namespace _KMH_Framework
{
    public static class PoolableUtility
    {
        public static GameObject EnablePool(this PoolType type, Action<GameObject> onBeforeEnableAction = null)
        {
            GameObject enabledObj = PoolableManager.Instance.EnablePool(type, onBeforeEnableAction);
            return enabledObj;
        }

        public static void DisablePool(this GameObject obj, PoolType type)
        {
            PoolableManager.Instance.DisablePool(type, obj);
        }
    }
}