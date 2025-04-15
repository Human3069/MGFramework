using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class PlayerAnimator
    {
        private PlayerData _data;

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private float normalLerpPower = 5f;

        private float currentNormal = 0f;
        private bool isInput = false;

        public void OnAwake(PlayerData data)
        {
            this._data = data;
        }

        public void Tick()
        {
            if (isInput == false)
            {
                currentNormal = Mathf.Lerp(currentNormal, 0f, normalLerpPower * Time.deltaTime);
            }

            animator.SetFloat("SpeedNormal", currentNormal);
        }

        public void OnInputDown()
        {
            isInput = true;
        }

        public void OnInput(Vector3 input)
        {
            float inputNormal = input.magnitude;
            currentNormal = Mathf.Lerp(currentNormal, inputNormal, normalLerpPower * Time.deltaTime);
        }

        public void OnInputUp()
        {
            isInput = false;
        }
    }
}
