using UnityEngine;

namespace MGFramework
{
    public enum OwnerType
    {
        None = -1,

        Players,
        Enemys,
        Neutral
    }

    public class Damageable : MonoBehaviour
    {
        private OwnerType ownerType = OwnerType.None;
        private Collider _collider;

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
            private set
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

        public void TakeDamage(float damage, Damageable attackerDamageable)
        {
            CurrentHealth -= damage;
            OnDamagedWithDataEvent?.Invoke(attackerDamageable);
        }

        public void UpdateMaxHealth(float health)
        {
            maxHealth = health;
            CurrentHealth = health;
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

        public delegate void DamagedWithDataDelegate(Damageable attackerDamageable);
        public event DamagedWithDataDelegate OnDamagedWithDataEvent;

        public delegate void DeadDelegate();
        public event DeadDelegate OnDeadEvent;

        private bool isShownHealthbar = false;
        private bool _isEntered = false; // 누군가 점유중일 때 true
        public bool IsEntered
        {
            get
            {
                return _isEntered;
            }
            private set
            {
                _isEntered = value;
            }
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            return _collider.ClosestPoint(point);
        }

        public OwnerType GetOwnerType()
        {
            return ownerType;
        }

        private void Awake()
        {
            MonoBehaviour[] monoBehaviours = this.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is Player or
                                     Employee or
                                     Hunter)
                {
                    ownerType = OwnerType.Players;
                    break;
                }
                if (monoBehaviour is Harvestable)
                {
                    ownerType = OwnerType.Neutral;
                    break;
                }
                else if (monoBehaviour is Monster)
                {
                    ownerType = OwnerType.Enemys;
                    break;
                }
            }

            Debug.Assert(ownerType != OwnerType.None);
        }

        private void Start()
        {
            _collider = this.GetComponent<Collider>();

            CurrentHealth = maxHealth;
            IsDead = false;
        }

        [ContextMenu("Alive")]
        public void Alive()
        {
            if (Application.isPlaying == true)
            {
                CurrentHealth = maxHealth;
                IsDead = false;

                OnAlivedEvent?.Invoke();
            }
        }

        private void OnDamaged()
        {
            OnDamagedEvent?.Invoke();
          
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

                if (isShownHealthbar == true)
                {
                    isShownHealthbar = false;
                }
            }
        }

        public void Enter()
        {
            IsEntered = true;
        }

        public void Exit()
        {
            IsEntered = false;
        }
    }
}
