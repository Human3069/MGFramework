using _KMH_Framework.Pool;
using System.Collections.Generic;

namespace MGFramework
{
    public class FindInputStorageHunterState : IHunterState
    {
        private HunterContext _context;
        private HunterData _data;
        
        public void Enter(HunterContext context, HunterData data)
        {
            this._context = context;
            this._data = data;
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
                _context.StateMachine.ChangeState(new MoveToInputStoreHunterState());
            }
        }
    }
}