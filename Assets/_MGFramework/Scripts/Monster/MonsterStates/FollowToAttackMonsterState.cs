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

            // Tick�� Enter�� ���� ȣ���̹Ƿ�, Enter������ �÷��̾� ���󰡰� ó��
            Vector3 towardedPos = _data._MonsterT.GetForwardPositionTowardsPlayer(0.1f);
            _data._Agent.destination = towardedPos;
        }

        public void Exit()
        {

        }

        public void SlowTick()
        {
            // �÷��̾� ����
            Vector3 towardedPos = _data._MonsterT.GetForwardPositionTowardsPlayer(0.1f);
            _data._Agent.destination = towardedPos;

            // �Ÿ��� ���� ��ȯ ó�� 
            float distance = _data._MonsterT.GetDistanceToPlayer();
            if (distance > _data._MinAlertRange) // (�߰� => ���)
            {
                _data._Monster._MonsterState = MonsterState.Alert;
            }
            else if (distance < _data._AttackRange) // (�߰� => ����)
            {
                _data._Monster._MonsterState = MonsterState.Attack;
            }
        }

        public void Tick()
        {
          
        }
    }
}