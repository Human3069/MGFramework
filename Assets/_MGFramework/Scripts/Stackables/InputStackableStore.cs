using Cysharp.Threading.Tasks;

namespace MGFramework
{
    /// <summary>
    /// 인벤토리 => 스택 전용 컴포넌트
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
