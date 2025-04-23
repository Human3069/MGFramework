using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    public class Hunter : MonoBehaviour
    {
        [SerializeField]
        private HunterData data;

        private HunterContext context;
        private HunterStateMachine stateMachine;

        private KeyframeReceiver receiver;

        private bool _isMoving;
        private bool IsMoving
        {
            get
            {
                return _isMoving;
            }
            set
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
            context = new HunterContext(this);
            stateMachine = new HunterStateMachine(context, data);
            context.Initialize(stateMachine);

            receiver = this.GetComponentInChildren<KeyframeReceiver>();
            receiver.OnKeyframeReachedEvent += OnKeyframeReachedEvent;

            context.OwnerDamageable.OnDamagedWithDataEvent += OnDamaged;
            context.OwnerDamageable.OnDeadEvent += OnDead;
        }

        private void OnDamaged(Damageable attackerDamageable)
        {
            if (context.OwnerDamageable.IsDead == false)
            {
                context.TargetDamageable = attackerDamageable;
                context.StateMachine.ChangeState(new MoveToHuntHunterState());
            }
        }

        private void OnDead()
        {
            context.StateMachine.ChangeState(null);
            context.AnimationController.PlayDead();

            context.Inventory.Clear();
        }

        private void OnKeyframeReachedEvent(int index)
        {
            if (index == 0)
            {
                if (context.TargetDamageable != null)
                {
                    context.TargetDamageable.TakeDamage(data.AttackDamage, context.OwnerDamageable);
                }
            }
        }

        private void OnEnable()
        {
            context.StateMachine.ChangeState(new FindHuntHunterState());
        }

        private void FixedUpdate()
        {
            if (context.OwnerDamageable.IsDead == false)
            {
                IsMoving = context.Agent.IsArrived() == false;
            }
        }
    }
}