using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class ChopEmployerState : IEmployerState
    {
        private EmployerData _data;

        public ChopEmployerState(EmployerData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            _data._Animator.SetBool("IsStartMining", true);
            _data._Animator.SetTrigger("IsStartMiningStateChanged");

            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            await UniTask.WaitWhile(() => _data._Employer.TargetHarvestable.IsHarvestable == true);

            _data._Employer._EmployerState = EmployerState.CarryItem;
        }

        public void Exit()
        {
            _data._Animator.SetBool("IsStartMining", false);
            _data._Animator.SetTrigger("IsStartMiningStateChanged");
        }

        public void SlowTick()
        {
            
        }

        public void FixedTick()
        {
           
        }
    }
}