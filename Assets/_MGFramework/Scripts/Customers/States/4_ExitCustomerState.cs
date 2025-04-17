
namespace MGFramework
{
    public class ExitCustomerState : ICustomerState
    {
        private CustomerContext _context;
        private CustomerData _data;

        public void Enter(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;
        }

        public void Exit()
        {   

        }

        public void FixedTick()
        {
        
        }

        public void SlowTick()
        {
        
        }
    }
}
