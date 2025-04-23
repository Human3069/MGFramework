using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MGFramework
{
    public class PurchasableFootstep : BaseFootstep
    {
        [Header("=== PurchasableFootstep ===")]
        [SerializeField]
        protected TextMeshProUGUI priceText;

        [SerializeField]
        protected int price = 0;
        [SerializeField]
        protected int goldPerTick = 1;

        [Space(10)]
        [ReadOnly]
        [SerializeField]
        protected int purchasedPrice = 0;

        [SerializeField]
        protected UnityEvent onPurchasedEvent;

        [SerializeField]
        private bool _hasPurchased = false;
        public bool HasPurchased
        {
            get
            {
                return _hasPurchased;
            }
            private set
            {
                _hasPurchased = value;
            }
        }

        private void OnEnable()
        {
            if (HasPurchased == true)
            {
                onPurchasedEvent?.Invoke();
                this.gameObject.SetActive(false);
            }

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
                            HasPurchased = true;
                            onPurchasedEvent?.Invoke();
                            this.gameObject.SetActive(false);

                            break;
                        }
                    }
                }
            }
        }
    }
}
