using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MGFramework
{
    public class UI_Healthbar : MonoBehaviour
    {
        private CanvasGroup group;

        [SerializeField]
        private Image damagedImage;
        [SerializeField]
        private Image progressedImage;

        [Space(10)]
        [SerializeField]
        private float towardSpeed = 1f;
        [SerializeField]
        private float fadeDuration = 0.5f;

        private Damageable _damageable;
        private float _yOffset;

        private void Awake()
        {
            group = this.GetComponent<CanvasGroup>();
        }

        public void Initialize(Damageable damageable, float yOffset)
        {
            this._damageable = damageable;
            this._damageable.OnDamagedEvent += OnDamaged;

            this._yOffset = yOffset;
        }

        private void OnDamaged()
        {
            progressedImage.fillAmount = _damageable.CurrentNormal;
        }

        private void OnEnable()
        {
            if (_damageable != null)
            {
                progressedImage.fillAmount = _damageable.CurrentNormal;

                StartCoroutine(group.FadeRoutine(0f, 1f, fadeDuration));
                StartCoroutine(ProgressRoutine());
            }
        }

        private IEnumerator ProgressRoutine()
        {
            while (_damageable.IsDead == false)
            {
                // 困摹 贸府
                Vector3 screenPos = Camera.main.WorldToScreenPoint(_damageable.transform.position);
                this.transform.position = screenPos + Vector3.up * _yOffset;

                // 局聪皋捞记 贸府
                float targetFillAmount = _damageable.CurrentNormal;
                damagedImage.fillAmount = Mathf.Lerp(damagedImage.fillAmount, targetFillAmount, Time.deltaTime * towardSpeed);

                yield return null;
            }

            this._damageable.OnDamagedEvent -= OnDamaged;
            progressedImage.fillAmount = 0f;
            damagedImage.fillAmount = 0f;
            _damageable = null;

            yield return group.FadeRoutine(1f, 0f, fadeDuration);

            this.gameObject.DisablePool(PoolType.UI_Healthbar);
        }
    }
}
