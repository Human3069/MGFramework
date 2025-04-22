using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class CustomerManagement 
    {
        public CustomerWaitingLine WaitingLine;

        [Space(10)]
        public Transform CustomerEnablePoint;
        public Transform CustomerReturnPoint;

        [Space(10)]
        [SerializeField]
        private float returnInterval = 1f;
        [SerializeField]
        private float returnRadius = 2.5f;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        private float enqueueInterval; // ��⿭ �մ� ���� ���� 1 ~ 100���� �þ.
        [SerializeField]
        private float enqueueIntervalBending = 1.5f; // 1 => ���� ����, ���ڰ� ���� ���� ���ϱ޼������� ����

        private GameManager _manager;
        private Collider[] overlapColliders = new Collider[10];

        private CancellationTokenSource tokenSource;

        public void OnAwake(GameManager manager)
        {
            this._manager = manager;
            tokenSource = new CancellationTokenSource();

            CheckReturnCustomerAsync().Forget();
            EnqueueCustomerAsync(tokenSource.Token).Forget();

            WaitingLine.OnCustomerEmpty += OnCustomerEmpty;
        }

        private void OnCustomerEmpty()
        {
            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            EnqueueCustomerAsync(tokenSource.Token).Forget();
        }

        private async UniTaskVoid CheckReturnCustomerAsync()
        {
            while (_manager.enabled == true)
            {
                int layerMask = ~(1 << 3);
                int overlapCount = Physics.OverlapSphereNonAlloc(CustomerReturnPoint.position, returnRadius, overlapColliders, layerMask);
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

        /// <summary>
        /// ���� ��⿭ �մ� ���� ���� enqueueInterval�� �����Ͽ� �մ��� ��⿭�� �߰��մϴ�.
        /// �մ��� 0���̶��, �ʹ� ���� ����ϴ� ���� �����ϱ� ���� ������մϴ�.
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid EnqueueCustomerAsync(CancellationToken token)
        {
            while (_manager.enabled == true)
            {
                float powered = Mathf.Pow(WaitingLine.CountNormal + 1, enqueueIntervalBending);
                enqueueInterval = powered;

                await UniTask.WaitForSeconds(enqueueInterval, cancellationToken: token);

                EnqueueCustomer();
            }
        }

        public void EnqueueCustomer()
        {
            PoolType.Customer.EnablePool(OnBeforeEnablePool);
            void OnBeforeEnablePool(GameObject customerObj)
            {
                Transform enableTransform = GameManager.Instance.CustomerManager.CustomerEnablePoint;
                customerObj.transform.position = enableTransform.position;
                customerObj.transform.forward = enableTransform.forward;

                Customer customer = customerObj.GetComponent<Customer>();
                CustomerWaitingLine waitingLine = GameManager.Instance.CustomerManager.WaitingLine;

                if (waitingLine.TryEnqueue(customer) == false)
                {
                    customerObj.DisablePool(PoolType.Customer);
                }
            }
        }

#if UNITY_EDITOR
        public void DrawGizmosSelected()
        {
            if (CustomerEnablePoint == null || CustomerReturnPoint == null)
            {
                Debug.Log(CustomerEnablePoint == null || CustomerReturnPoint == null);
                // DestroyImmediate(this.gameObject);

                Selection.objects = new Object[] { _manager.gameObject };
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CustomerEnablePoint.position, 1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(CustomerReturnPoint.position, returnRadius);
        }
#endif
    }
}
