using _KMH_Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class UI_UpgradableHandler : MonoSingleton<UI_UpgradableHandler>
    {
        public GameObject UpgradablePanel;

        private void Awake()
        {
            UpgradablePanel.SetActive(false);
        }
    }
}
