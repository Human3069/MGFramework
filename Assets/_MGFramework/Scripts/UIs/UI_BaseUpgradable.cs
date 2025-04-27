using _KMH_Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MGFramework
{
    public abstract class UI_BaseUpgradable<T, U> : MonoBehaviour where T : BaseUpgradeHandler<U>
                                                                  where U : IExcelRow, new()
    {
        [SerializeField]
        private Button healthButton;
        [SerializeField]
        private Button moveSpeedButton;
        [SerializeField]
        private Button attackRateButton;
        [SerializeField]
        private Button attackDamageButton;
     
        private TextMeshProUGUI healthGoldText;
        private TextMeshProUGUI moveSpeedGoldText;
        private TextMeshProUGUI attackRateGoldText;
        private TextMeshProUGUI attackDamageGoldText;

        private T upgrader;

        private void Start()
        {
            healthGoldText = healthButton.GetComponentInChildren<TextMeshProUGUI>();
            moveSpeedGoldText = moveSpeedButton.GetComponentInChildren<TextMeshProUGUI>();
            attackRateGoldText = attackRateButton.GetComponentInChildren<TextMeshProUGUI>();
            attackDamageGoldText = attackDamageButton.GetComponentInChildren<TextMeshProUGUI>();

            upgrader = UpgradeManager.Instance.GetUpgrader<T, U>();
            upgrader.OnUpgradeCostChanged += OnUpgradeCostChanged;

            int originHealthCost = upgrader.GetOriginCost(UpgradeType.Health);
            int originMoveSpeedCost = upgrader.GetOriginCost(UpgradeType.MoveSpeed);
            int originAttackRateCost = upgrader.GetOriginCost(UpgradeType.AttackSpeed);
            int originAttackDamageCost = upgrader.GetOriginCost(UpgradeType.AttackDamage);

            healthGoldText.text = originHealthCost.ToString("F0");
            moveSpeedGoldText.text = originMoveSpeedCost.ToString("F0");
            attackRateGoldText.text = originAttackRateCost.ToString("F0");
            attackDamageGoldText.text = originAttackDamageCost.ToString("F0");

            healthButton.onClick.AddListener(OnClickHealthButton);
            void OnClickHealthButton()
            {
                upgrader.Upgrade(UpgradeType.Health);
            }

            moveSpeedButton.onClick.AddListener(OnClickMovementSpeedButton);
            void OnClickMovementSpeedButton()
            {
                upgrader.Upgrade(UpgradeType.MoveSpeed);
            }

            attackRateButton.onClick.AddListener(OnClickAttackRateButton);
            void OnClickAttackRateButton()
            {
                upgrader.Upgrade(UpgradeType.AttackSpeed);
            }

            attackDamageButton.onClick.AddListener(OnClickAttackDamageButton);
            void OnClickAttackDamageButton()
            {
                upgrader.Upgrade(UpgradeType.AttackDamage);
            }
        }

        private TextMeshProUGUI GetTmpText(UpgradeType type)
        {
            TextMeshProUGUI tmpText;
            if (type == UpgradeType.Health)
            {
                tmpText = healthGoldText;
            }
            else if (type == UpgradeType.MoveSpeed)
            {
                tmpText = moveSpeedGoldText;
            }
            else if (type == UpgradeType.AttackSpeed)
            {
                tmpText = attackRateGoldText;
            }
            else if (type == UpgradeType.AttackDamage)
            {
                tmpText = attackDamageGoldText;
            }
            else
            {
                throw new System.NotImplementedException("type : " + type);
            }

            return tmpText;
        }

        private void OnUpgradeCostChanged(UpgradeType type, int costValue)
        {
            TextMeshProUGUI tmpText = GetTmpText(type);
            tmpText.text = costValue.ToString("F0");
        }
    }
}
