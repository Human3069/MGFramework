using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public enum MonsterState
    {
        None = -1,

        Sleep,
        Alert,
        FollowToAttack,
        Attack,
    }

    public class Monster : MonoBehaviour
    {
        [SerializeField]
        private bool isShowLog = false;
        [SerializeField]
        private KeyframeReceiver receiver;
        [SerializeField]
        private MonsterData monsterData;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private MonsterState _monsterState = MonsterState.None;
        public MonsterState _MonsterState
        {
            get
            {
                return _monsterState;
            }
            set
            {
                if (_monsterState != value)
                {
                    IMonsterState targetState = null;
                    if (value == MonsterState.Sleep)
                    {
                        targetState = new SleepMonsterState(monsterData);
                    }
                    else if (value == MonsterState.Alert)
                    {
                        targetState = new AlertMonsterState(monsterData);
                    }
                    else if (value == MonsterState.FollowToAttack)
                    {
                        targetState = new FollowToAttackMonsterState(monsterData);
                    }
                    else if (value == MonsterState.Attack)
                    {
                        targetState = new AttackMonsterState(monsterData);
                    }

                    stateMachine.ChangeState(targetState);
                    _monsterState = value;
                }
            }
        }

        private Damageable damageable;
        private MonsterStateMachine stateMachine;

        private UniTask.Awaiter awaitor;

        private void Awake()
        {
            this.damageable = GetComponent<Damageable>();
            this.stateMachine = new MonsterStateMachine();

            receiver.OnKeyframeReachedEvent += OnKeyframeReachedEvent;
            damageable.OnAlivedEvent += OnAlived;
            damageable.OnDamagedEvent += OnDamaged;
            damageable.OnDeadEvent += OnDead;
            damageable.OnAfterDeadEvent += OnAfterDead;
        }

        private void OnEnable()
        {
            if (awaitor.IsCompleted == true)
            {
                awaitor = AliveAsync().GetAwaiter();
            }

            _MonsterState = MonsterState.Sleep;
        }

        private void FixedUpdate()
        {
            if (damageable.IsDead == false)
            {
                stateMachine.Tick();
            }
        }

        private void OnKeyframeReachedEvent(int index)
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(this.transform.position, monsterData._AttackOverlapRange);
            foreach (Collider overlappedCollider in overlappedColliders)
            {
                if (overlappedCollider.transform != this.transform &&
                    overlappedCollider.TryGetComponent(out Damageable damageable) == true)
                {
                    damageable.CurrentHealth -= monsterData._AttackDamage;
                }
            }
        }

        private void OnAlived()
        {
            if (awaitor.IsCompleted == true)
            {
                awaitor = AliveAsync().GetAwaiter();
            }
        }

        private async UniTask AliveAsync()
        {
            while (damageable.IsDead == false)
            {
                stateMachine.SlowTick();
                await UniTask.WaitForSeconds(0.5f);
            }

            stateMachine.ChangeState(null);
        }

        private void OnDamaged(float maxHealth, float currentHealth)
        {
     
        }

        private void OnDead()
        {
        
        }

        private void OnAfterDead()
        {
         
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            stateMachine?.OnIsShowLogValueChange(isShowLog);
        }
#endif
    }
}