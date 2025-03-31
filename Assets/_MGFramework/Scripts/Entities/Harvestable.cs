using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class Harvestable : MonoBehaviour
    {
        private Damageable damageable;

        [Header("=== Harvestable ===")]
        [SerializeField]
        private GameObject alivedObj;
        [SerializeField]
        private GameObject deadObj;

        [Space(10)]
        [SerializeField]
        private PoolType outputPoolType;
        [SerializeField]
        private Vector2Int spawnCountRange = new Vector2Int(2, 5);
        [SerializeField]
        private Vector3 spawnOffset;
        [SerializeField]
        private float spawnRadius;
        [SerializeField]
        private float spawnLinearForce;
        [SerializeField]
        private float spawnAngularForce;

        [Space(10)]
        [SerializeField]
        private float regenerationDelay = 20f;

        private Collider[] colliders;
        private NavMeshObstacle obstacle;

        protected virtual void Awake()
        {
            damageable = this.GetComponent<Damageable>();

            alivedObj.SetActive(true);
            deadObj.SetActive(false);

            colliders = this.GetComponents<Collider>();
            obstacle = this.GetComponent<NavMeshObstacle>();

            damageable.OnAlivedEvent += OnAlived;
            damageable.OnDamagedEvent += OnDamaged;
            damageable.OnDeadEvent += OnDead;
            damageable.OnAfterDeadEvent += OnAfterDead;
        }

        public void OnAlived()
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
            obstacle.enabled = true;

            alivedObj.SetActive(true);
            deadObj.SetActive(false);
        }

        public void OnDamaged(float maxHealth, float currentHealth)
        {
        
        }

        public void OnDead()
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
            obstacle.enabled = false;

            int spawnCount = Random.Range(spawnCountRange.x, spawnCountRange.y);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomizedPos = this.transform.position + spawnOffset + (Random.insideUnitSphere * spawnRadius);

                outputPoolType.EnablePool(OnBeforePoolTargetItem);
                void OnBeforePoolTargetItem(GameObject obj)
                {
                    obj.transform.position = randomizedPos;
                    obj.transform.eulerAngles = new Vector3(Random.Range(0f, 360f),
                                                            Random.Range(0f, 360f),
                                                            Random.Range(0f, 360f));

                    Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
                    rigidbody.velocity = Random.insideUnitSphere * spawnLinearForce;
                    rigidbody.angularVelocity = Random.insideUnitSphere * spawnAngularForce;
                }
            }
        }

        public void OnAfterDead()
        {
            alivedObj.SetActive(false);
            deadObj.SetActive(true);

            OnAfterDeadAsync().Forget();
        }

        private async UniTaskVoid OnAfterDeadAsync()
        {
            await UniTask.WaitForSeconds(regenerationDelay);
            damageable.SetAlive();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position + spawnOffset, spawnRadius);
        }
    }
}