using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerCamera 
    {
        private PlayerData _data;

        [SerializeField]
        private Camera camera;

        public void OnAwake(PlayerData data)
        {
            this._data = data;
        }

        public void Tick()
        {
            Vector3 playerPosition = _data._Transform.position;
            Vector3 camDirection = camera.transform.forward;
            camera.transform.position = playerPosition - camDirection * 100f;
        }
    }
}
