using Cysharp.Threading.Tasks;

namespace MGFramework
{
    public class MoveToAttackMonsterState : IMonsterState
    {
        private MonsterContext _context;
        private MonsterData _data;

        private bool _isStopped = false;
        private bool IsStopped
        {
            get
            {
                return _isStopped;
            }
            set
            {
                if (_isStopped != value)
                {
                    _isStopped = value;
                    _context.AnimationController.PlayMove(value);
                }
            }
        }

        public void Enter(MonsterContext context, MonsterData data)
        {
            this._context = context;
            this._data = data;
        }

        public void Exit()
        {
   
        }

        public void FixedTick()
        {
            _context.LookTowardTarget(_data.LookAtSpeed);
            IsStopped = _context.Agent.IsArrived() == false;

            if (_context.IsInAttackRange(_data) == true)
            {
                _context.StateMachine.ChangeState(new AttackMonsterState());
            }
            else if (_context.IsInMoveToAttackRange(_data) == false)
            {
                _context.StateMachine.ChangeState(new AlertMonsterState());
            }
        }

        public void SlowTick()
        {
            _context.MoveToTargetForward(2f);
        }
    }
}