using _KMH_Framework.Pool;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using geniikw.DataRenderer2D.Hole;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
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

        private bool _isOn = false;
        protected bool IsOn
        {
            get
            {
                return _isOn;
            }
            private set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                    if (value == true)
                    {
                        OnAsync().Forget();
                    }
                }
            }
        }

        private async UniTaskVoid OnAsync()
        {
            while (IsOn == true)
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
            PlayerInventory inventory = PlayerController.Instance.Inventory;
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
            if (collider.TryGetComponent<PlayerController>(out _) == true)
            {
                IsOn = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<PlayerController>(out _) == true)
            {
                IsOn = false;
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