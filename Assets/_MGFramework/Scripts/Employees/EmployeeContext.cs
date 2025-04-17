using _KMH_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    public class EmployeeContext 
    {
        // Core Components
        public EmployeeStateMachine StateMachine;

        public Transform Transform;
        public NavMeshAgent Agent;
        public KeyframeReceiver Receiver;
        public Inventory Inventory;
        public EmployeeAnimationController AnimationController;

        // Reference Components
        public Harvestable TargetHarvestable;
        public Payload TargetPayload;

        public EmployeeContext(Employee employee)
        {
            this.Transform = employee.transform;
            this.Agent = employee.GetComponent<NavMeshAgent>();
            this.Receiver = employee.GetComponentInChildren<KeyframeReceiver>();
            this.Inventory = employee.GetComponent<Inventory>();

            Animator animator = employee.GetComponentInChildren<Animator>();
            this.AnimationController = new EmployeeAnimationController(animator);

            this.TargetHarvestable = null;
            this.TargetPayload = null;
        }

        /// <summary>
        /// Initialize because the state machine is not created in the constructor.
        /// </summary>
        /// <param name="stateMachine"></param>
        public void Initialize(EmployeeStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }
    }
}
