using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class HunterAnimationController
    {
        private const int HUNTING_CLIP_COUNT = 4;

        private Animator _animator;
        private Animation _animation;

        public delegate void DeadPlayedDelegate();
        public event DeadPlayedDelegate OnDeadPlayed;

        public HunterAnimationController(Animator animator, Animation animation)
        {
            this._animator = animator;
            this._animation = animation;
        }

        public void PlayMove(bool isMoving)
        {
            _animator.SetBool("IsMoving", isMoving);
            _animator.SetTrigger("IsMovingStateChanged");
        }

        public void PlayHunting(bool isHunt)
        {
            int randomized = Random.Range(0, HUNTING_CLIP_COUNT + 1);
            _animator.SetInteger("HuntingIndex", randomized);
            _animator.SetTrigger("IsHunting");
        }

        public void PlayDead()
        {
            PlayDeadAsync().Forget();
        }

        private async UniTaskVoid PlayDeadAsync()
        {
            _animator.SetTrigger("IsDead");
            await UniTask.WaitWhile(() => _animator.IsPlaying(1) == true);

            AnimationState state = _animation.PlayQueued("OnDead");
            await UniTask.WaitWhile(() => state != null);

            OnDeadPlayed?.Invoke();
        }
    }
}