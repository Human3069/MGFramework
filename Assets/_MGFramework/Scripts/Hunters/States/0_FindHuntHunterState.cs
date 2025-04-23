namespace MGFramework
{
    public class FindHuntHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;
        
        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;
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
            if (_context.TargetDamageable == null)
            {
                _context.TargetDamageable = _context.Transform.FindNearest<Damageable>(FindNearestPredicate);
                bool FindNearestPredicate(Damageable damageable)
                {
                    return damageable.GetOwnerType() == OwnerType.Enemys &&
                           damageable.IsDead == false;
                }
            }
            else
            {
                _context.StateMachine.ChangeState(new MoveToHuntHunterState());
            }
        }
    }
}