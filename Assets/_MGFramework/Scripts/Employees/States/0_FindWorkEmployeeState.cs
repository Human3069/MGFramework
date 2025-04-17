using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class FindWorkEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;
        }

        public void Exit()
        {   

        }

        public void SlowTick()
        {
            if (_context.TargetHarvestable == null)
            {
                _context.TargetHarvestable = _context.Transform.FindNearest<Harvestable>(FindNearestPredicate);
                bool FindNearestPredicate(Harvestable harvestable)
                {
                    return harvestable._Damageable.IsEntered == false &&
                           harvestable._Damageable.IsDead == false;
                }
            }
            else
            {
                _context.StateMachine.ChangeState(new MoveToWorkEmployeeState());
            }
        }
    }
}
