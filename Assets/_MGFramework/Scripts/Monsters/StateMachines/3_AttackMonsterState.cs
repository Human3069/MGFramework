using Cysharp.Threading.Tasks;

namespace MGFramework
{
    public class AttackMonsterState : IMonsterState
    {
        private MonsterContext _context;
        private MonsterData _data;

        private bool isEnter;

        public void Enter(MonsterContext context, MonsterData data)
        {
            this._context = context;
            this._data = data;

            isEnter = true;
            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            while (isEnter == true && _context.TargetDamageable.IsDead == false)
            {
                _context.AnimationController.PlayAttack();

                await UniTask.WaitForSeconds(_data.AttackSpeed);
            }
        }
        
        public void Exit()
        {
            isEnter = false;
        }

        public void FixedTick()
        {
            _context.LookTowardTarget(_data.LookAtSpeed);
        }

        public void SlowTick()
        {
            if (_context.IsInAttackRange(_data) == false)
            {
                _context.StateMachine.ChangeState(new MoveToAttackMonsterState());
            }
            else if (_context.TargetDamageable.IsDead == true)
            {
                _context.TargetDamageable = null;
                _context.StateMachine.ChangeState(new IdleMonsterState());
            }
        }
    }
}