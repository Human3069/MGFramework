using _KMH_Framework.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _KMH_Framework
{
    public class PoolableManager : MonoSingleton<PoolableManager>
    {
        private const string LOG_FORMAT = "<color=white><b>[PoolableManager]</b></color> {0}";

        [SerializeField]
        private List<PoolableScriptableObject> poolableList = new List<PoolableScriptableObject>();
        [SerializeField]
        private bool isAllowOverflow = false;

        private Dictionary<PoolType, Poolable> poolableDic = new Dictionary<PoolType, Poolable>();
        private Dictionary<PoolType, Queue<GameObject>> queueDic = new Dictionary<PoolType, Queue<GameObject>>();
        private Dictionary<PoolType, Transform> parentDic = new Dictionary<PoolType, Transform>();

        private void Awake()
        {
            foreach (PoolableScriptableObject poolableSO in poolableList)
            {
                RegisterPool(poolableSO);
            }
        }

        public void RegisterPool(PoolableScriptableObject poolableSO)
        {
            PoolType poolableType = poolableSO.poolable.type;
            if (queueDic.ContainsKey(poolableType) == true)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "이미 등록되어 있는 타입 : " + poolableType);
                return;
            }

            GameObject parentObj = new GameObject("Poolable_" + poolableType);
            Transform parentT = parentObj.transform;
            parentT.parent = this.transform;

            poolableDic.Add(poolableType, poolableSO.poolable);
            queueDic.Add(poolableType, new Queue<GameObject>());
            parentDic.Add(poolableType, parentT);

            for (int i = 0; i < poolableSO.poolable.initCount; i++)
            {
                GameObject poolInstance = InstantiatePool(poolableType);
                poolInstance.SetActive(false);

                queueDic[poolableType].Enqueue(poolInstance);
            }
        }

        public void ClearPool(PoolType type)
        {
            while (queueDic[type].TryDequeue(out GameObject obj) == true)
            {
                Destroy(obj);

                poolableDic.Remove(type);
                queueDic.Remove(type);
                parentDic.Remove(type);
            }
        }

        public void ClearAllPools()
        {
            foreach (KeyValuePair<PoolType, Queue<GameObject>> pair in queueDic)
            {
                ClearPool(pair.Key);
            }
        }

        private GameObject InstantiatePool(PoolType type)
        {
            GameObject prefab = poolableDic[type].prefab;
            GameObject instance = Instantiate(prefab);
            instance.transform.parent = parentDic[type];

            return instance;
        }

        public GameObject EnablePool(PoolType type, Action<GameObject> onBeforeEnableAction = null)
        {
            if (queueDic[type].Count >= poolableDic[type].maxCount)
            {
                Debug.LogErrorFormat(LOG_FORMAT, type + "최대 한도 " + poolableDic[type].maxCount + " 개를 초과하는 풀링 불가능");

                if (isAllowOverflow == false)
                {
                    return null;
                }
            }

            bool isInstantiated = false;
            if (queueDic[type].TryDequeue(out GameObject poolableObj) == false)
            {
                Debug.LogWarningFormat(LOG_FORMAT, type + " 풀 부족으로 새 인스턴스 생성됨, 기존 갯수 : " + queueDic[type].Count);

                isInstantiated = true;
                poolableObj = InstantiatePool(type);
            }

            IPoolable poolable = null;
            if (poolableObj.TryGetComponent(out poolable) == true &&
                isInstantiated == false)
            {
                poolable.OnBeforeEnable();
            }

            onBeforeEnableAction?.Invoke(poolableObj);
            poolableObj.SetActive(true);

            if (poolable != null && isInstantiated == false)
            {
                poolable.OnAfterEnable();
            }

            return poolableObj;
        }

        public void DisablePool(PoolType type, GameObject obj, Action<GameObject> onBeforeDisablePool = null)
        {
            onBeforeDisablePool?.Invoke(obj);

            IPoolable poolable = null;
            if (obj.TryGetComponent(out poolable) == true)
            {
                poolable.OnBeforeDisable();
            }
            obj.SetActive(false);
            obj.transform.parent = parentDic[type];
            if (poolable != null)
            {
                poolable.OnAfterDisable();
            }

            queueDic[type].Enqueue(obj);
        }

        public PoolState GetPoolStatus(PoolType type)
        {
            int activeCount = 0;
            foreach (GameObject obj in queueDic[type])
            {
                if (obj.activeSelf == true)
                {
                    activeCount++;
                }
            }

            PoolState state = new PoolState(type, activeCount, poolableDic[type].maxCount);
            Debug.LogFormat(LOG_FORMAT, state.ToString());

            return state;
        }

#if UNITY_EDITOR
        [ContextMenu("Find All SO In Project")]
        private void FindAllSOInProject()
        {
            poolableList.Clear();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:PoolableScriptableObject");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                PoolableScriptableObject foundSO = UnityEditor.AssetDatabase.LoadAssetAtPath<PoolableScriptableObject>(path);

                poolableList.Add(foundSO);
            }
        }
#endif
    }
}