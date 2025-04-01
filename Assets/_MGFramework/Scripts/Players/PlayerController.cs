using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Damageable))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        private Damageable damageable;

        [Header("=== PlayerController ===")]
        [SerializeField]
        private PlayerData data;

        [Space(10)]
        [SerializeField]
        private PlayerInputController input;
        [SerializeField]
        private PlayerAnimationController animator;
        [SerializeField]
        private PlayerStateController state;
        public PlayerInventory Inventory;

        private void Awake()
        {
            data._Anime = animator;
            damageable = this.GetComponent<Damageable>();

            input.OnAwake(data);
            animator.OnAwake(data);
            state.OnAwake(data);

            AwakeAsync().Forget();
        }

        private async UniTaskVoid AwakeAsync()
        {
            while (damageable.IsDead == false)
            {
                state.SlowTick();

                await UniTask.WaitForSeconds(0.5f);
            }
        }

        private void Update()
        {
            input.Tick();
            animator.Tick();
            state.Tick();
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