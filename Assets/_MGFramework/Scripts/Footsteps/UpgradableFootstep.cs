using Cysharp.Threading.Tasks;
using System.Threading;

namespace MGFramework
{
    public class UpgradableFootstep : BaseFootstep
    {
        protected override async UniTaskVoid OnInventoryEnteredAsync(CancellationToken token)
        {
            if (EnteredInventory.transform == Player.Instance.transform)
            {
                UI_UpgradableHandler.Instance.UpgradablePanel.SetActive(true);
            }

            await UniTask.Yield();
        }
    }
}
