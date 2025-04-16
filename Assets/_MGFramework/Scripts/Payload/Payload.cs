using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MGFramework
{
    /// <summary>
    /// Input�� ���� Output�� �����ϴ� ������Ʈ�Դϴ�.
    /// </summary>
    public class Payload : MonoBehaviour, ITimer
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
            // Output�� ������ �� �ִ��� üũ.
            if (isGeneratingOutput == false &&
                inputStores.IsAllPoppable() == true)
            {
                // Output�� �����ϱ� ���� ��� �Ҹ�
                inputStores.PopAll();

                // ���� �񵿱� ����
                GenerateOutputAsync().Forget();
            }
        }

        private async UniTaskVoid GenerateOutputAsync()
        {
            isGeneratingOutput = true;

            float time = 0f;
            NormalizedTime = 0f;

            // ��� ���� Ÿ�̸� ����
            this.EnableTimer(yOffset);
            while (time < outputSpeed)
            {
                await UniTask.Yield();

                time += Time.deltaTime;
                NormalizedTime = time / outputSpeed;
            }

            NormalizedTime = 1f;
            isGeneratingOutput = false;

            // Output ����
            outputStores.AddAll();

            // ���ȣ��
            OnPush();
        }
    }
}
