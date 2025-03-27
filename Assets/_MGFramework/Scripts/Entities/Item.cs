using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class Item : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Collider _collider;

        public ItemType _ItemType;
        [SerializeField]
        private Animation _animation;

        public bool IsFading = false;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private bool _isOnInventory = false;
        public bool IsOnInventory
        {
            get
            {
                return _isOnInventory;
            }
            set
            {
                if (_isOnInventory != value)
                {
                    _isOnInventory = value;
                    _rigidbody.isKinematic = value;
                    _collider.enabled = !value;
                }
            }
        }

        protected virtual void Awake()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
            _collider = this.GetComponent<Collider>();
        }

        protected virtual void OnEnable()
        {
            OnEnableAsync().Forget();
        }

        protected virtual async UniTaskVoid OnEnableAsync()
        {
            IsFading = true;
            _animation.PlayQueued("OnEnable", QueueMode.PlayNow);

            await UniTask.WaitWhile(() => _animation.isPlaying == true);
            IsFading = false;
        }

        public virtual async UniTaskVoid DisableAsync(Vector3? suctionPoint = null, bool isImmediately = false)
        {
            if (isImmediately == true)
            {
                if (_animation.isPlaying == true)
                {
                    _animation.Stop();
                }
            }
            else
            {
                await UniTask.WaitWhile(() => _animation.isPlaying == true);
            }
            
            IsFading = true;
            AnimationState state = _animation.PlayQueued("OnDisable", QueueMode.CompleteOthers);

            float timer = state.length;
            while (timer > 0f)
            {
                if (suctionPoint.HasValue == true)
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, suctionPoint.Value, Time.deltaTime);
                }

                timer -= Time.deltaTime;
                await UniTask.Yield();
            }

            IsFading = false;

            this.gameObject.ReturnPool(_ItemType);
        }
    }
}