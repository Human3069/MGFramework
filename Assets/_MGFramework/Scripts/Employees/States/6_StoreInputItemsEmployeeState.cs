using _KMH_Framework.Pool;
using System.Collections.Generic;

namespace MGFramework
{
    public class StoreInputItemsEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        private List<PoolType> targetPoolTypeList;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            Payload targetPayload = _context.TargetPayload;
            targetPoolTypeList = targetPayload.GetEnterablePoolTypeList(_context.Inventory.GetPoolTypeList());
        }

        public void Exit()
        {
            _context.TargetPayload = null;
        }

        public void SlowTick()
        {
            if (targetPoolTypeList.Count > 0)
            {
                Payload targetPayload = _context.TargetPayload;
                targetPoolTypeList = targetPayload.GetEnterablePoolTypeList(_context.Inventory.GetPoolTypeList());
            }
            else
            {
                _context.StateMachine.ChangeState(new FindOutputStorageEmployeeState());
            }
        }
    }
}
