using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using static MGFramework.UI_Main;

namespace MGFramework
{
    public class UI_Timer : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private Image progressedImage;

        private TimerInfo _info;

        public void Initialize(TimerInfo info)
        {
            progressedImage.fillAmount = 0f;

            this._info = info;
        }

        private void FixedUpdate()
        {
            Vector3 screenPos = _info._Camera.WorldToScreenPoint(_info._Timer.transform.position);
            this.transform.position = screenPos + Vector3.up * _info._Timer.TimerOffsetHeight;
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