using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class ReturningCustomerState : ICustomerState
    {
        private CustomerData _data;

        public ReturningCustomerState(CustomerData data)
        {
            this._data = data;
        }

        public void Enter()
        {
          
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            
        }

        public void FixedTick()
        {
           
        }
    }
}