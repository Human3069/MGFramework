using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class PickUpItemsHunterState : IHunterState
    {
        private const float PICKUP_DISTANCE = 5f;

        private HunterContext _context;
        private HunterData _data;

        private List<Item> targetItemList = new List<Item>();

        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;

            // 죽은 동물 주변의 아이템 리스트에 저장
            Vector3 destination = context.TargetDamageable.transform.position;
            context.TargetDamageable = null;

            int layerMask = ~(1 << 3);
            Collider[] overlappedColliders = Physics.OverlapSphere(destination, PICKUP_DISTANCE, layerMask);
            foreach (Collider collider in overlappedColliders)
            {
                if (collider.TryGetComponent(out Item item) == true)
                {
                    targetItemList.Add(item);
                }
            }

            targetItemList.Sort(SortComparison);
            int SortComparison(Item a, Item b)
            {
                float distanceA = Vector3.Distance(_context.Transform.position, a.transform.position);
                float distanceB = Vector3.Distance(_context.Transform.position, b.transform.position);
                return distanceA.CompareTo(distanceB);
            }
        }

        public void Exit()
        {
            // 상태 종료 처리
        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
            if (targetItemList.Count == 0)
            {
                _context.StateMachine.ChangeState(new FindInputStorageHunterState());
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