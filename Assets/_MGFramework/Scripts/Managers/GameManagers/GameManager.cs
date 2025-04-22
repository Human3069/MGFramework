using _KMH_Framework;
using UnityEngine;

namespace MGFramework
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public CustomerManagement CustomerManager;
        public BearManagement BearManager;
        public GoldManagement GoldManager;

        private void Awake()
        {
            CustomerManager.OnAwake(this);
            BearManager.OnAwake(this);
            GoldManager.OnAwake();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            CustomerManager.DrawGizmosSelected();
            BearManager.DrawGizmosSelected();
        }
#endif
    }
}