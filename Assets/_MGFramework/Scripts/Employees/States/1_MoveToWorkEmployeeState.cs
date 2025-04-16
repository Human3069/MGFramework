using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class MoveToWorkEmployeeState : IEmployeeState
    {
        private Employee _employee;

        private Vector3 destination;

        public void Enter(Employee employee)
        {
            this._employee = employee;

            Damageable targetDamageable = _employee.TargetHarvestable._Damageable;
            destination = targetDamageable.GetClosestPoint(_employee.transform.position);
            _employee.SetDestination(destination);
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
            float distance = Vector3.Distance(_employee.transform.position, destination);
            if (distance <= _employee.AttackRange)
            {
                _employee.State = EmployeeState.Work;
            }
        }
    }
}
