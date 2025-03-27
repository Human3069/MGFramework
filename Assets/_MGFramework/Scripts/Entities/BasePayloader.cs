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
        protected SerializedDictionary<ItemType, int> inputDic = new SerializedDictionary<ItemType, int>();
        
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
                if (TryInput() == true)
                {
                    OnInput();
                }

                await UniTask.WaitForSeconds(inputDelay);
            }
        }

        protected virtual bool TryInput()
        {
            PlayerInventory inventory = PlayerController.Instance.Inventory;
            Vector3? suctionPoint = isSuction == true ? this.transform.position : null;

            foreach (KeyValuePair<ItemType, int> pair in inputDic)
            {
                if (inventory.TryPop(pair.Key, suctionPoint) == true)
                {
                    inputDic[pair.Key]++;
                    return true;
                }
            }

            return false;
        }

        protected abstract void OnInput();

        private void Awake()
        {
            for (int i = 0; i < inputDic.Count; i++)
            {
                if (inputDic.ContainsKey((ItemType)i) == true)
                {
                    inputDic[(ItemType)i] = 0;
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
}