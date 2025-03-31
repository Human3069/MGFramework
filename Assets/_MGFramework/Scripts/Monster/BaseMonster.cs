using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class BaseMonster : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Damageable damageable;

        [Header("=== BaseMonster ===")]
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private KeyframeReceiver receiver;

        [Space(10)]
        [SerializeField]
        private PoolType outputPoolType;
        [SerializeField]
        private Vector2Int outputCountRange = new Vector2Int(2, 5);
        [SerializeField]
        private Vector3 spawnOffset;
        [SerializeField]
        private float spawnRadius;
        [SerializeField]
        private float spawnLinearForce;
        [SerializeField]
        private float spawnAngularForce;

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
            FollowToAttack,
            Attack,
        }

        [ReadOnly]
        [SerializeField]
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

                    switch (value)
                    {
                        case MonsterState.None:
                            break;

                        case MonsterState.Sleep:
                            int index = Random.Range(0, 2);
                            bool isSitOnSleep = index == 1;

                            _animator.SetBool("IsSitOnSleep", isSitOnSleep);
                            _animator.SetTrigger("IsSleep");

                            break;

                        case MonsterState.Alert:
                            _animator.SetTrigger("IsAlert");

                            break;

                        case MonsterState.FollowToAttack:
                            _animator.SetBool("IsStop", false);
                            _animator.SetTrigger("IsStopValueChanged");
                          
                            break;

                        case MonsterState.Attack:
                            _animator.SetBool("IsStop", true);
                            _animator.SetTrigger("IsStopValueChanged");

                            break;

                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
        }

        [Space(10)]
        [SerializeField]
        private int attackAnimeCount = 4;
        [SerializeField]
        private float alertLookSpeed = 4f;
        [SerializeField]
        private float attackRange = 2.5f;
        [SerializeField]
        private float attackOverlapRange = 3.5f;
        [SerializeField]
        private float attackDelay = 1f;
        [SerializeField]
        private float attackDamage = 10f;

        protected virtual void Awake()
        {
            damageable = this.GetComponent<Damageable>();
            agent = this.GetComponent<NavMeshAgent>();

            receiver.OnKeyframeReachedEvent += OnKeyframeReachedEvent;
            damageable.OnAlivedEvent += OnAlived;
            damageable.OnDamagedEvent += OnDamaged;
            damageable.OnDeadEvent += OnDead;
            damageable.OnAfterDeadEvent += OnAfterDead;

            AliveAsync().Forget();
        }

        public void OnAlived()
        {
            AliveAsync().Forget();
        }

        private async UniTaskVoid AliveAsync()
        {
            while (damageable.IsDead == false)
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
                    Vector3 playerPos = PlayerController.Instance.transform.position;
                    if (attackRange < Vector3.Distance(this.transform.position, playerPos))
                    {
                        _MonsterState = MonsterState.FollowToAttack;

                        Vector3 playerDir = (this.transform.position - playerPos).normalized;
                        Vector3 forwardDest = playerPos + (playerDir * attackRange);

                        agent.destination = forwardDest;
                    }
                    else
                    {
                        _MonsterState = MonsterState.Attack;

                        int randomizedIndex = Random.Range(0, attackAnimeCount);
                        _animator.SetInteger("AttackIndex", randomizedIndex);
                        _animator.SetTrigger("IsAttack");
                        await UniTask.WaitForSeconds(attackDelay);
                    }
                }

                await UniTask.WaitForSeconds(0.5f);
            }
        }

        private void FixedUpdate()
        {
            if (_MonsterState == MonsterState.Alert ||
                _MonsterState == MonsterState.Attack)
            {
                Vector3 playerPos = PlayerController.Instance.transform.position;
                Vector3 playerDir = (playerPos - this.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(playerDir);

                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, alertLookSpeed);
            }
        }

        private void OnKeyframeReachedEvent(int index)
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(this.transform.position, attackOverlapRange);
            foreach (Collider overlappedCollider in overlappedColliders)
            {
                if (overlappedCollider.transform != this.transform &&
                    overlappedCollider.TryGetComponent(out Damageable damageable) == true)
                {
                    damageable.CurrentHealth -= attackDamage;
                }
            }
        }

        public void OnDamaged(float maxHealth, float currentHealth)
        {
            
        }

        public void OnDead()
        {
            
        }

        public void OnAfterDead()
        {
            int outputCount = Random.Range(outputCountRange.x, outputCountRange.y);
            for (int i = 0; i < outputCount; i++)
            {
                Vector3 randomizedPos = this.transform.position + spawnOffset + (Random.insideUnitSphere * spawnRadius);

                outputPoolType.EnablePool(OnBeforePoolTargetItem);
                void OnBeforePoolTargetItem(GameObject obj)
                {
                    obj.transform.position = randomizedPos;
                    obj.transform.eulerAngles = new Vector3(Random.Range(0f, 360f),
                                                            Random.Range(0f, 360f),
                                                            Random.Range(0f, 360f));

                    Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
                    rigidbody.velocity = Random.insideUnitSphere * spawnLinearForce;
                    rigidbody.angularVelocity = Random.insideUnitSphere * spawnAngularForce;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position + spawnOffset, spawnRadius);
        }
    }
}