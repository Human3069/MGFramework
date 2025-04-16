using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class FindWorkEmployeeState : IEmployeeState
    {
        private Employee _employee;

        public void Enter(Employee employee)
        {
            this._employee = employee;
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
            if (_employee.TargetHarvestable == null)
            {
                _employee.TargetHarvestable = _employee.FindNearest<Harvestable>(FindNearestPredicate);
                bool FindNearestPredicate(Harvestable harvestable)
                {
                    return harvestable._Damageable.IsEntered == false &&
                           harvestable._Damageable.IsDead == false;
                }
            }
            else
            {
                _employee.State = EmployeeState.MoveToWork;
            }
        }
    }
}
