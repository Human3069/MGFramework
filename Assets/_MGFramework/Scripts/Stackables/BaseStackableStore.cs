using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// StackableStore는 Stackable을 스택 형태로 저장하는 클래스입니다.
    /// 핵심 함수는 Push, Pop이 있으며, 물리적인 형태로 구현된 Stack입니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class BaseStackableStore : MonoBehaviour
    {
        private Collider _collider;

        [SerializeField]
        protected PoolType poolType = PoolType.None;
        [SerializeField]
        protected Transform stackTransform;

        [Space(10)]
        [SerializeField]
        protected float speed = 0.5f;
        [SerializeField]
        protected int payCount = 1;
        [SerializeField]
        protected float stackHeight = 0.3f;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        protected List<Stackable> stackableList = new List<Stackable>();

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
                    OnInventoryEnteredAsync().Forget();
                }
            }
        }

        protected abstract UniTaskVoid OnInventoryEnteredAsync();

        public Vector3 GetClosestPoint(Vector3 point)
        {
            return _collider.ClosestPoint(point);
        }

        protected virtual void Awake()
        {
            _collider = this.GetComponent<Collider>();
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Inventory inventory) == true &&
                EnteredInventory == null)
            {
                EnteredInventory = inventory;
            }
        }

        protected void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out Inventory inventory) == true &&
                EnteredInventory == inventory)
            {
                EnteredInventory = null;
            }
        }

        /// <summary>
        /// StackableStore는 단일 PoolType을 가집니다.
        /// </summary>
        /// <returns>PoolType</returns>
        public PoolType GetPoolType()
        {
            if (poolType == PoolType.None)
            {
                Debug.LogError("None일 수 없습니다.");
            }

            return poolType;
        }

        public void Add()
        {
            Push(poolType);
        }

        /// <summary>
        /// 아이템을 스택에 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void Push(Item item)
        {
            // 기존 Item 타입 풀링 반환
            item.DisablePool();
            Push(item.ItemPoolType);
        }

        public void Push(Stackable stackable)
        {
            stackable.DisablePool();
            Push(stackable.StackablePoolType);
        }

        public void Push(PoolType itemPoolType)
        {
            // Stackable 타입이면 치환할 필요 없음.
            PoolType stackablePoolType = itemPoolType;
            if (itemPoolType.GetPoolCategory() == PoolCategory.Item)
            {
                stackablePoolType = itemPoolType.ToStackableType();
            }

            // 새로운 Stackable 타입 풀링 활성화
            GameObject stackableObj = stackablePoolType.EnablePool();
            Stackable stackable = stackableObj.GetComponent<Stackable>();

            // Stackable Transform 세팅
            int stackIndex = stackableList.Count;
            stackableList.Add(stackable);
            stackable.transform.SetParent(stackTransform);
            stackable.transform.localPosition = Vector3.up * stackHeight * stackIndex;
            stackable.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Stackable을 payCount만큼 스택에서 제거합니다.
        /// </summary>
        public Stackable[] Pop()
        {
            Stackable[] poppedStackables = new Stackable[payCount];

            int lastIndex = stackableList.Count - 1;
            int minimumIndex = Mathf.Max(0, lastIndex - payCount + 1);

            int i = 0;
            for (int iRev = lastIndex; iRev >= minimumIndex; iRev--)
            {
                poppedStackables[i] = stackableList[iRev];
                stackableList.Remove(poppedStackables[i]);
                poppedStackables[i].gameObject.DisablePool(poolType);

                i++;
            }

            return poppedStackables;
        }

        /// <summary>
        /// Pop이 가능할 경우 true를 반환합니다.
        /// </summary>
        public bool TryPop(out Stackable[] stackables)
        {
            bool isPoppable = IsPoppable();
            stackables = isPoppable ? Pop() : null;

            return isPoppable;
        }

        public bool IsPoppable()
        {
            return stackableList.Count >= payCount;
        }
    }
}
