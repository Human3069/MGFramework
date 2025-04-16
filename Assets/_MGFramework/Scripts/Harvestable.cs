using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    public class Harvestable : MonoBehaviour
    {
        private Collider _collider;

        public Damageable _Damageable;
        [SerializeField]
        private Animation _animation;

        [Space(10)]
        [SerializeField]
        private GameObject alivedObj;
        [SerializeField]
        private GameObject deadObj;

        [Space(10)]
        [SerializeField]
        private PoolType dropPoolType;
        [SerializeField]
        private Vector3 dropItemOffset;
        [SerializeField]
        private float dropItemRadius;
        [SerializeField, MinMaxSlider(0, 10)]
        private Vector2Int dropCountRange;

        [Space(10)]
        [SerializeField, MinMaxSlider(0f, 120f)]
        private Vector2 rebirthTimeRange;

        private void Awake()
        {
            _Damageable = this.GetComponent<Damageable>();
            _collider = this.GetComponent<Collider>();

            _Damageable.OnAlivedEvent += OnAlived;
            _Damageable.OnDamagedEvent += OnDamaged;
            _Damageable.OnDeadEvent += OnDead;
        }

        private void Start()
        {
            if (alivedObj != null)
            {
                alivedObj.SetActive(true);
            }

            if (deadObj != null)
            {
                deadObj.SetActive(false);
            }
        }

        private void OnAlived()
        {
            _collider.enabled = true;

            if (alivedObj != null)
            {
                alivedObj.SetActive(true);
            }
            if (deadObj != null)
            {
                deadObj.SetActive(false);
            }

            _animation.Play("OnAlived");
        }

        private void OnDamaged()
        {
            _animation.Play("OnDamaged");
        }

        private void OnDead()
        {
            AnimationState state = _animation.PlayQueued("OnDead");
            OnDeadAsync(state).Forget();
            CountRebirthAsync().Forget();
        }

        private async UniTaskVoid OnDeadAsync(AnimationState state)
        {
            if (deadObj != null)
            {
                deadObj.SetActive(true);
            }

            _collider.enabled = false;
            int dropCount = Random.Range(dropCountRange.x, dropCountRange.y + 1);
            for (int i = 0; i < dropCount; i++)
            {
                dropPoolType.EnablePool(OnBeforeEnablePool);
                void OnBeforeEnablePool(GameObject obj)
                {
                    Vector3 startPos = this.transform.position + dropItemOffset;
                    Vector3 randomizedSphere = Random.insideUnitSphere * dropItemRadius;
                    Vector3 randomizedPos = startPos + randomizedSphere;
                    obj.transform.position = randomizedPos;

                    Vector3 randomizedEuler = Random.insideUnitSphere * 360f;
                    obj.transform.eulerAngles = randomizedEuler;
                }
            }

            await UniTask.WaitWhile(() => state != null);

            if (alivedObj != null)
            {
                alivedObj.SetActive(false);
            }
        }

        private async UniTaskVoid CountRebirthAsync()
        {
            float rebirthTime = Random.Range(rebirthTimeRange.x, rebirthTimeRange.y);
            await UniTask.WaitForSeconds(rebirthTime);

            _Damageable.Alive();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position + dropItemOffset, dropItemRadius);
        }
    }
}
