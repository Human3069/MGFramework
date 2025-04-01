using UnityEngine;

namespace MGFramework
{
    public class FollowToAttackMonsterState : IMonsterState
    {
        private MonsterData _data;

        public FollowToAttackMonsterState(MonsterData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            _data._Animator.SetBool("IsStop", false);
            _data._Animator.SetTrigger("IsStopValueChanged");
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            // 플레이어 따라감
            Vector3 towardedPos = _data._MonsterT.GetForwardPositionTowardsPlayer(0.1f);
            _data._Agent.destination = towardedPos;

            // 거리별 상태 전환 처리 
            float distance = _data._MonsterT.GetDistanceToPlayer();
            if (distance > _data._MinAlertRange) // (추격 => 경계)
            {
                _data._Monster._MonsterState = MonsterState.Alert;
            }
            else if (distance < _data._AttackRange) // (추격 => 공격)
            {
                _data._Monster._MonsterState = MonsterState.Attack;
            }
        }

        public void Tick()
        {
          
        }
    }
}