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

        [Space(10)]
        [SerializeField]
        private MonsterData monsterData;

        private Damageable damageable;
        private MonsterStateMachine stateMachine;

        private UniTask.Awaiter awaitor;

        private void Awake()
        {
            this.damageable = this.GetComponent<Damageable>();
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

            monsterData._Agent.enabled = true;
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
            monsterData._Agent.enabled = false;
        }

        private void OnAfterDead()
        {
            int randomizedIndex = Random.Range(monsterData._OutputCountRange.x, monsterData._OutputCountRange.y);
            for (int i = 0; i < randomizedIndex; i++)
            {
                monsterData._OutputType.EnablePool(OnBeforeEnablePool);
                void OnBeforeEnablePool(GameObject poolObj)
                {
                    Vector3 randomizedPos = this.transform.position + monsterData._SpawnOffset + (Random.insideUnitSphere * monsterData._SpawnRadius);

                    poolObj.transform.position = randomizedPos;
                    poolObj.transform.eulerAngles = new Vector3(Random.Range(0f, 360f),
                                                                Random.Range(0f, 360f),
                                                                Random.Range(0f, 360f));

                    Rigidbody rigidbody = poolObj.GetComponent<Rigidbody>();
                    rigidbody.velocity = Random.insideUnitSphere * monsterData._SpawnLinearForce;
                    rigidbody.angularVelocity = Random.insideUnitSphere * monsterData._SpawnAngularForce;
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            stateMachine?.OnIsShowLogValueChange(isShowLog);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position + monsterData._SpawnOffset, monsterData._SpawnRadius);
        }
#endif
    }
}