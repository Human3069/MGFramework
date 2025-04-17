using UnityEngine;

namespace MGFramework
{
    public class MoveToWorkEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        private Vector3 destination;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            Damageable targetDamageable = _context.TargetHarvestable._Damageable;
            destination = targetDamageable.GetClosestPoint(_context.Transform.position);
            
            _context.Agent.SetDestination(destination);
        }

        public void Exit()
        {   

        }

        public void SlowTick()
        {
            if (_context.IsInAttackRange(_data, destination) == true)
            {
                _context.StateMachine.ChangeState(new WorkEmployeeState());
            }
        }
    }
}
