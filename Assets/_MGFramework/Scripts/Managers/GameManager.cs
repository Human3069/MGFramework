using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MGFramework
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public CustomerWaitingLine WaitingLine;

        [Space(10)]
        public Transform CustomerEnablePoint;
        public Transform CustomerReturnPoint;

        [Space(10)]
        [SerializeField]
        private float returnInterval = 1f;
        [SerializeField]
        private float returnRadius = 10f;

        private Collider[] overlapColliders = new Collider[10];

        private void Awake()
        {
            CheckReturnCustomerAsync().Forget();
        }

        private async UniTaskVoid CheckReturnCustomerAsync()
        {
            while (this.enabled == true)
            {
                int overlapCount = Physics.OverlapSphereNonAlloc(CustomerReturnPoint.position, returnRadius, overlapColliders);
                for (int i = 0; i < overlapCount; i++)
                {
                    Collider overlapCollider = overlapColliders[i];
                    if (overlapCollider.TryGetComponent(out Customer customer) == true &&
                        customer.IsExiting() == true)
                    {
                        customer.gameObject.DisablePool(PoolType.Customer);
                    }
                }

                await UniTask.WaitForSeconds(returnInterval);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (CustomerEnablePoint == null || CustomerReturnPoint == null)
            {
                Debug.Log(CustomerEnablePoint == null || CustomerReturnPoint == null);
                // DestroyImmediate(this.gameObject);

                Selection.objects = new Object[] { this.gameObject };
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CustomerEnablePoint.position, 1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(CustomerReturnPoint.position, returnRadius);
        }
#endif
    }
}