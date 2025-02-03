using _MG_Framework.Internal;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public enum PoolerType
    {
        Item_Tree,
        Item_Meat,

        Stackable_Tree,
        Stackable_Meat,
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

        public virtual Poolable TakeOutObj(PoolerType type)
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

        public virtual Poolable TakeOutObj(PoolerType type, Vector3 position, Quaternion rotation)
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

        public virtual void ReturnObj(Poolable poolable, PoolerType type)
        {
            if (poolable == null || poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot return object!");
                return;
            }

            poolerDic[type].ReturnObj(poolable);
        }
    }

    public static class ObjectPoolHelper
    {
        private const string LOG_FORMAT = "<color=white><b>[ObjectPoolHelper]</b></color> {0}";

        public static void ReturnObj(this Poolable poolable, PoolerType type)
        {
            if (poolable == null || ObjectPoolManager.Instance == null || ObjectPoolManager.Instance.poolerDic.ContainsKey(type) == false)
            {
                Debug.LogErrorFormat(LOG_FORMAT, "cannot return object!");
                return;
            }

            ObjectPoolManager.Instance.poolerDic[type].ReturnObj(poolable);
        }
    }
}

namespace _MG_Framework.Internal
{
    [System.Serializable]
    public class ObjectPooler
    {
        private Queue<Poolable> poolQueue;

        [SerializeField]
        private Poolable prefab;
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

            poolQueue = new Queue<Poolable>();
            for (int i = 0; i < instantiateCount; i++)
            {
                Poolable poolable = Object.Instantiate(prefab);
                poolable.gameObject.SetActive(false);
                poolable.transform.SetParent(disabledT);

                poolQueue.Enqueue(poolable);
            }
        }

        internal Poolable TakeOutObj()
        {
            if (poolQueue.TryDequeue(out Poolable poolable) == false)
            {
                poolable = Object.Instantiate(prefab);
            }

            poolable.transform.SetParent(enabledT);
            poolable.gameObject.SetActive(true);

            return poolable;
        }

        internal Poolable TakeOutObj(Vector3 position, Quaternion rotation)
        {
            if (poolQueue.TryDequeue(out Poolable poolable) == false)
            {
                poolable = Object.Instantiate(poolable);
            }

            poolable.transform.SetParent(enabledT);
            poolable.transform.SetPositionAndRotation(position, rotation);
            poolable.gameObject.SetActive(true);

            return poolable;
        }

        internal void ReturnObj(Poolable poolable)
        {
            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(disabledT);

            poolQueue.Enqueue(poolable);
        }
    }
}