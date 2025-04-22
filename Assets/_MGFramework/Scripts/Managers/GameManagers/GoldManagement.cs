using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    [System.Serializable]
    public class GoldManagement 
    {
        [ReadOnly]
        [SerializeField]
        private int _gold = 0;
        public int Gold
        {
            get
            {
                return _gold;
            }
            private set
            {
                if (_gold != value)
                {
                    _gold = value;
                    OnGoldChanged?.Invoke(_gold);
                }
            }
        }

        public delegate void GoldChangedDelegate(int gold);
        public event GoldChangedDelegate OnGoldChanged;

        public void OnAwake()
        {
            Gold = 0;
        }

        public void Add(int amount)
        {
            Gold += amount;
        }

        public bool TryRemove(int amount)
        {
            if (Gold - amount >= 0)
            {
                Remove(amount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Remove(int amount)
        {
            Gold -= amount;
        }
    }
}
