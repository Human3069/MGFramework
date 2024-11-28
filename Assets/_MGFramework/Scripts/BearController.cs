using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _MG_Framework
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BearController : BaseEnterable, IDamageable
    {
        protected Collider[] triggerColliders;

        public DamageableType _Type
        {
            get
            {
                return DamageableType.Bear;
            }
        }

        public event IDamageable.Damaged OnDamagedCallback;

        protected NavMeshAgent agent;

        [SerializeField]
        protected Animator animator;
        [SerializeField]
        protected KeyframeEventHandler eventHandler;

        public enum BearState
        {
            Idle,
            Alert,
            Attack,

            Dead
        }

        public enum AttackType
        {
            RightHand = 0,
            LeftHand = 1,
            Mouth = 2,
            BothHand = 3,
        }

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        protected BearState _bearState = BearState.Idle;
        protected BearState _BearState
        {
            get
            {
                return _bearState;
            }
            set
            {
                if (_bearState != value)
                {
                    _bearState = value;

                    UniTaskEx.Cancel(this, 0);
                    if (value == BearState.Alert)
                    {
                        AlertAsync().Forget();
                    }
                    else if (value == BearState.Attack)
                    {
                        MoveAndAttackAsync().Forget();
                    }
                }
            }
        }

        [SerializeField]
        protected float maxHealth;
        [ReadOnly]
        [SerializeField]
        protected float _currentHealth;
        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                if (isAlive == true)
                {
                    if (_currentHealth > value)
                    {
                        _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                        if (_currentHealth == 0f)
                        {
                            OnDeadAsync().Forget();
                        }
                        else
                        {
                            OnDamaged();
                        }

                        OnDamagedCallback?.Invoke(_currentHealth / maxHealth);
                    }
                    else
                    {
                        _currentHealth = value;
                    }
                }
                _currentHealth = value;
            }
        }

        protected bool isAlive = true;

        [SerializeField]
        protected float detectDuration = 0.5f;

        [Space(10)]
        [SerializeField]
        protected float alertDecisionRange = 12.5f;
        [SerializeField]
        protected float attackDecisionRange = 7.5f;

        [Space(10)]
        [SerializeField]
        protected float alertRotationDelta = 100f;

        [Space(10)]
        [SerializeField]
        protected float attackDamage = 30f;
        [SerializeField]
        protected float attackDistance = 2.5f;
        [SerializeField]
        protected float attackRange = 0.3f;
        [SerializeField]
        protected float attackSpeed = 1f;

        [Space(10)]
        [SerializeField]
        protected Transform rightHandT;
        [SerializeField]
        protected Transform leftHandT;
        [SerializeField]
        protected Transform mouthT;

        [Header("Instantiate Meat")]
        [SerializeField]
        protected Transform itemInstantiatePoint;
        [SerializeField]
        protected Vector2Int instantiateOnDeadRange = new Vector2Int(2, 5);
        [SerializeField]
        protected float instantiateRandomSphereRadius = 1f;

        protected PlayerController detectedPlayer = null;
        protected PlayerController attackingPlayer = null;

        protected virtual void Awake()
        {
            Debug.Assert(alertDecisionRange >= attackDecisionRange);

            Collider[] colliders = this.GetComponents<Collider>();
            List<Collider> triggerColliderList = new List<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger == true)
                {
                    triggerColliderList.Add(collider);
                }
            }
            this.triggerColliders = triggerColliderList.ToArray();

            this.agent = this.GetComponent<NavMeshAgent>();
            this.eventHandler.OnKeyframeReached_Int += OnKeyframeReached_Int;
        }

        protected virtual void OnDestroy()
        {
            eventHandler.OnKeyframeReached_Int -= OnKeyframeReached_Int;
        }

        protected virtual void OnEnable()
        {
            isAlive = true;
            DetectPlayerAsync().Forget();

            CurrentHealth = maxHealth;
        }

        protected virtual void OnDamaged()
        {
            animator.SetTrigger("damaged");
        }

        protected virtual async UniTaskVoid OnDeadAsync()
        {
            isAlive = false;
            foreach(Collider triggerCollider in triggerColliders)
            {
                triggerCollider.enabled = false;
            }

            int instantiateCount = Random.Range(instantiateOnDeadRange.x, instantiateOnDeadRange.y);
            for (int i = 0; i < instantiateCount; i++)
            {
                float randomX = Random.Range(-180f, 180f);
                float randomY = Random.Range(-180f, 180f);
                float randomZ = Random.Range(-180f, 180f);
                Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

                Vector3 randomPoint = (Random.insideUnitSphere * instantiateRandomSphereRadius) + itemInstantiatePoint.position;
                ObjectPoolManager.Instance.TakeOutObj(PoolerType.Item_Meat, randomPoint, randomRotation);
            }

            detectedPlayer = null;

            attackingPlayer.OnExitAsync().Forget();
            attackingPlayer = null;
            UniTaskEx.Cancel(this, 0);

            _BearState = BearState.Dead;

            animator.SetTrigger("dead");
        }

        protected virtual async UniTask DetectPlayerAsync()
        {
            while (true)
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, alertDecisionRange);
                PlayerController foundPlayer = null;

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent<PlayerController>(out PlayerController player) == true)
                    {
                        foundPlayer = player;
                        break;
                    }
                }

                detectedPlayer = foundPlayer;
                if (detectedPlayer == null)
                {
                    _BearState = BearState.Idle;
                }
                else
                {
                    float distance = (this.transform.position - detectedPlayer.transform.position).magnitude;
                    if (distance < attackDecisionRange)
                    {
                        _BearState = BearState.Attack;
                    }
                    else
                    {
                        _BearState = BearState.Alert;
                    }
                }

                await UniTaskEx.WaitForSeconds(this, 0, detectDuration);
            }
        }

        protected virtual async UniTask AlertAsync()
        {
            animator.SetTrigger("idleCombat");
            while (detectedPlayer != null)
            {
                Vector3 targetDir = (detectedPlayer.transform.position - this.transform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(targetDir, this.transform.up);

                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRot, Time.deltaTime * alertRotationDelta);

                await UniTaskEx.NextFrame(this, 0);
            }
        }

        protected virtual async UniTask MoveAndAttackAsync()
        {
            while (detectedPlayer != null)
            {
                float distance = (this.transform.position - detectedPlayer.transform.position).magnitude;
                if (distance >= attackDistance)
                {
                    animator.SetTrigger("run");
                }

                while (distance >= attackDistance)
                {
                    Vector3 targetPos = detectedPlayer.transform.position;
                    distance = (this.transform.position - targetPos).magnitude;

                    agent.destination = targetPos;

                    await UniTaskEx.NextFrame(this, 0);
                }

                animator.SetTrigger("idle");

                agent.destination = this.transform.position;
                int randomed = Random.Range(0, 4);

                animator.SetTrigger("attack_" + randomed);

                await UniTaskEx.WaitForSeconds(this, 0, attackSpeed);
            }
        }

        protected void OnKeyframeReached_Int(int typeIndex)
        {
            AttackType type = (AttackType)typeIndex;
            Vector3 targetPos;

            switch (type)
            {
                case AttackType.RightHand:
                    targetPos = rightHandT.position;
                    break;

                case AttackType.LeftHand:
                    targetPos = leftHandT.position;
                    break;

                case AttackType.Mouth:
                    targetPos = mouthT.position;
                    break;

                case AttackType.BothHand:
                    targetPos = (rightHandT.position + leftHandT.position) / 2f;
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            Collider[] colliders = Physics.OverlapSphere(targetPos, attackRange);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<PlayerController>(out PlayerController player) == true)
                {
                    player.CurrentHealth -= attackDamage;
                }
            }
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, alertDecisionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, attackDecisionRange);
        }

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController player) == true)
            {
                attackingPlayer = player;
                attackingPlayer.OnEnter(this);
            }
        }

        protected override void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<PlayerController>(out _) == true)
            {
                attackingPlayer.OnExitAsync().Forget();
            }
        }
    }
}