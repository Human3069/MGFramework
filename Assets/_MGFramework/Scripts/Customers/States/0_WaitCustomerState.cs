using UnityEngine;

namespace MGFramework
{
    public class WaitCustomerState : ICustomerState
    {
        private CustomerContext _context;
        private CustomerData _data;

        private int desiredFoodCount;

        public void Enter(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;

            Vector2Int countRange = _data.DesiredFoodCountRange;
            desiredFoodCount = Random.Range(countRange.x, countRange.y + 1);
        }

        public void Exit()
        {   

        }

        public void FixedTick()
        {
            bool isArrived = _context.Agent.IsArrived();
            Vector3 desiredDirection = _context.DesiredDirection;

            if (isArrived == true &&
                desiredDirection != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                _context.Transform.rotation = Quaternion.RotateTowards(_context.Transform.rotation, desiredRotation, _data.LookAtSpeed);
            }
        }

        public void SlowTick()
        {
        
        }
    }
}
