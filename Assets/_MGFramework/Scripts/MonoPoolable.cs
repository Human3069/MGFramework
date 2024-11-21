using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    // 한가지 큐로 오브젝트 풀이 가능한 컴포넌트 (상속하여 사용)
    public class MonoPoolable : MonoBehaviour
    {
        // private const string LOG_FORMAT = "<color=white><b>[ObjectPoolManager]</b></color> {0}";

        protected Queue<PooledObject> poolQueue = new Queue<PooledObject>();

        [Header("=== MonoPoolable ===")]
        [SerializeField]
        protected PooledObject prefab;
        [SerializeField]
        protected int instantiateCount = 5;

        protected GameObject enabledParentObj;
        protected GameObject disabledParentObj;

        protected virtual void Awake()
        {
            enabledParentObj = new GameObject("Parent_Enables");
            enabledParentObj.transform.SetParent(this.transform);

            disabledParentObj = new GameObject("Parent_Disables");
            disabledParentObj.transform.SetParent(this.transform);

            for (int i = 0; i < instantiateCount; i++)
            {
                PooledObject obj = Instantiate(prefab);
                obj.Initialize(this);

                ReturnObject(obj);
            }
        }

        public virtual void EnableObject(Vector3 pos)
        {
            if (poolQueue.TryDequeue(out PooledObject obj) == false)
            {
                obj = Instantiate(prefab);
                obj.Initialize(this);
            }

            obj.transform.SetParent(enabledParentObj.transform);
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);
        }

        public virtual void EnableObject(Vector3 pos, Quaternion rot) 
        {
            if (poolQueue.TryDequeue(out PooledObject obj) == false)
            {
                obj = Instantiate(prefab);
                obj.Initialize(this);
            }

            obj.transform.SetParent(enabledParentObj.transform);
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.gameObject.SetActive(true);
        }

        public virtual void ReturnObject(PooledObject obj)
        {
            obj.transform.SetParent(disabledParentObj.transform);
            obj.gameObject.SetActive(false);

            poolQueue.Enqueue(obj);
        }
    }
}