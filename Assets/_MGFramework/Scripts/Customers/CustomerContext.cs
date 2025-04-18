using UnityEngine.AI;
using UnityEngine;

namespace MGFramework
{
    public class CustomerContext
    {
        // Core Components
        public Customer Customer;
        public CustomerStateMachine StateMachine;

        public Transform Transform;
        public NavMeshAgent Agent;
        public CustomerAnimationController AnimationController;

        // Reference Components
        public CustomerSeat OccupiedSeat;

        // Reference Values
        public Vector3 DesiredDirection;
        public bool IsCustomerInitialized = false;

        public CustomerContext(Customer customer)
        {
            this.Customer = customer;

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
