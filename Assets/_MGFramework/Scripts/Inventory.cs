using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class Inventory : MonoBehaviour
    {
        private const float SLOW_TICK_DURATION = 0.2f;
        private const float MAGNETIC_RADIUS = 1f;

        [SerializeField]
        private float stackHeight = 0.3f;

        [Space(10)]
        [SerializeField]
        private Transform stackTransform;
        [SerializeField]
        private List<Stackable> stackableList = new List<Stackable>();

        private Collider[] overlapColliders = new Collider[10];

        public List<PoolType> GetPoolTypeList()
        {
            List<PoolType> poolTypeList = new List<PoolType>();
            foreach (Stackable stackable in stackableList)
            {
                PoolType poolType = stackable.StackablePoolType;
                if (poolTypeList.Contains(poolType) == false)
                {
                    poolTypeList.Add(poolType);
                }
            }

            return poolTypeList;
        }

        private void OnEnable()
        {
            OnEnableAsync().Forget();
        }

        private async UniTaskVoid OnEnableAsync()
        {
            while (this.enabled == true)
            {
                List<Item> itemList = FindItemList();
                foreach (Item item in itemList)
                {
                    StackItem(item);
                }

                await UniTask.WaitForSeconds(SLOW_TICK_DURATION);
            }
        }

        private List<Item> FindItemList()
        {
            int layerMask = ~(1 << 3);
            int overlapCount = Physics.OverlapSphereNonAlloc(this.transform.position, MAGNETIC_RADIUS, overlapColliders, layerMask);
            List<Item> itemList = new List<Item>();

            for (int i = 0; i < overlapCount; i++)
            {
                Collider overlapCollider = overlapColliders[i];
                if (overlapCollider.TryGetComponent(out Item item) == true)
                {
                    itemList.Add(item);
                }
            }

            return itemList;
        }

        // Item => ���������� ����.
        // Stack => Item�� �Ȱ��� ��������, �׿��ִ� ����.
        private void StackItem(Item item)
        {
            // Item�� ���� ó��
            PoolType itemPoolType = item.ItemPoolType;
            item.gameObject.DisablePool(itemPoolType);

            // Stack�� ���� ó��
            PoolType stackPoolType = item.ToStackableType();
            Push(stackPoolType);
        }

        public void Push(PoolType stackPoolType)
        {
            GameObject stackObj = stackPoolType.EnablePool();
            Stackable stack = stackObj.GetComponent<Stackable>();

            int stackIndex = stackableList.Count;
            Vector3 stackLocalPos = Vector3.up * stackIndex * stackHeight;

            stackableList.Add(stack);

            stack.transform.parent = stackTransform;
            stack.transform.localPosition = stackLocalPos;
            stack.transform.localRotation = Quaternion.identity;

            ReformPosition();
        }

        public bool TryPop(PoolType poolType, out Stackable stackable)
        {
            stackable = stackableList.FindLast(x => x.StackablePoolType == poolType);
            if (stackable != null)
            {
                stackableList.Remove(stackable);
            }

            ReformPosition();
            return stackable != null;
        }

        public void Clear()
        {
            for (int i = stackableList.Count - 1; i >= 0; i--)
            {
                Stackable stackable = stackableList[i];
                stackable.gameObject.DisablePool(stackable.StackablePoolType);
                stackableList.RemoveAt(i);
            }

            ReformPosition();
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        private void ReformPosition()
        {
            for (int i = 0; i < stackableList.Count; i++)
            {
                Vector3 stackLocalPos = Vector3.up * i * stackHeight;
                stackableList[i].transform.localPosition = stackLocalPos;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, MAGNETIC_RADIUS);
        }
    }
}
