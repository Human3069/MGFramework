using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MGFramework
{
    public abstract class BaseFootstep : MonoBehaviour
    {
        private List<Inventory> enteringInventoryList = new List<Inventory>();

        [Header("=== BaseFootstep ===")]
        [SerializeField]
        protected Image progressImage;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private Inventory _enteredInventory;
        protected Inventory EnteredInventory
        {
            get
            {
                return _enteredInventory;
            }
            set
            {
                _enteredInventory = value;

                if (value != null)
                {
                    tokenSoure?.Cancel();
                    tokenSoure = new CancellationTokenSource();

                    OnInventoryEnteredAsync(tokenSoure.Token).Forget();
                }
            }
        }

        protected abstract UniTaskVoid OnInventoryEnteredAsync(CancellationToken token);

        private CancellationTokenSource tokenSoure;

        public bool IsOccupied
        {
            get
            {
                return EnteredInventory != null;
            }
        }

        protected virtual void Awake()
        {
            progressImage.fillAmount = 0f;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Inventory inventory) == true &&
                collider.transform == Player.Instance.transform)
            {
                enteringInventoryList.Add(inventory);
                EnteredInventory = enteringInventoryList[0];
            }
        }

        protected void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out Inventory inventory) == true &&
                collider.transform == Player.Instance.transform)
            {
                enteringInventoryList.Remove(inventory);
                if (enteringInventoryList.Count == 0)
                {
                    EnteredInventory = null;
                }
                else
                {
                    EnteredInventory = enteringInventoryList[0];
                }
            }
        }
    }
}
