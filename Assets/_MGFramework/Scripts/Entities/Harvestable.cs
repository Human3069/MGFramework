using _KMH_Framework.Pool;
using UnityEngine;

namespace MGFramework
{
    public class Harvestable : BaseDamageable
    {
        [Header("=== Harvestable ===")]
        [SerializeField]
        private ItemType targetItem;
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

        private Collider[] colliders;

        protected override void Awake()
        {
            base.Awake();
            colliders = this.GetComponents<Collider>();
        }

        [ContextMenu("Alive")]
        public override void Alive()
        {
            base.Alive();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }

        protected override void OnDead()
        {
            base.OnDead();

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            int spawnCount = Random.Range(spawnCountRange.x, spawnCountRange.y);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomizedPos = this.transform.position + spawnOffset + (Random.insideUnitSphere * spawnRadius);

                targetItem.EnablePool(OnBeforePoolTargetItem);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position + spawnOffset, spawnRadius);
        }
    }
}