using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using static MGFramework.UI_Main;

namespace MGFramework
{
    public class UI_Manabar : MonoBehaviour
    {
        private ManabarInfo _info;

        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private Image progressedImage;
        [SerializeField]
        private Image damagedImage;

        public void Initialize(ManabarInfo info)
        {
            progressedImage.fillAmount = 1f;
            damagedImage.fillAmount = 1f;

            this._info = info;
            this._info._Progressable.OnProgressChangedEvent += OnProgressChanged;
        }

        private void FixedUpdate()
        {
            Vector3 screenPos = _info._Camera.WorldToScreenPoint(_info._Progressable.transform.position);
            this.transform.position = screenPos + Vector3.up * _info._Progressable.ProgressOffsetHeight;

            float currentAmount = damagedImage.fillAmount;
            float targetAmount = progressedImage.fillAmount;
            float fillSpeed = _info._DamagedFillSpeed;
            damagedImage.fillAmount = Mathf.Lerp(currentAmount, targetAmount, fillSpeed);
        }

        private void OnProgressChanged(float maxHealth, float currentHealth)
        {
            float normal = currentHealth / maxHealth;
            progressedImage.fillAmount = normal;
        }

        private async UniTaskVoid FadeAsync(float fromValue, float toValue, Action afterFadeAction = null)
        {
            float timer = 0f;
            while (timer < _info._FadeDuration)
            {
                float normal = timer / _info._FadeDuration;
                float normalized = Mathf.Lerp(fromValue, toValue, normal);
                group.alpha = normalized;

                timer += Time.deltaTime;
                await UniTask.Yield();
            }

            group.alpha = toValue;
            afterFadeAction?.Invoke();
        }
    }
}