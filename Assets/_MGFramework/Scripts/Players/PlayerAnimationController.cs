using UnityEngine;
using UnityEngine.AI;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerAnimationController
    {
        private NavMeshAgent _agent;

        [SerializeField]
        private Animator animator;

        private bool _isMoving = false;
        public bool IsMoving
        {
            get
            {
                return _isMoving;
            }
            set
            {
                if (_isMoving != value)
                {
                    _isMoving = value;

                    animator.SetBool("IsMoving", value);
                    animator.SetTrigger("IsMovingStateChanged");
                }
            }
        }

        public void OnAwake(NavMeshAgent agent)
        {
            this._agent = agent;
        }

        public void OnUpdate()
        {
            GetMovingState();
        }

        private void GetMovingState()
        {
            Vector3 destination = _agent.destination;
            Vector3 currentPos = _agent.nextPosition;
            float differenceBetween = Vector3.Distance(currentPos, destination);

            IsMoving = (differenceBetween > _agent.stoppingDistance);
        }

        public void StartMining()
        {
            animator.SetBool("IsStartMining", true);
            animator.SetTrigger("IsStartMiningStateChanged");

            _agent.destination = _agent.transform.position;
        }

        public void StopMining()
        {
            animator.SetBool("IsStartMining", false);
            animator.SetTrigger("IsStartMiningStateChanged");
        }
    }
}