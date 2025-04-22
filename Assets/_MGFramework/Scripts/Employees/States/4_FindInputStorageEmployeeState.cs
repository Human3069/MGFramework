using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class FindInputStorageEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;
        }

        public void Exit()
        {   

        }

        public void SlowTick()
        {
            if (_context.TargetPayload == null)
            {
                List<PoolType> poolTypeList = _context.Inventory.GetPoolTypeList();
                Payload foundPayload = _context.Transform.FindNearest<Payload>(FindPredicate);
                bool FindPredicate(Payload payload)
                {
                    bool isEnterable = payload.IsEnterablePoolTypeList(poolTypeList);
                    bool isActive = payload.gameObject.activeInHierarchy == true;
                    bool isCanUse = payload.IsCanUse == true;

                    return isEnterable == true &&
                           isActive == true &&
                           isCanUse == true;
                }

                _context.TargetPayload = foundPayload;
            }
            else
            {
                _context.StateMachine.ChangeState(new MoveToInputStorageEmployeeState());
            }
        }
    }
}
