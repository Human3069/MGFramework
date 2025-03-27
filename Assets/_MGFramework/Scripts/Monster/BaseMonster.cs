using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class BaseMonster : BaseDamageable
    {
        private NavMeshAgent agent;

        [Header("=== BaseMonster ===")]
        [SerializeField]
        private Animator _animator;

        [Space(10)]
        [SerializeField]
        private float minimumSleepRange = 10f;
        [SerializeField]
        private float minimumAlertRange = 5f;

        private enum MonsterState
        {
            None,

            Sleep,
            Alert,
            Attack,
        }

        private MonsterState _monsterState = MonsterState.None;
        private MonsterState _MonsterState
        {
            get
            {
                return _monsterState;
            }
            set
            {
                if (_monsterState != value)
                {
                    _monsterState = value;
                    
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            agent = this.GetComponent<NavMeshAgent>();
        }

        public override void Alive()
        {
            base.Alive();
            AliveAsync().Forget();
        }

        private async UniTaskVoid AliveAsync()
        {
            while (IsDead == false)
            {
                float distance = Vector3.Distance(PlayerController.Instance.transform.position, this.transform.position);
                if (distance > minimumSleepRange)
                {
                    _MonsterState = MonsterState.Sleep;
                }
                else if (distance > minimumAlertRange)
                {
                    _MonsterState = MonsterState.Alert;
                }
                else
                {
                    _MonsterState = MonsterState.Attack;
                    agent.destination = PlayerController.Instance.transform.position;
                }

                await UniTask.WaitForSeconds(0.5f);
            }
        }

        protected override void OnDamaged()
        {
            base.OnDamaged();
        }

        protected override void OnDead()
        {
            base.OnDead();
        }
    }
}