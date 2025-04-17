using UnityEngine;

namespace MGFramework
{
    public class PlayerCamera 
    {
        private PlayerContext _context;
        private PlayerData _data;

        public PlayerCamera(PlayerContext context, PlayerData data)
        {
            this._context = context;
            this._data = data;
        }

        public void Tick()
        {
            Vector3 playerPosition = _data.Transform.position;
            Vector3 camDirection = _data.Camera.transform.forward;
            _data.Camera.transform.position = playerPosition - camDirection * 100f;
        }
    }
}
