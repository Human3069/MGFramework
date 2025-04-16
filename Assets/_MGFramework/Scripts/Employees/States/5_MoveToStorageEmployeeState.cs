using UnityEngine;

namespace MGFramework
{
    public class MoveToStorageEmployeeState : IEmployeeState
    {
        private Employee _employee;

        private Vector3 destination;

        public void Enter(Employee employee)
        {
            this._employee = employee;

            Payload targetPayload = _employee.TargetPayload;
            destination = targetPayload.GetClosestPoint(_employee.transform.position);
            _employee.SetDestination(destination);
        }

        public void Exit()
        {   

        }

        public void Tick()
        {

        }

        public void FixedTick()
        {
      
        }

        public void SlowTick()
        {
            float distance = Vector3.Distance(_employee.transform.position, destination);
            if (distance <= _employee.GetStoppingDistance())
            {
                _employee.State = EmployeeState.StoreItems;
            }
        }
    }
}
