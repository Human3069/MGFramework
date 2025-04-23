using UnityEngine;

namespace MGFramework
{
    public class MoveToInputStoreHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;

        private Vector3 destination;

        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;

            Payload targetPayload = _context.TargetPayload;
            Vector3 employeePos = _context.Transform.position;

            destination = targetPayload.GetClosestInputStorePoint(employeePos);
            _context.Agent.SetDestination(destination);
        }
        
        public void Exit()
        {
            // 상태 종료 처리
        }

        public void FixedTick()
        {

        }

        public void SlowTick()
        {
            if (_context.Agent.IsArrived() == true)
            {
                _context.StateMachine.ChangeState(new StoreInputItemsHunterState());
            }
        }
    }
}