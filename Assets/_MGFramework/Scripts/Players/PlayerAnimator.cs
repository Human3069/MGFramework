using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class PlayerAnimator
    {
        private PlayerContext _context;
        private PlayerData _data;

        private float currentNormal = 0f;
        private bool isInput = false;

        private CancellationTokenSource tokenSource;

        public PlayerAnimator(PlayerContext context, PlayerData data)
        {
            this._context = context;
            this._data = data;

            this._context.Behaviour.OnTargetChanged += OnTargetChanged;
            tokenSource = new CancellationTokenSource();
        }

        private void OnTargetChanged(Damageable damageable)
        {
            if (damageable == null)
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();
            }
            else
            {
                OnTargetChangedAsync(tokenSource.Token).Forget();
            }
        }

        private async UniTaskVoid OnTargetChangedAsync(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                _context.Anime.SetTrigger("IsAttack");
                await UniTask.WaitForSeconds(_data.AttackSpeed, cancellationToken: token);
            }
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
