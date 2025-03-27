using _KMH_Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField]
        private PlayerInputController input;
        [SerializeField]
        private PlayerAnimationController animator;
        [SerializeField]
        private PlayerStateController state;
        public PlayerInventory Inventory;

        private void Awake()
        {
            NavMeshAgent agent = this.GetComponent<NavMeshAgent>();

            input.OnAwake(agent);
            animator.OnAwake(agent);
            state.OnAwake(this.transform, animator);
        }

        private void Update()
        {
            input.OnUpdate();
            animator.OnUpdate();
            state.OnUpdate();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Item item) == true &&
                item.IsOnInventory == false &&
                item.IsFading == false)
            {
                Inventory.TryPush(item);
            }
        }
    }
}