using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class OccupiableFootstep : BaseFootstep
    {
        [Header("=== OccupiableFootstep ===")]
        [SerializeField]
        private float fadeDuration = 0.5f;

        private Color inColor;
        private Color outColor;

        protected override void Awake()
        {
            inColor = new Color(progressImage.color.r, progressImage.color.g, progressImage.color.b, 1f);
            outColor = new Color(progressImage.color.r, progressImage.color.g, progressImage.color.b, 0f);

            progressImage.color = outColor;
        }

        protected override async UniTaskVoid OnInventoryEnteredAsync(CancellationToken token)
        {
            Color currentColor = progressImage.color;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                float normal = timer / fadeDuration;
                progressImage.color = Color.Lerp(currentColor, inColor, normal);

                if (IsOccupied == false)
                {
                    break;
                }

                await UniTask.Yield(cancellationToken: token);
                timer += Time.deltaTime;
            }

            await UniTask.WaitWhile(() => IsOccupied == true, cancellationToken: token);

            currentColor = progressImage.color;
            timer = 0f;
 
            while (timer < fadeDuration)
            {
                float normal = timer / fadeDuration;
                progressImage.color = Color.Lerp(currentColor, outColor, normal);

                await UniTask.Yield(cancellationToken: token);
                timer += Time.deltaTime;
            }
        }
    }
}
