using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class SleepMonsterState : IMonsterState
    {
        private MonsterData _data;

        public SleepMonsterState(MonsterData data)
        {
            this._data = data;
        }

        public void Enter()
        {
            int index = Random.Range(0, 2);

            _data._Animator.SetBool("IsSitOnSleep", index == 1);
            _data._Animator.SetTrigger("IsSleep");
        }

        public void Exit()
        {
        
        }

        public void SlowTick()
        {
            // �Ÿ��� ���� ��ȯ ó��
            float distance = _data._MonsterT.GetDistanceToPlayer();
            if (distance < _data._MinSleepRange) // (��ȭ => ���)
            {
                _data._Monster._MonsterState = MonsterState.Alert;
            }
        }

        public void Tick()
        {
          
        }
    }
}