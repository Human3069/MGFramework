using Cysharp.Threading.Tasks;

namespace MGFramework
{
    /// <summary>
    /// 스택 => 인벤토리 전용 컴포넌트
    /// </summary>
    public class OutputStackableStore : BaseStackableStore
    {
        protected override async UniTaskVoid OnInventoryEnteredAsync()
        {
            while (EnteredInventory != null)
            {
                if (TryPop(out Stackable[] stackables) == true)
                {
                    foreach (Stackable stackable in stackables)
                    {
                        EnteredInventory.Push(stackable.StackablePoolType);
                    }
                }

                await UniTask.WaitForSeconds(speed);
            }
        }
    }
}
