using UnityEngine;

namespace MGFramework
{
    public class MoveToSeatCustomerState : ICustomerState
    {
        private CustomerContext _context;
        private CustomerData _data;

        public void Enter(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;

            _context.Agent.SetDestination(_context.OccupiedSeat.transform.position);
        }

        public void Exit()
        {   

        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
            bool isArrived = _context.Agent.IsArrived();
            if (isArrived == true)
            {
                _context.StateMachine.ChangeState(new EatCustomerState());
            }
        }
    }
}
