using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class WorkEmployeeState : IEmployeeState
    {
        private EmployeeContext _context;
        private EmployeeData _data;

        public void Enter(EmployeeContext context, EmployeeData data)
        {
            this._context = context;
            this._data = data;

            // 추가 움직임을 방지하기 위해 이동을 멈춘다.
            this._context.Agent.destination = _context.Transform.position;

            EnterAsync().Forget();
        }

        private async UniTask EnterAsync()
        {
            while (_context.TargetHarvestable != null)
            {
                if (_context.TargetHarvestable != null &&
                    _context.TargetHarvestable._Damageable.IsDead == false)
                {
                    this._context.AnimationController.PlayWorking();
                }

                await UniTask.WaitForSeconds(_data.AttackSpeed);
            }
        }

        public void Exit()
        {
            this._context.TargetHarvestable = null;
        }

        public void SlowTick()
        {
            if (_context.TargetHarvestable._Damageable.IsDead == true)
            {
                _context.StateMachine.ChangeState(new PickUpItemsEmployeeState());
            }
        }
    }
}
