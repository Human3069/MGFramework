using _KMH_Framework;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _MG_Framework
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
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
        protected float chopDamage;

        protected EnterableIngredient enteredIngredient = null;

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
                        if (enteredIngredient != null)
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

        protected virtual void Update()
        {
            HandleMovingAnimation();
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
            if (enteredIngredient != null)
            {
                enteredIngredient.CurrentHealth -= chopDamage;
            }
        }

        public virtual void OnEnter(EnterableIngredient ingredient)
        {
            enteredIngredient = ingredient;
        }

        public virtual async UniTaskVoid OnExitAsync()
        {
            enteredIngredient = null;
            this.animator.SetTrigger("Trigger_StopMining");

            await UniTask.WaitForSeconds(0.5f);

            WorkUntilExistIngredient();
        }

        protected virtual void WorkUntilExistIngredient()
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(this.transform.position, 0.1f);
            foreach (Collider collider in overlappedColliders)
            {
                if (collider.gameObject.TryGetComponent<EnterableIngredient>(out EnterableIngredient ingredient) == true)
                {
                    OnEnter(ingredient);
                    RotateAndMiningIfStoppedAsync().Forget();

                    break;
                }
            }
        }

        protected virtual async UniTask RotateAndMiningIfStoppedAsync()
        {
            Vector3 targetDir = (enteredIngredient.transform.position - this.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(targetDir, this.transform.up);

            while (enteredIngredient != null &&
                   this.transform.rotation.Similiar(targetRot, 10f) == false)
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRot, Time.deltaTime * lookAtSpeed);

                await UniTask.NextFrame();
            }

            while (enteredIngredient != null)
            {
                if (IsStopped == true)
                {
                    animator.SetTrigger("Trigger_StartMining");
                    return;
                }

                await UniTask.NextFrame();
            }
        }
    }
}