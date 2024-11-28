using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _MG_Framework
{
    public class UI_Healthbar : MonoBehaviour
    {
        protected Camera _camera;
        protected IDamageable targetDamageable;
        protected RectTransform rectT;

        [SerializeField]
        protected Image currentHealthImage;
        [SerializeField]
        protected Image redMarkImage;

        [Space(10)]
        [SerializeField]
        protected float redMarkDelta = 10f;

        protected float _additionalHeight;

        public virtual void Initialize(Camera targetCamera, IDamageable damageable, float additionalHeight)
        {
            _camera = targetCamera;
            targetDamageable = damageable;
            rectT = this.transform as RectTransform;

            _additionalHeight = additionalHeight;

            targetDamageable.OnDamagedCallback += OnDamaged;
        }

        protected virtual void OnDamaged(float normalized)
        {
            currentHealthImage.fillAmount = normalized;

            UniTaskEx.Cancel(this, 0);
            OnDamagedAsync(normalized).Forget();
        }

        protected virtual async UniTask OnDamagedAsync(float normalized)
        {
            while (redMarkImage.fillAmount != currentHealthImage.fillAmount)
            {
                redMarkImage.fillAmount = Mathf.MoveTowards(redMarkImage.fillAmount, currentHealthImage.fillAmount, redMarkDelta * Time.deltaTime);

                await UniTaskEx.NextFrame(this, 0);
            }

            if (redMarkImage.fillAmount == 0f)
            {
                this.gameObject.SetActive(false);
            }
        }

        protected virtual void Update()
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(targetDamageable.transform.position) + new Vector3(0f, _additionalHeight, 0f);
            this.transform.position = screenPoint;
        }
    }
}