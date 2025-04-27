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

        public void Upgrade(EmployeeExcelRow row)
        {
            context.Agent.speed = row.MovementSpeed;
            data.UpdateData(row);
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
                context.TargetHarvestable._Damageable.TakeDamage(data.AttackDamage, null);
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
