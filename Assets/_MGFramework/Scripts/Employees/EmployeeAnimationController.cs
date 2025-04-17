using UnityEngine;

namespace MGFramework
{
    public class EmployeeAnimationController 
    {
        private Animator _animator;

        public EmployeeAnimationController(Animator animator)
        {
            this._animator = animator;
        }

        public void PlayWorking(bool isWork)
        {
            _animator.SetBool("IsStartMining", isWork);
            _animator.SetTrigger("IsStartMiningStateChanged");
        }

        public void PlayMove(bool isMove)
        {
            _animator.SetBool("IsMoving", isMove);
            _animator.SetTrigger("IsMovingStateChanged");
        }
    }
}
