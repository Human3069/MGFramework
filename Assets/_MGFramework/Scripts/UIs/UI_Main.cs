using _KMH_Framework;
using System;
using TMPro;
using UnityEngine;

namespace MGFramework
{
    public class UI_Main : MonoSingleton<UI_Main>
    {
        public Transform HealthbarParent;
        public Transform TimerParent;

        [Header("Toolbar")]
        [SerializeField]
        private TextMeshProUGUI goldText;

        private void Awake()
        {
            goldText.text = GameManager.Instance.Gold.ToString();
            GameManager.Instance.OnGoldChanged += OnGoldChanged;
        }

        private void OnGoldChanged(int gold)
        {
            goldText.text = gold.ToString();
        }
    }
}
