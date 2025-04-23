using _KMH_Framework;
using _KMH_Framework.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MGFramework
{
    public class HireableFootstep : PurchasableFootstep
    {
        [Header("=== HireableFootstep ===")]
        [SerializeField]
        private PoolType hireablePoolType;

        private void OnEnable()
        {
            priceText.text = price.ToString();
            purchasedPrice = 0;
        }

        protected override async UniTaskVoid OnInventoryEnteredAsync(CancellationToken token)
        {
            if (HasPurchased == false)
            {
                GoldManagement goldManager = GameManager.Instance.GoldManager;

                while (IsOccupied == true)
                {
                    await UniTask.Yield(cancellationToken: token);

                    if (goldManager.TryRemove(goldPerTick) == true)
                    {
                        purchasedPrice += goldPerTick;
                        float purchasedNormal = (float)purchasedPrice / price;

                        progressImage.fillAmount = purchasedNormal;
                        priceText.text = (price - purchasedPrice).ToString();

                        if (purchasedPrice == price)
                        {
                            OnPurchased();
                            break;
                        }
                    }
                }
            }
        }

        private void OnPurchased()
        {
            purchasedPrice = 0;
            priceText.text = price.ToString();
            progressImage.fillAmount = 0;

            onPurchasedEvent?.Invoke();

            hireablePoolType.EnablePool(OnBeforeEnablePool);
            void OnBeforeEnablePool(GameObject obj)
            {
                obj.transform.position = this.transform.position;
                obj.transform.rotation = this.transform.rotation;
            }
        }
    }
}
