using _KMH_Framework.Pool;
using System.Collections.Generic;

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
                _context.TargetPayload = _context.Transform.FindNearest<Payload>(x => x.IsEnterablePoolTypeList(poolTypeList) == true);
            }
            else
            {
                _context.StateMachine.ChangeState(new MoveToInputStorageEmployeeState());
            }
        }
    }
}
