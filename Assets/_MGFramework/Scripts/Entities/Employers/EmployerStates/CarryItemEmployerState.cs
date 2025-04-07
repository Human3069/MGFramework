using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class CarryItemEmployerState : IEmployerState
    {
        private EmployerData _data;
        private List<Item> targetItemList;

        private Collider[] overlappingColliders;

        public CarryItemEmployerState(EmployerData data)
        {
            this._data = data;
            this.targetItemList = new List<Item>();

            this.overlappingColliders = new Collider[5];
        }

        public void Enter()
        {
            // �� �ֺ��� �ֿ� �� �ִ� ������ ã��.
            Vector3 employerPos = _data._Transform.position;
            Collider[] overlappedColliders = Physics.OverlapSphere(employerPos, 10f);

            foreach (Collider overlappedCollider in overlappedColliders)
            {
                if (overlappedCollider.TryGetComponent(out Item foundItem) == true &&
                    foundItem.IsOnInventory == false &&
                    foundItem.IsFading == false)
                {
                    // ã�ƾ� �� ����Ʈ�� �߰�
                    targetItemList.Add(foundItem);
                }
            }

            // �Ÿ����� ����
            targetItemList.Sort(SortComparison);
            int SortComparison(Item item1, Item item2)
            {
                float distance1 = Vector3.Distance(employerPos, item1.transform.position);
                float distance2 = Vector3.Distance(employerPos, item2.transform.position);

                if (distance1 < distance2)
                {
                    return -1;
                }
                else if (distance1 > distance2)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            if (targetItemList.Count == 0f)
            {
                // ã�� �������� ���ų�, ��� �ֿ��� ��� ���� ���·� ��ȯ
                _data._Employer._EmployerState = EmployerState.DropItem;
            }
            else
            {
                Item targetItem = targetItemList[0];
                if (targetItem.IsOnInventory == true ||
                    targetItem.gameObject.activeSelf == false)
                {
                    targetItemList.Remove(targetItem);
                }
                else
                {
                    _data._Agent.destination = targetItem.transform.position;
                }
            }

            // OnEnter ȣ���� �ȵǴ� ��� ���� ȣ��
            int count = Physics.OverlapSphereNonAlloc(_data._Transform.position, 1f, overlappingColliders);
            for (int i = 0; i < count; i++)
            {
                Collider overlappingCollider = overlappingColliders[i];

                if (overlappingCollider.TryGetComponent(out Item item) == true &&
                    item.IsOnInventory == false &&
                    item.IsFading == false)
                {
                    // Ÿ�� ��Ͽ��� ������ �κ��丮�� ���� �� ���� ��Ȳ�� �� Ÿ�ٿ��� ����
                    if (_data._Inventory.TryPush(item) == false &&
                        targetItemList.Contains(item) == true)
                    {
                        targetItemList.Remove(item);
                    }
                }
            }
        }

        public void FixedTick()
        {

        }
    }
}