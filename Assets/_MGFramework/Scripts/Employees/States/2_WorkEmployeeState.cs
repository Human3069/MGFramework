using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class WorkEmployeeState : IEmployeeState
    {
        private Employee _employee;

        public void Enter(Employee employee)
        {
            this._employee = employee;
            this._employee.PlayWorkingAnimation(true);
        }

        public void Exit()
        {
            this._employee.PlayWorkingAnimation(false);
            this._employee.TargetHarvestable = null;
        }

        public void Tick()
        {

        }

        public void FixedTick()
        {
        
        }

        public void SlowTick()
        {
            if (_employee.TargetHarvestable._Damageable.IsDead == true)
            {
                _employee.State = EmployeeState.PickUpItems;
            }
        }
    }
}
