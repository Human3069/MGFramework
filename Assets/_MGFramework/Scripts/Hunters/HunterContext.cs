using _KMH_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class HunterContext
    {
        // Core Components
        public HunterStateMachine StateMachine;
        
        public Transform Transform;
        public NavMeshAgent Agent;
        public HunterAnimationController AnimationController;
        public Inventory Inventory;
        public Damageable OwnerDamageable;

        // Reference Components
        public Damageable TargetDamageable;
        public Payload TargetPayload;

        public HunterContext(Hunter owner)
        {
            this.Transform = owner.transform;
            this.Agent = owner.GetComponent<NavMeshAgent>();
            
            Animator animator = owner.GetComponentInChildren<Animator>();
            Animation animation = owner.GetComponentInChildren<Animation>();
            this.AnimationController = new HunterAnimationController(animator, animation);
            this.AnimationController.OnDeadPlayed += OnDeadPlayed;

            this.Inventory = owner.GetComponent<Inventory>();
            this.OwnerDamageable = owner.GetComponent<Damageable>();
        }

        private void OnDeadPlayed()
        {
            this.Transform.gameObject.DisablePool(_KMH_Framework.Pool.PoolType.Hunter);
        }

        public void Initialize(HunterStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }
    }
}