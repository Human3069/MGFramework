using System.Collections.Generic;
using _KMH_Framework.Pool;

namespace MGFramework
{
    public class StoreInputItemsHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;

        private List<PoolType> targetPoolTypeList;

        public void Enter(HunterContext context, HunterData data)
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

        public void FixedTick()
        {

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
                _context.StateMachine.ChangeState(new FindHuntHunterState());
            }
        }
    }
}