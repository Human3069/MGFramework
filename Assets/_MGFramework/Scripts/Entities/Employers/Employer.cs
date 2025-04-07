using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public enum EmployerState
    {
        None = -1,

        Seek,
        Chop,
        CarryItem,
        DropItem,
    }

    public class Employer : MonoBehaviour, IInventory
    {
        // private const string LOG_FORMAT = "<color=white><b>[Employer]</b></color> {0}";

        [SerializeField]
        private KeyframeReceiver receiver;

        [Space(10)]
        [SerializeField]
        private Inventory _inventory;
        public Inventory Inventory => _inventory;

        [SerializeField]
        private EmployerData employerData;
     
        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private EmployerState _employerState = EmployerState.None;
        public EmployerState _EmployerState
        {
            get
            {
                return _employerState;
            }
            set
            {
                if (_employerState != value)
                {
                    IEmployerState targetState = null;
                    if (value == EmployerState.Seek)
                    {
                        targetState = new SeekEmployerState(employerData);
                    }
                    else if (value == EmployerState.Chop)
                    {
                        targetState = new ChopEmployerState(employerData);
                    }
                    else if (value == EmployerState.CarryItem)
                    {
                        targetState = new CarryItemEmployerState(employerData);
                    }
                    else if (value == EmployerState.DropItem)
                    {
                        targetState = new DropItemEmployerState(employerData);
                    }

                    stateMachine.ChangeState(targetState);
                    _employerState = value;
                }
            }
        }

        [ReadOnly]
        [SerializeField]
        private Harvestable _targetHarvestable = null;
        public Harvestable TargetHarvestable
        {
            get
            {
                return _targetHarvestable;
            }
            set
            {
                _targetHarvestable = value;
            }
        }

        private EmployerStateMachine stateMachine;

        private bool _isMoving = false;
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

                    employerData._Animator.SetBool("IsMoving", value);
                    employerData._Animator.SetTrigger("IsMovingStateChanged");
                }
            }
        }

        private void Awake()
        {
            this.employerData._Inventory = Inventory;
            this.stateMachine = new EmployerStateMachine();
            _EmployerState = EmployerState.Seek;

            receiver.OnKeyframeReachedEvent += OnKeyframeReached;

            AwakeAsync().Forget();
        }

        private async UniTask AwakeAsync()
        {
            while (this.gameObject.activeSelf == true)
            {
                stateMachine.SlowTick();

                await UniTask.WaitForSeconds(0.5f);
            }

            stateMachine.ChangeState(null);
        }

        private void OnKeyframeReached(int index)
        {
            if (index == 0)
            {
                if (TargetHarvestable != null &&
                    TargetHarvestable.IsHarvestable == true)
                {
                    TargetHarvestable._Damageable.CurrentHealth -= employerData._HandlingDamage;
                }
            }
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick();

            // 스탑 상태인지 확인
            float stopDistance = employerData._Agent.stoppingDistance;
            float distance = Vector3.Distance(employerData._Transform.position, employerData._Agent.destination);
            IsMoving = distance > stopDistance;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Item item) == true &&
                item.IsOnInventory == false &&
                item.IsFading == false)
            {
                Inventory.TryPush(item);
            }
        }
    }
}