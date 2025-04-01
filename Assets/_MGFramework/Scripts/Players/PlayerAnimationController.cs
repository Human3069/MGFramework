using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerAnimationController
    {
        private PlayerData _data;

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

        public void OnAwake(PlayerData data)
        {
            this._data = data;
        }

        public void Tick()
        {
            GetMovingState();
        }

        private void GetMovingState()
        {
            Vector3 destination = _data._Agent.destination;
            Vector3 currentPos = _data._Agent.nextPosition;
            float differenceBetween = Vector3.Distance(currentPos, destination);

            IsMoving = (differenceBetween > _data._Agent.stoppingDistance);
        }

        public void StartMining()
        {
            animator.SetBool("IsStartMining", true);
            animator.SetTrigger("IsStartMiningStateChanged");

            _data._Agent.destination = _data._Agent.transform.position;
        }

        public void StopMining()
        {
            animator.SetBool("IsStartMining", false);
            animator.SetTrigger("IsStartMiningStateChanged");
        }
    }
}