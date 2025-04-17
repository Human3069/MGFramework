using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    public class PlayerContext
    {
        public KeyframeReceiver Receiver;
        public Damageable Damageable;
        public CharacterController Controller;
        public Animator Anime;

        public PlayerMovement Movement;
        public PlayerBehaviour Behaviour;
        public PlayerAnimator Animator;
        public PlayerCamera Camera;

        public PlayerContext(PlayerData data)
        {
            Receiver = data.Transform.GetComponentInChildren<KeyframeReceiver>();
            Damageable = data.Transform.GetComponent<Damageable>();
            Controller = data.Transform.GetComponent<CharacterController>();
            Anime = data.Transform.GetComponentInChildren<Animator>();

            Behaviour = new PlayerBehaviour(this, data);
            Movement = new PlayerMovement(this, data);
            Animator = new PlayerAnimator(this, data);
            Camera = new PlayerCamera(this, data);
        }
    }
}
