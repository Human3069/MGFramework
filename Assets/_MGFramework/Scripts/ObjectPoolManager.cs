using _MG_Framework.Internal;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public enum PoolerType
    {
        Item_Tree,
        Stackable_Tree
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        private const string LOG_FORMAT = "<color=white><b>[ObjectPoolManager]</b></color> {0}";

        protected static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }

        [SerializeField]
        [SerializedDictionary("PoolerType", "ObjectPooler")]
        internal SerializedDictionary<PoolerType, ObjectPooler> poolerDic = new SerializedDictionary<PoolerType, ObjectPooler>();

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogErrorFormat(LOG_FORMAT, "");
                Destroy(this.gameObject);
                return;
            }

            foreach (KeyValuePair<PoolerType, ObjectPooler> pair in poolerDic)
            {
                GameObject poolerObj = new GameObject("Pooler_" + pair.Key);
                poolerObj.transform.SetParent(this.transform);

                pair.Value.Initialize(poolerObj.transform);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            Instance = null;
        }

        public virtual GameObject TakeOutObj(PoolerType type)
        {
            if (poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot TakeOutObj because type " + type + ", doesn't exist");
                return null;
            }
            else
            {
                return poolerDic[type].TakeOutObj();
            }
        }

        public virtual GameObject TakeOutObj(PoolerType type, Vector3 position, Quaternion rotation)
        {
            if (poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot TakeOutObj because type " + type + ", doesn't exist");
                return null;
            }
            else
            {
                return poolerDic[type].TakeOutObj(position, rotation);
            }
        }

        public virtual void ReturnObj(GameObject obj, PoolerType type)
        {
            if (obj == null || poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot return object!");
                return;
            }

            poolerDic[type].ReturnObj(obj);
        }
    }

    public static class ObjectPoolHelper
    {
        private const string LOG_FORMAT = "<color=white><b>[ObjectPoolHelper]</b></color> {0}";

        public static void ReturnObj(this GameObject obj, PoolerType type)
        {
            if (obj == null || ObjectPoolManager.Instance == null || ObjectPoolManager.Instance.poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot return object!");
                return;
            }

            ObjectPoolManager.Instance.poolerDic[type].ReturnObj(obj);
        }
    }
}

namespace _MG_Framework.Internal
{
    [System.Serializable]
    public class ObjectPooler
    {
        private Queue<GameObject> poolQueue;

        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private int instantiateCount;

        private Transform enabledT;
        private Transform disabledT;

        internal void Initialize(Transform poolerT)
        {
            GameObject enabledObj = new GameObject("Obj_Enabled");
            enabledObj.transform.SetParent(poolerT);
            enabledT = enabledObj.transform;

            GameObject disabledObj = new GameObject("Obj_Disabled");
            disabledObj.transform.SetParent(poolerT);
            disabledT = disabledObj.transform;

            poolQueue = new Queue<GameObject>();
            for (int i = 0; i < instantiateCount; i++)
            {
                GameObject obj = Object.Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(disabledT);

                poolQueue.Enqueue(obj);
            }
        }

        internal GameObject TakeOutObj()
        {
            if (poolQueue.TryDequeue(out GameObject obj) == false)
            {
                obj = Object.Instantiate(prefab);
            }

            obj.transform.SetParent(enabledT);
            obj.SetActive(true);

            return obj;
        }

        internal GameObject TakeOutObj(Vector3 position, Quaternion rotation)
        {
            if (poolQueue.TryDequeue(out GameObject obj) == false)
            {
                obj = Object.Instantiate(obj);
            }

            obj.transform.SetParent(enabledT);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            return obj;
        }

        internal void ReturnObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(disabledT);

            poolQueue.Enqueue(obj);
        }
    }
}