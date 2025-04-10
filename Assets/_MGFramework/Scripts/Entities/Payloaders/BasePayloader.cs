using _KMH_Framework.Pool;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BasePayloader : MonoBehaviour
    {
        [Header("=== BasePayloader ===")]
        [SerializeField]
        protected SerializedDictionary<PoolType, ItemData> inputDic = new SerializedDictionary<PoolType, ItemData>();
        [SerializeField]
        protected SerializedDictionary<PoolType, ItemData> outputDic = new SerializedDictionary<PoolType, ItemData>();

        [Space(10)]
        [SerializeField]
        private float inputDelay = 0.5f;
        [SerializeField]
        private bool isSuction = true;

        public int TotalInputCount
        {
            get
            {
                int totalCount = 0;
                foreach (KeyValuePair<PoolType, ItemData> pair in inputDic)
                {
                    totalCount += pair.Value.Count;
                }

                return totalCount;
            }
        }

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
                if (TryInput(out PoolType inputItem) == true)
                {
                    OnInput(inputItem);
                }

                await UniTask.WaitForSeconds(inputDelay);
            }
        }

        protected virtual bool TryInput(out PoolType inputPoolType)
        {
            Inventory inventory = OpenedInventory.Inventory;
            Vector3? suctionPoint = isSuction == true ? this.transform.position : null;

            foreach (KeyValuePair<PoolType, ItemData> pair in inputDic)
            {
                if (inventory.TryPop(pair.Key, suctionPoint) == true)
                {
                    inputDic[pair.Key].Count++;

                    inputPoolType = pair.Key;
                    return true;
                }
            }

            inputPoolType = PoolType.None;
            return false;
        }

        protected abstract void OnInput(PoolType inputItem);

        private void Awake()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                if (inputDic.ContainsKey((PoolType)i) == true)
                {
                    inputDic[(PoolType)i].Count = 0;
                }
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

    [System.Serializable]
    public class ItemData
    {
        public int Count;
        public Transform StoreT;
    }
}