using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public abstract class BaseDamageable : MonoBehaviour
    {
        [Header("=== Damageable ===")]
        [SerializeField]
        private Animation _animation;
        [SerializeField]
        private GameObject alivedObj;
        [SerializeField]
        private GameObject deadObj;

        [Space(10)]
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
                        IsDead = true;
                        OnDead();
                    }
                    else
                    {
                        OnDamaged();
                    }
                }
                else if (_currentHealth < value)
                {
                    _currentHealth = value;
                }
            }
        }

        public delegate void AlivedDelegate();
        public event AlivedDelegate OnAlivedEvent;

        public delegate void DamagedDelegate(float maxHealth, float currentHealth);
        public event DamagedDelegate OnDamagedEvent;

        public delegate void DeadDelegate();
        public event DeadDelegate OnDeadEvent;

        [HideInInspector]
        public bool IsDead = false;

        [Header("=== HealthbarSetting ===")]
        public float OffsetHeight = 0f;

        public virtual void Alive()
        {
            CurrentHealth = maxHealth;
            IsDead = false;

            alivedObj.SetActive(true);
            deadObj.SetActive(false);

            _animation.PlayQueued("OnAlived");
            OnAlivedEvent?.Invoke();
        }

        protected virtual void OnDamaged()
        {
            _animation.PlayQueued("OnDamaged");
            OnDamagedEvent?.Invoke(maxHealth, CurrentHealth);
        }

        protected virtual void OnDead()
        {
            OnDeadAsync().Forget();
            OnDeadEvent?.Invoke();
        }

        protected virtual async UniTaskVoid OnDeadAsync()
        {
            deadObj.SetActive(true);

            AnimationState state = _animation.PlayQueued("OnDead");
            await UniTask.WaitForSeconds(state.length);

            alivedObj.SetActive(false);
        }
    }
}