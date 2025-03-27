using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using static MGFramework.UI_Main;

namespace MGFramework
{
    public class UI_Healthbar : MonoBehaviour
    {
        private HealthbarInfo _info;

        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private Image progressedImage;
        [SerializeField]
        private Image damagedImage;

        public void Initialize(HealthbarInfo info)
        {
            this._info = info;
            this._info._Damageable.OnAlivedEvent += OnAlived;
            this._info._Damageable.OnDamagedEvent += OnDamaged;
            this._info._Damageable.OnDeadEvent += OnDead;
        }

        private void FixedUpdate()
        {
            Vector3 screenPos = _info._Camera.WorldToScreenPoint(_info._Damageable.transform.position);
            this.transform.position = screenPos + Vector3.up * _info._Damageable.OffsetHeight;

            float currentAmount = damagedImage.fillAmount;
            float targetAmount = progressedImage.fillAmount;
            float fillSpeed = _info._DamagedFillSpeed;
            damagedImage.fillAmount = Mathf.Lerp(currentAmount, targetAmount, fillSpeed);
        }

        private void OnAlived()
        {
            progressedImage.fillAmount = 1f;
            damagedImage.fillAmount = 1f;

            this.gameObject.SetActive(true);

            FadeAsync(0f, 1f).Forget();
        }

        private void OnDamaged(float maxHealth, float currentHealth)
        {
            float normal = currentHealth / maxHealth;
            progressedImage.fillAmount = normal;
        }

        private void OnDead()
        {
            progressedImage.fillAmount = 0f;
            FadeAsync(1f, 0f, () => this.gameObject.SetActive(false)).Forget();
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