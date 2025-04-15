using Cysharp.Threading.Tasks;

namespace MGFramework
{
    /// <summary>
    /// �κ��丮 => ���� ���� ������Ʈ
    /// </summary>
    public class InputStackableStore : BaseStackableStore
    {
        public delegate void PushDelegate();
        public event PushDelegate OnPush;

        protected override async UniTaskVoid OnInventoryEnteredAsync()
        {
            while (EnteredInventory != null)
            {
                if (EnteredInventory.TryPop(poolType, out Stackable stackable) == true)
                {
                    Push(stackable);
                    OnPush?.Invoke();
                }

                await UniTask.WaitForSeconds(speed);
            }
        }
    }
}
