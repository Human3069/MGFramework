using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// StackableStore�� Stackable�� ���� ���·� �����ϴ� Ŭ�����Դϴ�.
    /// �ٽ� �Լ��� Push, Pop�� ������, �������� ���·� ������ Stack�Դϴ�.
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

        public int TotalCount
        {
            get
            {
                return stackableList.Count;
            }
        }

        private List<Inventory> enteringInventoryList = new List<Inventory>();
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
            if (collider.TryGetComponent(out Inventory inventory) == true)
            {
                enteringInventoryList.Add(inventory);
                EnteredInventory = enteringInventoryList[0];
            }
        }

        protected void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out Inventory inventory) == true)
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

        /// <summary>
        /// StackableStore�� ���� PoolType�� �����ϴ�.
        /// </summary>
        /// <returns>PoolType</returns>
        public PoolType GetPoolType()
        {
            if (poolType == PoolType.None)
            {
                Debug.LogError("None�� �� �����ϴ�.");
            }

            return poolType;
        }

        public void Add()
        {
            Push(poolType);
        }

        /// <summary>
        /// �������� ���ÿ� �߰��մϴ�.
        /// </summary>
        /// <param name="item"></param>
        public void Push(Item item)
        {
            // ���� Item Ÿ�� Ǯ�� ��ȯ
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
            // Stackable Ÿ���̸� ġȯ�� �ʿ� ����.
            PoolType stackablePoolType = itemPoolType;
            if (itemPoolType.GetPoolCategory() == PoolCategory.Item)
            {
                stackablePoolType = itemPoolType.ToStackableType();
            }

            // ���ο� Stackable Ÿ�� Ǯ�� Ȱ��ȭ
            GameObject stackableObj = stackablePoolType.EnablePool();
            Stackable stackable = stackableObj.GetComponent<Stackable>();

            // Stackable Transform ����
            int stackIndex = stackableList.Count;
            stackableList.Add(stackable);
            stackable.transform.SetParent(stackTransform);
            stackable.transform.localPosition = Vector3.up * stackHeight * stackIndex;
            stackable.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Stackable�� payCount��ŭ ���ÿ��� �����մϴ�.
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
        /// Pop�� ������ ��� true�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="type">���� Ÿ������ üũ</param>
        /// <param name="stackables">�˵� ������Ʈ���� �����մϴ�.</param>
        /// <returns>�� ����� �����մϴ� : ���� Ÿ���̸�, �� ������ ������ �� true, �� �ܿ� false</returns>
        public bool TryPop(PoolType type, out Stackable[] stackables)
        {
            bool isPoppable = IsPoppable(type);
            stackables = isPoppable ? Pop() : null;

            return isPoppable;
        }

        /// <summary>
        /// Pop�� ������ ��� true�� ��ȯ�մϴ�.
        /// </summary>
        public bool TryPop(out Stackable[] stackables)
        {
            bool isPoppable = IsPoppable();
            stackables = isPoppable ? Pop() : null;

            return isPoppable;
        }

        public bool IsPoppable(PoolType type)
        {
            bool isPoppable = IsPoppable();
            bool isTypePoppable = poolType == type;

            return isPoppable == true &&
                   isTypePoppable == true;
        }

        public bool IsPoppable()
        {
            return stackableList.Count >= payCount;
        }

        public bool HasOutput()
        {
            return stackableList.Count > 0;
        }
    }
}
