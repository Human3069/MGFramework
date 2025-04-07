using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    public enum CustomerState
    {
        None = -1,

        Waiting,
        Moving,
        Eating,
        Returning,
    }

    public class Customer : MonoBehaviour
    {
        // private const string LOG_FORMAT = "<color=white><b>[Customer]</b></color> {0}";

        [SerializeField]
        private KeyframeReceiver receiver;
        [SerializeField]
        private CustomerData data;
        [ReadOnly]
        [SerializeField]
        private CustomerState _customerState;
        public CustomerState CustomerState
        {
            get
            {
                return _customerState;
            }
            set
            {
                if (_customerState != value)
                {
                    ICustomerState targetState = null;
                    if (value == CustomerState.Waiting)
                    {
                        targetState = new WaitingCustomerState(data);
                    }
                    else if (value == CustomerState.Moving)
                    {
                        targetState = new MovingCustomerState(data);
                    }
                    else if (value == CustomerState.Eating)
                    {
                        targetState = new EatingCustomerState(data);
                    }
                    else if (value == CustomerState.Returning)
                    {
                        targetState = new ReturningCustomerState(data);
                    }

                    stateMachine.ChangeState(targetState);
                    _customerState = value;
                }
            }
        }

        private CustomerStateMachine stateMachine;

        public Vector3? DesiredDirection = null;

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

                    data._Animator.SetBool("IsMoving", value);
                    data._Animator.SetTrigger("IsMovingStateChanged");
                }
            }
        }

        private void Awake()
        {
            stateMachine = new CustomerStateMachine();
            CustomerState = CustomerState.Waiting;
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick();

            IsMoving = Vector3.Distance(this.transform.position, data._Agent.destination) > data._Agent.stoppingDistance;
        }

        public void SetDestination(Vector3 destination)
        {
            data._Agent.SetDestination(destination);
        }

        public void SetDirection(Vector3 direction)
        {
            DesiredDirection = direction;
        }
    }
}