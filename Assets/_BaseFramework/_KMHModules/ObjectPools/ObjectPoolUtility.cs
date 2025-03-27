using _KMH_Framework;
using System;
using UnityEngine;

namespace _KMH_Framework.Pool
{
    public static class ObjectPoolUtility
    {
        public static GameObject EnablePool(this ItemType type, Action<GameObject> beforeEnableAction = null)
        {
            GameObject pooledObj = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledObj;
        }

        public static GameObject EnablePool(this FxType type, Action<GameObject> beforeEnableAction = null)
        {
            GameObject pooledObj = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledObj;
        }

        public static GameObject EnablePool(this UnitType type, Action<GameObject> beforeEnableAction = null)
        {
            GameObject pooledObj = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledObj;
        }

        public static T EnablePool<T>(this ItemType type, Action<T> beforeEnableAction = null) where T : MonoBehaviour
        {
            T pooledComponent = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledComponent;
        }

        public static T EnablePool<T>(this FxType type, Action<T> beforeEnableAction = null) where T : MonoBehaviour
        {
            T pooledComponent = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledComponent;
        }

        public static T EnablePool<T>(this UnitType type, Action<T> beforeEnableAction = null) where T : MonoBehaviour
        {
            T pooledComponent = ObjectPoolManager.Instance.GetPoolHandler(type).EnableObj(beforeEnableAction);
            return pooledComponent;
        }

        public static void ReturnPool(this GameObject obj, ItemType type)
        {
            ObjectPoolManager.Instance.GetPoolHandler(type).DisableObj(obj);
        }

        public static void ReturnPool(this GameObject obj, FxType type)
        {
            ObjectPoolManager.Instance.GetPoolHandler(type).DisableObj(obj);
        }

        public static void ReturnPool(this GameObject obj, UnitType type)
        {
            ObjectPoolManager.Instance.GetPoolHandler(type).DisableObj(obj);
        }
    }
}