using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public abstract class EnterableIngredient : BaseEnterable
    {
        protected Collider[] colliders;

        [SerializeField]
        protected Animation _animation;
        [SerializeField]
        protected Transform itemInstantiatePoint;

        [Space(10)]
        [SerializeField]
        protected GameObject aliveObj;
        [SerializeField]
        protected GameObject deadObj;

        [Space(10)]
        [SerializeField]
        protected Vector2Int instantiateOnDeadRange = new Vector2Int(2, 5);
        public float instantiateRandomSphereRadius = 1f;

        [Space(10)]
        [SerializeField]
        protected float maxHealth;
        [ReadOnly]
        [SerializeField]
        protected float _currentHealth;
        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                if (isAlive == true)
                {
                    if (_currentHealth > value)
                    {
                        _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                        if (_currentHealth == 0f)
                        {
                            OnDeadAsync().Forget();
                        }
                        else
                        {
                            OnDamaged();
                        }
                    }
                    else
                    {
                        _currentHealth = value;
                    }
                }
            }
        }

        protected PlayerController currentController = null;
        protected bool isAlive = true;

        protected virtual void Awake()
        {
            colliders = this.GetComponents<Collider>();
        }

        protected virtual void OnEnable()
        {
            isAlive = true;

            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            CurrentHealth = maxHealth;

            aliveObj.SetActive(true);
            deadObj.SetActive(false);
        }

        protected virtual void OnDamaged()
        {
            _animation.PlayQueued("OnDamage");
        }

        protected virtual async UniTaskVoid OnDeadAsync()
        {
            isAlive = false;
            int instantiateCount = Random.Range(instantiateOnDeadRange.x, instantiateOnDeadRange.y);

            for (int i = 0; i < instantiateCount; i++)
            {
                float randomX = Random.Range(-180f, 180f);
                float randomY = Random.Range(-180f, 180f);
                float randomZ = Random.Range(-180f, 180f);
                Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

                Vector3 randomPoint = (Random.insideUnitSphere * instantiateRandomSphereRadius) + itemInstantiatePoint.position;
                ObjectPoolManager.Instance.TakeOutObj(PoolerType.Item_Tree, randomPoint, randomRotation);
            }
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            currentController.OnExitAsync().Forget();

            deadObj.SetActive(true);
            AnimationState state = _animation.PlayQueued("OnDisable");
            await UniTask.WaitForSeconds(state.length);
            aliveObj.SetActive(false);
        }

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<PlayerController>(out PlayerController controller) == true)
            {
                currentController = controller;
                currentController.OnEnter(this);
            }
        }

        protected override void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<PlayerController>(out _) == true)
            {
                currentController.OnExitAsync().Forget();
            }
        }
    }
}