using UnityEngine;

namespace MGFramework
{
    public class MoveToOutputStorageEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        private Vector3 destination;
  
        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            Payload targetPayload = _context.TargetPayload;
            Vector3 employeePos = _context.Transform.position;

            destination = targetPayload.GetClosestOutputStorePoint(employeePos);
            _context.Agent.SetDestination(destination);
        }

        public void Exit()
        {   

        }

        public void SlowTick()
        {
            if (_context.Agent.IsArrived() == true)
            {
                _context.StateMachine.ChangeState(new StoreOutputItemsEmployeeState());
            }
        }
    }
}
