using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class PickUpItemsEmployeeState : IEmployeeState
    {
        private const float PICKUP_DISTANCE = 10f;
        private Employee _employee;

        private List<Item> targetItemList = new List<Item>();

        public void Enter(Employee employee)
        {
            this._employee = employee;

            Collider[] overlapColliders = Physics.OverlapSphere(_employee.transform.position, PICKUP_DISTANCE);
            foreach (Collider overlapCollider in overlapColliders)
            {
                if (overlapCollider.TryGetComponent(out Item item) == true)
                {
                    targetItemList.Add(item);
                }
            }

            targetItemList.Sort(SortComparison);
            int SortComparison(Item a, Item b)
            {
                float distanceA = Vector3.Distance(_employee.transform.position, a.transform.position);
                float distanceB = Vector3.Distance(_employee.transform.position, b.transform.position);
                return distanceA.CompareTo(distanceB);
            }
        }

        public void Exit()
        {   

        }

        public void Tick()
        {

        }

        public void FixedTick()
        {
      
        }

        public void SlowTick()
        {
            if (targetItemList.Count == 0)
            {
                _employee.State = EmployeeState.FindStorage;
            }
            else
            {
                Item targetItem = targetItemList[0];
                _employee.SetDestination(targetItem.transform.position);

                float distance = Vector3.Distance(_employee.transform.position, targetItem.transform.position);
                float stoppingDistance = _employee.GetStoppingDistance();

                if (distance <= stoppingDistance ||
                    targetItem.gameObject.activeSelf == false)
                {
                    targetItemList.Remove(targetItem);
                }
            }
        }
    }
}
