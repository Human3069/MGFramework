using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Google.GData.Spreadsheets.ListEntry;

namespace MGFramework
{
    public class CustomerWaitingLine : MonoBehaviour
    {
        [SerializeField]
        private Customer customerPrefab;

        [Space(10)]
        [SerializeField]
        private Transform[] pointTransforms;
        [SerializeField]
        private float interval = 1f;
        [SerializeField]
        private int maxCount = 200;

        [Space(10)]
        [SerializeField]
        private Queue<Customer> customerQueue = new Queue<Customer>();

        private List<Vector3> pointList = new List<Vector3>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Customer customerInstance = Instantiate(customerPrefab, new Vector3(-56.85f, 21.9f, -2.64f), Quaternion.identity);
                Enqueue(customerInstance);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Customer dequeuedCustomer = Dequeue();
                dequeuedCustomer.SetDestination(new Vector3(-54f, 22f, 26f));
            }
        }

        private void Awake()
        {
            UpdatePointList();
        }

        /// <summary>
        /// 고객을 대기열에 추가합니다.
        /// </summary>
        /// <param name="customer"></param>
        public void Enqueue(Customer customer)
        {
            customerQueue.Enqueue(customer);
            AssignDestinationsAll();
        }

        /// <summary>
        /// 대기열에서 고객을 제거합니다.
        /// </summary>
        /// <returns></returns>
        public Customer Dequeue()
        {
            if (customerQueue.TryDequeue(out Customer customer) == false)
            {
                Debug.LogError("대기열에서 고객을 제거할 수 없습니다.");
                return null;
            }
            else
            {
                AssignDestinationsAll();
                return customer;
            }
        }

        /// <summary>
        /// 각 고객의 목적지를 지정합니다.
        /// </summary>
        private void AssignDestinationsAll()
        {
            List<Customer> customerList = new List<Customer>(customerQueue);
            for (int i = 0; i < customerList.Count; i++)
            {
                // 목적지 지정
                customerList[i].SetDestination(pointList[i]);

                // 방향벡터 구하기
                int segmentIndex = GetSegmentIndexBetween(pointList[i]);
                if (segmentIndex >= 0 && segmentIndex < pointList.Count - 1)
                {
                    Vector3 from = pointList[segmentIndex];
                    Vector3 to = pointList[segmentIndex + 1];
                    Vector3 direction = -(to - from).normalized;

                    customerList[i].SetDirection(direction);
                }
            }
        }

        private void UpdatePointList()
        {
            if (pointList == null)
            {
                pointList = new List<Vector3>();
            }
            pointList.Clear();

            if (interval < 0.01f)
            {
                Debug.LogError("interval = 0.01 보다 작을 수 없습니다.");
                return;
            }
            else if (pointTransforms == null || pointTransforms.Length < 2)
            {
                Debug.LogError("pointTransforms 에 최소한 3개를 지정해야 합니다.");
                return;
            }

            // 총 경로 길이 계산
            float totalLength = 0f;
            for (int i = 0; i < pointTransforms.Length - 1; i++)
            {
                totalLength += Vector3.Distance(pointTransforms[i].position, pointTransforms[i + 1].position);
            }

            int currentCount = 0;
            float currentDistance = 0f;

            // 1. 경로 위에서 최대한 포인트 추출
            while (currentCount < maxCount && currentDistance < totalLength)
            {
                Vector3 point = GetPointAtDistance(currentDistance);
                pointList.Add(point);

                currentDistance += interval;
                currentCount++;
            }

            // 2. 부족한 개수는 마지막 포인트에서 forward 방향으로 연장
            Transform lastTransform = pointTransforms[pointTransforms.Length - 1]; // C# 8.0 이상
            Vector3 startPos = lastTransform.position;
            Vector3 direction = -lastTransform.forward;

            for (int i = 0; currentCount < maxCount; i++, currentCount++)
            {
                Vector3 extendedPoint = startPos + direction * interval * (i + 1);
                pointList.Add(extendedPoint);
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
                    float normal = (distance - accumulated) / segmentLength;
                    Vector3 normalizedPoint = Vector3.Lerp(from, to, normal);

                    return normalizedPoint;
                }

                accumulated += segmentLength;
            }

            Vector3 lastPoint = pointTransforms[pointTransforms.Length - 1].position;
            return lastPoint;
        }

        /// <summary>
        /// 주어진 위치가 pointList의 어느 선분 구간 안에 있는지 찾습니다.
        /// </summary>
        /// <param name="tolerance">오차 허용 범위</param>
        /// <returns>선분 인덱스 (i ~ i+1 사이면 i를 반환), 찾지 못하면 -1</returns>
        private int GetSegmentIndexBetween(Vector3 position, float tolerance = 0.5f)
        {
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                Vector3 a = pointList[i];
                Vector3 b = pointList[i + 1];
                float segLen = Vector3.Distance(a, b);

                // 점과 선분 사이의 거리 계산
                float distToSegment = DistancePointToSegment(position, a, b);

                if (distToSegment <= tolerance)
                {
                    return i; // position은 i~i+1 사이에 있음
                }
            }

            return -1; // 어느 구간에도 포함되지 않음
        }

        private float DistancePointToSegment(Vector3 point, Vector3 a, Vector3 b)
        {
            Vector3 ap = point - a;
            Vector3 ab = b - a;

            float magnitudeAB = ab.sqrMagnitude;
            float dot = Vector3.Dot(ap, ab);
            float normal = Mathf.Clamp01(dot / magnitudeAB);
            Vector3 projection = a + ab * normal;

            return Vector3.Distance(point, projection);
        }

#if UNITY_EDITOR
        [ContextMenu("Attach Points")]
        private void AttachPointTransforms()
        {
            pointTransforms = new Transform[this.transform.childCount];
            for (int i = 0; i < this.transform.childCount; i++)
            {
                pointTransforms[i] = this.transform.GetChild(i);
            }
        }

        private void OnValidate()
        {
            UpdatePointList();
        }

        private void OnDrawGizmosSelected()
        {
            if (pointList == null || pointList.Count == 0)
            {
                UpdatePointList();
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < pointTransforms.Length; i++)
            {
                if (i > 0)
                {
                    Gizmos.DrawLine(pointTransforms[i - 1].position, pointTransforms[i].position);
                }
            }

            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            if (interval > 0f)
            {
                foreach (Vector3 point in pointList)
                {
                    Gizmos.DrawSphere(point, 0.5f);
                }
            }
        }
#endif
    }
}