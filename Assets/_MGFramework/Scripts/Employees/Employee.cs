using UnityEngine;

namespace MGFramework
{
    public class Employee : MonoBehaviour
    {
        [SerializeField]
        private EmployeeData data;
        private EmployeeContext context;
        private EmployeeStateMachine stateMachine;

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
            context = new EmployeeContext(this);
            stateMachine = new EmployeeStateMachine(context, data);
            context.Initialize(stateMachine);

            context.Receiver.OnKeyframeReachedEvent += OnKeyframeReached;
        }

        private void OnKeyframeReached(int index)
        {
            if (context.TargetHarvestable != null)
            {
                context.TargetHarvestable._Damageable.CurrentHealth -= data.AttackDamage;
            }
        }

        private void OnEnable()
        {
            context.StateMachine.ChangeState(new FindWorkEmployeeState());
        }

        private void FixedUpdate()
        {
            IsMoving = context.Agent.IsArrived() == false;
        }

        [ContextMenu("Log Current State")]
        public void LogCurrentState()
        {
            context.StateMachine.LogCurrentState();
        }
    }
}
