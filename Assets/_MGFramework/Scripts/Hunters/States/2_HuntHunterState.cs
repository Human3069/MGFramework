using Cysharp.Threading.Tasks;

namespace MGFramework
{
    public class HuntHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;
        
        private bool isEntered = false;

        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;

            isEntered = true;
            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            while (isEntered == true)
            {
                this._context.AnimationController.PlayHunting(true);

                await UniTask.WaitForSeconds(_data.AttackSpeed);
            }
        }

        public void Exit()
        {
            isEntered = false;
            this._context.AnimationController.PlayHunting(false);
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