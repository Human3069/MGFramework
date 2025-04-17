using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class WorkEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            this._context.AnimationController.PlayWorking(true);
        }

        public void Exit()
        {
            this._context.AnimationController.PlayWorking(false);
            this._context.TargetHarvestable = null;
        }

        public void SlowTick()
        {
            if (_context.TargetHarvestable._Damageable.IsDead == true)
            {
                _context.StateMachine.ChangeState(new PickUpItemsEmployeeState());
            }
        }
    }
}
