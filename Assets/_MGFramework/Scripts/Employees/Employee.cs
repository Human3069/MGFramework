using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public enum EmployeeState
    {
        None = -1,

        FindWork,
        MoveToWork,
        Work,
        PickUpItems,
        FindStorage,
        MoveToStorage,
        StoreItems,
    }

    public class Employee : MonoBehaviour
    {
        private EmployeeStateMachine stateMachine;
        private NavMeshAgent agent;
        private Inventory inventory;

        [SerializeField]
        private KeyframeReceiver receiver;
        [SerializeField]
        private Animator animator;
    
        [Space(10)]
        [SerializeField]
        private float attackDamage = 10f;
        public float AttackRange = 1f;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private EmployeeState _state = EmployeeState.None;
        public EmployeeState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;

                    IEmployeeState state = value.GetState();
                    stateMachine.ChangeState(state);
                }
            }
        }

        [Space(10)]
        [ReadOnly]
        public Harvestable TargetHarvestable = null;
        [ReadOnly]
        public Payload TargetPayload = null;

        private void Awake()
        {
            stateMachine = new EmployeeStateMachine(this);
            agent = this.GetComponent<NavMeshAgent>();
            inventory = this.GetComponent<Inventory>();

            receiver.OnKeyframeReachedEvent += OnKeyframeReached;
        }

        private void OnKeyframeReached(int index)
        {
            if (TargetHarvestable != null)
            {
                TargetHarvestable._Damageable.CurrentHealth -= attackDamage;
            }
        }

        private void OnEnable()
        {
            State = EmployeeState.FindWork;
        }

        private void Update()
        {
            stateMachine.Tick();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick();
        }

        public float GetStoppingDistance()
        {
            return agent.stoppingDistance;
        }

        public List<PoolType> GetPoolTypeList()
        {
            return inventory.GetPoolTypeList();
        }

        public void SetDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }

        public void PlayWorkingAnimation(bool isOn)
        {
            animator.SetBool("IsStartMining", isOn);
            animator.SetTrigger("IsStartMiningStateChanged");
        }
    }
}
