using _KMH_Framework;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _MG_Framework
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : BaseEnterable, IDamageable
    {
        public DamageableType _Type
        {
            get
            {
                return DamageableType.Player;
            }
        }

        public event IDamageable.Damaged OnDamagedCallback;

        protected NavMeshAgent agent;
        protected KeyframeEventHandler keyframeHandler;

        [Header("Required Components")]
        [SerializeField]
        protected MousePointHandler mouseHandler;
        [SerializeField]
        protected Animator animator;

        [Header("Infos")]
        [SerializeField]
        protected float lookAtSpeed;
        [SerializeField]
        protected float handleDamage = 10;

        [Space(10)]
        [SerializeField]
        protected float maxHealth = 100f;
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

        protected IDamageable enteredDamageable = null;

        public delegate void Stopped(bool isStopped);
        public event Stopped OnStopped;

        protected bool _isStopped = true;
        public bool IsStopped
        {
            get
            {
                return _isStopped;
            }
            protected set
            {
                if (_isStopped != value)
                {
                    _isStopped = value;
                    animator.SetBool("Bool_IsStopped", value);
                    animator.SetTrigger("Trigger_OnChanged");

                    if (value == true)
                    {
                        if (enteredDamageable != null)
                        {
                            RotateAndMiningIfStoppedAsync().Forget();
                        }
                    }
                    else
                    {
                        Vector3 worldDir = (agent.destination - this.transform.position).normalized;
                        movingDir = this.transform.InverseTransformDirection(worldDir);

                        this.animator.SetTrigger("Trigger_StopMining");
                    }

                    if (OnStopped != null)
                    {
                        OnStopped(value);
                    }
                }
            }
        }

        protected Vector3 movingDir = new Vector3(0f, 0f, 1f);

        protected virtual void Awake()
        {
            this.agent = this.GetComponent<NavMeshAgent>();
            this.keyframeHandler = animator.gameObject.GetComponent<KeyframeEventHandler>();

            Debug.Assert(mouseHandler != null);

            mouseHandler.OnMouseDown += OnMouseDown;
            keyframeHandler.OnKeyframeReached += OnKeyframeReached;
        }

        protected virtual void OnDestroy()
        {
            keyframeHandler.OnKeyframeReached -= OnKeyframeReached;
            mouseHandler.OnMouseDown -= OnMouseDown;
        }

        protected virtual void Start()
        {
            isAlive = true;
            CurrentHealth = maxHealth;
        }

        protected virtual void Update()
        {
            HandleMovingAnimation();
        }

        protected virtual void OnDamaged()
        {
            animator.SetTrigger("damaged");
        }

        protected virtual async UniTaskVoid OnDeadAsync()
        {
            isAlive = false;
        }

        protected virtual void HandleMovingAnimation()
        {
            float distanceBetween = Mathf.Abs((agent.destination - this.transform.position).magnitude);
            IsStopped = agent.stoppingDistance > distanceBetween;

            if (IsStopped == false)
            {
                Vector3 worldDir = (agent.destination - this.transform.position).normalized;
                Vector3 localDir = this.transform.InverseTransformDirection(worldDir);
                movingDir = Vector3.MoveTowards(movingDir, localDir, Time.deltaTime * 2f);

                animator.SetFloat("Float_Dir_Forward", movingDir.z);
                animator.SetFloat("Float_Dir_Right", movingDir.x);
            }
        }

        protected virtual void OnMouseDown(Vector3 worldPos)
        {
            agent.SetDestination(worldPos);
        }

        protected virtual void OnKeyframeReached()
        {
            if (enteredDamageable != null)
            {
                enteredDamageable.CurrentHealth -= handleDamage;
            }
        }

        public virtual void OnEnter(IDamageable damageable)
        {
            enteredDamageable = damageable;
        }

        public virtual async UniTaskVoid OnExitAsync()
        {
            enteredDamageable = null;
            this.animator.SetTrigger("Trigger_StopMining");

            await UniTask.WaitForSeconds(0.5f);

            WorkUntilExistIngredient();
        }

        protected virtual void WorkUntilExistIngredient()
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(this.transform.position, 0.1f);
            foreach (Collider collider in overlappedColliders)
            {
                if (collider.gameObject.TryGetComponent<BaseEnterable>(out BaseEnterable enterable) == true)
                {
                    if (enterable != this &&
                        enterable is IDamageable)
                    {
                        OnEnter(enterable as IDamageable);
                        RotateAndMiningIfStoppedAsync().Forget();

                        break;
                    }
                }
            }
        }

        protected virtual async UniTask RotateAndMiningIfStoppedAsync()
        {
            Vector3 targetDir = (enteredDamageable.transform.position - this.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(targetDir, this.transform.up);

            while (enteredDamageable != null &&
                   this.transform.rotation.Similiar(targetRot, 10f) == false)
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRot, Time.deltaTime * lookAtSpeed);

                await UniTask.NextFrame();
            }

            while (enteredDamageable != null)
            {
                if (IsStopped == true)
                {
                    animator.SetTrigger("Trigger_StartMining");
                    return;
                }

                await UniTask.NextFrame();
            }
        }

        protected override void OnTriggerEnter(Collider collider)
        {
           
        }

        protected override void OnTriggerExit(Collider collider)
        {
     
        }
    }
}