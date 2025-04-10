using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
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

        private bool _isOn = false;
        public bool IsOn
        {
            get
            {
                return _isOn;
            }
            set
            {
                if (_isOn != value)
                {
                    if (awaiter.IsCompleted == false)
                    {
                        tokenSource.Cancel();
                        tokenSource = new CancellationTokenSource();
                    }
                    
                    _isOn = value;
                    if (value == true)
                    {
                        if (this.gameObject.activeSelf == false)
                        {
                            this.gameObject.SetActive(true);
                        }

                        awaiter = FadeAsync(0f, 1f).GetAwaiter();
                    }
                    else
                    {
                        awaiter = FadeAsync(1f, 0f, OnAfterFaded).GetAwaiter();
                        void OnAfterFaded()
                        {
                            this.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private UniTask.Awaiter awaiter;

        public void Initialize(TimerInfo info)
        {
            progressedImage.fillAmount = 0f;

            this._info = info;
            this._info._Timer.OnNormalChangedEvent += OnNormalChanged;

            this.gameObject.SetActive(false);
        }

        private void OnNormalChanged(float normal)
        {
            if (normal == 0f || normal == 1f)
            {
                IsOn = false;
            }
            else
            {
                if (IsOn == false)
                {
                    IsOn = true;
                }
            }

            progressedImage.fillAmount = normal;
        }

        private void FixedUpdate()
        {
            Vector3 screenPos = _info._Camera.WorldToScreenPoint(_info._Timer.transform.position);
            this.transform.position = screenPos + Vector3.up * _info._Timer.TimerOffsetHeight;
        }

        private async UniTask FadeAsync(float fromValue, float toValue, Action afterFadeAction = null)
        {
            float timer = 0f;
            while (timer < _info._FadeDuration)
            {
                float normal = timer / _info._FadeDuration;
                float normalized = Mathf.Lerp(fromValue, toValue, normal);
                group.alpha = normalized;

                timer += Time.deltaTime;
                await UniTask.Yield(cancellationToken : tokenSource.Token);
            }

            group.alpha = toValue;
            afterFadeAction?.Invoke();
        }
    }
}