using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class PickUpItemsEmployeeState : IEmployeeState
    {
        private const float PICKUP_DISTANCE = 10f;

        private EmployeeContext _context;
        private EmployeeData _data;

        private List<Item> targetItemList;
    
        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            // �ֺ� �������� ���ٸ� FindWork ���� ����
            bool isFoundNearItems = TryFindNearItemList(out targetItemList);
            if (isFoundNearItems == false)
            {
                _context.StateMachine.ChangeState(new FindWorkEmployeeState());
            }
        }

        /// <summary>
        /// PICKUP_DISTANCE �ݰ� �� ������ ������Ʈ���� ã�� �ʵ忡 ����.
        /// </summary>
        /// <returns>��ó�� �������� ������ false, �ϳ� �̻� ������ true</returns>
        private bool TryFindNearItemList(out List<Item> itemList)
        {
            int layerMask = ~(1 << 3);
            Collider[] overlapColliders = Physics.OverlapSphere(_context.Transform.position, PICKUP_DISTANCE, layerMask);
            itemList = new List<Item>();

            foreach (Collider overlapCollider in overlapColliders)
            {
                if (overlapCollider.TryGetComponent(out Item item) == true)
                {
                    itemList.Add(item);
                }
            }

            itemList.Sort(SortComparison);
            int SortComparison(Item a, Item b)
            {
                float distanceA = Vector3.Distance(_context.Transform.position, a.transform.position);
                float distanceB = Vector3.Distance(_context.Transform.position, b.transform.position);
                return distanceA.CompareTo(distanceB);
            }

            return itemList.Count > 0;
        }

        public void Exit()
        {   

        }

        public void SlowTick()
        {
            if (targetItemList.Count == 0)
            {
                _context.StateMachine.ChangeState(new FindInputStorageEmployeeState());
            }
            else
            {
                Item targetItem = targetItemList[0];
                _context.Agent.SetDestination(targetItem.transform.position);

                if (_context.Agent.IsArrived() == true ||
                    targetItem.gameObject.activeSelf == false)
                {
                    targetItemList.Remove(targetItem);
                }
            }
        }
    }
}
