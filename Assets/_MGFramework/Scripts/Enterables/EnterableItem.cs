using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _MG_Framework
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnterableItem : BaseEnterable
    {
        protected Rigidbody _rigidbody;
        protected Collider triggerCollider;

        [SerializeField]
        protected ItemType type;
        [SerializeField]
        protected Animation _animation;

        [Space(10)]
        [SerializeField]
        protected float throwUpForce = 5f;
        [SerializeField]
        protected float throwSideForce = 2.5f;

        protected virtual void Awake()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
            Collider[] colliders = this.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger == true)
                {
                    triggerCollider = collider;
                    break;
                }
            }
        }

        protected virtual void OnEnable()
        {
            OnEnableAsync().Forget();
        }

        protected virtual async UniTaskVoid OnEnableAsync()
        {
            triggerCollider.enabled = false;

            float randomizedX = Random.Range(-throwSideForce, throwSideForce);
            float randomizedZ = Random.Range(-throwSideForce, throwSideForce);
            Vector3 randomizedPower = new Vector3(randomizedX, throwUpForce, randomizedZ);
            _rigidbody.AddForce(randomizedPower, ForceMode.VelocityChange);

            // "OnEnable" clip play automatically
            AnimationClip currentClip = _animation.GetClip("OnEnable");
            float duration = currentClip.length;

            await UniTask.WaitForSeconds(duration);

            triggerCollider.enabled = true;
        }

        protected override void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<PlayerInventory>(out PlayerInventory inventory) == true)
            {
                inventory.Push(type);

                DisableAsync().Forget();
            }
        }

        protected override void OnTriggerExit(Collider collider)
        {
            //
        }

        protected virtual async UniTaskVoid DisableAsync()
        {
            AnimationState state = _animation.PlayQueued("OnDisable");
            float duration = state.length;

            await UniTask.WaitForSeconds(duration);

            this.ReturnObj(PoolerType.Item_Tree);
        }
    }
}