using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class MonsterAnimationController
    {
        private Animator _animator;
        private Animation _animation;

        public MonsterAnimationController(Animator animator, Animation animation)
        {
            this._animator = animator;
            this._animation = animation;
        }

        public void PlaySleepOrSit()
        {
            int randomIndex = Random.Range(0, 2);
            bool isSitOnSleep = randomIndex == 0;

            _animator.SetBool("IsSitOnSleep", isSitOnSleep);
            _animator.SetTrigger("IsSleep");
        }

        public void PlayAlert()
        {
            PlayMove(false);
        }

        public void PlayMove(bool isMoving)
        {
            _animator.SetBool("IsMoving", isMoving);
            _animator.SetTrigger("IsMovingStateChanged");
        }

        public void PlayAttack()
        {
            int attackIndex = Random.Range(0, 5);
            _animator.SetInteger("AttackIndex", attackIndex);
            _animator.SetTrigger("IsAttack");
        }

        public void PlayAlived()
        {
            _animation.Play("OnAlived");
        }

        public void PlayDamaged()
        {
            _animation.Play("OnDamaged");
        }

        public void PlayDead()
        {
            PlayDeadAsync().Forget();
        }

        private async UniTaskVoid PlayDeadAsync()
        {
            _animator.SetTrigger("IsDead");

            await UniTask.WaitWhile(() => _animator.IsPlaying(0) == true);

            _animation.Play("OnDead");
        }
    }
}