using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CampfireOutputPayloader : MonoBehaviour
    {
        [SerializeField]
        private CampfirePayloader campfirePayloader;
        [SerializeField]
        private float inputDelay;

        private IInventory _openedInventory = null;
        protected IInventory OpenedInventory
        {
            get
            {
                return _openedInventory;
            }
            private set
            {
                if (_openedInventory != value)
                {
                    _openedInventory = value;
                    if (value != null)
                    {
                        OpenedAsync().Forget();
                    }
                }
            }
        }

        private async UniTaskVoid OpenedAsync()
        {
            while (OpenedInventory != null)
            {
                int meatCount = campfirePayloader.GetCurrentOutputCount(PoolType.CookedMeat);
                if (meatCount > 0 &&
                    OpenedInventory.Inventory.TryPush(PoolType.CookedMeat) == true)
                {
                    campfirePayloader.DecreaseOutput(PoolType.CookedMeat);
                    campfirePayloader.DisableLastItem(PoolType.CookedMeat);
                }

                await UniTask.WaitForSeconds(inputDelay);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out IInventory inventory) == true)
            {
                OpenedInventory = inventory;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out IInventory inventory) == true)
            {
                if (OpenedInventory == inventory)
                {
                    OpenedInventory = null;
                }
            }
        }
    }
}