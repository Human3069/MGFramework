using _KMH_Framework;
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

namespace MGFramework
{
    public static class PoolableUtility
    {
        public static void EnableTimer(this ITimer timer, float yOffset)
        {
            PoolableManager.Instance.EnablePool(PoolType.UI_Timer, onBeforeEnableAction);
            void onBeforeEnableAction(GameObject timerObj)
            {
                UI_Timer uiTimer = timerObj.GetComponent<UI_Timer>();
                uiTimer.Initialize(timer, yOffset);

                timerObj.transform.SetParent(UI_Main.Instance.TimerParent);
            }
        }

        public static void EnableHealthbar(this Damageable damageable, float yOffset)
        {
            PoolableManager.Instance.EnablePool(PoolType.UI_Healthbar, onBeforeEnableAction);
            void onBeforeEnableAction(GameObject healthbarObj)
            {
                UI_Healthbar uiHealthbar = healthbarObj.GetComponent<UI_Healthbar>();
                uiHealthbar.Initialize(damageable, yOffset);

                healthbarObj.transform.SetParent(UI_Main.Instance.HealthbarParent);
            }
        }

        public static void DisablePool(this Item item)
        {
            PoolableManager.Instance.DisablePool(item.ItemPoolType, item.gameObject);
        }

        public static void DisablePool(this Stackable stackable)
        {
            PoolableManager.Instance.DisablePool(stackable.StackablePoolType, stackable.gameObject);
        }
    }
}