
namespace MGFramework
{
    public class StoreOutputItemsEmployeeState : IEmployeeState
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
            _context.TargetPayload = null;
        }

        public void SlowTick()
        {
            if (_context.TargetPayload.HasOutputStore() == false)
            {
                _context.StateMachine.ChangeState(new FindInputStorageEmployeeState());
            }
        }
    }
}
