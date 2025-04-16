using _KMH_Framework.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class FindStorageEmployeeState : IEmployeeState
    {
        private Employee _employee;

        public void Enter(Employee employee)
        {
            this._employee = employee;
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
            if (_employee.TargetPayload == null)
            {
                List<PoolType> poolTypeList = _employee.GetPoolTypeList();
                _employee.TargetPayload = _employee.FindNearest<Payload>(x => x.IsEnterablePoolTypeList(poolTypeList) == true);
            }
            else
            {
                _employee.State = EmployeeState.MoveToStorage;
            }
        }
    }
}
