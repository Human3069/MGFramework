using _KMH_Framework.Pool;
using System.Collections.Generic;

namespace MGFramework
{
    public class StoreItemsEmployeeState : IEmployeeState
    {
        private Employee _employee;

        private List<PoolType> targetPoolTypeList;

        public void Enter(Employee employee)
        {
            this._employee = employee;

            Payload targetPayload = _employee.TargetPayload;
            targetPoolTypeList = targetPayload.GetEnterablePoolTypeList(_employee.GetPoolTypeList());
        }

        public void Exit()
        {
            _employee.TargetPayload = null;
        }

        public void Tick()
        {

        }

        public void FixedTick()
        {
      
        }

        public void SlowTick()
        {
            if (targetPoolTypeList.Count > 0)
            {
                Payload targetPayload = _employee.TargetPayload;
                targetPoolTypeList = targetPayload.GetEnterablePoolTypeList(_employee.GetPoolTypeList());
            }
            else
            {
                _employee.State = EmployeeState.FindWork;
            }
        }
    }
}
