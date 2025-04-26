using _KMH_Framework;
using _KMH_Framework.Pool;
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

            context.OwnerDamageable.OnAlivedEvent += OnAlived;
            context.OwnerDamageable.OnDamagedWithDataEvent += OnDamaged;
            context.OwnerDamageable.OnDeadEvent += OnDead;
        }

        private void OnAlived()
        {
            context.Inventory.enabled = true;
        }

        private void OnDamaged(Damageable attackerDamageable)
        {
            if (context.OwnerDamageable.IsDead == false)
            {
                context.TargetDamageable = attackerDamageable;

                IHunterState currentState = context.StateMachine.GetState();
                if (currentState is not HuntHunterState)
                {
                    context.StateMachine.ChangeState(new MoveToHuntHunterState());
                }
            }
        }

        private void OnDead()
        {
            context.StateMachine.ChangeState(null);
            context.AnimationController.PlayDead();
            context.Inventory.enabled = false;

            while (context.Inventory.TryPop(out Stackable stackable) == true)
            {
                PoolType itemPoolType = stackable.StackablePoolType.ToItemType();
                itemPoolType.EnablePool(obj => obj.transform.position = stackable.transform.position);
            }
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

            if (context.OwnerDamageable.IsDead == true)
            {
                context.OwnerDamageable.Alive();
            }
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