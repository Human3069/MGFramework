using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class FindInputStorageEmployeeState : IEmployeeState
    {
        private const float SAFETY_LOCK_DURATION = 10f;

        private EmployeeContext _context;
        private EmployeeData _data;

        private bool isEntered = false;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            isEntered = true;

            this._context = context;
            this._data = data;

            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            await UniTask.WaitForSeconds(SAFETY_LOCK_DURATION);
           
            if (isEntered == true)
            {
                if (_context.TargetPayload == null)
                {
                    _context.StateMachine.ChangeState(new FindWorkEmployeeState());
                }
            }
        }

        public void Exit()
        {
            isEntered = false;
        }

        public void SlowTick()
        {
            if (_context.TargetPayload == null)
            {
                List<PoolType> poolTypeList = _context.Inventory.GetPoolTypeList();
                Payload foundPayload = _context.Transform.FindPoorestPayload(PoolType.Stackable_Wood, PoolType.Stackable_RawMeat, FindPredicate);
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
