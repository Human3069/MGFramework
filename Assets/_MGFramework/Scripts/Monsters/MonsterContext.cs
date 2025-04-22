using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class MonsterContext
    {
        public MonsterStateMachine StateMachine;

        // Core Components
        public Transform Transform;
        public NavMeshAgent Agent;
        public MonsterAnimationController AnimationController;

        // Reference Components
        public Damageable TargetDamageable;

        public MonsterContext(Monster owner, MonsterData data)
        {
            this.Transform = owner.transform;
            this.Agent = owner.GetComponent<NavMeshAgent>();
            
            Animator animator = owner.GetComponentInChildren<Animator>();
            Animation animation = owner.GetComponentInChildren<Animation>();
            this.AnimationController = new MonsterAnimationController(animator, animation);
        }


        public void Initialize(MonsterStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }
    }
}