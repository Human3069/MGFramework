using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MGFramework
{
    public class UI_Timer : MonoBehaviour
    {
        [SerializeField]
        private Image progressedImage;

        private ITimer _timer;
        private float _yOffset;

        public void Initialize(ITimer timer, float yOffset)
        {
            this._timer = timer;
            this._yOffset = yOffset;
        }

        private void OnEnable()
        {
            progressedImage.fillAmount = 0f;
            if (_timer != null)
            {
                StartCoroutine(ProgressRoutine());
            }
        }

        private IEnumerator ProgressRoutine()
        {
            while (progressedImage.fillAmount < 1f)
            {
                // Foreground 贸府
                progressedImage.fillAmount = _timer.NormalizedTime;

                // 困摹 贸府
                Vector3 screenPos = Camera.main.WorldToScreenPoint(_timer.transform.position);
                this.transform.position = screenPos + Vector3.up * _yOffset;

                yield return null;
            }

            this.gameObject.DisablePool(PoolType.UI_Timer);
        }
    }
}
