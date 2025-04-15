using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// Input을 통해 Output을 생성하는 컴포넌트입니다.
    /// </summary>
    public class BasePayload : MonoBehaviour, ITimer
    {
        [SerializeField]
        private InputStackableStore[] inputStores;
        [SerializeField]
        private OutputStackableStore[] outputStores;

        [Space(10)]
        [SerializeField]
        private float inputSpeed = 0.3f;
        [SerializeField]
        private float outputSpeed = 10f;

        [Header("ITimer")]
        [SerializeField]
        private float yOffset = 0f;

        private bool isGeneratingOutput = false;

        protected float _normalizedTime = 0f;
        public float NormalizedTime
        {
            get
            {
                return _normalizedTime;
            }
            set
            {
                _normalizedTime = value;
            }
        }

        private void Awake()
        {
            foreach (InputStackableStore inputStore in inputStores)
            {
                inputStore.OnPush += OnPush;
            }
        }

        private void OnPush()
        {
            // Output을 생성할 수 있는지 체크.
            if (isGeneratingOutput == false &&
                inputStores.IsAllPoppable() == true)
            {
                // Output을 생성하기 위한 재료 소모
                inputStores.PopAll();

                // 생성 비동기 실행
                GenerateOutputAsync().Forget();
            }
        }

        private async UniTaskVoid GenerateOutputAsync()
        {
            isGeneratingOutput = true;

            float time = 0f;
            NormalizedTime = 0f;

            // 재료 만들 타이머 생성
            PoolType.UI_Timer.EnablePool(OnBeforeEnablePool);
            void OnBeforeEnablePool(GameObject timerObj)
            {
                UI_Timer timer = timerObj.GetComponent<UI_Timer>();
                timer.Initialize(this, yOffset);

                timerObj.transform.SetParent(UI_Main.Instance.TimerParent);
            }

            while (time < outputSpeed)
            {
                await UniTask.Yield();

                time += Time.deltaTime;
                NormalizedTime = time / outputSpeed;
            }

            NormalizedTime = 1f;
            isGeneratingOutput = false;

            // Output 생성
            outputStores.AddAll();

            // 재귀호출
            OnPush();
        }
    }
}
