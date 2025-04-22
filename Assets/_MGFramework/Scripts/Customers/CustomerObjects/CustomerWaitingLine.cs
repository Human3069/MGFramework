using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CustomerWaitingLine : MonoBehaviour
    {
        public Payload CounterPayload;

        [SerializeField]
        private Transform[] pointTransforms;
        
        [Space(10)]
        [SerializeField]
        private float interval = 1f;
        [SerializeField]
        private int maxCount = 200;

        private List<Vector3> pointList = new List<Vector3>();
        private List<Customer> customerList = new List<Customer>();

        public delegate void CustomerEmptyDelegate();
        public event CustomerEmptyDelegate OnCustomerEmpty;

        public int Count
        {
            get
            {
                return customerList.Count;
            }
        }

        public float CountNormal
        {
            get
            {
                return (float)customerList.Count / maxCount;
            }
        }

        /// <summary>
        /// ��⿭ �߰� ���� ����
        /// </summary>
        /// <returns></returns>
        public bool IsEnqueuable()
        {
            return customerList.Count < maxCount;
        }

        public bool TryEnqueue(Customer customer)
        {
            bool isEnqueuable = IsEnqueuable();
            if (isEnqueuable == true)
            {
                Enqueue(customer);
            }
            else
            {
                Debug.LogWarning("��⿭�� ���� á���ϴ�.");
            }

            return isEnqueuable;
        }

        /// <summary>
        /// �� �ڿ� �մ��� ��⿭�� �߰��մϴ�.
        /// </summary>
        /// <param name="customer">�߰��� �մ�</param>
        public void Enqueue(Customer customer)
        {
            customerList.Add(customer);
            UpdateAllCustomerPoses();
        }

        /// <summary>
        /// ��⿭ ���� ���� ����
        /// </summary>
        /// <returns></returns>
        public bool IsDequeuable()
        {
            return customerList.Count > 0;
        }

        /// <summary>
        /// �� �� �մ��� ��⿭���� �����մϴ�.
        /// </summary>
        /// <param name="customer">���ŵ� �մ�</param>
        public void Dequeue(out Customer customer)
        {
            customer = customerList[0];
            customerList.Remove(customer);

            UpdateAllCustomerPoses();
        }

        public bool TryDequeue(out Customer customer)
        {
            bool isDequeuable = IsDequeuable();
            if (isDequeuable == true)
            {
                Dequeue(out customer);
            }
            else
            {
                Debug.LogWarning("��⿭�� ��� �ֽ��ϴ�.");
                customer = null;
            }

            return isDequeuable;
        }

        public bool IsFirstCustomer(Customer customer)
        {
            if (IsDequeuable() == false)
            {
                Debug.LogWarning("��⿭�� ��� �ֽ��ϴ�.");
                return false;
            }

            return customerList[0] == customer;
        }

        private void UpdateAllCustomerPoses()
        {
            if (customerList.Count == 0)
            {
                OnCustomerEmpty?.Invoke();
            }
            else
            {
                foreach (Customer customer in customerList)
                {
                    int index = customerList.IndexOf(customer);

                    Vector3 position = pointList[index];
                    Transform segmentTransform = GetTransformBetween(position);
                    Vector3 direction = segmentTransform.forward;

                    customer.UpdatePoseAsync(position, direction).Forget();
                }
            }
        }

        public void UpdatePointList()
        {
            pointList.Clear();

            if (interval < 0.01f)
            {
                Debug.LogError("Interval�� 0.01 �̻��̾�� �մϴ�.");
                return;
            }
            else if (pointTransforms == null || pointTransforms.Length < 2)
            {
                Debug.LogWarning("pointTransforms ���̰� 2 �̻��̾�� �մϴ�.");
                return;
            }

            float totalLength = 0f;
            for (int i = 0; i < pointTransforms.Length - 1; i++)
            {
                totalLength += Vector3.Distance(pointTransforms[i].position, pointTransforms[i + 1].position);
            }

            float currentDistance = 0f;
            int currentCount = 0;

            // ��ο� ���� ����Ʈ ����
            while (currentDistance <= totalLength && currentCount < maxCount)
            {
                Vector3 pointAtDistance = GetPointAtDistance(currentDistance);
                pointList.Add(pointAtDistance);
                currentDistance += interval;

                currentCount++;
            }

            // Ȯ�� ����Ʈ ����
            Transform lastPointTransform = pointTransforms[pointTransforms.Length - 1];
            Vector3 startPoint = lastPointTransform.position;
            Vector3 direction = -lastPointTransform.forward;

            currentCount++;
            int currentPointCount = pointList.Count;
            while (currentCount < maxCount + 1)
            {
                Vector3 extendedPoint = startPoint + direction * interval * (currentCount - currentPointCount);
                pointList.Add(extendedPoint);

                currentCount++;
            }
        }

        private Vector3 GetPointAtDistance(float distance)
        {
            float accumulated = 0f;

            for (int i = 0; i < pointTransforms.Length - 1; i++)
            {
                Vector3 from = pointTransforms[i].position;
                Vector3 to = pointTransforms[i + 1].position;
                float segmentLength = Vector3.Distance(from, to);

                if (accumulated + segmentLength >= distance)
                {
                    float t = (distance - accumulated) / segmentLength;
                    return Vector3.Lerp(from, to, t);
                }

                accumulated += segmentLength;
            }

            return pointTransforms[pointTransforms.Length - 1].position;
        }

        /// <summary>
        /// ���� �� �ִ� ��ġ�� �������� i ~ i + 1 ������ Transform�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="position">���� ��ġ</param>
        /// <param name="tolerance">�ִ� �Ÿ� ������</param>
        /// <returns>pointList�� i ~ i + 1 ������ Transform</returns>
        private Transform GetTransformBetween(Vector3 position, float tolerance = 0.5f)
        {
            int segmentIndex = -1;
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                Vector3 a = pointList[i];
                Vector3 b = pointList[i + 1];

                float distance = DistancePointToSegment(position, a, b);
                if (distance <= tolerance)
                {
                    segmentIndex = i;
                }
            }

            if (segmentIndex == -1)
            {
                Debug.LogError("�ش� ��ġ������ ������ Transform�� ã�� �� ����.");
                return null;
            }
            else if (segmentIndex >= pointTransforms.Length)
            {
                return pointTransforms[pointTransforms.Length - 1];
            }
            else
            {
                return pointTransforms[segmentIndex];
            }
        }

        private float DistancePointToSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ap = p - a;
            Vector3 ab = b - a;
            float abSqr = ab.sqrMagnitude;
            float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / abSqr);
            Vector3 projection = a + ab * t;
            return Vector3.Distance(p, projection);
        }

#if UNITY_EDITOR
        [ContextMenu("Attach Child Points")]
        private void AttachChildPoints()
        {
            pointTransforms = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                pointTransforms[i] = transform.GetChild(i);
            }

            UpdatePointList();
        }

        private void OnValidate()
        {
            UpdatePointList();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < pointTransforms.Length - 1; i++)
            {
                Gizmos.DrawLine(pointTransforms[i].position, pointTransforms[i + 1].position);
            }

            foreach (Vector3 point in pointList)
            {
                Gizmos.DrawSphere(point, 0.15f);
            }
        }
#endif
    }
}
