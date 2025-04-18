using UnityEngine;

namespace MGFramework
{
    public class CustomerAnimationController
    {
        private Animator _animator;

        public CustomerAnimationController(Animator animator)
        {
            this._animator = animator;
        }

        public void PlayMove(bool isMove)
        {
            _animator.SetBool("IsMoving", isMove);
            _animator.SetTrigger("IsMovingStateChanged");
        }

        public void DoSit()
        {
            _animator.SetTrigger("DoSit");
        }
    }
}
