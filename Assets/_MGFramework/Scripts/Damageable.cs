using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class Damageable : MonoBehaviour
    {
        private Collider _collider;

        [SerializeField]
        private Animation _animation;

        [Space(10)]
        [SerializeField]
        private GameObject alivedObj;
        [SerializeField]
        private GameObject deadObj;

        [Space(10)]
        [SerializeField]
        private float maxHealth;

        [ReadOnly]
        [SerializeField]
        private float _currentHealth;
        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                if (_currentHealth > value)
                {
                    _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                    if (_currentHealth == 0f)
                    {
                        OnDead();
                    }
                    else
                    {
                        OnDamaged();
                    }
                }
                else
                {
                    _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                }
            }
        }

        public float CurrentNormal
        {
            get
            {
                return CurrentHealth / maxHealth;
            }
        }

        [ReadOnly]
        [SerializeField]
        private bool _isDead;
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            private set
            {
                _isDead = value;
            }
        }

        [Space(10)]
        [SerializeField]
        private float yOffset = 0f;

        public delegate void AlivedDelegate();
        public event AlivedDelegate OnAlivedEvent;

        public delegate void DamagedDelegate();
        public event DamagedDelegate OnDamagedEvent;

        public delegate void DeadDelegate();
        public event DeadDelegate OnDeadEvent;

        private bool isShownHealthbar = false;

        public Vector3 GetClosestPoint(Vector3 point)
        {
            return _collider.ClosestPoint(point);
        }

        private void Start()
        {
            _collider = this.GetComponent<Collider>();

            CurrentHealth = maxHealth;
            IsDead = false;

            if (alivedObj != null)
            {
                alivedObj.SetActive(true);
            }
            if (deadObj != null)
            {
                deadObj.SetActive(false);
            }
        }

        [ContextMenu("Alive")]
        private void Alive()
        {
            if (Application.isPlaying == true)
            {
                CurrentHealth = maxHealth;
                IsDead = false;

                if (alivedObj != null)
                {
                    alivedObj.SetActive(true);
                }
                if (deadObj != null)
                {
                    deadObj.SetActive(false);
                }

                OnAlivedEvent?.Invoke();

                _animation.Play("OnAlived");
            }
        }

        private void OnDamaged()
        {
            OnDamagedEvent?.Invoke();
            _animation.Play("OnDamaged");

            if (isShownHealthbar == false)
            {
                isShownHealthbar = true;
                this.EnableHealthbar(yOffset);
            }
        }

        private void OnDead()
        {
            if (IsDead == false)
            {
                IsDead = true;
                OnDeadEvent?.Invoke();

                AnimationState state = _animation.PlayQueued("OnDead");
                OnDeadAsync(state).Forget();

                if (isShownHealthbar == true)
                {
                    isShownHealthbar = false;
                }
            }
        }

        private async UniTaskVoid OnDeadAsync(AnimationState state)
        {
            if (deadObj != null)
            {
                deadObj.SetActive(true);
            }

            await UniTask.WaitWhile(() => state != null);

            if (alivedObj != null)
            {
                alivedObj.SetActive(false);
            }
        }
    }
}
