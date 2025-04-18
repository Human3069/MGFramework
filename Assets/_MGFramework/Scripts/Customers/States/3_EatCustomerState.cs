using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class EatCustomerState : ICustomerState
    {
        private CustomerContext _context;
        private CustomerData _data;

        public void Enter(CustomerContext context, CustomerData data)
        {
            this._context = context;
            this._data = data;

            _context.Agent.enabled = false;
            _context.Transform.forward = _context.OccupiedSeat.transform.forward;
            _context.AnimationController.DoSit();

            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            Vector2 range = _data.EatingDurationRange;
            float randomDuration = Random.Range(range.x, range.y);

            await UniTask.WaitForSeconds(randomDuration); 

            _context.StateMachine.ChangeState(new ExitCustomerState());
        }

        public void Exit()
        {
            _context.Agent.enabled = true;

            _context.OccupiedSeat.OccupiedCustomer = null;
            _context.OccupiedSeat = null;
        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
        
        }
    }
}
