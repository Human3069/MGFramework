using UnityEngine;

namespace MGFramework
{
    public class MoveToHuntHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;

        private Vector3 destination;

        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;

            Damageable targetDamageable = _context.TargetDamageable;
            destination = targetDamageable.GetClosestPoint(_context.Transform.position);
            _context.MoveToTargetForward(_data.AttackRange);
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
            if (_context.IsInAttackRange(_data, destination) == true)
            {
                _context.StateMachine.ChangeState(new HuntHunterState());
            }
            else
            {
                Damageable targetDamageable = _context.TargetDamageable;
                destination = targetDamageable.GetClosestPoint(_context.Transform.position);
                _context.MoveToTargetForward(_data.AttackRange);
            }
        }
    }
}