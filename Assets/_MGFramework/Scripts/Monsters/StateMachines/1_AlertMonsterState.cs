using UnityEngine;

namespace MGFramework
{
    public class AlertMonsterState : IMonsterState
    {
        private MonsterContext _context;
        private MonsterData _data;
        
        public void Enter(MonsterContext context, MonsterData data)
        {
            this._context = context;
            this._data = data;

            _context.AnimationController.PlayAlert();
        }
        
        public void Exit()
        {
            // 상태 종료 처리
        }

        public void FixedTick()
        {
            _context.LookTowardTarget(_data.LookAtSpeed);
        }

        public void SlowTick()
        {
            if (_context.TargetDamageable != null &&
                _context.TargetDamageable.IsDead == true)
            {
                _context.TargetDamageable = null;
                _context.StateMachine.ChangeState(new IdleMonsterState());
            }
            else if (_context.IsInMoveToAttackRange(_data) == true)
            {
                _context.StateMachine.ChangeState(new MoveToAttackMonsterState());
            }
            else if (_context.IsInAlertRange(_data) == false)
            {
                _context.StateMachine.ChangeState(new IdleMonsterState());
            }
        }
    }
}