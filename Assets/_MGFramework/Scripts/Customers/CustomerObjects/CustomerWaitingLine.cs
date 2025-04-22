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
        /// 대기열 추가 가능 여부
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
                Debug.LogWarning("대기열이 가득 찼습니다.");
            }

            return isEnqueuable;
        }

        /// <summary>
        /// 맨 뒤에 손님을 대기열에 추가합니다.
        /// </summary>
        /// <param name="customer">추가할 손님</param>
        public void Enqueue(Customer customer)
        {
            customerList.Add(customer);
            UpdateAllCustomerPoses();
        }

        /// <summary>
        /// 대기열 제거 가능 여부
        /// </summary>
        /// <returns></returns>
        public bool IsDequeuable()
        {
            return customerList.Count > 0;
        }

        /// <summary>
        /// 맨 앞 손님을 대기열에서 제거합니다.
        /// </summary>
        /// <param name="customer">제거된 손님</param>
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
                Debug.LogWarning("대기열이 비어 있습니다.");
                customer = null;
            }

            return isDequeuable;
        }

        public bool IsFirstCustomer(Customer customer)
        {
            if (IsDequeuable() == false)
            {
                Debug.LogWarning("대기열이 비어 있습니다.");
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
                Debug.LogError("Interval은 0.01 이상이어야 합니다.");
                return;
            }
            else if (pointTransforms == null || pointTransforms.Length < 2)
            {
                Debug.LogWarning("pointTransforms 길이가 2 이상이어야 합니다.");
                return;
            }

            float totalLength = 0f;
            for (int i = 0; i < pointTransforms.Length - 1; i++)
            {
                totalLength += Vector3.Distance(pointTransforms[i].position, pointTransforms[i + 1].position);
            }

            float currentDistance = 0f;
            int currentCount = 0;

            // 경로에 따른 포인트 생성
            while (currentDistance <= totalLength && currentCount < maxCount)
            {
                Vector3 pointAtDistance = GetPointAtDistance(currentDistance);
                pointList.Add(pointAtDistance);
                currentDistance += interval;

                currentCount++;
            }

            // 확장 포인트 생성
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
        /// 현재 서 있는 위치를 바탕으로 i ~ i + 1 구간의 Transform을 반환합니다.
        /// </summary>
        /// <param name="position">현재 위치</param>
        /// <param name="tolerance">최대 거리 오차값</param>
        /// <returns>pointList의 i ~ i + 1 구간의 Transform</returns>
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
                Debug.LogError("해당 위치에서는 적합한 Transform을 찾을 수 없음.");
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
