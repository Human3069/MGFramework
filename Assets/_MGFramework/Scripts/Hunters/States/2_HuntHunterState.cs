using Cysharp.Threading.Tasks;

namespace MGFramework
{
    public class HuntHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;
        
        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;

            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            while (_context.TargetDamageable != null)
            {
                if (_context.TargetDamageable != null &&
                    _context.TargetDamageable.IsDead == false)
                {
                    _context.AnimationController.PlayHunting();
                }

                await UniTask.WaitForSeconds(_data.AttackSpeed);
            }
        }

        public void Exit()
        {
    
        }

        public void FixedTick()
        {
            _context.LookTowardTarget(_data.LookAtSpeed);
        }

        public void SlowTick()
        {
            if (_context.TargetDamageable.IsDead == true)
            {
                _context.StateMachine.ChangeState(new PickUpItemsHunterState());
            }
        }
    }
}