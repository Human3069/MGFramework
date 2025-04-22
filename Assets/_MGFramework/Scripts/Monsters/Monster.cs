using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    public class Monster : MonoBehaviour
    {
        private Collider _collider;
        private Damageable damageable;
        private KeyframeReceiver receiver;

        [SerializeField]
        private MonsterData data;
     
        private MonsterContext context;
        private MonsterStateMachine stateMachine;
        
        private void Awake()
        {
            context = new MonsterContext(this, data);
            stateMachine = new MonsterStateMachine(context, data);
            context.Initialize(stateMachine);

            _collider = this.GetComponent<Collider>();

            damageable = this.GetComponent<Damageable>();
            damageable.OnAlivedEvent += OnAlived;
            damageable.OnDamagedEvent += OnDamagedEvent;
            damageable.OnDeadEvent += OnDeadEvent;

            receiver = this.GetComponentInChildren<KeyframeReceiver>();
            receiver.OnKeyframeReachedEvent += OnKeyframeReachedEvent;
        }

        private void OnEnable()
        {
            _collider.enabled = true;
            stateMachine.ChangeState(new IdleMonsterState());
        }

        private void OnAlived()
        {
            context.AnimationController.PlayAlived();
        }

        private void OnDamagedEvent()
        {
            context.AnimationController.PlayDamaged();
        }

        private void OnDeadEvent()
        {
            _collider.enabled = false;
            context.StateMachine.ChangeState(null);
            context.AnimationController.PlayDead();

            Vector2Int deadCountRange = data.DropItemCountRange;
            Vector3 itemPoolPoint = this.transform.position + data.DropItemOffset;
            int deadCount = Random.Range(deadCountRange.x, deadCountRange.y + 1);

            for (int i = 0; i < deadCount; i++)
            {
                data.DropItemType.EnablePool(OnBeforeEnablePool);
                void OnBeforeEnablePool(GameObject obj)
                {
                    Vector3 targetPoint = itemPoolPoint + (Random.insideUnitSphere * data.DropItemRadius);
                    obj.transform.position = targetPoint;

                    Vector3 randomizedEuler = Random.insideUnitSphere * 360f;
                    obj.transform.eulerAngles = randomizedEuler;
                }
            }
        }

        private void OnKeyframeReachedEvent(int index)
        {
            if (context.TargetDamageable != null)
            {
                context.TargetDamageable.CurrentHealth -= data.AttackDamage;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, data.MaxAttackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, data.MaxMoveToAttackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, data.MaxAlertRange);

            if (context != null && context.StateMachine != null)
            {
                IMonsterState currentState = context.StateMachine.GetState();
                switch (currentState)
                {
                    case IdleMonsterState:
                        Gizmos.color = Color.green;
                        break;

                    case AlertMonsterState:
                        Gizmos.color = Color.yellow;
                        break;

                    case MoveToAttackMonsterState:
                        Gizmos.color = Color.red;
                        break;

                    case AttackMonsterState:
                        Gizmos.color = Color.black;
                        break;

                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawSphere(this.transform.position + Vector3.up * 2.5f, 0.25f);
            }

            Gizmos.color = Color.black;

            Vector3 itemPoolPoint = this.transform.position + data.DropItemOffset;
            Gizmos.DrawWireSphere(itemPoolPoint, data.DropItemRadius);
        }
#endif
    }
}