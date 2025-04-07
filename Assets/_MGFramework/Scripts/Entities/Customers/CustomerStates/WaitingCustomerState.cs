using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class WaitingCustomerState : ICustomerState
    {
        private CustomerData _data;

        public WaitingCustomerState(CustomerData data)
        {
            this._data = data;
        }

        public void Enter()
        {
   
        }

        public void Exit()
        {
            _data._Customer.DesiredDirection = null;
        }

        public void SlowTick()
        {
            
        }

        public void FixedTick()
        {
            Vector3? desiredDirection = _data._Customer.DesiredDirection;

            if (desiredDirection != null &&
                _data._Customer.IsMoving == false)
            {
                _data._Transform.forward = Vector3.MoveTowards(_data._Transform.forward, desiredDirection.Value, _data._DirectionSpeed);
            }
        }
    }
}