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
        private OccupiableFootstep occupiableFootstep;
        [SerializeField]
        private PurchasableFootstep purchasableFootstep;

        [Space(10)]
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

        public bool IsCanUse
        {
            get
            {
                bool purchasableCondition = true;
                if (purchasableFootstep != null)
                {
                    purchasableCondition = purchasableFootstep.HasPurchased == true;
                }

                return purchasableCondition == true;
            }
        }

        public Vector3 GetClosestInputStorePoint(Vector3 originPoint)
        {
            Vector3 nearestPoint = Vector3.zero;
            float nearestDistance = float.MaxValue;

            foreach (InputStackableStore store in inputStores)
            {
                float distance = Vector3.Distance(originPoint, store.GetClosestPoint(originPoint));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = store.GetClosestPoint(originPoint);
                }
            }

            if (nearestPoint == Vector3.zero)
            {
                Debug.Assert(false);
                return Vector3.zero;
            }

            return nearestPoint;
        }

        public Vector3 GetClosestOutputStorePoint(Vector3 originPoint)
        {
            Vector3 nearestPoint = Vector3.zero;
            float nearestDistance = float.MaxValue;

            foreach (OutputStackableStore store in outputStores)
            {
                float distance = Vector3.Distance(originPoint, store.GetClosestPoint(originPoint));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = store.GetClosestPoint(originPoint);
                }
            }

            if (nearestPoint == Vector3.zero)
            {
                Debug.Assert(false);
                return Vector3.zero;
            }

            return nearestPoint;
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

        public bool HasOutputStore()
        {
            foreach (OutputStackableStore outputStore in outputStores)
            {
                if (outputStore.HasOutput() == true)
                {
                    return true; 
                }
            }

            return false;
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
                inputStores.IsEachPoppable() == true &&
                outputStores.Length > 0)
            {
                // Output을 생성하기 위한 재료 소모
                PopEachInputStore();

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
            AddEachOutputStore();

            // 재귀호출
            OnPush();
        }

        public bool TryPopInputStore(PoolType poolType)
        {
            if (IsCanUse == false)
            {
                return false;
            }

            foreach (InputStackableStore store in inputStores)
            {
                if (store.IsPoppable(poolType) == false)
                {
                    return false;
                }
            }

            foreach (InputStackableStore store in inputStores)
            {
                store.Pop();
            }

            return true;
        }

        public void PopEachInputStore()
        {
            foreach (InputStackableStore store in inputStores)
            {
                store.Pop();
            }
        }

        public void AddEachOutputStore()
        {
            foreach (OutputStackableStore store in outputStores)
            {
                store.Add();
            }
        }
    }
}
