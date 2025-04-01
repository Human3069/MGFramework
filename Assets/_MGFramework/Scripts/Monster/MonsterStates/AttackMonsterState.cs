using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class AttackMonsterState : IMonsterState
    {
        private MonsterData _data;
        private bool isEntered = false;

        public AttackMonsterState(MonsterData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            _data._Animator.SetBool("IsStop", true);
            _data._Animator.SetTrigger("IsStopValueChanged");

            isEntered = true;
            EnterAsync().Forget();
        }

        private async UniTaskVoid EnterAsync()
        {
            while (isEntered == true)
            {
                int randomizedIndex = Random.Range(0, _data._AttackAnimeCount);
                _data._Animator.SetInteger("AttackIndex", randomizedIndex);
                _data._Animator.SetTrigger("IsAttack");

                await UniTask.WaitForSeconds(_data._AttackDelay);
            }
        }

        public void Exit()
        {
            isEntered = false;
        }

        public void SlowTick()
        {
            // 거리별 상태 전환 처리
            float distance = _data._MonsterT.GetDistanceToPlayer();
            if (distance > _data._AttackRange) // (공격 => 추격)
            {
                _data._Monster._MonsterState = MonsterState.FollowToAttack;
            }
        }

        public void Tick()
        {
            _data._MonsterT.RotateTowardsPlayer(_data._TowardSpeed);
        }
    }
}   