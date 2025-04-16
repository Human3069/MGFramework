using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// Input을 통해 Output을 생성하는 컴포넌트입니다.
    /// </summary>
    public class Payload : MonoBehaviour, ITimer
    {
        [SerializeField]
        private InputStackableStore[] inputStores;
        [SerializeField]
        private OutputStackableStore[] outputStores;

        [Space(10)]
        [SerializeField]
        private float generateSpeed = 10f;

        [Header("ITimer")]
        [SerializeField]
        private float yOffset = 0f;

        private bool isGeneratingOutput = false;

        protected float _normalizedTime = 0f;
        public float NormalizedTime
        {
            get
            {
                return _normalizedTime;
            }
            set
            {
                _normalizedTime = value;
            }
        }

        public List<PoolType> GetEnterablePoolTypeList(List<PoolType> poolTypeList)
        {
            List<PoolType> includePoolTypeList = new List<PoolType>();
            foreach (InputStackableStore inputStore in inputStores)
            {
                PoolType storePoolType = inputStore.GetPoolType();
                if (poolTypeList.Contains(storePoolType) == true)
                {
                    includePoolTypeList.Add(storePoolType);
                }
            }

            return includePoolTypeList;
        }

        public bool IsEnterablePoolTypeList(List<PoolType> poolTypeList)
        {
            List<PoolType> enterablePoolTypeList = GetEnterablePoolTypeList(poolTypeList);
            return enterablePoolTypeList.Count > 0;
        }

        public Vector3 GetClosestPoint(Vector3 originPoint)
        {
            float nearestDistance = float.MaxValue;
            Vector3 nearestPoint = Vector3.zero;

            foreach (InputStackableStore inputStore in inputStores)
            {
                Vector3 point = inputStore.GetClosestPoint(originPoint);

                float distance = Vector3.Distance(originPoint, point);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = point;
                }
            }

            foreach (OutputStackableStore outputStore in outputStores)
            {
                Vector3 point = outputStore.GetClosestPoint(originPoint);

                float distance = Vector3.Distance(originPoint, point);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        private void Awake()
        {
            foreach (InputStackableStore inputStore in inputStores)
            {
                inputStore.OnPush += OnPush;
            }
        }

        private void OnPush()
        {
            // Output을 생성할 수 있는지 체크.
            if (isGeneratingOutput == false &&
                inputStores.IsAllPoppable() == true &&
                outputStores.Length > 0)
            {
                // Output을 생성하기 위한 재료 소모
                inputStores.PopAll();

                // 생성 비동기 실행
                GenerateOutputAsync().Forget();
            }
        }

        private async UniTaskVoid GenerateOutputAsync()
        {
            isGeneratingOutput = true;

            float time = 0f;
            NormalizedTime = 0f;

            // 재료 만들 타이머 생성
            this.EnableTimer(yOffset);
            while (time < generateSpeed)
            {
                await UniTask.Yield();

                time += Time.deltaTime;
                NormalizedTime = time / generateSpeed;
            }

            NormalizedTime = 1f;
            isGeneratingOutput = false;

            // Output 생성
            outputStores.AddAll();

            // 재귀호출
            OnPush();
        }
    }
}
