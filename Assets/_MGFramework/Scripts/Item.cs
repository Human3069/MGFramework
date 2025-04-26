using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGFramework
{
    public class Item : MonoBehaviour
    {
        public PoolType ItemPoolType;
        [SerializeField]
        private float lifetime = 600f;

        private CancellationTokenSource tokenSource;

        private void OnEnable()
        {
            tokenSource = new CancellationTokenSource();
            CheckLifetimeAsync(tokenSource.Token).Forget();
        }

        private void OnDisable()
        {
            tokenSource.Cancel();
        }

        private async UniTaskVoid CheckLifetimeAsync(CancellationToken token)
        {
            await UniTask.WaitForSeconds(lifetime, cancellationToken: token);

            this.gameObject.DisablePool(ItemPoolType);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string poolTypeName = ItemPoolType.ToString();
            if (poolTypeName.Contains("Item") == false)
            {
                Debug.LogError("Stack ������Ʈ�� StackPoolType�� �ݵ�� Stack�� ���ԵǴ� ���� �����ؾ� �մϴ�.");
            }
        }
#endif
    }
}
