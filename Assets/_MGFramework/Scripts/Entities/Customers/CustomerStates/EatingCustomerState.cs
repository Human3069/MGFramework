using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class EatingCustomerState : ICustomerState
    {
        private CustomerData _data;

        public EatingCustomerState(CustomerData data)
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