using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class Customer : MonoBehaviour
    {
        [SerializeField]
        private CustomerData data;
        private CustomerContext context;
        private CustomerStateMachine stateMachine;

        private bool _isMoving = false;
        public bool IsMoving
        {
            get
            {
                return _isMoving;
            }
            private set
            {
                if (_isMoving != value)
                {
                    _isMoving = value;
                    context.AnimationController.PlayMove(value);
                }
            }
        }

        private void Awake()
        {
            context = new CustomerContext(this);
            stateMachine = new CustomerStateMachine(context, data);

            context.Initialize(stateMachine);
        }

        private void OnEnable()
        {
            context.StateMachine.ChangeState(new WaitCustomerState());
            context.IsCustomerInitialized = true;
        }

        private void OnDisable()
        {
            context.StateMachine.ChangeState(null);
            context.IsCustomerInitialized = false;
        }

        private void FixedUpdate()
        {
            if (context.Agent.enabled == true)
            {
                IsMoving = context.Agent.IsArrived() == false;
            }
        }

        public async UniTaskVoid UpdatePoseAsync(Vector3 destination, Vector3 direction)
        {
            await UniTask.WaitWhile(() => context == null);
            await UniTask.WaitWhile(() => context.Agent == null);

            context.Agent.SetDestination(destination);
            context.DesiredDirection = direction;
        }

        public bool IsExiting()
        {
            ICustomerState state = context.StateMachine.GetState();
            return state is ExitCustomerState;
        }
    }
}
