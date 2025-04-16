using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerData
    {
        [HideInInspector]
        public PlayerMovement Movement;
        [HideInInspector]
        public PlayerBehaviour Behaviour;
        [HideInInspector]
        public PlayerAnimator Animator;
        [HideInInspector]
        public PlayerCamera Camera;

        public Transform _Transform;

        public void Initialize(PlayerMovement movement, PlayerBehaviour behaviour, PlayerAnimator animator, PlayerCamera camera)
        {
            this.Movement = movement;
            this.Behaviour = behaviour;
            this.Animator = animator;
            this.Camera = camera;
        }
    }
}
