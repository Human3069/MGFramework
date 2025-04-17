using UnityEngine.AI;
using UnityEngine;

namespace MGFramework
{
    public class CustomerContext
    {
        // Core Components
        public CustomerStateMachine StateMachine;

        public Transform Transform;
        public NavMeshAgent Agent;
        public CustomerAnimationController AnimationController;

        // Reference Values
        public Vector3 DesiredDirection;
        
        public CustomerContext(Customer customer)
        {
            this.Transform = customer.transform;
            this.Agent = customer.GetComponent<NavMeshAgent>();

            Animator animator = customer.GetComponentInChildren<Animator>();
            this.AnimationController = new CustomerAnimationController(animator);
        }

        /// <summary>
        /// Initialize because the state machine is not created in the constructor.
        /// </summary>
        /// <param name="stateMachine"></param>
        public void Initialize(CustomerStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }
    }
}
