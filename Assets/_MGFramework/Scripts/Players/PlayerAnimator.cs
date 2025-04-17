using UnityEngine;

namespace MGFramework
{
    public class PlayerAnimator
    {
        private PlayerContext _context;
        private PlayerData _data;

        private float currentNormal = 0f;
        private bool isInput = false;

        public PlayerAnimator(PlayerContext context, PlayerData data)
        {
            this._context = context;
            this._data = data;

            this._context.Behaviour.OnTargetChanged += OnTargetChanged;
        }

        private void OnTargetChanged(Damageable damageable)
        {
            _context.Anime.SetBool("IsAttack", damageable != null);
            _context.Anime.SetTrigger("IsAttackStateChanged");
        }

        public void Tick()
        {
            if (isInput == false)
            {
                currentNormal = Mathf.Lerp(currentNormal, 0f, _data.NormalLerpPower * Time.deltaTime);
            }

            _context.Anime.SetFloat("SpeedNormal", currentNormal);
        }

        public void OnInputDown()
        {
            isInput = true;
        }

        public void OnInput(Vector3 input)
        {
            float inputNormal = input.magnitude;
            currentNormal = Mathf.Lerp(currentNormal, inputNormal, _data.NormalLerpPower * Time.deltaTime);
        }

        public void OnInputUp()
        {
            isInput = false;
        }
    }
}
