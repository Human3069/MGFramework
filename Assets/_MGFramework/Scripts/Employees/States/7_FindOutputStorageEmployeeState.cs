
namespace MGFramework
{
    public class FindOutputStorageEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            Payload outputContainedPayload = _context.Transform.FindNearest<Payload>(x => x.HasOutputStore() == true);
            if (outputContainedPayload == null)
            {
                _context.StateMachine.ChangeState(new FindWorkEmployeeState());
            }
            else
            {
                _context.TargetPayload = outputContainedPayload;
                _context.StateMachine.ChangeState(new MoveToOutputStorageEmployeeState());
            }
        }

        public void Exit()
        {   

        }
        

        public void SlowTick()
        {
         
        }
    }
}
