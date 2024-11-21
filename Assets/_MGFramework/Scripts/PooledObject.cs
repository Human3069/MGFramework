using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    public class PooledObject : MonoBehaviour
    {
        protected MonoPoolable _handler;

        [SerializeField]
        protected float duration;

        public virtual void Initialize(MonoPoolable handler)
        {
            this._handler = handler;
        }

        protected virtual void OnEnable()
        {
            OnEnableAsync().Forget();
        }

        protected virtual async UniTaskVoid OnEnableAsync()
        {
            await UniTask.WaitForSeconds(duration);

            _handler.ReturnObject(this);
        }
    }
}