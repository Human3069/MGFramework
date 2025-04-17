using _KMH_Framework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MGFramework
{
    public class UI_MobileJoystick : MonoSingleton<UI_MobileJoystick>
    {
        [SerializeField]
        private Transform backgroundT;
        [SerializeField]
        private Transform foregroundT;

        [Space(10)]
        [SerializeField]
        private float tapTimeThreshold = 0.2f;
        [SerializeField, Range(10f, 500f)]
        private float maxRange = 50f;

        private Vector3 newForegroundPos;
        private Vector2 inputValue;
        private Vector2 foregroundOriginPos;
        private float inputTime;

        public delegate void InputDownDelegate();
        public event InputDownDelegate OnInputDown;

        public delegate void InputDelegate(Vector2 input);
        public event InputDelegate OnInput;

        public delegate void InputUpDelegate();
        public event InputUpDelegate OnInputUp;

        public delegate void TapDelegate();
        public event TapDelegate OnTap;

        private void Update()
        {
            bool isHovered = EventSystem.current.IsPointerOverGameObject();
            bool isClicked = Input.GetMouseButtonDown(0) == true;

            if (isHovered == false &&
                isClicked == true)
            {
                OnClickDownAsync().Forget();
            }
        }

        private async UniTaskVoid OnClickDownAsync()
        {
            OnClickDown();
            while (Input.GetMouseButton(0) == true)
            {
                OnClick();
                await UniTask.Yield();
            }
            OnClickUp();
        }

        private void OnClickDown()
        {
            backgroundT.transform.position = Input.mousePosition;
            foregroundOriginPos = Input.mousePosition;

            OnInputDown?.Invoke();
        }

        private void OnClick()
        {
            CalculateAndUpdateInput();
            inputTime += Time.deltaTime;

            OnInput?.Invoke(inputValue);
        }

        private void OnClickUp()
        {
            // 탭 체크 및 타이머 초기화
            if (inputTime <= tapTimeThreshold)
            {
                OnTap?.Invoke();
            }
            inputTime = 0f;

            // 조이스틱 초기화
            foregroundT.position = foregroundOriginPos;
            inputValue.x = 0f;
            inputValue.y = 0f;

            OnInputUp?.Invoke();
        }

        /// <summary>
        /// 조이스틱 입력값 계산
        /// </summary>
        private void CalculateAndUpdateInput()
        {
            Vector2 clampedPosition = Input.mousePosition;
            clampedPosition = Vector2.ClampMagnitude(clampedPosition - foregroundOriginPos, maxRange);

            inputValue.x = EvaluateInputValue(clampedPosition.x);
            inputValue.y = EvaluateInputValue(clampedPosition.y);

            newForegroundPos = foregroundOriginPos + clampedPosition;
            foregroundT.position = newForegroundPos;
        }

        /// <summary>
        /// 입력값 정규화
        /// </summary>
        /// <param name="vectorPosition"></param>
        /// <returns></returns>
        private float EvaluateInputValue(float vectorPosition)
        {
            float absValue = Mathf.Abs(vectorPosition);
            float signValue = Mathf.Sign(vectorPosition);

            return Mathf.InverseLerp(0, maxRange, absValue) * signValue;
        }
    }
}