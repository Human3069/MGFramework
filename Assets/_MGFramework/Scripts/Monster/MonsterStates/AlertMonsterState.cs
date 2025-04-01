using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class AlertMonsterState : IMonsterState
    {
        private MonsterData _data;

        public AlertMonsterState(MonsterData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            _data._Animator.SetTrigger("IsAlert");
            _data._Agent.destination = _data._MonsterT.position;
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            // �Ÿ��� ���� ��ȯ ó��
            float distance = _data._MonsterT.GetDistanceToPlayer();
            if (distance > _data._MinSleepRange) // (��� => ��ȭ)
            {
                _data._Monster._MonsterState = MonsterState.Sleep;
            }
            else if (distance < _data._MinAlertRange) // (��� => �߰�)
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