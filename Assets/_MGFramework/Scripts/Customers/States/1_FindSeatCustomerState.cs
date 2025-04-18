using UnityEngine;

namespace MGFramework
{
    public class FindSeatCustomerState : ICustomerState
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
            CustomerWaitingLine waitingLine = GameManager.Instance.WaitingLine;
            waitingLine.Dequeue(out Customer customer);

            Debug.Assert(customer == _context.Customer);
        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
            if (_context.OccupiedSeat == null)
            {
                CustomerSeat foundSeat = this._context.Transform.FindNearest<CustomerSeat>(FindNearestPredicate);
                bool FindNearestPredicate(CustomerSeat seat)
                {
                    return seat.OccupiedCustomer == null;
                }

                if (foundSeat != null)
                {
                    _context.OccupiedSeat = foundSeat;
                    _context.OccupiedSeat.OccupiedCustomer = _context.Customer;
                }
            }
            else
            {
                _context.StateMachine.ChangeState(new MoveToSeatCustomerState());
            }
        }
    }
}
