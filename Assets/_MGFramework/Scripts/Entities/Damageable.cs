using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField]
        private Animation _animation;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private bool _isDead = false;
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

        [SerializeField]
        private float maxHealth = 100f;
        [ReadOnly]
        [SerializeField]
        private float _currentHealth = 0f;
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
                        Dead();
                    }
                    else
                    {
                        Damaged();
                    }
                }
                else if (_currentHealth < value)
                {
                    _currentHealth = value;
                    if (IsDead == true)
                    {
                        Alive();
                    }
                    else
                    {
                        // Èú Àû¿ë
                    }
                }
            }
        }

        [Space(10)]
        public float OffsetHeight;

        public delegate void AlivedDelegate();
        public AlivedDelegate OnAlivedEvent;

        public delegate void DamagedDelegate(float maxHealth, float currentHealth);
        public DamagedDelegate OnDamagedEvent;

        public delegate void DeadDelegate();
        public DeadDelegate OnDeadEvent;

        public delegate void AfterDeadDelegate();
        public AfterDeadDelegate OnAfterDeadEvent;

        private void Awake()
        {
            IsDead = false;
            CurrentHealth = maxHealth;
        }

        public void SetAlive()
        {
            CurrentHealth = maxHealth;
        }

        private void Alive()
        {
            IsDead = false;

            _animation.PlayQueued("OnAlived");
            OnAlivedEvent?.Invoke();
        }

        private void Damaged()
        {
            _animation.PlayQueued("OnDamaged");
            OnDamagedEvent?.Invoke(maxHealth, CurrentHealth);
        }

        private void Dead()
        {
            IsDead = true;

            OnDeadEvent?.Invoke();
            _animation.PlayQueued("OnDead");
       
            DeadAsync().Forget();
        }

        private async UniTaskVoid DeadAsync()
        {
            await UniTask.WaitWhile(() => _animation.isPlaying == true);
            OnAfterDeadEvent?.Invoke();
        }
    }
}